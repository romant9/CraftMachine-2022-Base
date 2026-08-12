using System.Collections.Generic;

namespace TWDModel
{
	public class LeaderBuffForestStalkerTrait : ActionModifier
	{
		private FixedPoint finalDamageModifier = 0.0;

		public LeaderBuffForestStalkerTrait()
		{
		}

		public LeaderBuffForestStalkerTrait(FixedPoint damageModifier)
		{
			finalDamageModifier = damageModifier;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (actor.dashTraitAttackFlag)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is MoveAction { Replaced: false } moveAction)
			{
				if (moveAction.Actor != actor)
				{
					return ActionListClearFlag.Keep;
				}
				if (moveAction.Actor.HasAnyLevelTrait("LeaderBuffOneWithTheHerdStalker"))
				{
					return ActionListClearFlag.Keep;
				}
				if (moveAction.Actor.IsInvisible)
				{
					return ActionListClearFlag.Keep;
				}
				if (!moveAction.CanBeInterruptedForPassByAttack)
				{
					return ActionListClearFlag.Keep;
				}
				new List<GridCoordinate>();
				_ = base.manager.CombatModel;
				GetIntersectingCoordinate(base.manager, moveAction.Path, out var intersectingCoordinate, out var walkerToAttack);
				FixedPoint moveDistance = moveAction.Path.MoveDistance;
				if (intersectingCoordinate != GridCoordinate.Invalid && !walkerToAttack.HasTraitsThatContains("Whisperer") && !walkerToAttack.Definition.IsEnvironmental)
				{
					GridPath gridPath = GridPath.Create(moveAction.Path);
					gridPath.ClipFromStartUntil(intersectingCoordinate);
					GridCoordinate end = moveAction.Path.End;
					moveAction.Path.ClipTo(intersectingCoordinate);
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					if (weaponEquipment != null)
					{
						if (finalDamageModifier != 0.0)
						{
							weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, finalDamageModifier);
						}
						AbilityAction item = new AbilityAction(actor, weaponEquipment.Ability, walkerToAttack.GridCoordinate, walkerToAttack, OOTType.PassByAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
						actor.GridCoordinate = intersectingCoordinate;
						addedActions.Add(item);
						if (intersectingCoordinate != end)
						{
							bool consumeAP = moveDistance > moveAction.Actor.MoveRange;
							moveAction.Replaced = true;
							MoveAction moveAction2 = new MoveAction(moveAction.Actor, gridPath, consumeAP, globallyBlocking: true);
							moveAction2.CanBeInterruptedForPassByAttack = false;
							addedActions.Add(moveAction2);
						}
						actor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffForestStalker", false });
					}
					else
					{
						base.manager.Debug.LogWarning("Actor: " + actor.ToString() + " tried to perform FreeAttackTrait but could not find weapon equipment!");
					}
					return ActionListClearFlag.Clear;
				}
			}
			return ActionListClearFlag.Keep;
		}

		public static void GetIntersectingCoordinate(TWDModelManager manager, GridPath path, out GridCoordinate intersectingCoordinate, out ActorModel walkerToAttack)
		{
			walkerToAttack = null;
			intersectingCoordinate = GridCoordinate.Invalid;
			List<GridCoordinate> list = new List<GridCoordinate>(5);
			CombatModel combatModel = manager.CombatModel;
			GridModel grid = combatModel.Grid;
			FixedPoint fixedPoint = FixedPoint.MaxValue;
			for (int i = 0; i < combatModel.Walkers.Count; i++)
			{
				ActorModel actorModel = combatModel.Walkers[i];
				if (!actorModel.IsVisibleToSurvivors || (path.HasTargetCoordinate && path.TargetCoordinate == actorModel.GridCoordinate))
				{
					continue;
				}
				list.Clear();
				foreach (GridCoordinate item in grid.Neighbors(actorModel.GridCoordinate))
				{
					if (path.Contains(item) && combatModel.GetOccupier(item) == null && combatModel.CanTraverse(null, item, actorModel.GridCoordinate) && combatModel.IsGridCellVisible(item, actorModel.GridCoordinate) && item != path.End)
					{
						list.Add(item);
					}
				}
				if (list.Count >= 2)
				{
					FixedPoint fixedPoint2 = actorModel.GridCoordinate.DistanceTo(path.Start);
					if (fixedPoint2 < fixedPoint)
					{
						fixedPoint = fixedPoint2;
						intersectingCoordinate = list[0];
						walkerToAttack = actorModel;
					}
				}
			}
		}
	}
}
