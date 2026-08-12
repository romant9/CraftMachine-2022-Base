using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using BaseModel.ContentTypes;
using Newtonsoft.Json;

namespace TWDModel
{
	public class BuildingModel : CampObjectModel
	{
		public const string EventRemoveBuilding = "RemoveBuilding";

		public const string CouncilTypeName = "Council";

		public const string MissionCarTypeName = "MissionCar";

		public const string TentTypeName = "Tents";

		public const string WorkshopTypeName = "Workshop";

		public const string MedicTentTypeName = "MedicTent";

		public const string GraveyardTypeName = "Graveyard";

		public const string RadioTentTypeName = "RadioTent";

		public const string TrainingGroundTypeName = "TrainingGround";

		public const string BuildingBuffCriticalChanceTypeName = "BuffBuildingCriticalChance";

		public const string BuildingBuffDamageTypeName = "BuffBuildingDamage";

		public const string BuildingBuffHealthTypeName = "BuffBuildingHealth";

		public const string BuildingProduceSuppliesTypeName = "BuildingProduceSupplies";

		public const string BuildingProduceGasTypeName = "BuildingProduceGas";

		public const string BuildingStorageSuppliesTypeName = "BuildingStorageSupplies";

		public const string BuildingStorageGasTypeName = "BuildingStorageGas";

		public const string BuildingProduceFoodBoosterTypeName = "BuildingProduceFoodBooster";

		public const string BuildingOutpost = "Outpost";

		public const string BuildingCage = "Cage";

		public const string BuildingScavenger = "Scavenger";

		public const string BuildingResidence = "Residence";

		public const string BuildingArmory = "Armory";

		private BuildingType buildingType;

		[JsonIgnore]
		private int level = -1;

		private bool pendingProductionInitialization = true;

		public string TypeName { get; protected set; }

		public int TypeIndex { get; set; }

		[JsonIgnore]
		public BuildingType BuildingType
		{
			get
			{
				if (buildingType == null)
				{
					buildingType = base.manager.GameEconomyData.GetBuildingType(TypeName);
					if (buildingType == null)
					{
						base.Debug.LogError("Could not find building type \"" + TypeName + "\"!");
					}
				}
				return buildingType;
			}
		}

		public FixedVec2 Position { get; protected set; }

		[JsonIgnore]
		public int Level
		{
			get
			{
				if (level == -1)
				{
					level = base.manager.Blackboard.GetCounter(GetBuildingLevelBlackboardKey(TypeName, TypeIndex));
				}
				return level;
			}
			protected set
			{
				level = value;
				base.manager.Blackboard.SetCounter(GetBuildingLevelBlackboardKey(TypeName, TypeIndex), value);
			}
		}

		public int RepairDependencyLevelUpgrade { get; set; }

		public float RotationAngle { get; protected set; }

		public long UpgradeTimer { get; protected set; }

		public long OriginalUpgradeTimer { get; protected set; }

		[JsonIgnore]
		public int MaxUpgradeLevel => base.gameEconomyData.GetMaximumUpgradeLevel(BuildingType.Name);

		[JsonIgnore]
		public bool BuildingRepaired => Level > 0;

		[JsonIgnore]
		public bool IsMoveable
		{
			get
			{
				if (BuildingType.CanMove)
				{
					return BuildingRepaired;
				}
				return false;
			}
		}

		public bool CampMoved { get; set; }

		public bool SpeedUpByAd { get; set; }

		public int AddedAtCampLevel { get; set; }

		public bool MarkedToBeDeleted { get; private set; }

		public ProducerModel Producer { get; protected set; }

		[JsonIgnore]
		public bool CanCollect
		{
			get
			{
				if (Producer == null)
				{
					return false;
				}
				if (Producer.Amount == 0)
				{
					return false;
				}
				CurrencyModel currency = base.manager.Player.GetCurrency(Producer.CurrencyType);
				return currency.Value < currency.Max;
			}
		}

		public ModelList<StorageModel> Storages { get; protected set; }

