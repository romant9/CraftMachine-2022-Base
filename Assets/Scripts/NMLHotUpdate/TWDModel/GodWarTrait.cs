using System.Collections.Generic;

namespace TWDModel
{
	public class GodWarTrait : ActionModifier
	{
		private int DmgMaxCount;

		private FixedPoint MainTargetPercentage;

		public GodWarTrait(int dmgMaxCount, FixedPoint mainTargetPercentage)
		{
			DmgMaxCount = dmgMaxCount;
			MainTargetPercentage = mainTargetPercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.GetTraitsThatContain("GodWarBless").Count <= 0 || actor.GodWarTraitTurns <= 0)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is AbilityAction abilityAction)
			{
				_ = abilityAction.Actor;
			}
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor != null && postDamageAction.IsMainTarget)
			{
				int num = ((postDamageAction.DamagerActor.NumberOfEnemiesAttacked >= DmgMaxCount) ? DmgMaxCount : postDamageAction.DamagerActor.NumberOfEnemiesAttacked);
				int num2 = (int)(postDamageAction.DamageAction.FinalDamage * MainTargetPercentage * num);
				if (num2 < 0)
				{
					num2 = int.MaxValue;
				}
				postDamageAction.TargetActor.DealDamage(num2, postDamageAction.DamagerActor, DamageType.Base);
				postDamageAction.TargetActor.NotifyChange("ActorHealthChanged");
				postDamageAction.TargetActor.NotifyChange("HelpHandDamageChanged", num2);
			}
			return ActionListClearFlag.Keep;
		}
	}
}
