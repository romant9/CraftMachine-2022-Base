using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class DebugTWD
{
	public static bool IsDebugBuild => OfflineManager.IsDebug && Application.isEditor;

	public static List<DebugItem> DebugItems = new List<DebugItem>();

	public static bool IsCollectDebugString => OfflineManager.IsCollectDebugString;

	public delegate void On_Change(DebugItem item);
	public static event On_Change On_Debug;

	public static List<DebugType> CurrentDebugTypes => CommandHelper.Instance != null ? CommandHelper.Instance.CurrentDebugTypes : new List<DebugType>() { DebugType.All };
	public static List<DebugType> LogUserTypesAll => CommandHelper.Instance != null ? CommandHelper.Instance.LogUserTypesAll : new List<DebugType>() { DebugType.All };

	//выбранные пользователем в logPopup
	public static List<DebugType> LogUserTypesSelected = new List<DebugType>() { DebugType.All };

	//Пример использования глобального класса
	//global::Debug.Log("MoveActor " + actor.Name, DebugType.Wars);

	public static void Log(object message, DebugType debugType = DebugType.All, UnityEngine.Object context = null)
	{
		if (!IsDebugBuild) return;
		if (context != null) message += " | script: " + context.name;
		if (IsCollectDebugString)
		{
			DebugItem.AddItem(debugType, message.ToString());
			On_Debug?.Invoke(DebugItems.Last());
		}

		if (!CurrentDebugTypes.Contains(DebugType.None) && (CurrentDebugTypes.Contains(DebugType.All) || CurrentDebugTypes.Contains(debugType)))
		{
			UnityEngine.Debug.Log(message + $" ({debugType})");
		}
	}

	public static void LogWarning(object message, DebugType debugType = DebugType.All, UnityEngine.Object context = null)
	{
		if (!IsDebugBuild) return;
		if (context != null) message += " | script: " + context.name;
		if (IsCollectDebugString)
		{
			DebugItem.AddItem(debugType, message.ToString());
			On_Debug?.Invoke(DebugItems.Last());
		}

		if (!CurrentDebugTypes.Contains(DebugType.None) && (CurrentDebugTypes.Contains(DebugType.All) || CurrentDebugTypes.Contains(debugType)))
		{
			UnityEngine.Debug.LogWarning(message + $" ({debugType})");
		}
	}

	public static void LogError(object message, DebugType debugType = DebugType.All, UnityEngine.Object context = null)
	{
		if (!IsDebugBuild) return;
		if (context != null) message += " | script: " + context.name;
		if (IsCollectDebugString)
		{
			DebugItem.AddItem(debugType, message.ToString());
			On_Debug?.Invoke(DebugItems.Last());
		}

		if (!CurrentDebugTypes.Contains(DebugType.None) && (CurrentDebugTypes.Contains(DebugType.All) || CurrentDebugTypes.Contains(debugType)))
		{
			UnityEngine.Debug.LogError(message + $" ({debugType})");
		}
	}

	public static void LogMycode(string message)
	{
		if (!IsDebugBuild) return;
		// Get the frame of the method that called MyLogger.Log
		StackFrame frame = new StackFrame(1);
		MethodBase method = frame.GetMethod();
		string className = method.DeclaringType.Name;
		string methodName = method.Name;
		Log($"Mycode. Class: {className}, Method: {methodName} - {message}", DebugType.Mycode);
	}

	[Conditional("DEBUG")]
	public static void LogFormat(string message, params object[] args)
	{
		UnityEngine.Debug.LogFormat(message, args);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(format, args);
	}

	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(context, format, args);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogErrorFormat(format, args);
	}

	public static void LogException(Exception exception, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogException(exception, context);
	}

	public static void Assert(bool condition, string message = "")
	{
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		UnityEngine.Debug.DrawLine(start, end, color);
	}

	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		UnityEngine.Debug.DrawRay(start, dir, color);
	}

	public static string DateTimeToTimeString(DateTime date)
	{
		return date.ToLocalTime().ToString(UserPrefsKeys.TimeFormat);
	}
}

public class DebugItem
{
	public DebugItem(DebugType type, string debugMessage)
	{
		DebugTime = DebugTWD.DateTimeToTimeString(GameManager.Instance != null && GameManager.Instance.playerModel != null ? GameManager.Instance.playerModel.UtcTime : DateTime.UtcNow);
		DebugMessage = debugMessage;
		DebugTypeLog = type;
	}

	public static void AddItem(DebugType type, string debugMessage)
	{
		if (DebugTWD.DebugItems.Count > 0 && DebugTWD.DebugItems.Last().DebugMessage == debugMessage)
		{
			DebugTWD.DebugItems.Last().Count++;
		}
		else
		{
			var item = new DebugItem(type, debugMessage);
			item.Count = 0;
			DebugTWD.DebugItems.Add(item);
		}
	}

	public DebugType DebugTypeLog { get; set; }
	public string DebugMessage { get; set; }
	public string DebugTime { get; set; }
	public int Count { get; set; }
}

public enum DebugType
{
	All,
	Origin, //Debug.Log - исходный класс Debug
	Action,
	Craft,
	Endless,
	Call,
	Wars,
	Challenge,
	Guild,
	Equipment,
	Currency,
	ActivateObject,
	Connection,
	System,
	OnClick,
	UI,
	BattleBase,
	BattleDamage,
	Load,
	Random,
	Error,
	Warning,
	Metrics,
	Cashier,
	SignalR,
	Analytics,
	Command,
	CommandError,
	Mycode,
	Supabase,
	None
}

