public class MuteButton : SingleButton
{
    public AudioType type = AudioType.SOUNDS;

    protected override void Start()
    {
        Listen(true);
        SoundManager.OnSoundsChange += UpdateGraphics;
        base.Start();
    }

    void OnDestroy()
    {
        Listen(false);
    }

    void Listen(bool listen)
    {
        if (type == AudioType.SOUNDS)
            if (listen)
                SoundManager.OnSoundsChange += UpdateGraphics;
            else
                SoundManager.OnSoundsChange -= UpdateGraphics;
        else if (type == AudioType.MUSIC)
        {
            if (listen) SoundManager.OnMusicChange += UpdateGraphics;
            else SoundManager.OnMusicChange -= UpdateGraphics;
        }
    }

    protected override void Click()
    {
        SoundManager.Instance.MuteAudio(type);
    }

    protected override bool GetValue()
    {
        switch (type)
        {
            case AudioType.SOUNDS:
                return SoundManager.Instance.SfxOn;
            case AudioType.MUSIC:
                return SoundManager.Instance.MusicOn;
            default:
                return true;
        }
    }
}
