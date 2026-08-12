using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class RedactTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			try
			{
				PostDamageAction postDamageAction = action as PostDamageAction;
				if (postDamageAction != null && actor != null && postDamageAction.DamagerActor != null && postDamageAction.DamageAction.SourceSupport == null)
				{
					bool flag = false;
					DamageAction damageAction = postDamageAction.DamageAction;
					if (damageAction == null)
					{
						return ActionListClearFlag.Keep;
					}
					if (damageAction.DamageType != DamageType.Melee && damageAction.DamageType != DamageType.Ranged)
					{
						return ActionListClearFlag.Keep;
					}
					CombatModel combatModel = base.manager.CombatModel;
					if (combatModel == null)
					{
						return ActionListClearFlag.Keep;
					}
					ActorModel damagerActor = damageAction.DamagerActor;
					if (damagerActor == null)
					{
						return ActionListClearFlag.Keep;
					}
					if (damagerActor.Faction != Faction.Survivor && damagerActor.Faction != Faction.Raider)
					{
						return ActionListClearFlag.Keep;
					}
					ActorModel targetActor = damageAction.TargetActor;
					if (targetActor == null)
					{
						return ActionListClearFlag.Keep;
					}
					if (targetActor.Faction == Faction.Environmental)
					{
						return ActionListClearFlag.Keep;
					}
					bool flag2 = LeaderHasTheTrait(damagerActor);
					ActorModel actorModel = damagerActor;
					if (flag2)
					{
						ActorModel leader = GetLeader(damagerActor);
						if (leader != null)
						{
							actorModel = leader;
						}
					}
					FixedPoint value = 0.0;
					combatModel.AbilityManager.VisitParameter("LeaderBuffRedactStunChance", ref value, flag2 ? actorModel : damagerActor);
					if (!postDamageAction.TargetActor.IsDead && value > 0.0 && !targetActor.IsStunned)
					{
						PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
						if (value > 0.0)
						{
							playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Stun, value);
						}
						if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && !damagerActor.HasAnyLevelTrait("DebuffMarkEnemy"))
						{
							addedActions.Add(new StunAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, 1, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
							flag = false;
							targetActor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffRedact", false });
						}
						if (damagerActor.HasAnyLevelTrait("DebuffMarkEnemy"))
						{
							targetActor.NotifyChange("ActorRedact", new object[1] { playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension });
						}
					}
					FixedPoint value2 = 0.0;
					combatModel.AbilityManager.VisitParameter("LeaderBuffRedactChance", ref value2, flag2 ? actorModel : damagerActor);
					if (!damagerActor.VisitedRedactChance && value2 > 0.0)
					{
						damagerActor.VisitedRedactChance = true;
						PlayerRandomChanceResult playerRandomChanceResult2 = PlayerRandomChanceResult.Failed;
						if (value2 > 0.0)
						{
							playerRandomChanceResult2 = base.manager.Player.RollDice(RollDiceType.Redact, value2);
						}
						if (playerRandomChanceResult2 != PlayerRandomChanceResult.Failed)
						{
							flag = combatModel.StartRedactTimedEffect(flag2 ? actorModel : damagerActor);
						}
					}
					if (flag)
					{
						damagerActor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffRedact", false });
					}
				}
			}
			catch (Exception)
			{
			}
			return ActionListClearFlag.Keep;
		}

		private bool LeaderHasTheTrait(ActorModel a)
		{
			if (a != null && base.manager != null && base.manager.CombatModel != null)
			{
				SurvivorModel survivorModel = null;
				if (a.Faction == Faction.Raider && base.manager.CombatModel.Raiders != null && base.manager.CombatModel.Raiders.Count > 0 && base.manager.CombatModel.Raiders[0] is SurvivorModel)
				{
					survivorModel = (SurvivorModel)base.manager.CombatModel.Raiders[0];
				}
				if (a.Faction == Faction.Survivor && base.manager.CombatModel.Survivors != null && base.manager.CombatModel.Survivors.Count > 0 && base.manager.CombatModel.Survivors[0] is SurvivorModel)
				{
					survivorModel = (SurvivorModel)base.manager.CombatModel.Survivors[0];
				}
				if (survivorModel != null)
				{
					if (survivorModel.IsLeader)
					{
						return survivorModel.HasAnyLevelTrait("LeaderBuffRedact");
					}
					return false;
				}
			}
			return false;
		}

		private ActorModel GetLeader(ActorModel a)
		{
			SurvivorModel result = null;
			if (a != null && base.manager != null && base.manager.CombatModel != null)
			{
				if (a.Faction == Faction.Raider && base.manager.CombatModel.Raiders != null && base.manager.CombatModel.Raiders.Count > 0 && base.manager.CombatModel.Raiders[0] is SurvivorModel)
				{
					result = (SurvivorModel)base.manager.CombatModel.Raiders[0];
				}
				if (a.Faction == Faction.Survivor && base.manager.CombatModel.Survivors != null && base.manager.CombatModel.Survivors.Count > 0 && base.manager.CombatModel.Survivors[0] is SurvivorModel)
				{
					result = (SurvivorModel)base.manager.CombatModel.Survivors[0];
				}
			}
			return result;
		}
	}
}
