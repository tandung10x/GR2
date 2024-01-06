using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Tween/Tween Position")]
public class UITweenPosition : UITweenBase
{
    public bool local = true;
    public bool resetStartPositionOnPlay = false;
    public Vector3 from;
    public Vector3 to;

    Vector3 _startPos;

    Transform _transform;
    Transform _cachedTransform
    {
        get
        {
            if (_transform == null)
            {
                _transform = transform;
            }
            return _transform;
        }
    }

    protected override void Init()
    {
        base.Init();
        _startPos = _cachedTransform.position;
    }

    public override void Play(bool playForward, bool immd = false)
    {
        Init();
        if (resetStartPositionOnPlay)
        {
            _startPos = _cachedTransform.position;
        }
        base.Play(playForward, immd);
    }

    protected override void ApplyValue()
    {
        Vector3 newPos = Vector3.Lerp(_playForward ? from : to, _playForward ? to : from, _value);
        if (local)
        {
            newPos += _startPos;
        }
        _cachedTransform.position = newPos;
    }
}
