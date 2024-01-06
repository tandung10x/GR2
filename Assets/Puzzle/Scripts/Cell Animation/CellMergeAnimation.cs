using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MergeDirection : int
{
    TOP_RIGHT = 0,
    TOP_LEFT = 1,
    RIGHT = 2,
    BOTTOM_LEFT = 3,
    BOTTOM_RIGHT = 4,
    LEFT = 5
}

public class CellMergeAnimation : MonoBehaviour {

    public Action OnMergeFinished;

    [Header("Links")]
    public CellMergeAnimation mergeToCell;
    public List<CellMergeAnimation> prevCells = new List<CellMergeAnimation>();

    [Header("Graphics")]
    public Text text;
    public Image main;
	public Image pattern;
    public RectTransform FX;
    public GameObject smooth;
    public GameObject trail;
    public ParticleSystem particles;

    [HideInInspector]
    public HexagonController cellController;

    MergeDirection _mergeDirection; //for test
    Image _trailImage;
    Image[] _smoothImages;

    RectTransform _rectTransform;
    UITweenRectPosition _tweenPos;
    int tails;

    void Start ()
    {
        _trailImage = trail.GetComponent<Image>();
        _smoothImages = smooth.GetComponentsInChildren<Image>(true);
        _rectTransform = GetComponent<RectTransform>();
        _tweenPos = GetComponent<UITweenRectPosition>();
        FX.gameObject.SetActive(false);
    }

    public void InitMerge()
    {
        Link();
        if (prevCells.Count == 0)
        {
            Merge();
        }
        else
        {
            foreach (CellMergeAnimation c in prevCells)
            {
                c.InitMerge();
            }
        }
    }

    void Link()//int dirX, int dirY)
    {

        tails = prevCells.Count;
        _trailImage.color = main.color;
        particles.startColor = main.color;
        foreach(Image sm in _smoothImages)
        {
            sm.color = main.color;
        }
        text.enabled = false;

        if (mergeToCell == null) return;

        int dirX, dirY;

        dirX = mergeToCell.cellController.i - cellController.i;
        dirY = mergeToCell.cellController.j - cellController.j;

        if (dirX == 0)
        {
            dirX += ((dirY != 0 && mergeToCell.cellController.j % 2 == 0) ? 1 : -1);
        }

        if (dirX == -1 && dirY == 1)
        {
            _mergeDirection = MergeDirection.TOP_LEFT;
        }
        else if (dirX == 1 && dirY == 1)
        {
            _mergeDirection = MergeDirection.TOP_RIGHT;
        }
        else if (dirX == 1 && dirY == 0)
        {
            _mergeDirection = MergeDirection.LEFT;
        }
        else if (dirX == -1 && dirY == 0)
        {
            _mergeDirection = MergeDirection.RIGHT;
        }
        else if (dirX == -1 && dirY == -1)
        {
            _mergeDirection = MergeDirection.BOTTOM_LEFT;
        }
        else if (dirX == 1 && dirY == -1)
        {
            _mergeDirection = MergeDirection.BOTTOM_RIGHT;
        }
        else
        {
            Debug.LogError("direction is undefined: (" + dirX + "; " + dirY + ")");
        }

        FX.localEulerAngles = Vector3.forward * (int) _mergeDirection * 60;
        FX.gameObject.SetActive(true);
        smooth.SetActive(true);
        trail.SetActive(false);
    }

    public void Merge()
    {
        if (tails > 1)
        {
            tails--;
            return;
        }

        if (mergeToCell == null)
        {
            text.enabled = true;
            OnMergeEnd();
            return;
        }

        _tweenPos.from = _rectTransform.anchoredPosition;
        _tweenPos.to = mergeToCell.GetComponent< RectTransform>().anchoredPosition;

        main.enabled = false;
		pattern.enabled = false;
        particles.gameObject.SetActive(true);
        trail.SetActive(true);

        Invoke("DisableSmooth", _tweenPos.duration / 4);

        _tweenPos.OnFinished += OnMergeEnd;
        _tweenPos.PlayForward();
    }

    void OnMergeEnd()
    {
        prevCells.Clear();
        FX.gameObject.SetActive(false);
        if (mergeToCell != null)
        {
            mergeToCell.Merge();
            mergeToCell = null;
        }
        if (OnMergeFinished != null)
        {
            OnMergeFinished();
            OnMergeFinished = null;
        }
        else
        {
            main.color = Color.red;
        }
    }

    void DisableSmooth()
    {
        smooth.SetActive(false);
    }

	void OnEnable () {
		main.enabled = true;
		text.enabled = true;
		pattern.enabled = true;
	}
}
