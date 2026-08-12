using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseModel;
using Newtonsoft.Json;
using TWDModel.ResponsClass;
using TwdCustomMod;

namespace TWDModel
{
	public class EquipmentItemModel : TWDModelObject, IMockData<EquipmentItemMockData>
	{
		public const string ChargeEquipmentTraitTag = "ChargeEquipment";

		[JsonIgnore]
		private ActorModel _owner;

		private List<ModelModifier> equipmentModifiers;

		private int cachedEquipmentLevel = -1;

		private EquipmentLevelDefinition cachedEquipmentLevelDefinition;

		public TimedEffect ReloadingTimedEffect;

		private RarityBasedUpgradeDefinition rarityDefinition;

		private bool equipmentDefinitionInvalid = true;

		private EquipmentDefinition equipmentDefinition;

		private string equipmentDefinitionIdentifier;

		public TimedActionModel TimedActionModel { get; protected set; }

		public SpEquipmentRemoldModel SpEquipmentRemoldModel { get; set; }

		public ModSkillSlot[] ModSkillSlots { get; set; }

		[JsonIgnore]
		public ActorModel Owner
		{
			get
			{
				return base.manager.Player.SurvivorContainer.GetEquipmentHolder(this);
			}
			set
			{
				_owner = value;
			}
		}

		[JsonIgnore]
		public AbilityModel Ability { get; private set; }

		public int TotalUses { get; private set; }

		public int MaxUses { get; private set; }

		public int StartingLevel { get; set; }

		public int EquipmentUpgradeTokenLevelUpgrades { get; set; }

		public int Level { get; set; }

		public Rarity Rarity { get; set; }

		public int RarityLevel { get; set; }

		public List<UpgradeTraitsData> UpgradeTraits { get; set; }

		public string IdForAnalytics { get; set; }

		public bool IsFavourite { get; set; }

		[JsonIgnore]
		public bool IsCanBreak => RarityLevel >= base.manager.Player.gameEconomyData.ConfigData.EquipmentDecompositionRarity;

		[JsonIgnore]
		public bool Is7StarEquipment => RarityLevel == 6;

		[JsonIgnore]
		public bool NeedsReloading => Ability.Definition.NeedsReloading;

		[JsonIgnore]
		public bool LimitOOT => Ability.Definition.LimitOOT;

		[JsonIgnore]
		public bool IsReloading => ReloadingTimedEffect != null;

		[JsonIgnore]
		public int RemainingTurnsToReload
		{
			get
			{
				if (ReloadingTimedEffect != null)
				{
					return ReloadingTimedEffect.Duration - ReloadingTimedEffect.Counter;
				}
				return 0;
			}
		}

		public List<TemporaryTraitsData> TemporaryTraits { get; set; }

		[JsonIgnore]
		public EquipmentLevelDefinition EquipmentLevelDefinition
		{
			get
			{
				if (cachedEquipmentLevel != Level)
				{
					cachedEquipmentLevel = Level;
					cachedEquipmentLevelDefinition = base.gameEconomyData.GetEquipmentLevelDefinition(Level);
				}
				return cachedEquipmentLevelDefinition;
			}
		}

		[JsonIgnore]
		public SurvivorClass EquipmentSurvivorClass
		{
			get
			{
				if (Definition != null)
				{
					return Definition.SurvivorClass;
				}
				return SurvivorClass.None;
			}
		}

		public string GetEquipTypeNormalAndRemold
		{
			get
			{
				if (base.manager != null && Definition != null)
				{
					if (Definition.SwitchRemoldMode)
					{
						return "Remold";
					}
					return "Normal";
				}
				return null;
			}
		}

		[JsonIgnore]
		public RarityBasedUpgradeDefinition RarityDefinition
		{
			get
			{
				if (rarityDefinition == null)
				{
					rarityDefinition = base.manager.GameEconomyData.GetRarityBasedUpgradeDefinition(RarityLevel, UpgradeType.EquipmentUpgrade);
				}
				return rarityDefinition;
			}
		}

		[JsonIgnore]
		public int UpgradeTime
		{
			get
			{
				if (base.manager.Player.ActivityManager.TryGetActivityParam(ActivityType.WeaponSurvivorUpgrades5s, out var activityParams))
				{
					return int.Parse(activityParams[1]);
				}
				EquipmentDefinition definition = Definition;
				if (definition != null && definition.SurvivorClass == base.gameEconomyData.ConfigData.WeeklyEventClassEquipmentUpgrade5s)
				{
					return 5;
				}
				int upgradeTimeBase = EquipmentLevelDefinition.UpgradeTimeBase;
				FixedPoint fixedPoint = (FixedPoint)RarityDefinition.UpgradeTimeMultiplier / (FixedPoint)100.0;
				return (int)(upgradeTimeBase * (1.0 + fixedPoint));
			}
		}

