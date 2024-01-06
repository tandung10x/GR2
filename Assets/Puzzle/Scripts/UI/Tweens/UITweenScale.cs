using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Tween/Tween Scale")]
public class UITweenScale : UITweenBase
{
    public Vector3 from = Vector3.one;
    public Vector3 to = Vector3.one;

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
    }

    protected override void ApplyValue()
    {
        _cachedTransform.localScale = Vector3.Lerp(_playForward ? from : to, _playForward ? to : from, _value);
    }
}
