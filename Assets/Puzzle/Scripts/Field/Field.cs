using UnityEngine;

public enum Shape : int
{
	Hexagon = 0,
	Rectangle,
	Square,
	Triangle,
	Cross,
	CustomField_1,
    CustomField_2,
    CustomField_3,
    CustomField_4,
    CustomField_5,
    CustomField_6,
	enumLength
}

public enum OpenState
{
	Open,
	Locked,
	Tutorial
}

[System.Serializable]
public class FieldType
{
	public OpenState openState;

    public string name = "field";
	public Shape shape;
	public TextAsset field;

	public Vector3 nextCellStartPosition;
	public Vector3 startPosition;
	public Vector3 scale;

	public Sprite fieldView;

	public int cellsToSpawn;
}

[CreateAssetMenu(menuName = "Custom/Field")]
public class Field : ScriptableObject
{

	public FieldType[] fields;

}
