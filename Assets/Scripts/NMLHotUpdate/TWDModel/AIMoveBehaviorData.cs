using System;

namespace TWDModel
{
	[Serializable]
	public class AIMoveBehaviorData
	{
		public Faction Faction;

		public AIMode AIMode;

		public SurvivorClass Class;

		public FixedPoint ActorTargetEnemiesMultiplier;

		public FixedPoint ActorHitEnemiestMultiplier;

		public FixedPoint EnemiesTargetActorMultiplier;

		public FixedPoint ExploreMultiplier;

		public FixedPoint CurrentTargetMultiplier;

		public FixedPoint DistanceMultiplier;

		public FixedPoint DistanceToTargetMultiplier;

		public FixedPoint CoverBaseValue;

		public FixedPoint CoverMultiplier;

		public static AIMoveBehaviorData operator +(AIMoveBehaviorData a, AIMoveBehaviorData b)
		{
			return new AIMoveBehaviorData
			{
				Faction = a.Faction,
				AIMode = a.AIMode,
				Class = a.Class,
				ActorTargetEnemiesMultiplier = a.ActorTargetEnemiesMultiplier + b.ActorTargetEnemiesMultiplier,
				ActorHitEnemiestMultiplier = a.ActorHitEnemiestMultiplier + b.ActorHitEnemiestMultiplier,
				EnemiesTargetActorMultiplier = a.EnemiesTargetActorMultiplier + b.EnemiesTargetActorMultiplier,
				ExploreMultiplier = a.ExploreMultiplier + b.ExploreMultiplier,
				CurrentTargetMultiplier = a.CurrentTargetMultiplier + b.CurrentTargetMultiplier,
				DistanceMultiplier = a.DistanceMultiplier + b.DistanceMultiplier,
				DistanceToTargetMultiplier = a.DistanceToTargetMultiplier + b.DistanceToTargetMultiplier,
				CoverBaseValue = a.CoverBaseValue + b.CoverBaseValue,
				CoverMultiplier = a.CoverMultiplier + b.CoverMultiplier
			};
		}

		public static AIMoveBehaviorData operator -(AIMoveBehaviorData a, AIMoveBehaviorData b)
		{
			return new AIMoveBehaviorData
			{
				Faction = a.Faction,
				AIMode = a.AIMode,
				Class = a.Class,
				ActorTargetEnemiesMultiplier = a.ActorTargetEnemiesMultiplier - b.ActorTargetEnemiesMultiplier,
				ActorHitEnemiestMultiplier = a.ActorHitEnemiestMultiplier - b.ActorHitEnemiestMultiplier,
				EnemiesTargetActorMultiplier = a.EnemiesTargetActorMultiplier - b.EnemiesTargetActorMultiplier,
				ExploreMultiplier = a.ExploreMultiplier - b.ExploreMultiplier,
				CurrentTargetMultiplier = a.CurrentTargetMultiplier - b.CurrentTargetMultiplier,
				DistanceMultiplier = a.DistanceMultiplier - b.DistanceMultiplier,
				DistanceToTargetMultiplier = a.DistanceToTargetMultiplier - b.DistanceToTargetMultiplier,
				CoverBaseValue = a.CoverBaseValue - b.CoverBaseValue,
				CoverMultiplier = a.CoverMultiplier - b.CoverMultiplier
			};
		}
	}
}
