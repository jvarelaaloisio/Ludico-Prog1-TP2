using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VarelaAloisio.Core.Debugging
{
	[CreateAssetMenu(menuName = "Debug/Debugger", fileName = "Debugger", order = 0)]
	public class Debugger : ScriptableObject, ILogger, IGizmoDrawer
	{
		[Header("Logging")]
		[field:SerializeField]
		public bool logEnabled { get; set; } = true;

		[SerializeField]
		private bool logNames = false;

		[Space]
		[SerializeField]
		private bool allowLog = true;

		[SerializeField]
		private bool allowWarning = true;

		[SerializeField]
		private bool allowError = true;

		[SerializeField]
		private bool allowAssert = true;

		[SerializeField]
		private bool allowException = true;

		[Space]
		[Header("Drawing")]
		[SerializeField]
		private bool drawLines = true;

		[SerializeField]
		private bool drawRays = true;

		[Space]
		[SerializeField]
		[Tooltip("Tags to be excluded in logging")]
		private List<string> filteredTags;
		[SerializeField] private Color tagColor = Color.black;

		private ILogger _logger;
		public ILogger Logger => _logger ??= Debug.unityLogger;

		private Dictionary<LogType, bool> _logTypesAllowed;
		[SerializeField] private string tagFormat = "<color=black>{0}: </color>";

		public ILogHandler logHandler { get; set; }

		//TODO: This only works in a DLL
		private static string CurrentClass
		{
			get
			{
				var st = new System.Diagnostics.StackTrace();

				var index = Mathf.Min(st.FrameCount - 1, 3);

				if (index < 0)
					return "{NoClass}";

				return "{" + st.GetFrame(index).GetMethod().DeclaringType.Name + "}";
			}
		}

	#region ILogger

		//TODO: Find out what this does
		public LogType filterLogType { get; set; }

		private void OnValidate()
			=> SetupAllowedLogsDictionary();

		private void Awake()
			=> SetupAllowedLogsDictionary();

		private void SetupAllowedLogsDictionary()
		{
			_logTypesAllowed = new Dictionary<LogType, bool>()
								{
									{LogType.Log, allowLog},
									{LogType.Warning, allowWarning},
									{LogType.Error, allowError},
									{LogType.Assert, allowAssert},
									{LogType.Exception, allowException},
								};
		}

		public bool IsLogTypeAllowed(LogType logType)
			=> _logTypesAllowed[logType];

		[HideInCallstack]
		public void Log(LogType logType, string tag, object message, Object context)
			=> LogInternal(logType, tag, message, context);

		public void Log(string message)
			=> Log(LogType.Log, string.Empty, message);

		public void Log(object message)
			=> Log(LogType.Log, string.Empty, message);

		public void Log(string tag, object message)
			=> LogInternal(LogType.Log, tag, message);

		public void Log(string tag, object message, Object context)
			=> LogInternal(LogType.Log, tag, message, context);

		public void LogWarning(string tag, object message)
			=> LogInternal(LogType.Warning, tag, message);

		public void LogWarning(string tag, object message, Object context)
			=> LogInternal(LogType.Warning, tag, message, context);

		public void LogError(string tag, object message)
			=> Log(LogType.Error, tag, message);

		public void LogError(string tag, object message, Object context)
			=> LogInternal(LogType.Error, tag, message, context);

		public void Log(LogType logType, object message)
			=> LogInternal(logType, string.Empty, message);

		public void Log(LogType logType, string tag, object message)
			=> LogInternal(logType, tag, message);

		public void Log(LogType logType, object message, Object context)
			=> LogInternal(logType, string.Empty, message, context);

		//TODO: Try to get it to work with logInternal
		public void LogFormat(LogType logType, Object context, string format, params object[] args)
			=> Logger.LogFormat(logType, context, format, args);

		//TODO: Try to get it to work with logInternal
		[HideInCallstack]
		public void LogFormat(LogType logType, string format, params object[] args)
			=> Logger.LogFormat(logType, format, args);

		//TODO: Try to get it to work with logInternal
		public void LogException(Exception exception, Object context)
			=> Logger.LogException(exception, context);

		public void LogException(Exception exception)
			=> Logger.LogException(exception);

		[HideInCallstack]
		private void LogInternal(LogType logType, string tag, object message, Object context = null)
		{
			if (!logEnabled || !IsLogTypeAllowed(logType) || filteredTags.Contains(tag))
				return;
			var formattedLog = new StringBuilder();

			if (!string.IsNullOrEmpty(tag))
				tag = string.Format(tagFormat, tag);

			if (logNames && context)
				formattedLog.Append(context.name + ": ");
			formattedLog.Append(message);

			Logger.Log(logType, tag, formattedLog, context);
		}

		#endregion

		#region Draws

		public void DrawLine(string tag, Vector3 start, Vector3 end)
		{
			DrawLine(tag, start, end, Color.white);
		}

		public void DrawLine(string tag, Vector3 start, Vector3 end, Color color)
		{
			DrawLine(tag, start, end, color, 0);
		}

		public void DrawLine(string tag, Vector3 start, Vector3 end, Color color, float duration)
		{
			if (!drawLines || filteredTags.Contains(tag))
				return;
			Debug.DrawLine(start, end, color, duration);
		}

		public void DrawRay(string tag, Vector3 start, Vector3 dir)
		{
			DrawRay(tag, start, dir, Color.white);
		}

		public void DrawRay(string tag, Vector3 start, Vector3 dir, Color color)
		{
			DrawRay(tag, start, dir, color, 0);
		}

		public void DrawRay(string tag, Vector3 start, Vector3 dir, Color color, float duration)
		{
			if (!drawRays || filteredTags.Contains(tag))
				return;
			Debug.DrawRay(start, dir, color, duration);
		}

		#endregion
	}
}