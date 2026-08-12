using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel.ContentTypes;
using TWDModel;
using UnityEngine;

namespace Client.BlackMarket
{
	public class BlackMarketShopController : MonoBehaviour
	{
		[SerializeField]
		private UILabel timeLeft;

		[SerializeField]
		private UITexture heroImage;

		[SerializeField]
		private UIButton closeButton;

		[SerializeField]
		private UIButton refreshButton;

		[SerializeField]
		private UIButton refreshButtonTooltipRefreshedOnce;

		[SerializeField]
		private UIButton refreshButtonTooltipShopClosing;

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UILabel refreshCost;

		[Header("Main List")]
		[SerializeField]
		private NUIScrollableList scrollableList;

		[SerializeField]
		private GameObject container;

		[SerializeField]
		private ShopPopup shopPopup;

		[SerializeField]
		private GameObject refreshBlackMarketHeroPopupFallBack;

		[SerializeField]
		private GameObject refreshBlackMarketHeroPopupVariant;

		[SerializeField]
		private GameObject blackMarketAdsNotification;

		private readonly WaitForSeconds wait = new WaitForSeconds(1f);

		private Coroutine timeLeftCoroutine;

		public const string TitleLocalizationKey = "Popup.Shop.BlackMarket.ShopTitle{Name}";

		public static BlackMarketShopController Instance;

		public BlackMarketHeroSlot ActiveHero;

		private void Awake()
		{
			Instance = this;
		}

		private void OnDisable()
		{
			if (timeLeftCoroutine != null)
			{
				StopCoroutine(timeLeftCoroutine);
			}
			timeLeftCoroutine = null;
		}

		public void UpdateUI()
		{
			string heroSeasonIDArt = GameManager.Instance.gameEconomyData.BlackMarketHeroDefinitions.First((BlackMarketHeroDefinition x) => x.ActorDefinitionID == ActiveHero.ActiveActorDefinitionID).HeroSeasonIDArt;
			HelpersGfx.SetSeasonHeroMaterial(heroImage, heroSeasonIDArt);
			refreshCost.text = GameManager.Instance.gameEconomyData.ConfigData.BlackMarketRefreshCost.ToString();
			string text = GameManager.Instance.gameEconomyData.GetActorDefinition(ActiveHero.ActiveActorDefinitionID).Name;
			title.text = LocalizationManager.GetText("Popup.Shop.BlackMarket.ShopTitle{Name}", text);
			if (timeLeftCoroutine == null)
			{
				timeLeftCoroutine = StartCoroutine(UpdateTimeLeft());
			}
			else
			{
				UpdateTimer();
			}
			UpdateAdsNotification();
		}

		private IEnumerator UpdateTimeLeft()
		{
			while (true)
			{
				UpdateTimer();
				yield return wait;
			}
		}

		private void UpdateTimer()
		{
			refreshButtonTooltipRefreshedOnce.gameObject.SetActive(value: false);
			refreshButtonTooltipShopClosing.gameObject.SetActive(value: false);
			string text = Helpers.FormatTime(ActiveHero.NextUpdate - GameManager.Instance.playerModel.UtcTimeStamp);
			timeLeft.text = text;
			if (ActiveHero.CanRefresh(GameManager.Instance.playerModel))
			{
				HelpersUI.SetButtonState(refreshButton, UIButtonColor.State.Normal);
				return;
			}
			HelpersUI.SetButtonState(refreshButton, UIButtonColor.State.Disabled);
			if (ActiveHero.RefreshCounter > 0)
			{
				refreshButtonTooltipRefreshedOnce.gameObject.SetActive(value: true);
			}
			else
			{
				refreshButtonTooltipShopClosing.gameObject.SetActive(value: true);
			}
		}

		public void HideContent()
		{
			container.SetActive(value: false);
			ShopPopupHelper.UpdateListWithData(scrollableList, new List<BundleStoreDefinition>(), resetScrollPosition: true, isTabsIndexFeaturedShop: false);
		}

