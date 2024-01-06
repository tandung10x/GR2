using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI Tween/Tween Sprite")]
public class UITweenSprite : UITweenBase
{
    public Sprite[] sprites;
    public Image target;

    protected override void Init()
    {
        base.Init();
        if (target == null) target = GetComponent<Image>();
    }

    protected override void ApplyValue()
    {
        if (target != null && sprites != null && sprites.Length > 0)
        {
            int index = (int)((sprites.Length - 1) * _value);
            target.enabled = sprites[index] != null;
            target.sprite = sprites[index];
        }
    }
}
