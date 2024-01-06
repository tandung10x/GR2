using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour
{

	Text coins;

	void Start()
	{
		coins = GetComponent<Text>();
		OnCoinsChanged();
		GameData.OnCoinsChange += OnCoinsChanged;
	}

	void OnCoinsChanged()
	{
		coins.text = GameData.coins.ToString();
	}
}
