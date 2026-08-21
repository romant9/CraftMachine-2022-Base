using System.Collections.Generic;

namespace BaseModel
{
	public class WorldBossReturningTeamModel
	{
		public string Id { get; set; }

		public string PlayerHashedId { get; set; }

		public List<string> SurvivorIds { get; set; }

		public long StartUtcMs { get; set; }

		public long ReturnEndUtcMs { get; set; }

		public WorldBossReturningTeamModel()
		{
			SurvivorIds = new List<string>();
		}
	}
}