		[JsonIgnore]
		public bool CanUpgrade
		{
			get
			{
				if (CanBeManipulated() && !HasReachedMaxLevel)
				{
					return HasWorkshopLevelRequired;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanUpgradeWithEquipmentUpgradeToken
		{
			get
			{
				if (CanBeManipulated() && HasReachedMaxLevel && HasWorkshopLevelRequiredForEquipmentLevelIncrease)
				{
					return RarityLevel >= base.manager.GameEconomyData.ConfigData.EquipmentLevelUpTokenMinimumRarity;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasReachedMaxLevel => Level >= MaxLevel;

		[JsonIgnore]
		public bool HasWorkshopLevelRequired
		{
			get
			{
				if (Level != StartingLevel)
				{
					return true;
				}
				BuildingModel building = base.manager.CampModel.GetBuilding("Workshop");
				if (building == null)
				{
					return false;
				}
				return building.Level >= EquipmentLevelDefinition.WorkshopLevelRequired;
			}
		}

		[JsonIgnore]
		public bool HasWorkshopLevelRequiredForEquipmentLevelIncrease
		{
			get
			{
				BuildingModel building = base.manager.CampModel.GetBuilding("Workshop");
				if (building == null)
				{
					return false;
				}
				EquipmentLevelDefinition equipmentLevelDefinition = base.gameEconomyData.GetEquipmentLevelDefinition(StartingLevel + EquipmentUpgradeTokenLevelUpgrades + 1);
				return building.Level >= equipmentLevelDefinition.WorkshopLevelRequired;
			}
		}

		[JsonIgnore]
		public int MaxLevel => StartingLevel + RarityDefinition.UpgradesTotal + EquipmentUpgradeTokenLevelUpgrades;

		[JsonIgnore]
		public int GetTotalUpgrades => RarityDefinition.UpgradesTotal;

		[JsonIgnore]
		public Cashier GetScrapCashier
		{
			get
			{
				Cashier cashier = new Cashier(base.manager);
				CashierItem cashierItem = new CashierItem(PurchaseType.Refund);
				int num = 0;
				int num2 = (int)(EquipmentLevelDefinition.ScrapSurvivalPointsBase * (1.0 + (FixedPoint)RarityDefinition.ScrapValueMultiplier / (FixedPoint)100.0));
				for (int i = StartingLevel; i < Level; i++)
				{
					num = GetUpgradeCost(i);
					num2 += (int)(num * ((FixedPoint)RarityDefinition.ScrapUpgradeReturnPercentage / (FixedPoint)100.0));
				}
				num2 = (int)(num2 * base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints).AddMultiplier);
				cashierItem.SetCost(CurrencyType.SurvivalPoints, num2);
				int num3 = 0;
				if (Owner == null && RarityLevel >= 4 && RarityLevel >= base.manager.GameEconomyData.ConfigData.EquipmentDecompositionRarity)
				{
					num3 = ((RarityLevel <= 4 || RarityLevel > 5) ? base.manager.GameEconomyData.ConfigData.GoldWeaponsBreakDownFragmentsNumber : base.manager.GameEconomyData.ConfigData.ApocalypticWeaponsBreakDownFragmentsNumber);
					cashierItem.SetCost(CurrencyType.ApocalypticEquipToken, num3);
				}
				cashier.AddItem(cashierItem);
				return cashier;
			}
		}

		[JsonIgnore]
		public Cashier GetBreakCashier
		{
			get
			{
				Cashier cashier = new Cashier(base.manager);
				CashierItem cashierItem = new CashierItem(PurchaseType.Refund);
				int cost = base.manager.GameEconomyData.ConfigData.EquipBreakApocalypticEquipTokenCount(RarityLevel);
				cashierItem.SetCost(CurrencyType.ApocalypticEquipToken, cost);
				cashier.AddItem(cashierItem);
				return cashier;
			}
		}

		[JsonIgnore]
		public CashierRewardsListCalss GetModSkillCashierReward
		{
			get
			{
				Rewards rewards = new Rewards();
				int num = 0;
				int num2 = 0;
				Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
				Dictionary<CurrencyType, int> dictionary2 = new Dictionary<CurrencyType, int>();
				Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
				if (BreakthroughLevel > 0 && GetEquipTypeNormalAndRemold != null)
				{
					EquipBreakthroughDefinition remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode = base.manager.GameEconomyData.GetRemoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode(RarityLevel, BreakthroughLevel, GetEquipTypeNormalAndRemold);
					if (remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode != null && remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources != null && remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources.Count >= 3)
					{
						num2 = remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[2];
						Rewards rewards2 = new Rewards("ApocalypticEquipToken(" + Math.Abs(num2) + ")");
						rewards.RewardsList.AddRange(rewards2.RewardsList);
						if (remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[1] > 0)
						{
							CurrencyType survivorClassCurrencyType = EquipmentBreakthrough.GetSurvivorClassCurrencyType(Definition.SurvivorClass);
							Rewards rewards3 = new Rewards(survivorClassCurrencyType.ToString() + "(" + Math.Abs(remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[1]) + ")");
							rewards.RewardsList.AddRange(rewards3.RewardsList);
							if (survivorClassCurrencyType != CurrencyType.None)
							{
								if (!dictionary.ContainsKey(survivorClassCurrencyType))
								{
									dictionary[survivorClassCurrencyType] = 0;
								}
								dictionary[survivorClassCurrencyType] += Math.Abs(remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[1]);
							}
						}
						Dictionary<CurrencyType, int> scrapSpTokenReward = GetScrapSpTokenReward(remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode);
						if (scrapSpTokenReward != null)
						{
							foreach (KeyValuePair<CurrencyType, int> item in scrapSpTokenReward)
							{
								Rewards rewards4 = new Rewards(item.Key.ToString() + "(" + Math.Abs(item.Value) + ")");
								rewards.RewardsList.AddRange(rewards4.RewardsList);
								if (!dictionary2.ContainsKey(item.Key))
								{
									dictionary2[item.Key] = 0;
								}
								int value = item.Value;
								dictionary2[item.Key] += Math.Abs(value);
							}
						}
						if (remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[0] > 0)
						{
							EquipTokenDefinition equipTokenDefinitionByRelateEquipId = base.gameEconomyData.GetEquipTokenDefinitionByRelateEquipId(Definition.ID);
							if (equipTokenDefinitionByRelateEquipId != null)
							{
								Rewards rewards5 = new Rewards("EquipToken(" + equipTokenDefinitionByRelateEquipId.EquipTokenId + "," + remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[0] + ")");
								rewards.RewardsList.AddRange(rewards5.RewardsList);
								if (!dictionary3.ContainsKey(equipTokenDefinitionByRelateEquipId.EquipTokenId))
								{
									dictionary3[equipTokenDefinitionByRelateEquipId.EquipTokenId] = 0;
								}
								dictionary3[equipTokenDefinitionByRelateEquipId.EquipTokenId] += remoldEquipBreakthroughDefinitionByRarityAndLevelAndWeaponMode.ScrapResources[0];
							}
						}
					}
					int num3 = 0;
					int num4 = (int)(EquipmentLevelDefinition.ScrapSurvivalPointsBase * (1.0 + (FixedPoint)RarityDefinition.ScrapValueMultiplier / (FixedPoint)100.0));
					for (int i = StartingLevel; i < Level; i++)
					{
						num3 = GetUpgradeCost(i);
						num4 += (int)(num3 * ((FixedPoint)RarityDefinition.ScrapUpgradeReturnPercentage / (FixedPoint)100.0));
					}
					num4 = (int)(num4 * base.manager.Player.GetCurrency(CurrencyType.SurvivalPoints).AddMultiplier);
					Rewards rewards6 = new Rewards(CurrencyType.SurvivalPoints.ToString() + "(" + num4 + ")");
					rewards.RewardsList.AddRange(rewards6.RewardsList);
					num += num4;
				}
				return new CashierRewardsListCalss(rewards, dictionary, dictionary2, dictionary3, num, num2);
			}
		}

		[JsonIgnore]
		public EquipmentDefinition Definition
		{
			get
			{
				if (equipmentDefinitionInvalid)
				{
					equipmentDefinition = base.manager.GameEconomyData.GetEquipmentDefinition(EquipmentDefinitionIdentifier);
					equipmentDefinitionInvalid = false;
				}
				return equipmentDefinition;
			}
			private set
			{
			}
		}

		public string EquipmentDefinitionIdentifier
		{
			get
			{
				return equipmentDefinitionIdentifier;
			}
			set
			{
				if (value != equipmentDefinitionIdentifier)
				{
					equipmentDefinitionInvalid = true;
					equipmentDefinitionIdentifier = value;
				}
			}
		}

		public EquipmentItemModel ChargeEquipment { get; set; }

		public bool IsChargeEquipment { get; set; }

		[JsonIgnore]
		public bool IsWeaponEquipment
		{
			get
			{
				if (Definition != null)
				{
					if (Definition.Category != EquipmentCategory.MeleeWeapon)
					{
						return Definition.Category == EquipmentCategory.RangeWeapon;
					}
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsConsumable
		{
			get
			{
				if (Definition != null)
				{
					return Definition.Category == EquipmentCategory.Utility;
				}
				return false;
			}
		}

		[JsonIgnore]
		public int MainStat
		{
			get
			{
				if (Definition.Category == EquipmentCategory.Armor)
				{
					return Defense;
				}
				return Damage;
			}
		}

		[JsonIgnore]
		public int Damage => GetDamageForLevel(Level);

		[JsonIgnore]
		public int Defense => GetDefenseForLevel(Level);

		[JsonIgnore]
		public string GenerateName => string.Join("_", Definition.ID, "stl_" + StartingLevel, "cl_" + EquipmentSurvivorClass, "lvl_" + Level, "rarity_" + Rarity, "traits_" + string.Join(",", UpgradeTraits.Select((UpgradeTraitsData x) => x.Identifier).ToList()));

		public EquipmentBreakthroughModel EquipmentBreakthrough { get; set; }

		public int BreakthroughLevel
		{
			get
			{
				if (EquipmentBreakthrough != null)
				{
					return EquipmentBreakthrough.Level;
				}
				return 0;
			}
		}

		[JsonIgnore]
		public int Hit
		{
			get
			{
				EquipmentBreakthroughModel equipmentBreakthrough = EquipmentBreakthrough;
				if (equipmentBreakthrough != null && equipmentBreakthrough.Level > 0)
				{
					return base.manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, EquipmentBreakthrough.Level).Hit;
				}
				return 0;
			}
		}

		private bool BreakthroughFlag { get; set; }

		[JsonIgnore]
		public bool CanBreakthrough
		{
			get
			{
				if (EquipmentBreakthrough == null)
				{
					if (BreakthroughFlag)
					{
						return true;
					}
					if (RarityLevel < base.manager.GameEconomyData.ConfigData.EquipmentBreakthroughsRarity)
					{
						return false;
					}
					int num = 0;
					while (UpgradeTraits != null && num < UpgradeTraits.Count)
					{
						UpgradeTraitsData upgradeTraitsData = UpgradeTraits[num];
						if (upgradeTraitsData.Identifier != "ChargeEquipment" && upgradeTraitsData.UnlockingLevel > Level)
						{
							return false;
						}
						num++;
					}
					BreakthroughFlag = true;
					return true;
				}
				int level = EquipmentBreakthrough.Level + 1;
				return base.manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, level) != null;
			}
		}

		public ActorModel GetOwnerForClient()
		{
			return _owner;
		}

		public UpgradeTraitsData GetUpgradeTraitsDataForLevel(int level)
		{
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				if (upgradeTraitsData.UnlockingLevel == level)
				{
					return upgradeTraitsData;
				}
			}
			return null;
		}

		public void AddTemporaryTrait(string traitId, TraitExpirationType type, FixedPoint multiplier)
		{
			if (!HasTemporaryTrait(traitId))
			{
				TemporaryTraits.Add(new TemporaryTraitsData(traitId, type, multiplier));
			}
		}

		public bool HasTemporaryTrait(string traitId)
		{
			for (int i = 0; i < TemporaryTraits.Count; i++)
			{
				if (TemporaryTraits[i].Identifier == traitId)
				{
					return true;
				}
			}
			return false;
		}

		public List<TemporaryTraitsData> GetTemporaryTraitsByExpirationType(TraitExpirationType type)
		{
			List<TemporaryTraitsData> list = new List<TemporaryTraitsData>();
			for (int i = 0; i < TemporaryTraits.Count; i++)
			{
				TemporaryTraitsData temporaryTraitsData = TemporaryTraits[i];
				if (temporaryTraitsData.Type == type)
				{
					list.Add(temporaryTraitsData);
				}
			}
			return list;
		}

		public void RemoveTemporaryTraitsByExpirationType(TraitExpirationType type)
		{
			List<TemporaryTraitsData> list = new List<TemporaryTraitsData>();
			for (int i = 0; i < TemporaryTraits.Count; i++)
			{
				TemporaryTraitsData temporaryTraitsData = TemporaryTraits[i];
				if (temporaryTraitsData.Type == type)
				{
					list.Add(temporaryTraitsData);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				TemporaryTraitsData item = list[j];
				TemporaryTraits.Remove(item);
			}
		}

		public int GetUpgradeCost(int fromLevel = 0)
		{
			if (fromLevel == 0)
			{
				fromLevel = Level;
			}
			EquipmentLevelDefinition equipmentLevelDefinition = base.gameEconomyData.GetEquipmentLevelDefinition(fromLevel);
			int result = 0;
			if (equipmentLevelDefinition != null)
			{
				result = equipmentLevelDefinition.UpgradeCostSurvivalPointsBase;
			}
			return result;
		}

		public int GetEquipmentBaseLevelUpgradeCost()
		{
			return base.gameEconomyData.GetEquipmentLevelDefinition(Level).UpgradeCostEquipmentUpgradeTokens;
		}

		public bool IsUpgrading()
		{
			if (TimedActionModel != null)
			{
				return TimedActionModel.IsActionUnderway();
			}
			return false;
		}

		public bool CanBeManipulated()
		{
			if (base.manager.Player.PhoneCall.LootsList != null)
			{
				ModelList<LootEntry> lootsList = base.manager.Player.PhoneCall.LootsList;
				for (int i = 0; i < lootsList.Count; i++)
				{
					if (lootsList[i] != null && lootsList[i].GeneratedSurvivor != null && lootsList[i].GeneratedSurvivor.IsEquipped(this))
					{
						return false;
					}
				}
			}
			if (base.manager.Player.Combat != null && base.manager.Player.Combat.ExtraSurvivors != null)
			{
				for (int j = 0; j < base.manager.Player.Combat.ExtraSurvivors.Count; j++)
				{
					if (base.manager.Player.Combat.ExtraSurvivors[j] is SurvivorModel { IsNotGivenToPlayer: false } survivorModel && survivorModel.IsEquipped(this))
					{
						return false;
					}
				}
			}
			return true;
		}

		public List<UpgradeTraitsData> GetAvailableTraits()
		{
			List<UpgradeTraitsData> list = new List<UpgradeTraitsData>();
			int num = 0;
			while (UpgradeTraits != null && num < UpgradeTraits.Count)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[num];
				if (upgradeTraitsData.Identifier != "ChargeEquipment" && upgradeTraitsData.UnlockingLevel <= Level)
				{
					list.Add(upgradeTraitsData);
				}
				num++;
			}
			return list;
		}

		public bool HasTrait(string id)
		{
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[i];
				if (upgradeTraitsData.Identifier != "ChargeEquipment" && upgradeTraitsData.UnlockingLevel <= Level && (upgradeTraitsData.Identifier == id || UpgradeTraitsData.StripTraitLevelIdentifier(upgradeTraitsData.Identifier) == id))
				{
					return true;
				}
			}
			return false;
		}

		public EquipmentItemModel()
		{
			StartingLevel = 1;
			Level = StartingLevel;
			RarityLevel = 0;
		}

		public EquipmentItemModel(int startingLevel, int rarityLevel)
		{
			StartingLevel = startingLevel;
			Level = StartingLevel;
			RarityLevel = rarityLevel;
		}

		public override void Start()
		{
			base.Start();
			Ability = new AbilityModel();
			Ability.SetManager(base.manager);
			Ability.DefinitionID = Definition.AbilityIdentifier;
			Ability.TotalUses = 0;
			Ability.Start();
			equipmentModifiers = new List<ModelModifier>();
			TotalUses = 0;
			RefreshModifiers();
			Ability.MaxUses = MaxUses;
			if (IsChargeEquipment)
			{
				TimedActionModel = null;
			}
			if (TimedActionModel != null)
			{
				TimedActionModel.Changed += OnTimedActionModelChanged;
				if (IsUpgrading())
				{
					TimedActionModel.SetCashier(GetUpgradeCashier(instantUpgrade: false, addInitialSurvivorPoints: false, CanUpgradeWithEquipmentUpgradeToken));
				}
			}
			if (IdForAnalytics == "0" || string.IsNullOrEmpty(IdForAnalytics))
			{
				IdForAnalytics = CreateIdForAnalytics();
			}
			if (ModSkillSlots == null || ModSkillSlots.Length == 0)
			{
				return;
			}
			ModSkillSlot[] modSkillSlots = ModSkillSlots;
			foreach (ModSkillSlot modSkillSlot in modSkillSlots)
			{
				if (modSkillSlot.ModSkillMode != null)
				{
					modSkillSlot.ModSkillMode.EquipmentItemModel = this;
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			if (!IsChargeEquipment)
			{
				TimedActionModel = new TimedActionModel();
				TimedActionModel.SetManager(base.manager);
				TimedActionModel.Initialize();
				TimedActionModel.PurchaseType = PurchaseType.SpeedUpEquipmentUpgrade;
			}
			else
			{
				TimedActionModel = null;
			}
			TemporaryTraits = new List<TemporaryTraitsData>();
			IdForAnalytics = "0";
		}

		public void InitUpgradeTraits(ModelRandom random, SurvivorClass survivorClass)
		{
			if (Definition.Category == EquipmentCategory.Utility)
			{
				return;
			}
			Dictionary<int, TraitBucketsDefinition> levelsThatUnlockATrait = base.manager.GameEconomyData.GetLevelsThatUnlockATrait(RarityLevel, UpgradeType.EquipmentUpgrade, StartingLevel, replaceTacticalWithLowLevel: false);
			UpgradeTraits = new List<UpgradeTraitsData>();
			if (Definition != null && Definition.TraitsOverride != null && Definition.TraitsOverride.Count > 0)
			{
				bool flag = false;
				int num = 0;
				foreach (KeyValuePair<int, TraitBucketsDefinition> item in levelsThatUnlockATrait)
				{
					if (num >= Definition.TraitsOverride.Count)
					{
						continue;
					}
					string text = Definition.TraitsOverride[num];
					if (item.Value.IsTactical)
					{
						text = "ChargeEquipment";
					}
					else
					{
						num++;
					}
					UpgradeTraitsData upgradeTraitsData = new UpgradeTraitsData();
					TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(text);
					if (traitDefinition == null)
					{
						base.manager.Debug.LogError("Could not find trait definition for EquipmentItemModel:" + Definition.ID + " - " + text + "," + Definition.Type.ToString() + "," + Definition.Category);
					}
					else
					{
						upgradeTraitsData.Identifier = traitDefinition.Identifier;
						upgradeTraitsData.UnlockingLevel = item.Key;
						if (IsApocalypticTrait(traitDefinition.Identifier))
						{
							upgradeTraitsData.RarityLevel = 5;
						}
						else
						{
							upgradeTraitsData.RarityLevel = item.Value.RarityLevel;
						}
						upgradeTraitsData.IsLocked = item.Value.IsLocked;
						upgradeTraitsData.IsTactical = item.Value.IsTactical;
						UpgradeTraits.Add(upgradeTraitsData);
						flag = true;
					}
				}
				if (flag)
				{
					SetupChargeEquipmentOnUpgrade(random);
					return;
				}
			}
			foreach (KeyValuePair<int, TraitBucketsDefinition> item2 in levelsThatUnlockATrait)
			{
				List<string> list = new List<string> { UpgradeType.EquipmentUpgrade.ToString() };
				if (item2.Value.IsTactical)
				{
					list.Add(TraitDefinition.TRAIT_TAG_TACTICAL);
				}
				else if (item2.Value.IsLocked)
				{
					list.Add(TraitDefinition.TRAIT_TAG_LOCKED);
				}
				else
				{
					list.Add(TraitDefinition.TRAIT_TAG_RARITY_LEVEL + item2.Value.RarityLevel);
				}
				List<string> list2 = new List<string>(new string[2]
				{
					Definition.Category.ToString(),
					Definition.Type.ToString()
				});
				List<TraitDefinition> upgradeTraits = base.manager.GameEconomyData.GetUpgradeTraits(list, list2, Level, survivorClass);
				if (upgradeTraits != null && upgradeTraits.Count > 0)
				{
					UpgradeTraitsData upgradeTraitsData2 = new UpgradeTraitsData();
					TraitDefinition traitDefinition2 = PickRandomTraitDefinition(upgradeTraits, random);
					if (traitDefinition2 == null)
					{
						base.manager.Debug.LogError("Could not find random trait definition for EquipmentItemModel:" + Definition.ID + " - " + item2.Value?.ToString() + "," + Definition.Type.ToString() + "," + Definition.Category);
					}
					else
					{
						upgradeTraitsData2.Identifier = traitDefinition2.Identifier;
						upgradeTraitsData2.UnlockingLevel = item2.Key;
						if (IsApocalypticTrait(traitDefinition2.Identifier))
						{
							upgradeTraitsData2.RarityLevel = 5;
						}
						else
						{
							upgradeTraitsData2.RarityLevel = item2.Value.RarityLevel;
						}
						upgradeTraitsData2.IsLocked = item2.Value.IsLocked;
						upgradeTraitsData2.IsTactical = item2.Value.IsTactical;
						UpgradeTraits.Add(upgradeTraitsData2);
					}
				}
				else
				{
					string text2 = "";
					for (int i = 0; i < list.Count; i++)
					{
						text2 = text2 + list[i] + ", ";
					}
					string text3 = "";
					for (int j = 0; j < list2.Count; j++)
					{
						text3 = text3 + list2[j] + ", ";
					}
					base.manager.Debug.LogError("Could not find upgrade traits for [" + Definition.ID + "] - with tags: {" + text2 + "}, ownerFilters: {" + text3 + "}, Level: " + Level);
				}
			}
			SetupChargeEquipmentOnUpgrade(random);
		}

		public static bool IsApocalypticTrait(string identifier)
		{
			int num = identifier.LastIndexOf('.') + 1;
			if (num != -1 && identifier.Substring(num).ToLower() == "Level3".ToLower())
			{
				return true;
			}
			return false;
		}

		public void InitUpgradeTraitsFromMockData(List<EquipmentTraitMockData> traitsMockData, SurvivorClass survivorClass, bool preview = false)
		{
			UpgradeTraits = new List<UpgradeTraitsData>();
			for (int i = 0; i < traitsMockData.Count; i++)
			{
				EquipmentTraitMockData equipmentTraitMockData = traitsMockData[i];
				UpgradeTraitsData upgradeTraitsData = new UpgradeTraitsData();
				TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(equipmentTraitMockData.Identifier);
				if (traitDefinition == null)
				{
					base.manager.Debug.LogError("Could not find trait definition for EquipmentItemModel:" + Definition.ID + " - " + equipmentTraitMockData.Identifier + "," + Definition.Type.ToString() + "," + Definition.Category);
					continue;
				}
				upgradeTraitsData.Identifier = traitDefinition.Identifier;
				upgradeTraitsData.UnlockingLevel = 1;
				if (IsApocalypticTrait(traitDefinition.Identifier))
				{
					upgradeTraitsData.RarityLevel = 5;
				}
				else
				{
					upgradeTraitsData.RarityLevel = equipmentTraitMockData.RarityLevel;
				}
				upgradeTraitsData.IsLocked = false;
				upgradeTraitsData.IsTactical = equipmentTraitMockData.IsTactical;
				upgradeTraitsData.ConstructionMultiplier = 0L;
				upgradeTraitsData.RemodeValues = equipmentTraitMockData.RemodeValues;
				upgradeTraitsData.ThisRemodeParamIndex[traitDefinition.Identifier] = equipmentTraitMockData.RemodeIndexs;
				UpgradeTraits.Add(upgradeTraitsData);
			}
			SetupChargeEquipmentOnUpgradeFromMockData(traitsMockData);
		}

		public override bool IsValid()
		{
			return true;
		}

		private void AttachEquipmentModifiersToAbility(List<AbilityModifierDefinition> modifiers)
		{
			if (base.manager == null || Ability == null || equipmentModifiers == null || modifiers == null || modifiers.Count <= 0)
			{
				return;
			}
			foreach (ModelModifier equipmentModifier in equipmentModifiers)
			{
				if (equipmentModifier != null)
				{
					Ability.Modifiers.RemoveModifier(equipmentModifier);
				}
			}
			equipmentModifiers.Clear();
			foreach (AbilityModifierDefinition modifier in modifiers)
			{
				ModelModifier modelModifier = ReflectionUtils.Instantiate(ReflectionUtils.FindDerivedType(typeof(ModelModifier), modifier.Type), modifier.ConstructionParameters) as ModelModifier;
				equipmentModifiers.Add(modelModifier);
				if (modelModifier != null)
				{
					Ability.Modifiers.RegisterModifier(modelModifier);
				}
			}
		}

		public bool IsAvailable()
		{
			if (MaxUses >= 0)
			{
				return TotalUses < MaxUses;
			}
			return true;
		}

		public void RefreshModifiers()
		{
			if (base.manager != null)
			{
				List<AbilityModifierDefinition> list = new List<AbilityModifierDefinition>();
				FixedPoint fixedPoint = Damage;
				AbilityModifierDefinition abilityModifierDefinition = new AbilityModifierDefinition();
				abilityModifierDefinition.Type = "AbilityModifierIncreaseFinalDamage";
				abilityModifierDefinition.ConstructionParameters = new List<string> { fixedPoint.ToString() };
				list.Add(abilityModifierDefinition);
				FixedPoint fixedPoint2 = (FixedPoint)Definition.DamageVariation / (FixedPoint)100.0;
				AbilityModifierDefinition abilityModifierDefinition2 = new AbilityModifierDefinition();
				abilityModifierDefinition2.Type = "AbilityModifierIncreaseDamageVariation";
				abilityModifierDefinition2.ConstructionParameters = new List<string> { fixedPoint2.ToString() };
				list.Add(abilityModifierDefinition2);
				MaxUses = -1;
				AttachEquipmentModifiersToAbility(list);
				if (ChargeEquipment != null)
				{
					ChargeEquipment.RefreshModifiers();
				}
			}
		}

		private void SetupChargeEquipmentOnUpgrade(ModelRandom random)
		{
			if (ChargeEquipment == null && !IsChargeEquipment && Level >= UpgradeTraits[0].UnlockingLevel && UpgradeTraits[0].Identifier == "ChargeEquipment")
			{
				base.manager.Player.Equipment.AddChargeEquipmentToTarget(this, random);
			}
		}

		private void SetupChargeEquipmentOnUpgradeFromMockData(List<EquipmentTraitMockData> traits)
		{
			if (ChargeEquipment == null && !IsChargeEquipment && Level >= UpgradeTraits[0].UnlockingLevel && UpgradeTraits[0].Identifier == "ChargeEquipment")
			{
				base.manager.Player.Equipment.AddChargeEquipmentToTargetFromMockData(this, traits);
			}
		}

		public int GetMainStatForLevel(int level)
		{
			if (Definition.Category == EquipmentCategory.Armor)
			{
				return GetDefenseForLevel(level);
			}
			return GetDamageForLevel(level);
		}

		public int GetDamageForLevel(int level)
		{
			FixedPoint fixedPoint = base.gameEconomyData.GetEquipmentLevelDefinition(level).DamageBase;
			FixedPoint fixedPoint2 = (FixedPoint)Definition.DamageMultiplier / (FixedPoint)100.0;
			FixedPoint fixedPoint3 = (FixedPoint)RarityDefinition.DamageMultiplier / (FixedPoint)100.0;
			FixedPoint fixedPoint4 = fixedPoint * (fixedPoint2 + fixedPoint3);
			EquipmentBreakthroughModel equipmentBreakthrough = EquipmentBreakthrough;
			if (equipmentBreakthrough != null && equipmentBreakthrough.Level > 0)
			{
				EquipBreakthroughDefinition equipBreakthroughDefinitionByRarityAndLevel = base.gameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, EquipmentBreakthrough.Level);
				FixedPoint fixedPoint5 = (float)equipBreakthroughDefinitionByRarityAndLevel.AttackPercentage / 100f;
				fixedPoint4 = fixedPoint4 * (1L + fixedPoint5) + equipBreakthroughDefinitionByRarityAndLevel.AttackNumber;
			}
			return (int)fixedPoint4;
		}

		public int GetDefenseForLevel(int level)
		{
			FixedPoint fixedPoint = base.gameEconomyData.GetEquipmentLevelDefinition(level).ArmorBase;
			FixedPoint fixedPoint2 = (FixedPoint)Definition.ArmorMultiplier / (FixedPoint)100.0;
			FixedPoint fixedPoint3 = (FixedPoint)RarityDefinition.ArmorMultiplier / (FixedPoint)100.0;
			FixedPoint fixedPoint4 = fixedPoint * (fixedPoint2 + fixedPoint3);
			EquipmentBreakthroughModel equipmentBreakthrough = EquipmentBreakthrough;
			if (equipmentBreakthrough != null && equipmentBreakthrough.Level > 0)
			{
				EquipBreakthroughDefinition equipBreakthroughDefinitionByRarityAndLevel = base.gameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, EquipmentBreakthrough.Level);
				FixedPoint fixedPoint5 = (float)equipBreakthroughDefinitionByRarityAndLevel.DefensePercentage / 100f;
				fixedPoint4 = fixedPoint4 * (1L + fixedPoint5) + equipBreakthroughDefinitionByRarityAndLevel.DefenseNumber;
			}
			return (int)fixedPoint4;
		}

		public void SetupForCombat()
		{
			TotalUses = 0;
			if (Ability != null)
			{
				Ability.SetupForCombat();
			}
		}

		public void TriggerUpgradedDailyQuestAction()
		{
			if (base.manager != null)
			{
				QuestVariables questVariables = base.manager.Player.DailyQuestManager.StartAction("Upgrade");
				if (IsWeaponEquipment)
				{
					questVariables.TargetType = "Weapon";
				}
				else if (Definition.Category == EquipmentCategory.Armor)
				{
					questVariables.TargetType = "Armor";
				}
				base.manager.Player.DailyQuestManager.CommitAction();
				base.manager.Player.NotifyItemUpgraded(ReturnQuestType.UpgradeEquipment);
			}
		}

		public TWDModelResult UpgradeInstant(Cashier cashier = null)
		{
			if (cashier != null && cashier.useTokensForPayment)
			{
				return TimedActionModel.StartActionInstant(GetUpgradeCashierWithTokens(), this);
			}
			if (CanUpgrade && GetUpgradeCashier(instantUpgrade: true, !IsUpgrading()).CanAfford() && TimedActionModel != null)
			{
				return TimedActionModel.StartActionInstant(GetUpgradeCashier(instantUpgrade: true, !IsUpgrading()), this);
			}
			return TWDModelResult.Error;
		}

		public TWDModelResult StartUpgrade(int useDiamondsAmount)
		{
			if ((CanUpgrade || CanUpgradeWithEquipmentUpgradeToken) && TimedActionModel != null)
			{
				Cashier upgradeCashier = GetUpgradeCashier(instantUpgrade: false, addInitialSurvivorPoints: false, CanUpgradeWithEquipmentUpgradeToken);
				upgradeCashier.UseDiamondsAmount = useDiamondsAmount;
				TWDModelResult tWDModelResult = upgradeCashier.Pay();
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
				return UpgradeInstant();
			}
			return TWDModelResult.Error;
		}

		public Cashier GetUpgradeCashier(bool instantUpgrade, bool addInitialSurvivorPoints = false, bool useEquipmentUpgradeToken = false)
		{
			Cashier cashier = new Cashier(base.manager);
			if (instantUpgrade)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.InstantEquipmentUpgrade);
				int num = base.gameEconomyData.TimeToDiamonds(UpgradeTime * 1000);
				if (addInitialSurvivorPoints)
				{
					num += base.gameEconomyData.CurrencyToDiamonds(CurrencyType.SurvivalPoints, GetUpgradeCost());
				}
				cashierItem.SetCost(CurrencyType.Diamonds, num);
				cashier.AddItem(cashierItem);
			}
			else if (useEquipmentUpgradeToken)
			{
				CashierItem cashierItem2 = new CashierItem(PurchaseType.UpgradeEquipmentLevel);
				cashierItem2.SetCost(CurrencyType.EquipmentUpgradeToken, GetEquipmentBaseLevelUpgradeCost());
				cashierItem2.SetCost(CurrencyType.SurvivalPoints, GetUpgradeCost());
				cashier.AddItem(cashierItem2);
			}
			else
			{
				CashierItem cashierItem3 = new CashierItem(PurchaseType.UpgradeEquipment);
				cashierItem3.SetCost(CurrencyType.SurvivalPoints, GetUpgradeCost());
				cashier.AddItem(cashierItem3);
			}
			return cashier;
		}

		public Cashier GetUpgradeCashierWithTokens()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.InstantEquipmentUpgrade);
			cashierItem.SetCost(CurrencyType.SuperEquipmentTokenBP, 1);
			cashier.AddItem(cashierItem);
			cashier.useTokensForPayment = true;
			return cashier;
		}

