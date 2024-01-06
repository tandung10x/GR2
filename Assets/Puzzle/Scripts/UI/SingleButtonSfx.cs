using UnityEngine;
using UnityEngine.UI;

public class SingleButtonSfx : MonoBehaviour
{

	Button button;

	void Start()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(Play);
	}

	void Play()
	{
		GameManager.instance.ButtonsSfx();
	}
}