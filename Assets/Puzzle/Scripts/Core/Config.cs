//This class should not be modified
public static class Config
{

    public const string serverIp = "";

    public static string websiteLink = "";
    public static int overriddenAdsRate = -1;
    public static int overriddenCrossPromoRate = -1;
    public static int gamesUpdateVersion = -1;

    public static string emailAdress = "";
    public static string complainsEmailSubject = ProjectConfig.projectName + " sucks!";
    public static string complainsEmailBody = "I don't like your game because ";

    public static string complainsEmail
    {
        get
        {
            string s = string.Format("mailto:{0}?subject={1}&body={2}", emailAdress, complainsEmailSubject,
                complainsEmailBody);
            return s;
        }
    }

    public static string regularEmailSubject = ProjectConfig.projectName;
    public static string regularEmailBody = "Hello, LethargicaLab Team! I want to say...";

    public static string regularEmail
    {
        get
        {
            string s = string.Format("mailto:{0}?subject={1}&body={2}", emailAdress, regularEmailSubject,
                regularEmailBody);
            return s;
        }
    }
}
