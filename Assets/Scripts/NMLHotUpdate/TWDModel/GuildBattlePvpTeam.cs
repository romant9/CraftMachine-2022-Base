using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildBattlePvpTeam
	{
		public const int MaxTeamSize = 3;

		public List<SurvivorMockData> Survivors;

		//[JsonIgnore]
		public string MissionId = "";

		//[JsonIgnore]
		public int AverageAdjustedLevel
		{
			get
			{
				List<SurvivorMockData> survivors = Survivors;
				if (survivors == null)
				{
					return -1;
				}
				return survivors.Sum((SurvivorMockData x) => x.AdjustedLevel) / 3;
			}
		}

		public string OwnerHashedPlayerId => Survivors[0].OwnerHashedPlayerId;

		public GuildBattlePvpTeam(List<SurvivorMockData> survivors)
		{
			Survivors = survivors;
		}

		private bool IsSurvivorInTeam(SurvivorMockData survivorMockData)
		{
			return Survivors?.Contains(survivorMockData) ?? false;
		}

		public override string ToString()
		{
			return "Team Details: \nOwner : " + Survivors[0].OwnerHashedPlayerId + "\nTeam Actor Definition Ids : " + Survivors[0].ActorDefinitionId + ", " + Survivors[1].ActorDefinitionId + ", " + Survivors[2].ActorDefinitionId;
		}
	}
}
