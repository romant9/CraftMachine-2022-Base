using System;
using System.Collections.Generic;
using System.Reflection;
using TWDModel;
using UnityEngine;

public class ResourcesDeeplinksGridPanel : MonoBehaviour
{
	[SerializeField]
	private UIGridExtended Grid;

	[SerializeField]
	private GameObject ItemPrefab;

	[SerializeField]
	private CurrencyType currencyType;

	[SerializeField]
	private HUDElement parentPopup;

	private static Dictionary<string, Type> cachedImplementors = new Dictionary<string, Type>();

	public void Init()
	{
		if (!(Grid != null))
		{
			return;
		}
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		if (gameEconomyData == null || gameEconomyData.CampaignDeeplinks == null || gameEconomyData.CampaignDeeplinks.Length == 0)
		{
			return;
		}
		Grid.enabled = true;
		for (int i = 0; i < gameEconomyData.CampaignDeeplinks.Length; i++)
		{
			CampaignDeeplink campaignDeeplink = gameEconomyData.CampaignDeeplinks[i];
			if (campaignDeeplink.IsCurrencySpecific && !campaignDeeplink.AllowedCurrencyNames.Contains(currencyType.ToString()))
			{
				continue;
			}
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(ItemPrefab, Grid.gameObject);
			if (!(gameObject != null))
			{
				continue;
			}
			ResourceDeeplinkItem component = gameObject.GetComponent<ResourceDeeplinkItem>();
			if (component != null)
			{
				AvailableRewardsCollector availableRewardsCollector = InstantiateCollector(campaignDeeplink);
				if (availableRewardsCollector != null)
				{
					component.SetParameters(currencyType, availableRewardsCollector, GetModelObject(campaignDeeplink), campaignDeeplink.NameLocalizationKey, campaignDeeplink.URL, campaignDeeplink.RefreshLocKey, campaignDeeplink.TimerBased, parentPopup.UIType);
					component.UpdateUI();
				}
			}
		}
	}

	public void Clean()
	{
		for (int i = 0; i < ((!(Grid == null)) ? Grid.transform.childCount : 0); i++)
		{
			UnityEngine.Object.Destroy(Grid.transform.GetChild(i).gameObject);
		}
	}

	private TWDModelObject GetModelObject(CampaignDeeplink deeplink)
	{
		TWDModelObject result = GameManager.Instance.playerModel;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && !string.IsNullOrEmpty(deeplink.ObjectNameInPlayer))
		{
			PropertyInfo property = playerModel.GetType().GetProperty(deeplink.ObjectNameInPlayer);
			if (property != null)
			{
				result = (TWDModelObject)property.GetValue(playerModel, null);
			}
		}
		return result;
	}

	private AvailableRewardsCollector InstantiateCollector(CampaignDeeplink deeplink)
	{
		TWDModelObject modelObject = GetModelObject(deeplink);
		AvailableRewardsCollector result = null;
		if (modelObject != null)
		{
			Type value = null;
			if (!cachedImplementors.TryGetValue(deeplink.RewardCollectorImpl, out value))
			{
				value = ReflectionUtils.FindDerivedTypeOrInterfaceStartingWith(typeof(AvailableRewardsCollector), deeplink.RewardCollectorImpl);
				if (value != null)
				{
					cachedImplementors[deeplink.RewardCollectorImpl] = value;
				}
			}
			try
			{
				result = Activator.CreateInstance(value, modelObject) as AvailableRewardsCollector;
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Cannot instantiate rewardCollector '{deeplink.RewardCollectorImpl}' reason: {ex.Message}");
			}
		}
		return result;
	}
}
