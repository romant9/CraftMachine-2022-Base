using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel.ResponsClass;

namespace TWDModel
{
	public class EquipmentModel : TWDModelObject
	{
		public enum ConsumableType
		{
			Unknown = 0,
			Grenade = 1,
			MedKit = 2,
			Flare = 3,
			BlastGrenade = 4,
			Gore = 5
		}

		public static string EquipmentTypeUpgradedEvent = "EquipmentTypeUpgradedEvent";

		public ModelList<EquipmentItemModel> RangeWeapons { get; private set; }

		public ModelList<EquipmentItemModel> MeleeWeapons { get; private set; }

		public ModelList<EquipmentItemModel> Armors { get; private set; }

		public ModelList<BadgeModel> Badges { get; private set; }

		public ModelList<BounsModel> BounsModes { get; private set; }

		public ModelList<EquipmentItemModel> Consumables { get; private set; }

		public EquipmentModel()
		{
			RangeWeapons = new ModelList<EquipmentItemModel>();
			MeleeWeapons = new ModelList<EquipmentItemModel>();
			Armors = new ModelList<EquipmentItemModel>();
			Badges = new ModelList<BadgeModel>();
			Consumables = new ModelList<EquipmentItemModel>();
			BounsModes = new ModelList<BounsModel>();
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public List<EquipmentItemModel> GetAllEquipments()
		{
			List<EquipmentItemModel> list = new List<EquipmentItemModel>();
			list.AddRange(Armors);
			list.AddRange(MeleeWeapons);
			list.AddRange(RangeWeapons);
			return list;
		}

		public List<EquipmentItemModel> GetAllRemoldEquipments()
		{
			List<EquipmentItemModel> allEquipments = GetAllEquipments();
			for (int num = allEquipments.Count - 1; num >= 0; num--)
			{
				EquipmentItemModel equipmentItemModel = allEquipments[num];
				if (equipmentItemModel.Definition == null || !equipmentItemModel.Definition.SwitchRemoldMode)
				{
					allEquipments.RemoveAt(num);
				}
			}
			return allEquipments;
		}

		public List<EquipmentItemModel> GetEquipmentsForClass(SurvivorClass survivorClass, bool isWeapon)
		{
			List<EquipmentItemModel> allEquipments = GetAllEquipments();
			List<EquipmentItemModel> list = new List<EquipmentItemModel>();
			for (int i = 0; i < allEquipments.Count; i++)
			{
				if (allEquipments[i] != null && allEquipments[i].EquipmentSurvivorClass == survivorClass && allEquipments[i].IsWeaponEquipment == isWeapon)
				{
					list.Add(allEquipments[i]);
				}
			}
			return list;
		}

		public void GetEquipmentsForActorNoAlloc(ActorModel actor, bool isWeapon, List<EquipmentItemModel> allEquipments, ref List<EquipmentItemModel> resultList)
		{
			resultList.Clear();
			for (int i = 0; i < allEquipments.Count; i++)
			{
				if (allEquipments[i] != null && actor.CanEquip(allEquipments[i]) && allEquipments[i].IsWeaponEquipment == isWeapon)
				{
					resultList.Add(allEquipments[i]);
				}
			}
		}

		public int GetAllEquipmentCount()
		{
			return Armors.Count + MeleeWeapons.Count + RangeWeapons.Count;
		}

		public List<EquipmentItemModel> GetAllUpgradeableEquipments()
		{
			List<EquipmentItemModel> allEquipments = GetAllEquipments();
			List<EquipmentItemModel> list = new List<EquipmentItemModel>();
			for (int i = 0; i < allEquipments.Count; i++)
			{
				if (allEquipments[i] != null && allEquipments[i].CanUpgrade && allEquipments[i].GetUpgradeCashier(instantUpgrade: false).CanAfford())
				{
					list.Add(allEquipments[i]);
				}
			}
			return list;
		}

		public EquipmentItemModel GetUpgradingEquipment()
		{
			List<EquipmentItemModel> allEquipments = GetAllEquipments();
			for (int i = 0; i < allEquipments.Count; i++)
			{
				if (allEquipments[i] != null && allEquipments[i].IsUpgrading())
				{
					return allEquipments[i];
				}
			}
			return null;
		}

		private int ExcessItemSort(EquipmentItemModel a, EquipmentItemModel b)
		{
			if (a.MaxLevel == b.MaxLevel)
			{
				if (a.Level == b.Level)
				{
					return -a.RarityLevel.CompareTo(b.RarityLevel);
				}
				if (a.Level <= b.Level)
				{
					return 1;
				}
				return -1;
			}
			if (a.MaxLevel <= b.MaxLevel)
			{
				return 1;
			}
			return -1;
		}

		public bool CanAcquireEquipment(EquipmentDefinition equipmentDefinition)
		{
			if (equipmentDefinition != null)
			{
				if (equipmentDefinition.MaxAmountAtInventory > 0)
				{
					int num = 0;
					List<EquipmentItemModel> allEquipments = GetAllEquipments();
					for (int i = 0; i < allEquipments.Count; i++)
					{
						if (allEquipments[i] != null && allEquipments[i].EquipmentDefinitionIdentifier == equipmentDefinition.ID)
						{
							num++;
						}
					}
					return num < equipmentDefinition.MaxAmountAtInventory;
				}
				return true;
			}
			return false;
		}

		public int ScrapExcessItems()
		{
			int maxItemCount = base.manager.Player.gameEconomyData.ConfigData.MaxItemCount;
			int num = base.manager.Player.gameEconomyData.ConfigData.AutoScrapItemThreshold;
			if (maxItemCount <= 0 || num <= 0)
			{
				return 0;
			}
			if (num < maxItemCount)
			{
				num = maxItemCount;
			}
			int num2 = 0;
			List<EquipmentItemModel> allEquipments = GetAllEquipments();
			if (num > 0 && allEquipments.Count > num)
			{
				allEquipments.StableSort(ExcessItemSort);
				for (int num3 = allEquipments.Count - 1; num3 >= 0; num3--)
				{
					EquipmentItemModel equipmentItemModel = allEquipments[num3];
					if (equipmentItemModel.CanBeAutoScrapped() && ScrapEquipmentItem(equipmentItemModel).Result == TWDModelResult.OK)
					{
						base.manager.DeregisterModel(equipmentItemModel);
						allEquipments.RemoveAt(num3);
						base.manager.Debug.Log("Auto scrapped item " + equipmentItemModel.EquipmentDefinitionIdentifier + " with Rarity = " + equipmentItemModel.Rarity.ToString() + " MaxLevel = " + equipmentItemModel.MaxLevel + " Level = " + equipmentItemModel.Level);
						num2++;
						if (allEquipments.Count <= maxItemCount)
						{
							break;
						}
					}
				}
			}
			if (num2 > 0)
			{
				UpdateModelObjects();
				base.manager.Player.ScrappedExcessItems = true;
			}
			return num2;
		}

		public override void Start()
		{
			base.Start();
			ScrapExcessItems();
			SetupBadgeBonuses();
		}

		private void SetupBadgeBonuses()
		{
			for (int i = 0; i < Badges.Count; i++)
			{
				Badges[i].CreateBonusCondition(base.manager.GameEconomyData.GetBadgeBonusDefinition(Badges[i].BonusId));
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public int GetHighestEquipmentRarity()
		{
			int num = 0;
			foreach (EquipmentItemModel model in Armors.Models)
			{
				if (model.RarityLevel > num)
				{
					num = model.RarityLevel;
				}
			}
			foreach (EquipmentItemModel model2 in MeleeWeapons.Models)
			{
				if (model2.RarityLevel > num)
				{
					num = model2.RarityLevel;
				}
			}
			foreach (EquipmentItemModel model3 in RangeWeapons.Models)
			{
				if (model3.RarityLevel > num)
				{
					num = model3.RarityLevel;
				}
			}
			return num;
		}

		public int GetHighestLevelForEquipmentImmediateEquip(EquipmentDefinition equipmentDefinition)
		{
			if (equipmentDefinition != null)
			{
				int highestLevelOfSurvivorClass = base.manager.Player.SurvivorContainer.GetHighestLevelOfSurvivorClass(equipmentDefinition.SurvivorClass);
				return Math.Max(1, Math.Min(highestLevelOfSurvivorClass, base.manager.Player.gameEconomyData.GetMaxAvailableEquipmentLevel()));
			}
			return 1;
		}

		public int GetHighestEquipableEquipmentLevel()
		{
			int num = 0;
			Dictionary<SurvivorClass, int> dictionary = new Dictionary<SurvivorClass, int>();
			foreach (SurvivorClass value in Enum.GetValues(typeof(SurvivorClass)))
			{
				dictionary.Add(value, base.manager.Player.SurvivorContainer.GetHighestLevelOfSurvivorClass(value));
			}
			foreach (EquipmentItemModel model in Armors.Models)
			{
				if (model.Level > num && model.StartingLevel <= dictionary[model.EquipmentSurvivorClass])
				{
					num = model.Level;
				}
			}
			foreach (EquipmentItemModel model2 in MeleeWeapons.Models)
			{
				if (model2.Level > num && model2.StartingLevel <= dictionary[model2.EquipmentSurvivorClass])
				{
					num = model2.Level;
				}
			}
			foreach (EquipmentItemModel model3 in RangeWeapons.Models)
			{
				if (model3.Level > num && model3.StartingLevel <= dictionary[model3.EquipmentSurvivorClass])
				{
					num = model3.Level;
				}
			}
			return num - 3;
		}

		public void AddEquipment(EquipmentItemModel equipment, EquipmentSource source = EquipmentSource.Unknown)
		{
			DebugTWD.Log("AddEquipment: " + equipment.Definition.ID, DebugType.Equipment);

			_ = 4;
			GetEquipmentsOfCategory(equipment.Definition.Category).Add(equipment);
			if (base.manager != null && equipment.Definition.Category == EquipmentCategory.Utility)
			{
				foreach (CombatBackup combatBackup in base.manager.Player.CombatBackups)
				{
					combatBackup.Consumables.Add(equipment);
				}
			}
			if (base.manager == null || base.manager.Player.IsEquipmentAutoScrap == AutoScrapEquipmentType.None || source == EquipmentSource.Survivor)
			{
				return;
			}
			bool num = !string.IsNullOrEmpty(equipment.Definition.InfusedTrait);
			AbilityDefinition abilityDefinition = base.manager.Player.gameEconomyData.GetAbilityDefinition(equipment.Definition.AbilityIdentifier);
			bool flag = false;
			if (abilityDefinition != null)
			{
				flag = !string.IsNullOrEmpty(abilityDefinition.SpecialDescriptionKey);
			}
			bool flag2 = !string.IsNullOrEmpty(equipment.Definition.SpecialTrait);
			if (!num && !flag2 && !flag)
			{
				bool flag3 = false;
				if (base.manager.Player.IsEquipmentAutoScrap == AutoScrapEquipmentType.ThreeStar && equipment.RarityLevel <= 2)
				{
					flag3 = true;
				}
				else if (base.manager.Player.IsEquipmentAutoScrap == AutoScrapEquipmentType.FourStar && equipment.RarityLevel <= 3)
				{
					flag3 = true;
				}
				else if (base.manager.Player.IsEquipmentAutoScrap == AutoScrapEquipmentType.FiveStar && equipment.RarityLevel <= 4)
				{
					flag3 = true;
				}
				if (flag3)
				{
					base.manager.Player.AutoScrapmentEquipment.Add(equipment);
				}
			}
		}

		public void RemoveEquipment(EquipmentItemModel equipment)
		{
			DebugTWD.Log("RemoveEquipment: " + equipment.Definition.ID, DebugType.Equipment);

			ModelList<EquipmentItemModel> equipmentsOfCategory = GetEquipmentsOfCategory(equipment.Definition.Category);
			if (equipment.Definition.Category == EquipmentCategory.Utility)
			{
				base.manager.TdMetrics.SetEventType("tool_use").AddProperty("tool_object", equipment.EquipmentDefinitionIdentifier).AddProperty("tool_withhero", equipment.manager.CombatModel?.TurnManager.ActiveActor.Definition.ID)
					.Send();
			}
			equipmentsOfCategory.Remove(equipment);
		}

		public void ReplaceEquipmentList(EquipmentCategory category, ModelList<EquipmentItemModel> models)
		{
			switch (category)
			{
			case EquipmentCategory.Armor:
				Armors = models;
				break;
			case EquipmentCategory.MeleeWeapon:
				MeleeWeapons = models;
				break;
			case EquipmentCategory.RangeWeapon:
				RangeWeapons = models;
				break;
			}
			UpdateModelObjects();
		}

		public ModelList<EquipmentItemModel> GetEquipmentsOfCategory(EquipmentCategory equipmentCategory)
		{
			return equipmentCategory switch
			{
				EquipmentCategory.Utility => Consumables,
				EquipmentCategory.MeleeWeapon => MeleeWeapons,
				EquipmentCategory.RangeWeapon => RangeWeapons,
				_ => Armors,
			};
		}

		public List<EquipmentItemModel> GetConsumablesOfType(ConsumableType consumableType)
		{
			return Consumables.Where((EquipmentItemModel c) => c.Definition.ID == ConsumableUtils.ConsumableTypeToId(consumableType)).ToList();
		}

		public ModelList<EquipmentItemModel> GetEquipmentsOfType(EquipmentCategory category, params EquipmentType[] equipmentType)
		{
			ModelList<EquipmentItemModel> modelList = new ModelList<EquipmentItemModel>();
			ModelList<EquipmentItemModel>[] obj = new ModelList<EquipmentItemModel>[3] { MeleeWeapons, RangeWeapons, Armors };
			bool flag = false;
			ModelList<EquipmentItemModel>[] array = obj;
			foreach (ModelList<EquipmentItemModel> modelList2 in array)
			{
				int count = modelList2.Count;
				for (int j = 0; j < count; j++)
				{
					EquipmentItemModel item = modelList2[j];
					if (Array.Exists(equipmentType, (EquipmentType element) => element == item.Definition.Type) && item.Definition.Category == category)
					{
						flag = true;
						modelList.Add(item);
					}
				}
				if (flag)
				{
					break;
				}
			}
			return modelList;
		}

		public string DebugAllPossibleLootForMission(MissionData missionData)
		{
			List<string> list = new List<string>();
			string text = "";
			int rarityPreference;
			int num = (rarityPreference = 1);
			int maxAvailableDifficulty = base.manager.Player.gameEconomyData.GetMaxAvailableDifficulty();
			int maxAvailableEquipmentLevel = base.manager.Player.gameEconomyData.GetMaxAvailableEquipmentLevel();
			FixedPoint fixedPoint = (FixedPoint)maxAvailableEquipmentLevel / (FixedPoint)maxAvailableDifficulty;
			int val = (int)(num - fixedPoint * 0.5);
			int val2 = (int)(num + fixedPoint * 0.5);
			val = Math.Max(val, 0);
			val2 = Math.Min(val2, maxAvailableEquipmentLevel);
			List<int> possibleRarities = base.manager.Player.gameEconomyData.GetPossibleRarities(rarityPreference);
			for (int i = val; i <= val2; i++)
			{
				text = text + "\nEquipments at tier " + i + "\n";
				foreach (int item in possibleRarities)
				{
					int tier = i;
					EquipmentCategory category = EquipmentCategory.None;
					foreach (EquipmentDefinition item2 in GenerateListOfPossibleEquipments(category, tier, item))
					{
						string text2 = item2.ID + ".level:" + tier + ".rarity:" + item;
						if (!list.Contains(text2))
						{
							text = text + text2 + "\n";
							list.Add(text2);
						}
					}
				}
			}
			return text;
		}

		public string DebugAllPossibleLootTypesByTiers()
		{
			int maxAvailableEquipmentLevel = base.manager.Player.gameEconomyData.GetMaxAvailableEquipmentLevel();
			string text = "";
			for (int i = 0; i <= maxAvailableEquipmentLevel; i++)
			{
				int tier = i;
				text = text + "\nEquipments at tier " + i + "\n";
				List<string> list = new List<string>();
				foreach (EquipmentDefinition item in GenerateListOfPossibleEquipments(EquipmentCategory.None, tier))
				{
					string iD = item.ID;
					if (!list.Contains(iD))
					{
						list.Add(iD);
						text = text + iD + "\n";
					}
				}
			}
			return text;
		}

		public EquipmentItemModel GenerateRandomEquipmentFromMission(int minStartingLevel, int maxStartingLevel, int rarityLevel, bool isArmor, SurvivorClass equipmentClass, ModelRandom random)
		{
			EquipmentItemModel equipmentItemModel = null;
			EquipmentCategory category = EquipmentCategory.None;
			List<EquipmentCategory> list = new List<EquipmentCategory>();
			if (isArmor)
			{
				category = EquipmentCategory.Armor;
			}
			else
			{
				list.Add(EquipmentCategory.Armor);
			}
			int startingLevel = Math.Max(random.GetRandomInRange(minStartingLevel, maxStartingLevel), 1);
			if (rarityLevel < 0)
			{
				rarityLevel = random.GetRandomInRange(0, 4);
			}
			bool useSpecialization = random.GetRandomInRange(0, 100) > 50;
			equipmentItemModel = GenerateRandomEquipment(category, startingLevel, rarityLevel, useSpecialization, Faction.Survivor, equipmentClass, list, random);
			if (equipmentItemModel == null)
			{
				base.Debug.LogError("Could not generate equipment-> category:" + category.ToString() + " rarityLevel: " + rarityLevel + " tier: " + startingLevel);
				category = EquipmentCategory.None;
				rarityLevel = 0;
				startingLevel = 1;
				equipmentItemModel = GenerateRandomEquipment(category, startingLevel, rarityLevel, useSpecialization, Faction.Survivor, SurvivorClass.None, null, random);
			}
			return equipmentItemModel;
		}

		public EquipmentItemModel GenerateRandomEquipment(EquipmentCategory category = EquipmentCategory.None, int startingLevel = -1, int rarity = -1, bool useSpecialization = false, Faction holderFaction = Faction.Survivor, SurvivorClass survivorClass = SurvivorClass.None, List<EquipmentCategory> excludedCategories = null, ModelRandom random = null, bool startModel = true)
		{
			EquipmentItemModel equipmentItemModel = null;
			EquipmentDefinition randomEquipment = GetRandomEquipment(category, startingLevel, rarity, useSpecialization, holderFaction, survivorClass, excludedCategories, random);
			if (randomEquipment != null)
			{
				equipmentItemModel = GenerateAndInitializeEquipmentFromDefinition(randomEquipment.ID, rarity, startingLevel, random, startModel);
			}
			if (equipmentItemModel == null)
			{
				base.Debug.LogError("Could not find suitable equipment candidates-> category:" + category.ToString() + " tier: " + startingLevel + " specialization: " + useSpecialization + " faction: " + holderFaction.ToString() + " class " + survivorClass);
			}
			return equipmentItemModel;
		}

		public EquipmentDefinition GetRandomEquipment(EquipmentCategory category = EquipmentCategory.None, int startingLevel = -1, int rarity = -1, bool useSpecialization = false, Faction holderFaction = Faction.Survivor, SurvivorClass survivorClass = SurvivorClass.None, List<EquipmentCategory> excludedCategories = null, ModelRandom random = null)
		{
			if (random == null)
			{
				random = base.manager.Player.PlayerRandom;
			}
			if (rarity < 0)
			{
				rarity = random.GetRandomInRange(0, 4);
			}
			List<EquipmentDefinition> list = GenerateListOfPossibleEquipments(category, startingLevel, rarity, useSpecialization, holderFaction, survivorClass, excludedCategories);
			if (list != null && list.Count > 0)
			{
				return random.GetRandomElement(list, remove: false);
			}
			return null;
		}

		private List<EquipmentDefinition> GenerateListOfPossibleEquipments(EquipmentCategory category = EquipmentCategory.None, int tier = -1, int rarity = 0, bool useSpecialization = false, Faction holderFaction = Faction.Survivor, SurvivorClass survivorClass = SurvivorClass.None, List<EquipmentCategory> excludedCategories = null)
		{
			List<EquipmentDefinition> list = new List<EquipmentDefinition>();
			bool flag = false;
			List<SurvivorClass> list2 = new List<SurvivorClass>();
			for (int i = 0; i < 6; i++)
			{
				SurvivorClass survivorClass2 = (SurvivorClass)i;
				if (base.manager.Player.SurvivorContainer.IsSurvivorClassUnlocked(survivorClass2) || base.manager.Player.SurvivorContainer.IsHeroTypeUnlocked(survivorClass2))
				{
					list2.Add(survivorClass2);
				}
			}
			EquipmentDefinition[] equipmentDefinitions = base.manager.Player.gameEconomyData.EquipmentDefinitions;
			foreach (EquipmentDefinition equipmentDefinition in equipmentDefinitions)
			{
				bool num = (category == EquipmentCategory.None || (category == EquipmentCategory.Weapon && (equipmentDefinition.Category == EquipmentCategory.RangeWeapon || equipmentDefinition.Category == EquipmentCategory.MeleeWeapon)) || equipmentDefinition.Category == category) && (excludedCategories == null || excludedCategories.Count == 0 || !excludedCategories.Contains(equipmentDefinition.Category));
				bool flag2 = tier == -1 || (equipmentDefinition.MinTier <= tier && equipmentDefinition.MaxTier >= tier);
				bool flag3 = equipmentDefinition.AvailableRarityLevels == null || equipmentDefinition.AvailableRarityLevels.Count == 0 || equipmentDefinition.AvailableRarityLevels.Contains(rarity);
				bool flag4 = holderFaction == Faction.Any || equipmentDefinition.CanBeEquippedToFaction(holderFaction);
				bool cannotBeGivenAsLoot = equipmentDefinition.CannotBeGivenAsLoot;
				bool flag5 = holderFaction != Faction.Survivor || (survivorClass == SurvivorClass.None && list2.Contains(equipmentDefinition.SurvivorClass)) || equipmentDefinition.CanBeEquippedBySurvivorClass(survivorClass);
				if (num && flag2 && flag4 && flag5 && !cannotBeGivenAsLoot && (!flag || flag3))
				{
					list.Add(equipmentDefinition);
					if (rarity >= 0 && !flag && flag3)
					{
						flag = true;
					}
				}
			}
			if (flag && list != null && list.Count > 0)
			{
				int count = list.Count;
				int num2 = 0;
				while (num2 < count)
				{
					EquipmentDefinition equipmentDefinition2 = list[num2];
					if (equipmentDefinition2.AvailableRarityLevels != null && equipmentDefinition2.AvailableRarityLevels.Count > 0 && !equipmentDefinition2.AvailableRarityLevels.Contains(rarity))
					{
						list.Remove(equipmentDefinition2);
					}
					else
					{
						num2++;
					}
					count = list.Count;
				}
			}
			return list;
		}

		public EquipmentItemModel GenerateAndInitializeEquipmentFromDefinition(string definitionID, int rarityLevel = -1, int startingLevel = -1, ModelRandom random = null, bool startModel = true)
		{
			if (random == null)
			{
				random = base.manager.Player.PlayerRandom;
			}
			EquipmentDefinition equipmentDefinition = base.manager.GameEconomyData.GetEquipmentDefinition(definitionID);
			if (equipmentDefinition == null)
			{
				base.manager.Debug.LogError("Could not find definition for equipment with definition id = '" + definitionID + "'!");
				return null;
			}
			startingLevel = ((startingLevel == -1) ? random.GetRandomInRange(equipmentDefinition.MinTier, equipmentDefinition.MaxTier) : startingLevel);
			startingLevel = Math.Max(1, Math.Min(startingLevel, base.gameEconomyData.GetMaxAvailableEquipmentLevel()));
			if (rarityLevel < 0)
			{
				rarityLevel = random.GetRandomInRange(0, 4);
			}
			EquipmentItemModel equipmentItemModel = new EquipmentItemModel(startingLevel, rarityLevel);
			equipmentItemModel.EquipmentDefinitionIdentifier = definitionID;
			equipmentItemModel.SetManager(base.manager);
			equipmentItemModel.Initialize();
			equipmentItemModel.InitUpgradeTraits(random, equipmentDefinition.SurvivorClass);
			if (equipmentDefinition.SwitchRemoldMode && equipmentDefinition.RemoldTraitsSlotCount > 0 && equipmentItemModel.ModSkillSlots == null)
			{
				equipmentItemModel.ModSkillSlots = new ModSkillSlot[equipmentItemModel.Definition.RemoldTraitsSlotCount];
				for (int i = 0; i < equipmentItemModel.Definition.RemoldTraitsSlotCount; i++)
				{
					equipmentItemModel.ModSkillSlots[i] = new ModSkillSlot(i);
				}
			}
			if (base.Manager.IsStarted && startModel)
			{
				equipmentItemModel.Start();
			}
			return equipmentItemModel;
		}

		public EquipmentItemModel GenerateAndInitializeEquipmentFromMockData(EquipmentItemMockData equipmentItemMockData, int survivorLevel, bool preview = false)
		{
			EquipmentDefinition equipmentDefinition = base.manager.GameEconomyData.GetEquipmentDefinition(equipmentItemMockData.EquipmentDefinitionId);
			int num = equipmentItemMockData.UpgradeTraits.Count - 1 + survivorLevel;
			if (equipmentDefinition == null)
			{
				base.manager.Debug.LogError("Could not find definition for equipment with definition id = '" + equipmentItemMockData.EquipmentDefinitionId + "'!");
				return null;
			}
			EquipmentItemModel equipmentItemModel = new EquipmentItemModel(num, equipmentItemMockData.RarityLevel);
			equipmentItemModel.Level = num;
			equipmentItemModel.EquipmentDefinitionIdentifier = equipmentItemMockData.EquipmentDefinitionId;
			equipmentItemModel.ModSkillSlots = equipmentItemMockData.ModSkillSlots;
			equipmentItemModel.SetManager(base.manager);
			equipmentItemModel.Initialize();
			equipmentItemModel.InitUpgradeTraitsFromMockData(equipmentItemMockData.UpgradeTraits, equipmentDefinition.SurvivorClass, preview);
			if (!preview && base.Manager.IsStarted)
			{
				equipmentItemModel.Start();
			}
			return equipmentItemModel;
		}

		public EquipmentItemModel AddChargeEquipmentToTargetFromMockData(EquipmentItemModel target, List<EquipmentTraitMockData> traits)
		{
			if (!string.IsNullOrEmpty(target.Definition.ChargeEquipmentIdentifier))
			{
				EquipmentItemModel equipmentItemModel = new EquipmentItemModel(target.Level, target.RarityLevel);
				equipmentItemModel.SetManager(base.manager);
				equipmentItemModel.EquipmentDefinitionIdentifier = target.Definition.ChargeEquipmentIdentifier;
				equipmentItemModel.Level = target.Level;
				equipmentItemModel.IsChargeEquipment = true;
				equipmentItemModel.Initialize();
				equipmentItemModel.InitUpgradeTraitsFromMockData(traits, target.EquipmentSurvivorClass);
				target.ChargeEquipment = equipmentItemModel;
				return target.ChargeEquipment;
			}
			return null;
		}

		public EquipmentItemModel AddChargeEquipmentToTarget(EquipmentItemModel target, ModelRandom random)
		{
			if (!string.IsNullOrEmpty(target.Definition.ChargeEquipmentIdentifier))
			{
				EquipmentItemModel equipmentItemModel = new EquipmentItemModel(target.Level, target.RarityLevel);
				equipmentItemModel.SetManager(base.manager);
				equipmentItemModel.EquipmentDefinitionIdentifier = target.Definition.ChargeEquipmentIdentifier;
				equipmentItemModel.Level = target.Level;
				equipmentItemModel.IsChargeEquipment = true;
				equipmentItemModel.Initialize();
				equipmentItemModel.InitUpgradeTraits(random, target.Definition.SurvivorClass);
				target.ChargeEquipment = equipmentItemModel;
				return target.ChargeEquipment;
			}
			return null;
		}

		public bool Contains(EquipmentItemModel equipmentItemModel)
		{
			if (equipmentItemModel.Definition == null)
			{
				throw new ArgumentException("Definition for EquipmentItemModel is null. Check GED. EquipmentDefinitionIdentifier: " + equipmentItemModel.EquipmentDefinitionIdentifier);
			}
			return GetEquipmentsOfCategory(equipmentItemModel.Definition.Category).Contains(equipmentItemModel);
		}

		private void RefreshAllEquipmentsOfType(EquipmentType equipmentType)
		{
			EquipmentCategory categoryOfEquipmentType = base.manager.Player.gameEconomyData.GetCategoryOfEquipmentType(equipmentType);
			ModelList<EquipmentItemModel> equipmentsOfCategory = GetEquipmentsOfCategory(categoryOfEquipmentType);
			if (equipmentsOfCategory == null)
			{
				return;
			}
			foreach (EquipmentItemModel item in equipmentsOfCategory)
			{
				if (item.Definition.Type == equipmentType)
				{
					item.RefreshModifiers();
				}
			}
		}

		public ResponsScrapEquipmentItem ScrapEquipmentItem(EquipmentItemModel equipmentItemModel, bool deletedBySupport = false, Cashier cashier = null, CashierRewardsListCalss cashierRewardsListCalss = null)
		{
			PlayerModel playerModel = base.manager.GetPlayer() as PlayerModel;
			EquipmentModel equipment = playerModel.Equipment;
			ResponsScrapEquipmentItem responsScrapEquipmentItem = new ResponsScrapEquipmentItem();
			if (equipmentItemModel == null)
			{
				base.Debug.LogError("equipmentItemModel is null");
				responsScrapEquipmentItem.Result = TWDModelResult.Error;
				return responsScrapEquipmentItem;
			}
			if (!equipment.Contains(equipmentItemModel))
			{
				responsScrapEquipmentItem.Result = TWDModelResult.EquipmentNotFound;
				return responsScrapEquipmentItem;
			}
			if (equipmentItemModel.Definition.SwitchRemoldMode && !base.gameEconomyData.ConfigData.RemoldEquipCanBeBreakDown)
			{
				responsScrapEquipmentItem.Result = TWDModelResult.Error;
				return responsScrapEquipmentItem;
			}
			if (base.manager.Player.Camp.GetBuilding("Workshop") is WorkshopBuildingModel workshopBuildingModel && ((workshopBuildingModel.UpgradingEquipment != null && workshopBuildingModel.UpgradingEquipment == equipmentItemModel) || (workshopBuildingModel.UpgradedUnseenModel != null && workshopBuildingModel.UpgradedUnseenModel == equipmentItemModel)))
			{
				workshopBuildingModel.CancelUpgrade();
			}
			Dictionary<CurrencyType, OverflowableAmount> refundedAmounts = new Dictionary<CurrencyType, OverflowableAmount>();
			if (!deletedBySupport)
			{
				if (equipmentItemModel.BreakthroughLevel > 0)
				{
					cashierRewardsListCalss = equipmentItemModel.GetModSkillCashierReward;
					cashierRewardsListCalss.Rewards.Give(base.manager);
				}
				else
				{
					cashier = equipmentItemModel.GetScrapCashier;
					refundedAmounts = cashier.Refund(100, dontAllowMultiplier: true);
				}
				if (cashierRewardsListCalss == null)
				{
					cashierRewardsListCalss = new CashierRewardsListCalss();
				}
				if (cashier != null)
				{
					int totalCost = cashier.GetTotalCost(CurrencyType.ApocalypticEquipToken);
					if (totalCost > 0)
					{
						cashierRewardsListCalss.apocalypticEquipTokencount += totalCost;
					}
					int totalCost2 = cashier.GetTotalCost(CurrencyType.SurvivalPoints);
					if (totalCost2 > 0)
					{
						cashierRewardsListCalss.ScrapAmount += totalCost2;
					}
				}
				responsScrapEquipmentItem.Rewards = cashierRewardsListCalss;
			}
			equipmentItemModel.ResetModSkillSlots();
			equipment.RemoveEquipment(equipmentItemModel);
			for (int i = 0; i < playerModel.SurvivorContainer.Survivors.Count; i++)
			{
				playerModel.SurvivorContainer.Survivors[i].Unequip(equipmentItemModel);
			}
			if (!deletedBySupport)
			{
				base.manager.Metrics.AddFind().AddResources(refundedAmounts).AddEquipment(equipmentItemModel)
					.AddScrap()
					.Send();
			}
			responsScrapEquipmentItem.Result = TWDModelResult.OK;
			return responsScrapEquipmentItem;
		}

		public Cashier GetEquipmentListScrapCashier(List<EquipmentItemModel> equipmentItems)
		{
			Cashier cashier = new Cashier(base.manager);
			if (equipmentItems != null)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.Refund);
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < equipmentItems.Count; i++)
				{
					EquipmentItemModel equipmentItemModel = equipmentItems[i];
					num += Math.Abs(equipmentItemModel.GetScrapCashier.GetTotalCost(CurrencyType.SurvivalPoints));
					num2 += Math.Abs(equipmentItemModel.GetScrapCashier.GetTotalCost(CurrencyType.ApocalypticEquipToken));
				}
				cashierItem.SetCost(CurrencyType.SurvivalPoints, num);
				cashierItem.SetCost(CurrencyType.ApocalypticEquipToken, num2);
				cashier.AddItem(cashierItem);
			}
			return cashier;
		}

