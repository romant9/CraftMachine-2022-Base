using System.Collections.Generic;

namespace TWDModel
{
	public class FreeAttackTrait : ActionModifier
	{
		private FixedPoint freeAttackChance = 0.20000000298023224;

		private FixedPoint damageMultiplier = 1.0;

		public FreeAttackTrait()
		{
		}

		public FreeAttackTrait(int chance, FixedPoint multiplier)
		{
			freeAttackChance = (FixedPoint)chance / (FixedPoint)100.0;
			damageMultiplier = multiplier;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!actor.CanPerformOOT)
			{
				return ActionListClearFlag.Keep;
			}
			ActorModel model = base.manager.GetModel<ActorModel>(action.ModelId);
			if (model == null || actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (model != null && (model == actor || !model.IsEnemy(actor)))
			{
				return ActionListClearFlag.Keep;
			}
			if (model != null && !model.CanReceiveOOT)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is MoveAction moveAction)
			{
				if (moveAction.Actor.HasAnyLevelTrait("LeaderBuffForestStalker") || CheckCarolSneak(moveAction.Actor) || moveAction.Actor.HasAnyLevelTrait("LeaderBuffOneWithTheHerd") || moveAction.Actor.IsInvisible || HelpersModel.IsDodge)
				{
					return ActionListClearFlag.Keep;
				}
				if (moveAction.Path.HasTargetCoordinate && moveAction.Path.TargetCoordinate == actor.GridCoordinate)
				{
					return ActionListClearFlag.Keep;
				}
				List<GridCoordinate> list = new List<GridCoordinate>();
				CombatModel combatModel = base.manager.CombatModel;
				foreach (GridCoordinate item2 in combatModel.Grid.Neighbors(actor.GridCoordinate))
				{
					if (moveAction.Path.Contains(item2) && combatModel.GetOccupier(item2) == null && combatModel.CanTraverse(null, item2, actor.GridCoordinate) && item2 != moveAction.Path.End)
					{
						list.Add(item2);
					}
				}
				if (list.Count >= 2)
				{
					if (base.manager.Player.RollDice(RollDiceType.FreeAttack, freeAttackChance, 0.0) != PlayerRandomChanceResult.Failed)
					{
						if (actor.GetActiveLightState())
						{
							actor.NotifyChange("AbilityVisited", new object[2] { "Equipment_Active_Light", false });
							actor.ResetActiveLight();
							return ActionListClearFlag.Keep;
						}
						GridCoordinate randomElement = base.manager.Player.PlayerRandom.GetRandomElement(list, remove: false);
						moveAction.Path.ClipTo(randomElement);
						moveAction.Path.ClearTargetCoordinate();
						EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
						if (weaponEquipment != null)
						{
							weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, damageMultiplier);
							AbilityAction item = new AbilityAction(actor, weaponEquipment.Ability, randomElement, moveAction.Actor, OOTType.FreeAttack);
							addedActions.Add(item);
						}
						else
						{
							base.manager.Debug.LogWarning("Actor: " + actor.ToString() + " tried to perform FreeAttackTrait but could not find weapon equipment!");
						}
						actor.AIController.AttackTarget(moveAction.Actor);
						return ActionListClearFlag.Clear;
					}
					actor.NotifyChange("freeAttackFailed");
				}
			}
			return ActionListClearFlag.Keep;
		}

		private bool CheckCarolSneak(ActorModel source)
		{
			if (!source.IsSneak)
			{
				return false;
			}
			FixedPoint value = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, source);
			FixedPoint value2 = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("AbilityModifierCarolCannotAttackedChance", ref value2, source);
			if (base.manager.Player.RollDice(RollDiceType.Sneak, value2, value) != PlayerRandomChanceResult.Failed)
			{
				return true;
			}
			return false;
		}
	}
}
