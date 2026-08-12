using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class OverwatchTrait : ActionModifier
	{
		private FixedPoint multiplier = 1.0;

		private FixedPoint originalMultiplier;

		public OverwatchTrait()
		{
		}

		public OverwatchTrait(int chance)
		{
			originalMultiplier = chance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = base.manager.CombatModel;
			PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
			bool hadActionPointsAtEndOfTurn = actor.HadActionPointsAtEndOfTurn;
			if (!hadActionPointsAtEndOfTurn)
			{
				FixedPoint value = 0.0;
				if (combatModel.AbilityManager.VisitParameter("PrimedChance", ref value, actor) && !actor.dashTraitAttackFlag)
				{
					FixedPoint value2 = 0.0;
					combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, actor);
					playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.Generic, value, value2);
				}
			}
			MoveAction moveAction = action as MoveAction;
			if (actor.OverwatchedOnTurn || !actor.CanPerformOOT || (playerRandomChanceResult == PlayerRandomChanceResult.Failed && !hadActionPointsAtEndOfTurn) || actor.IsStruggling)
			{
				return ActionListClearFlag.Keep;
			}
			GridModel grid = combatModel.Grid;
			ActorModel model = base.manager.GetModel<ActorModel>(action.ModelId);
			if (combatModel.HasPvPRules && model != null && model.Faction == Faction.Walker && actor.Faction == Faction.Raider)
			{
				ActorModel currentTarget = model.AIDataModel.GetCurrentTarget();
				if (currentTarget == null || currentTarget.Faction != Faction.Raider || model.AIDataModel.Alertness < AIAlertness.Homing)
				{
					return ActionListClearFlag.Keep;
				}
			}
			if (model != null && model.IsSneak)
			{
				return ActionListClearFlag.Keep;
			}
			if (model != null && (model == actor || !model.IsEnemy(actor)))
			{
				return ActionListClearFlag.Keep;
			}
			if (actor.IsAIControlled && model is SurvivorModel && ((SurvivorModel)model).IsMeleeClass)
			{
				FixedPoint value3 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierChangeNotTriggerOverwatch", ref value3, model);
				if (value3 != 0.0)
				{
					PlayerRandomChanceResult playerRandomChanceResult2 = base.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, value3);
					if (playerRandomChanceResult2 != PlayerRandomChanceResult.Failed && !model.dashTraitAttackFlag)
					{
						model.NotifyChange("AbilityVisited", new object[2]
						{
							"LeaderBuffForestStalker",
							playerRandomChanceResult2 == PlayerRandomChanceResult.SuccessDueToExtension
						});
						return ActionListClearFlag.Keep;
					}
				}
			}
			if (moveAction != null && combatModel.TurnManager.ActiveFaction == Faction.Survivor)
			{
				List<ActorModel> list = combatModel.Walkers.Where((ActorModel x) => x.GetActiveLightState()).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					PlayerRandomChanceResult playerRandomChanceResult3 = PlayerRandomChanceResult.Success;
					list[num].NotifyChange("AbilityVisited", new object[2]
					{
						"Equipment_Active_Light",
						playerRandomChanceResult3 == PlayerRandomChanceResult.SuccessDueToExtension
					});
					list[num].ResetActiveLight();
				}
				List<ActorModel> list2 = combatModel.Raiders.Where((ActorModel x) => x.GetActiveLightState()).ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					PlayerRandomChanceResult playerRandomChanceResult4 = PlayerRandomChanceResult.Success;
					list2[num2].NotifyChange("AbilityVisited", new object[2]
					{
						"Equipment_Active_Light",
						playerRandomChanceResult4 == PlayerRandomChanceResult.SuccessDueToExtension
					});
					list2[num2].ResetActiveLight();
				}
				List<ActorModel> list3 = combatModel.Raiders.Where((ActorModel x) => x.GetFreeOWState()).ToList();
				for (int num3 = 0; num3 < list3.Count; num3++)
				{
					if (actor != null && list3[num3] == actor)
					{
						PlayerRandomChanceResult playerRandomChanceResult5 = PlayerRandomChanceResult.Success;
						list3[num3].NotifyChange("AbilityVisited", new object[2]
						{
							"Equipment_Passive_FreeOW",
							playerRandomChanceResult5 == PlayerRandomChanceResult.SuccessDueToExtension
						});
						list3[num3].ResetFreeOW();
						return ActionListClearFlag.Keep;
					}
				}
			}
			if (action is MoveAction moveAction2)
			{
				if (CheckRandomLeaderBuffCitadelEffect(moveAction2.Actor, actor))
				{
					return ActionListClearFlag.Keep;
				}
				List<GridCoordinate> list4 = new List<GridCoordinate>();
				if (!moveAction2.Actor.IsDead && !moveAction2.Actor.IsHerded && !moveAction2.Actor.IsDisoriented)
				{
					AbilityModel selectedAbility = actor.SelectedAbility;
					if (selectedAbility != null)
					{
						for (int num4 = 0; num4 < grid.NumCells; num4++)
						{
							GridCoordinate coordinate = grid.GetCoordinate(num4);
							bool flag = true;
							bool flag2 = true;
							bool flag3 = false;
							bool flag4 = false;
							if (selectedAbility.Definition.RequiresLineOfSight && !combatModel.IsGridCellVisible(actor.GridCoordinate, coordinate))
							{
								flag = false;
							}
							else if (selectedAbility.Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(actor.GridCoordinate, coordinate))
							{
								flag2 = false;
							}
							FixedPoint range = selectedAbility.Definition.AbilityRange;
							if (actor.Faction != Faction.Survivor && actor.AIDataModel.Alertness < AIAlertness.Alerted)
							{
								range = ((actor.ActivationRange < range) ? ((FixedPoint)actor.ActivationRange) : range);
							}
							if (!selectedAbility.IsConsumableAbility)
							{
								CombatHelpers.CalculateRangeExtension(ref range, actor, combatModel.AbilityManager);
							}
							FixedPoint fixedPoint = (range + (selectedAbility.Definition.AbilityTargetDiagonal ? 0.42f : 0f)) * grid.CellSize.X;
							FixedPoint fixedPoint2 = fixedPoint * fixedPoint;
							FixedVec3 position = grid.GetPosition(actor.GridCoordinate);
							FixedVec3 position2 = grid.GetPosition(coordinate);
							if ((position - position2).SqrMagnitude < fixedPoint2)
							{
								flag3 = true;
							}
							flag4 = selectedAbility.IsTargetValid(actor, model);
							bool flag5 = flag && flag2 && flag3 && flag4;
							if (moveAction2.Path.Contains(coordinate) && moveAction2.Path.Start != coordinate && combatModel.GetOccupier(coordinate) == null && flag5)
							{
								list4.Add(coordinate);
							}
						}
					}
				}
				if (list4.Count > 0)
				{
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					if (weaponEquipment != null)
					{
						GridCoordinate end = moveAction2.Path.End;
						GridCoordinate randomElement = base.manager.Player.PlayerRandom.GetRandomElement(list4, remove: false);
						moveAction2.Path.ClipTo(randomElement);
						multiplier = 100.0;
						base.manager.Player.AbilityManager.VisitParameter("PercentageIncreaseOverwatchDamage", ref multiplier, actor);
						base.manager.Player.AbilityManager.VisitParameter("PercentageIncreaseNewOverwatchDamage", ref multiplier, actor);
						multiplier = multiplier * originalMultiplier * 0.009999999776482582;
						weaponEquipment.AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, multiplier);
						OverwatchAttackAction overwatchAttackAction;
						if (base.manager.GameEconomyData.GetFeature("OverwatchDoesNotPreventOOT").Enabled)
						{
							overwatchAttackAction = new OverwatchAttackAction(actor, weaponEquipment.Ability, randomElement, moveAction2.Path.Start, model, OOTType.None, isTriggerExtraAttackDamage: true);
							actor.OverwatchedOnTurn = true;
						}
						else
						{
							overwatchAttackAction = new OverwatchAttackAction(actor, weaponEquipment.Ability, randomElement, moveAction2.Path.Start, model, OOTType.FreeAttack, isTriggerExtraAttackDamage: true);
						}
						addedActions.Add(overwatchAttackAction);
						if (actor.SelectedAbility.PushEffect != null && moveAction2.Actor != null && moveAction2.Actor.HasTraitsThatContains("Equipment_Passive_PassOW") && combatModel.TurnManager.TurnCount >= moveAction2.Actor.NextCanTriggerPassOW)
						{
							moveAction2.Actor.IsTriggerPassOW = true;
						}
						bool flag6 = false;
						if (actor.HasTraitsThatContains("Interruptor") || actor.HasAnyLevelTrait("Equipment_Active_Interruptor"))
						{
							if (!moveAction2.Actor.IsDisoriented)
							{
								FixedPoint value4 = 0.0;
								FixedPoint value5 = 0.0;
								combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseInterruptChance", ref value4, actor);
								combatModel.AbilityManager.VisitParameter("Equipment_Active_Interruptor", ref value4, actor);
								combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value5, actor);
								flag6 = (overwatchAttackAction.Interrupted = base.manager.Player.RollDice(RollDiceType.InterruptAttack, value4, value5) != PlayerRandomChanceResult.Failed);
							}
							MapMissionModel mapMissionModel = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
							if (mapMissionModel != null)
							{
								List<DifficultyIncrementalDebuff> challengeDebuffs = mapMissionModel.GetChallengeDebuffs();
								if (moveAction2.Actor.IsWalker)
								{
									int chance = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffInterruptRate);
									if (base.manager.Player.RollDice(RollDiceType.AvoidInterrupt, chance) == PlayerRandomChanceResult.Success)
									{
										overwatchAttackAction.Interrupted = false;
									}
								}
								else if (moveAction2.Actor.IsRaider)
								{
									int chance2 = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffs, ChallengeDebuffType.DebuffInterruptRateRaider);
									if (base.manager.Player.RollDice(RollDiceType.AvoidInterrupt, chance2) == PlayerRandomChanceResult.Success)
									{
										overwatchAttackAction.Interrupted = false;
									}
								}
							}
						}
						else if (actor.SelectedAbility.PushEffect != null && !moveAction2.Actor.IsDisoriented)
						{
							flag6 = true;
						}
						if (flag6)
						{
							moveAction2.Path.ClearTargetCoordinate();
							moveAction2.Actor.SecondMoveCompleted = true;
							moveAction2.Actor.EndAction();
						}
						else if (randomElement != end)
						{
							MoveAction item = new MoveAction(moveAction2.Actor, GridPath.Create(new List<GridCoordinate> { randomElement, end }), consumeAP: false);
							addedActions.Add(item);
						}
						AIDataModel aIDataModel = moveAction2.Actor.AIController.AIDataModel;
						if (aIDataModel.GetCurrentTarget() == null || aIDataModel.Alertness < AIAlertness.Homing)
						{
							moveAction2.Actor.AIController.AttackTarget(actor);
						}
						AIDataModel aIDataModel2 = actor.AIController.AIDataModel;
						if (aIDataModel2.GetCurrentTarget() == null || aIDataModel2.Alertness < AIAlertness.Homing)
						{
							actor.AIController.AttackTarget(moveAction2.Actor);
						}
					}
					else
					{
						base.Debug.LogError("Could not perform Overwatch for actor [" + actor.ToString() + "]: weapon is NULL");
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		public bool CheckRandomLeaderBuffCitadelEffect(ActorModel movingActor, ActorModel overwatchActor)
		{
			if (overwatchActor == null || !overwatchActor.IsRangedClass)
			{
				return false;
			}
			ActorModel actorModel = null;
			if (movingActor.HasAnyLevelTrait("LeaderBuffCitadel"))
			{
				actorModel = movingActor;
			}
			else if (!movingActor.IsRangedClass)
			{
				ActorModel leaderInFaction = movingActor.GetLeaderInFaction();
				if (leaderInFaction != null && leaderInFaction.HasAnyLevelTrait("LeaderBuffCitadel"))
				{
					actorModel = leaderInFaction;
				}
			}
			if (actorModel == null)
			{
				return false;
			}
			return CheckRandomLeaderBuffCitadelEffect_DownOverWatchPercent_Ranged(actorModel);
		}

		private bool CheckRandomLeaderBuffCitadelEffect_DownOverWatchPercent_Ranged(ActorModel citadelSource)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null || combatModel.AbilityManager == null || base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("LeaderBuffCitadel_DownOverWatchPercent", ref value, citadelSource);
			if (value < 0.0)
			{
				value = 0.0;
			}
			if (value > 1.0)
			{
				value = 1.0;
			}
			FixedPoint fixedPoint = 1.0;
			FixedPoint fixedPoint2 = 1.0 * (fixedPoint - value);
			if (fixedPoint2 < ActorTraitContainerModel.Citadel_PercentBase)
			{
				fixedPoint2 = ActorTraitContainerModel.Citadel_PercentBase;
			}
			if (fixedPoint2 > fixedPoint)
			{
				fixedPoint2 = fixedPoint;
			}
			FixedPoint fixedPoint3 = fixedPoint - fixedPoint2;
			if (fixedPoint3 > 0.0 && base.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, fixedPoint3) != PlayerRandomChanceResult.Failed)
			{
				return true;
			}
			return false;
		}
	}
}
