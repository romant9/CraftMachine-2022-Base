using System;

namespace Singular
{
	internal class SingularUnityLogger
	{
		public enum LogLevel
		{
			Verbose = 2,
			Debug = 3,
			Info = 4,
			Warn = 5,
			Error = 6,
			Assert = 7
		}

		private static bool _enableLogging;

		private static LogLevel _logLevel;

		private const string LogTag = "[SingularLog]";

		public static void SetLogLevel(int level)
		{
			if (Enum.IsDefined(typeof(LogLevel), level))
			{
				_logLevel = (LogLevel)level;
				return;
			}
			Debug.Log("invalid log level value - fallback to level = Debug.");
			_logLevel = LogLevel.Debug;
		}

		public static void EnableLogging(bool enable)
		{
			_enableLogging = enable;
		}

		public static void LogVerbose(string message)
		{
			TryLog(message, LogLevel.Verbose, Debug.Log);
		}

		public static void LogDebug(string message)
		{
			TryLog(message, LogLevel.Debug, Debug.Log);
		}

		public static void LogInfo(string message)
		{
			TryLog(message, LogLevel.Info, Debug.Log);
		}

		public static void LogWarn(string message)
		{
			TryLog(message, LogLevel.Warn, Debug.LogWarning);
		}

		public static void LogError(string message)
		{
			TryLog(message, LogLevel.Error, Debug.LogError);
		}

		public static void LogAssert(string message)
		{
			TryLog(message, LogLevel.Assert, Debug.LogError);
		}

		private static void TryLog(string message, LogLevel level, Action<string> logAction)
		{
			try
			{
				if (_enableLogging && _logLevel <= level)
				{
					logAction("[SingularLog]: " + message);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}