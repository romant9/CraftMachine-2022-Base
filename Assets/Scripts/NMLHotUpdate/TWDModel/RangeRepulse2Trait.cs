using System.Collections.Generic;

namespace TWDModel
{
	public class RangeRepulse2Trait : ActionModifier
	{
		private bool isChargeAttack;

		private FixedPoint AddWeakenPercentage;

		private int Turns;

		private bool RandomedCanAddChargePointFromCurrentAttack;

		public RangeRepulse2Trait(FixedPoint addWeakenPercentage, int turns)
		{
			AddWeakenPercentage = addWeakenPercentage;
			Turns = turns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction && actor != null && abilityAction.Actor == actor)
			{
				if (abilityAction.Ability.IsChargeAttack)
				{
					if (!abilityAction.Ability.IsConsumableAbility)
					{
						isChargeAttack = abilityAction.Ability.IsChargeAttack;
					}
				}
				else
				{
					isChargeAttack = abilityAction.Ability.IsChargeAttack;
				}
			}
			if (action is DamageAction { DamagerActor: not null } damageAction && isChargeAttack && actor == damageAction.DamagerActor && damageAction.TargetActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != Faction.Environmental && !damageAction.TargetActor.IsDead && damageAction.SourceSupport == null && !(damageAction is DamageConsumableAction))
			{
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.RemoteRepulse, AddWeakenPercentage, value) == PlayerRandomChanceResult.Failed)
				{
					return ActionListClearFlag.Keep;
				}
				damageAction.TargetActor.NotifyChange("ActorBeRemoteWeakened");
				addedActions.Add(new RemoteWeakenAction(actor, damageAction.TargetActor, Turns));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
