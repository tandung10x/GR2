using System.Collections.Generic;
using UnityEngine;

public static class GameConfig
{

	public static int maxWidth = 9;
	public static int maxHeight = 12;

	public const int undoTurnStartPrice = 50;
	public const int clearCellsStartPrice = 100;
	public const int openBigSizeModePrice = 3500;
	public const int openXTraModePrice = 5000;
	public const int openNewFieldPrice = 1000;

	public const int coinsWatchReward = 200;

	public const int undoTurnDeltaPrice = 50;
	public const int clearCellsDeltaPrice = 100;

	public const int continueForCoinsPrice = 500;

	public const int continueForAdsCount = 2;
	public const int showBonusTipAtGame = 2;
	public const int showBonusTipAfter_turnsCount = 30;

	public static string path = Application.persistentDataPath + "/" + "jsonData" + ".txt";
	public static string tutorial = Application.persistentDataPath + "/" + "tutorial" + ".txt";

	public const int coinsWatchAdReward = 50;

	public const string shopCoinsProductID_1 = "remove_ads";

	public const string shopCoinsProductID_2 = "coins_pack_1";

	public const string shopCoinsProductID_3 = "coins_pack_2";

	public const string shopCoinsProductID_4 = "coins_pack_3";

	public const string shopCoinsProductID_5 = "coins_pack_4";

	public static Dictionary<string, int> shopProducts = new Dictionary<string, int>()
	{
		{shopCoinsProductID_1, 0},
		{shopCoinsProductID_2, 800},
		{shopCoinsProductID_3, 1300},
		{shopCoinsProductID_4, 2200},
		{shopCoinsProductID_5, 3500}
	};
}
