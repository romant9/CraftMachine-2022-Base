using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierPushRequiem : ActionModifier
	{
		private int range;

		private int extraChargePointChance;

		public AbilityModifierPushRequiem()
		{
		}

		public AbilityModifierPushRequiem(int range, int extraChargePointChance)
		{
			this.range = range;
			this.extraChargePointChance = extraChargePointChance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction damageAction)
			{
				if (damageAction.DamagerActor == null && damageAction.TargetActor == null && damageAction.TargetActor.IsDead)
				{
					return ActionListClearFlag.Keep;
				}
				if (!damageAction.DamagerActor.OverwatchedOnTurn)
				{
					return ActionListClearFlag.Keep;
				}
				CombatModel combatModel = base.manager.CombatModel;
				if (combatModel == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (!CombatHelpers.IsWithinRange(combatModel, range, damageAction.DamagerActor.GridCoordinate, damageAction.TargetActor.GridCoordinate))
				{
					return ActionListClearFlag.Keep;
				}
				AbilityModel abilityUnderApplication = base.manager.Player.AbilityManager.AbilityUnderApplication;
				ActorModel abilityOwnerActor = base.manager.Player.AbilityManager.AbilityOwnerActor;
				bool flag = abilityOwnerActor == null || abilityOwnerActor == damageAction.DamagerActor;
				if (abilityUnderApplication != null && abilityUnderApplication.PushEffect != null && flag && abilityUnderApplication.PushEffect.Add(damageAction))
				{
					damageAction.DealDamagePostAbility = true;
					abilityUnderApplication.PostExecuteActions.Add(damageAction);
					FixedPoint value = 0.0;
					combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, damageAction.DamagerActor);
					if (combatModel.manager.Player.RollDice(RollDiceType.GainAP, (FixedPoint)extraChargePointChance / (FixedPoint)100.0, value) != PlayerRandomChanceResult.Failed)
					{
						damageAction.DamagerActor.AddChargePoints(1);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
