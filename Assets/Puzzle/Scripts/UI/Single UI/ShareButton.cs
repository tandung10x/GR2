using UnityEngine;

public class ShareButton : SingleButton
{
    public static CallbackDelegate callback;

    protected override void Click()
    {
        //StartCoroutine(ShareManager.ShareScreenshot(GetBestScore(), callback));
    }

    void OnDestroy()
    {
        callback = null;
    }

    public static int GetBestScore()
    {
        return PlayerPrefs.GetInt("HiScore");
    }
}