		public Rewards GetEquipmentListScrapReward(List<EquipmentItemModel> equipmentItems)
		{
			Rewards rewards = new Rewards();
			if (equipmentItems == null || equipmentItems.Count == 0)
			{
				return rewards;
			}
			int num = 0;
			int num2 = 0;
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			Dictionary<CurrencyType, int> dictionary2 = new Dictionary<CurrencyType, int>();
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			for (int i = 0; i < equipmentItems.Count; i++)
			{
				EquipmentItemModel equipmentItemModel = equipmentItems[i];
				if (equipmentItemModel == null || equipmentItemModel.Definition == null)
				{
					continue;
				}
				if (equipmentItemModel.BreakthroughLevel > 0)
				{
					if (equipmentItemModel.GetEquipTypeNormalAndRemold == null)
					{
						continue;
					}
					EquipBreakthroughDefinition remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode = base.manager.GameEconomyData.GetRemoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode(equipmentItemModel.RarityLevel, equipmentItemModel.BreakthroughLevel, equipmentItemModel.GetEquipTypeNormalAndRemold);
					if (remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode != null && remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources != null && remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources.Count >= 3)
					{
						num2 += Math.Abs(remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[2]);
						if (remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[1] > 0 && equipmentItemModel.EquipmentBreakthrough != null)
						{
							CurrencyType survivorClassCurrencyType = equipmentItemModel.EquipmentBreakthrough.GetSurvivorClassCurrencyType(equipmentItemModel.Definition.SurvivorClass);
							if (survivorClassCurrencyType != CurrencyType.None)
							{
								if (!dictionary.ContainsKey(survivorClassCurrencyType))
								{
									dictionary[survivorClassCurrencyType] = 0;
								}
								dictionary[survivorClassCurrencyType] += Math.Abs(remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[1]);
							}
						}
						Dictionary<CurrencyType, int> scrapSpTokenReward = equipmentItemModel.GetScrapSpTokenReward(remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode, isCharge: true);
						if (scrapSpTokenReward != null)
						{
							CurrencyType currencyType = CurrencyType.None;
							foreach (KeyValuePair<CurrencyType, int> item in scrapSpTokenReward)
							{
								if (currencyType == CurrencyType.None)
								{
									currencyType = item.Key;
								}
								if (!dictionary2.ContainsKey(item.Key))
								{
									dictionary2[item.Key] = 0;
								}
								int value = item.Value;
								dictionary2[currencyType] += Math.Abs(value);
							}
						}
						if (remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[0] > 0)
						{
							EquipTokenDefinition equipTokenDefinitionByRelateEquipId = base.gameEconomyData.GetEquipTokenDefinitionByRelateEquipId(equipmentItemModel.Definition.ID);
							if (equipTokenDefinitionByRelateEquipId != null && !string.IsNullOrEmpty(equipTokenDefinitionByRelateEquipId.EquipTokenId))
							{
								if (!dictionary3.ContainsKey(equipTokenDefinitionByRelateEquipId.EquipTokenId))
								{
									dictionary3[equipTokenDefinitionByRelateEquipId.EquipTokenId] = 0;
								}
								dictionary3[equipTokenDefinitionByRelateEquipId.EquipTokenId] += remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[0];
							}
						}
					}
					int num3 = 0;
					int num4 = (int)(equipmentItemModel.EquipmentLevelDefinition.ScrapSurvivalPointsBase * (1.0 + (FixedPoint)equipmentItemModel.RarityDefinition.ScrapValueMultiplier / (FixedPoint)100.0));
					for (int j = equipmentItemModel.StartingLevel; j < equipmentItemModel.Level; j++)
					{
						num3 = equipmentItemModel.GetUpgradeCost(j);
						num4 += (int)(num3 * ((FixedPoint)equipmentItemModel.RarityDefinition.ScrapUpgradeReturnPercentage / (FixedPoint)100.0));
					}
					num += (int)(num4 * base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints).AddMultiplier);
				}
				else
				{
					Cashier getScrapCashier = equipmentItemModel.GetScrapCashier;
					int totalCost = getScrapCashier.GetTotalCost(CurrencyType.ApocalypticEquipToken);
					if (totalCost > 0)
					{
						num2 += totalCost;
					}
					int totalCost2 = getScrapCashier.GetTotalCost(CurrencyType.SurvivalPoints);
					if (totalCost2 > 0)
					{
						num += totalCost2;
					}
				}
			}
			if (num > 0)
			{
				rewards.AddRewardCurrency(CurrencyType.SurvivalPoints, num, isDiamondExchange: false, canOverflowMax: false);
			}
			if (num2 > 0)
			{
				rewards.AddRewardCurrency(CurrencyType.ApocalypticEquipToken, num2, isDiamondExchange: false, canOverflowMax: false);
			}
			if (dictionary.Count > 0)
			{
				foreach (KeyValuePair<CurrencyType, int> item2 in dictionary)
				{
					if (item2.Value > 0)
					{
						rewards.AddRewardCurrency(item2.Key, item2.Value, isDiamondExchange: false, canOverflowMax: false);
					}
				}
			}
			if (dictionary2.Count > 0)
			{
				foreach (KeyValuePair<CurrencyType, int> item3 in dictionary2)
				{
					if (item3.Value > 0)
					{
						rewards.AddRewardCurrency(item3.Key, item3.Value, isDiamondExchange: false, canOverflowMax: false);
					}
				}
			}
			if (dictionary3.Count > 0)
			{
				foreach (KeyValuePair<string, int> item4 in dictionary3)
				{
					if (item4.Value > 0)
					{
						Rewards rewards2 = new Rewards("EquipToken(" + item4.Key + "," + item4.Value + ")");
						rewards.RewardsList.AddRange(rewards2.RewardsList);
					}
				}
			}
			return rewards;
		}

