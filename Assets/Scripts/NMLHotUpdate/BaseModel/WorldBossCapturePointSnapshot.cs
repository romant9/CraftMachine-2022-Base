using System.Collections.Generic;

namespace BaseModel
{
	public class WorldBossCapturePointSnapshot
	{
		public string CapturePoint { get; set; }

		public string CapturePointType { get; set; }

		public List<WorldBossCellStateSnapshot> CellStates { get; set; }

		public WorldBossCapturePointStateModel OwnershipState { get; set; }

		public List<WorldBossReturningTeamModel> ReturningTeams { get; set; }

		public List<WorldBossCellDefenderSnapshot> Defenders { get; set; }

		public string GroupId { get; set; }

		public WorldBossCapturePointSnapshot()
		{
			CellStates = new List<WorldBossCellStateSnapshot>();
		}
	}
}
