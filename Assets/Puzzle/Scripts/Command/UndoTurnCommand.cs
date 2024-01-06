using System.Collections.Generic;

public class UndoTurnCommand : Command
{
	public int[,] currentCellsValues;
	public int[] nextCellsValues;

	public UndoTurnCommand(HexagonController[,] cellsField, Queue<HexagonController> nextField)
	{
		int width = cellsField.GetLength(0);
		int height = cellsField.GetLength(1);

		currentCellsValues = new int[width, height];
		nextCellsValues = new int[nextField.Count];

		// remember cells values for current turn
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (cellsField[i, j] != null)
					currentCellsValues[i, j] = cellsField[i, j].CellValue;
				else
					currentCellsValues[i, j] = 0;
			}
		}

		// remember next cells values for current turn
		HexagonController[] nextF = nextField.ToArray();
		
		for (int q = 0; q < nextF.Length; q++)
			nextCellsValues[q] = nextF[q].CellValue;
	}

	public override void Execute()
	{
		GameController.instance.UndoField(currentCellsValues);
		GameController.instance.UndoNextField(nextCellsValues);
	}
}
