using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Tween/Tween Rotation")]
public class UITweenRotation : UITweenBase
{
    public Vector3 from;
    public Vector3 to;

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
        _cachedTransform.eulerAngles = Vector3.Lerp(_playForward ? from : to, _playForward ? to : from, _value);
    }
}
