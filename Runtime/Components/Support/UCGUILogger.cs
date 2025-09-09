using UnityEngine;

namespace UCGUI
{
    public class UCGUILogger
    {
        private static readonly Color LoggingColor = Color.cyan;
        
        public static void Log(object log)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(LoggingColor)}>UCGUI:</color> " + log);
        }

        public static void LogWarning(string log)
        {
            Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGBA(LoggingColor)}>UCGUI Warning:</color> " + log);
        }
        
        
        public static void LogError(string log)
        {
            Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGBA(LoggingColor)}>UCGUI Error:</color> " + log);
        }
    }
}