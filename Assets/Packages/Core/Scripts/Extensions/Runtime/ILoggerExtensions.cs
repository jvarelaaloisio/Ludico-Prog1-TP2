using UnityEngine;

namespace VarelaAloisio.Core.Runtime
{
    public static class LoggerRefExtensions
    {
        [HideInCallstack]
        public static void Log(this Ref<ILogger> logger, string tag, object message, Object context = null)
            => (logger?.HasValue ?? false ? logger.Value : Debug.unityLogger).Log(LogType.Log, tag, message, context);
        [HideInCallstack]
        public static void LogWarning(this Ref<ILogger> logger, string tag, object message, Object context = null)
            => (logger?.HasValue ?? false ? logger.Value : Debug.unityLogger).Log(LogType.Warning, tag, message, context);

        [HideInCallstack]
        public static void LogError(this Ref<ILogger> logger, string tag, object message, Object context = null)
            => (logger?.HasValue ?? false ? logger.Value : Debug.unityLogger).Log(LogType.Error, tag, message, context);

        [HideInCallstack]
        public static void LogException(this Ref<ILogger> logger, string tag, System.Exception exception, Object context = null)
            => (logger?.HasValue ?? false ? logger.Value : Debug.unityLogger).Log(LogType.Exception, tag, exception, context);
    }
}
