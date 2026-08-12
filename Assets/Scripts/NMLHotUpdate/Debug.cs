using System;
using System.Diagnostics;
using UnityEngine;

public static class Debug
{
	public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

	[Conditional("DEBUG")]
	public static void Log(object message, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.Log(message, context);
	}

	public static void Log(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("DEBUG")]
	public static void LogFormat(string message, params object[] args)
	{
		UnityEngine.Debug.LogFormat(message, args);
	}

	public static void LogWarning(object message, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}

	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(format, args);
	}

	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(context, format, args);
	}

	public static void LogError(object message, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogError(message, context);
	}

	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
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
}