		private TraitDefinition PickRandomTraitDefinition(List<TraitDefinition> traitDefinitions, ModelRandom random)
		{
			if (traitDefinitions.Count == 0)
			{
				return null;
			}
			TraitDefinition traitDefinition = random.GetRandomElement(traitDefinitions, remove: true);
			if (traitDefinition != null && !traitDefinition.CanBeDuplicate)
			{
				bool flag = false;
				for (int i = 0; i < UpgradeTraits.Count; i++)
				{
					string text = UpgradeTraitsData.StripTraitLevelIdentifier(UpgradeTraits[i].Identifier);
					string text2 = UpgradeTraitsData.StripTraitLevelIdentifier(traitDefinition.Identifier);
					if (text.ToLower() == text2.ToLower())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					traitDefinition = PickRandomTraitDefinition(traitDefinitions, random);
				}
			}
			return traitDefinition;
		}

		private void OnTimedActionModelChanged(ModelObject m, string changed, object args)
		{
			if (changed == "ActionStartEvent")
			{
				NotifyChange("ActionStartEvent", this);
			}
			else
			{
				if (!(changed == "ActionFinishedEvent"))
				{
					return;
				}
				TimedActionModel timedActionModel = args as TimedActionModel;
				if (HasReachedMaxLevel && timedActionModel != null && timedActionModel.GetCashier().GetTotalCost(CurrencyType.EquipmentUpgradeToken) >= 0 && timedActionModel.GetCashier().GetTotalCost(CurrencyType.EquipmentUpgradeToken) == GetEquipmentBaseLevelUpgradeCost())
				{
					EquipmentUpgradeTokenLevelUpgrades++;
				}
				Level++;
				Level = Math.Min(Level, MaxLevel);
				if (ChargeEquipment != null)
				{
					ChargeEquipment.Level = Level;
				}
				Metrics.UpgradeTypes upgradeType = Metrics.UpgradeTypes.Regular;
				if (timedActionModel != null)
				{
					if (timedActionModel.WasInstant)
					{
						upgradeType = Metrics.UpgradeTypes.Instant;
					}
					else if (timedActionModel.WasSpeedUp)
					{
						upgradeType = Metrics.UpgradeTypes.SpeedUp;
					}
				}
				base.manager.Metrics.AddEnd().AddUpgrade(upgradeType).AddEquipment(this)
					.AddLevel()
					.Send();
				RefreshModifiers();
				UpgradeTraitsData upgradeTraitsDataForLevel = GetUpgradeTraitsDataForLevel(Level);
				if (upgradeTraitsDataForLevel != null && Owner != null && !Owner.HasTrait(upgradeTraitsDataForLevel.Identifier))
				{
					Owner.AddTrait(upgradeTraitsDataForLevel.Identifier);
					Owner.ConfigureBaseAttributes();
				}
				if (base.Manager.Mode == ModelManagerMode.Client && !base.manager.Player.Camp.InCamp)
				{
					base.manager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.Equipment, base.ModelId, Definition.ID, Level);
				}
				NotifyChange("ActionFinishedEvent", this);
			}
		}

