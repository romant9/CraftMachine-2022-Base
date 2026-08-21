using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentPassivePreventControlTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action == null || actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (!(action is StunAction statusEffectAction))
			{
				if (!(action is PitfallAction statusEffectAction2))
				{
					if (!(action is ABTesterAction statusEffectAction3))
					{
						if (!(action is BurningOutAction statusEffectAction4))
						{
							if (!(action is RootAction statusEffectAction5))
							{
								if (!(action is CrippleAction statusEffectAction6))
								{
									if (!(action is StaggerAction statusEffectAction7))
									{
										if (!(action is SkinnedAction statusEffectAction8))
										{
											if (!(action is HerdAction statusEffectAction9))
											{
												if (!(action is DisorientAction statusEffectAction10))
												{
													if (!(action is TauntAction statusEffectAction11))
													{
														if (!(action is RemoteWeakenAction statusEffectAction12))
														{
															if (!(action is ElectricShockAction statusEffectAction13))
															{
																if (!(action is QuantunAction statusEffectAction14))
																{
																	if (!(action is QuantunCanNotMoveAction statusEffectAction15))
																	{
																		if (action is BleedingOutAction { Avoided: false } bleedingOutAction && actor == bleedingOutAction.Target && TryResistEffect(actor, "Bleeding", RollDiceType.AvoidBleed))
																		{
																			bleedingOutAction.Avoided = true;
																		}
																	}
																	else
																	{
																		TryAvoidStatusEffect(statusEffectAction15, actor, "Quantun", RollDiceType.AvoidQuantun);
																	}
																}
																else
																{
																	TryAvoidStatusEffect(statusEffectAction14, actor, "Quantun", RollDiceType.AvoidQuantun);
																}
															}
															else
															{
																TryAvoidStatusEffect(statusEffectAction13, actor, "ElectricShock", RollDiceType.AvoidElectricShock);
															}
														}
														else
														{
															TryAvoidStatusEffect(statusEffectAction12, actor, "RemoteWeakened", RollDiceType.Unknown);
														}
													}
													else
													{
														TryAvoidStatusEffect(statusEffectAction11, actor, "Taunted", RollDiceType.Taunt);
													}
												}
												else
												{
													TryAvoidStatusEffect(statusEffectAction10, actor, "Disoriented", RollDiceType.Disorient);
												}
											}
											else
											{
												TryAvoidStatusEffect(statusEffectAction9, actor, "Herd", RollDiceType.AvoidHerd);
											}
										}
										else
										{
											TryAvoidStatusEffect(statusEffectAction8, actor, "Skinned", RollDiceType.Skinned);
										}
									}
									else
									{
										TryAvoidStatusEffect(statusEffectAction7, actor, "StaggerActive", RollDiceType.AvoidStagger);
									}
								}
								else
								{
									TryAvoidStatusEffect(statusEffectAction6, actor, "Cripple", RollDiceType.AvoidCripple);
								}
							}
							else
							{
								TryAvoidStatusEffect(statusEffectAction5, actor, "Root", RollDiceType.AvoidRoot);
							}
						}
						else
						{
							TryAvoidStatusEffect(statusEffectAction4, actor, "Burning", RollDiceType.AvoidBurn);
						}
					}
					else
					{
						TryAvoidStatusEffect(statusEffectAction3, actor, "ABTesterAed", RollDiceType.AvoidBurn);
					}
				}
				else
				{
					TryAvoidStatusEffect(statusEffectAction2, actor, "Pitfall", RollDiceType.Unknown);
				}
			}
			else
			{
				TryAvoidStatusEffect(statusEffectAction, actor, "Stun", RollDiceType.AvoidStun);
			}
			return ActionListClearFlag.Keep;
		}

		private static bool TryAvoidStatusEffect(StatusEffectAction statusEffectAction, ActorModel actor, string effectTag, RollDiceType diceType)
		{
			if (statusEffectAction == null || statusEffectAction.Avoided || actor != statusEffectAction.TargetActor || !TryResistEffect(actor, effectTag, diceType))
			{
				return false;
			}
			statusEffectAction.Avoided = true;
			return true;
		}

		public static bool TryResistEffect(ActorModel target, string effectTag, RollDiceType diceType = RollDiceType.Unknown)
		{
			if (target == null || target.TraitContainer == null || string.IsNullOrEmpty(effectTag) || !IsSupportedByPreventControl(target, effectTag) || !TryRollResistance(target, diceType))
			{
				return false;
			}
			NotifyResistance(target);
			return true;
		}

		private static bool IsSupportedByPreventControl(ActorModel target, string effectTag)
		{
			TraitEntry traitAnyLevel = target.TraitContainer.GetTraitAnyLevel("Equipment.Passive.PreventControl");
			if (traitAnyLevel == null || string.IsNullOrEmpty(traitAnyLevel.TraitIdentifier))
			{
				return false;
			}
			TraitDefinition traitDefinition = target.manager?.GameEconomyData?.GetTraitDefinition(traitAnyLevel.TraitIdentifier);
			if (traitDefinition?.EffectIndex == null)
			{
				return false;
			}
			for (int i = 0; i < traitDefinition.EffectIndex.Count; i++)
			{
				if (string.Equals(traitDefinition.EffectIndex[i], effectTag, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static bool TryResistFistSpike(ActorModel target)
		{
			return TryResistEffect(target, "FistSpike", RollDiceType.FistSpike);
		}

		private static bool TryRollResistance(ActorModel target, RollDiceType diceType)
		{
			if (target == null)
			{
				return false;
			}
			TWDModelManager tWDModelManager = target.manager;
			AbilityManagerModel abilityManagerModel = (tWDModelManager?.CombatModel)?.AbilityManager;
			PlayerModel playerModel = tWDModelManager?.Player;
			if (abilityManagerModel == null || playerModel == null)
			{
				return false;
			}
			FixedPoint value = 0.0;
			if (!abilityManagerModel.VisitParameter("AbilityModifierEquipmentPassivePreventControlChance", ref value, target) || value <= 0.0)
			{
				return false;
			}
			FixedPoint successProbabilityExtension = 0.0;
			return playerModel.RollDice(diceType, value, successProbabilityExtension) != PlayerRandomChanceResult.Failed;
		}

		private static void NotifyResistance(ActorModel target)
		{
			target?.NotifyChange("AbilityVisited", new object[2] { "Equipment.Passive.PreventControl", false });
		}
	}
}
