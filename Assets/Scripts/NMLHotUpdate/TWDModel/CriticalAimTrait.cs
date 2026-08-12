using System.Collections.Generic;

namespace TWDModel
{
	public class CriticalAimTrait : ActionModifier
	{
		private FixedPoint chancePercentage;

		public CriticalAimTrait()
		{
		}

		public CriticalAimTrait(FixedPoint chancePercentage)
		{
			this.chancePercentage = chancePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && actor != null && postDamageAction.DamagerActor == actor && postDamageAction.DamageAction.DamageType == DamageType.Ranged && !postDamageAction.TargetActor.IsDead && !postDamageAction.TargetActor.IsStunned && postDamageAction.DamageAction.SourceSupport == null)
			{
				DamageAction damageAction = postDamageAction.DamageAction;
				CombatModel combatModel = base.manager.CombatModel;
				ActorModel damagerActor = damageAction.DamagerActor;
				ActorModel targetActor = damageAction.TargetActor;
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				FixedPoint value = 0.0;
				FixedPoint value2 = chancePercentage;
				combatModel.AbilityManager.VisitParameter(postDamageAction.DamageAction.Critical ? "AbilityModifierIncreaseCriticalAimChanceCriticalHit" : "AbilityModifierIncreaseCriticalAimChance", ref value2, actor);
				combatModel.AbilityManager.VisitParameter(postDamageAction.DamageAction.Critical ? "AbilityModifierIncreaseEquipCriticalAimChanceCriticalHit" : "AbilityModifierIncreaseEquipCriticalAimChance", ref value2, actor);
				if (value2 > 0.0)
				{
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.CriticalAim, value2, value);
				}
				if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && !actor.HasAnyLevelTrait("DebuffMarkEnemy"))
				{
					FixedPoint value3 = 0L;
					combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseCriticalAimStunTurnsModifier", ref value3, damagerActor);
					FixedPoint value4 = 0L;
					combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseEquipCriticalAimStunTurnsModifier", ref value4, damagerActor);
					if (value3 < value4)
					{
						value3 = value4;
					}
					addedActions.Add(new StunAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, (int)value3, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
				}
				if (actor.HasAnyLevelTrait("DebuffMarkEnemy"))
				{
					targetActor.NotifyChange("actorCriticalAim", new object[1] { playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension });
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
