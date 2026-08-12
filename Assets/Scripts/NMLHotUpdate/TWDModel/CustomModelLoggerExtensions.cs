using BaseModel;

namespace TWDModel
{
	public static class CustomModelLoggerExtensions
	{
		public static void GvGLog(this ModelManager manager, string message, ICustomLoggerDebugInfo debugData = null)
		{
			CustomModelLogger<GVGCustomLogger>.Log(manager, message, debugData);
		}

		public static void GvGLogError(this ModelManager manager, string message, ICustomLoggerDebugInfo debugData = null)
		{
			CustomModelLogger<GVGCustomLogger>.LogError(manager, message, debugData);
		}

		public static void GvGLogWarning(this ModelManager manager, string message, ICustomLoggerDebugInfo debugData = null)
		{
			CustomModelLogger<GVGCustomLogger>.LogWarning(manager, message, debugData);
		}
	}
}
