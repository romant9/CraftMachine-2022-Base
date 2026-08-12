using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleSectorDefinition
	{
		public int Id;

		public string MapConfigId;

		public string PrerequisitesIdsString;

		public int PVPEnemyAmount;

		public int MissionAmountPerPVPEnemy;

		public string MissionPoolName;

		public string MissionConfigPoolName;

		public string PVEModifier;

		public string PVPModifier;

		public int SectorVP;

		[JsonIgnore]
		public int[] ColumnsDifficulty;

		[JsonIgnore]
		public int[] PVPModifierPerArea;

		[JsonIgnore]
		public bool AllPrerequisitesMustBeCompleted;

		[JsonIgnore]
		public GvgMapIconConfig MapIconConfig;

		[JsonIgnore]
		public int[] PrerequisitesSectorIds;
	}
}
