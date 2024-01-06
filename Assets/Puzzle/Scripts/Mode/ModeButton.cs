using UnityEngine;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour
{

    public Shape thisShape;

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ChooseField);
    }

    void ChooseField()
    {
        ModeController.GetCurrentShape(thisShape);
    }
}