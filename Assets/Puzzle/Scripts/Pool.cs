using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{

	public GameObject cell;
	public GameObject ground;
	public GameObject tile;
	public RectTransform fielHolder;
	public RectTransform cellsHolder;

	[HideInInspector]
	public static Queue<GameObject> field = new Queue<GameObject>();
	[HideInInspector]
	public static Queue<GameObject> cells = new Queue<GameObject>();


	int poolCount
	{
		get { return GameConfig.maxWidth * GameConfig.maxHeight; }
	}

	int CellsPool
	{
		get
		{
			if (CellsPool <= 0)
				CreateMoreCells();
			return cells.Count;
		}
	}

	void Awake()
	{
		CreatePool();
	}

	void CreateMoreCells()
	{
		cells = Generate(cell, cellsHolder, poolCount);
	}

	void CreatePool()
	{
		field = Generate(ground, fielHolder, poolCount);
		cells = Generate(cell, cellsHolder, poolCount);
	}

	Queue<GameObject> Generate(GameObject obj, RectTransform holder, int count)
	{
		Queue<GameObject> queue = new Queue<GameObject>();

		for (int i = 0; i < count; i++)
		{
			GameObject objectToSpawn = Instantiate(obj, holder.position, Quaternion.identity) as GameObject;
			objectToSpawn.transform.SetParent(holder, false);
			objectToSpawn.SetActive(false);
			objectToSpawn.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
			queue.Enqueue(objectToSpawn);
		}

		return queue;
	}
}
