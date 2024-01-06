using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public FieldType currentField;
	public StateMachine<State, Trigger> stateMachine;
	SaveProgressManager saveManager;

	[SerializeField] private AudioClip buttonSfx;
	[SerializeField] private AudioClip gameOver;
	[SerializeField] private RectTransform OnFieldHolders;
	[SerializeField] private TextAsset tutorial;

	public static Action InTutorial;
	public static Action InContinueForAds;
	public static Action focusAtBonus;

	public string key;
	public int countToClear;
	public int countToSpawn;

	void Awake()
	{
		instance = this;

		stateMachine = new StateMachine<State, Trigger>(State.MAIN_MENU);

		stateMachine.Configure(State.MAIN_MENU)
			.AddTransition(Trigger.PlayButtonPressed, State.PLAY)
			.AddTransition(Trigger.Tutorial, State.TUTORIAL)
			.AddTransition(Trigger.Shop, State.SHOP)
			.onEnter += OnEnter;

		stateMachine.Configure(State.TUTORIAL)
			.AddTransition(Trigger.PlayButtonPressed, State.PLAY);

		stateMachine.Configure(State.PLAY)
			.AddTransition(Trigger.InGame, State.IN_GAME)
			.onEnter += StartGame;

		stateMachine.Configure(State.IN_GAME)
			.AddTransition(Trigger.PauseButtonPressed, State.PAUSED)
			.AddTransition(Trigger.Lose, State.GAME_OVER)
			.AddTransition(Trigger.Shop, State.SHOP);

		stateMachine.Configure(State.PAUSED)
			.AddTransition(Trigger.GoHome, State.MAIN_MENU)
			.AddTransition(Trigger.Resume, State.IN_GAME)
			.AddTransition(Trigger.Reset, State.PLAY);

		stateMachine.Configure(State.GAME_OVER)
			.AddTransition(Trigger.GoHome, State.MAIN_MENU)
			.AddTransition(Trigger.Restart, State.PLAY)
			.AddTransition(Trigger.InGame, State.IN_GAME)
			.AddTransition(Trigger.Shop, State.SHOP)
			.onExit += MusicPlay;

		stateMachine.Configure(State.SHOP)
			.AddTransition(Trigger.InGame, State.IN_GAME)
			.AddTransition(Trigger.BackToMainMenu, State.MAIN_MENU)
			.AddTransition(Trigger.Lose, State.GAME_OVER);
	}

	void Start()
	{
		saveManager = new SaveProgressManager();
		saveManager.SaveTutorial(tutorial.ToString(), GameConfig.tutorial);
		if (GameData.tutorialProgress == 0)
		{
			saveManager.Decode(GameConfig.tutorial);
			GameData.coins = 50;
		}
		else
		{
			if (System.IO.File.Exists(GameConfig.path))
				saveManager.Decode(GameConfig.path);
		}

//		ShowAdMobBanner ();
	}

	public void BackToMainMenu()
	{
		stateMachine.SetTrigger(Trigger.BackToMainMenu);
	}

	public void SetUpField(FieldType current)
	{
		currentField = current;
		OnFieldHolders.localScale = currentField.scale;
		key = currentField.shape.ToString();

		if (GameData.tutorialProgress == 0)
		{
			Debug.Log(stateMachine.CurrentState.ToString());
			stateMachine.SetTrigger(Trigger.Tutorial);
			Tutorial();
		}
		else
			Play();
	}

	public void Play()
	{
		stateMachine.SetTrigger(Trigger.PlayButtonPressed);
	}

	void StartGame()
	{
		stateMachine.SetTrigger(Trigger.InGame);
		if (GameData.tutorialProgress == 0)
		{
			GameController.instance.SpawnCells();
			GameData.tutorialProgress++;
		}
		else
		{
			if (!GameData.gameProperties.ContainsKey(key))
			{
				GameData.gameProperties.Add(key, new SaveProgress());
				GameData.TopScore = 0;
				GameController.instance.InNewGame();
			}
			else
			{
				Debug.Log("InGameContinue");
				GameController.instance.InGameContinue(key);
			}
		}
	}

	void Tutorial()
	{
		GameController.instance.InGameContinue(key);
		GameController.instance.Tutorial(1, 1, 1, 4);
	}

	public void Pause()
	{
		stateMachine.SetTrigger(Trigger.PauseButtonPressed);
	}

	public void GoHome()
	{
		GameController.instance.SaveGameProgress(key);
		SaveGameProgress();

		if (stateMachine.CurrentState == State.GAME_OVER)
		{
			GameData.ResetOnStart();
			GameController.instance.ResetCells();
		}

		// Save game progress

		GameController.instance.Exit();

		stateMachine.SetTrigger(Trigger.GoHome);
	}

	public void GameOver()
	{
		SoundManager.Instance.StopMusic();
		GameController.instance.SaveGameProgress(key);
		SoundManager.Instance.PlaySfx(gameOver, 1f);
		Invoke("Lose", 1.6f);
	}

	void Lose()
	{
		stateMachine.SetTrigger(Trigger.Lose);
	}

	public void Restart()
	{
		GameData.ResetOnStart();
		GameController.instance.ResetCells();
		//ShowAdMobIntersistal ();
		stateMachine.SetTrigger(Trigger.Restart);
	}

	public void Reset()
	{
		ResetCommand reset = new ResetCommand();
		reset.Execute();
		GameData.ResetOnStart();
		stateMachine.SetTrigger(Trigger.Reset);
	}

	public void Resume()
	{
		stateMachine.SetTrigger(Trigger.Resume);
	}

	public void Shop()
	{
		stateMachine.SetTrigger(Trigger.Shop);
	}

	public void HideShop()
	{
		stateMachine.SetTrigger(Trigger.BackToMainMenu);
	}

	public void HideShopOnGameOver()
	{
		stateMachine.SetTrigger(Trigger.Lose);
	}

	public void HideShopInGame()
	{
		if (GameController.instance.CheckIfGameOver())
			stateMachine.SetTrigger(Trigger.Lose);
		else
			stateMachine.SetTrigger(Trigger.InGame);
	}

	void OnEnter()
	{
		Debug.Log(stateMachine.CurrentState.ToString());
	}

	void MusicPlay()
	{
		SoundManager.PlayMusic(SoundManager.Instance.music, 1);
	}

	public void ButtonsSfx()
	{
		SoundManager.Instance.PlaySfx(buttonSfx, 1);
	}

	public void SecondChance()
	{
		if (stateMachine.CurrentState == State.GAME_OVER)
			stateMachine.SetTrigger(Trigger.InGame);

		GameController.instance.SecondChance();
		GameData.continueCounter++;
	}

	void SaveGameProgress()
	{
		if (stateMachine.CurrentState != State.MAIN_MENU &&
		    stateMachine.CurrentState != State.TUTORIAL)
		{
			if (!string.IsNullOrEmpty(key))
				GameController.instance.SaveGameProgress(key);
		}

		saveManager.Save();
	}

	void OnApplicationFocus(bool focusStatus)
	{
		if (!focusStatus)
			SaveGameProgress();
	}

	bool _pausedByAd = false;

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
			SaveGameProgress();
		else
			_pausedByAd = !_pausedByAd;
	}

	void OnApplicationQuit()
	{
		saveManager.Save();
		Debug.Log("Progress saved to " + GameConfig.path);
	}
}

public enum State
{
	MAIN_MENU,
	IN_GAME,
	PLAY,
	PAUSED,
	GAME_OVER,
	SHOP,
	TUTORIAL
}

public enum Trigger
{
	PlayButtonPressed,
	BackToMainMenu,
	PauseButtonPressed,
	InGame,
	Resume,
	GoHome,
	Lose,
	Reset,
	Restart,
	Shop,
	Tutorial
}