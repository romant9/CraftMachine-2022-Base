using System.Collections;
using BaseModel.ContentTypes;
using TWDModel;
using UnityEngine;

public class AdPopup : HUDElement
{
	private const float secondsToWaitToContineAfterBoxOpened = 3f;

	private const float secondsToWaitForAfterFailure = 5f;

	[SerializeField]
	private GameObject visibleBeforeShow;

	[SerializeField]
	private GameObject visibleAfterShow;

	[SerializeField]
	private GameObject visibleAfterShowOld;

	[SerializeField]
	private GameObject visibleAfterReward;

	[SerializeField]
	private GameObject visibleAfterRewardNoMoreRewards;

	[SerializeField]
	private GameObject visibleAfterFailure;

	[SerializeField]
	private UIButton watchAnotherButton;

	[SerializeField]
	private UIButton watchAnotherAfterFailureButton;

	[SerializeField]
	private UIButton goToCampAfterFailureButton;

	[SerializeField]
	private UILabel amountOfPrizesLeft;

	[SerializeField]
	private UILabel moreRewardsAvailableInTimeLabel;

	[SerializeField]
	private UITable uITable;

	[SerializeField]
	private UISprite[] prizes;

	private bool isPlaying;

	private bool askedForAdConsent;

	private float playStartTime;

	private GameObject lootCard;

	private bool isAdsFlowLessClicksFeatureEnabled;

