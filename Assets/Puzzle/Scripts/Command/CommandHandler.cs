using UnityEngine;

public class CommandHandler : MonoBehaviour
{

	Command clearCell;
	Command resetGame;

	void Start()
	{
		clearCell = new ClearCellsCommand();
		resetGame = new ResetCommand();
	}

	public void Clear()
	{
		GameData.OnCoinsChange += BuyHammerBonus;
		GameData.OnCoinsNotEnough += CancelHammerBuying;
		GameData.coins -= GameData.HummerPrice;
	}

	void BuyHammerBonus()
	{

		Invoke("Hammer", 0.6f);
	}

	void Hammer()
	{
		clearCell.Execute();
		GameData.HummerBonusLevel++;
		CancelHammerBuying();
	}

	void CancelHammerBuying()
	{
		GameData.OnCoinsChange -= BuyHammerBonus;
		GameData.OnCoinsNotEnough -= CancelHammerBuying;
	}

	public void UndoTurn()
	{
		if (GameController.instance.commands.Count > 0)
		{
			GameData.OnCoinsChange += BuyUndoBonus;
			GameData.OnCoinsNotEnough += CancelUndoBuying;
			GameData.coins -= GameData.UndoPrice;
		}
	}

	void BuyUndoBonus()
	{
		GameController.instance.commands.Pop().Execute();
		GameData.UndoBonusLevel++;
		CancelUndoBuying();
	}

	void CancelUndoBuying()
	{
		GameData.OnCoinsChange -= BuyUndoBonus;
		GameData.OnCoinsNotEnough -= CancelUndoBuying;
	}

	public void Reset()
	{
		resetGame.Execute();
	}

	public void SaveProgress()
	{ }
}
