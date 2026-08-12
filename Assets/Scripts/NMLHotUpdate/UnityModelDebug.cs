using BaseModel;

public class UnityModelDebug : IModelDebug
{
	public void Log(string message, DebugType debugType = DebugType.All)
	{
		LogDebug(message, debugType);
	}

	public void LogDebug(string message, DebugType debugType = DebugType.All)
	{
		DebugTWD.Log(message, debugType);
	}

	public void LogInfo(string message, DebugType debugType = DebugType.All)
	{
	}

	public void LogWarning(string message, DebugType debugType = DebugType.All)
	{
		DebugTWD.LogWarning(message, debugType);
	}

	public void LogError(string message, DebugType debugType = DebugType.All)
	{
		DebugTWD.LogError(message, debugType);
	}

	public void Log(string message)
	{
		LogDebug(message);
	}

	public void LogDebug(string message)
	{
	}

	public void LogInfo(string message)
	{
	}

	public void LogWarning(string message)
	{
		DebugTWD.LogWarning(message);
	}

	public void LogError(string message)
	{
		DebugTWD.LogError(message);
	}
}
