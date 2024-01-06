using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

	public Sprite bestScoreText;
	public Sprite simpleText;

	public GameObject particles;
	public GameObject crown;
	public GameObject continueForAds;
	public GameObject movableGroup;
	public Image topText;

	public GameObject adsButton;
	public GameObject coinsButton;

	public float animTime;
	public Text scoreText;
	public Text topScoreText;
	public Text coinsText;

	public Transform adsOn;
	public Transform adsOff;

	int score;
	int coins;
	int topScore;

	void OnEnable()
	{

		if (GameData.IsBestScore != 0)
		{
			topText.sprite = bestScoreText;
			crown.SetActive(true);
			Invoke("BestScoreTextAnim", 0.5f);
		}
		else
		{
			topText.sprite = simpleText;
			crown.SetActive(false);
			topScoreText.text = GameData.TopScore.ToString();
		}

		particles.SetActive(true);

		if (GameData.continueCounter < GameConfig.continueForAdsCount)
		{
			adsButton.SetActive(false);
			coinsButton.SetActive(true);
			movableGroup.transform.position = adsOn.position;
			continueForAds.SetActive(true);
		}
		else
		{
			continueForAds.SetActive(false);
			movableGroup.transform.position = adsOff.position;
		}

		movableGroup.SetActive(true);

		Invoke("CoinTextAnim", 0.5f);
		Invoke("ScoreTextAnim", 0.5f);
	}

	void Update()
	{
		scoreText.text = score.ToString();
		coinsText.text = coins.ToString();
		if (GameData.IsBestScore != 0)
		{
			topScoreText.text = topScore.ToString();
		}
	}

	void ScoreTextAnim()
	{
		StartCoroutine(GameController.instance.NumberAnim
			(
				0,
				GameData.Score,
				animTime,
				(int thisScore) => score = thisScore
			)
		);
	}

	void CoinTextAnim()
	{
		StartCoroutine(GameController.instance.NumberAnim
			(
				0,
				GameData.coins,
				animTime,
				(int thisCoins) => coins = thisCoins
			)
		);
	}

	void BestScoreTextAnim()
	{
		StartCoroutine(GameController.instance.NumberAnim
			(
				0,
				GameData.TopScore,
				animTime,
				(int thisTopScore) => topScore = thisTopScore
			)
		);
	}

	public void Hide()
	{
		continueForAds.SetActive(false);
		crown.SetActive(false);
		particles.SetActive(false);
		movableGroup.SetActive(false);
		coinsButton.SetActive(false);
		adsButton.SetActive(false);
	}
}
