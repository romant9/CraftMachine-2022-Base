using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class TauntTrait : ActionModifier
	{
		private bool isChargeAttack;

		private int turns;

		private int chargeTurns;

		private int shieldTurns;

		private int leaderShieldTurns;

		public TauntTrait(int turns, int chargeTurns, int shieldTurns, int leaderShieldTurns)
		{
			this.turns = turns;
			this.chargeTurns = chargeTurns;
			this.shieldTurns = shieldTurns;
			this.leaderShieldTurns = leaderShieldTurns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			try
			{
				PostDamageAction postDamageAction = action as PostDamageAction;
				if (postDamageAction != null)
				{
					ActorModel damagerActor = postDamageAction.DamagerActor;
					ActorModel targetActor = postDamageAction.TargetActor;
					CombatModel combatModel = base.manager.CombatModel;
					if (damagerActor == null || actor == null || combatModel == null || targetActor == null)
					{
						return ActionListClearFlag.Keep;
					}
					if (damagerActor == actor && postDamageAction.DamageAction.SourceSupport == null && (targetActor.IsWalker || targetActor.IsRaider) && actor.HasAnyLevelTrait("LeaderBuffProtect") && !targetActor.IsDead)
					{
						int num = (isChargeAttack ? chargeTurns : turns);
						addedActions.Add(new TauntAction(damagerActor, targetActor, num, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
					}
					if (damagerActor == actor && postDamageAction.DamageAction.SourceSupport == null && (targetActor.IsWalker || targetActor.IsRaider) && isChargeAttack && actor.HasAnyLevelTrait("LeaderBuffProtect"))
					{
						FixedPoint value = actor.MaxHitPoints;
						if (IsLeader(actor))
						{
							string paramName = "LeaderBuffProtectLeaderShieldChance";
							int num2 = leaderShieldTurns;
							if (num2 > 0 && combatModel.AbilityManager.VisitParameter(paramName, ref value, damagerActor))
							{
								List<ActorModel> list = combatModel.Survivors?.Models;
								if (list != null)
								{
									for (int num3 = 0; num3 < list.Count; num3++)
									{
										ActorModel actorModel = list[num3];
										if (actorModel != null)
										{
											addedActions.Add(new ShieldAction(actorModel, targetActor, num2, (int)value));
										}
									}
								}
							}
						}
						else if (combatModel.AbilityManager.VisitParameter("LeaderBuffProtectShieldChance", ref value, damagerActor) && shieldTurns > 0)
						{
							addedActions.Add(new ShieldAction(damagerActor, targetActor, shieldTurns, (int)value));
						}
					}
				}
				else if (action is AbilityAction abilityAction && !abilityAction.Ability.IsConsumableAbility)
				{
					isChargeAttack = abilityAction.Ability.IsChargeAttack;
				}
			}
			catch (Exception arg)
			{
				base.Debug.LogError($"TauntTrait Error:{arg}");
			}
			return ActionListClearFlag.Keep;
		}

		private bool IsLeader(ActorModel a)
		{
			if (a is SurvivorModel survivorModel)
			{
				if (survivorModel.IsLeader)
				{
					return survivorModel.HasAnyLevelTrait("LeaderBuffProtect");
				}
				return false;
			}
			return false;
		}
	}
}
