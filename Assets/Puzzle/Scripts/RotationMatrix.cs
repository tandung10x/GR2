using UnityEngine;
using UnityEngine.UI;

public class RotationMatrix : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public Vector2 texPivot = new Vector2(0.5f, 0.5f);
    Image _renderer;
    public float time;

    void Start()
    {
        _renderer = GetComponent<Image>();
    }

    protected void Update()
    {
        Matrix4x4 t = Matrix4x4.TRS(-texPivot, Quaternion.identity, Vector3.one);

        Quaternion rotation = Quaternion.Euler(0, 0, time * rotationSpeed);
        Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

        Matrix4x4 tInv = Matrix4x4.TRS(texPivot, Quaternion.identity, Vector3.one);
        _renderer.material.SetMatrix("_Rotation", tInv * r * t);
    }
}