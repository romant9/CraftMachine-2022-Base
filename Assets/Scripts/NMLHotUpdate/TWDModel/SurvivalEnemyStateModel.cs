namespace TWDModel
{
	public class SurvivalEnemyStateModel
	{
		public bool IsWalker { get; set; }

		public int ActorTag { get; set; }

		public WalkerType WalkerType { get; set; }

		public SurvivorClass EnemySurvivorClass { get; set; }

		public int Count { get; set; }

		public SurvivalEnemyStateModel()
		{
		}

		public SurvivalEnemyStateModel(WalkerType walkerType, int actorTag, int count)
		{
			IsWalker = true;
			WalkerType = walkerType;
			EnemySurvivorClass = SurvivorClass.Scout;
			ActorTag = actorTag;
			Count = count;
		}

		public SurvivalEnemyStateModel(SurvivorClass raiderType, int actorTag, int count)
		{
			IsWalker = false;
			EnemySurvivorClass = raiderType;
			WalkerType = WalkerType.WalkerNormal;
			ActorTag = actorTag;
			Count = count;
		}

		public bool MatchesWalkerSpawnRequirement(WalkerType requiredWalkerType, int requiredActorTag)
		{
			if (!IsWalker)
			{
				return false;
			}
			if (WalkerType != requiredWalkerType)
			{
				return false;
			}
			if (ActorTag != requiredActorTag)
			{
				return false;
			}
			return true;
		}

		public bool MatchesRaiderSpawnRequirement(SurvivorClass requiredSurvivorClass, int requiredActorTag)
		{
			if (IsWalker)
			{
				return false;
			}
			if (EnemySurvivorClass != requiredSurvivorClass)
			{
				return false;
			}
			if (ActorTag != requiredActorTag)
			{
				return false;
			}
			return true;
		}
	}
}
