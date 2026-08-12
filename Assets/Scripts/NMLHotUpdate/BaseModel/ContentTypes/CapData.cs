namespace BaseModel.ContentTypes
{
	public sealed class CapData
	{
		public int GameplayDuration { get; set; }

		public int TheaterSessionLength { get; set; }

		public int TheaterSessionCap { get; set; }

		public int AfterMissionSessionLength { get; set; }

		public int AfterMissionSessionCap { get; set; }

		public int BuildingUpgradeSessionCap { get; set; }

		public int BuildingUpgradeSessionLength { get; set; }

		public int BlackMarketRefreshSessionCap { get; set; }

		public int BlackMarketRefreshSessionLength { get; set; }
	}
}
