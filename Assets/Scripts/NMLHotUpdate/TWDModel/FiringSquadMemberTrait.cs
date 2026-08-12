using System.Collections.Generic;

namespace TWDModel
{
	public class FiringSquadMemberTrait : ActionModifier
	{
		private AbilityModel ability;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction)
			{
				if (abilityAction.Actor != null && abilityAction.Actor.dashTraitAttackFlag)
				{
					return ActionListClearFlag.Keep;
				}
				CombatModel combatModel = actor.manager.CombatModel;
				ActorModel actorModel = combatModel.Occupiers[abilityAction.TargetCell];
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (combatModel.TurnManager.ActiveFaction == actor.Faction && abilityAction.OOTType == OOTType.None && abilityAction.Actor != actor)
				{
					ActorModel actor2 = abilityAction.Actor;
					if (actor2 != null && actor2.HasTrait("FiringSquadLeader") && !abilityAction.Ability.IsConsumableAbility && actorModel != null && !actorModel.IsDead && !weaponEquipment.NeedsReloading && !actor.IsInvisible)
					{
						if (ability == null)
						{
							AbilityModel abilityModel = weaponEquipment.Ability;
							ability = new FiringSquadAbility(abilityModel.DefinitionID);
							ability.SetManager(base.manager);
							ability.Start();
							for (int i = ability.Modifiers.GetCount(); i < abilityModel.Modifiers.GetCount(); i++)
							{
								ModelModifier modifier = abilityModel.Modifiers.GetModifier(i);
								ability.Modifiers.RegisterModifier(modifier);
								modifier.OwningCollection = weaponEquipment.Ability.Modifiers;
							}
						}
						FixedPoint fixedPoint = actor.GetCitadel_PursuitDown_ParameterMultiplier();
						if (fixedPoint <= ActorTraitContainerModel.Citadel_PercentBase)
						{
							fixedPoint = ActorTraitContainerModel.Citadel_PercentBase;
						}
						if (base.manager.Player.RollDice(RollDiceType.Citadel, fixedPoint) == PlayerRandomChanceResult.Failed)
						{
							return ActionListClearFlag.Keep;
						}
						if (ability.CanAbilityBePerformedOnGridCell(combatModel, actor, actor.GridCoordinate, actorModel.GridCoordinate) == AbilityResult.Success)
						{
							FixedPoint value = 1.0;
							if (combatModel.AbilityManager.VisitParameter("LeaderBuffFiringSquad", ref value, abilityAction.Actor))
							{
								addedActions.Add(new FiringSquadAction(actor, ability, actorModel.GridCoordinate, actorModel, value, isTriggerExtraAttackDamage: true));
							}
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
