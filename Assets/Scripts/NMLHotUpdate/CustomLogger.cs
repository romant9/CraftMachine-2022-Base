using UnityEngine;

public class CustomLogger : MonoBehaviour
{
	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void HandleLog(string message, string stackTrace, LogType type)
	{
	}

	private static bool IsDebuggerAttached()
	{
		return false;
	}
}
