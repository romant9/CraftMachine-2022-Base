using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class LimitedSeasonTimeOfferButton : LimitedTimeOfferButton
{
	[SerializeField]
	private UISprite BundleSprite;

	private MissionHighlight featuredData;

	public void SetFeaturedData(MissionHighlight data)
	{
		featuredData = data;
		SetOffer(GetFirstLimitedBundle());
	}

	protected override void NoOffer()
	{
		base.NoOffer();
		featuredData = null;
	}

	protected override void SetOffer(LimitedBundleData bundle)
	{
		base.SetOffer(bundle);
		if (bundle != null && featuredData != null)
		{
			HelpersUI.SetSprite(BundleSprite, featuredData.BundleSpriteName);
		}
		Helpers.GameObjectSetActive(BundleSprite, featuredData != null && !string.IsNullOrEmpty(featuredData.BundleSpriteName));
	}

	protected override LimitedBundleData GetFirstLimitedBundle()
	{
		BundleManagerModel bundleManager = GameManager.Instance.playerModel.BundleManager;
		if (bundleManager != null && featuredData != null && !string.IsNullOrEmpty(featuredData.BundleId))
		{
			List<LimitedBundleData> initiatedLimitedBundles = bundleManager.InitiatedLimitedBundles;
			if (initiatedLimitedBundles != null)
			{
				for (int i = 0; i < initiatedLimitedBundles.Count; i++)
				{
					LimitedBundleData limitedBundleData = initiatedLimitedBundles[i];
					if (limitedBundleData != null && limitedBundleData.IsAvailable && limitedBundleData.BundleID == featuredData.BundleId)
					{
						return limitedBundleData;
					}
				}
			}
		}
		return null;
	}
}
