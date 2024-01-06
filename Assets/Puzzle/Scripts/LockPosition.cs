using UnityEngine;

public class LockPosition : MonoBehaviour
{
	Vector3 startPos;

	void Start()
	{
		startPos = transform.position;
	}

	void Update()
	{
		transform.position = startPos;
	}
}
