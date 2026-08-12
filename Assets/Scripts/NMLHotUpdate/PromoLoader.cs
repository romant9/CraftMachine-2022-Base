using System.Collections.Generic;
using UnityEngine;

public class PromoLoader : MonoBehaviourExtended
{
	[Header("Article placement id")]
	[SerializeField]
	private string PlacementId = "";

	private const string resourceSrcPrefix = "UI/Promos/";

	private List<PlayerHubNewsItem> newsPromoItems;

	private PromoBase promoBase;

	private void Awake()
	{
		DebugIdString = "PromoLoader";
	}

	public void Start()
	{
		bool flag = GameManager.Instance.playerModel.Tutorial != null && GameManager.Instance.playerModel.Tutorial.StaticTutorialComplete;
		if (GameManager.Instance.gameEconomyData.GetFeature("PromoLoader").Enabled && flag && newsPromoItems == null)
		{
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager) return");
				DebugTWD.Log("Ignore UpdateActiveNews in PromoLoader");
				return;
			}
			GameManager.Instance.PlayerHubManager.UpdateActiveNews();
			UpdateWithPlacementId(PlacementId);
		}
	}

	public void UpdateWithPlacementId(string id)
	{
		bool flag = GameManager.Instance.gameEconomyData.GetFeature("PromoLoader").Enabled;
		if (!(!TutorialView.Instance.RunningButNotSuggesting && flag))
		{
			return;
		}
		GameManager.Instance.PlayerHubManager.GetItemsWithAttribute(PlayerHubNewsItem.AttributeTag.PlacementId, id, ref newsPromoItems);
		if (newsPromoItems == null || newsPromoItems.Count <= 0 || newsPromoItems[0] == null)
		{
			return;
		}
		string attributeValue = newsPromoItems[0].GetAttributeValue(PlayerHubNewsItem.AttributeTag.PrefabSrc);
		if (string.IsNullOrEmpty(attributeValue))
		{
			return;
		}
		GameObject gameObject = UnityUtils.LoadFromAssetBundle(attributeValue, HUDElementConfig.BundleName) as GameObject;
		if (gameObject != null)
		{
			if (promoBase != null && promoBase.CachedItem != null && promoBase.CachedItem.EntryId != newsPromoItems[0].EntryId)
			{
				Object.Destroy(promoBase);
				promoBase = null;
			}
			if (promoBase == null)
			{
				promoBase = Helpers.InstantiateWithComponent<PromoBase>(gameObject, base.gameObject);
			}
		}
		else
		{
			DebugLogError("Could not load prefab with src: " + attributeValue);
		}
		if (promoBase != null)
		{
			promoBase.UpdateUIWithItem(newsPromoItems[0]);
		}
		else
		{
			DebugLogError("Failed to open Promo with PlayerHubNewsItem id: " + newsPromoItems[0].EntryId);
		}
	}
}
