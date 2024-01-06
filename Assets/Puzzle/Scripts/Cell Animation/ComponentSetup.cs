using UnityEngine;
using UnityEngine.UI;

public class ComponentSetup : MonoBehaviour
{

    public Color shadowColor = Color.black;
    public Vector2 shadowDistance;

    [ContextMenu("Add shadows")]
    void AddShadowsToText()
    {
        Text[] txts = GetComponentsInChildren<Text>(true);
        foreach (Text t in txts)
        {
            if (t.GetComponent<Shadow>() != null)
            {
                t.gameObject.GetComponent<Shadow>().effectColor = shadowColor;
                t.gameObject.GetComponent<Shadow>().effectDistance = shadowDistance;
            }
        }

        Debug.LogError(txts.Length);
    }
}
