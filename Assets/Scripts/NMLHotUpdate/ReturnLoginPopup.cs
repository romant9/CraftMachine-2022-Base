using System;
using TWDModel;
using UnityEngine;

public class ReturnLoginPopup : HUDElement
{
	[SerializeField]
	private UIButton btnClose;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private UITabs tabs;

	[SerializeField]
	private GameObject loginTab;

	[SerializeField]
	private ReturnLoginSevenDayPopup returnLoginSevenDayPopup;

	[SerializeField]
	private ReturnLoginPrivilegePopup returnLoginPrivilegePopup;

	[SerializeField]
	private ReturnLoginTaskPopup returnLoginTaskPopup;

	[SerializeField]
	private ReturnLoginThreeDayPopup returnLoginThreeDayPopup;

	[SerializeField]
	private ReturnLoginChainGiftPopup returnLoginChainGiftPopup;

	public const float StateRefreshInterval = 0.5f;

	private const int TabTypeCount = 5;

	private ReturnActivityTabType _selectedTab;

	private float _stateRefreshTimer;

	private void Awake()
	{
		btnClose.onClick.Add(new EventDelegate(Close));
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		if (tabs != null)
		{
			tabs.OnNewTabSelectedEvent -= OnTabSelected;
		}
	}

	public override void Open()
	{
		base.Open();
		RefreshLoginTabAvailability(selectPrivilegeWhenCompleted: false);
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		tabs.OnNewTabSelectedEvent += OnTabSelected;
		CloseExpiredTabs(enforceSelection: false);
		ReturnActivityTabType returnActivityTabType = ResolveDefaultTab();
		if (!IsTabAvailable(returnActivityTabType))
		{
			Close();
			return;
		}
		tabs.SelectTab((int)returnActivityTabType);
		OnTabSelected((int)returnActivityTabType);
		_stateRefreshTimer = 0.5f;
		RefreshRedDots();
		RefreshTimeLabel();
	}

	public override void Close()
	{
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		CloseAllPanels();
		base.Close();
	}

	public override void Update()
	{
		base.Update();
		RefreshTimeLabel();
		_stateRefreshTimer -= Time.deltaTime;
		if (_stateRefreshTimer <= 0f)
		{
			_stateRefreshTimer = 0.5f;
			CloseExpiredTabs(enforceSelection: true);
			RefreshRedDots();
		}
	}

	private void OnTabSelected(int tabIndex)
	{
		if (Enum.IsDefined(typeof(ReturnActivityTabType), tabIndex))
		{
			SelectTab((ReturnActivityTabType)tabIndex);
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "ReturnLoginSevenDayClaimEvent")
		{
			RefreshLoginTabAvailability(selectPrivilegeWhenCompleted: true);
			RefreshRedDots();
		}
	}

	private void RefreshLoginTabAvailability(bool selectPrivilegeWhenCompleted)
	{
	}

	private void CloseExpiredTabs(bool enforceSelection)
	{
		if (GetManager() == null || tabs == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < 5; i++)
		{
			ReturnActivityTabType tab = (ReturnActivityTabType)i;
			if (!IsTabAvailable(tab))
			{
				GameObject tabButtonObject = GetTabButtonObject(i);
				if (tabButtonObject != null && tabButtonObject.activeSelf)
				{
					Helpers.GameObjectSetActive(tabButtonObject, value: false);
					flag = true;
				}
			}
		}
		if (flag)
		{
			RepositionTabs();
		}
		if (enforceSelection && !IsTabAvailable(_selectedTab))
		{
			SwitchAwayFromExpiredTab();
		}
	}

	private void SwitchAwayFromExpiredTab()
	{
		if (IsTabAvailable(ReturnActivityTabType.QuestsAndExchange))
		{
			tabs.SelectTab(2);
			OnTabSelected(2);
		}
		else
		{
			Close();
		}
	}

	private ReturnActivityTabType ResolveDefaultTab()
	{
		if (!IsTabUsable(ReturnActivityTabType.Privilege))
		{
			return ReturnActivityTabType.QuestsAndExchange;
		}
		return ReturnActivityTabType.Privilege;
	}

