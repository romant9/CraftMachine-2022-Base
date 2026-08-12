using BaseModel;

namespace TWDModel
{
	public interface ICustomLogger
	{
		string Keyword { get; }

		void Log(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null);

		void LogError(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null);

		void LogWarning(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null);
	}
}
