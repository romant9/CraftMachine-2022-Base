using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public sealed class BuglyAgent
{
	public delegate void LogCallbackDelegate(string condition, string stackTrace, LogType type);

	private static bool _isInitialized = false;

	private static LogSeverity _autoReportLogLevel = LogSeverity.LogError;

	private static int _crashReporterType = 1;

	private static bool _debugMode = false;

	private static bool _autoQuitApplicationAfterReport = false;

	private static readonly int EXCEPTION_TYPE_UNCAUGHT = 1;

	private static readonly int EXCEPTION_TYPE_CAUGHT = 2;

	private static readonly string _pluginVersion = "1.5.1";

	private static Func<Dictionary<string, string>> _LogCallbackExtrasHandler;

	private static bool _uncaughtAutoReportOnce = false;

	public static string PluginVersion => _pluginVersion;

	public static bool IsInitialized => _isInitialized;

	public static bool AutoQuitApplicationAfterReport => _autoQuitApplicationAfterReport;

	private static event LogCallbackDelegate _LogCallbackEventHandler;

	public static void ConfigCrashReporter(int type, int logLevel)
	{
		_SetCrashReporterType(type);
		_SetCrashReporterLogLevel(logLevel);
	}

	public static void InitWithAppId(string appId)
	{
		if (IsInitialized)
		{
			DebugLog(null, "BuglyAgent has already been initialized.");
		}
		else if (!string.IsNullOrEmpty(appId))
		{
			InitBuglyAgent(appId);
			DebugLog(null, "Initialized with app id: {0}", appId);
			_RegisterExceptionHandler();
		}
	}

	public static void EnableExceptionHandler()
	{
		if (IsInitialized)
		{
			DebugLog(null, "BuglyAgent has already been initialized.");
			return;
		}
		DebugLog(null, "Only enable the exception handler, please make sure you has initialized the sdk in the native code in associated Android or iOS project.");
		_RegisterExceptionHandler();
	}

	public static void RegisterLogCallback(LogCallbackDelegate handler)
	{
		if (handler != null)
		{
			DebugLog(null, "Add log callback handler: {0}", handler);
			_LogCallbackEventHandler += handler;
		}
	}

	public static void SetLogCallbackExtrasHandler(Func<Dictionary<string, string>> handler)
	{
		if (handler != null)
		{
			_LogCallbackExtrasHandler = handler;
			DebugLog(null, "Add log callback extra data handler : {0}", handler);
		}
	}

	public static void ReportException(Exception e, string message)
	{
		if (IsInitialized)
		{
			DebugLog(null, "Report exception: {0}\n------------\n{1}\n------------", message, e);
			_HandleException(e, message, uncaught: false);
		}
	}

	public static void ReportException(string name, string message, string stackTrace)
	{
		if (IsInitialized)
		{
			DebugLog(null, "Report exception: {0} {1} \n{2}", name, message, stackTrace);
			_HandleException(LogSeverity.LogException, name, message, stackTrace, uncaught: false);
		}
	}

	public static void UnregisterLogCallback(LogCallbackDelegate handler)
	{
		if (handler != null)
		{
			DebugLog(null, "Remove log callback handler");
			_LogCallbackEventHandler -= handler;
		}
	}

	public static void SetUserId(string userId)
	{
		if (IsInitialized)
		{
			DebugLog(null, "Set user id: {0}", userId);
			SetUserInfo(userId);
		}
	}

	public static void SetScene(int sceneId)
	{
		if (IsInitialized)
		{
			DebugLog(null, "Set scene: {0}", sceneId);
			SetCurrentScene(sceneId);
		}
	}

	public static void AddSceneData(string key, string value)
	{
		if (IsInitialized)
		{
			DebugLog(null, "Add scene data: [{0}, {1}]", key, value);
			AddKeyAndValueInScene(key, value);
		}
	}

	public static void ConfigDebugMode(bool enable)
	{
		EnableDebugMode(enable);
		DebugLog(null, "{0} the log message print to console", enable ? "Enable" : "Disable");
	}

	public static void ConfigAutoQuitApplication(bool autoQuit)
	{
		_autoQuitApplicationAfterReport = autoQuit;
	}

	public static void ConfigAutoReportLogLevel(LogSeverity level)
	{
		_autoReportLogLevel = level;
	}

	public static void ConfigDefault(string channel, string version, string user, long delay)
	{
		DebugLog(null, "Config default channel:{0}, version:{1}, user:{2}, delay:{3}", channel, version, user, delay);
		ConfigDefaultBeforeInit(channel, version, user, delay);
	}

	public static void DebugLog(string tag, string format, params object[] args)
	{
		if (_debugMode && !string.IsNullOrEmpty(format))
		{
			Console.WriteLine("[BuglyAgent] <Debug> - {0} : {1}", tag, string.Format(format, args));
		}
	}

	public static void PrintLog(LogSeverity level, string format, params object[] args)
	{
		if (!string.IsNullOrEmpty(format))
		{
			LogRecord(level, string.Format(format, args));
		}
	}

	private static void InitBuglyAgent(string appId)
	{
	}

	private static void ConfigDefaultBeforeInit(string channel, string version, string user, long delay)
	{
	}

	private static void EnableDebugMode(bool enable)
	{
	}

	private static void SetUserInfo(string userInfo)
	{
	}

	private static void ReportException(int type, string name, string message, string stackTrace, bool quitProgram)
	{
	}

	private static void SetCurrentScene(int sceneId)
	{
	}

	private static void AddKeyAndValueInScene(string key, string value)
	{
	}

	private static void AddExtraDataWithException(string key, string value)
	{
	}

	private static void LogRecord(LogSeverity level, string message)
	{
	}

	private static void SetUnityVersion()
	{
	}

	private static void _SetCrashReporterType(int type)
	{
		_crashReporterType = type;
		_ = _crashReporterType;
		_ = 2;
	}

	private static void _SetCrashReporterLogLevel(int logLevel)
	{
	}

	private static void _RegisterExceptionHandler()
	{
		try
		{
			Application.RegisterLogCallback(_OnLogCallbackHandler);
			AppDomain.CurrentDomain.UnhandledException += _OnUncaughtExceptionHandler;
			_isInitialized = true;
			DebugLog(null, "Register the log callback in Unity {0}", Application.unityVersion);
		}
		catch
		{
		}
		SetUnityVersion();
	}

	private static void _UnregisterExceptionHandler()
	{
		try
		{
			Application.RegisterLogCallback(null);
			AppDomain.CurrentDomain.UnhandledException -= _OnUncaughtExceptionHandler;
			DebugLog(null, "Unregister the log callback in unity {0}", Application.unityVersion);
		}
		catch
		{
		}
	}

	private static void _OnLogCallbackHandler(string condition, string stackTrace, LogType type)
	{
		if (BuglyAgent._LogCallbackEventHandler != null)
		{
			BuglyAgent._LogCallbackEventHandler(condition, stackTrace, type);
		}
		if (IsInitialized && (string.IsNullOrEmpty(condition) || !condition.Contains("[BuglyAgent] <Log>")) && !_uncaughtAutoReportOnce)
		{
			LogSeverity logSeverity = LogSeverity.Log;
			switch (type)
			{
			case LogType.Exception:
				logSeverity = LogSeverity.LogException;
				break;
			case LogType.Error:
				logSeverity = LogSeverity.LogError;
				break;
			case LogType.Assert:
				logSeverity = LogSeverity.LogAssert;
				break;
			case LogType.Warning:
				logSeverity = LogSeverity.LogWarning;
				break;
			case LogType.Log:
				logSeverity = LogSeverity.LogDebug;
				break;
			}
			if (logSeverity != LogSeverity.Log)
			{
				_HandleException(logSeverity, null, condition, stackTrace, uncaught: true);
			}
		}
	}

	private static void _OnUncaughtExceptionHandler(object sender, UnhandledExceptionEventArgs args)
	{
		if (args == null || args.ExceptionObject == null)
		{
			return;
		}
		try
		{
			if (args.ExceptionObject.GetType() != typeof(Exception))
			{
				return;
			}
		}
		catch
		{
			if (UnityEngine.Debug.isDebugBuild)
			{
				UnityEngine.Debug.Log("BuglyAgent: Failed to report uncaught exception");
			}
			return;
		}
		if (IsInitialized && !_uncaughtAutoReportOnce)
		{
			_HandleException((Exception)args.ExceptionObject, null, uncaught: true);
		}
	}

	private static void _HandleException(Exception e, string message, bool uncaught)
	{
		if (e == null || !IsInitialized)
		{
			return;
		}
		string name = e.GetType().Name;
		string text = e.Message;
		if (!string.IsNullOrEmpty(message))
		{
			text = $"{text}{Environment.NewLine}***{message}";
		}
		StringBuilder stringBuilder = new StringBuilder("");
		StackTrace stackTrace = new StackTrace(e, fNeedFileInfo: true);
		int frameCount = stackTrace.FrameCount;
		for (int i = 0; i < frameCount; i++)
		{
			StackFrame frame = stackTrace.GetFrame(i);
			stringBuilder.AppendFormat("{0}.{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);
			ParameterInfo[] parameters = frame.GetMethod().GetParameters();
			if (parameters == null || parameters.Length == 0)
			{
				stringBuilder.Append(" () ");
			}
			else
			{
				stringBuilder.Append(" (");
				int num = parameters.Length;
				ParameterInfo parameterInfo = null;
				for (int j = 0; j < num; j++)
				{
					parameterInfo = parameters[j];
					stringBuilder.AppendFormat("{0} {1}", parameterInfo.ParameterType.Name, parameterInfo.Name);
					if (j != num - 1)
					{
						stringBuilder.Append(", ");
					}
				}
				parameterInfo = null;
				stringBuilder.Append(") ");
			}
			string fileName = frame.GetFileName();
			if (!string.IsNullOrEmpty(fileName) && !fileName.ToLower().Equals("unknown"))
			{
				fileName = fileName.Replace("\\", "/");
				int num2 = fileName.ToLower().IndexOf("/assets/");
				if (num2 < 0)
				{
					num2 = fileName.ToLower().IndexOf("assets/");
				}
				if (num2 > 0)
				{
					fileName = fileName.Substring(num2);
				}
				stringBuilder.AppendFormat("(at {0}:{1})", fileName, frame.GetFileLineNumber());
			}
			stringBuilder.AppendLine();
		}
		_reportException(uncaught, name, text, stringBuilder.ToString());
	}

	private static void _reportException(bool uncaught, string name, string reason, string stackTrace)
	{
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		if (string.IsNullOrEmpty(stackTrace))
		{
			stackTrace = StackTraceUtility.ExtractStackTrace();
		}
		if (string.IsNullOrEmpty(stackTrace))
		{
			stackTrace = "Empty";
		}
		else
		{
			try
			{
				string[] array = stackTrace.Split('\n');
				if (array != null && array.Length != 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					string text = null;
					int num = array.Length;
					for (int i = 0; i < num; i++)
					{
						text = array[i];
						if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.Trim()))
						{
							continue;
						}
						text = text.Trim();
						if (!text.StartsWith("System.Collections.Generic.") && !text.StartsWith("ShimEnumerator") && !text.StartsWith("Bugly") && !text.Contains("..ctor"))
						{
							int num2 = text.ToLower().IndexOf("(at");
							int num3 = text.ToLower().IndexOf("/assets/");
							if (num2 > 0 && num3 > 0)
							{
								stringBuilder.AppendFormat("{0}(at {1}", text.Substring(0, num2).Replace(":", "."), text.Substring(num3));
							}
							else
							{
								stringBuilder.Append(text.Replace(":", "."));
							}
							stringBuilder.AppendLine();
						}
					}
					stackTrace = stringBuilder.ToString();
				}
			}
			catch
			{
				PrintLog(LogSeverity.LogWarning, "{0}", "Error to parse the stack trace");
			}
		}
		PrintLog(LogSeverity.LogError, "ReportException: {0} {1}\n*********\n{2}\n*********", name, reason, stackTrace);
		_uncaughtAutoReportOnce = uncaught && _autoQuitApplicationAfterReport;
		ReportException(uncaught ? EXCEPTION_TYPE_UNCAUGHT : EXCEPTION_TYPE_CAUGHT, name, reason, stackTrace, uncaught && _autoQuitApplicationAfterReport);
	}

	private static void _HandleException(LogSeverity logLevel, string name, string message, string stackTrace, bool uncaught)
	{
		if (!IsInitialized)
		{
			DebugLog(null, "It has not been initialized.");
		}
		else
		{
			if (logLevel == LogSeverity.Log)
			{
				return;
			}
			if (uncaught && logLevel < _autoReportLogLevel)
			{
				DebugLog(null, "Not report exception for level {0}", logLevel.ToString());
				return;
			}
			string text = null;
			string text2 = null;
			if (!string.IsNullOrEmpty(message))
			{
				try
				{
					if (LogSeverity.LogException == logLevel && message.Contains("Exception"))
					{
						Match match = new Regex("^(?<errorType>\\S+):\\s*(?<errorMessage>.*)", RegexOptions.Singleline).Match(message);
						if (match.Success)
						{
							text = match.Groups["errorType"].Value.Trim();
							text2 = match.Groups["errorMessage"].Value.Trim();
						}
					}
					else if (LogSeverity.LogError == logLevel && message.StartsWith("Unhandled Exception:"))
					{
						Match match2 = new Regex("^Unhandled\\s+Exception:\\s*(?<exceptionName>\\S+):\\s*(?<exceptionDetail>.*)", RegexOptions.Singleline).Match(message);
						if (match2.Success)
						{
							string text3 = match2.Groups["exceptionName"].Value.Trim();
							string text4 = match2.Groups["exceptionDetail"].Value.Trim();
							int num = text3.LastIndexOf(".");
							text = ((num <= 0 || num == text3.Length) ? text3 : text3.Substring(num + 1));
							int num2 = text4.IndexOf(" at ");
							if (num2 > 0)
							{
								text2 = text4.Substring(0, num2);
								string text5 = text4.Substring(num2 + 3).Replace(" at ", "\n").Replace("in <filename unknown>:0", "")
									.Replace("[0x00000]", "");
								stackTrace = $"{stackTrace}\n{text5.Trim()}";
							}
							else
							{
								text2 = text4;
							}
							if (text.Equals("LuaScriptException") && text4.Contains(".lua") && text4.Contains("stack traceback:"))
							{
								num2 = text4.IndexOf("stack traceback:");
								if (num2 > 0)
								{
									text2 = text4.Substring(0, num2);
									string text6 = text4.Substring(num2 + 16).Replace(" [", " \n[");
									stackTrace = $"{stackTrace}\n{text6.Trim()}";
								}
							}
						}
					}
				}
				catch
				{
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = message;
				}
			}
			if (string.IsNullOrEmpty(name))
			{
				if (string.IsNullOrEmpty(text))
				{
					text = $"Unity{logLevel.ToString()}";
				}
			}
			else
			{
				text = name;
			}
			_reportException(uncaught, text, text2, stackTrace);
		}
	}
}
