using System;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class OptionalUpdatePopup : HUDElement
{
	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private GameObject UpdateGiftParent;

	[SerializeField]
	private GameObject FeatureLockedParent;

	private int lastTimeLeftUpdate;

	private static int contentIndex;

	public static void OpenUpdateGiftContent()
	{
		contentIndex = 0;
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OptionalUpdatePopup) as OptionalUpdatePopup).Open();
	}

	public static void OpenFeatureLockedContent()
	{
		contentIndex = 1;
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OptionalUpdatePopup) as OptionalUpdatePopup).Open();
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		if (contentIndex == 0)
		{
			Helpers.GameObjectSetActive(UpdateGiftParent, value: true);
			Helpers.GameObjectSetActive(FeatureLockedParent, value: false);
			if (timerLabel != null && GameManager.Instance.VersionValidUntil.HasValue && GameManager.Instance.VersionUpgradeNeeded)
			{
				int num = Helpers.ConvertToSecondsNoZero((long)(GameManager.Instance.VersionValidUntil.Value - DateTime.UtcNow).TotalMilliseconds);
				if (num != lastTimeLeftUpdate)
				{
					lastTimeLeftUpdate = num;
					timerLabel.text = Helpers.FormatTimeNoZero((long)num * 1000L);
				}
			}
		}
		else if (contentIndex == 1)
		{
			Helpers.GameObjectSetActive(UpdateGiftParent, value: false);
			Helpers.GameObjectSetActive(FeatureLockedParent, value: true);
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateUI();
	}

	public void TakeToStore()
	{
		Application.OpenURL(GameConfiguration.Instance.Config.StoreURL);
	}

	public void TakeToNews()
	{
		OnClickClose();
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		if (!string.IsNullOrEmpty(gameEconomyData.ConfigData.VersionUpdateContentEntryId))
		{
			DeepLinkNavigation.HandleItemDeepLinkClick(new PlayerHubNewsItem
			{
				NavigationLink = "OPEN_ARTICLE",
				PromoAttributes = Enum.GetName(typeof(PlayerHubNewsItem.AttributeTag), PlayerHubNewsItem.AttributeTag.EntryId) + ":" + gameEconomyData.ConfigData.VersionUpdateContentEntryId
			});
		}
	}
}
