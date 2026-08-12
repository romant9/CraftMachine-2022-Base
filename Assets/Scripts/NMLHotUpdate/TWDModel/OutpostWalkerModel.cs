using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class OutpostWalkerModel : TWDModelObject
	{
		public const string AmountUpdated = "AmountUpdated";

		private ActorModel walker;

		public string Id { get; private set; }

		public int Level { get; set; }

		public TimedActionModel TimedActionModel { get; protected set; }

		public int Amount { get; set; }

		public bool HasUnlockedWithMoney { get; set; }

		public int WalkerAmountUpgradesPerfomed { get; private set; }

		public int WalkerLevelUpgradesPerformed { get; private set; }

		[JsonIgnore]
		public CageDefinition CurrentUpgradeDefinition => base.gameEconomyData.GetCageDefinition(Id, Level);

		[JsonIgnore]
		public CageDefinition NextUpgradeDefinition => base.gameEconomyData.GetCageDefinition(Id, Level + 1);

		[JsonIgnore]
		public int UpgradeTime
		{
			get
			{
				CageDefinition nextUpgradeDefinition = NextUpgradeDefinition;
				if (nextUpgradeDefinition != null)
				{
					return nextUpgradeDefinition.UpgradeTime / 1000;
				}
				return 0;
			}
		}

		[JsonIgnore]
		public int UpgradeCostOutpost => NextUpgradeDefinition?.CostLevelUpOutpost ?? 0;

		[JsonIgnore]
		public bool HasReachedMaxLevel => Level >= MaxUpgradeLevel;

		[JsonIgnore]
		public bool CanUpgrade
		{
			get
			{
				if (!IsLocked && Level < MaxUpgradeLevel && NextUpgradeDefinition.DependencyLevelRequired <= base.manager.Player.Camp.GetBuildingLevel("Cage"))
				{
					return !IsUpgrading();
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsLocked
		{
			get
			{
				if (HasUnlockedWithMoney)
				{
					return false;
				}
				CageDefinition currentUpgradeDefinition = CurrentUpgradeDefinition;
				if (string.IsNullOrEmpty(currentUpgradeDefinition.EpisodeLock))
				{
					return false;
				}
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(currentUpgradeDefinition.EpisodeLock);
				if (missionGroupModelForSpawnPointGroup != null)
				{
					return !missionGroupModelForSpawnPointGroup.AreAllStoryMissionsCompleted();
				}
				return false;
			}
		}

		[JsonIgnore]
		public int UnlockCostDiamounds => NextUpgradeAmountDefinition?.UnlockPriceDiamonds ?? 0;

		[JsonIgnore]
		public int MaxUpgradeLevel => base.gameEconomyData.GetMaxCageWalkerLevel(Id);

		[JsonIgnore]
		public CageDefinition CurrentUpgradeAmountDefinition => base.gameEconomyData.GetCageDefinition(Id, Amount);

		[JsonIgnore]
		public CageDefinition NextUpgradeAmountDefinition => base.gameEconomyData.GetCageDefinition(Id, Amount + 1);

		[JsonIgnore]
		public int UpgradeAmountCostOuptost => NextUpgradeAmountDefinition?.CostAmountOuptost ?? 0;

		[JsonIgnore]
		public bool HasReachedMaxAmount => Amount >= MaxAmount;

		[JsonIgnore]
		public bool CanUpgradeAmount
		{
			get
			{
				if (Level > 0 && !HasReachedMaxAmount)
				{
					return NextUpgradeAmountDefinition.AmountDependencyLevelRequired <= base.manager.Player.Camp.GetBuildingLevel("Cage");
				}
				return false;
			}
		}

		[JsonIgnore]
		public int MaxAmount => base.gameEconomyData.GetMaxCageWalkerAmount(Id);

		[JsonIgnore]
		public ActorDefinition ActorDefinition => base.gameEconomyData.GetActorDefinition(Id);

		public bool IsUpgrading()
		{
			return TimedActionModel.IsActionUnderway();
		}

		public override void Start()
		{
			base.Start();
			TimedActionModel.Changed += OnTimedActionModelChanged;
		}

		public override void Initialize()
		{
			base.Initialize();
			HasUnlockedWithMoney = false;
			TimedActionModel = new TimedActionModel();
			TimedActionModel.SetManager(base.manager);
			TimedActionModel.Initialize();
			TimedActionModel.PurchaseType = PurchaseType.SpeedUpWalkerUpgrade;
			Level = 1;
			Amount = 0;
		}

		public void InitWalkerId(string id)
		{
			Id = id;
			if (Id == WalkerType.WalkerNormal.ToString())
			{
				Amount = base.manager.GameEconomyData.ConfigData.WalkerCageInitialNormalWalkerCount;
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public void ApplyUpgradeTrackingMigration(bool playerCreatedBeforeGEDChanges)
		{
			if (Id == WalkerType.WalkerNormal.ToString())
			{
				if (playerCreatedBeforeGEDChanges)
				{
					int num = Math.Min(base.manager.GameEconomyData.ConfigData.OutpostWalkersToAddIfCreatedWithDifferentMinimum, MaxAmount - Amount);
					if (num > 0)
					{
						Amount = Math.Min(Amount + num, MaxAmount);
						NotifyChange("AmountUpdated");
					}
				}
				WalkerAmountUpgradesPerfomed = Math.Max(0, Amount - base.manager.GameEconomyData.ConfigData.WalkerCageInitialNormalWalkerCount);
			}
			else
			{
				WalkerAmountUpgradesPerfomed = Math.Max(0, Amount - 1);
			}
			WalkerLevelUpgradesPerformed = Math.Max(0, Level - 1);
		}

		public TWDModelResult UpgradeAmount(int useDiamondsAmount)
		{
			Cashier upgradeAmountCashier = GetUpgradeAmountCashier();
			if (upgradeAmountCashier != null)
			{
				upgradeAmountCashier.UseDiamondsAmount = useDiamondsAmount;
				TWDModelResult tWDModelResult = upgradeAmountCashier.Pay(this);
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}
			WalkerAmountUpgradesPerfomed++;
			Amount++;
			NotifyChange("AmountUpdated");
			return TWDModelResult.OK;
		}

		public TWDModelResult UpgradeInstant()
		{
			if (CanUpgrade && GetUpgradeCashier(instantUpgrade: true, !IsUpgrading()).CanAfford())
			{
				return TimedActionModel.StartActionInstant(GetUpgradeCashier(instantUpgrade: true, !IsUpgrading()), this);
			}
			return TWDModelResult.Error;
		}

		public TWDModelResult StartUpgrade(int useDiamondsAmount)
		{
			if (CanUpgrade)
			{
				Cashier upgradeCashier = GetUpgradeCashier(instantUpgrade: false);
				upgradeCashier.UseDiamondsAmount = useDiamondsAmount;
				return TimedActionModel.StartAction(UpgradeTime, upgradeCashier, this);
			}
			return TWDModelResult.Error;
		}

		public Cashier GetUpgradeCashier(bool instantUpgrade, bool addInitialSurvivorPoints = false)
		{
			Cashier cashier = new Cashier(base.manager);
			if (instantUpgrade)
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.InstantWalkerUpgrade);
				int num = base.gameEconomyData.TimeToDiamonds(UpgradeTime * 1000);
				if (addInitialSurvivorPoints)
				{
					num += base.gameEconomyData.CurrencyToDiamonds(CurrencyType.Outpost, UpgradeCostOutpost);
				}
				cashierItem.SetCost(CurrencyType.Diamonds, num);
				cashier.AddItem(cashierItem);
			}
			else
			{
				CashierItem cashierItem2 = new CashierItem(PurchaseType.UpgradeWalker);
				cashierItem2.SetCost(CurrencyType.Outpost, UpgradeCostOutpost);
				cashier.AddItem(cashierItem2);
			}
			return cashier;
		}

		public Cashier GetUpgradeAmountCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeWalkerAmount);
			cashierItem.SetCost(CurrencyType.Outpost, UpgradeAmountCostOuptost);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public Cashier GetUnlockCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.UnlockWalker);
			cashierItem.SetCost(CurrencyType.Diamonds, UnlockCostDiamounds);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		private void OnTimedActionModelChanged(ModelObject m, string changed, object args)
		{
			if (changed == "ActionStartEvent")
			{
				NotifyChange("ActionStartEvent", this);
			}
			else if (changed == "ActionFinishedEvent")
			{
				Level++;
				WalkerLevelUpgradesPerformed++;
				base.manager.Metrics.AddEnd().AddUpgrade().AddOupostWalker(this)
					.AddLevel()
					.Send();
				if (base.Manager.Mode == ModelManagerMode.Client && !base.manager.Player.Camp.InCamp)
				{
					base.manager.Player.Camp.AddNotificationQueueItem(NotificationQueueItem.Type.Walker, base.ModelId, "", Level);
				}
				NotifyChange("ActionFinishedEvent", this);
			}
		}

		public ActorModel GetActorModel()
		{
			if (walker == null)
			{
				ActorDefinition actorDefinition = base.gameEconomyData.GetActorDefinition(Id);
				walker = ActorModel.Create(Faction.Walker);
				walker.ActorDefinitionID = actorDefinition.ID;
				walker.Faction = Faction.Walker;
				walker.Level = 1;
				walker.CharacterPrefab = actorDefinition.VisualAsset;
				walker.OutfitDefinitionID = actorDefinition.OutfitDefinitionID;
				walker.SetManager(base.manager);
			}
			return walker;
		}

		public CageDefinition GetCafeDefinitionForLevel(int level)
		{
			return base.gameEconomyData.GetCageDefinition(Id, level);
		}

		public int GetDamageForLevel(int level)
		{
			return base.manager.GameEconomyData.GetActorLevelDefinition(Id, level, returnBestMatch: false)?.Damage ?? 0;
		}

		public int GetHitpointsForLevel(int level)
		{
			return base.manager.GameEconomyData.GetActorLevelDefinition(Id, level, returnBestMatch: false)?.Health ?? 0;
		}

		public Dictionary<string, string> GetAnalyticsProperties()
		{
			PlayerModel player = base.manager.Player;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int level = player.Camp.GetBuilding("Council").Level;
			int level2 = player.Level;
			dictionary.Add("council_level", level.ToString());
			dictionary.Add("player_level", level2.ToString());
			dictionary.Add("walker_level", Level.ToString());
			dictionary.Add("walker_class", (Id != null) ? Id : "");
			dictionary.Add("walker_amount", Amount.ToString());
			return dictionary;
		}

		public void Unlock()
		{
			HasUnlockedWithMoney = true;
			Amount = 1;
			Level = 1;
		}
	}
}
