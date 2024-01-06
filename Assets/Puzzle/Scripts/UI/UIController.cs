using UnityEngine;

public class UIController : MonoBehaviour {

	public CanvasGroup gameCanvas;

	public GameObject mainScreen;
	public GameObject buttonsGrid;
	public GameObject inGameUI;
	public GameObject inGame;
	public GameObject inGameButtons;
	public GameObject gameOverScreen;
	public GameObject pauseMenu;
	public GameObject settingsMenu;
	public GameObject hammerBonus;
	public GameObject undoBonus;
	public GameObject tutorial_menu;
	public GameObject help;
	public GameObject bonusTip;
	public GameObject lockedButton;
	public GameObject openNewField;

	public GameObject comboText;
	public GameObject coinsUpText;
	public GameObject blocked;
	public GameObject lockImage;
	public GameObject shop;
	public GameObject tutorial_1;
	public GameObject tutorial_2;
	public GameObject continueForAds;
	public GameObject adsButton;
	public GameObject coinsButton;
	public GameObject quitRequest;
	public GameObject[] lockedInTutorial;

	public Transform buttonsGridPositionOffline;
	public Transform buttonsGridPositionOnline;

	public UITweenRectPosition tweenPos;

	bool settingsActive;
	bool open;
	bool isInteractable;
	bool undo_open;
	bool hammer_open;
	bool tutorialMenu_open;
	bool help_open;
	bool open_popUp;

	GameObject [] coinsUp = new GameObject[4];
	StateMachine<State, Trigger> stateMachine;


	void Start()
	{
		stateMachine = GameManager.instance.stateMachine;

		stateMachine.Configure(State.MAIN_MENU)
			.onEnter += ShowMainScreen;

		stateMachine.Configure(State.TUTORIAL)
			.onEnter += InTutorial;

		stateMachine.Configure(State.IN_GAME)
			.onEnter += InGame;

		stateMachine.Configure(State.PLAY)
			.onEnter += InGame;

		stateMachine.Configure(State.GAME_OVER)
			.onEnter += GameOver;

		stateMachine.Configure(State.GAME_OVER)
			.onExit += HideGameOver;

		buttonsGrid.transform.position = buttonsGridPositionOnline.position;

		GameData.OnCoinsUp += CoinsUp;
		GameData.onCombo += Combo;
		GameData.OnCoinsNotEnough += ShowShop;
		GameData.onNewFieldBuying.AddListener(ShowOpenNewField);
		GameData.onSuccesfulBuying.AddListener(Hide);
		GameManager.InTutorial += TutorialOff;
		GameManager.InContinueForAds += ShowContinueForAdsWindow;
		GameManager.focusAtBonus += AttractToBonus;

		for (int i = 0; i < coinsUp.Length - 1; i++)
		{
			GameObject coins = Instantiate(coinsUpText, Vector2.zero, Quaternion.identity);
			coins.transform.SetParent(inGameUI.transform, false);
			coins.SetActive(false);
			coinsUp[i] = coins;
		}

		ShowMainScreen();
	}

	public void OnStartScreen()
	{
		GameManager.instance.BackToMainMenu();
	}

	void ShowMainScreen()
	{
		mainScreen.SetActive(true);
		buttonsGrid.SetActive(true);

		tutorial_1.SetActive(false);
		tutorial_2.SetActive(false);
		gameOverScreen.SetActive(false);
		lockImage.SetActive(false);
		comboText.SetActive(false);
		inGameUI.SetActive(false);
		inGame.SetActive(false);

		isInteractable = GameData.tutorialProgress == 0;
		foreach (GameObject obj in lockedInTutorial)
			obj.SetActive(isInteractable);
	}

	void InGame()
	{
		mainScreen.SetActive(false);
		gameCanvas.gameObject.SetActive(true);
		inGame.SetActive(true);
		buttonsGrid.SetActive(false);
		gameOverScreen.SetActive(false);
		inGameUI.SetActive(true);
		inGameButtons.SetActive(true);
		tutorial_1.SetActive(false);
		tutorial_2.SetActive(false);
		comboText.SetActive(false);
	}

	void InTutorial()
	{
		Debug.Log("In tutorial UI");
		mainScreen.SetActive(false);
		gameCanvas.gameObject.SetActive(true);
		inGame.SetActive(true);
		buttonsGrid.SetActive(false);
		gameOverScreen.SetActive(false);
		inGameUI.SetActive(true);
		inGameButtons.SetActive(false);
		tutorial_1.SetActive(true);
	}

	void TutorialOff()
	{
		tutorial_1.SetActive(false);
		tutorial_2.SetActive(true);
	}

