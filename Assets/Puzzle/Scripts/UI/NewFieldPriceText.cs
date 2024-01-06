using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NewFieldPriceText : MonoBehaviour
{

	Text price;

	void OnEnable()
	{
		price = GetComponent<Text>();
		price.text = _price + "  ?";
	}

	int _price
	{
		get { return GameConfig.openNewFieldPrice; }
	}
}
