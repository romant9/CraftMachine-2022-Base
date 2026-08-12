using System;
using TWDModel;
using UnityEngine;

public class SurvivorCardMoreSlots : UIListCard<SurvivorModel>
{
	[Header("Buy more slots button")]
	[SerializeField]
	private PayButton payButton;

	[Header("Slots count label")]
	[SerializeField]
	private UILabel slotsCount;

	[SerializeField]
	private UILabel infoLabel;

	public void OnEnable()
	{
		SetupVisuals();
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void OnDestroy()
	{
		if (payButton != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(payButton.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnBuyClick));
		}
	}

	private GameObject GetChild(GameObject parent, string childName)
	{
		Transform transform = parent.transform.Find(childName);
		if (transform != null)
		{
			return transform.gameObject;
		}
		return null;
	}

	private void SetupVisuals()
	{
		if (payButton != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(payButton.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnBuyClick));
		}
		RefreshVisuals();
	}

	private void OnBuyClick(GameObject button)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Cashier purchaseNextSlotsLevelCashier = GameManager.Instance.playerModel.SurvivorContainer.GetPurchaseNextSlotsLevelCashier();
		if (purchaseNextSlotsLevelCashier != null)
		{
			if (purchaseNextSlotsLevelCashier.CanAfford())
			{
				ConsumeCurrencyCommandUtils.Execute(new BuyMoreSurvivorSlotsCommand
				{
					Cashier = purchaseNextSlotsLevelCashier
				}, BuyMoreSlotsCallback);
			}
			else
			{
				ShopPopupHelper.OpenForMissingCurrencyWithMissingAmount(purchaseNextSlotsLevelCashier.GetMissing(CurrencyType.Diamonds));
			}
		}
	}

	public void BuyMoreSlotsCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			UIEvent.Send("SurvivorExtraSlotBought");
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SurvivorDeleted":
		case "SurvivorExtraSlotBought":
			RefreshVisuals();
			break;
		case "OnBundleBought":
			RefreshVisuals();
			break;
		}
	}

	private void RefreshVisuals()
	{
		if (!(payButton != null))
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool flag = playerModel.SurvivorContainer.CanPurchaseMoreSlots();
		payButton.gameObject.SetActive(flag);
		if (flag)
		{
			payButton.UpdateUI(playerModel.SurvivorContainer.GetPurchaseNextSlotsLevelCashier(), LocalizationManager.GetText("Popup.TrainingGround.BuyMoreSurvivorSlots"));
			int survivorSlotsUpgradeLevel = GameManager.Instance.playerModel.SurvivorContainer.SurvivorSlotsUpgradeLevel;
			SurvivorSlotsData survivorSlotsData = GameManager.Instance.gameEconomyData.GetSurvivorSlotsData(survivorSlotsUpgradeLevel);
			SurvivorSlotsData survivorSlotsData2 = GameManager.Instance.gameEconomyData.GetSurvivorSlotsData(survivorSlotsUpgradeLevel + 1);
			if (survivorSlotsData != null && survivorSlotsData2 != null)
			{
				int num = survivorSlotsData2.AvailableSlotsCount - survivorSlotsData.AvailableSlotsCount;
				if (num == 1)
				{
					infoLabel.text = LocalizationManager.GetText("Popup.TrainingGround.BuyMoreSurvivorSlots.InfoOne");
				}
				else
				{
					infoLabel.text = LocalizationManager.GetText("Popup.TrainingGround.BuyMoreSurvivorSlots.Info{Parameter}", num);
				}
			}
		}
		else
		{
			infoLabel.text = LocalizationManager.GetText("Popup.TrainingGround.BuyMoreSurvivorSlots.MaxReach");
		}
		if (slotsCount != null && playerModel != null && playerModel.SurvivorContainer != null)
		{
			int level = Mathf.Min(playerModel.SurvivorContainer.SurvivorSlotsUpgradeLevel, playerModel.gameEconomyData.GetMaxSurvivorSlotsLevel());
			if (playerModel.gameEconomyData.GetSurvivorSlotsData(level) != null)
			{
				slotsCount.text = playerModel.SurvivorContainer.Survivors.Count + "/" + playerModel.SurvivorContainer.SurvivorSlotsCount;
			}
		}
	}

	public override int GetSortValue()
	{
		return UIListCard<SurvivorModel>.GetSortIntFor(SurvivorSortOrder.SurvivorMoreSlotsCard, 1000);
	}
}
