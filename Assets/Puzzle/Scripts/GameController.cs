using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public class Coords
	{
		public int i, j;

		public Coords()
		{ }

		public Coords(int i, int j)
		{
			this.i = i;
			this.j = j;
		}
	}

	public static GameController instance;

	GenerateField generator = new GenerateField();

	public RectTransform nextFieldHolder;
	public RectTransform cellHolder;
	public CanvasGroup inGameButtons;
	public GameObject blockedCell;
	public GameObject blockedLine;
	public GameObject blockedImage;
	public GameObject tutorialCell;
	public GameObject lockedImage;
	public GameObject explosion;
	public GameObject explosion_simple;

	public Transform holder;

	public AudioClip clearCells;
	public AudioClip undoTurn;

	public List<int> values = new List<int>();
	public List<float> probabilities = new List<float>();

	public float startAfter = 0.3f;
	public float movingTime = 0.3f;
	public float combiningTime = 3;
	public float whileSpawning = 0.4f;
	public float blockingTime = 0.4f;
	public float scoreTime = 0.2f;
	public int identicCells = 4;
	public int maxValue = 7;

	[HideInInspector]
	public bool isMoving;
	[HideInInspector]
	public Stack<Command> commands = new Stack<Command>();
	
	public int fieldCells;

	Vector3 nextCellScale = new Vector3(0.5f, 0.5f, 0.5f);
	List<HexagonController> cellsToSpawn = new List<HexagonController>();
	List<HexagonController> sameCells = new List<HexagonController>();
	Queue<HexagonController> nextCells = new Queue<HexagonController>();
	HexagonController[,] cells;
	ButtonController[,] backField;

	[HideInInspector]
	public int[] availableValues;
	float[] availableProbabilities;

	float currentTime;
	bool isCombining;
	bool isSpawning;

	int width;
	int height;

	int currentCellCount;
	int cellsOnField;

	Color col;


	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		GameData.onMaxValueChanged += ChoiceMaxValue;
	}

	public bool CheckIfGameOver()
	{
		return cellsOnField >= fieldCells;
	}

	public void InGameContinue(string key)
	{
		fieldCells = 0;
		cellsOnField = 0;

		SetUpField();

		if (GameData.gameProperties[key].cells.Count != 0 && GameData.gameProperties[key].nextCells.Count != 0)
		{

			for (int q = 0; q < GameData.gameProperties[key].cells.Count; q++)
			{
				int i = GameData.gameProperties[key].cells[q].i;
				int j = GameData.gameProperties[key].cells[q].j;

				if (i >= cells.GetLength(0) || j >= cells.GetLength(1))
				{
#if UNITY_EDITOR
					Debug.LogError("Skip: " + i + "/" + cells.GetLength(0) + ", " + j + "/" + cells.GetLength(1));
#endif
				}
				else
				{
					cells[i, j] = PasteFromPool();
					cells[i, j].CellValue = GameData.gameProperties[key].cells[q].value;
					cells[i, j].gameObject.transform.SetParent(cellHolder, false);
					cells[i, j].gameObject.GetComponent<Button>().interactable = true;
					cells[i, j].gameObject.SetActive(true);

					RectTransform rect = cells[i, j].gameObject.transform.GetComponent<RectTransform>();
					rect.anchoredPosition3D = generator.CellPosition(i, j);
					cells[i, j].gameObject.transform.localScale = Vector3.one;
					TransparentBackCell(i, j, 0);
					cells[i, j].i = i;
					cells[i, j].j = j;

					cellsOnField++;
				}
			}

			for (int n = 0; n < GameData.gameProperties[key].nextCells.Count; n++)
			{
				HexagonController next = SetUpNextCell(n);
				next.CellValue = GameData.gameProperties[key].nextCells[n];
				nextCells.Enqueue(next);
			}
		}
		else
		{
			GameData.ResetOnStart();
			nextCells = GenerateNextField(availableValues, availableProbabilities);
			foreach (HexagonController nCell in nextCells)
				nCell.gameObject.SetActive(false);

			StartCoroutine(SpawningCells());
		}

		GameData.TopScore = GameData.gameProperties[key].topScore;
		GameData.Score = GameData.gameProperties[key].score;
		GameData.UndoBonusLevel = GameData.gameProperties[key].undoLevel;
		GameData.HummerBonusLevel = GameData.gameProperties[key].hummerLevel;

		if (cellsOnField == fieldCells)
			GameManager.instance.GameOver();
	}

	public void InNewGame()
	{
		GameData.ResetOnStart();

		fieldCells = 0;
		SetUpField();
		nextCells = GenerateNextField(availableValues, availableProbabilities);

		foreach (HexagonController nCell in nextCells)
			nCell.gameObject.SetActive(false);

		StartCoroutine(SpawningCells());
	}

	void SetUpField()
	{
		backField = generator.SetUpField();
		width = backField.GetLength(0);
		height = backField.GetLength(1);
		currentCellCount = GameManager.instance.countToSpawn;
		cells = new HexagonController[width, height];
	}

	public void Tutorial(int iFrom, int jFrom, int iTo, int jTo)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (backField[i, j] != null && backField[i, j] != backField[iTo, jTo])
					backField[i, j].GetComponent<Button>().interactable = false;
				if (cells[i, j] != null && cells[i, j] != cells[iFrom, jFrom])
					cells[i, j].GetComponent<Button>().interactable = false;
			}
		}

		tutorialCell.transform.position = backField[iTo, jTo].gameObject.transform.position;
		tutorialCell.SetActive(true);
	}

	public void SpawnCells()
	{
		StartCoroutine(SpawningCells());
	}

	IEnumerator SpawningCells()
	{
		isSpawning = true;
		inGameButtons.interactable = false;
		GameData.ComboCounter = 0;

		int count = nextCells.Count;

		while (count > 0)
		{
			int q = 0;
			while (nextCells.Count > 0)
			{
				cellsToSpawn.Add(nextCells.Dequeue());
				if (cellsToSpawn[q].gameObject.activeSelf)
					cellsToSpawn[q].gameObject.SetActive(false);

				q++;
			}

			int i = Random.Range(0, width);
			int j = Random.Range(0, height);
			if (backField[i, j] != null)
			{
				if (cells[i, j] == null)
				{

					cells[i, j] = cellsToSpawn[0];

					cells[i, j].gameObject.transform.SetParent(cellHolder, false);
					cells[i, j].gameObject.GetComponent<Button>().interactable = true;
					cells[i, j].gameObject.SetActive(true);

					RectTransform rect = cells[i, j].gameObject.transform.GetComponent<RectTransform>();
					rect.anchoredPosition3D = generator.CellPosition(i, j);
					TransparentBackCell(i, j, 0);
					cells[i, j].OnSpawningAnim();

					cellsToSpawn.RemoveAt(0);

					cells[i, j].i = i;
					cells[i, j].j = j;
					cellsOnField++;
					count--;

					yield return new WaitForSeconds(cells[i, j].onSpawnTweenScale.duration);
					if (CombineCells(cells[i, j].i, cells[i, j].j))
					{
						while (isCombining)
						{
							yield return null;
						}

						yield return new WaitForFixedUpdate();
					}

					if (cellsOnField >= fieldCells)
					{
						GameManager.instance.GameOver();
						break;
					}
				}
			}
		}

		if (fieldCells - cellsOnField <= currentCellCount * GameConfig.showBonusTipAtGame)
		{
			if (GameData.bonusTipShowed < GameConfig.showBonusTipAtGame - 1)
				GameData.bonusTipShowed++;
			else if (GameData.bonusTipShowAfterUse > 0 && GameData.bonusTipShowAfterUse < GameConfig.showBonusTipAtGame)
				GameData.bonusTipShowAfterUse++;
		}

		nextCells = GenerateNextField(availableValues, availableProbabilities);
		inGameButtons.interactable = true;
		isSpawning = false;
	}

	bool CombineCells(int i, int j)
	{
		StartCoroutine(CombiningCells(i, j));
		return isCombining;
	}

	IEnumerator CombiningCells(int i, int j)
	{
		int cellValue = cells[i, j].CellValue;

		sameCells.Clear();
		sameCells = IdenticCells(i, j, cellValue, new List<HexagonController>());
		List<Coords> targetCoords = new List<Coords>();
		targetCoords.Add(new Coords(i, j));

		if (sameCells.Count >= identicCells)
		{
			isCombining = true;
			cellsOnField -= sameCells.Count - 1;
			for (int q = 1; q < sameCells.Count; q++)
				TransparentBackCell(sameCells[q].i, sameCells[q].j, 255);

			yield return new WaitForEndOfFrame();

			CombiningAnim(sameCells[0].i, sameCells[0].j);
			GameData.CoinsUp = 1 + (sameCells.Count - identicCells);
			int newCoins = GameData.CoinsUp + GameData.coins;
			StartCoroutine
			(
				NumberAnim
				(
					GameData.coins,
					newCoins,
					scoreTime,
					(int coins) => GameData.coins = coins
				)
			);
		}
		else
			isCombining = false;
	}

	void CombiningAnim(int iCentral, int jCentral)
	{
		cells[iCentral, jCentral].mergeAnimation.OnMergeFinished += OnMergeAnimationFinished;
		cells[iCentral, jCentral].OnMergeSound();
		LinkCells(cells[iCentral, jCentral]);
		cells[iCentral, jCentral].GetComponent<CellMergeAnimation>().InitMerge();
	}

	List<HexagonController> GetNearCells(HexagonController current)
	{
		List<HexagonController> nearCells = new List<HexagonController>();

		int i = current.i;
		int j = current.j;

		Coords[] toCheck = new Coords[6];
		toCheck[0] = new Coords(i + 1, j); //+
		toCheck[1] = new Coords(i - 1, j); //+
		toCheck[2] = new Coords(i, j + 1); //+
		toCheck[3] = new Coords(i, j - 1); //+
		if (j % 2 == 0)
		{
			toCheck[4] = new Coords(i + 1, j + 1);
			toCheck[5] = new Coords(i + 1, j - 1);
		}
		else
		{
			toCheck[4] = new Coords(i - 1, j - 1);
			toCheck[5] = new Coords(i - 1, j + 1);
		}

		foreach (Coords coord in toCheck)
		{
			if (coord.i >= 0 && coord.j >= 0 && coord.i < width && coord.j < height &&
			    cells[coord.i, coord.j] != null &&
			    cells[coord.i, coord.j].CellValue == current.CellValue)
			{

				nearCells.Add(cells[coord.i, coord.j]);
			}
		}

		return nearCells;
	}

	void LinkCells(HexagonController current) //call with central cell
	{
		List<HexagonController> nearCells = GetNearCells(current);
		List<HexagonController> nearFreeCells = new List<HexagonController>();

		foreach (HexagonController nearCell in nearCells)
		{
			if (nearCell.mergeAnimation != current.mergeAnimation.mergeToCell &&
			    nearCell.mergeAnimation.mergeToCell == null)
			{
				current.mergeAnimation.prevCells.Add(nearCell.mergeAnimation);
				nearCell.mergeAnimation.mergeToCell = current.mergeAnimation;
				nearFreeCells.Add(nearCell);
			}
		}

		foreach (HexagonController nearCell in nearFreeCells)
			LinkCells(nearCell);
	}

	void OnMergeAnimationFinished()
	{
		for (int q = 1; q < sameCells.Count; q++)
		{
			cells[sameCells[q].i, sameCells[q].j] = null;
			PasteToPool(sameCells[q]);
		}

		int newScore = GameData.Score + sameCells[0].CellValue;

		if (sameCells[0].CellValue == maxValue)
		{

			newScore += sameCells[0].CellValue;
			TransparentBackCell(sameCells[0].i, sameCells[0].j, 255);
			cells[sameCells[0].i, sameCells[0].j] = null;
			cellsOnField--;

			GameObject explo = Instantiate(explosion, Vector2.zero, Quaternion.identity) as GameObject;
			explo.transform.SetParent(holder, false);
			explo.transform.position = sameCells[0].transform.position;
			explosion.SetActive(true);
			// some animation

			PasteToPool(sameCells[0]);
			isCombining = false;

		}
		else
		{
			sameCells[0].CellValue++;
			if (GameManager.instance.stateMachine.CurrentState == State.TUTORIAL)
				PassedTutorial();

			if (CombineCells(sameCells[0].i, sameCells[0].j))
				GameData.ComboCounter++;
		}

		StartCoroutine
		(
			NumberAnim
			(
				GameData.Score,
				newScore,
				scoreTime,
				(int score) => GameData.Score = score
			)
		);
	}

	public static IEnumerator ChangePos(int iFrom, int jFrom, int iTo, int jTo)
	{

		if (instance.isSpawning || instance.isCombining)
			yield break;

		List<Coords> freeWay = instance.FindShortcut(iFrom, jFrom, iTo, jTo);

		if (freeWay.Count > 0)
		{
			Command com = new UndoTurnCommand(instance.cells, instance.nextCells);
			instance.commands.Push(com);

			instance.TransparentBackCell(iFrom, jFrom, 255);
			Color backCol = instance.backField[iTo, jTo].GetComponent<Image>().color;
			instance.backField[iTo, jTo].GetComponent<Image>().color = instance.cells[iFrom, jFrom].cellImage.color;

			instance.StartCoroutine(instance.MovingCell(instance.cells[iFrom, jFrom].transform, freeWay,
				instance.cells[iFrom, jFrom]));
			instance.cells[iTo, jTo] = instance.cells[iFrom, jFrom];
			instance.cells[iFrom, jFrom] = null;
			HexagonController hc = instance.cells[freeWay[freeWay.Count - 1].i, freeWay[freeWay.Count - 1].j];
			hc.i = freeWay[freeWay.Count - 1].i;
			hc.j = freeWay[freeWay.Count - 1].j;
			while (instance.isMoving)
			{
				yield return null;
			}

			instance.backField[iTo, jTo].GetComponent<Image>().color = backCol;
			instance.TransparentBackCell(iTo, jTo, 0);
			if (!instance.CombineCells(hc.i, hc.j))
			{
				yield return new WaitForSeconds(0.1f);
				instance.SpawnCells();
			}

			yield return new WaitWhile(() => instance.isSpawning);
		}
		else
		{
			instance.StartCoroutine(instance.BlockedWay(instance.backField[iTo, jTo], instance.cells[iFrom, jFrom]));
			instance.cells[iFrom, jFrom].OnBlockedPathSound();
		}
	}

	IEnumerator MovingCell(Transform tran, List<Coords> way, HexagonController cell)
	{
		isMoving = true;
		cell.TransparentState(0, true);
		cell.skin.enabled = false;
		cell.OnMovementSound();
		for (int i = 0; i < way.Count; i++)
		{
			currentTime = 0;
			Vector3 pos = tran.GetComponent<RectTransform>().anchoredPosition3D;
			while (currentTime < movingTime)
			{
				currentTime += Time.deltaTime;
				float moving = currentTime / movingTime;
				Vector3 target = generator.CellPosition(way[i].i, way[i].j);
				tran.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(pos, target, moving);
				yield return null;
			}
		}

		isMoving = false;
		cell.skin.enabled = true;
		cell.TransparentState(255, false);
	}

	IEnumerator BlockedWay(ButtonController cellTo, HexagonController cellFrom)
	{
		blockedCell.SetActive(true);
		blockedImage.SetActive(true);
		blockedCell.transform.position = cellTo.gameObject.transform.position;
		DrawLockedLine(cellTo, cellFrom);
		currentTime = 0;
		while (currentTime < blockingTime)
		{
			currentTime += Time.deltaTime;
			yield return null;
		}

		blockedCell.SetActive(false);
		blockedLine.SetActive(false);
		blockedImage.SetActive(false);
	}

	void DrawLockedLine(ButtonController cellTo, HexagonController cellFrom)
	{
		blockedLine.SetActive(true);
		Vector3 positionTo = cellTo.GetComponent<RectTransform>().localPosition;
		Vector3 positionFrom = cellFrom.GetComponent<RectTransform>().localPosition;
		float distance = Vector2.Distance(positionFrom, positionTo);
		blockedLine.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, distance);
		blockedLine.transform.position = cellFrom.gameObject.transform.position;
		blockedLine.transform.LookAt(cellTo.transform, Vector3.back);
	}

	// Needed for cell TweenScale animation

	public void TransparentBackCell(int i, int j, int aState)
	{
		Color color = backField[i, j].GetComponent<Image>().color;
		color = new Color(color.r, color.g, color.b, aState);
		backField[i, j].GetComponent<Image>().color = color;
	}

	public List<HexagonController> IdenticCells(int i, int j, int value, List<HexagonController> used)
	{
		List<HexagonController> checkCells = new List<HexagonController>();
		if (i >= 0 && j >= 0 && i < width && j < height && cells[i, j] != null)
		{
			if (!used.Contains(cells[i, j]))
				used.Add(cells[i, j]);
			else
				return checkCells;

			if (cells[i, j].CellValue == value)
			{
				checkCells.Add(cells[i, j]);
				checkCells.AddRange(IdenticCells(i + 1, j, value, used));
				checkCells.AddRange(IdenticCells(i - 1, j, value, used));
				checkCells.AddRange(IdenticCells(i, j + 1, value, used));
				checkCells.AddRange(IdenticCells(i, j - 1, value, used));
				if (j % 2 == 0)
				{
					checkCells.AddRange(IdenticCells(i + 1, j + 1, value, used));
					checkCells.AddRange(IdenticCells(i + 1, j - 1, value, used));
				}
				else
				{
					checkCells.AddRange(IdenticCells(i - 1, j - 1, value, used));
					checkCells.AddRange(IdenticCells(i - 1, j + 1, value, used));
				}
			}
		}

		return checkCells;
	}

	List<Coords> FindShortcut(int iStart, int jStart, int iTarget, int jTarget)
	{
		Coords[,] m = new Coords[width, height];
		Queue<Coords> queue = new Queue<Coords>();
		queue.Enqueue(new Coords(iStart, jStart));
		while (queue.Count > 0)
		{
			Coords current = queue.Dequeue();
			int i = current.i;
			int j = current.j;
			if (cells[i, j] == null && backField[i, j] != null || (i == iStart && j == jStart))
			{
				Coords[] toCheck = new Coords[6];
				toCheck[0] = new Coords(i + 1, j); //+
				toCheck[1] = new Coords(i - 1, j); //+
				toCheck[2] = new Coords(i, j + 1); //+
				toCheck[3] = new Coords(i, j - 1); //+
				if (j % 2 == 0)
				{
					toCheck[4] = new Coords(i + 1, j + 1);
					toCheck[5] = new Coords(i + 1, j - 1);
				}
				else
				{
					toCheck[4] = new Coords(i - 1, j - 1);
					toCheck[5] = new Coords(i - 1, j + 1);
				}

				foreach (Coords coord in toCheck)
				{
					if (coord.i >= 0 && coord.j >= 0 && coord.i < width && coord.j < height && m[coord.i, coord.j] == null)
					{
						queue.Enqueue(coord);
						m[coord.i, coord.j] = current;
					}
				}
			}
		}

		List<Coords> way = new List<Coords>();
		if (m[iTarget, jTarget] != null)
		{
			Coords current = new Coords(iTarget, jTarget);
			while (current.i != iStart || current.j != jStart)
			{
				way.Add(current);
				current = m[current.i, current.j];
			}

			way.Reverse();
		}

		return way;
	}

	public Queue<HexagonController> GenerateNextField(int[] availableVal, float[] availableProb)
	{

		Queue<HexagonController> queue = new Queue<HexagonController>();
		int[] nextCellVal = new int[currentCellCount];

		nextCellVal = Probabilities.GetRandomValues(availableVal, availableProb, currentCellCount);

		for (int i = 0; i < currentCellCount; i++)
		{
			HexagonController obj = SetUpNextCell(i);
			obj.CellValue = nextCellVal[i];
			queue.Enqueue(obj);
		}

		return queue;
	}

	HexagonController SetUpNextCell(int xPos)
	{
		HexagonController obj = PasteFromPool();
		obj.gameObject.GetComponent<Button>().interactable = false;
		obj.gameObject.transform.SetParent(nextFieldHolder, false);
		obj.gameObject.transform.localScale = nextCellScale;
		obj.gameObject.GetComponent<RectTransform>().anchoredPosition3D = generator.NextCellPosition(xPos, 0);
		return obj;
	}

	HexagonController PasteFromPool()
	{
		GameObject obj = Pool.cells.Dequeue();
		var cell = obj.GetComponent<HexagonController>();
		cell.gameObject.SetActive(true);
		cell.gameObject.GetComponent<Button>().interactable = false;
		return cell;
	}

	public void PasteToPool(HexagonController haxCell)
	{
		GameObject cell = haxCell.gameObject;
		cell.SetActive(false);
		Pool.cells.Enqueue(cell);
	}

	// Crop values array due to the MAX available value in game
	// Updates when new MAX value is opened

	void ChoiceMaxValue(int val)
	{
		if (val < 0) return;
		availableValues = values.GetRange(0, val).ToArray();
		availableProbabilities = probabilities.GetRange(0, val).ToArray();
	}

	void PassedTutorial()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (backField[i, j] != null)
					backField[i, j].GetComponent<Button>().interactable = true;
				if (cells[i, j] != null)
					cells[i, j].GetComponent<Button>().interactable = true;
			}
		}

		tutorialCell.SetActive(false);
		GameManager.instance.Play();
	}

	// Bonus options in UNDO TURN command pattern

	public void UndoField(int[,] cellsValues)
	{
		SoundManager.Instance.PlaySfx(undoTurn, 1f);

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (cells[i, j] != null && cellsValues[i, j] != 0)
					cells[i, j].CellValue = cellsValues[i, j];
				else if (cells[i, j] != null && cellsValues[i, j] == 0)
				{
					PasteToPool(cells[i, j]);
					cells[i, j] = null;
					TransparentBackCell(i, j, 255);
					cellsOnField--;
				}
				else if (cells[i, j] == null && cellsValues[i, j] != 0)
				{
					cells[i, j] = PasteFromPool();
					cells[i, j].CellValue = cellsValues[i, j];
					cells[i, j].gameObject.GetComponent<Button>().interactable = true;
					cells[i, j].gameObject.transform.SetParent(cellHolder, false);
					cells[i, j].i = i;
					cells[i, j].j = j;
					RectTransform rect = cells[i, j].gameObject.transform.GetComponent<RectTransform>();
					rect.anchoredPosition3D = generator.CellPosition(i, j);
					rect.localScale = Vector3.one;
					cellsOnField++;
				}
			}
		}
	}

	public void UndoNextField(int[] cellValues)
	{
		while (nextCells.Count > 0)
			PasteToPool(nextCells.Dequeue());

		for (int i = 0; i < cellValues.Length; i++)
		{
			HexagonController obj = SetUpNextCell(i);
			obj.CellValue = cellValues[i];
			nextCells.Enqueue(obj);
		}
	}

	// Bonus option in CLEAR CELLS command pattern

	public void ClearCell(int count)
	{
		lockedImage.SetActive(true);
		int q = count;
		int maxValueToClear = values[values.Count - 1];
		SoundManager.Instance.PlaySfx(clearCells, 1f);
		Invoke("UnlockField", 0.2f);

		while (q > 0 && cellsOnField > 1)
		{
			int i = Random.Range(0, width);
			int j = Random.Range(0, height);
			if (cells[i, j] != null)
			{
				if (cells[i, j].CellValue <= maxValueToClear)
				{
					cells[i, j].OnClearCell();
					TransparentBackCell(i, j, 255);
					cells[i, j] = null;
					cellsOnField--;
					q--;
				}
				else
				{
					int minCellVal = cells[i, j].CellValue;
					foreach (HexagonController hexa in cells)
					{
						if (hexa != null && hexa.CellValue < minCellVal)
							minCellVal = hexa.CellValue;
					}

					if (minCellVal > maxValueToClear)
						maxValueToClear = minCellVal;
				}
			}
		}
	}

	void UnlockField()
	{
		lockedImage.SetActive(false);
	}

	public void SecondChance()
	{
		SoundManager.Instance.PlaySfx(clearCells, 1f);

		List<Coords> nearCells = new List<Coords>();
		int q = 0;
		while (q == 0)
		{
			int i = Random.Range(1, width - 1);
			int j = Random.Range(1, height - 1);

			if (cells[i, j] != null && backField[i, j] != null)
			{
				nearCells.Add(new Coords(i, j));
				Coords[] toCheck = new Coords[6];
				toCheck[0] = new Coords(i + 1, j); //+
				toCheck[1] = new Coords(i - 1, j); //+
				toCheck[2] = new Coords(i, j + 1); //+
				toCheck[3] = new Coords(i, j - 1); //+
				if (j % 2 == 0)
				{
					toCheck[4] = new Coords(i + 1, j + 1);
					toCheck[5] = new Coords(i + 1, j - 1);
				}
				else
				{
					toCheck[4] = new Coords(i - 1, j - 1);
					toCheck[5] = new Coords(i - 1, j + 1);
				}

				foreach (Coords coord in toCheck)
				{
					if (backField[coord.i, coord.j] != null)
					{
						nearCells.Add(coord);
					}
					else
					{
						nearCells.Clear();
						break;
					}
				}
			}

			if (nearCells.Count == 7)
				q++;
		}

		for (int n = 0; n < nearCells.Count; n++)
		{
			GameObject explo = Instantiate(n == 0 ? explosion : explosion_simple, Vector2.zero, Quaternion.identity);

			explo.transform.SetParent(holder, false);
			explo.transform.position = cells[nearCells[n].i, nearCells[n].j].transform.position;
			explosion.SetActive(true);

			if (cells[nearCells[n].i, nearCells[n].j] != null)
			{
				cells[nearCells[n].i, nearCells[n].j].OnClearCell();
				TransparentBackCell(nearCells[n].i, nearCells[n].j, 255);
				cells[nearCells[n].i, nearCells[n].j] = null;
				cellsOnField--;
			}
		}

		HexagonController[] next = nextCells.ToArray();
		for (int n = 0; n < next.Length; n++)
			PasteToPool(next[n]);

		if (cellsToSpawn.Count > 0)
		{
			for (int m = 0; m < cellsToSpawn.Count; m++)
				PasteToPool(cellsToSpawn[m]);
		}

		cellsToSpawn.Clear();
		nextCells.Clear();
		commands.Clear();
		nextCells = GenerateNextField(availableValues, availableProbabilities);
	}

	public void ResetCells()
	{
		GameData.ResetDataPropperties(GameManager.instance.key);
		Exit();
	}

	public void SaveGameProgress(string key)
	{
		// save Game
		GameData.gameProperties[key].score = GameData.Score;
		GameData.gameProperties[key].topScore = GameData.TopScore;
		GameData.gameProperties[key].cells.Clear();
		GameData.gameProperties[key].nextCells.Clear();

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (cells[i, j] != null)
				{
					CellProperties thisCell = new CellProperties(cells[i, j].i, cells[i, j].j, cells[i, j].CellValue);
					GameData.gameProperties[key].cells.Add(thisCell);
				}
			}
		}

		HexagonController[] val = nextCells.ToArray();
		for (int q = 0; q < val.Length; q++)
			GameData.gameProperties[key].nextCells.Add(val[q].CellValue);

		GameData.gameProperties[key].score = GameData.Score;
		GameData.gameProperties[key].topScore = GameData.TopScore;
		GameData.gameProperties[key].undoLevel = GameData.UndoBonusLevel;
		GameData.gameProperties[key].hummerLevel = GameData.HummerBonusLevel;
	}

	public void Exit()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (cells[i, j] != null)
				{
					PasteToPool(cells[i, j]);
					TransparentBackCell(i, j, 255);
					cells[i, j] = null;
				}

				if (backField[i, j] != null)
				{
					backField[i, j].gameObject.SetActive(false);
					Pool.field.Enqueue(backField[i, j].gameObject);
				}
			}
		}

		HexagonController[] next = nextCells.ToArray();
		for (int n = 0; n < next.Length; n++)
			PasteToPool(next[n]);

		if (cellsToSpawn.Count > 0)
		{
			for (int m = 0; m < cellsToSpawn.Count; m++)
				PasteToPool(cellsToSpawn[m]);
		}

		cellsToSpawn.Clear();
		nextCells.Clear();
		commands.Clear();
		cellsOnField = 0;
	}

	public void CoinsUpAnim(GameObject coin)
	{
		coin.transform.position = sameCells[0].transform.position;
	}

	public IEnumerator NumberAnim(int valueFrom, int valueTo, float animTime, System.Action<int> onValueChanged)
	{
		float currentTime = 0;
		while (currentTime < animTime)
		{
			currentTime += Time.deltaTime;
			int currentValue = (int) Mathf.Lerp(valueFrom, valueTo, currentTime / animTime);
			onValueChanged.Invoke(currentValue);
			yield return null;
		}
	}
}
