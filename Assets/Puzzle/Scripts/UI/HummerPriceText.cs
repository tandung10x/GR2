using UnityEngine;
using UnityEngine.UI;

public class HummerPriceText : MonoBehaviour
{

	Text price;

	void OnEnable()
	{
		price = GetComponent<Text>();
		price.text = GameData.HummerPrice + "  ?";
	}
}