	public void Pause()
	{
		gameCanvas.blocksRaycasts = false;
		lockImage.SetActive(true);
		
		if (stateMachine.CurrentState != State.PAUSED)
			GameManager.instance.Pause();

		tweenPos.autoClearCallbacks = false;
		tweenPos.OnFinished = null;
		tweenPos.OnFinished += StopPausePosAnim;
		tweenPos.PlayForward();
		pauseMenu.SetActive(true);
	}

	public void HidePauseMenu()
	{
		gameCanvas.blocksRaycasts = true;
		tweenPos.autoClearCallbacks = false;
		tweenPos.OnFinished = null;
		tweenPos.OnFinished += StopPausePosAnim;
		tweenPos.PlayReverse();
	}

	void StopPausePosAnim()
	{
		tweenPos.OnFinished -= StopPausePosAnim;
		if (gameCanvas.blocksRaycasts)
			pauseMenu.SetActive(false);
	}

	public void ExitPauseMenu()
	{
		HidePauseMenu();
		lockImage.SetActive(false);
		GameManager.instance.Resume();
		Debug.Log(stateMachine.CurrentState.ToString());
	}

	public void ShowSettings()
	{
		open = true;
		settingsActive = true;
		gameCanvas.blocksRaycasts = false;
		lockImage.SetActive(true);
		pauseMenu.SetActive(false);
		tweenPos.autoClearCallbacks = false;
		tweenPos.OnFinished = null;
		tweenPos.OnFinished += StopSettingsPosAnim;
		tweenPos.PlayForward();
		tutorial_menu.SetActive(false);
		help.SetActive(false);
		settingsMenu.SetActive(true);
		Debug.Log("ShowSettings");
	}

	public void HideSettings()
	{
		open = false;
		settingsActive = false;
		tweenPos.autoClearCallbacks = false;
		tweenPos.OnFinished = null;
		tweenPos.OnFinished += StopSettingsPosAnim;
		tweenPos.PlayReverse();
	}

	void StopSettingsPosAnim()
	{
		tweenPos.OnFinished -= StopSettingsPosAnim;
		if (open) return;

		settingsMenu.SetActive(false);
		if (stateMachine.CurrentState == State.PAUSED)
		{
			gameCanvas.blocksRaycasts = true;
			GameManager.instance.Resume();
		}

		lockImage.SetActive(false);
		gameCanvas.blocksRaycasts = true;
	}

	public void ShowShop()
	{
		open = true;
		lockImage.SetActive(true);
		gameCanvas.blocksRaycasts = false;
		shop.SetActive(true);

		GameManager.instance.Shop();

		hammerBonus.SetActive(false);
		undoBonus.SetActive(false);

		if (openNewField)
			openNewField.SetActive(false);

		tweenPos.autoClearCallbacks = false;
		tweenPos.OnFinished = null;
		tweenPos.OnFinished += StopShopAnim;

		tweenPos.PlayForward();
	}

	public void HideShop()
	{
		open = false;
		gameCanvas.blocksRaycasts = true;
		tweenPos.autoClearCallbacks = false;
		tweenPos.OnFinished = null;
		tweenPos.OnFinished += StopShopAnim;
		tweenPos.PlayReverse();
	}

	void StopShopAnim()
	{
		tweenPos.OnFinished -= StopShopAnim;
		if (open) return;

		shop.SetActive(false);
		if (mainScreen.activeInHierarchy)
			GameManager.instance.HideShop();
		else if (inGameUI.activeInHierarchy)
			GameManager.instance.HideShopInGame();
		else if (gameOverScreen.activeInHierarchy)
			GameManager.instance.HideShopOnGameOver();

		if (hammer_open)
			ShowHammerBonus();
		else if (undo_open)
			ShowUndoBonus();
		else
			lockImage.SetActive(false);
	}

	public void Show(GameObject bonus)
	{
		hammerBonus.SetActive(false);
		undoBonus.SetActive(false);
		continueForAds.SetActive(false);
		quitRequest.SetActive(false);
		tutorial_menu.SetActive(false);
		help.SetActive(false);
		open_popUp = true;
		gameCanvas.blocksRaycasts = false;
		lockImage.SetActive(true);
		tweenPos.autoClearCallbacks = false;
		tweenPos.OnFinished = null;
		tweenPos.OnFinished += StopAnim;
		tweenPos.PlayForward();
		bonus.SetActive(true);
	}

	public void Hide()
	{
		open_popUp = false;

		lockImage.SetActive(open);
		if (stateMachine.CurrentState != State.SHOP)
		{
			gameCanvas.blocksRaycasts = true;

			tweenPos.autoClearCallbacks = false;
			tweenPos.OnFinished = null;
			tweenPos.OnFinished += StopAnim;
			tweenPos.PlayReverse();
		}
		else if (open && !open_popUp)
			lockImage.SetActive(true);
		else
			lockImage.SetActive(false);
	}