		public void OnCloseEventHandler()
		{
			HideContent();
			shopPopup.ShowMainContent();
			shopPopup.UpdateCardUIs();
		}

		public void OnRefreshEventHandler()
		{
			GameObject prefabVariant = ((GameManager.Instance.playerModel.gameEconomyData.ConfigData.AdsBlackMarketRefreshEnabled && SingularityMonoBehaviour<VideoAdManager>.Instance.IsVideoReadyForServe(AdUsage.RefreshBlackMarketSlot)) ? refreshBlackMarketHeroPopupVariant : refreshBlackMarketHeroPopupFallBack);
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RefreshBlackMarketHero, null, createIfNotExist: true, prefabVariant) as RefreshBlackMarketHeroPopup)?.OpenFor(ActiveHero);
		}

		public void OnTooltipRefreshedOnceEventHandler()
		{
			TooltipManager.OpenTextBoxWithText(refreshButtonTooltipRefreshedOnce.gameObject, LocalizationManager.GetText("Popup.Shop.BlackMarket.Refresh.Disabled.RefreshedOnce"));
		}

		public void OnTooltipShopClosingEventHandler()
		{
			TooltipManager.OpenTextBoxWithText(refreshButtonTooltipRefreshedOnce.gameObject, LocalizationManager.GetText("Popup.Shop.BlackMarket.Refresh.Disabled.ShopClosing"));
		}

		public void ShowFor(BlackMarketHeroSlot blackMarketHeroSlot)
		{
			ActiveHero = blackMarketHeroSlot;
			UpdateUI();
			List<BlackMarketDefinition> activeOffers = blackMarketHeroSlot.GetActiveOffers(GameManager.Instance.gameEconomyData);
			ShopPopupHelper.UpdateListWithData(scrollableList, activeOffers, resetScrollPosition: true, isTabsIndexFeaturedShop: false);
			shopPopup.HideMainContent();
			container.SetActive(value: true);
			scrollableList.SortAndReset();
			SetHasSeenSlot(blackMarketHeroSlot.ActiveActorDefinitionID, seen: true);
		}

		public void RefreshedHero(string newActorDefinitionId)
		{
			SetHasSeenSlot(newActorDefinitionId, seen: false);
			shopPopup.UpdateSelectedTab();
		}

		private void SetHasSeenSlot(string actorDefinitionId, bool seen)
		{
			PlayerPrefs.SetInt(GetPlayerPrefsSeenKey(actorDefinitionId), seen ? 1 : 0);
		}

		private string GetPlayerPrefsSeenKey(string actorDefinitionId)
		{
			int num = 0;
			for (int i = 0; i < GameManager.Instance.playerModel.BlackMarket.Slots.Length; i++)
			{
				if (GameManager.Instance.playerModel.BlackMarket.Slots[i].ActiveActorDefinitionID == actorDefinitionId)
				{
					num = i;
					break;
				}
			}
			return "BlackMarketHasSeenSlot" + num;
		}

		public bool HasSeenOfferFor(string actorDefinitionId)
		{
			return PlayerPrefs.GetInt(GetPlayerPrefsSeenKey(actorDefinitionId), 0) == 1;
		}

		private void UpdateAdsNotification()
		{
			if (GameManager.Instance.gameEconomyData.ConfigData.AdsBlackMarketRefreshEnabled && SingularityMonoBehaviour<VideoAdManager>.Instance.IsVideoReadyForServe(AdUsage.RefreshBlackMarketSlot) && GameManager.Instance.playerModel.IsVideoAdRewardAvailable(AdUsage.RefreshBlackMarketSlot))
			{
				ActiveHero.CanRefresh(GameManager.Instance.playerModel);
			}
			else
				_ = 0;
			Helpers.GameObjectSetActive(blackMarketAdsNotification, value: false);
		}
	}
}
