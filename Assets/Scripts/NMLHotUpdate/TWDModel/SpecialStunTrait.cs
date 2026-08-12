using System.Collections.Generic;

namespace TWDModel
{
	public class SpecialStunTrait : ActionModifier
	{
		private FixedPoint MakeStunPercentage;

		private FixedPoint MakeStunMaxPercentage;

		public SpecialStunTrait(FixedPoint makeStunPercentage, FixedPoint makeStunMaxPercentage)
		{
			MakeStunPercentage = makeStunPercentage;
			MakeStunMaxPercentage = makeStunMaxPercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action.GetType() == typeof(AbilityAction))
			{
				if (!(action is AbilityAction abilityAction))
				{
					return ActionListClearFlag.Keep;
				}
				ActorModel actorModel = base.manager.CombatModel.Occupiers[abilityAction.TargetCell];
				if (abilityAction.Actor == actor && base.manager.CombatModel.TurnManager.ActiveFaction == actor.Faction && actorModel != null && actorModel.Faction != actor.Faction && IsTargetEffect(actorModel) && !actorModel.IsEnvironmental)
				{
					int num = (base.manager.CombatModel.CombatHUDState.ShowThreatState ? base.manager.CombatModel.ThreatMeter.ThreatLevel : 0);
					FixedPoint fixedPoint = FixedPoint.Min(MakeStunPercentage * num, MakeStunMaxPercentage);
					if (fixedPoint > 0L)
					{
						FixedPoint value = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
						if (base.manager.Player.RollDice(RollDiceType.Stun, fixedPoint, value) != PlayerRandomChanceResult.Failed)
						{
							addedActions.Add(new StunAction(abilityAction.Actor, actorModel, 1, ignoreSourceBeingDead: false, null, null, CanNotAvoidStunType.SpecialStun));
							return ActionListClearFlag.Keep;
						}
					}
					EquipmentItemModel weaponEquipment = abilityAction.Actor.GetWeaponEquipment();
					actor.AddTemporaryTrait("Special_Stun_Active_Flag", default(FixedPoint), null, 0L);
					addedActions.Add(new SpecialStunExtraDamageAction(actor, weaponEquipment.Ability, abilityAction.TargetCell, actorModel));
				}
			}
			if (action is PostAbilityExecuteAction postAbilityExecuteAction && postAbilityExecuteAction.DamagerActor == actor && base.manager.CombatModel.TurnManager.ActiveFaction == actor.Faction && postAbilityExecuteAction.DamagerActor.HasTrait("Special_Stun_Active_Flag"))
			{
				postAbilityExecuteAction.DamagerActor.RemoveTrait("Special_Stun_Active_Flag");
			}
			return ActionListClearFlag.Keep;
		}

		private bool IsTargetEffect(ActorModel target)
		{
			if (!target.IsImmuneToStun)
			{
				return target.HasTraitsThatContains("HealthThresholdedStatusResistance");
			}
			return true;
		}
	}
}
