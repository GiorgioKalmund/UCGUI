using Unity.ProjectAuditor.Editor;
using UnityEngine;

namespace UCGUI
{
    public class UCGUILogger
    {
        private static readonly Color LoggingColor = Color.cyan;
        
        private static void LogInternal(LogLevel level, string logMessage, Object context)
        {
            
            if (!Defaults.Debug.DoLogging)
                return;
            
            var logPrefix = $"<color=#{ColorUtility.ToHtmlStringRGBA(LoggingColor)}>Dora {level}</color>: ";
            var log = logPrefix + logMessage;
            switch (level)
            {
                case LogLevel.Error: Debug.LogError(log, context); break;
                case LogLevel.Warning : Debug.LogWarning(log, context); break;
                case LogLevel.Info : Debug.Log(log, context); break;
            }
        }
        private static void LogInternal(LogLevel level, string logMessage) => LogInternal(level, logMessage, null);

        public static void Log(string message, Object context) => LogInternal(LogLevel.Info, message, context);
        public static void Log(string message) => LogInternal(LogLevel.Info, message);
        
        public static void LogWarning(string message, Object context) => LogInternal(LogLevel.Warning, message, context);
        public static void LogWarning(string message) => LogInternal(LogLevel.Warning, message);
        
        public static void LogError(string message, Object context) => LogInternal(LogLevel.Error, message, context);
        public static void LogError(string message) => LogInternal(LogLevel.Error, message);
    }
}