using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Tween/Tween Group Alpha")]
public class UITweenGroupAlpha : UITweenBase
{
    public float from = 0;
    public float to = 1;
    public bool affectOnInteractions = true;
    public bool disableOnHide = true;
    public bool interactable
    {
        get
        {
            return _cahcedGroup.interactable;
        }
        set
        {
            _cahcedGroup.interactable = value;
        }
    }
    CanvasGroup _group;
    CanvasGroup _cahcedGroup
    {
        get
        {
            if (_group == null)
            {
                _group = GetComponent<CanvasGroup>();
            }
            return _group;
        }
    }

    protected override void Init()
    {
        base.Init();
        if (_cahcedGroup == null)
        {
            Disable();
        }
    }

    protected override void ApplyValue()
    {
        _cahcedGroup.alpha = Mathf.Lerp(_playForward ? from : to, _playForward ? to : from, _value);
    }

    public override void Play(bool playForward, bool immd = false)
    {
        Init();
        _value = 0;// playForward ? 0 : 1;
        ApplyValue();
        gameObject.SetActive(true);
        _cahcedGroup.interactable = false;
        if (playForward)
        {
            OnFinished += () => _cahcedGroup.interactable = true;
        }
        else
        {
            if (disableOnHide)
            {
                OnFinished += () => gameObject.SetActive(false);
            }
        }
        base.Play(playForward, immd);
    }
}