	void StopAnim()
	{
		tweenPos.OnFinished -= StopAnim;
		if (open_popUp) return;

		hammer_open = false;
		undo_open = false;

		hammerBonus.SetActive(false);
		undoBonus.SetActive(false);
		continueForAds.SetActive(false);
		quitRequest.SetActive(false);

		tutorial_menu.SetActive(false);
		help.SetActive(false);

		if (openNewField)
			openNewField.SetActive(false);

		if (open)
			ShowSettings();
		else
			lockImage.SetActive(false);
	}

	public void ShowHammerBonus () 
	{
		hammer_open = true;
		Show (hammerBonus);
	}

	public void ShowUndoBonus () 
	{
		if (GameController.instance.commands.Count <= 0) return;
		
		undo_open = true;
		Show (undoBonus);
	}

	public void ShowContinueForAdsWindow () 
	{
		adsButton.SetActive (false);
		coinsButton.SetActive (true);
		Show (continueForAds);
	}

	public void ShowTutorialMenu ()
	{
		Show (tutorial_menu);
		settingsMenu.SetActive (false);
	}

	public void ShowHelp () 
	{
		Show (help);
		settingsMenu.SetActive (false);
	}

	public void ShowOpenNewField ()
	{
		Show (openNewField);
	}

	public void AttractToBonus () 
	{
		if (continueForAds.activeInHierarchy) return;
		
		lockedButton.SetActive (true);
		bonusTip.SetActive (true);
	}

	public void ShowQuitDialog () 
	{
		Show (quitRequest);
	}

	void HideGameOver() 
	{
		gameOverScreen.SetActive (false);
		gameOverScreen.GetComponentInChildren<GameOverUI> ().Hide ();
	}

	public void HideContinueForAds () 
	{
		Hide ();
		GameManager.instance.stateMachine.SetTrigger (Trigger.Lose);
	}

	void CoinsUp () 
	{
		for (int i = 0; i < coinsUp.Length - 1; i++) 
		{
			if (coinsUp[i].activeSelf) continue;
			GameController.instance.CoinsUpAnim (coinsUp [i]);
			coinsUp [i].SetActive (true);
			break;
		}
	}

	void Combo () 
	{
		comboText.SetActive (true);
	}

	void PathBlocked () 
	{
		blocked.SetActive (true);
	}

	public void Reset () 
	{
		HidePauseMenu ();
		lockImage.SetActive (false);
		GameManager.instance.Reset ();
	}

	public void Restart () 
	{
		if (stateMachine.CurrentState == State.GAME_OVER)
			HideGameOver ();
		GameManager.instance.Restart ();
	}

	void GameOver ()
	{
		gameOverScreen.SetActive (true);
		lockedButton.SetActive (false);
		bonusTip.SetActive (false);
	}

	public void Home () 
	{
		if (stateMachine.CurrentState == State.PAUSED)
		{
			gameCanvas.gameObject.SetActive(false);
			HidePauseMenu ();
			lockImage.SetActive (false);
		}
		if (stateMachine.CurrentState == State.GAME_OVER) 
			HideGameOver ();
		
		inGameUI.SetActive (false);
		mainScreen.SetActive (true);
		GameManager.instance.GoHome ();
	}


	public void ContunueForCoins ()
	{
		GameData.OnCoinsChange += BuyContinue;
		GameData.OnCoinsNotEnough += CancelBuying;
		GameData.coins -= GameConfig.continueForCoinsPrice;
	}

	void BuyContinue ()
	{
		CancelBuying ();
		Hide ();
		GameManager.instance.SecondChance ();
	}

	void CancelBuying ()
	{
		GameData.OnCoinsChange -= BuyContinue;
		GameData.OnCoinsNotEnough -= CancelBuying;
	}

	public void QuitGame()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			if (open_popUp)
			{
				Hide();
			}
			else if (settingsActive)
			{
				HideSettings();
			}
			else if (bonusTip.activeInHierarchy)
			{
				bonusTip.SetActive(false);
				lockedButton.SetActive(false);
			}
			else
			{
				switch (stateMachine.CurrentState)
				{
					case State.GAME_OVER:
					case State.PAUSED:
						ExitPauseMenu();
						break;
					case State.SHOP:
						HideShop();
						break;
					case State.IN_GAME:
					case State.PLAY:
						if (hammer_open || undo_open)
						{
							Hide();
						}
						else
						{
							GameManager.instance.Pause();
						}
						Pause();
						break;
					case State.MAIN_MENU:
						if (open_popUp)
						{
							Hide();
						}
						else
						{
							ShowQuitDialog();
						}
						break;
				}
			}
		}
	}
}