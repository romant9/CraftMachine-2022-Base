using System.Collections.Generic;

namespace TWDModel
{
	public class RangeRepulse1Trait : ActionModifier
	{
		private FixedPoint AddChargePointPercentage;

		private int AddChargePoints;

		public RangeRepulse1Trait(FixedPoint addChargePointPercentage, int addChargePoints)
		{
			AddChargePointPercentage = addChargePointPercentage;
			AddChargePoints = addChargePoints;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction { DamagerActor: not null } damageAction && actor == damageAction.DamagerActor && damageAction.TargetActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != Faction.Environmental && !damageAction.TargetActor.IsDead && damageAction.SourceSupport == null && !(damageAction is DamageConsumableAction))
			{
				damageAction.TargetActor.DebuffRemoteRepulseWeakenAddChargePointPercentage = AddChargePointPercentage;
				damageAction.TargetActor.DebuffRemoteRepulseWeakenAddChargePoints = AddChargePoints;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
