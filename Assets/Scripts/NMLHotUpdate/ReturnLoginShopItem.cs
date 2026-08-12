using System;
using TWDModel;
using UnityEngine;

public class ReturnLoginShopItem : UIListCard<ReturnExchangeStoreDefinition>
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private EquipmentButton rewardEquip;

	[SerializeField]
	private ReturnLoginShopRewardModSkillItem rewardModSkill;

	[SerializeField]
	private UISprite priceIcon;

	[SerializeField]
	private GameObject soldOutContainer;

	[SerializeField]
	private UIButton button;

	[SerializeField]
	private GameObject select;

	private ReturnExchangeStoreModel _model;

	private UILabel _statusLabel;

	private long _timeLeft;

	private void Awake()
	{
		if (button != null)
		{
			EventDelegate.Set(button.onClick, OnSelectClicked);
		}
		if (soldOutContainer != null)
		{
			_statusLabel = soldOutContainer.GetComponent<UILabel>();
		}
		SetSelected(selected: false);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void Update()
	{
		if (base.gameObject.activeInHierarchy && base.Item != null && _model != null && base.Item.Type == ReturnExchangeStoreType.Refresh && _timeLeft > 0)
		{
			_timeLeft = Math.Max(_timeLeft - (long)(Time.deltaTime * 1000f), 0L);
			UpdateRefreshTimeTexts();
		}
	}

	public void SetContext(ReturnExchangeStoreModel model)
	{
		_model = model;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Refresh();
	}

	public void Refresh()
	{
		if (base.Item == null)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		string content = (string.IsNullOrEmpty(base.Item.DisplayDescription) ? string.Empty : LocalizationManager.GetText(base.Item.DisplayDescription));
		HelpersUI.SetContentToLabel(titleLabel, content);
		ReturnLoginShopPanel.Apply(GetFirstReward(base.Item.RewardEntries), rewardIcon, rewardEquip, rewardModSkill);
		if (GetFirstReward(base.Item.CostRewardEntries) is RewardCurrency rewardCurrency)
		{
			HelpersUI.SetContentToLabel(priceLabel, Helpers.FormatNumber(rewardCurrency.Amount, 0, 1));
			HelpersUI.SetSprite(priceIcon, HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType));
		}
		bool flag = base.Item.Type == ReturnExchangeStoreType.Refresh;
		int num = ((_model != null) ? _model.GetRemainingCount(base.Item.Id) : 0);
		bool flag2 = num == 0;
		if (flag)
		{
			SyncTimeLeft();
		}
		if (!flag && base.Item.Limit > 0)
		{
			Helpers.GameObjectSetActive((amountLabel != null) ? amountLabel.gameObject : null, value: true);
			HelpersUI.SetContentToLabel(amountLabel, LocalizationManager.GetText("return.exchange.store.limit", Math.Max(num, 0), base.Item.Limit));
		}
		else if (!flag)
		{
			Helpers.GameObjectSetActive((amountLabel != null) ? amountLabel.gameObject : null, value: false);
		}
		else if (!flag2)
		{
			Helpers.GameObjectSetActive((amountLabel != null) ? amountLabel.gameObject : null, value: true);
			HelpersUI.SetContentToLabel(amountLabel, Helpers.FormatTimeNoZero(_timeLeft));
		}
		else
		{
			Helpers.GameObjectSetActive((amountLabel != null) ? amountLabel.gameObject : null, value: false);
		}
		Helpers.GameObjectSetActive(soldOutContainer, flag2);
		if (flag2)
		{
			if (flag)
			{
				HelpersUI.SetContentToLabel(_statusLabel, LocalizationManager.GetText("return.exchange.store.restock", Helpers.FormatTimeNoZero(_timeLeft)));
			}
			else
			{
				HelpersUI.SetContentToLabel(_statusLabel, LocalizationManager.GetText("return.exchange.store.sold.out"));
			}
		}
	}

	public void SetSelected(bool selected)
	{
		Helpers.GameObjectSetActive(select, selected);
	}

	private void SyncTimeLeft()
	{
		long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
		_timeLeft = ((_model != null) ? Math.Max(_model.NextRefreshTimestamp - valueOrDefault, 0L) : 0);
	}

	private void UpdateRefreshTimeTexts()
	{
		if (base.Item != null && base.Item.Type == ReturnExchangeStoreType.Refresh)
		{
			if (_model == null || _model.GetRemainingCount(base.Item.Id) != 0)
			{
				HelpersUI.SetContentToLabel(amountLabel, Helpers.FormatTimeNoZero(_timeLeft));
				return;
			}
			HelpersUI.SetContentToLabel(_statusLabel, LocalizationManager.GetText("return.exchange.store.restock", Helpers.FormatTimeNoZero(_timeLeft)));
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "ReturnLoginShopChangedEvent")
		{
			Refresh();
		}
		else if (!(type != "ReturnLoginShopItemSelectedEvent"))
		{
			ReturnExchangeStoreDefinition returnExchangeStoreDefinition = parameter as ReturnExchangeStoreDefinition;
			SetSelected(base.Item != null && returnExchangeStoreDefinition != null && base.Item.Id == returnExchangeStoreDefinition.Id);
		}
	}

	private void OnSelectClicked()
	{
		if (base.Item != null)
		{
			UIEvent.Send("ReturnLoginShopItemSelectedEvent", base.Item);
		}
	}

	private static IReward GetFirstReward(Rewards rewards)
	{
		if (rewards == null || rewards.Count <= 0)
		{
			return null;
		}
		return rewards.GetRewardAt(0);
	}
}
