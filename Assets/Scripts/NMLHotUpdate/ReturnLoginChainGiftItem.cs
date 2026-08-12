using TWDModel;
using UnityEngine;

public class ReturnLoginChainGiftItem : MonoBehaviour
{
	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel priceLabel;

	[SerializeField]
	private GameObject lockedObject;

	[SerializeField]
	private UIButton button;

	private ReturnEndlessDealDefinition _definition;

	private bool _isCurrent;

	private Rewards _rewards;

	public void Bind(ReturnEndlessDealDefinition definition, bool isCurrent)
	{
		_definition = definition;
		_isCurrent = isCurrent;
		_rewards = ((definition != null && !string.IsNullOrEmpty(definition.Reward)) ? new Rewards(definition.Reward) : null);
		ResolveReferences();
		Refresh();
	}

	private void ResolveReferences()
	{
		button = ((button != null) ? button : GetComponentInChildren<UIButton>(includeInactive: true));
		if (button != null)
		{
			EventDelegate.Set(button.onClick, OnChainItemClicked);
		}
		UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			string text = componentsInChildren[i].name.ToLowerInvariant();
			if (priceLabel == null && text.Contains("price"))
			{
				priceLabel = componentsInChildren[i];
			}
			else if (amountLabel == null && (text.Contains("num") || text.Contains("amount")))
			{
				amountLabel = componentsInChildren[i];
			}
		}
		UISprite[] componentsInChildren2 = GetComponentsInChildren<UISprite>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (componentsInChildren2[j].name.ToLowerInvariant().Contains("icon"))
			{
				rewardIcon = componentsInChildren2[j];
				break;
			}
		}
		Transform[] componentsInChildren3 = GetComponentsInChildren<Transform>(includeInactive: true);
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			if (!(lockedObject == null))
			{
				break;
			}
			if (componentsInChildren3[k].name.ToLowerInvariant().Contains("lock"))
			{
				lockedObject = componentsInChildren3[k].gameObject;
			}
		}
	}

	private void Refresh()
	{
		IReward reward = ((_rewards != null && _rewards.Count > 0) ? _rewards.GetRewardAt(0) : null);
		if (reward != null)
		{
			HelpersGfx.GetIconNameForIReward(reward, out var spriteName, null, null, null);
			HelpersUI.SetSprite(rewardIcon, spriteName);
			int numsForIReward = Helpers.GetNumsForIReward(reward);
			HelpersUI.SetContentToLabel(amountLabel, (numsForIReward > 1) ? ("x" + numsForIReward) : string.Empty);
		}
		Helpers.GameObjectSetActive(lockedObject, !_isCurrent);
		if (button != null)
		{
			button.isEnabled = _isCurrent;
		}
		ReturnEndlessDealDefinition definition = _definition;
		if (definition != null && definition.Type == ReturnEndlessDealPackType.Free)
		{
			HelpersUI.SetContentToLabel(priceLabel, LocalizationManager.GetText("Generic.Free"));
		}
		else
		{
			HelpersUI.SetContentToLabel(priceLabel, GetPaidPrice());
		}
	}

	private string GetPaidPrice()
	{
		if (_definition == null || string.IsNullOrEmpty(_definition.BundleIdentifier))
		{
			return string.Empty;
		}
		BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(_definition.BundleIdentifier);
		InAppPurchaseProductApple inAppPurchaseProductApple = ((bundleContentDefinition == null) ? null : GameManager.Instance.gameEconomyData.GetInAppPurchaseProduct(bundleContentDefinition.IAPProduct));
		if (inAppPurchaseProductApple == null)
		{
			return string.Empty;
		}
		if (!(inAppPurchaseProductApple.PriceUSD > 0f))
		{
			return LocalizationManager.GetText("Generic.Free");
		}
		return GameManager.Instance.IAPManager.GetFormattedPrice(bundleContentDefinition.IAPProduct);
	}

	public void OnChainItemClicked()
	{
		if (!_isCurrent || _definition == null)
		{
			return;
		}
		if (_definition.Type == ReturnEndlessDealPackType.Free)
		{
			if (Helpers.ExecuteCommand(new ClaimReturnEndlessDealFreePackCommand()) == TWDModelResult.OK)
			{
				UIEvent.Send("ReturnLoginChainItemClickEvent", this);
			}
			return;
		}
		BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(_definition.BundleIdentifier);
		BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(_definition.BundleIdentifier);
		if (bundleStoreDefinition != null && bundleContentDefinition != null)
		{
			GameManager.Instance.IAPManager.Buy(bundleStoreDefinition, bundleContentDefinition);
		}
	}
}
