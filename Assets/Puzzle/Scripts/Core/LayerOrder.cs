using UnityEngine;

public class LayerOrder : MonoBehaviour
{

    public int sortingOrder;
    public string sortingLayerName = "Default";

    [ContextMenu("Apply")]
    void Start()
    {
        try
        {
            GetComponent<Renderer>().sortingOrder = sortingOrder;
            GetComponent<Renderer>().sortingLayerName = sortingLayerName;
        }
        catch
        {
            Debug.LogWarning("No renderer on " + gameObject.name);
        }
    }
}
