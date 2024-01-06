using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScreenshotManager : MonoBehaviour
{

	public delegate void OnScreenshotDelegate(Texture2D scr);

	public OnScreenshotDelegate OnScreenshot;

	public Camera mainCam;
	public Camera screenshotCam;
	public GameObject canvas;

	public GameObject[] inactive;

	public Text shareText;
	public Text scoreText;
	public RawImage fieldTexture;

	Texture2D mainTexture;
	string text = "I've got" + '\n' + "a new record!";

	string _screenshotPath;

	public void SetScreen()
	{
		shareText.text = text;
		scoreText.text = GameData.TopScore.ToString();

		foreach (GameObject obj in inactive)
			obj.SetActive(false);

		canvas.SetActive(true);

		StartCoroutine(ScreenMainField());
	}

	IEnumerator ScreenMainField()
	{
		yield return new WaitForEndOfFrame();

		mainTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		mainCam.targetTexture = RenderTexture.active;
		mainCam.Render();
		mainTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		mainTexture.Apply();

		RenderTexture.active = null;

		fieldTexture.texture = mainTexture;
		fieldTexture.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 1.5f, Screen.height * 1.5f);

		StartCoroutine(DoScreenShot());
	}

	IEnumerator DoScreenShot()
	{
		yield return new WaitForEndOfFrame();

		int width = screenshotCam.targetTexture.width;
		int height = screenshotCam.targetTexture.height;

		Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
		RenderTexture.active = screenshotCam.targetTexture;

		screenshotCam.Render();
		texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		texture.Apply();

		RenderTexture.active = null;

		canvas.SetActive(false);

		byte[] bytes = texture.EncodeToPNG();
		string name = "Screenshot.png";
		_screenshotPath = Application.persistentDataPath + "/" + name;

		File.WriteAllBytes(_screenshotPath, bytes);

		while (!File.Exists(_screenshotPath))
		{
			yield return null;
		}

		if (OnScreenshot != null)
		{
			OnScreenshot(texture);
			OnScreenshot = null;
		}

		foreach (GameObject obj in inactive)
			obj.SetActive(true);

		ShareManager.ShareImage(_screenshotPath, string.Format(ShareManager.shareText, GameData.TopScore), null);
	}
}
