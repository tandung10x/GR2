using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Tween/Tween Rect Position")]
public class UITweenRectPosition : UITweenBase
{
    public bool local = true;
    public bool resetStartPositionOnPlay = false;
    public Vector3 from;
    public Vector3 to;

    Vector3 _startPos;
    RectTransform _transform;
    RectTransform _cachedTransform
    {
        get
        {
            if (_transform == null)
            {
                _transform = GetComponent<RectTransform>();
            }
            return _transform;
        }
    }

    protected override void Init()
    {
        base.Init();
        _startPos = _cachedTransform.anchoredPosition;
    }

    public override void Play(bool playForward, bool immd = false)
    {
        Init();
        if (resetStartPositionOnPlay)
        {
            _startPos = _cachedTransform.anchoredPosition;
        }
        base.Play(playForward, immd);
    }

    protected override void ApplyValue()
    {
        //Vector3 newPos = Vector3.Lerp(_playForward ? from : to, _playForward ? to : from, _value);

        Vector3 fromPos = _playForward ? from : to;
        Vector3 toPos = _playForward ? to : from;

        Vector3 newPos = fromPos * (1f - _value) + toPos * _value;
        if (local)
        {
            newPos += _startPos;
        }
        _cachedTransform.anchoredPosition = newPos;
    }
}