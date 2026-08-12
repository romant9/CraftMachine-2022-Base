using System.Collections.Generic;

namespace TWDModel
{
	public class QuantunTrait : ActionModifier
	{
		private FixedPoint AddQuantunPercentage;

		private int Turns;

		private FixedPoint BaseDamagePercentage;

		private FixedPoint AdditionalDamagePercentage;

		private int MaxLayer;

		private FixedPoint CanNotActionPercentage;

		public QuantunTrait(FixedPoint addQuantunPercentage, int turns, FixedPoint baseDamagePercentage, FixedPoint additionalDamagePercentage, int maxLayer, FixedPoint canNotActionPercentage)
		{
			AddQuantunPercentage = addQuantunPercentage;
			Turns = turns;
			BaseDamagePercentage = baseDamagePercentage;
			AdditionalDamagePercentage = additionalDamagePercentage;
			MaxLayer = maxLayer;
			CanNotActionPercentage = canNotActionPercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction { DamagerActor: not null, TargetActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && postDamageAction.TargetActor.Faction != postDamageAction.DamagerActor.Faction && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				FixedPoint fixedPoint = 0.0;
				if (postDamageAction.TargetActor.IsWalker)
				{
					MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
					if (mapMissionModel != null)
					{
						fixedPoint = (float)(int)ChallengeDebufHelps.GetDebufTotalFirstParam(mapMissionModel.GetChallengeDebuffs(), ChallengeDebuffType.DebuffQuantunRate) / 100f;
					}
				}
				else if (postDamageAction.TargetActor.IsWalker)
				{
					MapMissionModel mapMissionModel2 = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
					if (mapMissionModel2 != null)
					{
						fixedPoint = (float)(int)ChallengeDebufHelps.GetDebufTotalFirstParam(mapMissionModel2.GetChallengeDebuffs(), ChallengeDebuffType.DebuffQuantunRateRaider) / 100f;
					}
				}
				FixedPoint successProbability = ((AddQuantunPercentage - fixedPoint > 0L) ? (AddQuantunPercentage - fixedPoint) : ((FixedPoint)0L));
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.Quantun, successProbability, value) == PlayerRandomChanceResult.Failed)
				{
					return ActionListClearFlag.Keep;
				}
				addedActions.Add(new QuantunAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, Turns, BaseDamagePercentage, AdditionalDamagePercentage, MaxLayer, CanNotActionPercentage));
			}
			return ActionListClearFlag.Keep;
		}
	}
}
