using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SingleButton : MonoBehaviour
{
    public delegate void CallbackDelegate();

    public Image colorTarget;
    public Color onColor = Color.white;
    public Color offColor = Color.grey;

    public Image imageTarget;
    public Sprite onSprite;
    public Sprite offSprite;

    protected virtual void Start()
    {
        UpdateGraphics();
        GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => Click()));
    }

    /// <summary>
    /// Method called on click.
    /// Override it with functions you want to execute on button click.
    /// </summary>
    protected virtual void Click()
    {

    }

    protected virtual bool GetValue()
    {
        return true;
    }

    protected void UpdateGraphics()
    {
        bool value = GetValue();

        if (colorTarget != null)
            colorTarget.color = value ? onColor : offColor;

        if (imageTarget != null)
            imageTarget.sprite = value ? onSprite : offSprite;
    }
}
