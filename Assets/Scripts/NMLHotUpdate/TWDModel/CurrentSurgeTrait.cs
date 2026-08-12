using System.Collections.Generic;

namespace TWDModel
{
	public class CurrentSurgeTrait : ActionModifier
	{
		private FixedPoint SurgeDamagePercentage;

		public CurrentSurgeTrait(FixedPoint surgeDamagePercentage)
		{
			SurgeDamagePercentage = surgeDamagePercentage;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction { DamagerActor: not null } postDamageAction && actor == postDamageAction.DamagerActor && actor.HasTraitsThatContains("Conductive") && postDamageAction.IsMainTarget && postDamageAction.TargetActor.IsElectricShocked && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.DamagerActor.Faction != Faction.Environmental && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null && !(postDamageAction.DamageAction is DamageConsumableAction))
			{
				if (!postDamageAction.TargetActor.DebuffParameterManager.TryGetParameterValueByParameterKey<int>("ElectronShockAsElectronChargeLayer", out var value))
				{
					return ActionListClearFlag.Keep;
				}
				List<ActorModel> targetCircleEnemies = GetTargetCircleEnemies(postDamageAction.TargetActor, actor);
				if (targetCircleEnemies == null || targetCircleEnemies.Count == 0)
				{
					return ActionListClearFlag.Keep;
				}
				postDamageAction.TargetActor.NotifyChange("ActorElectricSurgedEvent");
				float num = (float)postDamageAction.DamageAction.FinalDamage * (float)SurgeDamagePercentage * (float)value;
				CombatModel combatModel = postDamageAction.DamagerActor.manager.CombatModel;
				foreach (ActorModel item in targetCircleEnemies)
				{
					CombatHelpers.ExecuteDamage(combatModel, null, item, (int)num, 0, DamageType.Surge, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
				}
			}
			return ActionListClearFlag.Keep;
		}

		private List<ActorModel> GetTargetCircleEnemies(ActorModel target, ActorModel source)
		{
			CombatModel combatModel = source.manager.CombatModel;
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(source.GridCoordinate.GetEnemiesByDistanceAndFaction(target.GridCoordinate, combatModel, 1, source.Faction));
			return list.FindAll((ActorModel x) => x != target);
		}
	}
}
