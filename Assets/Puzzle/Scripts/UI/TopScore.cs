using UnityEngine;
using UnityEngine.UI;

public class TopScore : MonoBehaviour
{

	Text topScore;

	void Start()
	{
		topScore = GetComponent<Text>();
		GameData.onScoresChanged.AddListener(OnValueChanged);
		OnValueChanged();
	}

	void OnValueChanged()
	{
		topScore.text = GameData.TopScore.ToString();
	}
}