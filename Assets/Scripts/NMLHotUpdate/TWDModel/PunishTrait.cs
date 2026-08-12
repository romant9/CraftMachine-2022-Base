using System.Collections.Generic;

namespace TWDModel
{
	public class PunishTrait : ActionModifier
	{
		private FixedPoint multiplier = 1.0;

		public PunishTrait()
		{
		}

		public PunishTrait(FixedPoint damageMultiplier)
		{
			multiplier = damageMultiplier;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.dashTraitAttackFlag)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is ChangeTurnAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (actor != null && combatModel != null && !combatModel.MissionCompleted)
				{
					if (actor.Faction != combatModel.TurnManager.ActiveFaction)
					{
						return ActionListClearFlag.Keep;
					}
					if (actor.IsInvisible)
					{
						return ActionListClearFlag.Keep;
					}
					EnumerableNeighbors enumerableNeighbors = combatModel.Grid.Neighbors(actor.GridCoordinate);
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					List<ActorModel> list = new List<ActorModel>();
					foreach (GridCoordinate item2 in enumerableNeighbors)
					{
						ActorModel occupier = combatModel.GetOccupier(item2);
						if (occupier != null && actor.IsEnemy(occupier) && !occupier.Definition.IsEnvironmental)
						{
							list.Add(occupier);
						}
					}
					ActorModel actorModel = ((list.Count > 0) ? combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: false) : null);
					if (weaponEquipment != null && actorModel != null)
					{
						FixedPoint value = multiplier;
						combatModel.AbilityManager.VisitParameter("AbilityModifierIncreasePunishDamage", ref value, actor);
						if (actor.HasAnyLevelTrait("Equipment.Punish"))
						{
							combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseNewPunishDamage", ref value, actor);
						}
						weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, value * 100.0);
						GenericAbilityAction item = new GenericAbilityAction(actor, weaponEquipment.Ability, actorModel.GridCoordinate, "ActorNotification.Punish", actorModel, OOTType.None, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true);
						addedActions.Add(item);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
