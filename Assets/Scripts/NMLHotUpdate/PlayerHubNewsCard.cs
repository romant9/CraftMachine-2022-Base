using System;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class PlayerHubNewsCard : UIListCard<PlayerHubNewsItem>
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private GameObject unreadContainer;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private UITexture thumbnailTexture;

	[SerializeField]
	private UIButton actionButton;

	[SerializeField]
	private UILabel actionButtonLabel;

	[SerializeField]
	private int thumbnailTextureMaxHeight = 150;

	private long timeUntilNotValid;

	public override void UpdateUI()
	{
		titleLabel.text = base.Item.Title;
		contentLabel.text = base.Item.Abstract;
		unreadContainer.SetActive(!base.Item.HasBeenRead);
		if (timerLabel != null)
		{
			timerContainer.SetActive(base.Item.ShowCounter);
		}
		timeUntilNotValid = Math.Max((long)(base.Item.EndUnixTime.FromUnixTimeSeconds() - DateTime.UtcNow).TotalMilliseconds, 0L);
		LoadImageFromUrl component = GetComponent<LoadImageFromUrl>();
		if (base.Item.ThumbnailUrl != null && component != null)
		{
			component.LoadImage(base.Item.ThumbnailUrl, thumbnailTexture, thumbnailTextureMaxHeight);
		}
		bool active = false;
		if (base.Item.NavigationLink != null)
		{
			active = true;
			string navigationLink = base.Item.NavigationLink;
			if (!(navigationLink == "OPEN_BUNDLE"))
			{
				if (navigationLink == "OPEN_CHALLENGE")
				{
					WeeklyChallengeModel weeklyChallenge = GameManager.Instance.playerModel.WeeklyChallenge;
					if (weeklyChallenge == null || weeklyChallenge.CurrentDefinition == null || !weeklyChallenge.CanPlayWeeklyChallenge)
					{
						active = false;
					}
				}
				else if (GameManager.Instance != null && GameManager.Instance.gameEconomyData.GetFeature("DeepLinkNavigation_From_News").Enabled)
				{
					active = DeepLinkNavigation.IsDeepLinkAccessable(base.Item);
				}
			}
			else if (GameManager.Instance.playerModel.BundleManager != null)
			{
				BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(base.Item.PromoAttributes);
				if (bundleStoreDefinition == null || !GameManager.Instance.playerModel.BundleManager.CanBuyBundle(bundleStoreDefinition))
				{
					active = false;
				}
			}
			actionButtonLabel.text = LocalizationManager.GetText("Popup.Hub.News.Button." + base.Item.NavigationLink);
		}
		actionButton.gameObject.SetActive(active);
	}

	private void Update()
	{
		if (base.Item.ShowCounter)
		{
			long num = (long)(Time.deltaTime * 1000f);
			if (timeUntilNotValid > 0 && timeUntilNotValid - num <= 0)
			{
				UIEvent.Send("NewsBecameUnvalid");
			}
			timeUntilNotValid = Math.Max(timeUntilNotValid - num, 0L);
			if (timerLabel != null)
			{
				timerLabel.text = Helpers.FormatTimeNoZero(timeUntilNotValid);
			}
		}
	}

	public override int GetSortValue()
	{
		return base.Item.OrderNumber;
	}

	public void OnButton()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (base.Item.NavigationLink == null)
		{
			return;
		}
		Helpers.ExecuteCommand(new PlayerHubCommand
		{
			EventName = "player_hub_open_news",
			ItemId = base.Item.EntryId
		});
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampCampMapHud) as CampHUD;
		switch (base.Item.NavigationLink)
		{
		case "MORE_INFO":
		{
			PlayerHubNewsPopup playerHubNewsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlayerHubNewsPopup) as PlayerHubNewsPopup;
			if (playerHubNewsPopup != null)
			{
				playerHubNewsPopup.Item = base.Item;
				playerHubNewsPopup.Open();
			}
			break;
		}
		case "POLL":
		case "QUIZ":
		{
			PlayerHubNewsPopup playerHubNewsPopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlayerHubQuizPopup) as PlayerHubNewsPopup;
			if (playerHubNewsPopup2 != null)
			{
				playerHubNewsPopup2.Item = base.Item;
				playerHubNewsPopup2.Open();
			}
			break;
		}
		case "OPEN_BUNDLE":
			if (GameManager.Instance != null)
			{
				string promoAttributes = base.Item.PromoAttributes;
				BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.playerModel.gameEconomyData.GetBundleStoreDefinition(promoAttributes);
				if (bundleStoreDefinition != null && GameManager.Instance.playerModel.BundleManager.CanBuyBundle(bundleStoreDefinition))
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
					GameManager.Instance.BundleSource = Metrics.BundleSource.PlayerHub;
					BundleCardPopup.OpenBundle(bundleStoreDefinition.BundleIdentifier);
				}
			}
			break;
		case "OPEN_SHOP":
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
			ShopPopupHelper.OpenWithIndex(2);
			break;
		case "OPEN_SHOP_TRADE":
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
			ShopPopupHelper.OpenWithIndex(3);
			break;
		case "OPEN_CHALLENGE":
		{
			WeeklyChallengeModel weeklyChallenge = GameManager.Instance.playerModel.WeeklyChallenge;
			if (weeklyChallenge != null && weeklyChallenge.CurrentDefinition != null && weeklyChallenge.CanPlayWeeklyChallenge)
			{
				CampManager.Instance.GoToMap(weeklyChallenge.GetMapMissionGroupModel());
			}
			break;
		}
		case "OPEN_URL":
		case "OPEN_VIDEO":
			Application.OpenURL(base.Item.PromoAttributes);
			break;
		case "OPEN_RADIO_TENT":
			if (campHUD != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
				campHUD.OnClickPhone();
			}
			break;
		case "OPEN_CYCLES":
			if (campHUD != null)
			{
				campHUD.OnClickOutpostManagement();
			}
			Invoke("OpenCycles", 0.1f);
			break;
		case "OPEN_WORKSHOP":
			if (campHUD != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
				campHUD.OnClickWorkshop();
			}
			break;
		case "OPEN_OUTPOST":
			if (campHUD != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
				campHUD.OnClickOutpostManagement();
			}
			break;
		case "OPEN_SURVIVORS":
			if (campHUD != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
				campHUD.OnClickTrainingGround();
			}
			break;
		case "OPEN_FAQ":
			SingularityMonoBehaviour<SDKManager>.Instance.ShowFAQs();
			break;
		case "OPEN_ACHIEVEMENTS":
			AchievementPopup.OpenAchievement();
			Invoke("SelectTabAchievements", 0.1f);
			break;
		default:
			if (GameManager.Instance != null && GameManager.Instance.gameEconomyData.GetFeature("DeepLinkNavigation_From_News").Enabled)
			{
				DeepLinkNavigation.HandleItemDeepLinkClick(base.Item);
			}
			break;
		}
	}

	public void SelectTabAchievements()
	{
		AchievementPopup achievementPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AchievementPopup) as AchievementPopup;
		if (achievementPopup != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
			achievementPopup.SelectTabAchievements();
		}
	}

	public void OpenCycles()
	{
		OutpostPopup outpostPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopup) as OutpostPopup;
		if (outpostPopup != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.PlayerHubPopup);
			outpostPopup.GoToCycles();
		}
	}
}