	public override void Open()
	{
		Helpers.ClearUnusedMemory(gcCollect: true);
		base.Open();
		CampView.Instance.EnableCampControls(enable: false);
		RewardScreenHandler.Instance.ShowScene(LootScreenType.Ad);
		if (CampView.Instance != null && CampView.Instance.IsShown)
		{
			CampView.Instance.Hud.ShowcampHudContainer(show: false);
			CampView.Instance.Hud.ShowcampUiContainer(show: false);
			CampView.Instance.Hud.UpdateGenericElementsAfterChange();
		}
		EventManager.OnEvent += OnEvent;
		isAdsFlowLessClicksFeatureEnabled = GameManager.Instance.playerModel.gameEconomyData.GetFeature("AdsFlowLessClicksImprovements").Enabled;
		PopulateRewardsFromTheGED();
		UpdateUI();
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.OnRewardBoxOpened += OnRewardBoxOpened;
		}
	}

	public override void Close()
	{
		base.Close();
		CampHUD.Get().PauseCurrencyMeters = false;
		if (CampView.Instance != null && CampView.Instance.IsShown && CampView.Instance.Hud != null)
		{
			CampView.Instance.Hud.ShowcampHudContainer(show: true);
			CampView.Instance.Hud.ShowcampUiContainer(show: true);
			CampView.Instance.Hud.UpdateGenericElementsAfterChange();
		}
		RewardScreenHandler.Instance.HideScene();
		CampView.Instance.EnableCampControls(enable: true);
		EventManager.OnEvent -= OnEvent;
		isPlaying = false;
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
		}
		Helpers.ClearUnusedMemory(gcCollect: true);
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype == EventManager.EventType.VideoWatched)
		{
			OnVideoWatched((bool)parameter);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		bool pendingVideoAdReward = GameManager.Instance.playerModel.PendingVideoAdReward;
		visibleBeforeShow.SetActive(!pendingVideoAdReward);
		if (isAdsFlowLessClicksFeatureEnabled)
		{
			visibleAfterShow.SetActive(pendingVideoAdReward);
			if (pendingVideoAdReward)
			{
				GetReward();
			}
		}
		else
		{
			visibleAfterShowOld.SetActive(pendingVideoAdReward);
		}
		visibleAfterReward.SetActive(value: false);
		visibleAfterRewardNoMoreRewards.SetActive(value: false);
		visibleAfterFailure.SetActive(value: false);
	}

	public void OnClickPlay()
	{
		if (isPlaying && Time.time > playStartTime + 15f)
		{
			isPlaying = false;
		}
		if (!isPlaying)
		{
			if (SingularityMonoBehaviour<VideoAdManager>.Instance.IsPlaying)
			{
				HUDNotification.Error(LocalizationManager.GetText("Error.AdShowFailed"));
				SingularityMonoBehaviour<VideoAdManager>.Instance.CancelAd(AdUsage.CinemaReward);
				Close();
			}
			else if (GameManager.Instance.HasAnsweredTargetedAdsConsentQuestion())
			{
				StartPlayingAd();
			}
			else
			{
				askedForAdConsent = true;
				GameManager.Instance.AskForAdConsent(AdUsage.CinemaReward, StartPlayingAd, OnNoAdsErrorShown);
			}
		}
	}

	public void StartPlayingAd()
	{
		isPlaying = true;
		askedForAdConsent = false;
		playStartTime = Time.time;
		visibleBeforeShow.SetActive(value: false);
		StartCoroutine(DelayShowCancelButton(5f));
		SingularityMonoBehaviour<VideoAdManager>.Instance.FadeOutAudio();
		SingularityMonoBehaviour<VideoAdManager>.Instance.PlayAd(AdUsage.CinemaReward);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void GetReward()
	{
		isPlaying = false;
		if (!isAdsFlowLessClicksFeatureEnabled)
		{
			visibleAfterShowOld.SetActive(value: false);
		}
		RewardScreenHandler.Instance.ShowAdsRewards();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnClickCancelWatchAnother()
	{
		isPlaying = false;
		SingularityMonoBehaviour<VideoAdManager>.Instance.CancelAd(AdUsage.CinemaReward);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		UpdateUI();
	}

	public void OnClickCancelGoToCamp()
	{
		isPlaying = false;
		SingularityMonoBehaviour<VideoAdManager>.Instance.CancelAd(AdUsage.CinemaReward);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Close();
	}

	private void OnRewardBoxOpened(GameObject box, LootEntry reward, LootEntry reward2)
	{
		if (reward2 != null)
		{
			Debug.LogError("AdPopup supports single reward boxes only (but non-null reward2 parameter was provided).");
		}
		lootCard = RewardScreenHandler.Instance.CreateLootCard(box, reward, base.transform);
		visibleAfterShow.SetActive(value: false);
		StartCoroutine(DelayShowButtonAfterRewards());
	}

	private IEnumerator DelayShowButtonAfterRewards()
	{
		yield return new WaitForSeconds(3f);
		CapData capData = GameManager.Instance.playerModel.GetCapData();
		if (capData != null)
		{
			int num = capData.TheaterSessionCap - GameManager.Instance.playerModel.VideoAdsServed;
			HelpersUI.SetContentToLabel(amountOfPrizesLeft, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Ads.Available{parameter}", num), num > 0);
		}
		else
		{
			Helpers.GameObjectSetActive(amountOfPrizesLeft, value: false);
		}
		bool flag = SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(AdUsage.CinemaReward);
		visibleAfterReward.SetActive(flag);
		visibleAfterRewardNoMoreRewards.SetActive(!flag);
		if (!flag)
		{
			HelpersUI.SetContentToLabel(moreRewardsAvailableInTimeLabel, LocalizationManager.GetText("Popup.Ads.TimeLeftForNewAds") + " [b]" + Helpers.FormatTimeWithoutSeconds(GameManager.Instance.playerModel.GetVideoAdAvailabilityTimeByType(AdUsage.CinemaReward)), SingularityMonoBehaviour<VideoAdManager>.Instance.GetAdAvailabilityWithoutCaps(AdUsage.CinemaReward));
		}
	}

	private IEnumerator DelayShowCancelButton(float delay = 0f)
	{
		yield return new WaitForSeconds(delay);
		if (isPlaying || askedForAdConsent)
		{
			visibleAfterFailure.SetActive(value: true);
			visibleAfterReward.SetActive(value: false);
			bool flag = SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(AdUsage.CinemaReward);
			watchAnotherAfterFailureButton.gameObject.SetActive(flag);
			goToCampAfterFailureButton.gameObject.SetActive(!flag);
		}
	}

	public void OnNoAdsErrorShown()
	{
		visibleBeforeShow.SetActive(value: false);
		StartCoroutine(DelayShowCancelButton());
	}

	public void OnRewardConfirmed()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Close();
	}

	public void OnVideoWatched(bool completely)
	{
		if (!isPlaying)
		{
			Debug.LogError("Received OnVideoWatched even though not playing.");
			return;
		}
		isPlaying = false;
		if (!completely && !SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(AdUsage.CinemaReward))
		{
			Debug.LogWarning("OnVideoWatched() Not completely watched, ignoring.");
			Close();
		}
		UpdateUI();
	}

	public void OnWatchAnother()
	{
		isPlaying = false;
		RewardScreenHandler.Instance.HideScene();
		RewardScreenHandler.Instance.ShowScene(LootScreenType.Ad);
		Object.Destroy(lootCard);
		if (isAdsFlowLessClicksFeatureEnabled)
		{
			OnClickPlay();
		}
		else
		{
			UpdateUI();
		}
	}

	private void PopulateRewardsFromTheGED()
	{
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		DropCurrenciesProbabilitiesDefinition dropCurrenciesProbabilities = gameEconomyData.GetDropCurrenciesProbabilities(DropEventDefinition.DropEventType.VideoAd, DropType.Gold, DropEventDefinition.DropEventTag.VideoAds, GameManager.Instance.playerModel.Level);
		int num = 0;
		string[] array = new string[prizes.Length];
		if (num < prizes.Length && dropCurrenciesProbabilities.HeroTokenProbability > 0f)
		{
			FeaturedHeroDefinition activeFeaturedHero = gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);
			array[num] = HelpersGfx.GetCurrencyIconName((activeFeaturedHero != null) ? gameEconomyData.GetActorDefinition(activeFeaturedHero.ActorDefinitionID).TraitUpgradeCurrency : CurrencyType.DarylToken);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.GvGGasProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.GvGGas);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.ComponentProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.Badge4);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.PhoneProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.Phone);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.ClassTokenProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.AssaultToken);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.DiamondsProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.Diamonds);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.ReplayTokenProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.ReplayToken);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.SurvivalPointsProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.SurvivalPoints);
			num++;
		}
		if (num < prizes.Length && dropCurrenciesProbabilities.SuppliesProbability > 0f)
		{
			array[num] = HelpersGfx.GetCurrencyIconName(CurrencyType.Supplies);
			num++;
		}
		for (int i = 0; i < prizes.Length; i++)
		{
			int num2 = prizes.Length - 1 - i;
			if (!string.IsNullOrEmpty(array[num2]))
			{
				prizes[i].spriteName = array[num2];
			}
			else
			{
				prizes[i].gameObject.SetActive(value: false);
			}
		}
		uITable.Reposition();
	}
}
