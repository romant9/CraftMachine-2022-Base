using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleMissionPoolDefinition
	{
		public string PoolName;

		public string MissionId;

		[JsonIgnore]
		public int OrderNumber;
	}
}
