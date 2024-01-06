using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI Tween/Tween Color")]
public class UITweenColor : UITweenBase
{
    public Color from = Color.white;
    public Color to = new Color(1, 1, 1, 0);

    SpriteRenderer[] _sprites;
    Image[] _images;
    Text[] _texts;

    protected override void Init()
    {
        base.Init();
        _sprites = GetComponentsInChildren<SpriteRenderer>();
        _images = GetComponentsInChildren<Image>();
        _texts = GetComponentsInChildren<Text>();
    }

    protected override void ApplyValue()
    {
        if (_sprites != null)
        {
            foreach (SpriteRenderer sr in _sprites)
            {
                sr.color = Color.Lerp(_playForward ? from : to, _playForward ? to : from, _value);
            }
        }

        if (_images != null)
        {
            foreach (Image im in _images)
            {
                im.color = Color.Lerp(_playForward ? from : to, _playForward ? to : from, _value);
            }
        }

        if (_texts != null)
        {
            foreach (Text text in _texts)
            {
                text.color = Color.Lerp(_playForward ? from : to, _playForward ? to : from, _value);
            }
        }
    }
}
