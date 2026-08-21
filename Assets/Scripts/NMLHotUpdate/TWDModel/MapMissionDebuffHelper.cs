using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public static class MapMissionDebuffHelper
	{
		public static IChallengeDebuffProvider CanUseDebuffMission(TWDModelManager manager)
		{
			if (manager == null)
			{
				return null;
			}
			if (manager.Player == null)
			{
				return null;
			}
			MapMissionModel mapMissionModel = manager.Player.GetAttackTargetMissionModel() as MapMissionModel;
			WorldBossMissionModel worldBossMissionModel = manager.Player.GetAttackTargetMissionModel() as WorldBossMissionModel;
			if (mapMissionModel == null)
			{
				if (worldBossMissionModel != null)
				{
					return worldBossMissionModel;
				}
				return null;
			}
			if (mapMissionModel.IsInWeeklyChallenge)
			{
				return mapMissionModel;
			}
			if (mapMissionModel.IsInApocalyptiWeeklyChallenge)
			{
				return mapMissionModel;
			}
			if (mapMissionModel.IsEndlessMission && manager.Player.EndlessModeManager != null && manager.Player.EndlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				return mapMissionModel;
			}
			return null;
		}

		public static bool CheckChallengeDebuffAvoid(IChallengeDebuffProvider provider, TWDModelManager manager, ChallengeDebuffType challengeDebuffType, RollDiceType rollDiceType)
		{
			if (provider == null || manager?.Player == null)
			{
				return false;
			}
			int chance = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(provider.GetChallengeDebuffs(), challengeDebuffType);
			return manager.Player.RollDice(rollDiceType, chance) != PlayerRandomChanceResult.Failed;
		}

		public static bool CheckChallengeHardtoAim(IChallengeDebuffProvider provider, TWDModelManager manager, ActorModel source, ActorModel target)
		{
			if (provider == null || manager?.Player == null || source == null || target == null)
			{
				return false;
			}
			int num = source.GridCoordinate.SquaredDistanceTo(target.GridCoordinate);
			foreach (List<FixedPoint> item in ChallengeDebufHelps.GetDebufAllParam(provider.GetChallengeDebuffs(), ChallengeDebuffType.HardtoAim))
			{
				if (item[0] * item[0] < num && manager.Player.RollDice(RollDiceType.Dodge, item[1]) != PlayerRandomChanceResult.Failed)
				{
					return true;
				}
			}
			return false;
		}

		public static bool CheckChallengeWalkerDodge(IChallengeDebuffProvider provider, TWDModelManager manager, ActorModel source, ActorModel target)
		{
			if (provider == null || manager?.Player == null || source == null || target == null || !target.IsWalker)
			{
				return false;
			}
			List<DifficultyIncrementalDebuff> challengeDebuffs = provider.GetChallengeDebuffs();
			FixedPoint successProbability = (10000L - MapMissionModel.GetChallengeActorHit(source, challengeDebuffs, manager.GameEconomyData.ConfigData.MinHit)) / 10000.0;
			return manager.Player.RollDice(RollDiceType.Dodge, successProbability) != PlayerRandomChanceResult.Failed;
		}

		public static bool IsCombatSameClass(TWDModelManager manager)
		{
			if (manager?.CombatModel != null)
			{
				return (from x in manager.CombatModel.MissionRoster
					group x by x.SurvivorClass into g
					select g.Count()).Max() >= 3;
			}
			return false;
		}

		public static void VisitChallengeDebuffActions(IChallengeDebuffProvider provider, TWDModelManager manager, ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (provider == null || manager?.Player == null || action == null)
			{
				return;
			}
			if (!(action is StunAction stunAction))
			{
				if (!(action is RootAction rootAction))
				{
					if (!(action is CrippleAction crippleAction))
					{
						if (!(action is BurningOutAction burningOutAction))
						{
							if (!(action is DamageAction damageAction) || damageAction is DamageConsumableAction || damageAction.DamagerActor == null)
							{
								return;
							}
							ApocalypseWeeklyChallengeModel apocalypseWeeklyChallenge = manager.Player.ApocalypseWeeklyChallenge;
							int num = 0;
							List<DifficultyIncrementalDebuff> challengeDebuffs = provider.GetChallengeDebuffs();
							if (damageAction.DamagerActor.Faction == Faction.Survivor)
							{
								SurvivorModel survivorModel = damageAction.DamagerActor as SurvivorModel;
								ActorModel targetActor = damageAction.TargetActor;
								if (CheckChallengeHardtoAim(provider, manager, survivorModel, targetActor))
								{
									damageAction.Dodged = true;
									return;
								}
								if (CheckChallengeWalkerDodge(provider, manager, survivorModel, targetActor))
								{
									damageAction.Dodged = true;
									return;
								}
								if (targetActor.IsWalker)
								{
									EquipmentItemModel weaponEquipment = survivorModel.GetWeaponEquipment();
									if (weaponEquipment != null && weaponEquipment.Definition.Category == EquipmentCategory.RangeWeapon && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffRangeDodge, RollDiceType.Dodge))
									{
										damageAction.Dodged = true;
										return;
									}
									foreach (List<FixedPoint> item in ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.DebuffDmgreductionPercentage))
									{
										if (item.Count > 0)
										{
											num -= (int)item[(targetActor.DamageCount > item.Count - 1) ? (item.Count - 1) : targetActor.DamageCount];
										}
									}
								}
								if (targetActor.IsRaider)
								{
									EquipmentItemModel weaponEquipment2 = survivorModel.GetWeaponEquipment();
									if (weaponEquipment2 != null && weaponEquipment2.Definition.Category == EquipmentCategory.RangeWeapon && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffRangeDodgeRaider, RollDiceType.Dodge))
									{
										damageAction.Dodged = true;
										return;
									}
									foreach (List<FixedPoint> item2 in ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.DebuffDmgreductionPercentageRaider))
									{
										if (item2.Count > 0)
										{
											num -= (int)item2[(targetActor.DamageCount > item2.Count - 1) ? (item2.Count - 1) : targetActor.DamageCount];
										}
									}
								}
								if (provider.IsInApocalyptiWeeklyChallenge)
								{
									num += (int)apocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.FinalDamage);
									if (survivorModel.GridCoordinate.SquaredDistanceTo(targetActor.GridCoordinate) <= 4)
									{
										num += (int)apocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.CloseCombat);
									}
									if (targetActor.IsChallengeApocalypseEffectDmgIncStatus)
									{
										num += (int)apocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.EffectDmgInc);
									}
									num += (int)apocalypseWeeklyChallenge.GetApocalypseBuffClassDmgUp(survivorModel.SurvivorClass);
									if (IsCombatSameClass(manager))
									{
										num += (int)apocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.SameClass);
									}
									if (damageAction.IsTriggerExtraAttackDamage)
									{
										num += (int)apocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.BonusAttack);
									}
								}
								num -= (int)ChallengeDebufHelps.GetDmgReductionByClass(challengeDebuffs, survivorModel.SurvivorClass);
							}
							else if (damageAction.DamagerActor.IsWalker && provider.IsInApocalyptiWeeklyChallenge)
							{
								num -= (int)apocalypseWeeklyChallenge.GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.DefenseInc);
							}
							num = Math.Max(num, -100);
							damageAction.CalculateFinalDamage();
							damageAction.ModifyDamage += damageAction.FinalDamage * num / 100;
							if (damageAction.TargetActor.IsWalker && damageAction.DamageType != DamageType.Explode)
							{
								List<List<FixedPoint>> debufAllParam = ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.PercentageDmgReduction);
								debufAllParam.OrderByDescending((List<FixedPoint> x) => x[0]);
								foreach (List<FixedPoint> item3 in debufAllParam)
								{
									damageAction.CalculateFinalDamage();
									if (damageAction.FinalDamage >= damageAction.TargetActor.MaxHitPoints * item3[0] / 100L)
									{
										damageAction.ModifyDamage -= (int)(damageAction.FinalDamage * item3[1] / 100L);
										break;
									}
								}
							}
							if (!damageAction.TargetActor.IsRaider || damageAction.DamageType == DamageType.Explode)
							{
								return;
							}
							List<List<FixedPoint>> debufAllParam2 = ChallengeDebufHelps.GetDebufAllParam(challengeDebuffs, ChallengeDebuffType.PercentageDmgReductionRaider);
							debufAllParam2.OrderByDescending((List<FixedPoint> x) => x[0]);
							{
								foreach (List<FixedPoint> item4 in debufAllParam2)
								{
									damageAction.CalculateFinalDamage();
									if (damageAction.FinalDamage >= damageAction.TargetActor.MaxHitPoints * item4[0] / 100L)
									{
										damageAction.ModifyDamage -= (int)(damageAction.FinalDamage * item4[1] / 100L);
										break;
									}
								}
								return;
							}
						}
						if (burningOutAction.TargetActor.IsWalker && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffFireRate, RollDiceType.AvoidBurn))
						{
							burningOutAction.Avoided = true;
						}
						if (burningOutAction.TargetActor.IsRaider && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffFireRateRaider, RollDiceType.AvoidBurn))
						{
							burningOutAction.Avoided = true;
						}
					}
					else
					{
						if (crippleAction.TargetActor.IsWalker && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffCrippleRate, RollDiceType.AvoidCripple))
						{
							crippleAction.Avoided = true;
						}
						if (crippleAction.TargetActor.IsRaider && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffCrippleRateRaider, RollDiceType.AvoidCripple))
						{
							crippleAction.Avoided = true;
						}
					}
				}
				else
				{
					if (rootAction.TargetActor.IsWalker && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffRootRate, RollDiceType.AvoidRoot))
					{
						rootAction.Avoided = true;
					}
					if (rootAction.TargetActor.IsRaider && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffRootRateRaider, RollDiceType.AvoidRoot))
					{
						rootAction.Avoided = true;
					}
				}
			}
			else
			{
				if (stunAction.TargetActor.IsWalker && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffStunRate, RollDiceType.AvoidStun) && stunAction.CanNotAvoidStunType == CanNotAvoidStunType.None)
				{
					stunAction.Avoided = true;
				}
				if (stunAction.TargetActor.IsRaider && CheckChallengeDebuffAvoid(provider, manager, ChallengeDebuffType.DebuffStunRateRaider, RollDiceType.AvoidStun) && stunAction.CanNotAvoidStunType == CanNotAvoidStunType.None)
				{
					stunAction.Avoided = true;
				}
			}
		}
	}
}
