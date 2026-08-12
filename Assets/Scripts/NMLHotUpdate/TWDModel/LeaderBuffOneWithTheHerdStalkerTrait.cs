using System.Collections.Generic;

namespace TWDModel
{
	public class LeaderBuffOneWithTheHerdStalkerTrait : ActionModifier
	{
		private FixedPoint finalDamageModifier = 0.0;

		public LeaderBuffOneWithTheHerdStalkerTrait()
		{
		}

		public LeaderBuffOneWithTheHerdStalkerTrait(FixedPoint damageModifier)
		{
			finalDamageModifier = damageModifier;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (!(action is MoveAction { Replaced: false } moveAction))
			{
				return ActionListClearFlag.Keep;
			}
			if (moveAction.Actor != actor)
			{
				return ActionListClearFlag.Keep;
			}
			if (moveAction.Actor.IsInvisible)
			{
				return ActionListClearFlag.Keep;
			}
			if (!moveAction.CanBeInterruptedForPassByPull || !moveAction.CanBeInterruptedForPassByAttack)
			{
				return ActionListClearFlag.Keep;
			}
			GridCoordinate end = moveAction.Path.End;
			LeaderBuffOneWithTheHerdTrait.GetIntersectingCoordinates(base.manager, moveAction.Path, out var intersectingCoordinateList, out var walkersToAddToTheHerd, isStalker: true);
			bool consumeAP = moveAction.Path.MoveDistance > moveAction.Actor.MoveRange;
			MoveAction moveAction2 = moveAction;
			actor.GainedChargePointOnMove = false;
			for (int i = 0; i < walkersToAddToTheHerd.Count; i++)
			{
				GridPath gridPath = GridPath.Create(moveAction2.Path);
				GridCoordinate gridCoordinate = intersectingCoordinateList[i];
				ActorModel actorModel = walkersToAddToTheHerd[i];
				if (!(gridCoordinate != GridCoordinate.Invalid) || actorModel.HasTraitsThatContains("Whisperer"))
				{
					continue;
				}
				gridPath.ClipFromStartUntil(gridCoordinate);
				ModelAction modelAction = null;
				if (i == 0)
				{
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					if (weaponEquipment != null)
					{
						if (finalDamageModifier != 0.0)
						{
							weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, finalDamageModifier);
						}
						modelAction = new AbilityAction(actor, weaponEquipment.Ability, actorModel.GridCoordinate, actorModel, OOTType.PassByAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
						actor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffForestStalker", false });
					}
				}
				else
				{
					modelAction = new HerdAction(actor, actorModel, 1, addedActions.FindAll((ModelAction t) => t is HerdAction).Count);
					actor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffOneWithTheHerd", false });
				}
				if (modelAction != null)
				{
					addedActions.Add(modelAction);
				}
				actor.GridCoordinate = gridCoordinate;
				if (moveAction2.Path.Start != gridCoordinate)
				{
					moveAction2.Path.ClipTo(gridCoordinate);
					if ((i <= 0 || !(gridCoordinate == intersectingCoordinateList[i - 1])) && gridCoordinate != end && gridPath.IsValid)
					{
						moveAction2.Replaced = true;
						moveAction2 = new MoveAction(moveAction.Actor, gridPath, consumeAP, globallyBlocking: true);
						moveAction2.CanBeInterruptedForPassByPull = false;
						moveAction2.CanBeInterruptedForPassByAttack = false;
						addedActions.Add(moveAction2);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
