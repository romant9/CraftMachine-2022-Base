using BaseModel;

namespace TWDModel
{
	public class GVGGroupModelChildCustomLogger : ICustomLogger
	{
		private const string keyword = "GVGLogger group model";

		public string Keyword => "GVGLogger group model";

		public void Log(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null)
		{
		}

		public void LogError(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null)
		{
		}

		public void LogWarning(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null)
		{
		}
	}
}
