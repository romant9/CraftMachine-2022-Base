using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class WalkerMoveBehavior : BehaviorBase
	{
		public WalkerMoveBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (base.Actor.MoveCompleted || base.AIDataModel.Alertness == AIAlertness.Idle || base.Actor.IsRooted || base.Actor.IsPitfalled)
			{
				return 0;
			}
			return 100;
		}

		private GridCoordinate GetAttackMoveTarget(ActorModel currentTarget)
		{
			return currentTarget.GridCoordinate;
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
			ActorModel currentTarget = base.AIDataModel.GetCurrentTarget();
			if (currentTarget != null && !currentTarget.IsDead && !currentTarget.IsStruggling && !currentTarget.IsBleedingOut)
			{
				return currentTarget.GridCoordinate;
			}
			Faction faction = ((!base.AIDataModel.HasEvent(AIDataModel.ForceCivilianTargets)) ? Faction.Survivor : Faction.Civilian);
			List<ActorModel> factionActors = base.CombatModel.GetFactionActors(faction);
			if (factionActors.Count == 0 && faction != Faction.Survivor)
			{
				faction = Faction.Survivor;
				factionActors = base.CombatModel.GetFactionActors(faction);
			}
			if (base.Actor.IsWalker && base.Actor.IsDisoriented)
			{
				return GridCoordinate.Invalid;
			}
			if (base.Actor.IsWalker && base.Actor.IsABTesterA2ed)
			{
				return GridCoordinate.Invalid;
			}
			if (base.Actor.IsWalker && base.Actor.IsTaunted)
			{
				return GridCoordinate.Invalid;
			}
			ActorModel actorModel = AIBehaviorHelpers.PickClosestTarget(base.Actor, base.CombatModel, factionActors);
			if (actorModel != null)
			{
				base.Controller.AttackTarget(actorModel);
				return actorModel.GridCoordinate;
			}
			if (factionActors.TrueForAll((ActorModel s) => s.IsInvisible))
			{
				return GetWanderingMoveTarget();
			}
			return GridCoordinate.Invalid;
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
			switch (base.AIDataModel.Alertness)
			{
			case AIAlertness.Aggressive:
				if (AIBehaviorHelpers.CanSeeTarget(base.Actor, base.CombatModel, currentTarget))
				{
					gridCoordinate = GetAttackMoveTarget(currentTarget);
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
			bool flag = false;
			GridCoordinate flankingCoordinate = AIBehaviorHelpers.GetFlankingCoordinate(base.Actor, base.CombatModel, gridCoordinate);
			if (gridCoordinate != flankingCoordinate)
			{
				gridCoordinate = flankingCoordinate;
				flag = true;
			}
			GridCoordinate gridCoordinate2 = base.AIDataModel.GetGridCoordinate(AIDataModel.MoveToCoordinate);
			if (base.CombatModel.Grid.IsCoordinateValid(gridCoordinate) && gridCoordinate != gridCoordinate2)
			{
				base.AIDataModel.SetGridCoordinate(AIDataModel.MoveToCoordinate, gridCoordinate);
			}
			ActorModel occupier = base.CombatModel.GetOccupier(gridCoordinate);
			bool flag2 = base.CombatModel.CanTraverse(null, base.Actor.GridCoordinate, gridCoordinate) && occupier != null && occupier.IsEnemy(base.Actor);
			if (flag || !flag2)
			{
				if (base.CombatModel.Grid.IsCoordinateValid(gridCoordinate))
				{
					GridPath pathTowardsTarget = GridHelpers.GetPathTowardsTarget(base.Actor, base.CombatModel, gridCoordinate, base.Actor.MoveRange + 1);
					if (pathTowardsTarget.IsValid)
					{
						if (pathTowardsTarget.End == gridCoordinate || (base.CombatModel.GetOccupier(gridCoordinate) != null && base.CombatModel.Grid.AreNeighbors(pathTowardsTarget.End, gridCoordinate)))
						{
							base.AIDataModel.SetGridCoordinate(AIDataModel.WanderingMoveTarget, GridCoordinate.Invalid);
						}
						MoveCommand.PerformActions(base.Actor.manager, base.Actor, pathTowardsTarget);
					}
					else
					{
						base.AIDataModel.SetGridCoordinate(AIDataModel.WanderingMoveTarget, GridCoordinate.Invalid);
						if (AIController.VerboseDebug)
						{
							IModelDebug debug = base.Actor.manager.Debug;
							string[] obj = new string[5]
							{
								"Walker '",
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
					base.Actor.manager.Debug.LogWarning("Walker '" + base.Actor?.ToString() + "' tried to move but could not get valid move to coordinate!");
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
