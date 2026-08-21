using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnExchangeStoreModel : TWDModelObject
	{
		public const string ReturnExchangeStoreChanged = "ReturnExchangeStoreChanged";

		private const int DefaultRefreshIntervalSeconds = 172800;

		private const long MillisecondsPerSecond = 1000L;

		private static readonly List<ReturnExchangeStoreDefinition> EmptyDefinitions = new List<ReturnExchangeStoreDefinition>();

		private bool _isRefreshStateInitialized;

		public Dictionary<int, int> ExchangeBoughtCounts { get; set; }

		public Dictionary<int, int> SlotManualRefreshCounts { get; set; }

		public List<int> ActiveRefreshExchangeIds { get; set; }

		public long NextRefreshTimestamp { get; set; }

		[JsonIgnore]
		public List<ReturnExchangeStoreDefinition> ExchangeDefinitions
		{
			get
			{
				EnsureStorageInitialized();
				List<ReturnExchangeStoreDefinition> configuredDefinitions = GetConfiguredDefinitions();
				List<ReturnExchangeStoreDefinition> list = new List<ReturnExchangeStoreDefinition>(configuredDefinitions.Count);
				int num = 0;
				for (int i = 0; i < configuredDefinitions.Count; i++)
				{
					ReturnExchangeStoreDefinition returnExchangeStoreDefinition = configuredDefinitions[i];
					if (returnExchangeStoreDefinition == null || returnExchangeStoreDefinition.Type != ReturnExchangeStoreType.Refresh)
					{
						list.Add(returnExchangeStoreDefinition);
						continue;
					}
					if (num < ActiveRefreshExchangeIds.Count)
					{
						ReturnExchangeStoreDefinition returnExchangeStoreDefinition2 = FindDefinitionById(ActiveRefreshExchangeIds[num]);
						if (returnExchangeStoreDefinition2 != null)
						{
							list.Add(returnExchangeStoreDefinition2);
						}
					}
					num++;
				}
				return list;
			}
		}

		[JsonIgnore]
		public bool HasRedDot => false;

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			EnsureStorageInitialized();
		}

		public override void Start()
		{
			EnsureStorageInitialized();
			base.Start();
			EnsureRefreshStateInitialized();
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			InitializeNextRefreshTimestamp(valueOrDefault);
			TryAutoRefresh(valueOrDefault);
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (TryAutoRefresh((base.manager?.Player?.UtcTimeStamp).GetValueOrDefault()))
			{
				NotifyChange("ReturnExchangeStoreChanged");
			}
		}

		public void ResetForNewActivity(long currentTimestamp)
		{
			ExchangeBoughtCounts.Clear();
			SlotManualRefreshCounts?.Clear();
			ActiveRefreshExchangeIds?.Clear();
			NextRefreshTimestamp = 0L;
			_isRefreshStateInitialized = false;
			EnsureRefreshStateInitialized();
			InitializeNextRefreshTimestamp(currentTimestamp);
			NotifyChange("ReturnExchangeStoreChanged");
		}

		public int GetBoughtCount(int exchangeId)
		{
			if (!ExchangeBoughtCounts.TryGetValue(exchangeId, out var value))
			{
				return 0;
			}
			return value;
		}

		public int GetRemainingCount(int exchangeId)
		{
			ReturnExchangeStoreDefinition activeExchangeDefinition = GetActiveExchangeDefinition(exchangeId);
			return GetRemainingCount(exchangeId, activeExchangeDefinition);
		}

		private int GetRemainingCount(int exchangeId, ReturnExchangeStoreDefinition definition)
		{
			if (definition == null)
			{
				return 0;
			}
			if (IsRefreshDefinition(definition))
			{
				return Math.Max(1 - GetBoughtCount(exchangeId), 0);
			}
			if (definition.Limit < 0)
			{
				return -1;
			}
			return Math.Max(definition.Limit - GetBoughtCount(exchangeId), 0);
		}

		public bool CanExchange(int exchangeId)
		{
			TWDModelManager tWDModelManager = base.manager;
			if (tWDModelManager == null || tWDModelManager.Player?.ReturnActivityManager?.IsReturnExchangeAvailable() != true)
			{
				return false;
			}
			ReturnExchangeStoreDefinition activeExchangeDefinition = GetActiveExchangeDefinition(exchangeId);
			if (activeExchangeDefinition == null || GetRemainingCount(exchangeId, activeExchangeDefinition) == 0 || !HasValidCost(activeExchangeDefinition))
			{
				return false;
			}
			return BuildCashier(activeExchangeDefinition).CanAfford();
		}

		public bool CanManualRefresh(int exchangeId)
		{
			TWDModelManager tWDModelManager = base.manager;
			if (tWDModelManager == null || tWDModelManager.Player?.ReturnActivityManager?.IsReturnExchangeAvailable() != true)
			{
				return false;
			}
			if (GetBoughtCount(exchangeId) > 0)
			{
				return false;
			}
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			long num = (long)(base.gameEconomyData?.ReturnConfig?.ManualRefreshBanTime).GetValueOrDefault() * 1000L;
			if (num > 0 && NextRefreshTimestamp > valueOrDefault && NextRefreshTimestamp - valueOrDefault <= num)
			{
				return false;
			}
			return HasRefreshCandidate(exchangeId);
		}

		public TWDModelResult Exchange(int exchangeId)
		{
			TWDModelManager tWDModelManager = base.manager;
			if (tWDModelManager == null || tWDModelManager.Player?.ReturnActivityManager?.IsReturnExchangeAvailable() != true)
			{
				return TWDModelResult.Error;
			}
			ReturnExchangeStoreDefinition activeExchangeDefinition = GetActiveExchangeDefinition(exchangeId);
			if (activeExchangeDefinition == null || GetRemainingCount(exchangeId, activeExchangeDefinition) == 0 || !HasValidCost(activeExchangeDefinition))
			{
				return TWDModelResult.Error;
			}
			Cashier cashier = BuildCashier(activeExchangeDefinition);
			cashier.UseDiamondsAmount = -2;
			TWDModelResult tWDModelResult = cashier.Pay();
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			activeExchangeDefinition.RewardEntries?.Give(base.manager);
			ExchangeBoughtCounts[exchangeId] = GetBoughtCount(exchangeId) + 1;
			ReturnerAnalytics.SendExchange(base.manager, activeExchangeDefinition);
			NotifyChange("ReturnExchangeStoreChanged");
			return TWDModelResult.OK;
		}

		public bool TryManualRefresh(int exchangeId)
		{
			TWDModelManager tWDModelManager = base.manager;
			if (tWDModelManager == null || tWDModelManager.Player?.ReturnActivityManager?.IsReturnExchangeAvailable() != true)
			{
				return false;
			}
			if (GetBoughtCount(exchangeId) > 0)
			{
				return false;
			}
			long valueOrDefault = (base.manager?.Player?.UtcTimeStamp).GetValueOrDefault();
			long num = (long)(base.gameEconomyData?.ReturnConfig?.ManualRefreshBanTime).GetValueOrDefault() * 1000L;
			if (num > 0 && NextRefreshTimestamp > valueOrDefault && NextRefreshTimestamp - valueOrDefault <= num)
			{
				return false;
			}
			int num2 = -1;
			if (ActiveRefreshExchangeIds != null)
			{
				for (int i = 0; i < ActiveRefreshExchangeIds.Count; i++)
				{
					if (ActiveRefreshExchangeIds[i] == exchangeId)
					{
						num2 = i;
						break;
					}
				}
			}
			if (num2 < 0)
			{
				return false;
			}
			if (!HasRefreshCandidate(exchangeId))
			{
				base.manager?.Debug?.LogWarning($"[ReturnExchangeStore] Manual refresh aborted for exchangeId={exchangeId}: no replacement candidate available (all configured Refresh items are already active). Check RefreshSlotNum against the number of Refresh definitions for this council level.");
				return false;
			}
			(int, CurrencyType) manualRefreshCost = GetManualRefreshCost(num2);
			if (manualRefreshCost.Item1 > 0)
			{
				Cashier cashier = new Cashier(base.manager);
				CashierItem cashierItem = new CashierItem(PurchaseType.ReturnExchangeStore);
				cashierItem.SetCost(manualRefreshCost.Item2, manualRefreshCost.Item1);
				cashier.AddItem(cashierItem);
				cashier.UseDiamondsAmount = -2;
				if (!cashier.CanAfford() || cashier.Pay() != TWDModelResult.OK)
				{
					return false;
				}
			}
			bool num3 = RefreshSingleExchangeContent(exchangeId);
			if (num3)
			{
				if (SlotManualRefreshCounts == null)
				{
					SlotManualRefreshCounts = new Dictionary<int, int>();
				}
				if (SlotManualRefreshCounts.TryGetValue(num2, out var value))
				{
					SlotManualRefreshCounts[num2] = value + 1;
				}
				else
				{
					SlotManualRefreshCounts[num2] = 1;
				}
				NotifyChange("ReturnExchangeStoreChanged");
			}
			return num3;
		}

		public (int amount, CurrencyType currency) GetManualRefreshCost(int slotIndex)
		{
			int val = 0;
			if (SlotManualRefreshCounts != null && SlotManualRefreshCounts.TryGetValue(slotIndex, out var value))
			{
				val = value;
			}
			string text = base.gameEconomyData?.ReturnConfig?.ReturnExchangeStoreRefreshSlotSpend;
			if (string.IsNullOrEmpty(text))
			{
				return (amount: 0, currency: CurrencyType.Diamonds);
			}
			string[] array = text.Split(';');
			int num = Math.Min(val, array.Length - 1);
			return HelpersModel.ParsePrice(array[num]);
		}

		private Cashier BuildCashier(ReturnExchangeStoreDefinition definition)
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.ReturnExchangeStore);
			if (definition?.CostRewardEntries?.RewardsList != null)
			{
				for (int i = 0; i < definition.CostRewardEntries.RewardsList.Count; i++)
				{
					if (definition.CostRewardEntries.RewardsList[i] is RewardCurrency rewardCurrency)
					{
						cashierItem.SetCost(rewardCurrency.CurrencyType, rewardCurrency.Amount);
					}
				}
			}
			cashier.AddItem(cashierItem);
			return cashier;
		}

		private bool HasValidCost(ReturnExchangeStoreDefinition definition)
		{
			if (definition?.CostRewardEntries?.RewardsList == null)
			{
				return false;
			}
			for (int i = 0; i < definition.CostRewardEntries.RewardsList.Count; i++)
			{
				if (definition.CostRewardEntries.RewardsList[i] is RewardCurrency { Amount: >0 })
				{
					return true;
				}
			}
			return false;
		}

		private void EnsureStorageInitialized()
		{
			if (ExchangeBoughtCounts == null)
			{
				ExchangeBoughtCounts = new Dictionary<int, int>();
			}
			if (SlotManualRefreshCounts == null)
			{
				SlotManualRefreshCounts = new Dictionary<int, int>();
			}
			if (ActiveRefreshExchangeIds == null)
			{
				ActiveRefreshExchangeIds = new List<int>();
			}
		}

		private void EnsureRefreshStateInitialized()
		{
			if (_isRefreshStateInitialized)
			{
				return;
			}
			List<ReturnExchangeStoreDefinition> configuredRefreshDefinitions = GetConfiguredRefreshDefinitions();
			if (configuredRefreshDefinitions.Count == 0)
			{
				ActiveRefreshExchangeIds.Clear();
				_isRefreshStateInitialized = true;
				return;
			}
			if (!HasValidRefreshState(configuredRefreshDefinitions))
			{
				ResetConfiguredRefreshExchangeBoughtCounts(configuredRefreshDefinitions);
				RebuildRefreshExchangeState(configuredRefreshDefinitions);
			}
			_isRefreshStateInitialized = true;
		}

		private bool TryAutoRefresh(long currentTimestamp)
		{
			if (currentTimestamp > 0)
			{
				TWDModelManager tWDModelManager = base.manager;
				if (tWDModelManager != null && tWDModelManager.Player?.ReturnActivityManager?.IsReturnExchangeAvailable() == true)
				{
					if (NextRefreshTimestamp <= 0 || currentTimestamp < NextRefreshTimestamp)
					{
						return false;
					}
					bool flag = false;
					long num = -1L;
					while (currentTimestamp >= NextRefreshTimestamp)
					{
						flag |= RefreshExchangeContent();
						if (num < 0)
						{
							num = GetRefreshIntervalMilliseconds();
						}
						NextRefreshTimestamp += num;
						flag = true;
					}
					return flag;
				}
			}
			return false;
		}

		private void InitializeNextRefreshTimestamp(long currentTimestamp)
		{
			if (NextRefreshTimestamp <= 0)
			{
				long num = (base.manager?.Player?.ReturnActivityManager?.ReturnIdentityStartTimestamp).GetValueOrDefault();
				if (num <= 0)
				{
					num = currentTimestamp;
				}
				if (num > 0)
				{
					NextRefreshTimestamp = num + GetRefreshIntervalMilliseconds();
				}
			}
		}

		private bool ResetConfiguredRefreshExchangeBoughtCounts(List<ReturnExchangeStoreDefinition> refreshDefinitions)
		{
			bool result = false;
			for (int i = 0; i < refreshDefinitions.Count; i++)
			{
				if (ExchangeBoughtCounts.Remove(refreshDefinitions[i].Id))
				{
					result = true;
				}
			}
			return result;
		}

		private bool RefreshExchangeContent()
		{
			List<ReturnExchangeStoreDefinition> configuredRefreshDefinitions = GetConfiguredRefreshDefinitions();
			if (configuredRefreshDefinitions.Count == 0)
			{
				return false;
			}
			return ResetConfiguredRefreshExchangeBoughtCounts(configuredRefreshDefinitions) | RebuildRefreshExchangeState(configuredRefreshDefinitions);
		}

		private bool HasRefreshCandidate(int exchangeId)
		{
			if (ActiveRefreshExchangeIds == null || !ActiveRefreshExchangeIds.Contains(exchangeId))
			{
				return false;
			}
			List<ReturnExchangeStoreDefinition> configuredRefreshDefinitions = GetConfiguredRefreshDefinitions();
			for (int i = 0; i < configuredRefreshDefinitions.Count; i++)
			{
				int id = configuredRefreshDefinitions[i].Id;
				if (id == exchangeId || !ActiveRefreshExchangeIds.Contains(id))
				{
					return true;
				}
			}
			return false;
		}

		private bool RefreshSingleExchangeContent(int exchangeId)
		{
			if (ActiveRefreshExchangeIds == null)
			{
				return false;
			}
			int num = -1;
			for (int i = 0; i < ActiveRefreshExchangeIds.Count; i++)
			{
				if (ActiveRefreshExchangeIds[i] == exchangeId)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				return false;
			}
			List<ReturnExchangeStoreDefinition> configuredRefreshDefinitions = GetConfiguredRefreshDefinitions();
			List<int> list = new List<int>();
			for (int j = 0; j < configuredRefreshDefinitions.Count; j++)
			{
				int id = configuredRefreshDefinitions[j].Id;
				if (id == exchangeId || !ActiveRefreshExchangeIds.Contains(id))
				{
					list.Add(id);
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			List<int> list2 = PickRefreshExchangeIds(list, 1);
			if (list2.Count > 0)
			{
				int num2 = list2[0];
				ExchangeBoughtCounts.Remove(exchangeId);
				ExchangeBoughtCounts.Remove(num2);
				ActiveRefreshExchangeIds[num] = num2;
				return true;
			}
			return false;
		}

		private List<ReturnExchangeStoreDefinition> GetConfiguredRefreshDefinitions()
		{
			List<ReturnExchangeStoreDefinition> configuredDefinitions = GetConfiguredDefinitions();
			List<ReturnExchangeStoreDefinition> list = new List<ReturnExchangeStoreDefinition>();
			for (int i = 0; i < configuredDefinitions.Count; i++)
			{
				ReturnExchangeStoreDefinition returnExchangeStoreDefinition = configuredDefinitions[i];
				if (returnExchangeStoreDefinition != null && returnExchangeStoreDefinition.Type == ReturnExchangeStoreType.Refresh)
				{
					list.Add(configuredDefinitions[i]);
				}
			}
			return list;
		}

		private int GetRefreshSlotCount(int refreshDefinitionCount)
		{
			if (refreshDefinitionCount <= 0)
			{
				return 0;
			}
			return Math.Min(Math.Max((base.gameEconomyData?.ReturnConfig?.RefreshSlotNum).GetValueOrDefault(), 0), refreshDefinitionCount);
		}

		private long GetRefreshIntervalMilliseconds()
		{
			int num = Math.Max((base.gameEconomyData?.ReturnConfig?.ReturnExchangeStoreRefreshTime).GetValueOrDefault(), 0);
			if (num <= 0)
			{
				num = 172800;
			}
			return (long)num * 1000L;
		}

		private List<ReturnExchangeStoreDefinition> GetConfiguredDefinitions()
		{
			return base.gameEconomyData?.GetReturnExchangeStoreDefinitions(GetCouncilLevelSnapshot()) ?? EmptyDefinitions;
		}

		private ReturnExchangeStoreDefinition GetActiveExchangeDefinition(int exchangeId)
		{
			ReturnExchangeStoreDefinition returnExchangeStoreDefinition = FindDefinitionById(exchangeId);
			if (returnExchangeStoreDefinition == null)
			{
				return null;
			}
			if (!IsRefreshDefinition(returnExchangeStoreDefinition))
			{
				return returnExchangeStoreDefinition;
			}
			if (ActiveRefreshExchangeIds != null && ActiveRefreshExchangeIds.Contains(exchangeId))
			{
				return returnExchangeStoreDefinition;
			}
			return null;
		}

		private bool HasValidRefreshState(List<ReturnExchangeStoreDefinition> refreshDefinitions)
		{
			if (ActiveRefreshExchangeIds == null || ActiveRefreshExchangeIds.Count != GetRefreshSlotCount(refreshDefinitions.Count))
			{
				return false;
			}
			for (int i = 0; i < ActiveRefreshExchangeIds.Count; i++)
			{
				int num = ActiveRefreshExchangeIds[i];
				bool flag = false;
				for (int j = 0; j < refreshDefinitions.Count; j++)
				{
					if (refreshDefinitions[j] != null && refreshDefinitions[j].Id == num)
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
		}

		private bool RebuildRefreshExchangeState(List<ReturnExchangeStoreDefinition> refreshDefinitions)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < refreshDefinitions.Count; i++)
			{
				list.Add(refreshDefinitions[i].Id);
			}
			List<int> list2 = PickRefreshExchangeIds(list, GetRefreshSlotCount(list.Count));
			ActiveRefreshExchangeIds.Clear();
			ActiveRefreshExchangeIds.AddRange(list2);
			return list2.Count > 0;
		}

		private List<int> PickRefreshExchangeIds(List<int> refreshExchangeIds, int refreshSlotCount)
		{
			List<int> list = new List<int>();
			if (refreshExchangeIds == null || refreshExchangeIds.Count == 0 || refreshSlotCount <= 0)
			{
				return list;
			}
			if (base.manager?.Player?.PlayerRandom != null)
			{
				return base.manager.Player.PlayerRandom.WeightedRandomList(refreshExchangeIds, refreshSlotCount, (int x) => 1L, isRepeat: false);
			}
			for (int num = 0; num < refreshExchangeIds.Count && num < refreshSlotCount; num++)
			{
				list.Add(refreshExchangeIds[num]);
			}
			return list;
		}

		private ReturnExchangeStoreDefinition FindDefinitionById(int definitionId)
		{
			List<ReturnExchangeStoreDefinition> configuredDefinitions = GetConfiguredDefinitions();
			for (int i = 0; i < configuredDefinitions.Count; i++)
			{
				ReturnExchangeStoreDefinition returnExchangeStoreDefinition = configuredDefinitions[i];
				if (returnExchangeStoreDefinition != null && returnExchangeStoreDefinition.Id == definitionId)
				{
					return configuredDefinitions[i];
				}
			}
			return null;
		}

		private bool IsRefreshDefinition(ReturnExchangeStoreDefinition definition)
		{
			if (definition == null)
			{
				return false;
			}
			return definition.Type == ReturnExchangeStoreType.Refresh;
		}

		private int GetCouncilLevelSnapshot()
		{
			ReturnActivityManager returnActivityManager = base.manager?.Player?.ReturnActivityManager;
			if (returnActivityManager == null)
			{
				return 0;
			}
			if (returnActivityManager.IdentityCouncilLevelSnapshot <= 0)
			{
				return base.manager.Player.CouncilLevel;
			}
			return returnActivityManager.IdentityCouncilLevelSnapshot;
		}
	}
}
