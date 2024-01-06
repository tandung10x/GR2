using System;
using UnityEngine;

public class NewField : MonoBehaviour
{

	public bool open;

	public Action<bool> onClicked;

	public void OpenPop_up()
	{

		if (onClicked != null)
		{
			onClicked.Invoke(open);
			onClicked = null;
		}
	}
}
