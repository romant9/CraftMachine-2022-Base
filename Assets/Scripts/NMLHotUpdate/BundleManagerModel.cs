using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class BundleManagerModel : TWDModelObject
{
	public static string FAKE_SUPPORT_BUNDLE_FOR_REWARDS = "fakeSupportBundleForRewards";

	public const string LimitedBundleAvailableEvent = "LimitedBundleAvailableEvent";

	public const string LimitedBundleExpiredEvent = "LimitedBundleExpiredEvent";

	private static long CheckForNewBundlesDefaultTime = 60000L;

	private static long CheckForLimitedBundlesCombatInterval = 5000L;

	private static long CheckForLimitedBundlesNonCombatInterval = 500L;

	public List<string> BoughtBundles { get; private set; }

	public Dictionary<string, int> BoughtBundlesAmount { get; set; }

	public Dictionary<string, long> BoughtBundlesLastPurchaseTime { get; set; }

	public List<LimitedBundleData> InitiatedLimitedBundles { get; set; }

	public List<CheatOfferEndTime> InitiatedCheatOffers { get; set; }

	public string PendingViewBundleContentDefinition { get; private set; }

	public string PendingViewBundleStoreDefinition { get; private set; }

	public bool PendingViewBundleWasGivenBySupport { get; set; }

	public string PendingViewRewardsGivenBySupport { get; private set; }

	[IgnoreModelProperty]
	public ModelList<EquipmentItemModel> PendingViewEquipments { get; set; }

	[IgnoreModelProperty]
	public ModelList<SurvivorModel> PendingViewSurvivors { get; set; }

	public List<string> PendingViewOutfits { get; set; }

	public List<string> PendingViewHeroSkins { get; set; }

	public ModelList<EquipTokenItemModel> PendingViewEquipTokens { get; set; }

	public EquipmentItemModel ViewEquipment { get; set; }

	public SurvivorModel ViewSurvivor { get; set; }

	public string ViewOutfit { get; set; }

	public string ViewHeroSkin { get; set; }

	public int BoughtIAPsHotfixAppliedTimes { get; set; }

	public long CheckForNewBundlesTimer { get; set; }

	public long CheckForLimitedBundlesTimer { get; set; }

	public long LastPurchaseUTCTime { get; set; }

	public long FirstPurchaseUTCTime { get; set; }

	public string LastInitiatedBundleId { get; set; }

	public string LastInitiatedBundleProductId { get; set; }

	public LootEntry IAPBonusGiftLootEntry { get; set; }

	public ModelList<LootEntry> WebShopLootEntrys { get; set; }

	public ModelList<LootEntry> ShareRewardEntrys { get; set; }

	public RotatingBundleManager RotatingBundleManager { get; set; }

	public override void Initialize()
	{
		base.Initialize();
		BoughtBundles = new List<string>();
		BoughtBundlesAmount = new Dictionary<string, int>();
		BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
		InitiatedLimitedBundles = new List<LimitedBundleData>();
		InitiatedCheatOffers = new List<CheatOfferEndTime>();
		PendingViewEquipments = new ModelList<EquipmentItemModel>();
		PendingViewEquipTokens = new ModelList<EquipTokenItemModel>();
		PendingViewOutfits = new List<string>();
		PendingViewHeroSkins = new List<string>();
		PendingViewSurvivors = new ModelList<SurvivorModel>();
		CheckForNewBundlesTimer = 0L;
		LastPurchaseUTCTime = 0L;
		FirstPurchaseUTCTime = 0L;
		RotatingBundleManager = new RotatingBundleManager();
		RotatingBundleManager.SetManager(base.manager);
		RotatingBundleManager.Initialize();
		WebShopLootEntrys = new ModelList<LootEntry>();
		WebShopLootEntrys.SetManager(base.manager);
		WebShopLootEntrys.Initialize();
		ShareRewardEntrys = new ModelList<LootEntry>();
		ShareRewardEntrys.SetManager(base.manager);
		ShareRewardEntrys.Initialize();
	}

	public override void Start()
	{
		base.Start();
		if (BoughtBundles == null)
		{
			BoughtBundles = new List<string>();
		}
		if (BoughtBundlesAmount == null)
		{
			BoughtBundlesAmount = new Dictionary<string, int>();
		}
		if (BoughtBundlesLastPurchaseTime == null)
		{
			BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
		}
		if (InitiatedLimitedBundles == null)
		{
			InitiatedLimitedBundles = new List<LimitedBundleData>();
		}
		if (InitiatedCheatOffers == null)
		{
			InitiatedCheatOffers = new List<CheatOfferEndTime>();
		}
		if (PendingViewEquipments == null)
		{
			PendingViewEquipments = new ModelList<EquipmentItemModel>();
		}
		if (PendingViewEquipTokens == null)
		{
			PendingViewEquipTokens = new ModelList<EquipTokenItemModel>();
		}
		if (PendingViewOutfits == null)
		{
			PendingViewOutfits = new List<string>();
		}
		if (PendingViewHeroSkins == null)
		{
			PendingViewHeroSkins = new List<string>();
		}
		if (PendingViewSurvivors == null)
		{
			PendingViewSurvivors = new ModelList<SurvivorModel>();
		}
		foreach (KeyValuePair<string, long> item in BoughtBundlesLastPurchaseTime)
		{
			BundleContentDefinition bundleContentDefinition = base.gameEconomyData.GetBundleContentDefinition(item.Key);
			if (bundleContentDefinition != null && !string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
			{
				if (LastPurchaseUTCTime < item.Value)
				{
					LastPurchaseUTCTime = item.Value;
				}
				if (FirstPurchaseUTCTime == 0L || FirstPurchaseUTCTime > item.Value)
				{
					FirstPurchaseUTCTime = item.Value;
				}
			}
		}
		CheckForNewBundlesTimer = 0L;
	}

	public void CreateRotatingBundleManagerForOldPlayer()
	{
		if (RotatingBundleManager == null)
		{
			RotatingBundleManager = new RotatingBundleManager();
			RotatingBundleManager.Initialize();
		}
	}

	public void SetInitiatedBundlePurchase(string bundleId)
	{
		BundleContentDefinition bundleContentDefinition = ((bundleId == null) ? null : base.manager.GameEconomyData.GetBundleContentDefinition(bundleId));
		CustomBundleDefinition customBundleDefinition = null;
		if (bundleContentDefinition == null)
		{
			base.manager.Debug.LogError("BundleManagerModel3.SetInitiatedBundlePurchase: Custom bundle " + bundleId + " is not available for store");
			customBundleDefinition = base.manager.GameEconomyData.GetCustomBundleDefinition(bundleId);
		}
		if (bundleContentDefinition == null)
		{
			if (customBundleDefinition != null)
			{
				base.manager.Debug.LogError("BundleManagerModel0.SetInitiatedBundlePurchase: Custom bundle " + bundleId + " is not available for store");
				LastInitiatedBundleId = bundleId;
				LastInitiatedBundleProductId = customBundleDefinition.IAPProduct;
			}
			else
			{
				base.manager.Debug.LogError("BundleManagerModel2.SetInitiatedBundlePurchase: Custom bundle " + bundleId + " is not available for store");
				LastInitiatedBundleId = null;
				LastInitiatedBundleProductId = null;
			}
		}
		else
		{
			base.manager.Debug.LogError("BundleManagerModel1.SetInitiatedBundlePurchase: Custom bundle " + bundleId + " is not available for store");
			LastInitiatedBundleId = bundleId;
			LastInitiatedBundleProductId = bundleContentDefinition.IAPProduct;
		}
	}

	private void CheckForNewLimitedBundles()
	{
		List<BundleStoreDefinition> orderedAvailableBundles = GetOrderedAvailableBundles();
		for (int i = 0; i < orderedAvailableBundles.Count; i++)
		{
			BundleStoreDefinition bundleStoreDefinition = orderedAvailableBundles[i];
			if ((bundleStoreDefinition.HasDateLimit || bundleStoreDefinition.AvailabilityTime > 0) && GetInitiatedLimitedBundle(bundleStoreDefinition.BundleIdentifier) == null && !bundleStoreDefinition.IsPartOfRotation)
			{
				SetupNewLimitedBundle(bundleStoreDefinition);
			}
		}
	}

	private void ResetLimitedBundlesAmountCounter()
	{
		if (BoughtBundlesLastPurchaseTime == null)
		{
			return;
		}
		List<string> list = null;
		foreach (KeyValuePair<string, long> item in BoughtBundlesLastPurchaseTime)
		{
			BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(item.Key);
			if (bundleStoreDefinition != null && bundleStoreDefinition.HasDateLimit && bundleStoreDefinition.StartTimeMilliseconds > item.Value)
			{
				if (list == null)
				{
					list = new List<string>();
				}
				list.Add(item.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			string key = list[i];
			if (BoughtBundlesAmount.ContainsKey(key))
			{
				BoughtBundlesAmount.Remove(key);
			}
			if (BoughtBundlesLastPurchaseTime.ContainsKey(key))
			{
				BoughtBundlesLastPurchaseTime.Remove(key);
			}
		}
	}

	private void SetupNewLimitedBundle(BundleStoreDefinition bundle, bool skipValidation = false)
	{
		if (skipValidation || (IsBundleAvailableForStore(bundle) && GetInitiatedLimitedBundle(bundle.BundleIdentifier) == null && (bundle.HasDateLimit || bundle.AvailabilityTime > 0)))
		{
			LimitedBundleData limitedBundleData = new LimitedBundleData();
			limitedBundleData.BundleID = bundle.BundleIdentifier;
			limitedBundleData.IsAvailable = true;
			if (bundle.HasDateLimit && bundle.AvailabilityTime <= 0)
			{
				long utcTimeStamp = base.manager.Player.UtcTimeStamp;
				long bundleStoreEndTimeConsideringCheats = GetBundleStoreEndTimeConsideringCheats(bundle);
				limitedBundleData.Timer = Math.Max(0L, bundleStoreEndTimeConsideringCheats - utcTimeStamp);
			}
			else
			{
				limitedBundleData.Timer = bundle.AvailabilityTime * 1000;
			}
			limitedBundleData.StartTimestamp = bundle.StartTimestamp;
			limitedBundleData.EndTimestamp = bundle.EndTimestamp;
			limitedBundleData.AvailabilityTime = bundle.AvailabilityTime;
			limitedBundleData.MinTimeFromLastCategoryBought = bundle.MinTimeFromLastCategoryBought;
			InitiatedLimitedBundles.Add(limitedBundleData);
			NotifyChange("LimitedBundleAvailableEvent");
		}
	}

	public bool CanTriggerNewRotatingBundle(BundleStoreDefinition rotatingBundle)
	{
		if (rotatingBundle != null)
		{
			for (int i = 0; i < ((InitiatedLimitedBundles != null) ? InitiatedLimitedBundles.Count : 0); i++)
			{
				LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
				if (limitedBundleData != null && limitedBundleData.BundleID == rotatingBundle.BundleIdentifier)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public void InitiateRotatingBundle(BundleStoreDefinition bundle)
	{
		SetupNewLimitedBundle(bundle, skipValidation: true);
	}

	public void RemoveRotatingBundle(BundleStoreDefinition bundle)
	{
		for (int i = 0; i < ((InitiatedLimitedBundles != null) ? InitiatedLimitedBundles.Count : 0); i++)
		{
			LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
			if (limitedBundleData != null && limitedBundleData.BundleID == bundle.BundleIdentifier)
			{
				InitiatedLimitedBundles.Remove(limitedBundleData);
				break;
			}
		}
	}

	private void TickRegisteredLimitedBundles(long deltaTime)
	{
		List<LimitedBundleData> list = null;
		for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
		{
			LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
			BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(limitedBundleData.BundleID);
			if (bundleStoreDefinition == null || !IsBundleAvailableForStore(bundleStoreDefinition) || HasLimitedBundleDefinitionChanged(limitedBundleData))
			{
				if (list == null)
				{
					list = new List<LimitedBundleData>();
				}
				list.Add(limitedBundleData);
				continue;
			}
			long num = GetBundleStoreEndTimeConsideringCheats(bundleStoreDefinition) - base.manager.Player.UtcTimeStamp;
			long timer = limitedBundleData.Timer;
			limitedBundleData.Timer -= deltaTime;
			if (num > 0)
			{
				limitedBundleData.Timer = Math.Min(limitedBundleData.Timer, num);
			}
			if (limitedBundleData.Timer > 0)
			{
				continue;
			}
			limitedBundleData.IsAvailable = !limitedBundleData.IsAvailable;
			if (limitedBundleData.IsAvailable)
			{
				if (bundleStoreDefinition.HasDateLimit && bundleStoreDefinition.AvailabilityTime <= 0)
				{
					long utcTimeStamp = base.manager.Player.UtcTimeStamp;
					long bundleStoreEndTimeConsideringCheats = GetBundleStoreEndTimeConsideringCheats(bundleStoreDefinition);
					limitedBundleData.Timer = Math.Max(0L, bundleStoreEndTimeConsideringCheats - utcTimeStamp);
				}
				else
				{
					limitedBundleData.Timer = bundleStoreDefinition.AvailabilityTime * 1000;
				}
			}
			else
			{
				limitedBundleData.Timer = limitedBundleData.MinTimeFromLastCategoryBought * 1000;
				long num2 = deltaTime - timer;
				limitedBundleData.Timer = Math.Max(0L, Math.Min(limitedBundleData.Timer, limitedBundleData.Timer - num2));
			}
			NotifyChange(limitedBundleData.IsAvailable ? "LimitedBundleAvailableEvent" : "LimitedBundleExpiredEvent", limitedBundleData.BundleID);
		}
		if (list != null)
		{
			for (int j = 0; j < list.Count; j++)
			{
				LimitedBundleData limitedBundleData2 = list[j];
				string text = ((limitedBundleData2 != null) ? limitedBundleData2.BundleID : "");
				InitiatedLimitedBundles.Remove(list[j]);
				RotatingBundleManager.LimitedBundleRemovedFromBundleManager(text);
				NotifyChange("LimitedBundleExpiredEvent", text);
			}
		}
		ResetLimitedBundlesAmountCounter();
	}

	public override bool IsValid()
	{
		return true;
	}

	public void MarkManagerAsSeen()
	{
		PendingViewBundleContentDefinition = null;
		PendingViewBundleStoreDefinition = null;
		PendingViewBundleWasGivenBySupport = false;
		PendingViewSurvivors = null;
		PendingViewRewardsGivenBySupport = null;
		if (PendingViewOutfits != null)
		{
			PendingViewOutfits.Clear();
		}
		else
		{
			PendingViewOutfits = new List<string>();
		}
		if (PendingViewHeroSkins != null)
		{
			PendingViewHeroSkins.Clear();
		}
		else
		{
			PendingViewHeroSkins = new List<string>();
		}
		if (PendingViewEquipments != null)
		{
			PendingViewEquipments.Clear();
		}
		else
		{
			PendingViewEquipments = new ModelList<EquipmentItemModel>();
		}
		if (PendingViewEquipTokens != null)
		{
			PendingViewEquipTokens.Clear();
		}
		else
		{
			PendingViewEquipTokens = new ModelList<EquipTokenItemModel>();
		}
	}

	public override void Tick(long deltaTime)
	{
		base.Tick(deltaTime);
		long num = CheckForLimitedBundlesNonCombatInterval;
		if (base.manager.Player.Combat != null)
		{
			num = CheckForLimitedBundlesCombatInterval;
		}
		CheckForLimitedBundlesTimer += deltaTime;
		if (CheckForLimitedBundlesTimer >= num)
		{
			TickRegisteredLimitedBundles(CheckForLimitedBundlesTimer);
			CheckForLimitedBundlesTimer = 0L;
		}
		CheckForNewBundlesTimer -= deltaTime;
		if (CheckForNewBundlesTimer <= 0)
		{
			CheckForNewLimitedBundles();
			CheckForNewBundlesTimer = CheckForNewBundlesDefaultTime;
		}
		RotatingBundleManager.Tick(deltaTime);
	}

	public void ResetNewBundlesCheckTimer()
	{
		CheckForNewBundlesTimer = 0L;
	}

	public bool IsBundleBought(string identifier)
	{
		return BoughtBundles.Contains(identifier);
	}

	public bool CanBuyBundle(BundleStoreDefinition bundleStoreEntry)
	{
		if (bundleStoreEntry == null)
		{
			return false;
		}
		PlayerModel player = base.manager.Player;
		GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
		if (player != null && gameEconomyData != null)
		{
			LimitedBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(bundleStoreEntry.BundleIdentifier);
			if (initiatedLimitedBundle != null && !HasLimitedBundleDefinitionChanged(initiatedLimitedBundle) && !initiatedLimitedBundle.IsAvailable)
			{
				base.Debug.Log("Bundle can't be bought because it is during cooldown timer");
				return false;
			}
			if (bundleStoreEntry.IsPartOfRotation && (RotatingBundleManager.CurrentRotationDefinition == null || bundleStoreEntry.BundleIdentifier != RotatingBundleManager.CurrentRotatingBundleIdentifier))
			{
				return false;
			}
			return IsBundleAvailableForStore(bundleStoreEntry);
		}
		return false;
	}

	public long GetSecondsSinceLastPurchaseThatCostMoney()
	{
		PlayerModel player = base.manager.Player;
		if (player != null && BoughtBundlesLastPurchaseTime != null)
		{
			long num = 0L;
			foreach (KeyValuePair<string, long> item in BoughtBundlesLastPurchaseTime)
			{
				BundleStoreDefinition bundleStoreDefinition = player.gameEconomyData.GetBundleStoreDefinition(item.Key);
				BundleContentDefinition bundleContentDefinition = null;
				if (bundleStoreDefinition != null)
				{
					bundleContentDefinition = player.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
				}
				if (item.Value > num && bundleContentDefinition != null && !string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
				{
					num = item.Value;
				}
			}
			return (player.UtcTimeStamp - num) / 1000;
		}
		return -1L;
	}

	private bool IsBundleAvailableForStore(BundleStoreDefinition bundleStoreEntry)
	{
		if (bundleStoreEntry == null)
		{
			return false;
		}
		PlayerModel player = base.manager.Player;
		GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
		if (player != null && gameEconomyData != null)
		{
			if (bundleStoreEntry.SpenderTiers != null && bundleStoreEntry.SpenderTiers.Count > 0)
			{
				long lifeTimeInDays = player.LifeTimeInDays;
				long secondsSinceLastPurchaseThatCostMoney = GetSecondsSinceLastPurchaseThatCostMoney();
				bool flag = false;
				for (int i = 0; i < bundleStoreEntry.SpenderTiers.Count; i++)
				{
					if (gameEconomyData.IsInSpenderTier(player, bundleStoreEntry.SpenderTiers[i], player.TotalUSDSpent, (int)lifeTimeInDays, player.GetTotalPurchases(), secondsSinceLastPurchaseThatCostMoney, player.CreationTimeStamp, player.CouncilLevel))
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
			if (bundleStoreEntry.MaxPurchases >= 0 && (bundleStoreEntry.MaxPurchases == 0 || (BoughtBundlesAmount != null && BoughtBundlesAmount.ContainsKey(bundleStoreEntry.BundleIdentifier) && BoughtBundlesAmount[bundleStoreEntry.BundleIdentifier] >= bundleStoreEntry.MaxPurchases)))
			{
				return false;
			}
			bool flag2 = string.IsNullOrEmpty(bundleStoreEntry.PreviousBundle) || (BoughtBundlesAmount != null && BoughtBundlesAmount.ContainsKey(bundleStoreEntry.PreviousBundle));
			if (string.IsNullOrEmpty(bundleStoreEntry.EquivalentPreviousRotation))
			{
				if (!flag2)
				{
					return false;
				}
			}
			else
			{
				bool flag3 = RotatingBundleManager.PurchasedRotations.Contains(bundleStoreEntry.EquivalentPreviousRotation);
				if (!flag2 && !flag3)
				{
					return false;
				}
			}
			if (!flag2 && string.IsNullOrEmpty(bundleStoreEntry.EquivalentPreviousRotation))
			{
				return false;
			}
			if (bundleStoreEntry.SurvivorClassRequired != SurvivorClass.None && !player.SurvivorContainer.IsSurvivorClassUnlocked(bundleStoreEntry.SurvivorClassRequired) && !player.SurvivorContainer.IsHeroTypeUnlocked(bundleStoreEntry.SurvivorClassRequired))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(bundleStoreEntry.MapIdRequired))
			{
				MissionSpawnPoint spawnPoint = player.gameEconomyData.MissionSpawnPointData.GetSpawnPoint(bundleStoreEntry.MapIdRequired, bundleStoreEntry.MissionIndexRequired);
				if (spawnPoint != null && !player.MapContainerModel.IsMissionCompleted(spawnPoint))
				{
					return false;
				}
			}
			if (bundleStoreEntry.HasDateLimit && (player.UtcTimeStamp < bundleStoreEntry.StartTimeMilliseconds || player.UtcTimeStamp > GetBundleStoreEndTimeConsideringCheats(bundleStoreEntry)))
			{
				return false;
			}
			if (!bundleStoreEntry.IsPartOfRotation && bundleStoreEntry.MinTimeFromLastCategoryBought > 0 && !string.IsNullOrEmpty(bundleStoreEntry.BundleIdentifier))
			{
				BundleContentDefinition bundleContentDefinition = gameEconomyData.GetBundleContentDefinition(bundleStoreEntry.BundleIdentifier);
				if (bundleContentDefinition != null && !string.IsNullOrEmpty(bundleContentDefinition.Category))
				{
					long lastCategoryBoughtTimestamp = GetLastCategoryBoughtTimestamp(bundleContentDefinition.Category);
					if (lastCategoryBoughtTimestamp > 0)
					{
						return (player.UtcTimeStamp - lastCategoryBoughtTimestamp) / 1000 > bundleStoreEntry.MinTimeFromLastCategoryBought;
					}
				}
			}
			return true;
		}
		return false;
	}

	private long GetLastCategoryBoughtTimestamp(string category)
	{
		long num = 0L;
		if (!string.IsNullOrEmpty(category) && BoughtBundlesLastPurchaseTime != null)
		{
			foreach (KeyValuePair<string, long> item in BoughtBundlesLastPurchaseTime)
			{
				BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(item.Key);
				if (bundleStoreDefinition != null)
				{
					BundleContentDefinition bundleContentDefinition = base.manager.Player.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
					if (bundleContentDefinition != null && bundleContentDefinition.Category == category && num < item.Value)
					{
						num = item.Value;
					}
				}
			}
		}
		return num;
	}

	private bool HasLimitedBundleDefinitionChanged(LimitedBundleData bundleData)
	{
		if (base.manager != null && base.manager.Player != null && bundleData != null)
		{
			BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(bundleData.BundleID);
			if (bundleStoreDefinition != null && !(bundleStoreDefinition.StartTimestamp != bundleData.StartTimestamp) && !(bundleStoreDefinition.EndTimestamp != bundleData.EndTimestamp) && bundleStoreDefinition.AvailabilityTime == bundleData.AvailabilityTime)
			{
				return bundleStoreDefinition.MinTimeFromLastCategoryBought != bundleData.MinTimeFromLastCategoryBought;
			}
			return true;
		}
		return false;
	}

	public LimitedBundleData GetInitiatedLimitedBundle(string bundleID)
	{
		if (InitiatedLimitedBundles == null)
		{
			return null;
		}
		for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
		{
			LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
			if (limitedBundleData.BundleID == bundleID)
			{
				return limitedBundleData;
			}
		}
		return null;
	}

	public BundleStoreDefinition GetBundleStoreDefinitionToShowInPromo(double secondsSinceLastOpened)
	{
		List<BundleStoreDefinition> orderedStoreBundles = base.manager.Player.gameEconomyData.GetOrderedStoreBundles(base.manager.Player.UtcTimeStamp);
		for (int i = 0; i < orderedStoreBundles.Count; i++)
		{
			BundleStoreDefinition bundleStoreDefinition = orderedStoreBundles[i];
			if (bundleStoreDefinition.ShowOfferPopup && CanBuyBundle(bundleStoreDefinition) && (secondsSinceLastOpened == -1.0 || (double)bundleStoreDefinition.PopupCooldownTimer <= secondsSinceLastOpened))
			{
				return bundleStoreDefinition;
			}
		}
		return null;
	}

	public List<BundleStoreDefinition> GetOrderedAvailableBundles()
	{
		if (base.manager != null && base.manager.Player != null)
		{
			PlayerModel player = base.manager.Player;
			List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
			List<BundleStoreDefinition> orderedStoreBundles = player.gameEconomyData.GetOrderedStoreBundles(player.UtcTimeStamp);
			for (int i = 0; i < orderedStoreBundles.Count; i++)
			{
				BundleStoreDefinition bundleStoreDefinition = orderedStoreBundles[i];
				if (CanBuyBundle(bundleStoreDefinition))
				{
					list.Add(bundleStoreDefinition);
				}
			}
			return list;
		}
		return null;
	}

	public List<BundleStoreDefinition> GetOrderedAvailableBundlesWithShopTabIndex(int shopTabIndex)
	{
		if (base.manager != null && base.manager.Player != null)
		{
			PlayerModel player = base.manager.Player;
			List<BundleStoreDefinition> list = new List<BundleStoreDefinition>();
			List<BundleStoreDefinition> orderedStoreBundles = player.gameEconomyData.GetOrderedStoreBundles(player.UtcTimeStamp);
			for (int i = 0; i < orderedStoreBundles.Count; i++)
			{
				BundleStoreDefinition bundleStoreDefinition = orderedStoreBundles[i];
				if (bundleStoreDefinition != null && bundleStoreDefinition.ShopTabIndex == shopTabIndex && CanBuyBundle(bundleStoreDefinition))
				{
					list.Add(bundleStoreDefinition);
				}
			}
			return list;
		}
		return null;
	}

	public bool BuyBundle(BundleStoreDefinition bundleDefinition, bool givenBySupport = false, Metrics.BundleSource bundleSource = Metrics.BundleSource.Unknown, long supportGivenTimestamp = 0L, string supportEntityGuid = null, string metricsResourceChangeObtainReason = "")
	{
		if (bundleDefinition != null)
		{
			if (bundleSource != Metrics.BundleSource.Banana && bundleSource != Metrics.BundleSource.Subscription && bundleSource != Metrics.BundleSource.TradeFairPay && bundleSource != Metrics.BundleSource.IAPBundleBanana)
			{
				PendingViewBundleWasGivenBySupport = givenBySupport;
				PendingViewBundleStoreDefinition = bundleDefinition.BundleIdentifier;
			}
			BundleContentDefinition bundleContentDefinition = base.manager.GameEconomyData.GetBundleContentDefinition(bundleDefinition.BundleIdentifier);
			if (bundleContentDefinition != null)
			{
				if (bundleSource != Metrics.BundleSource.Banana && bundleSource != Metrics.BundleSource.Subscription && bundleSource != Metrics.BundleSource.TradeFairPay && bundleSource != Metrics.BundleSource.IAPBundleBanana)
				{
					PendingViewBundleContentDefinition = bundleContentDefinition.Identifier;
				}
				if (!string.IsNullOrEmpty(bundleContentDefinition.IAPProduct))
				{
					LastPurchaseUTCTime = base.manager.Player.UtcTimeStamp;
					if (FirstPurchaseUTCTime == 0L)
					{
						FirstPurchaseUTCTime = LastPurchaseUTCTime;
						base.manager.TdUserMetrics.SetEventType("first_pay_time").AddProperty("first_pay_time", DateTime.UtcNow).SendUser();
					}
					base.manager.TdUserMetrics.SetEventType("last_pay_time").AddProperty("last_pay_time", DateTime.UtcNow).SendUser();
				}
			}
			if (bundleSource != Metrics.BundleSource.Banana && bundleSource != Metrics.BundleSource.Subscription)
			{
				string key = bundleDefinition.BundleIdentifier;
				if (bundleSource == Metrics.BundleSource.IAPBundleBanana && bundleDefinition.BundleIdentifier.EndsWith("_WB"))
				{
					key = bundleDefinition.BundleIdentifier.Substring(0, bundleDefinition.BundleIdentifier.Length - 3);
				}
				if (BoughtBundlesAmount == null)
				{
					BoughtBundlesAmount = new Dictionary<string, int>();
				}
				if (BoughtBundlesAmount.ContainsKey(key))
				{
					BoughtBundlesAmount[key] += 1;
				}
				else
				{
					BoughtBundlesAmount.Add(key, 1);
				}
				if (BoughtBundlesLastPurchaseTime == null)
				{
					BoughtBundlesLastPurchaseTime = new Dictionary<string, long>();
				}
				if (BoughtBundlesLastPurchaseTime.ContainsKey(key))
				{
					BoughtBundlesLastPurchaseTime[key] = base.manager.Player.UtcTimeStamp;
				}
				else
				{
					BoughtBundlesLastPurchaseTime.Add(key, base.manager.Player.UtcTimeStamp);
				}
			}
			BundleContentDefinition bundleContentDefinition2 = base.manager.Player.gameEconomyData.GetBundleContentDefinition(bundleDefinition.BundleIdentifier);
			bundleContentDefinition2 = ExtraGiftIfBundleHasPriceRangeGift(bundleDefinition, bundleContentDefinition2, bundleSource);
			if (bundleContentDefinition2 != null && bundleContentDefinition2.RewardEntries != null)
			{
				if (bundleContentDefinition2.RewardEntries != null && bundleContentDefinition2.RewardEntries.RewardsList != null && bundleContentDefinition2.RewardEntries.RewardsList.Count > 0)
				{
					Metrics.MetricsResourcesData metricsResourcesData = new Metrics.MetricsResourcesData();
					for (int i = 0; i < bundleContentDefinition2.RewardEntries.RewardsList.Count; i++)
					{
						IReward reward = bundleContentDefinition2.RewardEntries.RewardsList[i];
						if (reward.Type == RewardType.Equipment || reward.Type == RewardType.RandomEquipment)
						{
							if (PendingViewEquipments == null)
							{
								PendingViewEquipments = new ModelList<EquipmentItemModel>();
							}
							EquipmentItemModel equipmentItemModel = reward.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom }) as EquipmentItemModel;
							RewardEquipment rewardEquipment = reward as RewardEquipment;
							if (equipmentItemModel != null)
							{
								if (givenBySupport)
								{
									base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel, "Equipment", rewardEquipment?.Amount ?? 1).AddBundle(bundleDefinition, bundleSource)
										.AddSupport(supportGivenTimestamp, supportEntityGuid)
										.Send();
								}
								else
								{
									base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel, "Equipment", rewardEquipment?.Amount ?? 1).AddBundle(bundleDefinition, bundleSource)
										.Send();
								}
								if (!equipmentItemModel.IsConsumable && bundleSource != Metrics.BundleSource.Banana)
								{
									PendingViewEquipments.Add(equipmentItemModel);
								}
							}
							continue;
						}
						if (reward.Type == RewardType.Outfit)
						{
							if (PendingViewOutfits == null)
							{
								PendingViewOutfits = new List<string>();
							}
							string text = reward.Give(base.manager) as string;
							if (!string.IsNullOrEmpty(text))
							{
								if (givenBySupport)
								{
									base.manager.Metrics.AddFind().AddOutfit(base.gameEconomyData.GetOutfitDefinition(text)).AddBundle(bundleDefinition, bundleSource)
										.AddSupport(supportGivenTimestamp, supportEntityGuid)
										.Send();
								}
								else
								{
									base.manager.Metrics.AddFind().AddOutfit(base.gameEconomyData.GetOutfitDefinition(text)).AddBundle(bundleDefinition, bundleSource)
										.Send();
								}
								if (bundleSource != Metrics.BundleSource.Banana)
								{
									PendingViewOutfits.Add(text);
								}
							}
							continue;
						}
						if (reward.Type == RewardType.HeroSkin)
						{
							if (PendingViewHeroSkins == null)
							{
								PendingViewHeroSkins = new List<string>();
							}
							string text2 = reward.Give(base.manager) as string;
							if (!string.IsNullOrEmpty(text2) && bundleSource != Metrics.BundleSource.Banana)
							{
								PendingViewHeroSkins.Add(text2);
							}
							continue;
						}
						if (reward.Type == RewardType.EquipToken)
						{
							if (PendingViewEquipTokens == null)
							{
								PendingViewEquipTokens = new ModelList<EquipTokenItemModel>();
							}
							if (reward.Give(base.manager) is EquipTokenItemModel model)
							{
								PendingViewEquipTokens.Add(model);
							}
							continue;
						}
						reward.Give(base.manager);
						if (bundleContentDefinition == null)
						{
							continue;
						}
						if (reward is RewardCurrency)
						{
							RewardCurrency rewardCurrency = reward as RewardCurrency;
							if (rewardCurrency.Amount > 0)
							{
								base.manager.Player.GetCurrency(rewardCurrency.CurrencyType).AddBought(rewardCurrency.Amount);
								metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount());
							}
							else if (rewardCurrency.Amount == -1)
							{
								metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded);
							}
						}
						else if (reward is RewardSurvivorSlot)
						{
							if (givenBySupport)
							{
								base.manager.Metrics.AddFind().AddSurvivorSlot().AddBundle(bundleDefinition, bundleSource)
									.AddSupport(supportGivenTimestamp, supportEntityGuid)
									.Send();
							}
							else
							{
								base.manager.Metrics.AddFind().AddSurvivorSlot().AddBundle(bundleDefinition, bundleSource)
									.Send();
							}
						}
						else if (reward is RewardTimedBonus)
						{
							if (givenBySupport)
							{
								base.manager.Metrics.AddFind().AddTimedBonus(reward as RewardTimedBonus).AddBundle(bundleDefinition, bundleSource)
									.AddSupport(supportGivenTimestamp, supportEntityGuid)
									.Send();
							}
							else
							{
								base.manager.Metrics.AddFind().AddTimedBonus(reward as RewardTimedBonus).AddBundle(bundleDefinition, bundleSource)
									.Send();
							}
						}
					}
					if (metricsResourcesData.HasResources())
					{
						if (string.IsNullOrEmpty(metricsResourceChangeObtainReason))
						{
							base.manager.Metrics.ResourceChangeObtainReason = "buy_bundle";
						}
						else
						{
							base.manager.Metrics.ResourceChangeObtainReason = metricsResourceChangeObtainReason;
						}
						base.manager.Metrics.ResourceChangeIsByCharging = "1";
						bool freeResource = bundleContentDefinition != null && string.IsNullOrEmpty(bundleContentDefinition.IAPProduct);
						if (givenBySupport)
						{
							base.manager.Metrics.AddFind().AddResources(metricsResourcesData, freeResource, combineDuplicates: true).AddBundle(bundleDefinition, bundleSource)
								.AddSupport(supportGivenTimestamp, supportEntityGuid)
								.Send();
						}
						else
						{
							base.manager.Metrics.AddFind().AddResources(metricsResourcesData, freeResource, combineDuplicates: true).AddBundle(bundleDefinition, bundleSource)
								.Send();
						}
					}
				}
				if (bundleSource != Metrics.BundleSource.Banana && bundleSource != Metrics.BundleSource.Subscription && bundleSource != Metrics.BundleSource.TradeFairPay)
				{
					InAppPurchaseProductApple inAppPurchaseProduct = base.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct);
					if (inAppPurchaseProduct != null && inAppPurchaseProduct.PriceUSD >= (float)base.gameEconomyData.ConfigData.MinIAPPriceGift && base.manager.Player.Combat == null)
					{
						IAPBonusGiftLootEntry = base.manager.Player.LootManager.ShuffleOneLootWithoutTag(new LootEntryGenParams
						{
							eventType = DropEventDefinition.DropEventType.IAPBonusGift,
							targetLevel = base.manager.Player.Level,
							context = DropEventDefinition.DropEventContext.Normal,
							dropType = DropType.Gold
						});
						IAPBonusGiftLootEntry.Type = LootEntryType.IAPBonusGift;
					}
					CheckForNewBundlesTimer = 0L;
					RotatingBundleManager.LimitedBundlePurchased(bundleDefinition.BundleIdentifier);
				}
				else if (bundleSource != Metrics.BundleSource.Subscription && bundleSource != Metrics.BundleSource.TradeFairPay)
				{
					InAppPurchaseProductApple inAppPurchaseProduct2 = base.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct);
					if (inAppPurchaseProduct2 != null && inAppPurchaseProduct2.PriceUSD >= (float)base.gameEconomyData.ConfigData.MinIAPPriceGift)
					{
						LootEntry lootEntry = base.manager.Player.LootManager.ShuffleOneLootWithoutTag(new LootEntryGenParams
						{
							eventType = DropEventDefinition.DropEventType.IAPBonusGift,
							targetLevel = base.manager.Player.Level,
							context = DropEventDefinition.DropEventContext.Normal,
							dropType = DropType.Gold
						});
						lootEntry.Type = LootEntryType.IAPBonusGift;
						WebShopLootEntrys.Add(lootEntry);
					}
				}
				return true;
			}
			base.Debug.LogError("BuyBundle failed: invalid bundleContentDefinition");
			return false;
		}
		base.Debug.LogError("BuyBundle failed: invalid bundleDefinition");
		if (bundleSource != Metrics.BundleSource.Banana && bundleSource != Metrics.BundleSource.Subscription)
		{
			LimitedBundleData initiatedLimitedBundle = GetInitiatedLimitedBundle(bundleDefinition.BundleIdentifier);
			if (initiatedLimitedBundle != null && InitiatedLimitedBundles.Contains(initiatedLimitedBundle))
			{
				initiatedLimitedBundle.IsAvailable = false;
				initiatedLimitedBundle.Timer = initiatedLimitedBundle.MinTimeFromLastCategoryBought * 1000;
				BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(initiatedLimitedBundle.BundleID);
				if (bundleStoreDefinition != null && !IsBundleAvailableForStore(bundleStoreDefinition))
				{
					InitiatedLimitedBundles.Remove(initiatedLimitedBundle);
				}
			}
		}
		return false;
	}

	public bool GiveRewardsGivenBySupport(string rewardString, long supportGivenTimestamp = 0L, string supportEntityGuid = null)
	{
		PendingViewBundleWasGivenBySupport = true;
		PendingViewRewardsGivenBySupport = rewardString;
		PendingViewBundleContentDefinition = FAKE_SUPPORT_BUNDLE_FOR_REWARDS;
		PendingViewBundleStoreDefinition = FAKE_SUPPORT_BUNDLE_FOR_REWARDS;
		Rewards rewards = new Rewards(rewardString);
		for (int i = 0; i < rewards.RewardsList.Count; i++)
		{
			IReward reward = rewards.RewardsList[i];
			if (reward.Type == RewardType.Equipment || reward.Type == RewardType.RandomEquipment)
			{
				if (PendingViewEquipments == null)
				{
					PendingViewEquipments = new ModelList<EquipmentItemModel>();
				}
				EquipmentItemModel equipmentItemModel = reward.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom }) as EquipmentItemModel;
				RewardEquipment rewardEquipment = reward as RewardEquipment;
				if (equipmentItemModel != null)
				{
					if (!equipmentItemModel.IsConsumable)
					{
						PendingViewEquipments.Add(equipmentItemModel);
					}
					base.manager.Metrics.AddFind().AddEquipment(equipmentItemModel, "Equipment", rewardEquipment?.Amount ?? 1).AddSupport(supportGivenTimestamp, supportEntityGuid)
						.Send();
				}
				continue;
			}
			if (reward.Type == RewardType.Outfit)
			{
				if (PendingViewOutfits == null)
				{
					PendingViewOutfits = new List<string>();
				}
				string text = reward.Give(base.manager) as string;
				if (!string.IsNullOrEmpty(text))
				{
					PendingViewOutfits.Add(text);
					base.manager.Metrics.AddFind().AddOutfit(base.gameEconomyData.GetOutfitDefinition(text)).AddSupport(supportGivenTimestamp, supportEntityGuid)
						.Send();
				}
				continue;
			}
			if (reward.Type == RewardType.HeroSkin)
			{
				if (PendingViewHeroSkins == null)
				{
					PendingViewHeroSkins = new List<string>();
				}
				string text2 = reward.Give(base.manager) as string;
				if (!string.IsNullOrEmpty(text2))
				{
					PendingViewHeroSkins.Add(text2);
				}
				continue;
			}
			if (reward.Type == RewardType.EquipToken)
			{
				if (PendingViewEquipTokens == null)
				{
					PendingViewEquipTokens = new ModelList<EquipTokenItemModel>();
				}
				if (reward.Give(base.manager) is EquipTokenItemModel model)
				{
					PendingViewEquipTokens.Add(model);
				}
				continue;
			}
			reward.Give(base.manager);
			if (reward is RewardCurrency)
			{
				RewardCurrency rewardCurrency = reward as RewardCurrency;
				if (rewardCurrency.Amount > 0)
				{
					base.manager.Metrics.AddFind().AddResources(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount()).AddSupport(supportGivenTimestamp, supportEntityGuid)
						.Send();
				}
			}
		}
		return true;
	}

	public void SetPendingViewDefinitionId(string definitionId)
	{
		PendingViewBundleContentDefinition = definitionId;
	}

	private LimitedBundleData GetHighestPriorityLimitedBundle(bool isAvailable)
	{
		List<LimitedBundleData> list = new List<LimitedBundleData>();
		if (InitiatedLimitedBundles != null && InitiatedLimitedBundles.Count > 0)
		{
			for (int i = 0; i < InitiatedLimitedBundles.Count; i++)
			{
				LimitedBundleData limitedBundleData = InitiatedLimitedBundles[i];
				if (limitedBundleData != null && isAvailable == limitedBundleData.IsAvailable)
				{
					list.Add(limitedBundleData);
				}
			}
		}
		if (list.Count > 0)
		{
			list.StableSort(delegate(LimitedBundleData a, LimitedBundleData b)
			{
				if (a == null || b == null)
				{
					return 0;
				}
				BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(a.BundleID);
				BundleStoreDefinition bundleStoreDefinition2 = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(b.BundleID);
				if (bundleStoreDefinition == null || bundleStoreDefinition2 == null)
				{
					return 0;
				}
				if (bundleStoreDefinition.DisplayOrder < bundleStoreDefinition2.DisplayOrder)
				{
					return -1;
				}
				return (bundleStoreDefinition.DisplayOrder > bundleStoreDefinition2.DisplayOrder) ? 1 : 0;
			});
			return list[0];
		}
		return null;
	}

	private long GetBundleStoreEndTimeConsideringCheats(BundleStoreDefinition bundleStoreDefinition)
	{
		if (bundleStoreDefinition != null)
		{
			if (InitiatedCheatOffers != null)
			{
				for (int i = 0; i < InitiatedCheatOffers.Count; i++)
				{
					CheatOfferEndTime cheatOfferEndTime = InitiatedCheatOffers[i];
					if (cheatOfferEndTime != null && cheatOfferEndTime.OfferID == bundleStoreDefinition.BundleIdentifier)
					{
						return cheatOfferEndTime.OfferEndTimestamp;
					}
				}
			}
			return bundleStoreDefinition.EndTimeMilliseconds;
		}
		return 0L;
	}

	public void DEBUG_forceHighestPriorityOfferState(bool forceAvailable, long availableTimer = 5000L)
	{
		LimitedBundleData highestPriorityLimitedBundle = GetHighestPriorityLimitedBundle(!forceAvailable);
		if (highestPriorityLimitedBundle != null)
		{
			highestPriorityLimitedBundle.Timer = availableTimer;
		}
	}

	public void DEBUG_forceHighestPriorityOfferEndTimestamp(long msDelay = 10000L)
	{
		LimitedBundleData highestPriorityLimitedBundle = GetHighestPriorityLimitedBundle(isAvailable: true);
		if (highestPriorityLimitedBundle == null)
		{
			return;
		}
		BundleStoreDefinition bundleStoreDefinition = base.manager.Player.gameEconomyData.GetBundleStoreDefinition(highestPriorityLimitedBundle.BundleID);
		if (bundleStoreDefinition == null)
		{
			return;
		}
		if (InitiatedCheatOffers == null)
		{
			InitiatedCheatOffers = new List<CheatOfferEndTime>();
		}
		List<CheatOfferEndTime> list = new List<CheatOfferEndTime>();
		for (int i = 0; i < InitiatedCheatOffers.Count; i++)
		{
			CheatOfferEndTime cheatOfferEndTime = InitiatedCheatOffers[i];
			if (cheatOfferEndTime != null && cheatOfferEndTime.OfferID == bundleStoreDefinition.BundleIdentifier)
			{
				list.Add(cheatOfferEndTime);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			InitiatedCheatOffers.Remove(list[j]);
		}
		CheatOfferEndTime cheatOfferEndTime2 = new CheatOfferEndTime();
		cheatOfferEndTime2.OfferID = bundleStoreDefinition.BundleIdentifier;
		cheatOfferEndTime2.OfferEndTimestamp = base.manager.Player.UtcTimeStamp + msDelay;
		InitiatedCheatOffers.Add(cheatOfferEndTime2);
	}

	public BundleContentDefinition ExtraGiftIfBundleHasPriceRangeGift(BundleStoreDefinition sourceBundleStoreDefinition, BundleContentDefinition sourceBundleContentDefinition, Metrics.BundleSource bundleSource = Metrics.BundleSource.Unknown)
	{
		if (bundleSource != Metrics.BundleSource.IAPBundleBanana)
		{
			return sourceBundleContentDefinition;
		}
		if (!sourceBundleStoreDefinition.BundleIdentifier.EndsWith("_WB"))
		{
			return sourceBundleContentDefinition;
		}
		if (!base.gameEconomyData.ConfigData.IsPriceRangeEnabled)
		{
			return sourceBundleContentDefinition;
		}
		string identifier = sourceBundleContentDefinition.Identifier.Substring(0, sourceBundleContentDefinition.Identifier.Length - 3);
		BundleContentDefinition bundleContentDefinition = base.manager.Player.gameEconomyData.GetBundleContentDefinition(identifier);
		InAppPurchaseProductApple inAppPurchaseProduct = base.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct);
		if (!base.gameEconomyData.ConfigData.IsPriceInRange(inAppPurchaseProduct.PriceUSD))
		{
			return sourceBundleContentDefinition;
		}
		BundleContentDefinition bundleContentDefinition2 = CloneBundleContentDefinition(sourceBundleContentDefinition);
		Rewards extraGiftRewards = base.gameEconomyData.ConfigData.GetExtraGiftRewards();
		bundleContentDefinition2.RewardEntries.Add(extraGiftRewards);
		return bundleContentDefinition2;
	}

	public BundleContentDefinition CloneBundleContentDefinition(BundleContentDefinition source)
	{
		if (source == null)
		{
			return null;
		}
		return new BundleContentDefinition
		{
			Identifier = source.Identifier,
			IAPProduct = source.IAPProduct,
			EpicOfferID = source.EpicOfferID,
			SteamOfferID = source.SteamOfferID,
			TradefairPrice = source.TradefairPrice,
			IsAPP = source.IsAPP,
			IsEpic = source.IsEpic,
			IsSteam = source.IsSteam,
			IsTradeFair = source.IsTradeFair,
			IsThirdParty = source.IsThirdParty,
			Category = source.Category,
			Rewards = source.Rewards,
			RewardsPrefabsData = ((source.RewardsPrefabsData != null) ? new List<string>(source.RewardsPrefabsData) : null),
			RewardsExtraLocalization = ((source.RewardsExtraLocalization != null) ? new List<string>(source.RewardsExtraLocalization) : null),
			RewardsImageURL = ((source.RewardsImageURL != null) ? new List<string>(source.RewardsImageURL) : null),
			StrikePricePercentage = source.StrikePricePercentage,
			TradeFairPriceNew = source.TradeFairPriceNew,
			BundleType = source.BundleType,
			Classification = source.Classification,
			RewardEntries = ((!string.IsNullOrEmpty(source.Rewards) && base.manager != null) ? new Rewards(source.Rewards, base.manager) : null)
		};
	}
}