		public void MigrateBadgesForOldPlayers()
		{
			if (Badges == null)
			{
				Badges = new ModelList<BadgeModel>();
			}
		}

		public void MigrateBounsForOldPlayers()
		{
			if (BounsModes == null)
			{
				BounsModes = new ModelList<BounsModel>();
			}
		}

		public void AddBounsModel(BounsModel bounsModel)
		{
			BounsModes.Add(bounsModel);
		}

		public BounsModel GetBounsModelWithItemId(int ItemID)
		{
			foreach (BounsModel bounsMode in BounsModes)
			{
				if (bounsMode.ItemID == ItemID)
				{
					return bounsMode;
				}
			}
			return null;
		}

		public void AddBadge(BadgeModel badge)
		{
			Badges.Add(badge);
		}

		public void RemoveBadge(BadgeModel badge)
		{
			Badges.Remove(badge);
		}

		public TWDModelResult ScrapBadge(BadgeModel badge)
		{
			if (badge == null)
			{
				return TWDModelResult.Error;
			}
			if (Badges.Contains(badge))
			{
				RemoveBadge(badge);
			}
			Dictionary<CurrencyType, OverflowableAmount> refundedAmounts = badge.GetScrapCashier().Refund();
			base.manager.Metrics.AddFind().AddResources(refundedAmounts).AddScrap()
				.AddBadge(badge)
				.Send();
			return TWDModelResult.OK;
		}

