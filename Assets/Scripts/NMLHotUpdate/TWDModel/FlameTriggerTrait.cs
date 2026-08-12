using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class FlameTriggerTrait : ActionModifier
	{
		private bool isCurrentBurnning;

		private bool isChargeAttack;

		private ActorModel attackMainTarget;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			try
			{
				if (action is AbilityAction abilityAction && actor != null && abilityAction.Actor == actor && !abilityAction.Ability.IsConsumableAbility)
				{
					isCurrentBurnning = false;
					isChargeAttack = abilityAction.Ability.IsChargeAttack;
					ActorModel occupier = base.manager.CombatModel.GetOccupier(abilityAction.TargetCell);
					if (occupier != null && !occupier.IsEnvironmental)
					{
						isCurrentBurnning = occupier.IsBurning;
						attackMainTarget = occupier;
					}
				}
				if (action is PostDamageAction postDamageAction && actor != null && postDamageAction.DamageAction != null && postDamageAction.DamagerActor != null && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor != null && !postDamageAction.TargetActor.IsEnvironmental && postDamageAction.DamageAction.SourceSupport == null && !postDamageAction.TargetActor.IsDead)
				{
					if (postDamageAction.TargetActor.IsBurning && !actor.VisitedExtraApChance)
					{
						actor.VisitedExtraApChance = true;
						FixedPoint value = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_ChargePointChance", ref value, actor);
						FixedPoint value2 = 0.0;
						if (value != 0.0)
						{
							base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
						}
						PlayerRandomChanceResult playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.GainAP, value, value2);
						actor.EnsureExtraAP = playerRandomChanceResult != PlayerRandomChanceResult.Failed;
						actor.NotifyChange("AbilityVisited", new object[2]
						{
							"LeaderBuffNoExceptions",
							playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
					if (!postDamageAction.TargetActor.IsBurning)
					{
						FixedPoint value3 = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value3, actor);
						FixedPoint value4 = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_SetFireChance", ref value4, actor);
						if (postDamageAction.TargetActor.AttributeModel?.GetAttributeModelValue("burn_be_ratio") != 0L)
						{
							FixedPoint value5 = value4;
							FixedPoint value6 = 1L;
							FixedPoint? obj = postDamageAction.TargetActor.AttributeModel?.GetAttributeModelValue("burn_be_ratio");
							FixedPoint? fixedPoint = value6 + obj;
							value4 = (value5 * fixedPoint).Value;
						}
						if (base.manager.Player.RollDice(RollDiceType.Burn, value4, value3) != PlayerRandomChanceResult.Failed)
						{
							addedActions.Add(new BurningOutAction(actor, postDamageAction.TargetActor, onRedHealthBar: false));
						}
					}
					if (postDamageAction.TargetActor.IsBurning && isChargeAttack)
					{
						FixedPoint value7 = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_BurnLayerChance", ref value7, actor);
						FixedPoint value8 = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value8, actor);
						if (base.manager.Player.RollDice(RollDiceType.ExtraBurnLayer, value7, value8) != PlayerRandomChanceResult.Failed)
						{
							FixedPoint value9 = 0.0;
							base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_MaxBurnLayer", ref value9, actor);
							postDamageAction.TargetActor.ExtraBurnLayer = UtilsMath.Clamp(postDamageAction.TargetActor.ExtraBurnLayer + 1, 0, (int)value9);
							FixedPoint value10 = 0.0;
							base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_BurnLayerTurn", ref value10, actor);
							postDamageAction.TargetActor.ExtraBurnTurn = (int)value10;
							postDamageAction.TargetActor.NotifyChange("PerlieFlameTrigger");
						}
					}
				}
				if (action is AbilityBeforeRemoveActiveTraitAction { AbilityAction: not null } abilityBeforeRemoveActiveTraitAction && actor != null && abilityBeforeRemoveActiveTraitAction.AbilityAction.Actor == actor && !abilityBeforeRemoveActiveTraitAction.AbilityAction.Ability.IsConsumableAbility)
				{
					FlameTrigger(actor, attackMainTarget, isCurrentBurnning);
					isCurrentBurnning = attackMainTarget.IsBurning;
				}
				if (action is ChangeTurnAction && actor != null && !base.manager.CombatModel.MissionCompleted && actor.Faction == base.manager.CombatModel.TurnManager.ActiveFaction)
				{
					base.manager.CombatModel.CurrentTurnFlameTriggerCount = 0;
				}
			}
			catch (Exception arg)
			{
				base.Debug.LogError($"FlameTriggerTrait Error:{arg}");
			}
			return ActionListClearFlag.Keep;
		}

		private bool CheckLeader(ActorModel a)
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
						return survivorModel.HasAnyLevelTrait("LeaderBuffNoExceptions");
					}
					return false;
				}
			}
			return false;
		}

		private void FlameTrigger(ActorModel actor, ActorModel target, bool isBurning)
		{
			FixedPoint value = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_LeaderMaxTriggerCount", ref value, actor);
			FixedPoint value2 = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_MaxTriggerCount", ref value2, actor);
			if (!(target != null && isBurning) || target.IsEnvironmental || !(base.manager.CombatModel.CurrentTurnFlameTriggerCount < (CheckLeader(actor) ? value : value2)))
			{
				return;
			}
			int burnLayer = ((target.ExtraBurnTurn > 0) ? target.ExtraBurnLayer : 0);
			FixedPoint dealBurningDamage = target.GetDealBurningDamage(burnLayer);
			target.FlameBurningDamage(dealBurningDamage);
			target.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffNoExceptions", false });
			base.manager.CombatModel.CurrentTurnFlameTriggerCount++;
			FixedPoint value3 = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_FlameTriggerRange", ref value3, actor);
			FixedPoint value4 = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_MaxEnemy", ref value4, actor);
			FixedPoint value5 = 0.0;
			base.manager.CombatModel.AbilityManager.VisitParameter("LeaderBuffNoExceptions_BurnDamageRatio", ref value5, actor);
			int num = 0;
			List<ActorModel> allActors = base.manager.CombatModel.GetAllActors();
			if (allActors == null || allActors.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < allActors.Count; i++)
			{
				ActorModel actorModel = allActors[i];
				if (actorModel != null && actorModel.IsEnemy(actor) && actorModel != target && CombatHelpers.IsWithinRange(base.manager.CombatModel, (int)value3, target.GridCoordinate, actorModel.GridCoordinate) && num < value4)
				{
					num++;
					actorModel.FlameTriggerDamage((int)(dealBurningDamage * value5));
				}
			}
		}
	}
}
