using System.Collections.Generic;

namespace TWDModel
{
	public class ShieldBreakerStrikeTrait : ActionModifier
	{
		private FixedPoint Parameter0;

		private int Parameter1;

		public ShieldBreakerStrikeTrait(FixedPoint parameter0, int parameter1)
		{
			Parameter0 = parameter0;
			Parameter1 = parameter1;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction { DamagerActor: not null } damageAction && actor == damageAction.DamagerActor && damageAction.TargetActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != Faction.Environmental && !damageAction.TargetActor.IsDead && damageAction.SourceSupport == null && !(damageAction is DamageConsumableAction))
			{
				FixedPoint value = 0.0;
				if (Parameter0 != 0.0)
				{
					base.manager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					if (actor.manager.Player.RollDice(RollDiceType.BreakShield, Parameter0, value) != PlayerRandomChanceResult.Failed)
					{
						if (damageAction.TargetActor.ShieldTimedEffect != null)
						{
							damageAction.TargetActor.FinishShieldTimedEffect();
						}
						List<CoexistTimedEffectType> list = new List<CoexistTimedEffectType>();
						if (damageAction.TargetActor.SkillEquipTauntShieldTimedEffect != null)
						{
							list.Add(CoexistTimedEffectType.SkillEquipTauntShield);
						}
						if (damageAction.TargetActor.SkillShieldType1TimedEffect != null)
						{
							list.Add(CoexistTimedEffectType.SkillShieldType1);
						}
						if (list.Count > 0)
						{
							damageAction.TargetActor.CoexistTimedEffectsManager.RemoveCoexistTimedEffectByCoexistTimedEffectTypeList(list);
						}
						if (damageAction.TargetActor.ShieldHitPoints > 0)
						{
							damageAction.TargetActor.ChangeShieldHitPoints(-damageAction.TargetActor.ShieldHitPoints);
						}
						addedActions.Add(new ShieldBreakerAction(actor, damageAction.TargetActor, Parameter1));
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
