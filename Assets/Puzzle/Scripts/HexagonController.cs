using UnityEngine;
using UnityEngine.UI;
using System;

public class HexagonController : MonoBehaviour
{
	[SerializeField] private Color[] colors;
	[SerializeField] private Color[] textColors;

	public Image cellImage;
	public Image skin;
	public Text cellText;
	public ParticleSystem particlesOnMove;

	public AudioClip onAppearSfx;
	public AudioClip onConnectionSfx;
	public AudioClip coinUpSfx;
	public AudioClip movingSfx;
	public AudioClip onClickSfx;
	public AudioClip blockedPathSfx;

	public UITweenScale onClickTweenScale;
	public UITweenScale onSpawnTweenScale;

	public static int currentI = -1;
	public static int currentJ = -1;
	[HideInInspector]
	public Color currentCol;

	public int i;
	public int j;

	Color textColor;
	bool isClear;

	Action OnValueChange;

	int _cellValue;
	public int CellValue
	{
		get { return _cellValue; }
		set
		{
			if (value > GameController.instance.values[GameData.MaxCellValue] &&
			    value <= GameController.instance.values[GameController.instance.values.Count - 1])
			{
				GameData.MaxCellValue++;
			}

			_cellValue = value;

			SetUpColor(value);
			if (OnValueChange != null)
				OnValueChange.Invoke();
		}
	}

	int _valueCounter;
	public int ValueCounter
	{
		get { return _valueCounter; }
		set { _valueCounter = value; }
	}

	CellMergeAnimation _mergeAnimation;
	public CellMergeAnimation mergeAnimation
	{
		get
		{
			if (_mergeAnimation == null)
			{
				_mergeAnimation = GetComponent<CellMergeAnimation>();
				_mergeAnimation.cellController = this;
			}

			return _mergeAnimation;
		}
	}

	void OnEnable()
	{
		textColor = cellText.color;
		OnValueChange += SetUpText;
	}

	public void OnSpawningAnim()
	{
		onSpawnTweenScale.autoClearCallbacks = false;
		onSpawnTweenScale.OnFinished = null;
		onSpawnTweenScale.OnFinished += OnStopAnim;
		SoundManager.Instance.PlaySfx(onAppearSfx, 1);
		onSpawnTweenScale.PlayForward();
	}

	void OnStopAnim()
	{
		onSpawnTweenScale.OnFinished -= OnStopAnim;
		if (!isClear) return;

		isClear = false;
		GameController.instance.PasteToPool(this);
		GameController.instance.TransparentBackCell(i, j, 255);
	}

	public void OnMergeSound()
	{
		SoundManager.Instance.PlaySfx(onConnectionSfx, 1);
	}

	public void OnMovementSound()
	{
		SoundManager.Instance.PlaySfx(movingSfx, 1);
	}

	public void OnBlockedPathSound()
	{
		SoundManager.Instance.PlaySfx(blockedPathSfx, 1);
	}

	void SetUpText()
	{
		if (CellValue > 8192)
			cellText.text = PuzzleGames.Tools.IntToKiloStringFormat(CellValue, 0, PuzzleGames.RoundType.FLOOR);
		else
			cellText.text = CellValue.ToString();
	}

	void SetUpColor(int val)
	{
		int maxVal = GameController.instance.values[GameController.instance.values.Count - 1];
		for (int q = 0; q < GameController.instance.values.Count; q++)
		{
			skin.gameObject.SetActive(false);
			if (val != GameController.instance.values[q]) continue;
			_valueCounter = q;
			break;
		}

		cellImage.color = colors[_valueCounter];
		cellText.color = textColors[_valueCounter];
		textColor = cellText.color;
		currentCol = cellImage.color;
	}

	public void MovingCell()
	{
		currentI = i;
		currentJ = j;
		OnClickCell();
	}

	public void OnClickCell()
	{
		SoundManager.Instance.PlaySfx(onClickSfx, 1);

		if (GameManager.instance.stateMachine.CurrentState == State.TUTORIAL)
		{
			if (GameManager.InTutorial != null)
				GameManager.InTutorial.Invoke();
		}

		onClickTweenScale.PlayForward();
		onClickTweenScale.autoClearCallbacks = false;
		onClickTweenScale.OnFinished = null;
		onClickTweenScale.OnFinished += ClickOff;
	}

	public void ClickOff()
	{
		onClickTweenScale.Stop();
		onClickTweenScale.OnFinished -= ClickOff;
	}

	public void OnClearCell()
	{
		isClear = true;
		onSpawnTweenScale.autoClearCallbacks = false;
		onSpawnTweenScale.OnFinished = null;
		onSpawnTweenScale.OnFinished += OnStopAnim;
		onSpawnTweenScale.PlayReverse();
	}

	public void TransparentState(float aState, bool txtcol)
	{
		Color currentCellColor = cellImage.color;
		cellImage.color = new Color(cellImage.color.r, cellImage.color.g, cellImage.color.b, aState);
		if (txtcol != true)
		{
			particlesOnMove.gameObject.SetActive(false);
			cellText.color = textColor;
		}
		else
		{
			cellText.color = currentCellColor;
			particlesOnMove.gameObject.SetActive(true);
			particlesOnMove.startColor = currentCellColor;
		}
	}
}
