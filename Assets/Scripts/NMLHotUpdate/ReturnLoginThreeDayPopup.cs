using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ReturnLoginThreeDayPopup : MonoBehaviour
{
	[SerializeField]
	private Transform rewardContainer;

	[SerializeField]
	private GameObject buyButton;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	private readonly List<ReturnLoginThreeDayRewardItem> _rewardItems = new List<ReturnLoginThreeDayRewardItem>();

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		ResolveReferences();
		Refresh();
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnBundleBought")
		{
			Refresh();
		}
	}

	private void ResolveReferences()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			string text = componentsInChildren[i].name.ToLowerInvariant();
			if (rewardContainer == null && (text == "rewardscontainer" || text == "reward_content"))
			{
				rewardContainer = componentsInChildren[i];
			}
			if (buyButton == null && text == "buy_button")
			{
				buyButton = componentsInChildren[i].gameObject;
			}
		}
		if (buyButton != null)
		{
			UIButton component = buyButton.GetComponent<UIButton>();
			if (component != null)
			{
				EventDelegate.Set(component.onClick, OnBuyClicked);
			}
			priceLabel = ((priceLabel != null) ? priceLabel : buyButton.GetComponentInChildren<UILabel>(includeInactive: true));
		}
		if (!(descriptionLabel == null))
		{
			return;
		}
		UILabel[] componentsInChildren2 = GetComponentsInChildren<UILabel>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (componentsInChildren2[j].name.ToLowerInvariant().Contains("desc"))
			{
				descriptionLabel = componentsInChildren2[j];
				break;
			}
		}
	}

	private void Refresh()
	{
		ReturnThreeDayModel model = GetModel();
		if (model == null)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		Helpers.GameObjectSetActive(buyButton, !model.HasBuy);
		HelpersUI.SetContentToLabel(descriptionLabel, LocalizationManager.GetText("return.special.offer.desc"));
		RefreshPrice(model);
		RefreshRewards(model);
	}

	private void RefreshRewards(ReturnThreeDayModel model)
	{
		if (rewardContainer == null)
		{
			return;
		}
		if (_rewardItems.Count == 0)
		{
			for (int i = 0; i < rewardContainer.childCount; i++)
			{
				GameObject gameObject = rewardContainer.GetChild(i).gameObject;
				ReturnLoginThreeDayRewardItem item = gameObject.GetComponent<ReturnLoginThreeDayRewardItem>() ?? gameObject.AddComponent<ReturnLoginThreeDayRewardItem>();
				_rewardItems.Add(item);
			}
		}
		List<Rewards> currentReward = model.CurrentReward;
		for (int j = 0; j < _rewardItems.Count; j++)
		{
			bool flag = currentReward != null && model.RewardsStatus != null && j < currentReward.Count && j < model.RewardsStatus.Count;
			Helpers.GameObjectSetActive(_rewardItems[j].gameObject, flag);
			if (flag)
			{
				_rewardItems[j].Bind(currentReward[j], model.RewardsStatus[j], j, Refresh);
			}
		}
	}

	private void RefreshPrice(ReturnThreeDayModel model)
	{
		string text = model.CurrentDefinition?.BundleIdentifier;
		BundleContentDefinition bundleContentDefinition = (string.IsNullOrEmpty(text) ? null : GameManager.Instance.gameEconomyData.GetBundleContentDefinition(text));
		InAppPurchaseProductApple inAppPurchaseProductApple = ((bundleContentDefinition == null) ? null : GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct));
		if (inAppPurchaseProductApple == null)
		{
			HelpersUI.SetContentToLabel(priceLabel, string.Empty);
			return;
		}
		string content = ((inAppPurchaseProductApple.PriceUSD > 0f) ? GameManager.Instance.IAPManager.GetFormattedPrice(bundleContentDefinition.IAPProduct) : LocalizationManager.GetText("Generic.Free"));
		HelpersUI.SetContentToLabel(priceLabel, content);
	}

	public void OnBuyClicked()
	{
		ReturnThreeDayModel model = GetModel();
		ReturnThreeDayDefinition returnThreeDayDefinition = model?.CurrentDefinition;
		if (model != null && returnThreeDayDefinition != null && !model.HasBuy)
		{
			BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(returnThreeDayDefinition.BundleIdentifier);
			BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(returnThreeDayDefinition.BundleIdentifier);
			if (bundleStoreDefinition != null && bundleContentDefinition != null)
			{
				GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
			}
		}
	}

	private static ReturnThreeDayModel GetModel()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnThreeDay;
	}
}
