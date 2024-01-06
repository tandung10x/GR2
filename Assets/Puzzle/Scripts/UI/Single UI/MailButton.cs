using UnityEngine;

public class MailButton : SingleButton
{

    protected override void Click()
    {
        Application.OpenURL(Config.regularEmail);
    }
}
