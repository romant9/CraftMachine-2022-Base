using System.Collections.Generic;

namespace TWDModel
{
	public class ActorTurnEntry
	{
		public int Id;

		public int Turn;

		public ActorModel Actor;

		public List<BehaviorLogEntry> BehaviorLog;

		public BehaviorLogEntry CurrentBehaviorLogEntry;

		public List<GridCoordinate> VisibleEnemyLocations;

		public void AddBehaviorLogEntry(BehaviorBase behavior)
		{
			if (BehaviorLog == null)
			{
				BehaviorLog = new List<BehaviorLogEntry>();
			}
			AIBehaviorEnum behaviorEnumValue = GetBehaviorEnumValue(behavior);
			CurrentBehaviorLogEntry = new BehaviorLogEntry
			{
				Id = AILog.logEntryId++,
				TurnId = Id,
				Behavior = behaviorEnumValue
			};
			BehaviorLog.Add(CurrentBehaviorLogEntry);
		}

		public void SetBeginCoordinate(GridCoordinate coordinate)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.BeginCoordinate = coordinate;
			}
		}

		public void SetEndCoordinate(GridCoordinate coordinate)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.EndCoordinate = coordinate;
			}
		}

		public void SetMoveToTarget(GridCoordinate coordinate)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.MoveTargetCoordinate = coordinate;
			}
		}

		public void SetPreExecuteCurrentTarget(ActorModel actor)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.PreExecuteCurrentTarget = actor;
			}
		}

		public void SetAfterExecuteCurrentTarget(ActorModel actor)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.AfterExecuteCurrentTarget = actor;
			}
		}

		public void SetAlertnessState(AIAlertness alertness)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.AlertnessState = alertness;
			}
		}

		public void SetAIMode(AIMode mode)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.AIMode = mode;
			}
		}

		public void SetMovementField(GridField<FixedPoint> field)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.MovementField = field;
			}
		}

		public void SetAttackField(GridField<FixedPoint> field, FixedPoint multiplier, FixedPoint currentTargetMultiplier)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.AttackField = field;
				CurrentBehaviorLogEntry.AttackMultiplier = multiplier;
				CurrentBehaviorLogEntry.CurrentTargetMultiplier = currentTargetMultiplier;
			}
		}

		public void SetDefenceField(GridField<FixedPoint> field, FixedPoint multiplier)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.DefenceField = field;
				CurrentBehaviorLogEntry.DefenceMultiplier = multiplier;
			}
		}

		public void SetExploreField(GridField<FixedPoint> field, FixedPoint multiplier)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.ExploreField = field;
				CurrentBehaviorLogEntry.ExploreMultiplier = multiplier;
			}
		}

		public void SetCoverField(GridField<bool> coverLocations, GridField<FixedPoint> field, FixedPoint value, FixedPoint multiplier)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.CoverLocations = coverLocations;
				CurrentBehaviorLogEntry.CoverField = field;
				CurrentBehaviorLogEntry.CoverBaseValue = value;
				CurrentBehaviorLogEntry.CoverEnemyMultiplier = multiplier;
			}
		}

		public void SetDistanceField(GridField<FixedPoint> field, FixedPoint multiplier)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.DistanceField = field;
				CurrentBehaviorLogEntry.DistanceMultiplier = multiplier;
			}
		}

		public void SetDistanceToTargetField(GridField<FixedPoint> field, FixedPoint multiplier)
		{
			if (CurrentBehaviorLogEntry != null)
			{
				CurrentBehaviorLogEntry.DistanceToTarget = field;
				CurrentBehaviorLogEntry.DistanceToTargetMultiplier = multiplier;
			}
		}

		public void SetVisibleEnemyLocations(List<ActorModel> enemies)
		{
			VisibleEnemyLocations.Clear();
			for (int i = 0; i < enemies.Count; i++)
			{
				VisibleEnemyLocations.Add(enemies[i].GridCoordinate);
			}
		}

		public BehaviorLogEntry GetLogEntryById(int id)
		{
			if (BehaviorLog != null)
			{
				for (int i = 0; i < BehaviorLog.Count; i++)
				{
					BehaviorLogEntry behaviorLogEntry = BehaviorLog[i];
					if (behaviorLogEntry.Id == id)
					{
						return behaviorLogEntry;
					}
				}
			}
			return null;
		}

		private AIBehaviorEnum GetBehaviorEnumValue(BehaviorBase behavior)
		{
			if (behavior is WalkerMoveBehavior || behavior is RaiderMoveBehavior)
			{
				return AIBehaviorEnum.Move;
			}
			if (behavior is WalkerAttackBehavior || behavior is RaiderAttackBehavior)
			{
				return AIBehaviorEnum.Attack;
			}
			if (behavior is ActorEndTurnBehavior)
			{
				return AIBehaviorEnum.EndTurn;
			}
			if (behavior is WalkerIdleBehavior || behavior is RaiderIdleBehavior)
			{
				return AIBehaviorEnum.Idle;
			}
			if (behavior is RaiderBuddyAidBehavior)
			{
				return AIBehaviorEnum.BuddyAid;
			}
			return AIBehaviorEnum.Invalid;
		}
	}
}
