using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RaiderMoveBehavior : BehaviorBase
	{
		public RaiderMoveBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (base.AIDataModel.Mode == AIMode.Stationary || base.Actor.IsBossClass || base.Actor is TankActorModel)
			{
				return 0;
			}
			if (base.Actor.MoveCompleted || base.Actor.IsRooted || base.Actor.IsPitfalled || base.Actor.IsInFortifications || base.AIDataModel.Alertness == AIAlertness.Idle)
			{
				return 0;
			}
			return 100;
		}

		private GridCoordinate GetAttackMoveTarget(ActorModel currentTarget)
		{
			return currentTarget.GridCoordinate;
		}

		private TacticalMoveTargetInfo GetTacticalMoveTarget(ActorModel currentTarget)
		{
			return base.CombatModel.GetFactionAIController(Faction.Raider)?.GetTacticalMoveTarget(base.Actor, currentTarget) ?? new TacticalMoveTargetInfo
			{
				Coordinate = currentTarget.GridCoordinate,
				Value = 0.0
			};
		}

		private GridCoordinate GetWanderingMoveTarget()
		{
			GridCoordinate gridCoordinate = base.AIDataModel.GetGridCoordinate(AIDataModel.WanderingMoveTarget);
			if (!gridCoordinate.IsValid)
			{
				gridCoordinate = AIBehaviorHelpers.GetRandomMoveCoordinate(base.Actor, base.CombatModel);
				base.AIDataModel.SetGridCoordinate(AIDataModel.WanderingMoveTarget, gridCoordinate);
			}
			return gridCoordinate;
		}

		private GridCoordinate GetHomingMoveTarget()
		{
			List<ActorModel> list = null;
			list = ((!base.AIDataModel.HasEvent(AIDataModel.ForceCivilianTargets)) ? base.CombatModel.GetEnemyFactionsActors(Faction.Raider) : base.CombatModel.GetFactionActors(Faction.Civilian));
			if (list == null || list.Count == 0)
			{
				list = base.CombatModel.GetFactionActors(Faction.Survivor);
			}
			return AIBehaviorHelpers.PickClosestTarget(base.Actor, base.CombatModel, list)?.GridCoordinate ?? GridCoordinate.Invalid;
		}

		private GridCoordinate GetFollowMoveTarget()
		{
			ActorModel modelReference = base.AIDataModel.GetModelReference<ActorModel>(AIDataModel.FollowTarget);
			if (modelReference != null)
			{
				if (!modelReference.IsDead)
				{
					return modelReference.GridCoordinate;
				}
				base.AIDataModel.Alertness = AIAlertness.Wandering;
				return GetWanderingMoveTarget();
			}
			return GridCoordinate.Invalid;
		}

		public override void ExecuteAction()
		{
			ActorModel currentTarget = base.AIDataModel.GetCurrentTarget();
			GridCoordinate gridCoordinate = GridCoordinate.Invalid;
			TacticalMoveTargetInfo tacticalMoveTargetInfo = new TacticalMoveTargetInfo
			{
				Coordinate = GridCoordinate.Invalid,
				Value = 0.0
			};
			switch (base.AIDataModel.Alertness)
			{
			case AIAlertness.Aggressive:
				if (AIBehaviorHelpers.CanSeeTarget(base.Actor, base.CombatModel, currentTarget))
				{
					gridCoordinate = GetTacticalMoveTarget(currentTarget).Coordinate;
				}
				else if (base.AIDataModel.GetGridCoordinate(AIDataModel.LastEnemyLocation) != GridCoordinate.Invalid)
				{
					gridCoordinate = base.AIDataModel.GetGridCoordinate(AIDataModel.LastEnemyLocation);
					base.AIDataModel.SetGridCoordinate(AIDataModel.LastEnemyLocation, GridCoordinate.Invalid);
				}
				break;
			case AIAlertness.Alerted:
				gridCoordinate = GetFollowMoveTarget();
				if (!base.CombatModel.Grid.IsCoordinateValid(gridCoordinate))
				{
					gridCoordinate = base.AIDataModel.GetGridCoordinate(AIDataModel.MoveToCoordinate);
				}
				break;
			case AIAlertness.Homing:
				gridCoordinate = GetHomingMoveTarget();
				break;
			case AIAlertness.Wandering:
				gridCoordinate = GetWanderingMoveTarget();
				break;
			}
			if (gridCoordinate != base.Actor.GridCoordinate && gridCoordinate != GridCoordinate.Invalid)
			{
				if (base.CombatModel.Grid.IsCoordinateValid(gridCoordinate))
				{
					GridPath pathTowardsTarget = GridHelpers.GetPathTowardsTarget(base.Actor, base.CombatModel, gridCoordinate, base.Actor.MoveRange * 2);
					if (pathTowardsTarget.IsValid)
					{
						if (pathTowardsTarget.End == gridCoordinate || (base.CombatModel.GetOccupier(gridCoordinate) != null && base.CombatModel.Grid.AreNeighbors(pathTowardsTarget.End, gridCoordinate)))
						{
							base.AIDataModel.SetGridCoordinate(AIDataModel.WanderingMoveTarget, GridCoordinate.Invalid);
						}
						MoveCommand.PerformActions(base.Actor.manager, base.Actor, pathTowardsTarget);
						GridCoordinate gridCoordinate2 = base.AIDataModel.GetGridCoordinate(AIDataModel.MoveToCoordinate);
						if (base.CombatModel.Grid.IsCoordinateValid(gridCoordinate) && gridCoordinate != gridCoordinate2)
						{
							base.AIDataModel.SetGridCoordinate(AIDataModel.MoveToCoordinate, gridCoordinate);
						}
					}
					else
					{
						base.AIDataModel.SetGridCoordinate(AIDataModel.WanderingMoveTarget, GridCoordinate.Invalid);
						if (AIController.VerboseDebug)
						{
							IModelDebug debug = base.Actor.manager.Debug;
							string[] obj = new string[5]
							{
								"Raider '",
								base.Actor?.ToString(),
								"' tried to move but could not find path to target ",
								null,
								null
							};
							GridCoordinate gridCoordinate3 = gridCoordinate;
							obj[3] = gridCoordinate3.ToString();
							obj[4] = "!";
							debug.LogWarning(string.Concat(obj));
						}
					}
				}
				else if (AIController.VerboseDebug)
				{
					base.Actor.manager.Debug.LogWarning("Raider '" + base.Actor?.ToString() + "' tried to move but could not get valid move to coordinate!");
				}
				base.Controller.IsStuck(!base.Actor.MoveCompleted);
			}
			if (!base.Actor.MoveCompleted)
			{
				base.Actor.EndMovement();
			}
		}
	}
}
