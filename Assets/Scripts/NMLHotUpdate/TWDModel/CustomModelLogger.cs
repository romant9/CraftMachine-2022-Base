using BaseModel;

namespace TWDModel
{
	public static class CustomModelLogger<T> where T : ICustomLogger, new()
	{
		private static ICustomLogger _logger;

		public static ICustomLogger Logger
		{
			get
			{
				if (_logger == null)
				{
					_logger = new T();
				}
				return _logger;
			}
		}

		public static void Log(ModelManager manager, string message, ICustomLoggerDebugInfo debugData = null)
		{
			Logger.Log(manager, message, debugData);
		}

		public static void LogError(ModelManager manager, string message, ICustomLoggerDebugInfo debugData = null)
		{
			Logger.LogError(manager, message, debugData);
		}

		public static void LogWarning(ModelManager manager, string message, ICustomLoggerDebugInfo debugData = null)
		{
			Logger.LogWarning(manager, message, debugData);
		}
	}
}
