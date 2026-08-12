using System.Collections.Generic;

namespace TWDModel
{
	public class FlameTrait : ActionModifier
	{
		private FixedPoint Percentage;

		private bool IsCanFlame;

		public FlameTrait(FixedPoint percentage)
		{
			Percentage = percentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction && abilityAction.Actor == actor)
			{
				IsCanFlame = true;
			}
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor != null && IsCanFlame && postDamageAction.TargetActor.IsBurning && DamageType.Flame != postDamageAction.DamageAction.DamageType && postDamageAction.DamagerActor is SurvivorModel survivorModel)
			{
				FixedPoint fixedPoint = survivorModel.GetDamageForPreferredWeapon() * Percentage;
				List<ActorModel> enemyFactionsActors = base.manager.CombatModel.GetEnemyFactionsActors(postDamageAction.DamagerActor.Faction);
				for (int i = 0; i < enemyFactionsActors.Count; i++)
				{
					if (enemyFactionsActors[i].IsBurning)
					{
						enemyFactionsActors[i].DealDamage((int)fixedPoint, postDamageAction.DamagerActor, DamageType.Flame);
						enemyFactionsActors[i].NotifyChange("ActorPassiveFlameMessage", ((int)fixedPoint).ToString() ?? "");
					}
				}
				IsCanFlame = false;
			}
			return ActionListClearFlag.Keep;
		}
	}
}
