using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public static class GameData
{

	public delegate void OnDataChange();

	public static OnDataChange OnCoinsChange;
	public static OnDataChange OnCoinsAdded;
	public static OnDataChange OnCoinsUp;
	public static OnDataChange OnCoinsNotEnough;
	public static OnDataChange onCombo;
	public static Action<int> onMaxValueChanged;
	public static UnityEvent onScoresChanged = new UnityEvent();
	public static UnityEvent onNewFieldBuying = new UnityEvent();
	public static UnityEvent onSuccesfulBuying = new UnityEvent();

	public static Dictionary<string, SaveProgress> gameProperties = new Dictionary<string, SaveProgress>();

	const string coinsKey = "coins";
	const string coinsInGameKey = "coinsInGame";
	const string isBestSroreKey = "isBestSrore";
	const string topScoreKey = "topScore";
	const string tutorialProgressKey = "tutorialProgress";
	const string undoBonusLevelKey = "undoBonusLevel";
	const string hummerBonusLevelKey = "hummerBonusLevel";
	const string bigSizeOpenKey = "bigSizeOpen";
	const string extraOpenKey = "extraOpen";
	const string continueCounterKey = "continueCounter";
	const string bonusTipShowedKey = "bonusTipShowed";
	const string bonusTipShowAfterUseKey = "bonusTipShowAfterUse";

	public static int tutorialProgress
	{
		get { return PlayerPrefs.GetInt(tutorialProgressKey); }
		set { PlayerPrefs.SetInt(tutorialProgressKey, value); }
	}

	public static int continueCounter
	{
		get { return PlayerPrefs.GetInt(continueCounterKey); }
		set { PlayerPrefs.SetInt(continueCounterKey, value); }
	}

	public static int bonusTipShowed
	{
		get { return PlayerPrefs.GetInt(bonusTipShowedKey); }
		set
		{
			PlayerPrefs.SetInt(bonusTipShowedKey, value);
			if (value != 0)
			{
				if (GameManager.focusAtBonus != null)
					GameManager.focusAtBonus.Invoke();
			}
		}
	}

	public static int bonusTipShowAfterUse
	{
		get { return PlayerPrefs.GetInt(bonusTipShowAfterUseKey); }
		set
		{
			PlayerPrefs.SetInt(bonusTipShowAfterUseKey, value);
			if (value == GameConfig.showBonusTipAfter_turnsCount)
				bonusTipShowed++;
		}
	}

	public static int coins
	{
		get { return PlayerPrefs.GetInt(coinsKey, 0); }
		set
		{
			if (value < 0)
			{
				if (OnCoinsNotEnough != null)
					OnCoinsNotEnough.Invoke();
			}
			else
			{
				bool added = value > coins;

				PlayerPrefs.SetInt(coinsKey, value);

				if (OnCoinsChange != null)
					OnCoinsChange.Invoke();

				if (added && OnCoinsAdded != null)
					OnCoinsAdded.Invoke();
			}
		}
	}

	public static int IsBestScore
	{
		get { return PlayerPrefs.GetInt(isBestSroreKey, 0); }
		set { PlayerPrefs.SetInt(isBestSroreKey, value); }
	}

	static int score;
	public static int Score
	{
		get { return score; }
		set
		{
			score = value;
			if (value > TopScore)
			{
				TopScore = value;
				IsBestScore++;
			}

			onScoresChanged.Invoke();
		}
	}

	static int topScore;
	public static int TopScore
	{
		get { return topScore; }
		set { topScore = value; }
	}

	static int maxCellValue;
	public static int MaxCellValue
	{
		get { return maxCellValue; }
		set
		{
			maxCellValue = value;
			if (onMaxValueChanged != null)
				onMaxValueChanged.Invoke(value);
		}
	}

	static int coinsUp;
	public static int CoinsUp
	{
		get { return coinsUp; }
		set
		{
			coinsUp = value;
			if (OnCoinsUp != null)
				OnCoinsUp.Invoke();
		}
	}

	static int comboCounter;
	public static int ComboCounter
	{
		get { return comboCounter; }
		set
		{
			comboCounter = value;
			if (value <= 0) return;
			if (onCombo == null) return;
			onCombo.Invoke();
			comboCounter = 0;
		}
	}

	public static int UndoBonusLevel
	{
		get { return PlayerPrefs.GetInt(undoBonusLevelKey, 0); }
		set { PlayerPrefs.SetInt(undoBonusLevelKey, value); }
	}

	public static int HummerBonusLevel
	{
		get { return PlayerPrefs.GetInt(hummerBonusLevelKey, 0); }
		set { PlayerPrefs.SetInt(hummerBonusLevelKey, value); }
	}


	public static int UndoPrice
	{
		get { return UndoBonusLevel * GameConfig.undoTurnDeltaPrice + GameConfig.undoTurnStartPrice; }
	}

	public static int HummerPrice
	{
		get { return HummerBonusLevel * GameConfig.clearCellsDeltaPrice + GameConfig.clearCellsStartPrice; }
	}

	public static void ResetOnStart()
	{
		continueCounter = 0;
		bonusTipShowed = 0;
		bonusTipShowAfterUse = 0;
		score = 0;
		MaxCellValue = 2;
		comboCounter = 0;
		HummerBonusLevel = 0;
		UndoBonusLevel = 0;
		IsBestScore = 0;
	}

	public static void ResetDataPropperties(string key)
	{
		gameProperties[key].cells.Clear();
		gameProperties[key].nextCells.Clear();
		gameProperties[key].score = 0;
		gameProperties[key].topScore = TopScore;
		gameProperties[key].undoLevel = 0;
		gameProperties[key].hummerLevel = 0;
	}
}
