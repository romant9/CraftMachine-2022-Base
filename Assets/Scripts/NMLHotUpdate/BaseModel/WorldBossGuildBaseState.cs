using System.Collections.Generic;

namespace BaseModel
{
	public class WorldBossGuildBaseState
	{
		public string GroupId { get; set; }

		public int SeasonId { get; set; }

		public int CycleId { get; set; }

		public WorldBossCycleStatus Status { get; set; }

		public List<string> SignedUpMemberIds { get; set; }

		public WorldBossGuildBaseState()
		{
			SignedUpMemberIds = new List<string>();
		}
	}
}
