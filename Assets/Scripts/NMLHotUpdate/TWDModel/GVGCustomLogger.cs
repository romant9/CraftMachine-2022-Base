using BaseModel;

namespace TWDModel
{
	public class GVGCustomLogger : ICustomLogger
	{
		private const string keyword = "GVGLogger";

		public const string ErrorKeyword = "Error";

		public const string LogKeyword = "Log";

		public const string WarningKeyword = "Warning";

		public string Keyword => "GVGLogger";

		public void Log(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null)
		{
			manager.Debug.LogInfo(FormatMessage(Keyword, "Log", message, debuData));
		}

		public void LogError(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null)
		{
			manager.Debug.LogError(FormatMessage(Keyword, "Error", message, debuData));
		}

		public void LogWarning(ModelManager manager, string message, ICustomLoggerDebugInfo debuData = null)
		{
			manager.Debug.LogWarning(FormatMessage(Keyword, "Warning", message, debuData));
		}

		public static string FormatMessage(string keyword, string type, string message, ICustomLoggerDebugInfo debuData)
		{
			string text = null;
			if (debuData != null)
			{
				text = debuData.GetDebugInfo();
				return $"[{keyword}] [{text}] [{type}: {message}]";
			}
			return $"[{keyword}] [{type}: {message}]";
		}
	}
}
