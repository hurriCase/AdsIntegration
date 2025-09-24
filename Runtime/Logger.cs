using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace AdsIntegration.Runtime
{
    internal static class Logger
    {
        [Conditional("ADS_LOG_ALL")]
        internal static void Log(string message)
        {
            Debug.Log(message);
        }

        [Conditional("ADS_LOG_ALL")]
        internal static void LogWarning(string message)
        {
            Debug.Log(message);
        }

        [Conditional("ADS_LOG_ALL")]
        internal static void LogError(string message)
        {
            Debug.Log(message);
        }
    }
}