	private bool IsTabUsable(ReturnActivityTabType tab)
	{
		GameObject tabButtonObject = GetTabButtonObject((int)tab);
		if (IsTabAvailable(tab))
		{
			if (!(tabButtonObject == null))
			{
				return tabButtonObject.activeSelf;
			}
			return true;
		}
		return false;
	}

	private bool IsTabAvailable(ReturnActivityTabType tab)
	{
		ReturnActivityManager manager = GetManager();
		if (manager == null)
		{
			return false;
		}
		if (tab != ReturnActivityTabType.QuestsAndExchange)
		{
			return manager.IsReturnActivityAvailable();
		}
		if (!manager.IsReturnActivityAvailable())
		{
			return manager.IsReturnExchangeAvailable();
		}
		return true;
	}

	private void RefreshRedDots()
	{
		if (!(tabs == null))
		{
			for (int i = 0; i < 5; i++)
			{
				tabs.SetRedDot(i, HasRedDot((ReturnActivityTabType)i));
			}
		}
	}

	private bool HasRedDot(ReturnActivityTabType tab)
	{
		ReturnActivityManager manager = GetManager();
		if (manager == null || !IsTabAvailable(tab))
		{
			return false;
		}
		return tab switch
		{
			ReturnActivityTabType.Login => manager.ReturnLogin?.HasRedDot ?? false, 
			ReturnActivityTabType.Privilege => manager.ReturnPrivilege?.HasRedDot ?? false, 
			ReturnActivityTabType.QuestsAndExchange => manager.ReturnQuestAndExchange?.HasRedDot ?? false, 
			ReturnActivityTabType.SpecialOffer => manager.ReturnThreeDay?.HasRedDot ?? false, 
			ReturnActivityTabType.EndlessGiftDeal => manager.ReturnEndlessDeal?.HasRedDot ?? false, 
			_ => false, 
		};
	}

	private GameObject GetTabButtonObject(int tabIndex)
	{
		UIToggle uIToggle = ((tabs != null) ? tabs.GetButton(tabIndex) : null);
		if (!(uIToggle != null))
		{
			return null;
		}
		return uIToggle.gameObject;
	}

	private void RepositionTabs()
	{
		UIGrid component = tabs.GetComponent<UIGrid>();
		if (component != null)
		{
			component.repositionNow = true;
			return;
		}
		UITable component2 = tabs.GetComponent<UITable>();
		if (component2 != null)
		{
			component2.repositionNow = true;
		}
	}

	private void SelectTab(ReturnActivityTabType tabType)
	{
		_selectedTab = tabType;
		CloseAllPanels();
		switch (tabType)
		{
		case ReturnActivityTabType.Privilege:
			returnLoginPrivilegePopup?.Open();
			break;
		case ReturnActivityTabType.QuestsAndExchange:
			returnLoginTaskPopup?.Open();
			break;
		case ReturnActivityTabType.Login:
		case ReturnActivityTabType.SpecialOffer:
		case ReturnActivityTabType.EndlessGiftDeal:
			break;
		}
	}

	private void CloseAllPanels()
	{
		returnLoginSevenDayPopup?.Close();
		returnLoginPrivilegePopup?.Close();
		returnLoginTaskPopup?.Close();
		returnLoginThreeDayPopup?.Close();
		returnLoginChainGiftPopup?.Close();
	}

	private void RefreshTimeLabel()
	{
		if (!(timeLabel == null))
		{
			ReturnActivityManager manager = GetManager();
			long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
			long num = ((_selectedTab != ReturnActivityTabType.QuestsAndExchange) ? (manager?.ReturnActivityEndTimestamp ?? 0) : (manager?.ReturnExchangeEndTimestamp ?? 0));
			string text = LocalizationManager.GetText("UI_Roulette_Countdown", (num > valueOrDefault) ? Helpers.FormatTimeNoZero(num - valueOrDefault) : "0");
			HelpersUI.SetContentToLabel(timeLabel, text);
		}
	}

	private static ReturnActivityManager GetManager()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager;
	}
}
