using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnQuestAndExchangeModel : TWDModelObject
	{
		public const string ReturnQuestAndExchangeChanged = "ReturnQuestAndExchangeChanged";

		public ReturnDailyQuestModel DailyQuest { get; private set; }

		public ReturnRepeatQuestModel RepeatQuest { get; private set; }

		public ReturnExchangeStoreModel ExchangeStore { get; private set; }

		[JsonIgnore]
		public bool HasRedDot
		{
			get
			{
				ReturnDailyQuestModel dailyQuest = DailyQuest;
				if (dailyQuest == null || !dailyQuest.HasRedDot)
				{
					ReturnRepeatQuestModel repeatQuest = RepeatQuest;
					if (repeatQuest == null || !repeatQuest.HasRedDot)
					{
						return ExchangeStore?.HasRedDot ?? false;
					}
				}
				return true;
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			ensureChildren();
		}

		public override void Start()
		{
			ensureChildren();
			base.Start();
			if (base.manager?.Player != null)
			{
				base.manager.Player.OnCurrencySpentEvent -= OnCurrencySpent;
				base.manager.Player.OnCurrencySpentEvent += OnCurrencySpent;
				base.manager.Player.OnWalkersKilledEvent -= OnWalkersKilled;
				base.manager.Player.OnWalkersKilledEvent += OnWalkersKilled;
				base.manager.Player.OnItemUpgradedEvent -= OnItemUpgraded;
				base.manager.Player.OnItemUpgradedEvent += OnItemUpgraded;
				base.manager.Player.OnMissionCompletedEvent -= OnMissionCompleted;
				base.manager.Player.OnMissionCompletedEvent += OnMissionCompleted;
				base.manager.Player.OnCouncilLevelUpEvent -= OnCouncilLevelUp;
				base.manager.Player.OnCouncilLevelUpEvent += OnCouncilLevelUp;
			}
		}

		public void ResetForNewActivity(long currentTimestamp)
		{
			ensureChildren();
			DailyQuest.ResetForNewActivity(currentTimestamp);
			RepeatQuest.ResetForNewActivity();
			ExchangeStore.ResetForNewActivity(currentTimestamp);
			NotifyChange("ReturnQuestAndExchangeChanged");
		}

		public void OnLogin(long currentTimestamp)
		{
			ensureChildren();
			if (DailyQuest.OnLogin(currentTimestamp) | RepeatQuest.OnLogin(currentTimestamp))
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
		}

		public void OnCurrencySpent(CurrencyType currencyType, int amount)
		{
			if (amount > 0)
			{
				ensureChildren();
				if (DailyQuest.OnCurrencySpent(currencyType, amount) | RepeatQuest.OnCurrencySpent(currencyType, amount))
				{
					NotifyChange("ReturnQuestAndExchangeChanged");
				}
			}
		}

		public void OnWalkersKilled(int amount)
		{
			if (amount > 0)
			{
				ensureChildren();
				if (DailyQuest.OnWalkersKilled(amount) | RepeatQuest.OnWalkersKilled(amount))
				{
					NotifyChange("ReturnQuestAndExchangeChanged");
				}
			}
		}

		public void OnItemUpgraded(ReturnQuestType upgradeQuestType)
		{
			if (upgradeQuestType.IsUpgradeQuest())
			{
				ensureChildren();
				if (DailyQuest.OnItemUpgraded(upgradeQuestType) | RepeatQuest.OnItemUpgraded(upgradeQuestType))
				{
					NotifyChange("ReturnQuestAndExchangeChanged");
				}
			}
		}

		public void OnMissionCompleted()
		{
			ensureChildren();
			if (DailyQuest.OnMissionCompleted() | RepeatQuest.OnMissionCompleted())
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
		}

		public void OnCouncilLevelUp(int level)
		{
			ensureChildren();
			if (RepeatQuest.OnCouncilLevelUp())
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
		}

		public bool TryClaimDailyQuestReward(int definitionId)
		{
			ensureChildren();
			bool num = DailyQuest.TryClaimReward(definitionId);
			if (num)
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
			return num;
		}

		public bool TryClaimRepeatQuestReward()
		{
			ensureChildren();
			bool num = RepeatQuest.TryClaimReward();
			if (num)
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
			return num;
		}

		public bool TryClaimRepeatQuestReward(int definitionId)
		{
			ensureChildren();
			bool num = RepeatQuest.TryClaimReward(definitionId);
			if (num)
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
			return num;
		}

		public TWDModelResult TryExchange(int exchangeId)
		{
			ensureChildren();
			TWDModelResult num = ExchangeStore.Exchange(exchangeId);
			if (num == TWDModelResult.OK)
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
			return num;
		}

		public bool TryRefreshExchangeStore(int exchangeId)
		{
			ensureChildren();
			bool num = ExchangeStore.TryManualRefresh(exchangeId);
			if (num)
			{
				NotifyChange("ReturnQuestAndExchangeChanged");
			}
			return num;
		}

		private void ensureChildren()
		{
			if (DailyQuest == null)
			{
				DailyQuest = new ReturnDailyQuestModel();
				DailyQuest.SetManager(base.manager);
				DailyQuest.Initialize();
			}
			if (RepeatQuest == null)
			{
				RepeatQuest = new ReturnRepeatQuestModel();
				RepeatQuest.SetManager(base.manager);
				RepeatQuest.Initialize();
			}
			if (ExchangeStore == null)
			{
				ExchangeStore = new ReturnExchangeStoreModel();
				ExchangeStore.SetManager(base.manager);
				ExchangeStore.Initialize();
			}
		}
	}
}
