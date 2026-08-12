using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class SpEquipmentRemoldModel : TWDModelObject
	{
		private const string SYSTEM_ID_EQUIP_REMOLD = "SystemBase.SPEquipRemold";

		public List<SPTraitSlot> SPTraitSlots { get; set; }

		public List<SPTraitSlot> PendingSPTraitSlots { get; private set; }

		public bool HasPendingRemold { get; private set; }

		public override void Initialize()
		{
			base.Initialize();
			if (SPTraitSlots == null)
			{
				SPTraitSlots = new List<SPTraitSlot>();
			}
			if (PendingSPTraitSlots == null)
			{
				PendingSPTraitSlots = new List<SPTraitSlot>();
			}
			HasPendingRemold = false;
		}

		public override void Start()
		{
			base.Start();
			if (SPTraitSlots == null)
			{
				SPTraitSlots = new List<SPTraitSlot>();
			}
			if (PendingSPTraitSlots == null)
			{
				PendingSPTraitSlots = new List<SPTraitSlot>();
			}
			CheckTraitSlots();
		}

		private void CheckTraitSlots()
		{
			if (base.manager != null && base.manager.Player != null && base.manager.Player.gameEconomyData != null)
			{
				GameEconomyData economyData = base.manager.Player.gameEconomyData;
				if (SPTraitSlots == null)
				{
					SPTraitSlots = new List<SPTraitSlot>();
				}
				if (PendingSPTraitSlots == null)
				{
					PendingSPTraitSlots = new List<SPTraitSlot>();
				}
				UpdateTraitSlots(SPTraitSlots, economyData);
				UpdateTraitSlots(PendingSPTraitSlots, economyData);
			}
		}

		private void UpdateTraitSlots(List<SPTraitSlot> traitSlots, GameEconomyData economyData)
		{
			if (traitSlots == null || traitSlots.Count == 0)
			{
				return;
			}
			foreach (SPTraitSlot traitSlot in traitSlots)
			{
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = economyData.GetSPTraitsRemodeDefinition(traitSlot.ID);
				if (sPTraitsRemodeDefinition == null)
				{
					continue;
				}
				if (sPTraitsRemodeDefinition.MaxLevel != traitSlot.MaxLevel)
				{
					traitSlot.MaxLevel = sPTraitsRemodeDefinition.MaxLevel;
				}
				if (traitSlot.LockState == SPTraitsLockState.ForceLocked)
				{
					if (!sPTraitsRemodeDefinition.Locked)
					{
						traitSlot.LockState = SPTraitsLockState.Unlocked;
					}
				}
				else if (sPTraitsRemodeDefinition.Locked)
				{
					traitSlot.LockState = SPTraitsLockState.ForceLocked;
				}
			}
		}

		public void InitializeTraits(EquipmentDefinition equipmentDefinition, ModelRandom random)
		{
			if (base.manager == null || base.manager.Player == null || equipmentDefinition == null || base.gameEconomyData == null)
			{
				return;
			}
			SPTraitSlots = new List<SPTraitSlot>();
			List<string> equipTypeFilters = new List<string>(new string[2]
			{
				equipmentDefinition.Category.ToString(),
				equipmentDefinition.Type.ToString()
			});
			if (equipmentDefinition.SPTraitsRemoldType != null && equipmentDefinition.SPTraitsRemoldType.Count > 0)
			{
				foreach (string item in equipmentDefinition.SPTraitsRemoldType)
				{
					if (string.IsNullOrEmpty(item))
					{
						continue;
					}
					List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = base.gameEconomyData.GetSPTraitsRemodeDefinitionByType(item);
					if (sPTraitsRemodeDefinitionByType == null || sPTraitsRemodeDefinitionByType.Count == 0)
					{
						continue;
					}
					List<SPTraitsRemoldDefinitions> list = sPTraitsRemodeDefinitionByType.Where(delegate(SPTraitsRemoldDefinitions traitDef)
					{
						if (traitDef.SurvivorClass != null && traitDef.SurvivorClass.Count > 0 && !traitDef.SurvivorClass.Contains(equipmentDefinition.SurvivorClass.ToString()))
						{
							return false;
						}
						if (traitDef.EquipType != null && traitDef.EquipType.Count > 0)
						{
							bool flag = false;
							foreach (string item2 in equipTypeFilters)
							{
								if (traitDef.EquipType.Contains(item2))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								return false;
							}
						}
						return true;
					}).ToList();
					if (list.Count == 0)
					{
						continue;
					}
					int minLevel = list.Min((SPTraitsRemoldDefinitions t) => t.Level);
					SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions = list.FirstOrDefault((SPTraitsRemoldDefinitions t) => t.Level == minLevel);
					if (CheckTraitCompatibilityWithOthers(sPTraitsRemoldDefinitions.ID, SPTraitSlots, -1))
					{
						SPTraitSlots.Add(new SPTraitSlot(sPTraitsRemoldDefinitions.ID));
						if (SPTraitSlots.Count >= 6)
						{
							break;
						}
					}
				}
			}
			HashSet<string> existingTraitIds = new HashSet<string>(GetTraitIds());
			if (SPTraitSlots.Count < 6)
			{
				FillTraitsFromDefaultPackages(equipmentDefinition, existingTraitIds, random);
			}
			if (SPTraitSlots.Count > 6)
			{
				SPTraitSlots = SPTraitSlots.Take(6).ToList();
			}
			InitializeSPTraitSlotsData();
		}

		private bool HasTraitId(string traitId)
		{
			if (SPTraitSlots != null)
			{
				return SPTraitSlots.Any((SPTraitSlot slot) => slot.ID == traitId);
			}
			return false;
		}

		private List<string> GetTraitIds()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return new List<string>();
			}
			if (SPTraitSlots == null)
			{
				return new List<string>();
			}
			return SPTraitSlots.Select((SPTraitSlot slot) => slot.ID).ToList();
		}

		private void FillTraitsFromDefaultPackages(EquipmentDefinition equipmentDefinition, HashSet<string> existingTraitIds, ModelRandom random)
		{
			List<SPTraitsDefaultTrait> list = base.gameEconomyData.SPTraitsRemoldConfigs?.GetDefaultTraits();
			if (list == null || list.Count == 0)
			{
				return;
			}
			foreach (SPTraitsDefaultTrait item in list)
			{
				string type = item.Type;
				int count = item.Count;
				SPTraitsRemoldRandomPackage sPTraitsRemoldRandomPackage = base.gameEconomyData.GetSPTraitsRemoldRandomPackage(type);
				if (sPTraitsRemoldRandomPackage == null || sPTraitsRemoldRandomPackage.TraitsRemoldInfos == null || sPTraitsRemoldRandomPackage.TraitsRemoldInfos.Count == 0)
				{
					continue;
				}
				List<SPTraitWithWeight> list2 = new List<SPTraitWithWeight>();
				foreach (KeyValuePair<string, int> traitsRemoldInfo in sPTraitsRemoldRandomPackage.TraitsRemoldInfos)
				{
					string key = traitsRemoldInfo.Key;
					int value = traitsRemoldInfo.Value;
					if (string.IsNullOrEmpty(key))
					{
						continue;
					}
					List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = base.gameEconomyData.GetSPTraitsRemodeDefinitionByType(key);
					if (sPTraitsRemodeDefinitionByType == null)
					{
						continue;
					}
					foreach (SPTraitsRemoldDefinitions item2 in sPTraitsRemodeDefinitionByType)
					{
						if (string.Equals(item2.Type, key) && !existingTraitIds.Contains(item2.ID) && (item2.SurvivorClass == null || item2.SurvivorClass.Count <= 0 || item2.SurvivorClass.Contains(equipmentDefinition.SurvivorClass.ToString())) && (item2.EquipType == null || item2.EquipType.Count <= 0 || !item2.EquipType.Contains(equipmentDefinition.Type.ToString())) && CheckTraitCompatibilityForInitialization(item2.ID))
						{
							list2.Add(new SPTraitWithWeight(item2, value));
						}
					}
				}
				if (list2.Count == 0)
				{
					continue;
				}
				int minLevel = list2.Min((SPTraitWithWeight t) => t.TraitDef.Level);
				List<SPTraitWithWeight> list3 = list2.Where((SPTraitWithWeight t) => t.TraitDef.Level == minLevel).ToList();
				int count2 = Math.Min(count, list3.Count);
				foreach (SPTraitWithWeight item3 in random.WeightedRandomList(list3, count2, (SPTraitWithWeight x) => x.Weight, isRepeat: false))
				{
					if (item3 != null && item3.TraitDef != null && SPTraitSlots.Count < 6 && CheckTraitCompatibilityForInitialization(item3.TraitDef.ID))
					{
						SPTraitSlots.Add(new SPTraitSlot(item3.TraitDef.ID));
						existingTraitIds.Add(item3.TraitDef.ID);
					}
				}
				if (SPTraitSlots.Count >= 6)
				{
					break;
				}
			}
		}

		private List<string> GetRandomTraitsFromPackage(string packageTag, int count, EquipmentType equipmentType, SurvivorClass survivorClass, HashSet<string> existingTraitIds)
		{
			List<string> list = new List<string>();
			SPTraitsRemoldRandomPackage[] sPTraitsRemoldRandomPackages = base.gameEconomyData.SPTraitsRemoldRandomPackages;
			if (sPTraitsRemoldRandomPackages == null)
			{
				return list;
			}
			List<SPTraitWithWeight> list2 = new List<SPTraitWithWeight>();
			SPTraitsRemoldRandomPackage[] array = sPTraitsRemoldRandomPackages;
			foreach (SPTraitsRemoldRandomPackage sPTraitsRemoldRandomPackage in array)
			{
				if (sPTraitsRemoldRandomPackage.PackageTag != packageTag || sPTraitsRemoldRandomPackage.TraitsRemoldInfos == null || sPTraitsRemoldRandomPackage.TraitsRemoldInfos.Count == 0)
				{
					continue;
				}
				foreach (KeyValuePair<string, int> traitsRemoldInfo in sPTraitsRemoldRandomPackage.TraitsRemoldInfos)
				{
					string key = traitsRemoldInfo.Key;
					int value = traitsRemoldInfo.Value;
					if (string.IsNullOrEmpty(key))
					{
						continue;
					}
					SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition = base.gameEconomyData.SPTraitsRemodeDefinition;
					if (sPTraitsRemodeDefinition == null)
					{
						continue;
					}
					SPTraitsRemoldDefinitions[] array2 = sPTraitsRemodeDefinition;
					foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in array2)
					{
						if (string.Equals(sPTraitsRemoldDefinitions.Type, key) && (sPTraitsRemoldDefinitions.SurvivorClass == null || sPTraitsRemoldDefinitions.SurvivorClass.Count <= 0 || !sPTraitsRemoldDefinitions.SurvivorClass.Contains(survivorClass.ToString())) && (sPTraitsRemoldDefinitions.EquipType == null || sPTraitsRemoldDefinitions.EquipType.Count <= 0 || !sPTraitsRemoldDefinitions.EquipType.Contains(equipmentType.ToString())) && CheckTraitCompatibility(sPTraitsRemoldDefinitions.ID, existingTraitIds) && !existingTraitIds.Contains(sPTraitsRemoldDefinitions.ID))
						{
							list2.Add(new SPTraitWithWeight(sPTraitsRemoldDefinitions, value));
						}
					}
				}
			}
			if (list2.Count > 0)
			{
				int count2 = Math.Min(count, list2.Count);
				foreach (SPTraitWithWeight item in base.manager.Player.PlayerRandom.WeightedRandomList(list2, count2, (SPTraitWithWeight x) => x.Weight, isRepeat: false))
				{
					if (item != null && item.TraitDef != null)
					{
						list.Add(item.TraitDef.ID);
					}
				}
			}
			return list;
		}

		private List<string> GetTraitsFromPackage(string packageId, EquipmentType equipmentType, SurvivorClass survivorClass, HashSet<string> existingTraitIds)
		{
			return GetRandomTraitsFromPackage(packageId, int.MaxValue, equipmentType, survivorClass, existingTraitIds);
		}

		private bool CheckTraitCompatibility(string newTraitId, HashSet<string> existingTraitIds)
		{
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(newTraitId);
			if (sPTraitsRemodeDefinition == null)
			{
				return false;
			}
			string type = sPTraitsRemodeDefinition.Type;
			foreach (string existingTraitId in existingTraitIds)
			{
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition2 = base.gameEconomyData.GetSPTraitsRemodeDefinition(existingTraitId);
				if (sPTraitsRemodeDefinition2 != null && sPTraitsRemodeDefinition2.ExclusionForSp != null && sPTraitsRemodeDefinition2.ExclusionForSp.Count > 0 && sPTraitsRemodeDefinition2.ExclusionForSp.Contains(type))
				{
					return false;
				}
			}
			return true;
		}

		private void InitializeSPTraitSlotsData()
		{
			if (base.manager == null || base.manager.Player == null || SPTraitSlots == null || base.gameEconomyData == null)
			{
				return;
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot == null)
				{
					continue;
				}
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(sPTraitSlot.ID);
				if (sPTraitsRemodeDefinition != null)
				{
					if (sPTraitsRemodeDefinition.Locked)
					{
						sPTraitSlot.LockState = SPTraitsLockState.ForceLocked;
					}
					else
					{
						sPTraitSlot.LockState = SPTraitsLockState.Unlocked;
					}
					sPTraitSlot.Level = sPTraitsRemodeDefinition.Level;
					sPTraitSlot.MaxLevel = sPTraitsRemodeDefinition.MaxLevel;
					sPTraitSlot.CanUpgrade = sPTraitsRemodeDefinition.UpgradeType == 1;
				}
			}
		}

		private SPTraitSlot GetTraitSlotOrPendingSPTraitSlot(string traitId)
		{
			if (string.IsNullOrEmpty(traitId))
			{
				return null;
			}
			return (HasPendingRemold ? PendingSPTraitSlots : SPTraitSlots)?.FirstOrDefault((SPTraitSlot slot) => slot.ID == traitId);
		}

		private SPTraitSlot GetSlot(string traitId)
		{
			if (string.IsNullOrEmpty(traitId))
			{
				return null;
			}
			return SPTraitSlots?.FirstOrDefault((SPTraitSlot slot) => slot.ID == traitId);
		}

		public SPTraitsLockState GetTraitLockState(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return SPTraitsLockState.Unlocked;
			}
			return GetTraitSlotOrPendingSPTraitSlot(traitId)?.LockState ?? SPTraitsLockState.Unlocked;
		}

		public bool IsTraitLocked(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			SPTraitsLockState traitLockState = GetTraitLockState(traitId);
			if (traitLockState != SPTraitsLockState.Locked)
			{
				return traitLockState == SPTraitsLockState.ForceLocked;
			}
			return true;
		}

		public bool IsTraitForceLocked(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			return GetTraitLockState(traitId) == SPTraitsLockState.ForceLocked;
		}

		public bool CanToggleTraitLock(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			return !IsTraitForceLocked(traitId);
		}

		public bool ToggleTraitLock(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			if (!CanToggleTraitLock(traitId))
			{
				return false;
			}
			SPTraitSlot traitSlotOrPendingSPTraitSlot = GetTraitSlotOrPendingSPTraitSlot(traitId);
			if (traitSlotOrPendingSPTraitSlot == null)
			{
				return false;
			}
			if (traitSlotOrPendingSPTraitSlot.LockState == SPTraitsLockState.Unlocked)
			{
				traitSlotOrPendingSPTraitSlot.LockState = SPTraitsLockState.Locked;
			}
			else if (traitSlotOrPendingSPTraitSlot.LockState == SPTraitsLockState.Locked)
			{
				traitSlotOrPendingSPTraitSlot.LockState = SPTraitsLockState.Unlocked;
			}
			return true;
		}

		public bool LockTrait(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			if (IsTraitForceLocked(traitId))
			{
				return false;
			}
			SPTraitSlot traitSlotOrPendingSPTraitSlot = GetTraitSlotOrPendingSPTraitSlot(traitId);
			if (traitSlotOrPendingSPTraitSlot == null)
			{
				return false;
			}
			traitSlotOrPendingSPTraitSlot.LockState = SPTraitsLockState.Locked;
			return true;
		}

		public bool UnlockTrait(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			if (IsTraitForceLocked(traitId))
			{
				return false;
			}
			SPTraitSlot traitSlotOrPendingSPTraitSlot = GetTraitSlotOrPendingSPTraitSlot(traitId);
			if (traitSlotOrPendingSPTraitSlot == null)
			{
				return false;
			}
			traitSlotOrPendingSPTraitSlot.LockState = SPTraitsLockState.Unlocked;
			return true;
		}

		public int GetLockedNoForceTraitCount()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return 0;
			}
			if (SPTraitSlots == null)
			{
				return 0;
			}
			int num = 0;
			foreach (SPTraitSlot item in HasPendingRemold ? PendingSPTraitSlots : SPTraitSlots)
			{
				if (item != null && item.LockState == SPTraitsLockState.Locked)
				{
					num++;
				}
			}
			return num;
		}

		public int GetNormalLockedTraitCount()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return 0;
			}
			if (SPTraitSlots == null)
			{
				return 0;
			}
			int num = 0;
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot != null && sPTraitSlot.LockState == SPTraitsLockState.Locked)
				{
					num++;
				}
			}
			return num;
		}

		public Dictionary<CurrencyType, int> GetRemoldBaseCost()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return new Dictionary<CurrencyType, int>();
			}
			Dictionary<CurrencyType, int> dictionary = base.gameEconomyData.SPTraitsRemoldConfigs?.GetRemoldCost();
			if (dictionary == null)
			{
				return new Dictionary<CurrencyType, int>();
			}
			return new Dictionary<CurrencyType, int>(dictionary);
		}

		public Dictionary<CurrencyType, int> CalculateRemoldLockedCost(int lockedCount)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			if (base.manager == null || base.manager.Player == null)
			{
				return new Dictionary<CurrencyType, int>();
			}
			if (lockedCount <= 0)
			{
				return dictionary;
			}
			Dictionary<CurrencyType, int> dictionary2 = base.gameEconomyData.SPTraitsRemoldConfigs?.GetRemoldCostForLocked();
			if (dictionary2 != null && dictionary2.Count > 0)
			{
				foreach (KeyValuePair<CurrencyType, int> item in dictionary2)
				{
					dictionary[item.Key] = item.Value * lockedCount;
				}
			}
			return dictionary;
		}

		public bool CanRemoldTraits()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			if (SPTraitSlots == null || SPTraitSlots.Count == 0)
			{
				return false;
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot != null && sPTraitSlot.LockState != SPTraitsLockState.ForceLocked)
				{
					return true;
				}
			}
			return false;
		}

		public bool RemoldTraits(EquipmentDefinition equipmentDefinition)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			if (!CanRemoldTraits() || equipmentDefinition == null || base.gameEconomyData == null)
			{
				return false;
			}
			List<string> sPTraitsRemoldRandomPackage = equipmentDefinition.SPTraitsRemoldRandomPackage;
			if (sPTraitsRemoldRandomPackage == null || sPTraitsRemoldRandomPackage.Count == 0)
			{
				return false;
			}
			List<SPTraitSlot> source = (HasPendingRemold ? PendingSPTraitSlots : SPTraitSlots);
			List<SPTraitSlot> list = CloneTraitSlots(source);
			List<string> equipTypeFilters = new List<string>(new string[2]
			{
				equipmentDefinition.Category.ToString(),
				equipmentDefinition.Type.ToString()
			});
			for (int i = 0; i < list.Count; i++)
			{
				SPTraitSlot sPTraitSlot = list[i];
				if (sPTraitSlot.LockState == SPTraitsLockState.ForceLocked || sPTraitSlot.LockState == SPTraitsLockState.Locked)
				{
					continue;
				}
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(sPTraitSlot.ID);
				if (sPTraitsRemodeDefinition == null)
				{
					continue;
				}
				List<SPTraitWithWeight> availableTraitsForRemold = GetAvailableTraitsForRemold(sPTraitsRemodeDefinition, sPTraitsRemoldRandomPackage, equipmentDefinition.SurvivorClass.ToString(), equipTypeFilters, list, i);
				if (availableTraitsForRemold.Count > 0)
				{
					List<SPTraitWithWeight> list2 = base.manager.Player.PlayerRandom.WeightedRandomList(availableTraitsForRemold, 1, (SPTraitWithWeight x) => x.Weight, isRepeat: false);
					if (list2.Count > 0 && list2[0] != null && list2[0].TraitDef != null)
					{
						SPTraitsRemoldDefinitions traitDef = list2[0].TraitDef;
						sPTraitSlot.ID = traitDef.ID;
						sPTraitSlot.Level = traitDef.Level;
						sPTraitSlot.MaxLevel = traitDef.MaxLevel;
						sPTraitSlot.CanUpgrade = traitDef.UpgradeType == 1;
					}
				}
			}
			PendingSPTraitSlots = list;
			HasPendingRemold = true;
			return true;
		}

		public bool ConfirmRemold()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			if (!HasPendingRemold || PendingSPTraitSlots == null)
			{
				return false;
			}
			SPTraitSlots = CloneTraitSlots(PendingSPTraitSlots);
			PendingSPTraitSlots.Clear();
			HasPendingRemold = false;
			return true;
		}

		public void CancelRemold()
		{
			if (base.manager != null && base.manager.Player != null)
			{
				PendingSPTraitSlots.Clear();
				HasPendingRemold = false;
			}
		}

		public List<SPTraitSlot> GetDisplaySPTraitSlots(bool forRemoldUI = false)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return new List<SPTraitSlot>();
			}
			if (forRemoldUI && HasPendingRemold && PendingSPTraitSlots != null)
			{
				return PendingSPTraitSlots;
			}
			return SPTraitSlots;
		}

		private List<SPTraitSlot> CloneTraitSlots(List<SPTraitSlot> source)
		{
			if (source == null)
			{
				return new List<SPTraitSlot>();
			}
			List<SPTraitSlot> list = new List<SPTraitSlot>();
			foreach (SPTraitSlot item in source)
			{
				list.Add(new SPTraitSlot
				{
					ID = item.ID,
					LockState = item.LockState,
					Level = item.Level,
					MaxLevel = item.MaxLevel,
					CanUpgrade = item.CanUpgrade
				});
			}
			return list;
		}

		private List<SPTraitWithWeight> GetAvailableTraitsForRemold(SPTraitsRemoldDefinitions currentTrait, List<string> availablePackages, string survivorClass, List<string> equipTypeFilters, List<SPTraitSlot> allSlots, int currentIndex)
		{
			Dictionary<string, SPTraitWithWeight> dictionary = new Dictionary<string, SPTraitWithWeight>();
			List<string> list = new List<string>();
			foreach (SPTraitSlot allSlot in allSlots)
			{
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(allSlot.ID);
				if (sPTraitsRemodeDefinition != null)
				{
					list.Add(sPTraitsRemodeDefinition.Type);
				}
			}
			List<SPTraitsRemoldRandomPackage> list2 = new List<SPTraitsRemoldRandomPackage>();
			foreach (string availablePackage in availablePackages)
			{
				SPTraitsRemoldRandomPackage sPTraitsRemoldRandomPackage = base.gameEconomyData.GetSPTraitsRemoldRandomPackage(availablePackage);
				if (sPTraitsRemoldRandomPackage != null)
				{
					list2.Add(sPTraitsRemoldRandomPackage);
				}
			}
			if (list2 == null)
			{
				return dictionary.Values.ToList();
			}
			foreach (SPTraitsRemoldRandomPackage item in list2)
			{
				if (item.PackageStar != currentTrait.Star || (currentTrait.TagMatch != null && currentTrait.TagMatch.Count > 0 && currentTrait.TagMatch.Contains(item.PackageTag)) || item.TraitsRemoldInfos == null || item.TraitsRemoldInfos.Count == 0)
				{
					continue;
				}
				foreach (KeyValuePair<string, int> traitsRemoldInfo in item.TraitsRemoldInfos)
				{
					string key = traitsRemoldInfo.Key;
					int value = traitsRemoldInfo.Value;
					if (string.IsNullOrEmpty(key) || list.Contains(key))
					{
						continue;
					}
					List<SPTraitsRemoldDefinitions> sPTraitsRemodeDefinitionByType = base.gameEconomyData.GetSPTraitsRemodeDefinitionByType(key);
					if (sPTraitsRemodeDefinitionByType == null)
					{
						continue;
					}
					foreach (SPTraitsRemoldDefinitions item2 in sPTraitsRemodeDefinitionByType)
					{
						if (item2.Star != currentTrait.Star || item2.Level != currentTrait.Level || item2.UpgradeType != 1 || (item2.SurvivorClass != null && item2.SurvivorClass.Count > 0 && !item2.SurvivorClass.Contains(survivorClass)))
						{
							continue;
						}
						if (item2.EquipType != null && item2.EquipType.Count > 0)
						{
							bool flag = false;
							foreach (string equipTypeFilter in equipTypeFilters)
							{
								if (item2.EquipType.Contains(equipTypeFilter))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								continue;
							}
						}
						if (!CheckTraitCompatibilityWithOthers(item2.ID, allSlots, currentIndex) || item2.ID == currentTrait.ID)
						{
							continue;
						}
						bool flag2 = false;
						foreach (SPTraitSlot allSlot2 in allSlots)
						{
							if (allSlot2.ID == item2.ID)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2 && !dictionary.ContainsKey(item2.ID))
						{
							dictionary[item2.ID] = new SPTraitWithWeight(item2, value);
						}
					}
				}
			}
			return dictionary.Values.ToList();
		}

		private bool CheckTraitCompatibilityWithOthers(string newTraitId, List<SPTraitSlot> allSlots, int currentIndex)
		{
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(newTraitId);
			if (sPTraitsRemodeDefinition == null)
			{
				return false;
			}
			List<string> exclusionForSp = sPTraitsRemodeDefinition.ExclusionForSp;
			for (int i = 0; i < allSlots.Count; i++)
			{
				if (i == currentIndex)
				{
					continue;
				}
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition2 = base.gameEconomyData.GetSPTraitsRemodeDefinition(allSlots[i].ID);
				if (sPTraitsRemodeDefinition2 == null)
				{
					continue;
				}
				List<string> exclusionForSp2 = sPTraitsRemodeDefinition2.ExclusionForSp;
				if (exclusionForSp == null || exclusionForSp.Count <= 0 || exclusionForSp2 == null || exclusionForSp2.Count <= 0)
				{
					continue;
				}
				foreach (string item in exclusionForSp2)
				{
					if (exclusionForSp.Contains(item))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CheckTraitCompatibilityForInitialization(string newTraitId)
		{
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(newTraitId);
			if (sPTraitsRemodeDefinition == null)
			{
				return false;
			}
			string type = sPTraitsRemodeDefinition.Type;
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot == null)
				{
					continue;
				}
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition2 = base.gameEconomyData.GetSPTraitsRemodeDefinition(sPTraitSlot.ID);
				if (sPTraitsRemodeDefinition2 != null)
				{
					string type2 = sPTraitsRemodeDefinition2.Type;
					if (sPTraitsRemodeDefinition2.ExclusionForSp != null && sPTraitsRemodeDefinition2.ExclusionForSp.Count > 0 && sPTraitsRemodeDefinition2.ExclusionForSp.Contains(type))
					{
						return false;
					}
					if (sPTraitsRemodeDefinition.ExclusionForSp != null && sPTraitsRemodeDefinition.ExclusionForSp.Count > 0 && sPTraitsRemodeDefinition.ExclusionForSp.Contains(type2))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool CanUpgradeTrait(string traitId)
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			SPTraitSlot slot = GetSlot(traitId);
			if (slot == null)
			{
				return false;
			}
			if (!slot.CanUpgrade)
			{
				return false;
			}
			if (slot.Level >= slot.MaxLevel)
			{
				return false;
			}
			if (string.IsNullOrEmpty(GetNextLevelTraitId(traitId, slot.Level)))
			{
				return false;
			}
			return true;
		}

		private string GetNextLevelTraitId(string currentTraitId, int currentLevel)
		{
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(currentTraitId);
			if (sPTraitsRemodeDefinition == null)
			{
				return null;
			}
			SPTraitsRemoldDefinitions[] sPTraitsRemodeDefinition2 = base.gameEconomyData.SPTraitsRemodeDefinition;
			if (sPTraitsRemodeDefinition2 == null)
			{
				return null;
			}
			SPTraitsRemoldDefinitions[] array = sPTraitsRemodeDefinition2;
			foreach (SPTraitsRemoldDefinitions sPTraitsRemoldDefinitions in array)
			{
				if (!(sPTraitsRemoldDefinitions.ID == currentTraitId) && !(sPTraitsRemoldDefinitions.Type != sPTraitsRemodeDefinition.Type) && sPTraitsRemoldDefinitions.Level == currentLevel + 1)
				{
					return sPTraitsRemoldDefinitions.ID;
				}
			}
			return null;
		}

		public string UpgradeTrait(string traitId)
		{
			if (!CanUpgradeTrait(traitId))
			{
				return null;
			}
			SPTraitSlot slot = GetSlot(traitId);
			if (slot == null)
			{
				return null;
			}
			if (slot.Level >= slot.MaxLevel)
			{
				return null;
			}
			string nextLevelTraitId = GetNextLevelTraitId(traitId, slot.Level);
			if (string.IsNullOrEmpty(nextLevelTraitId))
			{
				return null;
			}
			SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(nextLevelTraitId);
			if (sPTraitsRemodeDefinition == null)
			{
				return null;
			}
			if (sPTraitsRemodeDefinition.Level != slot.Level + 1)
			{
				return null;
			}
			if (sPTraitsRemodeDefinition.Level > sPTraitsRemodeDefinition.MaxLevel)
			{
				return null;
			}
			slot.ID = nextLevelTraitId;
			slot.Level = sPTraitsRemodeDefinition.Level;
			slot.MaxLevel = sPTraitsRemodeDefinition.MaxLevel;
			slot.CanUpgrade = sPTraitsRemodeDefinition.UpgradeType == 1;
			NotifyChange("SpEquipmentRemoldTraitsUpgrade", new object[2] { traitId, nextLevelTraitId });
			return nextLevelTraitId;
		}

		public Dictionary<CurrencyType, int> CalculateUpgradeCost(string traitId)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			if (base.manager == null || base.manager.Player == null)
			{
				return dictionary;
			}
			SPTraitSlot slot = GetSlot(traitId);
			if (slot == null || !slot.CanUpgrade)
			{
				return dictionary;
			}
			Dictionary<CurrencyType, int> dictionary2 = base.gameEconomyData.SPTraitsRemoldConfigs?.GetUpgradeCost();
			if (dictionary2 != null && dictionary2.Count > 0)
			{
				foreach (KeyValuePair<CurrencyType, int> item in dictionary2)
				{
					dictionary[item.Key] = item.Value;
				}
			}
			return dictionary;
		}

		public bool HasAnyUpgradeableTrait()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			if (SPTraitSlots == null)
			{
				return false;
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot != null && CanUpgradeTrait(sPTraitSlot.ID))
				{
					return true;
				}
			}
			return false;
		}

		public List<string> GetUpgradeableTraitIds()
		{
			List<string> list = new List<string>();
			if (base.manager == null || base.manager.Player == null)
			{
				return list;
			}
			if (SPTraitSlots == null)
			{
				return list;
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot != null && CanUpgradeTrait(sPTraitSlot.ID))
				{
					list.Add(sPTraitSlot.ID);
				}
			}
			return list;
		}

		public Dictionary<CurrencyType, int> CalculateTotalUpgradeCost(string traitId, int fromLevel = 1, int toLevel = 0)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			if (base.manager == null || base.manager.Player == null)
			{
				return dictionary;
			}
			SPTraitSlot slot = GetSlot(traitId);
			if (slot == null || !slot.CanUpgrade)
			{
				return dictionary;
			}
			if (toLevel == 0)
			{
				toLevel = slot.Level;
			}
			if (fromLevel < 1 || toLevel < fromLevel || toLevel > slot.MaxLevel)
			{
				return dictionary;
			}
			Dictionary<CurrencyType, int> dictionary2 = base.gameEconomyData.SPTraitsRemoldConfigs?.GetUpgradeCost();
			if (dictionary2 == null || dictionary2.Count == 0)
			{
				return dictionary;
			}
			int num = toLevel - fromLevel;
			if (num <= 0)
			{
				return dictionary;
			}
			foreach (KeyValuePair<CurrencyType, int> item in dictionary2)
			{
				dictionary[item.Key] = item.Value * num;
			}
			return dictionary;
		}

		public Dictionary<CurrencyType, int> CalculateAllTraitsTotalUpgradeCost(int fromLevel = 1)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			if (base.manager == null || base.manager.Player == null)
			{
				return dictionary;
			}
			if (SPTraitSlots == null || SPTraitSlots.Count == 0)
			{
				return dictionary;
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot == null || sPTraitSlot.LockState == SPTraitsLockState.ForceLocked || !sPTraitSlot.CanUpgrade || sPTraitSlot.Level <= fromLevel)
				{
					continue;
				}
				foreach (KeyValuePair<CurrencyType, int> item in CalculateTotalUpgradeCost(sPTraitSlot.ID, fromLevel, sPTraitSlot.Level))
				{
					if (dictionary.ContainsKey(item.Key))
					{
						dictionary[item.Key] += item.Value;
					}
					else
					{
						dictionary[item.Key] = item.Value;
					}
				}
			}
			return dictionary;
		}

		public bool IsEquipRemoldOpen()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return false;
			}
			SystemOpen systemOpen = base.gameEconomyData?.GetSystemOpenById("SystemBase.SPEquipRemold");
			if (systemOpen == null)
			{
				return false;
			}
			if (base.manager.Player.CouncilLevel >= systemOpen.OpenCampLv)
			{
				return false;
			}
			if (systemOpen.HasDateLimit)
			{
				long utcTimeStamp = base.manager.Player.UtcTimeStamp;
				if (utcTimeStamp < systemOpen.StartTimeMilliseconds || utcTimeStamp > systemOpen.EndTimeMilliseconds)
				{
					return false;
				}
			}
			return true;
		}

		public int GetEquipRemoldShowLevel()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return -1;
			}
			return (base.gameEconomyData?.GetSystemOpenById("SystemBase.SPEquipRemold"))?.ShowCampLv ?? (-1);
		}

		public int GetEquipRemoldOpenLevel()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return -1;
			}
			return (base.gameEconomyData?.GetSystemOpenById("SystemBase.SPEquipRemold"))?.OpenCampLv ?? (-1);
		}

		public SystemOpen GetEquipRemoldSystemOpen()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return null;
			}
			return base.gameEconomyData?.GetSystemOpenById("SystemBase.SPEquipRemold");
		}

		public string GetRandomUpgradeableTraitId()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return string.Empty;
			}
			List<string> upgradeableTraitIds = GetUpgradeableTraitIds();
			if (upgradeableTraitIds == null || upgradeableTraitIds.Count == 0)
			{
				return string.Empty;
			}
			if (base.manager != null && base.manager.Player != null && base.manager.Player.PlayerRandom != null)
			{
				int randomInRange = base.manager.Player.PlayerRandom.GetRandomInRange(0, upgradeableTraitIds.Count - 1);
				if (randomInRange >= 0 && randomInRange < upgradeableTraitIds.Count)
				{
					return upgradeableTraitIds[randomInRange];
				}
			}
			return null;
		}

		public List<TraitDefinition> GetPassiveTraits()
		{
			List<TraitDefinition> list = new List<TraitDefinition>();
			if (base.manager == null || base.manager.Player == null)
			{
				return list;
			}
			if (SPTraitSlots == null || base.gameEconomyData == null)
			{
				return list;
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot == null)
				{
					continue;
				}
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(sPTraitSlot.ID);
				if (sPTraitsRemodeDefinition == null || sPTraitsRemodeDefinition.PassiveTraits == null)
				{
					continue;
				}
				foreach (string passiveTrait in sPTraitsRemodeDefinition.PassiveTraits)
				{
					TraitDefinition traitDefinition = base.gameEconomyData.GetTraitDefinition(passiveTrait);
					if (traitDefinition != null)
					{
						list.Add(traitDefinition);
					}
				}
			}
			return list;
		}

		public List<TraitDefinition> GetActiveTraits()
		{
			Dictionary<string, TraitDefinition> dictionary = new Dictionary<string, TraitDefinition>();
			if (base.manager == null || base.manager.Player == null)
			{
				return dictionary.Values.ToList();
			}
			if (SPTraitSlots == null || base.gameEconomyData == null)
			{
				return dictionary.Values.ToList();
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot == null)
				{
					continue;
				}
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(sPTraitSlot.ID);
				if (sPTraitsRemodeDefinition == null || sPTraitsRemodeDefinition.ActiveTraits == null)
				{
					continue;
				}
				foreach (string activeTrait in sPTraitsRemodeDefinition.ActiveTraits)
				{
					TraitDefinition traitDefinition = base.gameEconomyData.GetTraitDefinition(activeTrait);
					if (traitDefinition != null && !dictionary.ContainsKey(traitDefinition.Identifier))
					{
						dictionary[traitDefinition.Identifier] = traitDefinition;
					}
				}
			}
			return dictionary.Values.ToList();
		}

		public List<TraitDefinition> GetChargeActiveTraits()
		{
			List<TraitDefinition> list = new List<TraitDefinition>();
			if (base.manager == null || base.manager.Player == null)
			{
				return list;
			}
			if (SPTraitSlots == null || base.gameEconomyData == null)
			{
				return list;
			}
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot == null)
				{
					continue;
				}
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(sPTraitSlot.ID);
				if (sPTraitsRemodeDefinition == null || sPTraitsRemodeDefinition.ActiveTraitsForCharge == null)
				{
					continue;
				}
				foreach (string item in sPTraitsRemodeDefinition.ActiveTraitsForCharge)
				{
					TraitDefinition traitDefinition = base.gameEconomyData.GetTraitDefinition(item);
					if (traitDefinition != null)
					{
						list.Add(traitDefinition);
					}
				}
			}
			return list;
		}

		public override bool IsValid()
		{
			return true;
		}

		public string GetRateStr()
		{
			if (base.manager == null || base.manager.Player == null)
			{
				return null;
			}
			if (SPTraitSlots == null || base.gameEconomyData == null)
			{
				return string.Empty;
			}
			int num = 0;
			foreach (SPTraitSlot sPTraitSlot in SPTraitSlots)
			{
				if (sPTraitSlot != null)
				{
					SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(sPTraitSlot.ID);
					if (sPTraitsRemodeDefinition != null)
					{
						num += ((sPTraitsRemodeDefinition.Star == 0) ? 1 : sPTraitsRemodeDefinition.Star) * ((sPTraitsRemodeDefinition.Value == 0) ? 1 : sPTraitsRemodeDefinition.Value) * ((sPTraitsRemodeDefinition.Level == 0) ? 1 : sPTraitsRemodeDefinition.Level);
					}
				}
			}
			return base.gameEconomyData.SPTraitsRemoldConfigs.GetRatingByScore(num);
		}
	}
}