		public void NullifyChargeEquipmentTimedActionModel()
		{
			if (ChargeEquipment != null && ChargeEquipment.IsChargeEquipment && ChargeEquipment.TimedActionModel != null)
			{
				ChargeEquipment.TimedActionModel = null;
			}
		}

		public bool CanBeAutoScrapped()
		{
			if (Owner == null && ((RarityLevel < 3) & !IsInWorkshopUpgrading()) && CanBeManipulated())
			{
				if (base.manager.Player.Combat != null)
				{
					return !base.manager.Player.Combat.ContainsSurvivorEquipment(this);
				}
				return true;
			}
			return false;
		}

		public bool IsInWorkshopUpgrading()
		{
			if (!(base.manager.Player.Camp.GetBuilding("Workshop") is WorkshopBuildingModel workshopBuildingModel))
			{
				return false;
			}
			EquipmentItemModel equipmentItemModel = workshopBuildingModel.UpgradedUnseenModel as EquipmentItemModel;
			if (workshopBuildingModel.UpgradingEquipment == null || workshopBuildingModel.UpgradingEquipment != this)
			{
				if (equipmentItemModel != null)
				{
					return equipmentItemModel == this;
				}
				return false;
			}
			return true;
		}

		private string CreateIdForAnalytics()
		{
			string hashedId = base.manager.Player.HashedId;
			string text = base.manager.Player.UtcTimeStamp.ToString();
			return ModelHelpers.MD5Sum(StartingLevel + ModelHelpers.GetRarityNameForAnalytics(RarityLevel) + Definition.Type.ToString() + base.ModelId + hashedId + text);
		}