		public Cashier GetBadgeListScrapCashier(List<BadgeModel> badgeItems)
		{
			Cashier cashier = new Cashier(base.manager);
			if (badgeItems != null)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.Refund);
				int num = 0;
				for (int i = 0; i < badgeItems.Count; i++)
				{
					BadgeModel badgeModel = badgeItems[i];
					num += Math.Abs(badgeModel.GetScrapCashier().GetTotalCost(CurrencyType.SurvivalPoints));
				}
				cashierItem.SetCost(CurrencyType.SurvivalPoints, num);
				cashier.AddItem(cashierItem);
			}
			return cashier;
		}

		public bool ContainsBadgeWithSlotIndex(int index)
		{
			if (Badges != null)
			{
				for (int i = 0; i < Badges.Count; i++)
				{
					if (Badges[i] != null && Badges[i].SlotIndex == index)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void GetBadgesWithSlotIndex(int index, ref List<BadgeModel> resultList)
		{
			if (resultList == null)
			{
				resultList = new List<BadgeModel>();
			}
			else
			{
				resultList.Clear();
			}
			if (Badges == null)
			{
				return;
			}
			for (int i = 0; i < Badges.Count; i++)
			{
				if (Badges[i] != null && Badges[i].SlotIndex == index)
				{
					resultList.Add(Badges[i]);
				}
			}
		}

		public int GetBadgeCountWithType(BadgeType type)
		{
			int num = 0;
			if (Badges != null)
			{
				for (int i = 0; i < Badges.Count; i++)
				{
					if (Badges[i] != null && Badges[i].Type == type)
					{
						num++;
					}
				}
			}
			return num;
		}

		public void GetBadgesWithType(BadgeType type, ref List<BadgeModel> resultList)
		{
			if (resultList == null)
			{
				resultList = new List<BadgeModel>();
			}
			else
			{
				resultList.Clear();
			}
			if (Badges == null)
			{
				return;
			}
			for (int i = 0; i < Badges.Count; i++)
			{
				if (Badges[i] != null && Badges[i].Type == type)
				{
					resultList.Add(Badges[i]);
				}
			}
		}

		public void OnCouncilBuildingChange(ModelObject m, string changed, object args)
		{
			if (changed == "EventLevelUpBuilding" && args is BuildingModel buildingModel)
			{
				_ = buildingModel.TypeName == "Council";
			}
		}



		#region mycode
		public EquipmentItemModel ChangeEqupmentModel(EquipmentItemModel ChangedModel, out bool isWeapon)
		{
			for (int i = 0; i < Armors.Models.Count; i++)
			{
				if (Armors.Models[i].IdForAnalytics == ChangedModel.IdForAnalytics)
				{
					Armors.Models[i] = ChangedModel;
					Armors.Models[i].SetManager(base.manager);
					Armors.Models[i].Start();
					isWeapon = false;
					return Armors.Models[i];
				}
			}
			for (int i = 0; i < RangeWeapons.Models.Count; i++)
			{
				if (RangeWeapons.Models[i].IdForAnalytics == ChangedModel.IdForAnalytics)
				{
					RangeWeapons.Models[i] = ChangedModel;
					RangeWeapons.Models[i].SetManager(base.manager);
					RangeWeapons.Models[i].Start();
					isWeapon = true;
					return RangeWeapons.Models[i];
				}
			}
			for (int i = 0; i < MeleeWeapons.Models.Count; i++)
			{
				if (MeleeWeapons.Models[i].IdForAnalytics == ChangedModel.IdForAnalytics)
				{
					MeleeWeapons.Models[i] = ChangedModel;
					MeleeWeapons.Models[i].SetManager(base.manager);
					MeleeWeapons.Models[i].Start();
					isWeapon = true;
					return MeleeWeapons.Models[i];
				}
			}
			isWeapon = true;
			return null;
		}
		#endregion
	}
}
