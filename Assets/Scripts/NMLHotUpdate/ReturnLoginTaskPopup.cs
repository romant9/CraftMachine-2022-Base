using BaseModel;
using TWDModel;
using UnityEngine;

public class ReturnLoginTaskPopup : MonoBehaviour
{
	[SerializeField]
	private UITabs tabs;

	[SerializeField]
	private UILabel labelMoney;

	[SerializeField]
	private ReturnLoginTaskPanel returnLoginTaskPanel;

	[SerializeField]
	private ReturnLoginShopPanel returnLoginShopPanel;

	private const CurrencyType DefaultMoneyCurrency = CurrencyType.AssaultStar;

	private const int ExchangeTabIndex = 0;

	private const int QuestTabIndex = 1;

	private float _stateRefreshTimer;

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		SubscribePlayerChanged(subscribe: true);
		RefreshMoney();
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		tabs.OnNewTabSelectedEvent += OnTabSelected;
		CloseExpiredTabs();
		int num = ResolveTabIndex(tabs.CurrentTabIndex);
		tabs.SelectTab(num);
		OnTabSelected(num);
		_stateRefreshTimer = 0.5f;
		RefreshRedDot();
	}

	public void Close()
	{
		if (tabs != null)
		{
			tabs.OnNewTabSelectedEvent -= OnTabSelected;
		}
		SubscribePlayerChanged(subscribe: false);
		returnLoginTaskPanel?.Close();
		returnLoginShopPanel?.Close();
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		SubscribePlayerChanged(subscribe: false);
		if (tabs != null)
		{
			tabs.OnNewTabSelectedEvent -= OnTabSelected;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void Update()
	{
		_stateRefreshTimer -= Time.deltaTime;
		if (!(_stateRefreshTimer > 0f))
		{
			_stateRefreshTimer = 0.5f;
			CloseExpiredTabs();
			RefreshRedDot();
		}
	}

	private void OnTabSelected(int tabindex)
	{
		tabindex = ResolveTabIndex(tabindex);
		if (tabs.GetContent(tabindex) != null)
		{
			switch (tabindex)
			{
			case 0:
				returnLoginTaskPanel.Close();
				returnLoginShopPanel.Open();
				break;
			case 1:
				returnLoginShopPanel.Close();
				returnLoginTaskPanel.Open();
				break;
			}
		}
		RefreshMoney();
		RefreshRedDot();
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "ReturnLoginShopGoToTasksEvent" && IsTabAvailable(1))
		{
			tabs.SelectTab(1);
			OnTabSelected(1);
		}
	}

	private void CloseExpiredTabs()
	{
		if (!(tabs == null))
		{
			HideTabIfExpired(0);
			HideTabIfExpired(1);
			int num = ResolveTabIndex(tabs.CurrentTabIndex);
			if (num != tabs.CurrentTabIndex && IsTabAvailable(num))
			{
				tabs.SelectTab(num);
				OnTabSelected(num);
			}
		}
	}

	private void HideTabIfExpired(int tabIndex)
	{
		if (!IsTabAvailable(tabIndex))
		{
			UIToggle button = tabs.GetButton(tabIndex);
			if (button != null)
			{
				Helpers.GameObjectSetActive(button.gameObject, value: false);
			}
			if (tabIndex == 1)
			{
				tabs.SetRedDot(tabIndex, show: false);
			}
		}
	}

	private bool IsTabAvailable(int tabIndex)
	{
		ReturnActivityManager activityManager = GetActivityManager();
		if (activityManager == null)
		{
			return false;
		}
		if (tabIndex != 1)
		{
			return activityManager.IsReturnExchangeAvailable();
		}
		return activityManager.IsReturnActivityAvailable();
	}

	private int ResolveTabIndex(int tabIndex)
	{
		if (tabIndex == 1)
		{
			if (!IsTabAvailable(1))
			{
				return 0;
			}
			return 1;
		}
		if (!IsTabAvailable(0))
		{
			return 1;
		}
		return 0;
	}

	private void RefreshRedDot()
	{
		if (!(tabs == null))
		{
			ReturnQuestAndExchangeModel obj = GetActivityManager()?.ReturnQuestAndExchange;
			tabs.SetRedDot(1, obj?.HasRedDot ?? false);
		}
	}

	private void SubscribePlayerChanged(bool subscribe)
	{
		PlayerModel playerModel = GameManager.Instance?.playerModel;
		if (playerModel != null)
		{
			if (subscribe)
			{
				playerModel.Changed -= OnPlayerChanged;
				playerModel.Changed += OnPlayerChanged;
			}
			else
			{
				playerModel.Changed -= OnPlayerChanged;
			}
		}
	}

	private void OnPlayerChanged(ModelObject model, string changed, object args)
	{
		if (changed == "currencyChangedEvent")
		{
			RefreshMoney();
			RefreshRedDot();
		}
	}

	private void RefreshMoney()
	{
		if (!(labelMoney == null))
		{
			CurrencyType currencyType = ResolveMoneyCurrencyType();
			int num = (GameManager.Instance?.playerModel?.GetCurrency(currencyType))?.Value ?? 0;
			HelpersUI.SetContentToLabel(labelMoney, Helpers.FormatNumber(num, 0, 1));
		}
	}

	private static CurrencyType ResolveMoneyCurrencyType()
	{
		ReturnExchangeStoreModel returnExchangeStoreModel = GetActivityManager()?.ReturnQuestAndExchange?.ExchangeStore;
		if (returnExchangeStoreModel?.ExchangeDefinitions != null)
		{
			for (int i = 0; i < returnExchangeStoreModel.ExchangeDefinitions.Count; i++)
			{
				Rewards rewards = returnExchangeStoreModel.ExchangeDefinitions[i]?.CostRewardEntries;
				if (rewards != null && rewards.Count > 0 && rewards.GetRewardAt(0) is RewardCurrency rewardCurrency)
				{
					return rewardCurrency.CurrencyType;
				}
			}
		}
		return CurrencyType.AssaultStar;
	}

	private static ReturnActivityManager GetActivityManager()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager;
	}
}
