namespace TWDModel
{
	public class NotificationQueueItem
	{
		public enum Type
		{
			LaunchTutorial = 0,
			Building = 1,
			Survivor = 2,
			Walker = 3,
			Equipment = 4,
			GuildKickedOut = 5,
			GuildRequestAccepted = 6,
			GuildRequestRefused = 7,
			GuildPromoted = 8,
			GuildDemoted = 9,
			GuildPromotedLeader = 10,
			GuildPromotedLeaderByDemotion = 11,
			GuildDemotedDueInactivity = 12,
			GuildRemovedFromBattleSlot = 13
		}

		public Type NotificationType { get; set; }

		public int ModelId { get; set; }

		public string Name { get; set; }

		public int Level { get; set; }
	}
}
