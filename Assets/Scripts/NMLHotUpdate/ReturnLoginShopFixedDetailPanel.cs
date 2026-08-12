using System;
using TWDModel;
using UnityEngine;

public class ReturnLoginShopFixedDetailPanel : MonoBehaviour
{
	[Header("有库存")]
	[SerializeField]
	private GameObject productRoot;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel limitLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private EquipmentButton rewardEquip;

	[SerializeField]
	private ReturnLoginShopRewardModSkillItem rewardModSkill;

	[SerializeField]
	private UIButton purchaseButton;

	[SerializeField]
	private UILabel purchaseCostLabel;

	[SerializeField]
	private UISprite purchaseCostIcon;

	[Header("售罄")]
	[SerializeField]
	private GameObject soldOutRoot;

	private ReturnExchangeStoreDefinition _definition;

	private ReturnExchangeStoreModel _model;

	private Color _purchaseCostDefaultColor;

	private void Awake()
	{
		if (purchaseCostLabel != null)
		{
			_purchaseCostDefaultColor = purchaseCostLabel.color;
		}
		if (purchaseButton != null)
		{
			EventDelegate.Set(purchaseButton.onClick, OnPurchaseClicked);
		}
	}

	public void Show(ReturnExchangeStoreDefinition definition, ReturnExchangeStoreModel model)
	{
		_definition = definition;
		_model = model;
		if (_definition == null || _model == null || _definition.Type != ReturnExchangeStoreType.Fixed)
		{
			Hide();
			return;
		}
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		int remainingCount = _model.GetRemainingCount(_definition.Id);
		bool flag = remainingCount <= 0;
		Helpers.GameObjectSetActive(productRoot, !flag);
		Helpers.GameObjectSetActive(soldOutRoot, flag);
		if (!flag)
		{
			HelpersUI.SetContentToLabel(titleLabel, string.IsNullOrEmpty(_definition.DisplayDescription) ? string.Empty : LocalizationManager.GetText(_definition.DisplayDescription));
			IReward firstReward = GetFirstReward(_definition.RewardEntries);
			ReturnLoginShopPanel.Apply(firstReward, rewardIcon, rewardEquip, rewardModSkill);
			if (firstReward != null)
			{
				int numsForIReward = Helpers.GetNumsForIReward(firstReward);
				HelpersUI.SetContentToLabel(amountLabel, $"{numsForIReward}");
			}
			RewardCurrency rewardCurrency = GetFirstReward(_definition.CostRewardEntries) as RewardCurrency;
			bool flag2 = ReturnLoginShopPanel.IsCurrencyInsufficient(_definition);
			if (rewardCurrency != null)
			{
				HelpersUI.SetContentToLabel(purchaseCostLabel, Helpers.FormatNumber(rewardCurrency.Amount, 0, 1));
				HelpersUI.SetSprite(purchaseCostIcon, HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType));
			}
			if (purchaseCostLabel != null)
			{
				purchaseCostLabel.color = (flag2 ? Color.red : _purchaseCostDefaultColor);
			}
			if (_definition.Limit > 0)
			{
				HelpersUI.SetContentToLabel(limitLabel, LocalizationManager.GetText("return.exchange.store.limit", Math.Max(remainingCount, 0), _definition.Limit));
			}
			else
			{
				Helpers.GameObjectSetActive((limitLabel != null) ? limitLabel.gameObject : null, value: false);
			}
			if (purchaseButton != null)
			{
				bool flag3 = _model.CanExchange(_definition.Id);
				purchaseButton.isEnabled = flag2 || flag3;
				purchaseButton.SetState((flag2 || !flag3) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, true);
			}
		}
	}

	public void Hide()
	{
		_definition = null;
		_model = null;
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnPurchaseClicked()
	{
		if (_definition == null || _model == null)
		{
			return;
		}
		if (_model.GetRemainingCount(_definition.Id) <= 0)
		{
			HUDNotification.Info(LocalizationManager.GetText("return.exchange.store.sold.out"));
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

	private static IReward GetFirstReward(Rewards rewards)
	{
		if (rewards == null || rewards.Count <= 0)
		{
			return null;
		}
		return rewards.GetRewardAt(0);
	}
}
