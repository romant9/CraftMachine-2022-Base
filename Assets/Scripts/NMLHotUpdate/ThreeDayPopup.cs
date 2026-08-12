using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ThreeDayPopup : MonoBehaviour
{
	[SerializeField]
	private List<ThreeDayReward> rewardEntries;

	[SerializeField]
	private GameObject checkObj;

	[SerializeField]
	private GameObject checkContain;

	[SerializeField]
	private GameObject payButton;

	[SerializeField]
	private UILabel priceLabel;

	private bool _isCheckBox;

	private bool _isInitialized;

	private bool IsValid
	{
		get
		{
			if (GameManager.Instance.playerModel.ThreeDayModel == null)
			{
				return false;
			}
			if (!GameManager.Instance.playerModel.ThreeDayModel.CanShowThreeDay)
			{
				return false;
			}
			return true;
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (!(type == "ThreeDayFreshEvent"))
		{
			if (type == "OnBundleBought")
			{
				UpdateUI();
			}
		}
		else
		{
			UpdateUI();
		}
	}

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		_isCheckBox = false;
		UpdateUI();
	}

	public void Close()
	{
		if (IsValid)
		{
			if (_isCheckBox)
			{
				SetNoPop();
			}
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}

	private void Update()
	{
		if (!_isInitialized && GameManager.Instance.IAPManager.IsInitialized())
		{
			_isInitialized = true;
			string identifier = GameManager.Instance.playerModel?.ThreeDayModel?.CurrentDefinition?.BundleIdentifier;
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			string iAPProduct = gameEconomyData.GetBundleContentDefinition(identifier).IAPProduct;
			string content = ((gameEconomyData.GetInAppPurchaseProduct(iAPProduct).PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(iAPProduct) : LocalizationManager.GetText("Generic.Free"));
			HelpersUI.SetContentToLabel(priceLabel, content);
		}
	}

	public void UpdateUI()
	{
		if (!IsValid)
		{
			return;
		}
		Helpers.GameObjectSetActive(checkContain, value: false);
		Helpers.GameObjectSetActive(checkObj, _isCheckBox);
		Helpers.GameObjectSetActive(payButton, !GameManager.Instance.playerModel.ThreeDayModel.HasBuy);
		_ = rewardEntries.Count;
		List<Rewards> currentReward = GameManager.Instance.playerModel.ThreeDayModel.CurrentReward;
		List<ThreeDayRewardStatus> rewardsStatus = GameManager.Instance.playerModel.ThreeDayModel.RewardsStatus;
		if (currentReward.Count == rewardsStatus.Count)
		{
			int count = currentReward.Count;
			int num = ((rewardEntries.Count <= count) ? rewardEntries.Count : count);
			for (int i = 0; i < num; i++)
			{
				rewardEntries[i].UpdateUI(currentReward[i], rewardsStatus[i], i);
			}
			string identifier = GameManager.Instance.playerModel?.ThreeDayModel?.CurrentDefinition?.BundleIdentifier;
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			string iAPProduct = gameEconomyData.GetBundleContentDefinition(identifier).IAPProduct;
			string content = ((gameEconomyData.GetInAppPurchaseProduct(iAPProduct).PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(iAPProduct) : LocalizationManager.GetText("Generic.Free"));
			HelpersUI.SetContentToLabel(priceLabel, content);
		}
	}

	public void OnBuyClick()
	{
		if (GameManager.Instance.playerModel.ThreeDayModel != null && GameManager.Instance.playerModel.ThreeDayModel.CanShowThreeDay && !GameManager.Instance.playerModel.ThreeDayModel.HasBuy)
		{
			ThreeDayDefinition currentDefinition = GameManager.Instance.playerModel.ThreeDayModel.CurrentDefinition;
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			BundleStoreDefinition bundleStoreDefinition = gameEconomyData.GetBundleStoreDefinition(currentDefinition.BundleIdentifier);
			BundleContentDefinition bundleContentDefinition = gameEconomyData.GetBundleContentDefinition(currentDefinition.BundleIdentifier);
			GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
			UIEvent.Send("ThreeDayFreshEvent");
		}
	}

	private void SetNoPop()
	{
		Helpers.ExecuteCommand(new ThreeDayNoPopCommand());
	}

	public void OnClickNoPopup()
	{
		_isCheckBox = !_isCheckBox;
		Helpers.GameObjectSetActive(checkObj, _isCheckBox);
	}
}
