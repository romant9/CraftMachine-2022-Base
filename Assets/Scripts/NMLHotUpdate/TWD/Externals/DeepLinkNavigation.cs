using BaseModel;
using TWDModel;
using UnityEngine;

namespace TWD.Externals
{
	public class DeepLinkNavigation
	{
		public static bool HandleDeepLink(string deepLinkAction)
		{
			CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampCampMapHud) as CampHUD;
			switch (deepLinkAction)
			{
			case "OPEN_GOLD_SHOP":
				ShopPopupHelper.OpenWithIndex(2);
				return true;
			case "OPEN_CHALLENGE":
				MissionHubNavigation.TryOpenChallengeMap();
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_accept");
				return true;
			case "OPEN_RADIO_TENT":
				if (campHUD != null && TutorialView.Allowed("PhoneButton") && !TutorialView.Instance.Running)
				{
					SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
					campHUD.OnClickPhone();
				}
				return true;
			case "OPEN_OUTPOST":
				MissionHubNavigation.TryOpenOutpost();
				return true;
			case "MISSION_HUB":
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
				MissionHubPopup.OpenPopup();
				return true;
			case "CAMP":
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
				return true;
			case "WORKSHOP":
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampWorkshopPopup).Open();
				return true;
			case "TRAINING_GROUNDS":
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds).Open();
				return true;
			case "TRADE_GOOD_SHOP":
				ShopPopupHelper.OpenWithIndex(3);
				return true;
			case "HIGHLIGHTED_SEASON":
				if (GameManager.Instance != null && GameManager.Instance.gameEconomyData != null)
				{
					MissionHubNavigation.OpenSeasonMap(GameManager.Instance.gameEconomyData.GetHighlightedSeasonDefinition());
				}
				return true;
			case "DISTANCE":
				MissionHubNavigation.TryOpenSurvivalMap();
				return true;
			case "SCAVENGE":
				MissionHubNavigation.OpenScavenge();
				return true;
			case "STORY":
				MissionHubNavigation.ContinueStoryMap();
				return true;
			case "GUILD":
				CampHUD.OpenGuildOrChallenge(UIType.SocialPopupGuild);
				return true;
			case "GUILD_BATTLE_MAP":
				MissionHubNavigation.TryOpenGvGBattleMap();
				return true;
			case "GUILD_BATTLE_MAP_SPECTATOR":
				MissionHubNavigation.TryOpenGvGBattleMap(isSpectator: true);
				return true;
			case "GUILD_CHAT":
				if (GameManager.Instance.GuildManager.GuildOffline || GameManager.Instance.GuildManager.IsBusy || string.IsNullOrEmpty(GameManager.Instance.playerModel.Name))
				{
					CampHUD.OpenGuildOrChallenge(UIType.SocialPopupGuild);
				}
				else
				{
					(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialPopupGuild) as SocialPopupGuild).OpenForTab(3);
				}
				return true;
			case "DAILY_QUESTS":
			{
				CampHUD campHUD2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
				if (campHUD2 != null)
				{
					campHUD2.OnClickAchievement();
					return true;
				}
				return false;
			}
			case "CAMPAIGNS":
				return true;
			case "OPEN_GUILD_BATTLE_OVERVIEW":
				if (!GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled)
				{
					AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Alert.NotAvailableTitle"), LocalizationManager.GetText("Popup.Alert.NotAvailableMessage"), LocalizationManager.GetText("Button.Ok"));
					return false;
				}
				HUDManager.TryOpenPopup(UIType.GuildBattleOverviewPopup);
				return true;
			case "OPEN_GUILD_BATTLE_HIGHSCORE":
				HUDManager.TryOpenPopup(UIType.GuildBattleHighscorePopup);
				return true;
			case "OPEN_GUILD_SHOP":
				GuildShopPopup.OpenGuildShop();
				return true;
			case "OPEN_GVG_START_FLOW":
				HUDManager.TryOpenPopup(UIType.GvGStartBattleFlowPopup, Helpers.GetUIParent(HUDManager.Instance.UIContainerTopCameras));
				return true;
			case "OPEN_GUILD_BATTLE_INFO":
			{
				PopupQuickTip popupQuickTip = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleInfoPopup) as PopupQuickTip;
				if (popupQuickTip != null && !popupQuickTip.IsOpen)
				{
					popupQuickTip.TipId = "Info_GuildBattle";
					popupQuickTip.Open();
					popupQuickTip.CustomBulletsPosition();
				}
				return true;
			}
			default:
				return false;
			}
		}

		public static void HandleItemDeepLinkClick(PlayerHubNewsItem Item)
		{
			if (Item == null)
			{
				Debug.LogError("Could not HandleItemClick with NULL Item");
			}
			else
			{
				if (HandleDeepLink(Item.NavigationLink))
				{
					return;
				}
				switch (Item.NavigationLink)
				{
				case "OPEN_ARTICLE":
				{
					PlayerHubNewsItem articleWithId = GameManager.Instance.PlayerHubManager.GetArticleWithId(Item.GetAttributeValue(PlayerHubNewsItem.AttributeTag.EntryId));
					if (articleWithId == null)
					{
						break;
					}
					if ((articleWithId.NavigationLink != null && articleWithId.NavigationLink == "POLL") || articleWithId.NavigationLink == "QUIZ")
					{
						PlayerHubNewsPopup playerHubNewsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlayerHubQuizPopup) as PlayerHubNewsPopup;
						if (playerHubNewsPopup != null)
						{
							playerHubNewsPopup.Item = articleWithId;
							playerHubNewsPopup.Open();
						}
					}
					else
					{
						PlayerHubNewsPopup playerHubNewsPopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.PlayerHubNewsPopup) as PlayerHubNewsPopup;
						if (playerHubNewsPopup2 != null)
						{
							playerHubNewsPopup2.Item = articleWithId;
							playerHubNewsPopup2.Open();
						}
					}
					break;
				}
				case "OPEN_BUNDLE":
					if (GameManager.Instance != null && GameManager.Instance.playerModel.BundleManager != null)
					{
						string attributeValue = Item.GetAttributeValue(PlayerHubNewsItem.AttributeTag.EntryId);
						BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(attributeValue);
						if (bundleStoreDefinition != null && GameManager.Instance.playerModel.BundleManager.CanBuyBundle(bundleStoreDefinition))
						{
							GameManager.Instance.BundleSource = Metrics.BundleSource.PlayerHub;
							BundleCardPopup.OpenBundle(bundleStoreDefinition.BundleIdentifier);
						}
					}
					break;
				case "HERO_PREVIEW":
					if (!(GameManager.Instance == null) && GameManager.Instance.gameEconomyData != null)
					{
						string attributeValue2 = Item.GetAttributeValue(PlayerHubNewsItem.AttributeTag.EntryId);
						ActorDefinition actorDefinition = (string.IsNullOrEmpty(attributeValue2) ? null : GameManager.Instance.gameEconomyData.GetActorDefinition(attributeValue2));
						if (actorDefinition != null && actorDefinition.ID.ToLower().Contains("hero") && !GameManager.Instance.playerModel.SurvivorContainer.HasHero(actorDefinition.ID))
						{
							SurvivorInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
							int num = GameManager.Instance.playerModel.SurvivorContainer.GetHighestLevelSurvivor() + actorDefinition.InitialLevelOffset;
							SurvivorModel survivorModel = GameManager.Instance.playerModel.SurvivorContainer.CreateSurvivorFromDefinition(actorDefinition.ID, num, num, actorDefinition.RarityLevel, num, actorDefinition.InitialEquipmentRarityLevel, new ModelRandom(), actorDefinition.InitialEquipmentsData[0].ID, actorDefinition.InitialEquipmentsData[1].ID, isMock: true);
							survivorModel.SetupMockTraits();
							ActorView.PrepareActor(survivorModel, isTransient: true);
							obj.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorHeroPreview;
							obj.OpenForModel(survivorModel);
						}
					}
					break;
				case "OPEN_GUILD_SHOP":
					GuildShopPopup.OpenGuildShop();
					break;
				default:
					if (Item.NavigationLink.StartsWith("http://") || Item.NavigationLink.StartsWith("https://"))
					{
						Application.OpenURL(Item.NavigationLink);
					}
					else
					{
						Debug.LogError("DeepLinkNavigation: Unsupported DeepLink Action: " + Item.NavigationLink);
					}
					break;
				}
			}
		}

		public static bool HandleNativeDeepLink(string deepLink)
		{
			return GameManager.Instance.HandleNativeDeeplinkUrl(deepLink);
		}

		public static bool IsDeepLinkAccessable(PlayerHubNewsItem item)
		{
			if (item == null)
			{
				Debug.LogError("ValidateDeepLink: NULL Item");
				return false;
			}
			if (!string.IsNullOrEmpty(item.NavigationLink) && item.NavigationLink == "OPEN_BUNDLE")
			{
				if (GameManager.Instance == null || GameManager.Instance.playerModel.BundleManager == null)
				{
					return false;
				}
				string attributeValue = item.GetAttributeValue(PlayerHubNewsItem.AttributeTag.EntryId);
				BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(attributeValue);
				if (bundleStoreDefinition == null || !GameManager.Instance.playerModel.BundleManager.CanBuyBundle(bundleStoreDefinition))
				{
					return false;
				}
			}
			return true;
		}
	}
}
