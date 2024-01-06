using UnityEngine;
using UnityEngine.UI;

public class ModePriceText : MonoBehaviour
{

	Text price;

	void OnEnable()
	{
		price = GetComponent<Text>();
		price.text = _price + "  ?";
	}

	int _price
	{
		get
		{
			int value = 0;
			return value;
		}
	}
}