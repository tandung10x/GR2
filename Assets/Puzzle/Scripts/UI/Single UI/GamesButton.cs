using UnityEngine;

public class GamesButton : SingleButton
{

    public GameObject notification;

    const string lastGamesUpdateVersionKey = "lastGamesUpdateVersion";

    protected override void Start()
    {
        base.Start();
        notification.SetActive(PuzzleGames.Tools.isOnline && ShowNotification());
    }

    protected override void Click()
    {
        Application.OpenURL(Config.websiteLink);
        if (PuzzleGames.Tools.isOnline && ShowNotification())
        {
            PlayerPrefs.SetInt(lastGamesUpdateVersionKey, Config.gamesUpdateVersion);
            notification.SetActive(false);
        }
    }

    protected bool ShowNotification()
    {
        return Config.gamesUpdateVersion > -1 &&
               Config.gamesUpdateVersion > PlayerPrefs.GetInt(lastGamesUpdateVersionKey);
    }
}
