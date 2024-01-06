using UnityEngine;
using Procurios.Public;

namespace PuzzleGames
{
    public enum RoundType
    {
        NONE,
        CEIL,
        FLOOR,
        ROUND
    }

    public static class Tools
    {
        public static ScreenOrientation screenOrientation
        {
            get
            {
                return Screen.orientation;
            }
        }

        public static UnityEngine.RuntimePlatform platform
        {
            get
            {
                return Application.platform;
            }
        }

        public static bool isOnline
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        public static object JSONDecode(string json)
        {
            return JSON.JsonDecode(json);
        }

        public static string JSONEncode(object obj)
        {
            return JSON.JsonEncode(obj);
        }

		public static string IntToKiloStringFormat(int value, int decimals, RoundType roundType = RoundType.NONE, string suffix = "k", bool ignoreLess1000 = true)
		{
			string res = "";

			if (ignoreLess1000 && value < 1000)
			{
				res = value.ToString();
			}
			else
			{
                string FORMAT = "F" + decimals;

				float kilos = (float)value / 1000;

                switch (roundType)
                {
                    case RoundType.CEIL:
                        kilos = Mathf.Ceil(kilos);
                        break;
                    case RoundType.ROUND:
                        kilos = Mathf.Round(kilos);
                        break;
                    case RoundType.FLOOR:
                        kilos = Mathf.Floor(kilos);
                        break;
                }

				res = string.Format("{0:" + FORMAT + "}" + suffix, kilos);
			}

			return res;
		}
    }
}