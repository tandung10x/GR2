using UnityEngine;
using UnityEngine.UI;

public class UndoPriceText : MonoBehaviour
{
	Text price;

	void OnEnable()
	{
		price = GetComponent<Text>();
		price.text = GameData.UndoPrice + "  ?";
	}
}
