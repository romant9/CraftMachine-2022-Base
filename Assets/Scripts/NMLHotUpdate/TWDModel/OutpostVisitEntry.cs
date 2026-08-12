namespace TWDModel
{
	public class OutpostVisitEntry
	{
		public OutpostVisitEntryType EntryType;

		public string OtherPlayerHashedId;

		public long UtcTime;

		public string OtherPlayerName;

		public int OtherPlayerLevel;

		public int OtherOutpostLevel;

		public int ResourcesStolen;

		public int RankingScoreChange;

		public long ProductionHaltedTime;

		public ECombatResult CombatResult;

		public int[] SurvivorLevels;

		public SurvivorClass[] SurvivorClasses;

		public int[] SurvivorRarityLevels;

		public bool[] SurvivorDefeated;

		public int[] OtherSurvivorLevels;

		public SurvivorClass[] OtherSurvivorClasses;

		public int[] OtherSurvivorRarityLevels;

		public bool[] OtherSurvivorDefeated;

		public PvPMissionType MissionType;

		public bool FirstObjectiveCompleted;

		public bool SecondObjectiveCompleted;

		public bool DefendersObjectiveCompleted;

		public string OutpostVisitId;

		public bool RequiresShield()
		{
			if (EntryType == OutpostVisitEntryType.Defended)
			{
				return AllSurvivorsDefeated(SurvivorDefeated);
			}
			return false;
		}

		private static bool AllSurvivorsDefeated(bool[] defeats)
		{
			bool result = false;
			for (int i = 0; i < ((defeats != null) ? defeats.Length : 0); i++)
			{
				if (!defeats[i])
				{
					result = false;
					break;
				}
				result = true;
			}
			return result;
		}
	}
}
