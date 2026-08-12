using System.Collections.Generic;

namespace TWDModel
{
	public class PoisonBurstTrait : ActionModifier
	{
		private int OverLayerCount;

		private FixedPoint DamagePercentage;

		public PoisonBurstTrait(int overLayerCount, FixedPoint damagePercentage)
		{
			OverLayerCount = overLayerCount;
			DamagePercentage = damagePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor != null && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && postDamageAction.DamagerActor.Faction != Faction.Environmental && postDamageAction.DamageAction.DamageType != DamageType.Poison)
			{
				PoisonRelationsManager model = actor.manager.CombatModel.GetModel<PoisonRelationsManager>();
				if (model == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (model.ExistedPoisonRelations == null || model.ExistedPoisonRelations.Count == 0)
				{
					return ActionListClearFlag.Keep;
				}
				PoisonRelation poisonRelation = model.ExistedPoisonRelations.Find((PoisonRelation x) => x.SourceActor == postDamageAction.DamagerActor && x.TargetActor == postDamageAction.TargetActor);
				if (poisonRelation == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (poisonRelation.CurrentLayerCount < OverLayerCount)
				{
					return ActionListClearFlag.Keep;
				}
				if (poisonRelation.SourceActor is SurvivorModel survivorModel)
				{
					FixedPoint fixedPoint = survivorModel.GetDamageForPreferredWeapon() * poisonRelation.AttackerDamagePercentage * poisonRelation.CurrentLayerCount * DamagePercentage;
					CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, postDamageAction.TargetActor, (int)fixedPoint, 0, DamageType.PoisonBurst, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
