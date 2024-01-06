using UnityEngine;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour
{
    [SerializeField] private Shape thisShape;
    [SerializeField] private AudioClip playSound;

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ChooseField);
    }

    void ChooseField()
    {
        SoundManager.Instance.PlaySfx(playSound);
        ModeController.GetCurrentShape(thisShape);
    }
}