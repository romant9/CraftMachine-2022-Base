using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class ModSkillManager : TWDModelObject
	{
		public const string ModSkillMadeEvent = "ModSkillMadeEvent";

		public const string ModSkillEquippedEvent = "ModSkillEquippedEvent";

		public const string ModSkillUnequippedEvent = "ModSkillUnequippedEvent";

		public ModelList<ModSkillMode> ModSkillModes { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			ModSkillModes = new ModelList<ModSkillMode>();
			ModSkillModes.SetManager(base.manager);
			ModSkillModes.Initialize();
		}

		public override void Start()
		{
			base.Start();
			if (ModSkillModes == null)
			{
				ModSkillModes = new ModelList<ModSkillMode>();
				ModSkillModes.SetManager(base.manager);
				ModSkillModes.Initialize();
			}
		}

		public Dictionary<CurrencyType, int> GetMakingCost(string spTraitsId)
		{
			if (string.IsNullOrEmpty(spTraitsId))
			{
				return null;
			}
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(spTraitsId);
			if (sPTraitsRemodeDefinition == null)
			{
				return null;
			}
			if (!sPTraitsRemodeDefinition.Available)
			{
				return null;
			}
			return sPTraitsRemodeDefinition.GetMakingCost();
		}

		public bool CanMakeModSkill(string spTraitsId)
		{
			Dictionary<CurrencyType, int> makingCost = GetMakingCost(spTraitsId);
			if (makingCost == null || makingCost.Count == 0)
			{
				return false;
			}
			if (OfflineManager.IsFreeAll) return true;
			foreach (KeyValuePair<CurrencyType, int> item in makingCost)
			{
				CurrencyModel currency = base.manager.Player.GetCurrency(item.Key);
				if (currency == null || currency.Value < item.Value)
				{
					return false;
				}
			}
			return true;
		}

		public Dictionary<CurrencyType, int> GetMissingMakingCost(string spTraitsId)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			Dictionary<CurrencyType, int> makingCost = GetMakingCost(spTraitsId);
			if (makingCost == null || makingCost.Count == 0)
			{
				return dictionary;
			}
			foreach (KeyValuePair<CurrencyType, int> item in makingCost)
			{
				int num = base.manager.Player.GetCurrency(item.Key)?.Value ?? 0;
				int num2 = item.Value - num;
				if (num2 > 0)
				{
					dictionary.Add(item.Key, num2);
				}
			}
			return dictionary;
		}

		public ModSkillMode MakeModSkill(string spTraitsId, string groupID, SurvivorClass survivorClass)
		{
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(spTraitsId);
			if (sPTraitsRemodeDefinition == null)
			{
				return null;
			}
			if (!sPTraitsRemodeDefinition.Available)
			{
				return null;
			}
			if (HasModSkillMode(groupID))
			{
				return null;
			}
			ModSkillMode modSkillMode = new ModSkillMode(sPTraitsRemodeDefinition.ID, sPTraitsRemodeDefinition.Type, sPTraitsRemodeDefinition.AvailableClass, ModSkillState.Unequipped, null, ModSkillLockState.Unlocked);
			modSkillMode.SetManager(base.manager);
			modSkillMode.Initialize();
			modSkillMode.Start();
			ModSkillModes.Add(modSkillMode);
			base.manager.TdMetrics.SetEventType("EquipRemold");
			base.manager.TdMetrics.AddProperty("RemoldSkillUnlock", new BiEquipRemold(sPTraitsRemodeDefinition.ID, sPTraitsRemodeDefinition.Level));
			base.manager.TdMetrics.Send();
			NotifyChange("ModSkillMadeEvent");
			return modSkillMode;
		}

		public List<ModSkillMode> GetAcquiredModSkills()
		{
			List<ModSkillMode> list = new List<ModSkillMode>();
			if (ModSkillModes == null)
			{
				return list;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillMode.GetSpTraitsDefaultTrait();
				if (spTraitsDefaultTrait != null && spTraitsDefaultTrait.Available)
				{
					list.Add(modSkillMode);
				}
			}
			return list;
		}

		public ModSkillMode GetModSkillMode(string spTraitsId, string modSkillType)
		{
			if (ModSkillModes == null || string.IsNullOrEmpty(spTraitsId) || string.IsNullOrEmpty(modSkillType))
			{
				return null;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				if (modSkillMode.ID == spTraitsId && modSkillMode.Type == modSkillType)
				{
					return modSkillMode;
				}
			}
			return null;
		}

		public bool HasModSkillMode(string groupID)
		{
			if (ModSkillModes == null || string.IsNullOrEmpty(groupID))
			{
				return false;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				if (modSkillMode.Type == groupID)
				{
					return true;
				}
			}
			return false;
		}

		public ModSkillMode GetModSkillModeByGroupID(string groupID)
		{
			if (ModSkillModes == null || string.IsNullOrEmpty(groupID))
			{
				return null;
			}
			return GetExistingReward(groupID);
		}

		public ModSkillMode GetModSkillMode(string spTraitsId)
		{
			if (ModSkillModes == null || string.IsNullOrEmpty(spTraitsId))
			{
				return null;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				if (modSkillMode.ID == spTraitsId)
				{
					return modSkillMode;
				}
			}
			return null;
		}

		public List<ModSkillMode> GetUnequippedModSkills()
		{
			List<ModSkillMode> list = new List<ModSkillMode>();
			if (ModSkillModes == null)
			{
				return list;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillMode.GetSpTraitsDefaultTrait();
				if (spTraitsDefaultTrait != null && spTraitsDefaultTrait.Available && modSkillMode.ModSkillState == ModSkillState.Unequipped)
				{
					list.Add(modSkillMode);
				}
			}
			return list;
		}

		public List<ModSkillMode> GetAcquiredModSkillsByClass(EquipmentItemModel targetEquipment)
		{
			return GetAcquiredModSkillsByClass(targetEquipment.Definition.SurvivorClass, targetEquipment);
		}

		public List<ModSkillMode> GetAcquiredModSkillsByClass(SurvivorClass survivorClass, EquipmentItemModel targetEquipment)
		{
			List<ModSkillMode> list = new List<ModSkillMode>();
			if (ModSkillModes == null)
			{
				return list;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				if (modSkillMode.SurvivorClass == survivorClass)
				{
					SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillMode.ID);
					if (sPTraitsRemodeDefinition != null && sPTraitsRemodeDefinition.Available)
					{
						modSkillMode.CanEquip = !IsModSkillExcludedByEquipped(modSkillMode, targetEquipment);
						list.Add(modSkillMode);
					}
				}
			}
			return list;
		}

		public List<ModSkillMode> GetAcquiredModSkillsByClass(SurvivorClass survivorClass)
		{
			List<ModSkillMode> list = new List<ModSkillMode>();
			if (ModSkillModes == null)
			{
				return list;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				if (modSkillMode.SurvivorClass == survivorClass)
				{
					SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillMode.ID);
					if (sPTraitsRemodeDefinition != null && sPTraitsRemodeDefinition.Available)
					{
						list.Add(modSkillMode);
					}
				}
			}
			return list;
		}

		public bool IsModSkillExcludedByEquipped(ModSkillMode modSkillToEquip, EquipmentItemModel targetEquipment)
		{
			if (modSkillToEquip == null || targetEquipment == null)
			{
				return false;
			}
			List<ModSkillMode> equippedModSkills = GetEquippedModSkills(targetEquipment);
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillToEquip.ID);
			if (sPTraitsRemodeDefinition == null)
			{
				return false;
			}
			foreach (ModSkillMode item in equippedModSkills)
			{
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition2 = base.gameEconomyData.GetSPTraitsRemodeDefinition(item.ID);
				if (sPTraitsRemodeDefinition2 != null)
				{
					if (sPTraitsRemodeDefinition2.ExclusionForSp != null && sPTraitsRemodeDefinition2.ExclusionForSp.Contains(modSkillToEquip.Type))
					{
						return true;
					}
					if (sPTraitsRemodeDefinition.ExclusionForSp != null && sPTraitsRemodeDefinition.ExclusionForSp.Contains(item.Type))
					{
						return true;
					}
				}
			}
			return false;
		}

		public List<ModSkillMode> GetEquippedModSkills(EquipmentItemModel equippedItemModel)
		{
			List<ModSkillMode> list = new List<ModSkillMode>();
			if (ModSkillModes == null)
			{
				return list;
			}
			foreach (ModSkillMode modSkillMode in ModSkillModes)
			{
				if (modSkillMode.ModSkillState == ModSkillState.Equipped && modSkillMode.EquipmentItemModel == equippedItemModel)
				{
					list.Add(modSkillMode);
				}
			}
			return list;
		}

		public TWDModelResult EquipModSkill(int slotIndex, ModSkillMode modSkillMode, EquipmentItemModel equipmentItemModel)
		{
			ModSkillMode modSkillMode2 = GetModSkillMode(modSkillMode.ID, modSkillMode.Type);
			if (modSkillMode2 == null)
			{
				return TWDModelResult.Error;
			}
			if (IsModSkillExcludedByEquipped(modSkillMode2, equipmentItemModel))
			{
				return TWDModelResult.Error;
			}
			if (equipmentItemModel != null)
			{
				ModSkillMode modSkillSlotByIndex = equipmentItemModel.GetModSkillSlotByIndex(slotIndex);
				if (modSkillSlotByIndex != null)
				{
					UnEquipModSkill(modSkillSlotByIndex);
				}
			}
			if (modSkillMode2.ModSkillState == ModSkillState.Equipped)
			{
				UnEquipModSkill(modSkillMode);
			}
			modSkillMode2.ModSkillState = ModSkillState.Equipped;
			modSkillMode2.EquipmentItemModel = equipmentItemModel;
			modSkillMode2.SlotIndex = slotIndex;
			if (equipmentItemModel != null)
			{
				equipmentItemModel.SetModSkillSlot(slotIndex, modSkillMode2);
				List<string> passiveTraits = modSkillMode2.GetSpTraitsDefaultTrait().PassiveTraits;
				if (passiveTraits != null)
				{
					foreach (string item in passiveTraits)
					{
						equipmentItemModel.ApplyModSkillPassiveTraitToOwner(item);
					}
				}
			}
			NotifyChange("ModSkillEquippedEvent");
			base.manager.TdMetrics.SetEventType("EquipRemold");
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillMode2.GetSpTraitsDefaultTrait();
			base.manager.TdMetrics.AddProperty("RemoldSkillEquiped", new BiEquipRemold(modSkillMode2.ID, spTraitsDefaultTrait?.Level ?? 0, modSkillMode2.Type));
			base.manager.TdMetrics.Send();
			return TWDModelResult.OK;
		}

		public TWDModelResult UnEquipModSkill(ModSkillMode modSkillMode)
		{
			ModSkillMode modSkillMode2 = GetModSkillMode(modSkillMode.ID, modSkillMode.Type);
			if (modSkillMode2 == null)
			{
				return TWDModelResult.Error;
			}
			if (modSkillMode2.ModSkillState == ModSkillState.Unequipped)
			{
				return TWDModelResult.Error;
			}
			if (modSkillMode2.EquipmentItemModel != null)
			{
				modSkillMode2.EquipmentItemModel.RemoveModSkillSlot(modSkillMode2.SlotIndex);
				List<string> passiveTraits = modSkillMode2.GetSpTraitsDefaultTrait().PassiveTraits;
				if (passiveTraits != null)
				{
					foreach (string item in passiveTraits)
					{
						modSkillMode2.EquipmentItemModel.RemoveModSkillPassiveTrait(item);
					}
				}
				modSkillMode2.ModSkillState = ModSkillState.Unequipped;
				modSkillMode2.SlotIndex = -1;
				modSkillMode2.EquipmentItemModel = null;
				NotifyChange("ModSkillUnequippedEvent");
				return TWDModelResult.OK;
			}
			base.manager.Debug.LogError("modSkill.EquipmentItemModel is Null");
			return TWDModelResult.Error;
		}

		public bool CanUpgradeModSkillForUnlock(string spTraitsId)
		{
			Dictionary<CurrencyType, int> makingCost = GetMakingCost(spTraitsId);
			if (makingCost == null || makingCost.Count == 0)
			{
				return false;
			}
			if (OfflineManager.IsFreeAll) return true;
			foreach (KeyValuePair<CurrencyType, int> item in makingCost)
			{
				CurrencyModel currency = base.manager.Player.GetCurrency(item.Key);
				if (currency == null || currency.Value < item.Value)
				{
					return false;
				}
			}
			return true;
		}

		public List<ModSkillMode> GetUnlockableModSkills(SurvivorClass survivorClass)
		{
			List<ModSkillMode> list = new List<ModSkillMode>();
			if (base.gameEconomyData == null || base.gameEconomyData.SPTraitsRemodeDefinition == null)
			{
				return list;
			}
			HashSet<string> hashSet = new HashSet<string>();
			if (ModSkillModes != null)
			{
				foreach (ModSkillMode modSkillMode3 in ModSkillModes)
				{
					if (!string.IsNullOrEmpty(modSkillMode3.Type) && survivorClass == modSkillMode3.SurvivorClass)
					{
						hashSet.Add(modSkillMode3.Type);
					}
				}
			}
			Dictionary<string, SPTraitsRemoldDefinitions> dictionary = new Dictionary<string, SPTraitsRemoldDefinitions>();
			SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition = base.gameEconomyData.SPTraitsRemodeDefinition;
			foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in sPTraitsRemodeDefinition)
			{
				if (sPTraitsRemoldDefinitions != null && sPTraitsRemoldDefinitions.Level == 1 && sPTraitsRemoldDefinitions.Available && sPTraitsRemoldDefinitions.AvailableClass == survivorClass && !string.IsNullOrEmpty(sPTraitsRemoldDefinitions.Type) && !dictionary.ContainsKey(sPTraitsRemoldDefinitions.Type))
				{
					dictionary.Add(sPTraitsRemoldDefinitions.Type, sPTraitsRemoldDefinitions);
				}
			}
			foreach (KeyValuePair<string, SPTraitsRemoldDefinitions> item in dictionary)
			{
				string key = item.Key;
				SPTraitsRemoldDefinitions value = item.Value;
				if (!hashSet.Contains(key))
				{
					ModSkillMode modSkillMode2 = new ModSkillMode(value.ID, value.Type, survivorClass, ModSkillState.Unequipped, null, ModSkillLockState.Locked);
					modSkillMode2.SetManager(base.manager);
					if (CanMakeModSkill(value.ID))
					{
						modSkillMode2.ModSkillLockState = ModSkillLockState.CanUnlock;
					}
					else
					{
						modSkillMode2.ModSkillLockState = ModSkillLockState.Locked;
					}
					list.Add(modSkillMode2);
				}
			}
			return list;
		}

		public Dictionary<CurrencyType, int> GetUpgradeModSkillCost(string spTraitsId)
		{
			if (string.IsNullOrEmpty(spTraitsId))
			{
				return null;
			}
			return base.gameEconomyData.GetSPTraitsRemodeDefinition(spTraitsId)?.GetUpgradeCost();
		}

		public bool CanUpgradeModSkill(string spTraitsId)
		{
			Dictionary<CurrencyType, int> upgradeModSkillCost = GetUpgradeModSkillCost(spTraitsId);
			if (upgradeModSkillCost == null || upgradeModSkillCost.Count == 0)
			{
				return false;
			}
			if (OfflineManager.IsFreeAll) return true;
			foreach (KeyValuePair<CurrencyType, int> item in upgradeModSkillCost)
			{
				CurrencyModel currency = base.manager.Player.GetCurrency(item.Key);
				if (currency == null || currency.Value < item.Value)
				{
					return false;
				}
			}
			return true;
		}

		public Dictionary<CurrencyType, int> GetMissingUpgradeModSkillCost(string spTraitsId)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			Dictionary<CurrencyType, int> upgradeModSkillCost = GetUpgradeModSkillCost(spTraitsId);
			if (upgradeModSkillCost == null || upgradeModSkillCost.Count == 0)
			{
				return dictionary;
			}
			foreach (KeyValuePair<CurrencyType, int> item in upgradeModSkillCost)
			{
				int num = base.manager.Player.GetCurrency(item.Key)?.Value ?? 0;
				int num2 = item.Value - num;
				if (num2 > 0)
				{
					dictionary.Add(item.Key, num2);
				}
			}
			return dictionary;
		}

		public TWDModelResult UpgradeModSkill(string modSkillModeID, string groupID)
		{
			ModSkillMode modSkillMode = GetModSkillMode(modSkillModeID, groupID);
			if (modSkillMode == null)
			{
				return TWDModelResult.Error;
			}
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillMode.GetSpTraitsDefaultTrait();
			if (spTraitsDefaultTrait == null || !spTraitsDefaultTrait.Available)
			{
				return TWDModelResult.Error;
			}
			if (modSkillMode.IsMaxLevel())
			{
				return TWDModelResult.AlreadyMaxLevel;
			}
			ModSkillUpgradeResult modSkillUpgradeResult = modSkillMode.Upgrade();
			if (modSkillUpgradeResult == null)
			{
				return TWDModelResult.Error;
			}
			if (modSkillMode.ModSkillState == ModSkillState.Equipped)
			{
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillUpgradeResult.OldId);
				if (sPTraitsRemodeDefinition.PassiveTraits != null && sPTraitsRemodeDefinition.PassiveTraits.Count > 0)
				{
					foreach (string passiveTrait in sPTraitsRemodeDefinition.PassiveTraits)
					{
						modSkillMode.EquipmentItemModel.RemoveModSkillPassiveTrait(passiveTrait);
					}
				}
				if (modSkillUpgradeResult.NextTraitDef.PassiveTraits != null && modSkillUpgradeResult.NextTraitDef.PassiveTraits.Count > 0)
				{
					foreach (string passiveTrait2 in modSkillUpgradeResult.NextTraitDef.PassiveTraits)
					{
						modSkillMode.EquipmentItemModel.ApplyModSkillPassiveTraitToOwner(passiveTrait2);
					}
				}
			}
			base.manager.TdMetrics.SetEventType("EquipRemold");
			base.manager.TdMetrics.AddProperty("RemoldSkillUpgrade", new BiEquipRemold(modSkillUpgradeResult.NextTraitDef.ID, modSkillUpgradeResult.NextTraitDef.Level));
			base.manager.TdMetrics.Send();
			return TWDModelResult.OK;
		}

		public ModSkillRewardResult AddRemoldSkill(string spTraitsGroupId, int amount)
		{
			if (base.manager == null || string.IsNullOrEmpty(spTraitsGroupId))
			{
				return new ModSkillRewardResult();
			}
			if (ModSkillModes == null)
			{
				ModSkillModes = new ModelList<ModSkillMode>();
				ModSkillModes.SetManager(base.manager);
				ModSkillModes.Initialize();
			}
			ModSkillMode existingReward = GetExistingReward(spTraitsGroupId);
			SPTraitsRemoldDefinitions minRemoldDefinition = GetMinRemoldDefinition(spTraitsGroupId);
			if (minRemoldDefinition == null)
			{
				return new ModSkillRewardResult();
			}
			if (existingReward != null)
			{
				return ModSkillRewardResult.DuplicateResult(GiveDuplicateRemoldSkillTokens(minRemoldDefinition));
			}
			ModSkillMode modSkillMode = new ModSkillMode(minRemoldDefinition.ID, minRemoldDefinition.Type, minRemoldDefinition.AvailableClass, ModSkillState.Unequipped, null, ModSkillLockState.Unlocked);
			modSkillMode.SetManager(base.manager);
			modSkillMode.Initialize();
			modSkillMode.Start();
			ModSkillModes.Add(modSkillMode);
			NotifyChange("ModSkillMadeEvent");
			return ModSkillRewardResult.NewAcquisitionResult(modSkillMode);
		}

		private SPTraitsRemoldDefinitions GetMinRemoldDefinition(string spTraitsGroupId)
		{
			List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = base.gameEconomyData.GetSPTraitsRemodeDefinitionByType(spTraitsGroupId);
			if (sPTraitsRemodeDefinitionByType == null || sPTraitsRemodeDefinitionByType.Count == 0)
			{
				return null;
			}
			return sPTraitsRemodeDefinitionByType.OrderBy((SPTraitsRemoldDefinitions x) => x.Level).FirstOrDefault();
		}

		private Rewards GiveDuplicateRemoldSkillTokens(SPTraitsRemoldDefinitions remoldDefinition)
		{
			Dictionary<CurrencyType, int> dictionary = remoldDefinition?.GetMakingCost();
			if (dictionary == null || dictionary.Count == 0)
			{
				return null;
			}
			Rewards rewards = new Rewards();
			foreach (KeyValuePair<CurrencyType, int> item in dictionary)
			{
				if (base.manager.Player.GetCurrency(item.Key) != null && item.Value > 0)
				{
					rewards.AddRewardCurrency(item.Key, item.Value, isDiamondExchange: false, canOverflowMax: false);
				}
			}
			if (rewards.Count == 0)
			{
				return null;
			}
			rewards.Give(base.manager);
			return rewards;
		}

		private ModSkillMode GetExistingReward(string spTraitsGroupId)
		{
			if (ModSkillModes != null && ModSkillModes.Count > 0)
			{
				for (int i = 0; i < ModSkillModes.Count; i++)
				{
					if (ModSkillModes[i].Type == spTraitsGroupId)
					{
						return ModSkillModes[i];
					}
				}
			}
			return null;
		}

		public bool ResetModSkill(ModSkillMode modSkill)
		{
			if (modSkill != null)
			{
				GetModSkillMode(modSkill.ID, modSkill.Type).Reset();
			}
			return false;
		}

		public override bool IsValid()
		{
			return true;
		}



		#region myparams
		public ModelList<ModSkillMode> ModSkillModesBackup { get; set; }
		#endregion
	}
}
