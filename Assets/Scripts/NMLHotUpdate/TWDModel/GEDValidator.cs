using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class GEDValidator
	{
		public List<GEDValidationErrorInfo> ErrorInfos { get; private set; }

		public GEDValidator()
		{
			ErrorInfos = new List<GEDValidationErrorInfo>();
		}

		public bool IsNotNullOrEmpty(string s)
		{
			if (s != null)
			{
				return s.Length > 0;
			}
			return false;
		}

		private void AddError(string message)
		{
			ErrorInfos.Add(new GEDValidationErrorInfo
			{
				Message = "[Error] " + message,
				Exception = null
			});
		}

		private void AddLocalizationError(string message)
		{
			ErrorInfos.Add(new GEDValidationErrorInfo
			{
				Message = "[Localization] " + message,
				Exception = null
			});
		}

		private void AddException(Exception e)
		{
			ErrorInfos.Add(new GEDValidationErrorInfo
			{
				Message = "[Exception] " + e.Message,
				Exception = e
			});
		}

		public int Validate(GameEconomyData ged, LocalizationExistsHandler localizationExists = null)
		{
			ErrorInfos.Clear();
			if (ged.ConfigData.SuppliesToDiamondsConversion.Count % 2 == 1)
			{
				AddError("Odd number of entries in Config.SuppliesToDiamondsConversion");
			}
			if (ged.ConfigData.PhoneToDiamondsConversion.Count % 2 == 1)
			{
				AddError("Odd number of entries in Config.TimeToDiamondsConversion");
			}
			if (ged.ConfigData.SPToDiamondsConversion.Count % 2 == 1)
			{
				AddError("Odd number of entries in Config.SPToDiamondsConversion");
			}
			if (ged.ConfigData.OutpostToDiamondsConversion.Count % 2 == 1)
			{
				AddError("Odd number of entries in Config.OutpostToDiamondsConversion");
			}
			if (ged.ConfigData.TimeToDiamondsConversion.Count % 2 == 1)
			{
				AddError("Odd number of entries in Config.TimeToDiamondsConversion");
			}
			QuestDefinition[] questDefinitions = ged.QuestDefinitions;
			foreach (QuestDefinition questDefinition in questDefinitions)
			{
				try
				{
					if (IsNotNullOrEmpty(questDefinition.AdditionalActor) && ged.GetActorDefinition(questDefinition.AdditionalActor) == null)
					{
						AddError($"Could not find additional actor with ID '{questDefinition.AdditionalActor}' for quest definition with id '{questDefinition.Identifier}'.");
					}
					if (localizationExists != null)
					{
						if (IsNotNullOrEmpty(questDefinition.BriefingKey) && !localizationExists(questDefinition.BriefingKey))
						{
							AddLocalizationError($"Quest '{questDefinition.Identifier}' BriefingKey '{questDefinition.BriefingKey}' is missing.");
						}
						if (IsNotNullOrEmpty(questDefinition.DebriefingKey) && !localizationExists(questDefinition.DebriefingKey))
						{
							AddLocalizationError($"Quest '{questDefinition.Identifier}' DebriefingKey '{questDefinition.DebriefingKey}' is missing.");
						}
						if (IsNotNullOrEmpty(questDefinition.CompletionKey) && !localizationExists(questDefinition.CompletionKey))
						{
							AddLocalizationError($"Quest '{questDefinition.Identifier}' CompletionKey '{questDefinition.CompletionKey}' is missing.");
						}
						if (IsNotNullOrEmpty(questDefinition.FailureKey) && !localizationExists(questDefinition.FailureKey))
						{
							AddLocalizationError($"Quest '{questDefinition.Identifier}' FailureKey '{questDefinition.FailureKey}' is missing.");
						}
						if (IsNotNullOrEmpty(questDefinition.TitleKey) && !localizationExists(questDefinition.TitleKey))
						{
							AddLocalizationError($"Quest '{questDefinition.Identifier}' TitleKey '{questDefinition.TitleKey}' is missing.");
						}
					}
					if (IsNotNullOrEmpty(questDefinition.ClassName) && questDefinition.ClassName == "MissionQuest" && ged.MissionSpawnPointData.GetSpawnPointGroupByMapId(questDefinition.GetMissionQuestMapId()) == null)
					{
						AddError($"Quest '{questDefinition.Identifier}' has spawn point group with map id '{questDefinition.GetMissionQuestMapId()}' which cannot be found.");
					}
					if (!IsNotNullOrEmpty(questDefinition.Rewards))
					{
						continue;
					}
					foreach (IReward rewards in questDefinition.GetRewards().RewardsList)
					{
						if (rewards is RewardEquipment rewardEquipment && (!IsNotNullOrEmpty(rewardEquipment.EquipmentId) || ged.GetEquipmentDefinition(rewardEquipment.EquipmentId) == null))
						{
							AddError(string.Format("Quest '{0}' has equipment reward with ID '{1}' which cannot be found.", questDefinition.Identifier, (rewardEquipment.EquipmentId != null) ? rewardEquipment.EquipmentId : "<empty>"));
						}
					}
				}
				catch (Exception e)
				{
					AddException(e);
				}
			}
			EquipmentDefinition[] equipmentDefinitions = ged.EquipmentDefinitions;
			foreach (EquipmentDefinition equipmentDefinition in equipmentDefinitions)
			{
				if (IsNotNullOrEmpty(equipmentDefinition.AbilityIdentifier) && ged.GetAbilityDefinition(equipmentDefinition.AbilityIdentifier) == null)
				{
					AddError($"Could not find ability with ID '{equipmentDefinition.AbilityIdentifier}' for equipment definition with id '{equipmentDefinition.ID}'.");
				}
				if (equipmentDefinition.ActiveTraits != null)
				{
					foreach (string activeTrait in equipmentDefinition.ActiveTraits)
					{
						if (IsNotNullOrEmpty(activeTrait) && ged.GetTraitDefinition(activeTrait) == null)
						{
							AddError($"Could not find active trait with ID '{activeTrait}' for equipment definition with id '{equipmentDefinition.ID}'.");
						}
					}
				}
				if (equipmentDefinition.PassiveTraits != null)
				{
					foreach (string passiveTrait in equipmentDefinition.PassiveTraits)
					{
						if (IsNotNullOrEmpty(passiveTrait) && ged.GetTraitDefinition(passiveTrait) == null)
						{
							AddError($"Could not find passive trait with ID '{passiveTrait}' for equipment definition with id '{equipmentDefinition.ID}'.");
						}
					}
				}
				if (IsNotNullOrEmpty(equipmentDefinition.ChargeEquipmentIdentifier) && ged.GetEquipmentDefinition(equipmentDefinition.ChargeEquipmentIdentifier) == null)
				{
					AddError($"Could not find charge equipment with ID '{equipmentDefinition.ChargeEquipmentIdentifier}' for equipment definition with id {equipmentDefinition.ID}.");
				}
			}
			TraitDefinition[] traitDefinitions = ged.TraitDefinitions;
			foreach (TraitDefinition traitDefinition in traitDefinitions)
			{
				if (localizationExists != null && traitDefinition.ProbabilityWeight > 0L)
				{
					if (traitDefinition.DisplayName != null && traitDefinition.ConstructionParameters != null && traitDefinition.ConstructionParameters.Count > 0)
					{
						string text = traitDefinition.DisplayName + ".Description{Parameter}";
						if (IsNotNullOrEmpty(traitDefinition.DisplayName) && !localizationExists(text))
						{
							AddLocalizationError($"Trait '{traitDefinition.Identifier}' DisplayName (Desc) '{text}' is missing.");
						}
					}
					if (IsNotNullOrEmpty(traitDefinition.DisplayName) && !localizationExists(traitDefinition.DisplayName))
					{
						AddLocalizationError($"Trait '{traitDefinition.Identifier}' DisplayName '{traitDefinition.DisplayName}' is missing.");
					}
				}
				if (IsNotNullOrEmpty(traitDefinition.ClassName) && ReflectionUtils.FindDerivedTypeStartingWith(typeof(ModelModifier), traitDefinition.GetTraitClassName()) == null)
				{
					AddError($"Trait with ID '{traitDefinition.Identifier}' has class specified as '{traitDefinition.GetTraitClassName()}' which is unrecognizable class.");
				}
			}
			foreach (AbilityDefinition abilityDefinition in ged.AbilityDefinitions)
			{
				if (IsNotNullOrEmpty(abilityDefinition.LinkedAbilityIdentifier) && ged.GetAbilityDefinition(abilityDefinition.LinkedAbilityIdentifier) == null)
				{
					AddError($"Ability '{abilityDefinition.Identifier}' has linked ability '{abilityDefinition.LinkedAbilityIdentifier}' which cannot be found.");
				}
				if (abilityDefinition.EffectDefinitions != null)
				{
					foreach (AbilityEffectDefinition effectDefinition in abilityDefinition.EffectDefinitions)
					{
						if (!string.IsNullOrEmpty(effectDefinition.Type) && ReflectionUtils.FindDerivedType(typeof(AbilityEffect), effectDefinition.Type) == null)
						{
							AddError($"Ability '{abilityDefinition.Identifier}' has effect which points to non-existing class '{effectDefinition.Type}'.");
						}
					}
				}
				if (abilityDefinition.Modifiers == null)
				{
					continue;
				}
				foreach (AbilityModifierDefinition modifier in abilityDefinition.Modifiers)
				{
					if (!string.IsNullOrEmpty(modifier.Type) && ReflectionUtils.FindDerivedType(typeof(ModelModifier), modifier.Type) == null)
					{
						AddError($"Ability '{abilityDefinition.Identifier}' has modifier which points to non-existing class '{modifier.Type}'.");
					}
				}
			}
			foreach (ActorDefinition actorDefinition in ged.ActorDefinitions)
			{
				if (actorDefinition.InitialAbilities != null)
				{
					foreach (string initialAbility in actorDefinition.InitialAbilities)
					{
						if (!string.IsNullOrEmpty(initialAbility) && ged.GetAbilityDefinition(initialAbility) == null)
						{
							AddError($"Actor '{actorDefinition.ID}' has initial ability '{initialAbility}' which cannot be found.");
						}
					}
				}
				if (actorDefinition.InitialEquipmentsData != null)
				{
					foreach (EquipmentSetupData initialEquipmentsDatum in actorDefinition.InitialEquipmentsData)
					{
						if (!string.IsNullOrEmpty(initialEquipmentsDatum.ID) && ged.GetEquipmentDefinition(initialEquipmentsDatum.ID) == null)
						{
							AddError($"Actor '{actorDefinition.ID}' has initial equipment '{initialEquipmentsDatum.ID}' which cannot be found.");
						}
					}
				}
				if (actorDefinition.InitialTraits != null)
				{
					foreach (string initialTrait in actorDefinition.InitialTraits)
					{
						if (!string.IsNullOrEmpty(initialTrait) && ged.GetTraitDefinition(initialTrait) == null)
						{
							AddError($"Actor '{actorDefinition.ID}' has initial trait '{initialTrait}' which cannot be found.");
						}
					}
				}
				if (actorDefinition.PvPTraits == null)
				{
					continue;
				}
				foreach (string pvPTrait in actorDefinition.PvPTraits)
				{
					if (!string.IsNullOrEmpty(pvPTrait) && ged.GetTraitDefinition(pvPTrait) == null)
					{
						AddError($"Actor '{actorDefinition.ID}' has PvP trait '{pvPTrait}' which cannot be found.");
					}
				}
			}
			foreach (WeeklyChallenge weeklyChallenge in ged.WeeklyChallenges)
			{
				if (weeklyChallenge.Identifier < 0)
				{
					AddError($"WeeklyChallenge '{weeklyChallenge.Identifier}' has invalid ID, it should be a positive integer number.");
				}
				if (ged.MissionSpawnPointData.GetSpawnPointGroup(weeklyChallenge.DetailMapId) == null)
				{
					AddError($"WeeklyChallenge '{weeklyChallenge.Identifier}' has invalid map id '{weeklyChallenge.DetailMapId}'.");
				}
				if (ged.MissionSpawnPointData.GetSpawnPointGroup(weeklyChallenge.ApocalypticMapId) == null)
				{
					AddError($"WeeklyChallenge '{weeklyChallenge.Identifier}' has invalid map id '{weeklyChallenge.ApocalypticMapId}'.");
				}
				if (weeklyChallenge.EndTimeMilliseconds <= weeklyChallenge.StartTimeMilliseconds)
				{
					AddError($"WeeklyChallenge '{weeklyChallenge.Identifier}' end time is before or equal to start time.");
				}
			}
			foreach (WeeklySurvival weeklySurvival in ged.WeeklySurvivals)
			{
				if (weeklySurvival.Identifier < 0)
				{
					AddError($"WeeklySurvival '{weeklySurvival.Identifier}' has invalid ID, it should be a positive integer number.");
				}
				if (ged.MissionSpawnPointData.GetSpawnPointGroup(weeklySurvival.DetailMapId) == null)
				{
					AddError($"WeeklySurvival '{weeklySurvival.Identifier}' has invalid map id '{weeklySurvival.DetailMapId}'.");
				}
				if (weeklySurvival.EndTimeMilliseconds <= weeklySurvival.StartTimeMilliseconds)
				{
					AddError($"WeeklySurvival '{weeklySurvival.Identifier}' end time is before or equal to start time.");
				}
			}
			for (int j = 0; j < ged.BundleContentDefinitions.Count; j++)
			{
				BundleContentDefinition bundleContentDefinition = ged.BundleContentDefinitions[j];
				if (!string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
				{
					bool flag = false;
					for (int k = 0; k < ged.InAppPurchaseProductsApple.Length; k++)
					{
						if (ged.InAppPurchaseProductsApple[k].Id == bundleContentDefinition.IAPProduct)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						AddError($"Bundle '{bundleContentDefinition.Identifier}' has invalid IAP identifier, could not find it from PurchaseProducts table.");
					}
				}
				if (bundleContentDefinition.Rewards != null)
				{
					try
					{
						new Rewards(bundleContentDefinition.Rewards, null, 0, EquipmentSource.Bundle);
					}
					catch (Exception ex)
					{
						AddError($"Bundle '{bundleContentDefinition.Identifier}' has invalid rewards string. {ex.Message}");
					}
				}
			}
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			Dictionary<string, List<BundleStoreDefinition>> dictionary = new Dictionary<string, List<BundleStoreDefinition>>();
			for (int l = 0; l < ged.BundleStoreDefinitions.Count; l++)
			{
				BundleStoreDefinition bundleStoreDefinition = ged.BundleStoreDefinitions[l];
				BundleContentDefinition bundleContentDefinition2 = null;
				for (int m = 0; m < ged.BundleContentDefinitions.Count; m++)
				{
					if (bundleStoreDefinition.BundleIdentifier == ged.BundleContentDefinitions[m].Identifier)
					{
						bundleContentDefinition2 = ged.BundleContentDefinitions[m];
						break;
					}
				}
				if (bundleContentDefinition2 == null)
				{
					AddError($"Could not find bundle content with identifier '{bundleStoreDefinition.BundleIdentifier}'.");
				}
				if (!string.IsNullOrEmpty(bundleStoreDefinition.PreviousBundle))
				{
					bool flag2 = false;
					for (int n = 0; n < ged.BundleContentDefinitions.Count; n++)
					{
						if (bundleStoreDefinition.PreviousBundle == ged.BundleContentDefinitions[n].Identifier)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						AddError($"Could not find bundle content with identifier '{bundleStoreDefinition.PreviousBundle}' in bundle store definition '{bundleStoreDefinition.BundleIdentifier}'.");
					}
				}
				if ((!string.IsNullOrEmpty(bundleStoreDefinition.StartTimestamp) && string.IsNullOrEmpty(bundleStoreDefinition.EndTimestamp)) || (string.IsNullOrEmpty(bundleStoreDefinition.StartTimestamp) && !string.IsNullOrEmpty(bundleStoreDefinition.EndTimestamp)))
				{
					AddError($"Bundle store has invalid timestamps '{bundleStoreDefinition.BundleIdentifier}'.");
				}
				else if (!string.IsNullOrEmpty(bundleStoreDefinition.StartTimestamp) && !string.IsNullOrEmpty(bundleStoreDefinition.EndTimestamp))
				{
					long num = (long)(GameEconomyData.ParseDateTime(bundleStoreDefinition.StartTimestamp) - dateTime).TotalSeconds * 1000;
					if ((long)(GameEconomyData.ParseDateTime(bundleStoreDefinition.EndTimestamp) - dateTime).TotalSeconds * 1000 <= num)
					{
						AddError($"Bundle store end time is not greater than start time: '{bundleStoreDefinition.BundleIdentifier}'.");
					}
				}
				if (bundleContentDefinition2 != null && !string.IsNullOrEmpty(bundleContentDefinition2.IAPProduct))
				{
					if (!dictionary.ContainsKey(bundleContentDefinition2.IAPProduct))
					{
						List<BundleStoreDefinition> value = new List<BundleStoreDefinition>();
						dictionary.Add(bundleContentDefinition2.IAPProduct, value);
					}
					dictionary[bundleContentDefinition2.IAPProduct].Add(bundleStoreDefinition);
				}
			}
			foreach (KeyValuePair<string, List<BundleStoreDefinition>> item in dictionary)
			{
				if (item.Value.Count <= 1)
				{
					continue;
				}
				for (int num2 = 0; num2 < item.Value.Count; num2++)
				{
					BundleStoreDefinition bundleStoreDefinition2 = item.Value[num2];
					if (string.IsNullOrEmpty(bundleStoreDefinition2.StartTimestamp) || string.IsNullOrEmpty(bundleStoreDefinition2.EndTimestamp))
					{
						continue;
					}
					for (int num3 = 0; num3 < item.Value.Count; num3++)
					{
						BundleStoreDefinition bundleStoreDefinition3 = item.Value[num3];
						if (num2 != num3 && !string.IsNullOrEmpty(bundleStoreDefinition3.StartTimestamp) && !string.IsNullOrEmpty(bundleStoreDefinition3.EndTimestamp))
						{
							long num4 = (long)(GameEconomyData.ParseDateTime(bundleStoreDefinition2.StartTimestamp) - dateTime).TotalSeconds * 1000;
							long num5 = (long)(GameEconomyData.ParseDateTime(bundleStoreDefinition2.StartTimestamp) - dateTime).TotalSeconds * 1000;
							long num6 = (long)(GameEconomyData.ParseDateTime(bundleStoreDefinition3.StartTimestamp) - dateTime).TotalSeconds * 1000;
							long num7 = (long)(GameEconomyData.ParseDateTime(bundleStoreDefinition3.StartTimestamp) - dateTime).TotalSeconds * 1000;
							if ((num6 < num5 && num6 > num4) || (num7 < num5 && num7 > num4))
							{
								AddError($"Bundle store entries '{bundleStoreDefinition2.BundleIdentifier}' and '{bundleStoreDefinition3.BundleIdentifier}' have overlapping timestamps while using the same iap product id.");
							}
						}
					}
				}
			}
			for (int num8 = 0; num8 < ged.TradefairBundleContentDefinitions.Count; num8++)
			{
				TradefairBundleContentDefinition tradefairBundleContentDefinition = ged.TradefairBundleContentDefinitions[num8];
				if (tradefairBundleContentDefinition.Rewards != null)
				{
					try
					{
						new Rewards(tradefairBundleContentDefinition.Rewards, null, 0, EquipmentSource.Bundle);
					}
					catch (Exception ex2)
					{
						AddError($"Bundle '{tradefairBundleContentDefinition.Identifier}' has invalid rewards string. {ex2.Message}");
					}
				}
			}
			for (int num9 = 0; num9 < ged.TradefairBundleStoreDefinitions.Count; num9++)
			{
				TradefairBundleStoreDefinition tradefairBundleStoreDefinition = ged.TradefairBundleStoreDefinitions[num9];
				TradefairBundleContentDefinition tradefairBundleContentDefinition2 = null;
				for (int num10 = 0; num10 < ged.TradefairBundleContentDefinitions.Count; num10++)
				{
					if (tradefairBundleStoreDefinition.BundleIdentifier == ged.TradefairBundleContentDefinitions[num10].Identifier)
					{
						tradefairBundleContentDefinition2 = ged.TradefairBundleContentDefinitions[num10];
						break;
					}
				}
				if (tradefairBundleContentDefinition2 == null)
				{
					AddError($"Could not find bundle content with identifier '{tradefairBundleStoreDefinition.BundleIdentifier}'.");
				}
				if (!string.IsNullOrEmpty(tradefairBundleStoreDefinition.PreviousBundle))
				{
					bool flag3 = false;
					for (int num11 = 0; num11 < ged.TradefairBundleContentDefinitions.Count; num11++)
					{
						if (tradefairBundleStoreDefinition.PreviousBundle == ged.TradefairBundleContentDefinitions[num11].Identifier)
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						AddError($"Could not find bundle content with identifier '{tradefairBundleStoreDefinition.PreviousBundle}' in bundle store definition '{tradefairBundleStoreDefinition.BundleIdentifier}'.");
					}
				}
				if ((!string.IsNullOrEmpty(tradefairBundleStoreDefinition.StartTimestamp) && string.IsNullOrEmpty(tradefairBundleStoreDefinition.EndTimestamp)) || (string.IsNullOrEmpty(tradefairBundleStoreDefinition.StartTimestamp) && !string.IsNullOrEmpty(tradefairBundleStoreDefinition.EndTimestamp)))
				{
					AddError($"Bundle store has invalid timestamps '{tradefairBundleStoreDefinition.BundleIdentifier}'.");
				}
				else if (!string.IsNullOrEmpty(tradefairBundleStoreDefinition.StartTimestamp) && !string.IsNullOrEmpty(tradefairBundleStoreDefinition.EndTimestamp))
				{
					long num12 = (long)(GameEconomyData.ParseDateTime(tradefairBundleStoreDefinition.StartTimestamp) - dateTime).TotalSeconds * 1000;
					if ((long)(GameEconomyData.ParseDateTime(tradefairBundleStoreDefinition.EndTimestamp) - dateTime).TotalSeconds * 1000 <= num12)
					{
						AddError($"Bundle store end time is not greater than start time: '{tradefairBundleStoreDefinition.BundleIdentifier}'.");
					}
				}
			}
			for (int num13 = 0; num13 < ged.RarityBasedUpgradeDefinitions.Length; num13++)
			{
				RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = ged.RarityBasedUpgradeDefinitions[num13];
				if (rarityBasedUpgradeDefinition.UpgradeType == UpgradeType.EquipmentUpgrade)
				{
					if (rarityBasedUpgradeDefinition.UpgradesTotal < rarityBasedUpgradeDefinition.LowLevelTraitsCount + rarityBasedUpgradeDefinition.MidLevelTraitsCount + rarityBasedUpgradeDefinition.HighLevelTraitsCount)
					{
						AddError($"Equipment Upgrade definition for rarityLevel '{rarityBasedUpgradeDefinition.RarityLevel.ToString()}' does not have enough upgrades for all traits.");
					}
				}
				else if (rarityBasedUpgradeDefinition.UpgradesTotal < rarityBasedUpgradeDefinition.TacticalTraitsCount + rarityBasedUpgradeDefinition.LowLevelTraitsCount + rarityBasedUpgradeDefinition.MidLevelTraitsCount + rarityBasedUpgradeDefinition.HighLevelTraitsCount)
				{
					AddError($"Survivor Upgrade definition for rarityLevel '{rarityBasedUpgradeDefinition.RarityLevel.ToString()}' does not have enough upgrades for all traits.");
				}
			}
			for (int num14 = 0; num14 < ged.AchievementDefinitions.Length; num14++)
			{
				AchievementDefinition achievementDefinition = ged.AchievementDefinitions[num14];
				if (!string.IsNullOrEmpty(achievementDefinition.DependsOn) && ged.GetAchievementDefinition(achievementDefinition.DependsOn) == null)
				{
					AddError($"AchievementDefinition '{achievementDefinition.ID}' depends on another achievement '{achievementDefinition.DependsOn}' which cannot be found.");
				}
				if (localizationExists != null)
				{
					if (IsNotNullOrEmpty(achievementDefinition.LocalizationKey) && (!localizationExists(achievementDefinition.TitleLocalizationKey) || !localizationExists(achievementDefinition.TitleLocalizationKey + "{Param}")))
					{
						AddLocalizationError($"Achievement '{achievementDefinition.ID}' LocalizationKey '{achievementDefinition.TitleLocalizationKey}' is missing.");
					}
					if (IsNotNullOrEmpty(achievementDefinition.LocalizationKey) && (!localizationExists(achievementDefinition.TitleLocalizationKey) || !localizationExists(achievementDefinition.TitleLocalizationKey + "{Param}")))
					{
						AddLocalizationError($"Achievement '{achievementDefinition.ID}' LocalizationKey '{achievementDefinition.DescriptionLocalizationKey}' is missing.");
					}
				}
			}
			for (int num15 = 0; num15 < ged.DropEventDefinitions.Length; num15++)
			{
				DropEventDefinition dropEventDefinition = ged.DropEventDefinitions[num15];
				int num16 = (int)FixedPoint.Round(dropEventDefinition.SumOfProbabilities);
				if (num16 != 100)
				{
					AddError($"DropEventDefinition '{dropEventDefinition.EventType}', '{dropEventDefinition.DropContext}' probabilities do not sum up to 100 but {num16}!");
				}
			}
			for (int num17 = 0; num17 < ged.DropCurrencyProbabilitiesDefinitions.Length; num17++)
			{
				DropCurrenciesProbabilitiesDefinition dropCurrenciesProbabilitiesDefinition = ged.DropCurrencyProbabilitiesDefinitions[num17];
				int num18 = (int)FixedPoint.Round(dropCurrenciesProbabilitiesDefinition.SumOfProbabilities);
				if (num18 != 100)
				{
					AddError($"DropCurrenciesProbabilitiesDefinition '{dropCurrenciesProbabilitiesDefinition.DropType}', '{dropCurrenciesProbabilitiesDefinition.EventType}' probabilities do not sum up to 100 but {num18}!");
				}
			}
			for (int num19 = 0; num19 < ged.DropEquipmentsAndSurvivorsRaritiesDefinitions.Length; num19++)
			{
				DropEquipmentsAndSurvivorsRaritiesDefinition dropEquipmentsAndSurvivorsRaritiesDefinition = ged.DropEquipmentsAndSurvivorsRaritiesDefinitions[num19];
				int num20 = (int)FixedPoint.Round(dropEquipmentsAndSurvivorsRaritiesDefinition.SumOfProbabilities);
				if (num20 != 100)
				{
					AddError($"DropEquipmentsAndSurvivorsRaritiesDefinition '{dropEquipmentsAndSurvivorsRaritiesDefinition.DropType}', '{dropEquipmentsAndSurvivorsRaritiesDefinition.RewardType}' probabilities do not sum up to 100 but {num20}!");
				}
			}
			for (int num21 = 0; num21 < ged.PhoneCallDefinitions.Length; num21++)
			{
				PhoneCallDefinition phoneCallDefinition = ged.PhoneCallDefinitions[num21];
				bool parseError = false;
				phoneCallDefinition.ParseCurrencyTypeValues(out parseError);
				if (parseError)
				{
					AddError($"PhoneCallDefinition at list index {num21}, with start time '{phoneCallDefinition.StartTimeMilliseconds}' has invalid currency type in the string: '{phoneCallDefinition.CurrencyTypes}'");
				}
				phoneCallDefinition.ParseCurrencyTypeDistributionValues(out parseError);
				if (parseError)
				{
					AddError($"PhoneCallDefinition at list index {num21}, with start time '{phoneCallDefinition.StartTimeMilliseconds}' has invalid distribution value (list of integers expected) in the string: '{phoneCallDefinition.CurrencyTypesDistribution}'");
				}
			}
			return ErrorInfos.Count;
		}
	}
}