		public EquipmentItemMockData CreateMockData()
		{
			EquipmentItemMockData equipmentItemMockData = new EquipmentItemMockData();
			equipmentItemMockData.EquipmentDefinitionId = EquipmentDefinitionIdentifier;
			equipmentItemMockData.RarityLevel = RarityLevel;
			equipmentItemMockData.AnalyticsId = IdForAnalytics;
			if (ModSkillSlots != null)
			{
				equipmentItemMockData.ModSkillSlots = new ModSkillSlot[ModSkillSlots.Length];
				for (int i = 0; i < ModSkillSlots.Length; i++)
				{
					ModSkillSlot modSkillSlot = ModSkillSlots[i];
					ModSkillMode modSkillMode = null;
					if (modSkillSlot.ModSkillMode != null)
					{
						modSkillMode = new ModSkillMode(modSkillSlot.ModSkillMode.ID, modSkillSlot.ModSkillMode.Type, modSkillSlot.ModSkillMode.SurvivorClass, modSkillSlot.ModSkillMode.ModSkillState, null, modSkillSlot.ModSkillMode.ModSkillLockState);
						modSkillMode.SlotIndex = modSkillSlot.ModSkillMode.SlotIndex;
					}
					equipmentItemMockData.ModSkillSlots[i] = new ModSkillSlot(modSkillSlot.Index, modSkillMode);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < UpgradeTraits.Count; j++)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[j];
				if (!upgradeTraitsData.IsLocked)
				{
					stringBuilder.Append(upgradeTraitsData.Identifier);
					if (upgradeTraitsData.RemodeValues != null && upgradeTraitsData.ThisRemodeParamIndex.TryGetValue(upgradeTraitsData.Identifier, out var value))
					{
						stringBuilder.Append("_");
						stringBuilder.Append(string.Join("|", upgradeTraitsData.RemodeValues));
						stringBuilder.Append("_");
						stringBuilder.Append(string.Join("|", value));
					}
					stringBuilder.Append(',');
				}
			}
			equipmentItemMockData.UpgradeTraitsList = stringBuilder.ToString();
			return equipmentItemMockData;
		}

		public void StartReloading(ActorModel actor)
		{
			ReloadingTimedEffect = new TimedEffect(TimedEffectType.Reloading, Ability.Definition.TurnsToReload, 0, actor.Faction);
			actor.NotifyChange("ActorReloadingStarted");
		}

		public void UpdateReloading(ActorModel actor)
		{
			if (ReloadingTimedEffect != null)
			{
				ReloadingTimedEffect.Counter++;
				if (ReloadingTimedEffect.Counter >= ReloadingTimedEffect.Duration)
				{
					FinishReloading(actor);
				}
			}
		}

		private void FinishReloading(ActorModel actor)
		{
			ReloadingTimedEffect = null;
			actor.NotifyChange("ActorReloadingFinished");
		}

		public void ResetReloading()
		{
			ReloadingTimedEffect = null;
		}

		public UpgradeTraitsData GetUpgradeTraitsData(string traitId)
		{
			return UpgradeTraits.Find((UpgradeTraitsData T) => T.Identifier == traitId);
		}

		public int GetUpgradeTraitsDataIndex(string traitId)
		{
			return UpgradeTraits.FindIndex((UpgradeTraitsData T) => T.Identifier == traitId);
		}

