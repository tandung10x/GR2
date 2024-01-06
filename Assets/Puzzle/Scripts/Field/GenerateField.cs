using UnityEngine;

public class GenerateField
{

	FieldType currentField;

	int cellCount;

	int sizeW = 34;
	int sizeH = 30;

	int width;
	int height;

	public Vector3 CellPosition(int i, int j)
	{
		FieldType curField = GameManager.instance.currentField;
		Vector3 cellPos = curField.startPosition + new Vector3(i * sizeW, j * sizeH);
		if (j % 2 == 0)
		{
			cellPos.x += sizeW / 2;
		}

		return cellPos;
	}

	public Vector3 NextCellPosition(int i, int j)
	{
		FieldType curField = GameManager.instance.currentField;
		Vector3 nextCellPos = curField.nextCellStartPosition + new Vector3(i * sizeW / 1.3f, j * sizeH / 1.3f);
		return nextCellPos;
	}


	public ButtonController[,] SetUpField()
	{
		currentField = GameManager.instance.currentField;
		int[,] numbers = Parse();

		width = numbers.GetLength(0);
		height = numbers.GetLength(1);

		ButtonController[,] backField = new ButtonController[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (numbers[i, j] != 0)
				{
					GameObject obj = Pool.field.Dequeue();
					backField[i, j] = obj.GetComponent<ButtonController>();
					backField[i, j].i = i;
					backField[i, j].j = j;
					backField[i, j].gameObject.transform.localScale = Vector3.one;
					backField[i, j].gameObject.GetComponent<RectTransform>().anchoredPosition3D = CellPosition(i, j);
					backField[i, j].gameObject.SetActive(true);
					GameController.instance.fieldCells++;
				}
				else
				{
					backField[i, j] = null;
				}
			}
		}

		return backField;
	}

	public int[,] Parse()
	{
		char[] lineEndings = new char[] {'\n', '\r'};
		string[] lines = currentField.field.text.Split(lineEndings, System.StringSplitOptions.RemoveEmptyEntries);
		int[,] numbers = new int [lines.Length, lines[0].Length];
		for (int i = 0; i < lines.Length; i++)
		{
			for (int j = 0; j < lines[i].Length; j++)
			{
				numbers[i, j] = int.Parse(lines[i][j].ToString());
			}
		}

		return numbers;
	}
}
