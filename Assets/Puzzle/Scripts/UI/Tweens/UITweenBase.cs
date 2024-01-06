using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenBase : MonoBehaviour {

    public enum PlayMode
    {
        ONCE,
        LOOP,
        PING_PONG
    }

    public enum BeginPlay
    {
        ON_CALL,
        ON_ENABLE_FORWARD,
        ON_ENABLE_REVERSE
    }

    public enum OnDisableAction
    {
        NONE,
        RESET_TO_BEGINNING,
        RESET_TO_END
    }

    public delegate void OnEventDelegate();
    public OnEventDelegate OnFinished;

    public PlayMode playMode = PlayMode.ONCE;
    public BeginPlay beginPlay = BeginPlay.ON_CALL;
    public OnDisableAction onDisableAction = OnDisableAction.NONE;
    public float delay = 0;
    public float duration = 1f;
    public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0f,0f), new Keyframe(1f, 1f)});

	[HideInInspector]
	public bool autoClearCallbacks = true;

    protected float _value;
    private float _startTime;
    protected bool _playForward;
    private bool _inited = false;
    private bool _immediate;
    private bool _playOnEnable = false;
    private bool _isPlaying = false;
    public bool isPlaying
    {
        get
        {
            return _isPlaying;
        }
    }

    private Coroutine _playCoroutine;

    protected virtual void Init()
    {
        if (_inited) return;
        _inited = true;
    }

    protected void Disable()
    {
        this.enabled = false;
    }

    public void Stop()
    {
        StopAllCoroutines();
        _isPlaying = false;
        _value = 0;
        ApplyValue();
    }

    public void Reset(bool toBeginning)
    {
        Play(toBeginning, true);
    }

    public virtual void Play(bool playForward, bool immd = false)
    {
        _playForward = playForward;
        _immediate = immd;

        if ((!gameObject.activeInHierarchy || !gameObject.activeSelf) && !_immediate)
        {
            _value = playForward ? 0 : 1;
            ApplyValue();
            _playOnEnable = true;
            gameObject.SetActive(true);
            return;
        }

        _isPlaying = true;
        _playOnEnable = false;
        _playForward = playForward;
        _immediate = immd;

        if (_immediate && playMode == PlayMode.ONCE)
        {
            _value = 0;// _playForward ? 1 : 0;
            ApplyValue();
        }
        else
        {
            if (_playCoroutine != null) StopCoroutine(_playCoroutine);
            _playCoroutine = StartCoroutine(Evaluate());
        }
    }

    IEnumerator Evaluate()
    {
        if (!Mathf.Approximately(delay, 0f))
            yield return new WaitForSeconds(delay);
        
        _startTime = Time.time;
        float time = 0;
        while (time <= duration)
        {
            time = Time.time - _startTime;
            _value = curve.Evaluate(time / duration);
            ApplyValue();
            yield return null;
        }

        switch(playMode)
        {
            case PlayMode.ONCE:
                if (OnFinished != null)
                {
                    OnFinished();
					if (autoClearCallbacks)
                    	OnFinished = null;
                }
                break;
            case PlayMode.LOOP:
                Play(_playForward);
                break;
            case PlayMode.PING_PONG:
                _playForward = !_playForward;
                Play(_playForward);
                break;
        }
    }

    protected virtual void ApplyValue()
    {
        //use value here
    }

    public virtual void PlayForward(bool immd = false)
    {
        Play(true, immd);
    }

    public virtual void PlayReverse(bool immd = false)
    {
        Play(false, immd);
    }

    void OnEnable()
    {
        Init();

        switch (beginPlay)
        {
            case BeginPlay.ON_ENABLE_FORWARD:
                PlayForward();
                break;
            case BeginPlay.ON_ENABLE_REVERSE:
                PlayReverse();
                break;
            default:
                if (_playOnEnable)
                {
                    Play(_playForward, _immediate);
                }
                break;
        }
    }

    void OnDisable()
    {
        switch (onDisableAction)
        {
            case OnDisableAction.RESET_TO_BEGINNING:
                Reset(true);
                break;
            case OnDisableAction.RESET_TO_END:
                Reset(false);
                break;
        }
    }
}
