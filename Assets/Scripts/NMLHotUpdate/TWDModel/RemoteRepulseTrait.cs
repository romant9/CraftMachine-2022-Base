using System.Collections.Generic;

namespace TWDModel
{
	public class RemoteRepulseTrait : ActionModifier
	{
		private FixedPoint AddWeakenPercentage;

		private int Turns;

		private FixedPoint AddChargePointPercentage;

		private int AddChargePoints;

		private bool RandomedCanAddChargePointFromCurrentAttack;

		public RemoteRepulseTrait(FixedPoint addWeakenPercentage, int turns, FixedPoint addChargePointPercentage, int addChargePoints)
		{
			AddWeakenPercentage = addWeakenPercentage;
			Turns = turns;
			AddChargePointPercentage = addChargePointPercentage;
			AddChargePoints = addChargePoints;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction { DamagerActor: not null } damageAction && actor == damageAction.DamagerActor && damageAction.TargetActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != damageAction.TargetActor.Faction && !damageAction.TargetActor.IsDead && damageAction.SourceSupport == null && !(damageAction is DamageConsumableAction))
			{
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.RemoteRepulse, AddWeakenPercentage, value) == PlayerRandomChanceResult.Failed)
				{
					return ActionListClearFlag.Keep;
				}
				damageAction.TargetActor.NotifyChange("ActorBeRemoteWeakened");
				damageAction.TargetActor.DebuffRemoteRepulseWeakenAddChargePointPercentage = AddChargePointPercentage;
				damageAction.TargetActor.DebuffRemoteRepulseWeakenAddChargePoints = AddChargePoints;
				addedActions.Add(new RemoteWeakenAction(actor, damageAction.TargetActor, Turns));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
