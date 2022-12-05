using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
namespace Dragon.SDK
{
    public class DebugCustom
    {
        private static bool _isLogBug = true;
        private static bool IsLogBug
        {
            get
            {
                if (!_isLogBug)
                    return false;
                return true;
            }
        }
        public static void LogWarning(params object[] content)
        {
#if UNITY_EDITOR
            if (!IsLogBug)
                return;
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += content[i].ToString();
                }
                else
                {
                    str += content[i].ToString() + "__";
                }
            }
            Debug.LogWarning(str);
#endif
        }
        public static void Log(params object[] content)
        {
#if UNITY_EDITOR
            if (!IsLogBug)
                return;
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += content[i].ToString();
                }
                else
                {
                    str += content[i].ToString() + "__";
                }
            }
            Debug.Log(str);
#endif
        }
#if UNITY_EDITOR
        public static string ReturnLog(params object[] content)
        {

            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += content[i].ToString();
                }
                else
                {
                    str += content[i].ToString() + "__";
                }
            }
            Debug.Log(str);
            return str;

        }
#endif
        public static void LogError(params object[] content)
        {
#if UNITY_EDITOR
            if (!IsLogBug)
                return;
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += content[i].ToString();
                }
                else
                {
                    str += content[i].ToString() + "__";
                }
            }
            Debug.LogError(str);
#endif
        }
        public static void LogColor(params object[] content)
        {
#if UNITY_EDITOR || GAME_ROCKET
            if (!IsLogBug)
                return;
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += content[i].ToString();
                }
                else
                {
                    str += content[i].ToString() + "__";
                }
            }
            Debug.Log("<color=\"" + "#ffa500ff" + "\">" + str + "</color>");
#endif
        }
        public static void LogColorJson(params object[] content)
        {
#if UNITY_EDITOR
            if (!IsLogBug)
                return;
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += JsonConvert.SerializeObject(content[i]);
                }
                else
                {
                    str += JsonConvert.SerializeObject(content[i]) + "__";
                }
            }
            Debug.Log("<color=\"" + "#ffa500ff" + "\">" + str + "</color>");
#endif
        }
        public static void LogJson(params object[] content)
        {
#if UNITY_EDITOR
            if (!IsLogBug)
                return;
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += JsonConvert.SerializeObject(content[i]);
                }
                else
                {
                    str += JsonConvert.SerializeObject(content[i]) + "__";
                }
            }
            Debug.Log(str);
#endif
        }
        public static void LogErrorJson(params object[] content)
        {
#if UNITY_EDITOR
            if (!IsLogBug)
                return;
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                if (i == content.Length - 1)
                {
                    str += JsonConvert.SerializeObject(content[i]);
                }
                else
                {
                    str += JsonConvert.SerializeObject(content[i]) + "__";
                }
            }
            Debug.LogError(str);
#endif
        }
    }
}
