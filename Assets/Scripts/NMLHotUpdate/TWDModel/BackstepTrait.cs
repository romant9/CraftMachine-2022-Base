using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class BackstepTrait : ActionModifier
	{
		private int backstepDistance;

		public BackstepTrait(int distance)
		{
			backstepDistance = distance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && postDamageAction.DamagerActor == actor)
			{
				if (actor == null || !actor.HasAnyLevelTrait("Equipment_Passive_Backstep"))
				{
					return ActionListClearFlag.Keep;
				}
				if (!postDamageAction.IsChargeAttack)
				{
					return ActionListClearFlag.Keep;
				}
				if (!postDamageAction.IsMainTarget)
				{
					return ActionListClearFlag.Keep;
				}
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel == null || actor.IsDead || actor.IsStruggling)
				{
					return ActionListClearFlag.Keep;
				}
				GridCoordinate gridCoordinate = CalculateBackstepCoordinate(actor, postDamageAction.TargetActor, combatModel);
				if (gridCoordinate.IsValid && gridCoordinate != actor.GridCoordinate)
				{
					DamageAction damageAction = new DamageAction(actor, actor, 0, 0, bodyShot: false, critical: false, PlayerRandomChanceResult.Failed, DamageType.Melee);
					PushEffect effect = new PushEffect
					{
						DamageAction = damageAction,
						OriginalCoordinate = actor.GridCoordinate,
						PushCoordinate = gridCoordinate
					};
					addedActions.Add(new PushActorAction(effect));
				}
			}
			return ActionListClearFlag.Keep;
		}

		private GridCoordinate CalculateBackstepCoordinate(ActorModel actor, ActorModel target, CombatModel combatModel)
		{
			GridCoordinate gridCoordinate = actor.GridCoordinate;
			GridCoordinate gridCoordinate2 = target.GridCoordinate;
			int num = gridCoordinate2.X - gridCoordinate.X;
			int num2 = gridCoordinate2.Y - gridCoordinate.Y;
			bool flag = num == 0;
			bool flag2 = num2 == 0;
			if (flag && flag2)
			{
				return GridCoordinate.Invalid;
			}
			int num3 = 0;
			int num4 = 0;
			GridCoordinate end;
			if (flag)
			{
				num4 = ((num2 <= 0) ? 1 : (-1));
				end = new GridCoordinate(gridCoordinate.X, gridCoordinate.Y + num4 * backstepDistance);
			}
			else if (flag2)
			{
				num3 = ((num <= 0) ? 1 : (-1));
				end = new GridCoordinate(gridCoordinate.X + num3 * backstepDistance, gridCoordinate.Y);
			}
			else
			{
				int num5 = (int)Math.Ceiling((float)backstepDistance / 2f);
				num3 = ((num <= 0) ? 1 : (-1));
				num4 = ((num2 <= 0) ? 1 : (-1));
				end = new GridCoordinate(gridCoordinate.X + num3 * num5, gridCoordinate.Y + num4 * num5);
			}
			return FindValidBackstepCoordinate(actor, gridCoordinate, end, combatModel);
		}

		private GridCoordinate FindValidBackstepCoordinate(ActorModel actor, GridCoordinate start, GridCoordinate end, CombatModel combatModel)
		{
			int num = ((end.X > start.X) ? 1 : ((end.X < start.X) ? (-1) : 0));
			int num2 = ((end.Y > start.Y) ? 1 : ((end.Y < start.Y) ? (-1) : 0));
			int num3 = Math.Max(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y));
			GridCoordinate gridCoordinate = start;
			for (int i = 1; i <= num3; i++)
			{
				GridCoordinate gridCoordinate2 = new GridCoordinate(start.X + num * i, start.Y + num2 * i);
				if (!combatModel.Grid.IsCoordinateValid(gridCoordinate2))
				{
					return gridCoordinate;
				}
				if (combatModel.IsBlocked(gridCoordinate2))
				{
					return gridCoordinate;
				}
				ActorModel occupier = combatModel.GetOccupier(gridCoordinate2);
				if (occupier != null && occupier != actor)
				{
					return gridCoordinate;
				}
				if (!combatModel.CanTraverse(actor, gridCoordinate, gridCoordinate2))
				{
					return gridCoordinate;
				}
				gridCoordinate = gridCoordinate2;
			}
			return gridCoordinate;
		}
	}
}
