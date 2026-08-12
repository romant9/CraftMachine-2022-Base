namespace TWDModel
{
	public struct MapMissionParameters
	{
		public string MissionId;

		public string MissionFlavor;

		public int MissionLevel;

		public int RandomSeed;

		public int MissionSectorId;

		public bool IsDeadly;

		public bool IsSurvival;

		public bool IsPvP;

		public GuildBattleMapMissionModel.MissionState GuildBattleState;

		public DropEventDefinition.DropEventTag LootTag;
	}
}
