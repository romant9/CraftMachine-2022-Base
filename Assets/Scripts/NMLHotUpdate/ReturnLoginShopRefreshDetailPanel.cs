using System;
using TWDModel;
using UnityEngine;

public class ReturnLoginShopRefreshDetailPanel : MonoBehaviour
{
	[Header("有库存")]
	[SerializeField]
	private GameObject productRoot;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private EquipmentButton rewardEquip;

	[SerializeField]
	private ReturnLoginShopRewardModSkillItem rewardModSkill;

	[SerializeField]
	private UIButton refreshButton;

	[SerializeField]
	private UILabel refreshCostLabel;

	[SerializeField]
	private UISprite refreshCostIcon;

	[SerializeField]
	private UIButton purchaseButton;

	[SerializeField]
	private UILabel purchaseCostLabel;

	[SerializeField]
	private UISprite purchaseCostIcon;

	[Header("售罄")]
	[SerializeField]
	private GameObject soldOutRoot;

	[SerializeField]
	private UILabel restockLabel;

	private ReturnExchangeStoreModel _model;

	private ReturnExchangeStoreDefinition _definition;

	private long _timeLeft;

	private Color _purchaseCostDefaultColor;

	private void Awake()
	{
		if (purchaseCostLabel != null)
		{
			_purchaseCostDefaultColor = purchaseCostLabel.color;
		}
		refreshButton.onClick.Add(new EventDelegate(OnRefreshClicked));
		purchaseButton.onClick.Add(new EventDelegate(OnPurchaseClicked));
	}

	public void Show(ReturnExchangeStoreDefinition definition, ReturnExchangeStoreModel model)
	{
		_definition = definition;
		_model = model;
		if (_definition == null || _model == null || _definition.Type != ReturnExchangeStoreType.Refresh)
		{
			Hide();
			return;
		}
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		bool flag = _model.GetRemainingCount(_definition.Id) <= 0;
		Helpers.GameObjectSetActive(productRoot, !flag);
		Helpers.GameObjectSetActive(soldOutRoot, flag);
		if (!flag)
		{
			RefreshProductInfo();
		}
		RefreshTimeLeft();
	}

	public void Hide()
	{
		_model = null;
		_definition = null;
		_timeLeft = 0L;
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void Update()
	{
		if (base.gameObject.activeInHierarchy && _model != null && _definition != null)
		{
			_timeLeft = Math.Max(_timeLeft - (long)(Time.deltaTime * 1000f), 0L);
			UpdateLabel();
		}
	}

	private void RefreshProductInfo()
	{
		HelpersUI.SetContentToLabel(titleLabel, string.IsNullOrEmpty(_definition.DisplayDescription) ? string.Empty : LocalizationManager.GetText(_definition.DisplayDescription));
		ReturnLoginShopPanel.Apply(GetFirstReward(_definition.RewardEntries), rewardIcon, rewardEquip, rewardModSkill);
		RewardCurrency rewardCurrency = GetFirstReward(_definition.CostRewardEntries) as RewardCurrency;
		bool flag = ReturnLoginShopPanel.IsCurrencyInsufficient(_definition);
		if (rewardCurrency != null)
		{
			HelpersUI.SetContentToLabel(purchaseCostLabel, Helpers.FormatNumber(rewardCurrency.Amount, 0, 1));
			HelpersUI.SetSprite(purchaseCostIcon, HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType));
		}
		if (purchaseCostLabel != null)
		{
			purchaseCostLabel.color = (flag ? Color.red : _purchaseCostDefaultColor);
		}
		int refreshSlotIndex = GetRefreshSlotIndex(_model, _definition.Id);
		if (refreshSlotIndex >= 0)
		{
			var (num, currencyType) = _model.GetManualRefreshCost(refreshSlotIndex);
			HelpersUI.SetContentToLabel(refreshCostLabel, Helpers.FormatNumber(num, 0, 1));
			HelpersUI.SetSprite(refreshCostIcon, HelpersGfx.GetCurrencyIconName(currencyType));
		}
		if (purchaseButton != null)
		{
			bool flag2 = _model.CanExchange(_definition.Id);
			purchaseButton.isEnabled = flag || flag2;
			purchaseButton.SetState((flag || !flag2) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, true);
		}
		if (refreshButton != null)
		{
			refreshButton.isEnabled = _model.CanManualRefresh(_definition.Id);
		}
	}

	private void RefreshTimeLeft()
	{
		long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
		_timeLeft = Math.Max(_model.NextRefreshTimestamp - valueOrDefault, 0L);
		UpdateLabel();
	}

	private void UpdateLabel()
	{
		HelpersUI.SetContentToLabel(timerLabel, Helpers.FormatTimeNoZero(_timeLeft));
		HelpersUI.SetContentToLabel(restockLabel, LocalizationManager.GetText("return.exchange.store.restock", Helpers.FormatTimeNoZero(_timeLeft)));
	}

	private void OnRefreshClicked()
	{
		if (_definition == null || _model == null || !_model.CanManualRefresh(_definition.Id))
		{
			HUDNotification.Info(LocalizationManager.GetText("return.exchange.store.cannot.refresh"));
		}
		else if (Helpers.ExecuteCommand(new RefreshReturnExchangeStoreCommand
		{
			ExchangeId = _definition.Id
		}) != TWDModelResult.OK)
		{
			HUDNotification.Info(LocalizationManager.GetText("return.exchange.store.cannot.refresh"));
		}
		else
		{
			UIEvent.Send("ReturnLoginShopChangedEvent");
		}
	}

	private void OnPurchaseClicked()
	{
		if (_definition == null || _model == null || _model.GetRemainingCount(_definition.Id) <= 0)
		{
			return;
		}
		if (ReturnLoginShopPanel.IsCurrencyInsufficient(_definition))
		{
			UIEvent.Send("ReturnLoginShopGoToTasksEvent");
			return;
		}
		int id = _definition.Id;
		Rewards rewardEntries = _definition.RewardEntries;
		if (Helpers.ExecuteCommand(new ReturnExchangeStoreCommand(id)) == TWDModelResult.OK)
		{
			BuildingsHUD buildingsHUD = BuildingsHUD.Get();
			if (buildingsHUD != null && rewardEntries != null)
			{
				buildingsHUD.CreateCollectAnim(rewardEntries, base.gameObject);
			}
			ReturnLoginShopPanel.ShowRewardPopup(rewardEntries);
			UIEvent.Send("ReturnLoginShopChangedEvent");
		}
	}

	private static int GetRefreshSlotIndex(ReturnExchangeStoreModel model, int exchangeId)
	{
		if (model?.ActiveRefreshExchangeIds != null)
		{
			return model.ActiveRefreshExchangeIds.IndexOf(exchangeId);
		}
		return -1;
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