		[JsonIgnore]
		public bool CanUpgrade
		{
			get
			{
				if (Level == 0)
				{
					if (HasDepencyLevelToUpgrade)
					{
						return HasRequiredBuilding;
					}
					return false;
				}
				if (Level < base.gameEconomyData.GetMaximumUpgradeLevel(BuildingType.Name) && !BuildingType.DisableUpgrade && !HasReachedMaxUpgradeLevel && HasDepencyLevelToUpgrade)
				{
					return HasPlayerLevelToUpgrade;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasRequiredBuilding
		{
			get
			{
				if (string.IsNullOrEmpty(BuildingType.RequiredBuilding))
				{
					return true;
				}
				return base.Camp.GetBuildingLevel(BuildingType.RequiredBuilding) > 0;
			}
		}

		[JsonIgnore]
		public bool HasDepencyLevelToUpgrade
		{
			get
			{
				if (HasReachedMaxUpgradeLevel)
				{
					return true;
				}
				int buildingDependencyLevel = base.Camp.GetBuildingDependencyLevel();
				if (Level == 0 && RepairDependencyLevelUpgrade != 0)
				{
					return buildingDependencyLevel >= RepairDependencyLevelUpgrade;
				}
				return buildingDependencyLevel >= GetNextUpgradeLevel().DependencyLevelRequired;
			}
		}

		[JsonIgnore]
		public bool HasPlayerLevelToUpgrade
		{
			get
			{
				if (HasReachedMaxUpgradeLevel)
				{
					return true;
				}
				return base.manager.Player.Level >= GetNextUpgradeLevel().PlayerLevelRequired;
			}
		}

		[JsonIgnore]
		public int DependencyLevelRequiredToUpgrade
		{
			get
			{
				if (HasReachedMaxUpgradeLevel)
				{
					return 0;
				}
				if (TypeName == base.gameEconomyData.ConfigData.DependencyLevelBuilding)
				{
					return 0;
				}
				if (Level == 0 && RepairDependencyLevelUpgrade != 0)
				{
					return RepairDependencyLevelUpgrade;
				}
				return GetNextUpgradeLevel().DependencyLevelRequired;
			}
		}

		[JsonIgnore]
		public bool HasReachedMaxUpgradeLevel
		{
			get
			{
				if (HasReachedCouncilMaxForcedLevel)
				{
					return true;
				}
				if (Level < MaxUpgradeLevel)
				{
					return Level == base.gameEconomyData.GetMaximumUpgradeLevel(BuildingType.Name);
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool HasReachedCouncilMaxForcedLevel
		{
			get
			{
				if (BuildingType.Name == "Council" && base.manager.GameEconomyData.ConfigData.ForceCouncilMaxLevel != 0)
				{
					return Level >= base.manager.GameEconomyData.ConfigData.ForceCouncilMaxLevel;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanPayUpgrade
		{
			get
			{
				if (CanUpgrade)
				{
					return GetUpgradeCashier(instantUpgrade: false).CanAfford();
				}
				return false;
			}
		}

		[JsonIgnore]
		public virtual bool UpgradeInside => false;

		[JsonIgnore]
		public bool IsUpgrading => UpgradeTimer > 0;

		public static string GetBuildingLevelBlackboardKey(string buildingTypeName, int buildingTypeIndex)
		{
			return "Counter." + buildingTypeName + "." + buildingTypeIndex + ".Level";
		}

		public override void Initialize()
		{
			base.Initialize();
			if (BuildingType != null)
			{
				MarkedToBeDeleted = false;
				TypeIndex = base.manager.CampModel.GetBuildingCount(TypeName);
				InitProducer();
				InitStorages();
				base.Size = new GridSize((int)Math.Ceiling((float)base.gameEconomyData.ScaleToGrid(BuildingType.Size.X) * 0.5f) * 2, (int)Math.Ceiling((float)base.gameEconomyData.ScaleToGrid(BuildingType.Size.Y) * 0.5f) * 2);
			}
		}

		protected virtual void InitProducer()
		{
			if (BuildingType.ProductionType != CurrencyType.None)
			{
				Producer = new ProducerModel(BuildingType.ProductionType);
				Producer.SetManager(base.manager);
				Producer.Initialize();
				UpdateProduction();
			}
		}

		private void InitStorages()
		{
			Storages = new ModelList<StorageModel>();
			Storages.SetManager(base.manager);
			Storages.Initialize();
			BuildingUpgradeLevel currentUpgradeLevel = GetCurrentUpgradeLevel();
			if (currentUpgradeLevel != null)
			{
				if (currentUpgradeLevel.SuppliesCapacity != 0)
				{
					AddStorage(CurrencyType.Supplies);
				}
				if (currentUpgradeLevel.SPTraitCapacity != 0)
				{
					AddStorage(CurrencyType.SPTraitsUpgradeToken);
				}
				if (currentUpgradeLevel.SPCapacity != 0)
				{
					AddStorage(CurrencyType.SurvivalPoints);
				}
				if (currentUpgradeLevel.OutpostCapacity != 0)
				{
					AddStorage(CurrencyType.Outpost);
				}
			}
		}

		private void AddStorage(CurrencyType type)
		{
			StorageModel storageModel = new StorageModel(type);
			storageModel.SetManager(base.manager);
			storageModel.Initialize();
			Storages.Add(storageModel);
		}

		public StorageModel GetStorage(CurrencyType type)
		{
			for (int i = 0; i < Storages.Count; i++)
			{
				StorageModel storageModel = Storages[i];
				if (storageModel.CurrencyType == type)
				{
					return storageModel;
				}
			}
			return null;
		}

		private void UpdateStorages()
		{
			for (int i = 0; i < Storages.Count; i++)
			{
				StorageModel storageModel = Storages[i];
				storageModel.SetCapacity(GetCurrentUpgradeCapacity(storageModel.CurrencyType));
				base.manager.Player.UpdateCurrencyCapacity(storageModel.CurrencyType);
			}
		}

		public int GetCurrentUpgradeCapacity(CurrencyType currencyType)
		{
			return GetCurrentUpgradeLevel()?.GetCapacity(currencyType) ?? 0;
		}

		public override void Start()
		{
			base.Start();
			if (BuildingType != null)
			{
				pendingProductionInitialization = true;
				UpdateStorages();
				base.Size = new GridSize((int)Math.Ceiling((float)base.gameEconomyData.ScaleToGrid(BuildingType.Size.X) * 0.5f) * 2, (int)Math.Ceiling((float)base.gameEconomyData.ScaleToGrid(BuildingType.Size.Y) * 0.5f) * 2);
			}
		}

		public BuildingUpgradeLevel GetCurrentUpgradeLevel()
		{
			if (BuildingType == null)
			{
				return null;
			}
			return base.gameEconomyData.GetBuildingUpgradeLevel(BuildingType.Name, Level);
		}

		public BuildingUpgradeLevel GetNextUpgradeLevel()
		{
			if (BuildingType == null)
			{
				return null;
			}
			return base.gameEconomyData.GetBuildingUpgradeLevel(BuildingType.Name, Level + 1);
		}

		public void SetPosition(FixedVec2 position)
		{
			Position = position;
			NotifyChange("position");
		}

		public void SetRotationAngle(float rotationAngle)
		{
			RotationAngle = rotationAngle;
			NotifyChange("RotationAngle");
		}

		public void SetLevel(int level)
		{
			Level = level;
			NotifyChange("level");
		}

		public void SetTypeName(string typeName)
		{
			TypeName = typeName;
			buildingType = null;
		}

		public Cashier GetUpgradeCashier(bool instantUpgrade, bool addSpeedUpCashier = true)
		{
			return base.manager.Player.Camp.GetBuildingUpgradeCashier(buildingType.Name, Level + 1, instantUpgrade, addSpeedUpCashier);
		}

		public Cashier GetInstantUpgradeCashierWithTokens()
		{
			return base.manager.Player.Camp.GetInstantBuildingUpgradeCashierWithTokens();
		}

		public Cashier GetSpeedUpUpgradeCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.SpeedUp);
			cashierItem.SetCost(CurrencyType.Diamonds, GetSpeedUpUpgradeCost());
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public Cashier GetSpeedUpUpgradeCashierWithTokens()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.SpeedUp);
			cashierItem.SetCost(CurrencyType.BuildingTokenBP, 1);
			cashier.AddItem(cashierItem);
			cashier.useTokensForPayment = true;
			return cashier;
		}

		public TWDModelResult UpgradeInstant(int useDiamondsAmount = -2, Cashier cashier = null)
		{
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (IsUpgrading)
			{
				return TWDModelResult.AlreadyUpgrading;
			}
			if (!CanUpgrade)
			{
				return TWDModelResult.AlreadyMaxLevel;
			}
			if (cashier != null && cashier.useTokensForPayment)
			{
				tWDModelResult = GetInstantUpgradeCashierWithTokens().PayWithTokens(this);
			}
			else
			{
				cashier = GetUpgradeCashier(instantUpgrade: true);
				cashier.UsedReason = "UpgradeBuildingInstant";
				tWDModelResult = cashier.Pay(this);
				cashier.UseDiamondsAmount = useDiamondsAmount;
			}
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			UpdateProduction();
			CompleteUpgrade(Metrics.UpgradeTypes.Instant);
			return TWDModelResult.OK;
		}

		public TWDModelResult StartUpgrade(int useDiamondsAmount = -2)
		{
			if (IsUpgrading)
			{
				return TWDModelResult.AlreadyUpgrading;
			}
			if (!CanUpgrade)
			{
				return TWDModelResult.AlreadyMaxLevel;
			}
			BuildingUpgradeLevel nextUpgradeLevel = GetNextUpgradeLevel();
			Cashier upgradeCashier = GetUpgradeCashier(instantUpgrade: false);
			upgradeCashier.UseDiamondsAmount = useDiamondsAmount;
			TWDModelResult tWDModelResult = upgradeCashier.Pay(this);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			base.manager.Player.Camp.IsBuildingUpgradeInProgress()?.CompleteUpgrade(Metrics.UpgradeTypes.SpeedUp);
			UpgradeTimer = ModelHelpers.SecondsToMilliSeconds(base.manager.Player.ActivityManager.GetBuildingUpgradeTime(nextUpgradeLevel));
			OriginalUpgradeTimer = UpgradeTimer;
			UpdateProduction();
			NotifyChange("build");
			return TWDModelResult.OK;
		}

		public TWDModelResult SpeedUpUpgrade(Cashier cashier = null)
		{
			if (UpgradeTimer == 0L)
			{
				return TWDModelResult.NotUpgrading;
			}
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (cashier == null)
			{
				goto IL_02c9;
			}
			if (!cashier.useTokensForPayment)
			{
				Dictionary<CurrencyType, int> useExtraTokens = cashier.UseExtraTokens;
				if (useExtraTokens == null || useExtraTokens.Count <= 0)
				{
					goto IL_02c9;
				}
			}
			if (cashier.useTokensForPayment)
			{
				tWDModelResult = GetSpeedUpUpgradeCashierWithTokens().PayWithTokens(this);
			}
			else
			{
				Dictionary<CurrencyType, int> useExtraTokens2 = cashier.UseExtraTokens;
				if (useExtraTokens2 != null && useExtraTokens2.Count > 0)
				{
					Dictionary<long, SpeedupTokenTimeTranslate> dictionary = new Dictionary<long, SpeedupTokenTimeTranslate>();
					foreach (KeyValuePair<CurrencyType, int> useExtraToken in cashier.UseExtraTokens)
					{
						if (useExtraToken.Value > 0)
						{
							SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency = base.manager.GameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(useExtraToken.Key.ToString());
							if (speedupTokenTimeDefinitionByCurrency == null || speedupTokenTimeDefinitionByCurrency.SpeedupType != SpeedupType.Building || speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime() <= 0)
							{
								return TWDModelResult.Error;
							}
							if (dictionary.ContainsKey(useExtraToken.Value))
							{
								return TWDModelResult.Error;
							}
							SpeedupTokenTimeTranslate value = new SpeedupTokenTimeTranslate
							{
								CurrencyType = useExtraToken.Key,
								SpeedupTimeMilliseconds = speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime(),
								ConsumeAmount = useExtraToken.Value,
								SpeedupTokenTimeDefinition = speedupTokenTimeDefinitionByCurrency
							};
							dictionary.Add(speedupTokenTimeDefinitionByCurrency.GetSpeedupMSTime(), value);
						}
					}
					if (dictionary.Count == 0)
					{
						return TWDModelResult.Error;
					}
					Dictionary<CurrencyType, int> dictionary2 = new Dictionary<CurrencyType, int>();
					List<KeyValuePair<long, SpeedupTokenTimeTranslate>> list = dictionary.OrderByDescending((KeyValuePair<long, SpeedupTokenTimeTranslate> kv) => kv.Key).ToList();
					long num = UpgradeTimer;
					bool flag = false;
					foreach (KeyValuePair<long, SpeedupTokenTimeTranslate> item in list)
					{
						for (int num2 = 0; num2 < item.Value.ConsumeAmount; num2++)
						{
							num -= item.Value.SpeedupTimeMilliseconds;
							if (dictionary2.ContainsKey(item.Value.CurrencyType))
							{
								dictionary2[item.Value.CurrencyType]++;
							}
							else
							{
								dictionary2[item.Value.CurrencyType] = 1;
							}
							if (num <= 0)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
					foreach (KeyValuePair<CurrencyType, int> item2 in dictionary2)
					{
						tWDModelResult = Cashier.CreateOneItemCashier(base.manager, PurchaseType.SpeedupAndBuildingUpgrade, item2.Key, item2.Value).PayWithTokens(this);
						if (tWDModelResult != TWDModelResult.OK)
						{
							return tWDModelResult;
						}
					}
					UpgradeTimer = num;
					if (UpgradeTimer <= 0)
					{
						CompleteUpgrade(Metrics.UpgradeTypes.SpeedUp);
					}
					return TWDModelResult.OK;
				}
			}
			goto IL_02e4;
			IL_02e4:
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			CompleteUpgrade(Metrics.UpgradeTypes.SpeedUp);
			return TWDModelResult.OK;
			IL_02c9:
			Cashier speedUpUpgradeCashier = GetSpeedUpUpgradeCashier();
			speedUpUpgradeCashier.UsedReason = "UpgradeBuildingSpeedUp";
			tWDModelResult = speedUpUpgradeCashier.Pay(this);
			goto IL_02e4;
		}

		public TWDModelResult AdSpeedUpUpgrade()
		{
			if (UpgradeTimer == 0L)
			{
				return TWDModelResult.NotUpgrading;
			}
			if (!CanUpgrade)
			{
				return TWDModelResult.AlreadyMaxLevel;
			}
			if (SpeedUpByAd)
			{
				return TWDModelResult.Error;
			}
			SpeedUpByAd = true;
			BuildingUpgradeLevel nextUpgradeLevel = GetNextUpgradeLevel();
			long num = ModelHelpers.SecondsToMilliSeconds(base.manager.Player.ActivityManager.GetBuildingUpgradeTime(nextUpgradeLevel));
			UpgradeTimer -= (long)(num * base.gameEconomyData.ConfigData.AdsBuildingSpeedUpMultiplier);
			if (UpgradeTimer <= 0)
			{
				CompleteUpgrade(Metrics.UpgradeTypes.AdSpeedUpgrade);
			}
			return TWDModelResult.OK;
		}

		public virtual TWDModelResult CancelUpgrade()
		{
			if (UpgradeTimer == 0L)
			{
				return TWDModelResult.NotUpgrading;
			}
			Dictionary<CurrencyType, OverflowableAmount> refundedAmounts = GetUpgradeCashier(instantUpgrade: false, addSpeedUpCashier: false).Refund(base.manager.Player.gameEconomyData.ConfigData.CancelUpgradeRefundPercentage, dontAllowMultiplier: true);
			base.manager.Metrics.AddFind().AddResources(refundedAmounts).AddCancelUpgrade(this)
				.AddBuilding(this)
				.Send();
			UpgradeTimer = 0L;
			OriginalUpgradeTimer = 0L;
			NotifyChange("cancelUpgrade");
			if (Level == 0)
			{
				DeleteMe();
			}
			return TWDModelResult.OK;
		}

		protected virtual void CompleteUpgrade(Metrics.UpgradeTypes upgradeType)
		{
			UpgradeTimer = 0L;
			OriginalUpgradeTimer = 0L;
			SpeedUpByAd = false;
			Level++;
			UpdateProduction();
			UpdateStorages();
			base.manager.Player.AddXp(GetCurrentUpgradeLevel().AwardedXp);
			if (BuildingType.Name == "Council")
			{
				base.manager.Player.NewbieSenvenQuest.OnCouncilLevelUp(Level);
				base.manager.Player.NotifyCouncilLevelUp(Level);
			}
			if (upgradeType == Metrics.UpgradeTypes.AdSpeedUpgrade)
			{
				base.manager.Metrics.AddEnd().AddVideoAd(AdProvider.UnityAds, AdStatus.OK).AddUpgrade()
					.AddBuilding(this)
					.Send();
			}
			base.manager.Metrics.AddEnd().AddUpgrade(upgradeType).AddBuilding(this)
				.AddLevel()
				.Send();
			if (base.Manager.Mode == ModelManagerMode.Client && !base.manager.Player.Camp.InCamp)
			{
				base.manager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.Building, base.ModelId, BuildingType.Name, Level);
			}
			if (base.Manager.Mode == ModelManagerMode.Client)
			{
				base.manager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.LaunchTutorial, base.ModelId, BuildingType.Name, Level);
			}
			if (BuildingType.Name == "MissionCar" && Level == 1)
			{
				base.manager.Player.GetCurrency(CurrencyType.ReplayToken).SetCapacity(base.manager.Player.GetCapacity(CurrencyType.ReplayToken));
				base.manager.Player.GetCurrency(CurrencyType.ReplayToken).SetValue(base.manager.Player.GetCapacity(CurrencyType.ReplayToken));
			}
			if (TypeName == "RadioTent")
			{
				if (Level == 1)
				{
					int councilLevel = base.Camp.GetCouncilLevel();
					if (councilLevel > 5)
					{
						councilLevel -= 2;
						int num = 1;
						int num2 = 1;
						BuildingUpgradeLevel buildingUpgradeLevel = null;
						do
						{
							buildingUpgradeLevel = base.gameEconomyData.GetBuildingUpgradeLevel(TypeName, num2);
							if (buildingUpgradeLevel != null && buildingUpgradeLevel.DependencyLevelRequired <= councilLevel)
							{
								num = buildingUpgradeLevel.Level;
							}
							num2++;
						}
						while (buildingUpgradeLevel != null && buildingUpgradeLevel.DependencyLevelRequired < councilLevel);
						Level = num;
					}
				}
				base.manager.Player.PhoneCall.UpdateFreeCallTimers(onBuildingUpgrade: true);
			}
			NotifyChange("level");
			base.manager.Player.DailyQuestManager.StartAction("Upgrade").TargetType = "Building";
			base.manager.Player.DailyQuestManager.CommitAction();
			base.manager.Player.NotifyItemUpgraded(ReturnQuestType.UpgradeBuilding);
		}

		private int GetMaximumUpgradeLevel(string buildingName)
		{
			int councilLevel = base.Camp.GetCouncilLevel();
			int maximumUpgradeLevel = base.gameEconomyData.GetMaximumUpgradeLevel(buildingName);
			for (int i = 2; i <= maximumUpgradeLevel; i++)
			{
				if (base.gameEconomyData.GetBuildingUpgradeLevel(buildingName, i).DependencyLevelRequired > councilLevel)
				{
					return i - 1;
				}
			}
			return maximumUpgradeLevel;
		}

		public TWDModelResult Collect()
		{
			CurrencyModel currency = base.manager.Player.GetCurrency(Producer.CurrencyType);
			int num = Producer.Collect();
			if (Producer.LastCollectedAmount > 0)
			{
				NotifyChange("collected");
				if (base.manager != null && base.manager.Player != null)
				{
					if ((Producer.CurrencyType == CurrencyType.SurvivalPoints && base.manager.Player.HasSurvivalPointsDoubleBonus()) || (Producer.CurrencyType == CurrencyType.Supplies && base.manager.Player.GetSuppliesMultiplierValue() > 1))
					{
						num = (int)(num * currency.AddMultiplier);
					}
					base.manager.Metrics.AddFind().AddResources(Producer.CurrencyType, num, currency.LastAdded).AddBuilding(this)
						.Send();
				}
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}

		protected virtual void UpdateProduction()
		{
			if (Producer == null || base.manager.Player == null)
			{
				return;
			}
			if (IsUpgrading || !BuildingRepaired)
			{
				Producer.SetRate(0);
				return;
			}
			BuildingUpgradeLevel currentUpgradeLevel = GetCurrentUpgradeLevel();
			Producer.SetRate(base.manager.Player.ActivityManager.GetBuildingUpgradeRate(currentUpgradeLevel));
			Producer.SetCapacity(currentUpgradeLevel.ProductionCapacity);
			base.manager.Player.UpdateCurrencyCapacity(Producer.CurrencyType);
			if (!base.manager.Player.Tutorial.Completed && Producer.Accumulated == 0L && Producer.CurrencyType == CurrencyType.Supplies)
			{
				Producer.SetAmount(240);
			}
		}

		public TWDModelResult MoveBuilding(GridPosition destination)
		{
			GridPosition gridPosition = base.GridPosition;
			base.GridPosition = destination;
			if (base.Camp.CanPlaceBuilding(this))
			{
				SetGridPosition(destination.X, destination.Y);
				return TWDModelResult.OK;
			}
			base.GridPosition = gridPosition;
			return TWDModelResult.InvalidPosition;
		}

		public override void Tick(long deltaTime)
		{
			if (pendingProductionInitialization)
			{
				UpdateProduction();
				pendingProductionInitialization = false;
			}
			base.Tick(deltaTime);
			if (UpgradeTimer <= 0)
			{
				return;
			}
			UpgradeTimer -= deltaTime;
			if (UpgradeTimer <= 0)
			{
				long num = -UpgradeTimer;
				CompleteUpgrade(Metrics.UpgradeTypes.Regular);
				if (Producer != null && num > 0)
				{
					Producer.TickProduction(num);
				}
			}
		}

		public override bool IsValid()
		{
			if (BuildingType == null)
			{
				base.Debug.LogError("Building has no building type!");
			}
			return BuildingType != null;
		}

		public int GetSpeedUpUpgradeCost()
		{
			if (!base.manager.Player.Tutorial.Completed && !base.manager.Player.Tutorial.ShowDiamondsHud && UpgradeTimer < 200000)
			{
				return 0;
			}
			return base.gameEconomyData.TimeToDiamonds(UpgradeTimer);
		}

		public void DeleteMe()
		{
			NotifyChange("RemoveBuilding", this);
			MarkedToBeDeleted = true;
		}

		public List<long> GetFreeCallTimers()
		{
			return GetCurrentUpgradeLevel().FreeCallTimeSeconds;
		}

		public long GetFreeCallTimersOnBuildingUpgrade()
		{
			return GetCurrentUpgradeLevel().FreeCallTimeOnUpgrade;
		}

		public List<int> GetFreeCallMaxAmounts()
		{
			return GetCurrentUpgradeLevel().FreeCallMaxStackable;
		}

		public List<int> GetUpgradedCallChances()
		{
			return GetCurrentUpgradeLevel().UpgradedCallChance;
		}

		public bool NeedSpeedUpButton()
		{
			if (!IsUpgrading && !IsUpgradingEquipment() && !IsUpgradingSurvivor())
			{
				return IsUpgradingWalker();
			}
			return true;
		}

		public bool IsUpgradingEquipment()
		{
			if (this is WorkshopBuildingModel)
			{
				return ((WorkshopBuildingModel)this).UpgradingEquipment != null;
			}
			return false;
		}

		public bool IsUpgradingWalker()
		{
			if (this is CageBuildingModel)
			{
				return ((CageBuildingModel)this).UpgradingWalker != null;
			}
			return false;
		}

		public bool IsUpgradingSurvivor()
		{
			if (this is TrainingGroundBuildingModel)
			{
				return ((TrainingGroundBuildingModel)this).UpgradingSurvivor != null;
			}
			return false;
		}
	}
}