		public TWDModelResult EquipmentRemodel(string identifier, bool exchange)
		{
			UpgradeTraitsData upgradeTraitsData = GetUpgradeTraitsData(identifier);
			if (upgradeTraitsData == null)
			{
				return TWDModelResult.Error;
			}
			if (IsRemodeling(upgradeTraitsData))
			{
				return TWDModelResult.Error;
			}
			int upgradeTraitsDataIndex = GetUpgradeTraitsDataIndex(identifier);
			if (upgradeTraitsDataIndex == -1 || upgradeTraitsData.Identifier == "ChargeEquipment" || upgradeTraitsData.UnlockingLevel > Level)
			{
				return TWDModelResult.Error;
			}
			if (identifier != upgradeTraitsData.Identifier)
			{
				return TWDModelResult.Error;
			}
			int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(upgradeTraitsData.Identifier);
			List<string> expectTratIdList = GetExpectTratIdList(traitLevelIdentifier);
			List<EquipTraitsDefinition> equipTraitsDefinitions = base.gameEconomyData.getEquipTraitsDefinitions(Definition.SurvivorClass, Definition.Category, upgradeTraitsDataIndex, traitLevelIdentifier, expectTratIdList);
			if (equipTraitsDefinitions.Count < 2)
			{
				if (base.manager.ServerService != null)
				{
					base.manager.ServerService.SendFeiShuHook(string.Format("装备改造EquipTraits配置缺失,请策划同学检查,职业:{0},装备类型:{1},格子:{2},稀有度:{3},互斥:{4}", Definition.SurvivorClass, Definition.Category, upgradeTraitsDataIndex, traitLevelIdentifier, string.Join(",", expectTratIdList)));
				}
				return TWDModelResult.OK;
			}
			List<int> equipTraitsRemodelToken = base.gameEconomyData.ConfigData.EquipTraitsRemodelToken;
			int num = base.manager.Player.ActivityManager.GetEquipTraitsRemodelGold(base.gameEconomyData.ConfigData);
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.EquipmentRemodel);
			if (exchange)
			{
				int currencyAmount = base.manager.Player.GetCurrencyAmount(CurrencyType.EquipTraitsRemodelToken);
				if (currencyAmount <= equipTraitsRemodelToken[0])
				{
					int num2 = (equipTraitsRemodelToken[0] - currencyAmount) * equipTraitsRemodelToken[1];
					cashierItem.SetCost(CurrencyType.EquipTraitsRemodelToken, currencyAmount);
					num += num2;
				}
				else
				{
					cashierItem.SetCost(CurrencyType.EquipTraitsRemodelToken, equipTraitsRemodelToken[0]);
				}
			}
			else
			{
				if (OfflineManager.IsLoadDataManager)
				{
					if (DataManager.Instance.SurvivorManagementPopUp.remodelTraitsTree.isActiveAndEnabled)
					{
						DataManager.Instance.Player.SetCurrency(CurrencyType.EquipTraitsRemodelToken, equipTraitsRemodelToken[0]);
					}
				}
				cashierItem.SetCost(CurrencyType.EquipTraitsRemodelToken, equipTraitsRemodelToken[0]);
			}
			cashierItem.SetCost(CurrencyType.Diamonds, num);
			cashier.AddItem(cashierItem);
			TWDModelResult tWDModelResult = cashier.Pay(upgradeTraitsData);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			base.manager.Metrics.ResourceChangeUsedReason = "traitremodel";
			base.manager.Metrics.AddItemChange().AddResources(cashier).Send();
			if (upgradeTraitsData.RemodeValues == null)
			{
				upgradeTraitsData.ThisRemodeValues = new Dictionary<string, List<int>>();
				upgradeTraitsData.ThisRemodeParamIndex = new Dictionary<string, List<int>>();
			}
			List<EquipTraitsDefinition> list = base.manager.Player.PlayerRandom.WeightedRandomList(equipTraitsDefinitions, 2, (EquipTraitsDefinition x) => 1L, isRepeat: false);
			foreach (EquipTraitsDefinition item in list)
			{
				if (!upgradeTraitsData.ThisRemodeValues.TryGetValue(item.TraitsGroup, out var value))
				{
					value = item.MinConstructionParameters;
				}
				List<int> list2 = new List<int>();
				for (int num3 = 0; num3 < value.Count; num3++)
				{
					list2.Add(base.manager.Player.PlayerRandom.GetRandomInRange(value[num3], item.MaxConstructionParameters[num3]));
				}
				upgradeTraitsData.ThisRemodeValues[item.TraitsGroup] = list2;
				upgradeTraitsData.ThisRemodeParamIndex[item.TraitsGroup] = item.ConstructionParametersNumber;
			}
			upgradeTraitsData.ThisRemodeIds = list.Select((EquipTraitsDefinition x) => x.TraitsGroup).ToList();
			upgradeTraitsData.RemodelIng = true;
			return TWDModelResult.OK;
		}

		private bool IsRemodeling(UpgradeTraitsData upgradeTraitsData)
		{
			if (upgradeTraitsData == null)
			{
				return true;
			}
			int equipmentRemodelRarity = base.gameEconomyData.ConfigData.EquipmentRemodelRarity;
			if (RarityLevel < equipmentRemodelRarity || upgradeTraitsData.UnlockingLevel > Level)
			{
				return true;
			}
			return false;
		}

		public TWDModelResult SelectRemodeId(string identifier, int selectIndex)
		{
			UpgradeTraitsData upgradeTraitsData = GetUpgradeTraitsData(identifier);
			List<string> list = new List<string>();
			for (int i = 1; i < UpgradeTraits.Count; i++)
			{
				list.Add(UpgradeTraits[i].Identifier);
			}
			if (identifier != upgradeTraitsData.Identifier)
			{
				return TWDModelResult.Error;
			}
			string identifier2 = upgradeTraitsData.Identifier;
			if (upgradeTraitsData.ThisRemodeIds == null)
			{
				return TWDModelResult.Error;
			}
			if (selectIndex == 2)
			{
				upgradeTraitsData.ThisRemodeIds.Clear();
				upgradeTraitsData.RemodelIng = false;
				upgradeTraitsData.RemodelEd = true;
				return TWDModelResult.OK;
			}
			if (Owner != null)
			{
				TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
				if (traitDefinition != null && traitDefinition.HasTag("EquipmentPassive"))
				{
					Owner.RemoveTrait(upgradeTraitsData.Identifier);
				}
			}
			upgradeTraitsData.Identifier = upgradeTraitsData.ThisRemodeIds[selectIndex];
			string identifier3 = upgradeTraitsData.Identifier;
			if (upgradeTraitsData.ThisRemodeValues.TryGetValue(upgradeTraitsData.Identifier, out var value) && upgradeTraitsData.ThisRemodeParamIndex.TryGetValue(upgradeTraitsData.Identifier, out var _))
			{
				upgradeTraitsData.RemodeValues = value;
			}
			if (Owner != null)
			{
				TraitDefinition traitDefinition2 = base.manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
				if (traitDefinition2 != null && traitDefinition2.HasTag("EquipmentPassive"))
				{
					ActorModel owner = Owner;
					string identifier4 = upgradeTraitsData.Identifier;
					FixedPoint constructionMultiplier = upgradeTraitsData.ConstructionMultiplier;
					List<int> remodeValues = upgradeTraitsData.RemodeValues;
					List<int> remodeIndex = upgradeTraitsData.ThisRemodeParamIndex[upgradeTraitsData.Identifier];
					owner.AddTrait(identifier4, constructionMultiplier, doNotInstantiateTrait: false, null, "", remodeIndex, remodeValues);
				}
			}
			upgradeTraitsData.ThisRemodeIds.Clear();
			upgradeTraitsData.RemodelIng = false;
			upgradeTraitsData.RemodelEd = true;
			base.manager.TdMetrics.SetEventType("trait_remodel").AddProperty("equipment_id", EquipmentDefinitionIdentifier).AddProperty("trait_before", identifier2)
				.AddProperty("trait_after", identifier3)
				.AddProperty("equipment_alltraits", list)
				.Send();
			return TWDModelResult.OK;
		}

		private List<string> GetExpectTratIdList(int level)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < UpgradeTraits.Count; i++)
			{
				int num = UpgradeTraits[i].Identifier.LastIndexOf('.');
				if (num != -1)
				{
					string item = UpgradeTraits[i].Identifier.Substring(0, num) + ".Level" + level;
					list.Add(item);
				}
				EquipTraitsMutualExclusion equipTraitsMutualExclusion = base.gameEconomyData.getEquipTraitsMutualExclusion(UpgradeTraits[i].Identifier);
				if (equipTraitsMutualExclusion != null)
				{
					list.AddRange(equipTraitsMutualExclusion.MutualExclusionTraits);
				}
			}
			return list.Distinct().ToList();
		}

		public TWDModelResult BreakthroughLevelUp(List<string> consumeEquipTokenIdList, int consumeApocalypticEquipTokenAmount)
		{
			if (!CanBreakthrough)
			{
				return TWDModelResult.Error;
			}
			if (EquipmentBreakthrough == null)
			{
				EquipmentBreakthrough = new EquipmentBreakthroughModel();
				EquipmentBreakthrough.SetLevel(0);
				EquipmentBreakthrough.SetManager(base.manager);
				EquipmentBreakthrough.Start();
			}
			List<string> equipmentPassiveTraits = GetEquipmentPassiveTraits();
			List<EquipTokenItemModel> list = new List<EquipTokenItemModel>();
			foreach (string consumeEquipTokenId in consumeEquipTokenIdList)
			{
				EquipTokenItemModel equipTokenItemModel = base.manager.Player.EquipTokenContainer.EquipTokenItems.Find((EquipTokenItemModel x) => x.EquipTokenId == consumeEquipTokenId);
				if (equipTokenItemModel == null)
				{
					return TWDModelResult.Error;
				}
				list.Add(equipTokenItemModel);
			}
			TWDModelResult tWDModelResult = ((!Definition.SwitchRemoldMode) ? EquipmentBreakthrough.BreakthroughLevelUp(EquipmentDefinitionIdentifier, RarityLevel, list, consumeApocalypticEquipTokenAmount) : EquipmentBreakthrough.BreakthroughRemoldLevelUp(EquipmentDefinitionIdentifier, RarityLevel, consumeApocalypticEquipTokenAmount, Definition.SurvivorClass));
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			EquipBreakthroughDefinition equipBreakthroughDefinition = ((!Definition.SwitchRemoldMode) ? base.manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, EquipmentBreakthrough.Level) : base.manager.GameEconomyData.GetRemoldEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, EquipmentBreakthrough.Level));
			if (equipBreakthroughDefinition == null)
			{
				return TWDModelResult.Error;
			}
			int[] overrideTraitsLevels = new int[4] { equipBreakthroughDefinition.Traits1QualityLevel, equipBreakthroughDefinition.Traits2QualityLevel, equipBreakthroughDefinition.Traits3QualityLevel, equipBreakthroughDefinition.Traits4QualityLevel };
			if (!UpgradeTraitsLevelByBreakthrough(overrideTraitsLevels))
			{
				return TWDModelResult.Error;
			}
			if (Owner != null)
			{
				for (int num = 0; num < equipmentPassiveTraits.Count; num++)
				{
					string traitIdentifier = equipmentPassiveTraits[num];
					if (base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier).IsApocalypticTrait)
					{
						Owner.RemoveTrait(traitIdentifier);
						string traitIdentifier2 = UpgradeTraitsData.CompileUpgradeTraitIdentifier(UpgradeTraitsData.StripTraitLevelIdentifier(traitIdentifier), equipBreakthroughDefinition.ApocalypticTraitLevel, isLocked: false);
						Owner.AddTrait(traitIdentifier2);
					}
				}
			}
			RefreshModifiers();
			return TWDModelResult.OK;
		}

		public bool UpgradeTraitsLevelByBreakthrough(int[] overrideTraitsLevels)
		{
			int num = 0;
			if (!EquipmentBreakthrough.UnlockedRandomTrait && overrideTraitsLevels[3] > -1 && !AddRandomBreakthroughTrait(overrideTraitsLevels[3]))
			{
				return false;
			}
			int num2 = 0;
			while (UpgradeTraits != null && num2 < UpgradeTraits.Count)
			{
				UpgradeTraitsData upgradeTraitsData = UpgradeTraits[num2];
				if (!(upgradeTraitsData.Identifier == "ChargeEquipment"))
				{
					int num3 = overrideTraitsLevels[num];
					string text = UpgradeTraitsData.CompileUpgradeTraitIdentifier(upgradeTraitsData.Identifier, num3, isLocked: false);
					if (text != upgradeTraitsData.Identifier)
					{
						if (Owner != null)
						{
							TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
							if (traitDefinition != null && traitDefinition.HasTag("EquipmentPassive"))
							{
								Owner.RemoveTrait(upgradeTraitsData.Identifier);
							}
						}
						upgradeTraitsData.Identifier = text;
						upgradeTraitsData.RarityLevel = GetEquipTraitRarityByTraitLevel(num3);
						upgradeTraitsData.ResetRemodel();
						if (Owner != null)
						{
							TraitDefinition traitDefinition2 = base.manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
							if (traitDefinition2 != null && traitDefinition2.HasTag("EquipmentPassive"))
							{
								Owner.AddTrait(upgradeTraitsData.Identifier, upgradeTraitsData.ConstructionMultiplier);
							}
						}
					}
					num++;
				}
				num2++;
			}
			return true;
		}

		private bool AddRandomBreakthroughTrait(int traitLevel)
		{
			EquipBreakthroughTrait[] equipBreakthroughTraitsBySurvivolClassAndEquipmentCategory = base.manager.GameEconomyData.GetEquipBreakthroughTraitsBySurvivolClassAndEquipmentCategory(Definition.SurvivorClass, Definition.Category);
			if (equipBreakthroughTraitsBySurvivolClassAndEquipmentCategory.Length == 0)
			{
				base.manager.Debug.LogError("Could not find BreakthroughTraitsDefinition for EquipmentItemModel: " + Definition.ID + ", SurvivorClass: " + Definition.SurvivorClass.ToString() + ", Category: " + Definition.Category);
				return false;
			}
			List<EquipBreakthroughTrait> list = new List<EquipBreakthroughTrait>();
			EquipBreakthroughTrait[] array = equipBreakthroughTraitsBySurvivolClassAndEquipmentCategory;
			foreach (EquipBreakthroughTrait equipBreakthroughTrait in array)
			{
				if (!UpgradeTraits.Exists((UpgradeTraitsData x) => UpgradeTraitsData.StripTraitLevelIdentifier(x.Identifier).ToLower() == equipBreakthroughTrait.TraitsGroup.ToLower()))
				{
					list.Add(equipBreakthroughTrait);
				}
			}
			if (list.Count <= 0)
			{
				base.manager.Debug.LogError("Could not find unique BreakthroughTraitsDefinition for EquipmentItemModel: " + Definition.ID + ", SurvivorClass: " + Definition.SurvivorClass.ToString() + ", Category: " + Definition.Category);
				return false;
			}
			EquipBreakthroughTrait randomElement = base.manager.Player.PlayerRandom.GetRandomElement(list.ToArray());
			UpgradeTraitsData upgradeTraitsData = new UpgradeTraitsData();
			upgradeTraitsData.Identifier = UpgradeTraitsData.CompileUpgradeTraitIdentifier(randomElement.TraitsGroup, traitLevel, isLocked: false);
			upgradeTraitsData.UnlockingLevel = Level;
			upgradeTraitsData.RarityLevel = GetEquipTraitRarityByTraitLevel(traitLevel);
			upgradeTraitsData.IsLocked = false;
			upgradeTraitsData.IsTactical = false;
			upgradeTraitsData.IsBreakthroughUnlockTrait = true;
			upgradeTraitsData.ConstructionMultiplier = 0L;
			UpgradeTraits.Add(upgradeTraitsData);
			if (Owner != null)
			{
				TraitDefinition traitDefinition = base.manager.GameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
				if (traitDefinition != null && traitDefinition.HasTag("EquipmentPassive"))
				{
					Owner.AddTrait(upgradeTraitsData.Identifier, upgradeTraitsData.ConstructionMultiplier);
				}
			}
			EquipmentBreakthrough.UnlockRandomTrait();
			return true;
		}

		public bool IsLevelBreakThrough(int level)
		{
			bool result = false;
			if (EquipmentBreakthrough != null && EquipmentBreakthrough.Level >= level)
			{
				result = true;
			}
			return result;
		}

		public List<EquipTokenItemModel> GetBreakthroughConsumables()
		{
			List<EquipTokenItemModel> result = new List<EquipTokenItemModel>();
			int level = BreakthroughLevel + 1;
			EquipBreakthroughDefinition nextLevelBreakthroughDefinition = base.manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, level);
			if (nextLevelBreakthroughDefinition == null)
			{
				return result;
			}
			EquipTokenDefinition equipTokenDefinition = base.manager.GameEconomyData.GetEquipTokenDefinitionByRelateEquipId(EquipmentDefinitionIdentifier);
			if (equipTokenDefinition == null)
			{
				return result;
			}
			if (base.manager.Player.EquipTokenContainer.EquipTokenItems == null)
			{
				return result;
			}
			if (nextLevelBreakthroughDefinition.WeaponDrawingType == WeaponDrawingType.SameClassWeapon)
			{
				result = base.manager.Player.EquipTokenContainer.EquipTokenItems.Models.Where((EquipTokenItemModel x) => x.Definition.Category == equipTokenDefinition.Category && x.Definition.SurvivorClass == equipTokenDefinition.SurvivorClass && x.Definition.Star == nextLevelBreakthroughDefinition.NeedTokenStar).ToList();
			}
			else if (nextLevelBreakthroughDefinition.WeaponDrawingType == WeaponDrawingType.SameNameWeapon)
			{
				result = base.manager.Player.EquipTokenContainer.EquipTokenItems.Models.Where((EquipTokenItemModel x) => x.Definition.Category == equipTokenDefinition.Category && x.Definition.EquipmentBreakthroughsType == equipTokenDefinition.EquipmentBreakthroughsType && x.Definition.Star == nextLevelBreakthroughDefinition.NeedTokenStar).ToList();
			}
			return result;
		}

		public int GetMaxBreakThroughLevel()
		{
			EquipBreakthroughDefinition[] equipBreakthroughDefinitions = base.gameEconomyData.EquipBreakthroughDefinitions;
			int num = 0;
			for (int i = 0; i < equipBreakthroughDefinitions.Length; i++)
			{
				if (num <= equipBreakthroughDefinitions[i].Level)
				{
					num = equipBreakthroughDefinitions[i].Level;
				}
			}
			return num;
		}

		private void OnCouncilBuildingChange(ModelObject m, string changed, object args)
		{
			if (changed == "level" && m is BuildingModel buildingModel)
			{
				_ = buildingModel.TypeName == "Council";
			}
		}

		public void ApplyModSkillPassiveTraitsToOwner()
		{
			if (Owner == null)
			{
				return;
			}
			List<TraitDefinition> passiveTraits = GetPassiveTraits();
			if (passiveTraits == null || passiveTraits.Count == 0)
			{
				return;
			}
			foreach (TraitDefinition item in passiveTraits)
			{
				if (item != null && !string.IsNullOrEmpty(item.Identifier) && !Owner.HasTrait(item.Identifier))
				{
					Owner.AddTrait(item.Identifier);
				}
			}
		}

		public void RemoveModSkillPassiveTraits()
		{
			if (Owner == null)
			{
				return;
			}
			List<TraitDefinition> passiveTraits = GetPassiveTraits();
			if (passiveTraits == null || passiveTraits.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (TraitDefinition item in passiveTraits)
			{
				if (item != null && !string.IsNullOrEmpty(item.Identifier))
				{
					list.Add(item.Identifier);
				}
			}
			foreach (string item2 in list)
			{
				if (Owner.HasTrait(item2))
				{
					Owner.RemoveTrait(item2);
				}
			}
		}

		public void ApplyModSkillPassiveTraitToOwner(string traitid)
		{
			if (Owner != null && !Owner.HasTrait(traitid))
			{
				Owner.AddTrait(traitid);
			}
		}

		public void RemoveModSkillPassiveTrait(string traitid)
		{
			if (Owner != null && Owner.HasTrait(traitid))
			{
				Owner.RemoveTrait(traitid);
			}
		}

		public int GetBreakThroughWeaponFragmentsNumber()
		{
			if (BreakthroughLevel >= GetMaxBreakThroughLevel())
			{
				return 0;
			}
			int result = 0;
			EquipBreakthroughDefinition[] equipBreakthroughDefinitions = base.gameEconomyData.EquipBreakthroughDefinitions;
			for (int i = 0; i < equipBreakthroughDefinitions.Length; i++)
			{
				string text = (Definition.SwitchRemoldMode ? "Remold" : "Normal");
				if (BreakthroughLevel + 1 == equipBreakthroughDefinitions[i].Level && text.ToLower() == equipBreakthroughDefinitions[i].WeaponMode.ToLower())
				{
					return equipBreakthroughDefinitions[i].WeaponFragmentsNumber;
				}
			}
			return result;
		}

		public int GetBreakThroughWeaponApocalypticNumber()
		{
			if (BreakthroughLevel >= GetMaxBreakThroughLevel())
			{
				return 0;
			}
			int result = 0;
			EquipBreakthroughDefinition[] equipBreakthroughDefinitions = base.gameEconomyData.EquipBreakthroughDefinitions;
			for (int i = 0; i < equipBreakthroughDefinitions.Length; i++)
			{
				string text = (Definition.SwitchRemoldMode ? "Remold" : "Normal");
				if (BreakthroughLevel + 1 == equipBreakthroughDefinitions[i].Level && text.ToLower() == equipBreakthroughDefinitions[i].WeaponMode.ToLower())
				{
					if (text.ToLower() == "remold")
					{
						return equipBreakthroughDefinitions[i].CommonBluePrintCost;
					}
					return equipBreakthroughDefinitions[i].WeaponDrawingNumber;
				}
			}
			return result;
		}

		public List<string> GetEquipmentActiveTraits()
		{
			if (Definition == null)
			{
				return null;
			}
			List<string> activeTraits = Definition.ActiveTraits;
			if (BreakthroughLevel <= 0)
			{
				return activeTraits;
			}
			if (activeTraits == null || activeTraits.Count == 0)
			{
				return activeTraits;
			}
			EquipBreakthroughDefinition equipBreakthroughDefinition = ((!Definition.SwitchRemoldMode) ? base.manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, BreakthroughLevel) : base.manager.GameEconomyData.GetRemoldEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, BreakthroughLevel));
			if (equipBreakthroughDefinition == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < activeTraits.Count; i++)
			{
				string text = activeTraits[i];
				if (base.manager.GameEconomyData.GetTraitDefinition(text).IsApocalypticTrait)
				{
					int apocalypticTraitLevel = equipBreakthroughDefinition.ApocalypticTraitLevel;
					text = UpgradeTraitsData.CompileUpgradeTraitIdentifier(text, apocalypticTraitLevel, isLocked: false);
				}
				list.Add(text);
			}
			return list;
		}

		public List<string> GetEquipmentPassiveTraits()
		{
			if (Definition == null)
			{
				return null;
			}
			List<string> list = Definition.PassiveTraits ?? new List<string>();
			if (BreakthroughLevel <= 0)
			{
				return list;
			}
			if (list == null || list.Count == 0)
			{
				return list;
			}
			EquipBreakthroughDefinition equipBreakthroughDefinition = ((!Definition.SwitchRemoldMode) ? base.manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, BreakthroughLevel) : base.manager.GameEconomyData.GetRemoldEquipBreakthroughDefinitionByRarityAndLevel(RarityLevel, BreakthroughLevel));
			if (equipBreakthroughDefinition == null)
			{
				return null;
			}
			List<string> list2 = new List<string>();
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i];
				if (base.manager.GameEconomyData.GetTraitDefinition(text).IsApocalypticTrait)
				{
					int apocalypticTraitLevel = equipBreakthroughDefinition.ApocalypticTraitLevel;
					text = UpgradeTraitsData.CompileUpgradeTraitIdentifier(text, apocalypticTraitLevel, isLocked: false);
				}
				list2.Add(text);
			}
			return list2;
		}

		public UpgradeTraitsData GetBreakThroughUpgradeTraitsData()
		{
			if (UpgradeTraits == null || UpgradeTraits.Count <= 0)
			{
				return null;
			}
			if (OfflineManager.IsLoadDataManager && UpgradeTraits.Count > 4)
			{
				return UpgradeTraits.Last();
			}
			else
			{
				return UpgradeTraits.Find((UpgradeTraitsData t) => t.IsBreakthroughUnlockTrait);
			}
		}

		public int GetBreakThroughUpgradeNeedBTlevel()
		{
			EquipBreakthroughDefinition[] equipBreakthroughDefinitions = base.gameEconomyData.EquipBreakthroughDefinitions;
			int num = int.MaxValue;
			for (int i = 0; i < equipBreakthroughDefinitions.Length; i++)
			{
				if (equipBreakthroughDefinitions[i].Traits4QualityLevel >= 0 && equipBreakthroughDefinitions[i].Level <= num)
				{
					num = equipBreakthroughDefinitions[i].Level;
				}
			}
			return num;
		}

		private int GetEquipTraitRarityByTraitLevel(int level)
		{
			return level switch
			{
				0 => 0,
				1 => 1,
				2 => 2,
				3 => 5,
				_ => 0,
			};
		}

		public bool SetModSkillSlot(int slotIndex, ModSkillMode modSkillMode)
		{
			if (ModSkillSlots == null)
			{
				ModSkillSlots = new ModSkillSlot[Definition.RemoldTraitsSlotCount];
				for (int i = 0; i < Definition.RemoldTraitsSlotCount; i++)
				{
					ModSkillSlots[i] = new ModSkillSlot(i);
				}
			}
			if (slotIndex < 0 || slotIndex >= Definition.RemoldTraitsSlotCount)
			{
				return false;
			}
			if (modSkillMode == null)
			{
				return false;
			}
			if (modSkillMode.SlotIndex >= 0 && modSkillMode.SlotIndex < ModSkillSlots.Length && ModSkillSlots[modSkillMode.SlotIndex].ModSkillMode != null)
			{
				ModSkillSlots[modSkillMode.SlotIndex].ModSkillMode = null;
			}
			if (ModSkillSlots[slotIndex].ModSkillMode != null)
			{
				ModSkillSlots[slotIndex].ModSkillMode.SlotIndex = -1;
			}
			ModSkillSlots[slotIndex].ModSkillMode = modSkillMode;
			modSkillMode.SlotIndex = slotIndex;
			return true;
		}

		public bool RemoveModSkillSlot(int slotIndex)
		{
			if (ModSkillSlots == null)
			{
				return false;
			}
			ModSkillSlots[slotIndex].ModSkillMode = null;
			return true;
		}

		public List<TraitDefinition> GetPassiveTraits()
		{
			List<TraitDefinition> list = new List<TraitDefinition>();
			if (base.manager == null || base.manager.Player == null)
			{
				return list;
			}
			if (ModSkillSlots == null || ModSkillSlots.Length == 0 || base.gameEconomyData == null)
			{
				return list;
			}
			for (int i = 0; i < ModSkillSlots.Length; i++)
			{
				if (ModSkillSlots[i].ModSkillMode == null)
				{
					continue;
				}
				ModSkillMode modSkillMode = ModSkillSlots[i].ModSkillMode;
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillMode.ID);
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
			List<TraitDefinition> list = new List<TraitDefinition>();
			if (base.manager == null || base.manager.Player == null)
			{
				return list;
			}
			if (ModSkillSlots == null || ModSkillSlots.Length == 0 || base.gameEconomyData == null)
			{
				return list;
			}
			for (int i = 0; i < ModSkillSlots.Length; i++)
			{
				if (ModSkillSlots[i].ModSkillMode == null)
				{
					continue;
				}
				ModSkillMode modSkillMode = ModSkillSlots[i].ModSkillMode;
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillMode.ID);
				if (sPTraitsRemodeDefinition == null || sPTraitsRemodeDefinition.ActiveTraits == null)
				{
					continue;
				}
				foreach (string activeTrait in sPTraitsRemodeDefinition.ActiveTraits)
				{
					TraitDefinition traitDefinition = base.gameEconomyData.GetTraitDefinition(activeTrait);
					if (traitDefinition != null)
					{
						list.Add(traitDefinition);
					}
				}
			}
			return list;
		}

		public List<TraitDefinition> GetChargeActiveTraits()
		{
			List<TraitDefinition> list = new List<TraitDefinition>();
			if (base.manager == null || base.manager.Player == null)
			{
				return list;
			}
			if (ModSkillSlots == null || ModSkillSlots.Length == 0 || base.gameEconomyData == null)
			{
				return list;
			}
			for (int i = 0; i < ModSkillSlots.Length; i++)
			{
				if (ModSkillSlots[i].ModSkillMode == null)
				{
					continue;
				}
				ModSkillMode modSkillMode = ModSkillSlots[i].ModSkillMode;
				SPTraitsRemoldDefinitions sPTraitsRemodeDefinition = base.gameEconomyData.GetSPTraitsRemodeDefinition(modSkillMode.ID);
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

		public Dictionary<CurrencyType, int> GetScrapSpTokenReward(EquipBreakthroughDefinition equipBreakthroughDefinition, bool isCharge = false)
		{
			if (Definition?.ScrapSPTokenPackage == null || Definition.ScrapSPTokenPackage.Count == 0)
			{
				return null;
			}
			List<string> scrapSPTokenPackage = Definition.ScrapSPTokenPackage;
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			for (int i = 0; i < scrapSPTokenPackage.Count; i++)
			{
				string[] array = scrapSPTokenPackage[i].Split(':');
				if (array.Length < 3)
				{
					continue;
				}
				string packageId = array[0];
				if (!int.TryParse(array[1], out var result) || !int.TryParse(array[2], out var result2))
				{
					continue;
				}
				List<EquipmentScrapSPTokenPackage> list = RollScrapSpTokenPackage(packageId, result, isCharge);
				if (list == null || list.Count <= 0)
				{
					continue;
				}
				for (int j = 0; j < list.Count; j++)
				{
					EquipmentScrapSPTokenPackage equipmentScrapSPTokenPackage = list[j];
					int num = result2 * equipBreakthroughDefinition.ScrapSkillToken;
					if (dictionary.TryGetValue(equipmentScrapSPTokenPackage.TokenID, out var value))
					{
						dictionary[equipmentScrapSPTokenPackage.TokenID] = value + num;
					}
					else
					{
						dictionary[equipmentScrapSPTokenPackage.TokenID] = num;
					}
				}
			}
			return dictionary;
		}

		public List<EquipmentScrapSPTokenPackage> RollScrapSpTokenPackage(string packageId, int rollCount, bool isCharge = false)
		{
			if (base.manager != null && base.manager.Player != null && base.manager.Player.gameEconomyData != null && base.manager.GameEconomyData.EquipmentScrapSPTokenPackagesByPackageId.TryGetValue(packageId, out var value))
			{
				List<EquipmentScrapSPTokenPackage> list = null;
				if (isCharge)
				{
					return new ModelRandom().WeightedRandomList(value, rollCount, (EquipmentScrapSPTokenPackage x) => x.Weight, isRepeat: false);
				}
				return base.manager.Player.PlayerRandom.WeightedRandomList(value, rollCount, (EquipmentScrapSPTokenPackage x) => x.Weight, isRepeat: false);
			}
			return null;
		}

		public bool ResetModSkillSlots()
		{
			if (ModSkillSlots != null && ModSkillSlots.Length != 0)
			{
				for (int i = 0; i < ModSkillSlots.Length; i++)
				{
					if (ModSkillSlots[i].ModSkillMode != null)
					{
						base.manager.Player.ModSkillManager.ResetModSkill(ModSkillSlots[i].ModSkillMode);
						ModSkillSlots[i].Reset();
					}
				}
			}
			return false;
		}

		public ModSkillMode GetModSkillSlotByIndex(int index)
		{
			if (ModSkillSlots != null && ModSkillSlots.Length != 0)
			{
				for (int i = 0; i < ModSkillSlots.Length; i++)
				{
					if (ModSkillSlots[i].Index == index)
					{
						return ModSkillSlots[i].ModSkillMode;
					}
				}
			}
			return null;
		}
	}
}
