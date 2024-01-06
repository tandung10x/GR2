using UnityEngine;

public class AppLinkButton : SingleButton
{

    protected override void Click()
    {
#if UNITY_ANDROID
        Application.OpenURL(ProjectConfig.googlePlayLink);
#endif
    }
}
