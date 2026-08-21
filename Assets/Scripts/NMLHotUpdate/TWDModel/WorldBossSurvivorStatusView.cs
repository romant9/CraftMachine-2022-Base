namespace TWDModel
{
	public class WorldBossSurvivorStatusView
	{
		public bool IsDispatched { get; set; }

		public string DispatchedCapturePoint { get; set; } = string.Empty;

		public bool IsReturning { get; set; }

		public long ReturnRemainingMs { get; set; }
	}
}
