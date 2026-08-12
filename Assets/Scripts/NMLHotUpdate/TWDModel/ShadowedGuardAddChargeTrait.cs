using System.Collections.Generic;

namespace TWDModel
{
	public class ShadowedGuardAddChargeTrait : ActionModifier
	{
		private FixedPoint AtkAddChargeNum = 0.0;

		private FixedPoint UnatkAddChargeNum = 0.0;

		public ShadowedGuardAddChargeTrait(FixedPoint atkAddChargeNum, FixedPoint unatkAddChargeNum)
		{
			AtkAddChargeNum = atkAddChargeNum;
			UnatkAddChargeNum = unatkAddChargeNum;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction)
			{
				if (abilityAction.Actor != null && abilityAction.Actor.Faction == actor.Faction)
				{
					actor.ChargeNum += AtkAddChargeNum;
					base.manager.CombatModel?.NotifyChange("UpdateShadowedGuardEvent");
				}
			}
			else if (action is PostDamageAction { TargetActor: not null } postDamageAction && postDamageAction.TargetActor.Faction == actor.Faction)
			{
				actor.ChargeNum += UnatkAddChargeNum;
				base.manager.CombatModel?.NotifyChange("UpdateShadowedGuardEvent");
			}
			return ActionListClearFlag.Keep;
		}
	}
}
