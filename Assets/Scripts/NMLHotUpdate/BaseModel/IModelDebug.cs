namespace BaseModel
{
	public interface IModelDebug
	{
		void Log(string message);

		void LogDebug(string message);

		void LogInfo(string message);

		void LogWarning(string message);

		void LogError(string message);

		void Log(string message, DebugType debugType);

		void LogDebug(string message, DebugType debugType);

		void LogInfo(string message, DebugType debugType);

		void LogWarning(string message, DebugType debugType);

		void LogError(string message, DebugType debugType);
	}
}
