using System.Collections;
using System.Linq;
using BaseModel.ContentTypes;
using Client.BlackMarket;
using Client.Connectivity;
using TWDModel;
using UnityEngine;
using UnityEngine.Events;

public class AdRewardButton : MonoBehaviour
{
	[SerializeField]
	private UnityEvent updateUI;

	[SerializeField]
	private AdUsage adUsage;

	[SerializeField]
	private AdProvider adProvider;

	[SerializeField]
	private UILabel speedUpAdLabel;

	[SerializeField]
	private UISprite adIconSprite;

	[SerializeField]
	private string disabledAdIconName = "Ui_Icon_Cinema_Disabled";

	[SerializeField]
	private string enabledAdIconName = "Ui_Icon_Cinema_Enabled";

	private bool isAdPlaying;

	private float adPlayStartTime;

	private BuildingModel currentUpgradedBuildingModel;

	private Coroutine waitCommandCoroutine;

	public void OnEnable()
	{
		EventManager.OnEvent += OnEvent;
		if (waitCommandCoroutine != null)
		{
			StartWaitCommandQueueCoroutine();
		}
		if (adUsage == AdUsage.BuildUpgradeSpeedUp)
		{
			currentUpgradedBuildingModel = CampView.Instance.CampViewBuildings.SelectedBuilding.Model;
		}
	}

	public void OnDisable()
	{
		EventManager.OnEvent -= OnEvent;
	}

	public void Update()
	{
		UpdateAdButtonDetails();
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.VideoWatched)
		{
			OnVideoWatched((bool)parameter);
		}
	}

	public void OnClickWatchAddButton()
	{
		if (isAdPlaying && Time.time > adPlayStartTime + 60f)
		{
			isAdPlaying = false;
		}
		if (SingularityMonoBehaviour<VideoAdManager>.Instance.IsPlaying)
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.AdShowFailed"));
			SingularityMonoBehaviour<VideoAdManager>.Instance.CancelAd(adUsage);
			updateUI?.Invoke();
		}
		else if (GameManager.Instance.HasAnsweredTargetedAdsConsentQuestion())
		{
			StartPlayingAd();
		}
		else
		{
			GameManager.Instance.AskForAdConsent(adUsage, StartPlayingAd, delegate
			{
				updateUI?.Invoke();
			});
		}
	}

	private void StartPlayingAd()
	{
		isAdPlaying = true;
		adPlayStartTime = Time.time;
		SingularityMonoBehaviour<VideoAdManager>.Instance.FadeOutAudio();
		SingularityMonoBehaviour<VideoAdManager>.Instance.PlayAd(adUsage);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnVideoWatched(bool completely)
	{
		if (!isAdPlaying)
		{
			Debug.LogError("Received OnVideoWatched even though not playing.");
			return;
		}
		isAdPlaying = false;
		if (!completely && !SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(adUsage))
		{
			updateUI?.Invoke();
		}
		else
		{
			AdRewardPlayer();
		}
	}

	private void StartWaitCommandQueueCoroutine()
	{
		if (waitCommandCoroutine != null)
		{
			StopCoroutine(waitCommandCoroutine);
		}
		else
		{
			waitCommandCoroutine = StartCoroutine(WaitCommandQueueToContinue());
		}
	}

	private IEnumerator WaitCommandQueueToContinue()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		while (SignalRClient.Instance.IsWaitingForResponse)
		{
			yield return null;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		updateUI?.Invoke();
		waitCommandCoroutine = null;
	}

	public void AdRewardPlayer()
	{
		isAdPlaying = false;
		switch (adUsage)
		{
		case AdUsage.CombatRewardKey:
			GiveLootKeys();
			break;
		case AdUsage.BuildUpgradeSpeedUp:
			SpeedUpBuildingBuiltTime();
			break;
		case AdUsage.RefreshBlackMarketSlot:
			RefreshBlackMarketSlot();
			break;
		}
		StartWaitCommandQueueCoroutine();
	}

	private void SpeedUpBuildingBuiltTime()
	{
		Helpers.ExecuteCommand(new GiveAdWatchedRewardCommand(adUsage, currentUpgradedBuildingModel, adProvider));
	}

	private void GiveLootKeys()
	{
		Helpers.ExecuteCommand(new GiveAdWatchedRewardCommand(adUsage));
	}

	private void RefreshBlackMarketSlot()
	{
		string actorDefinitionId = BlackMarketShopController.Instance.ActiveHero.ActiveActorDefinitionID;
		if (Helpers.ExecuteCommand(new GiveAdWatchedRewardCommand(adUsage, actorDefinitionId, adProvider)) == TWDModelResult.OK)
		{
			BlackMarketHeroSlot blackMarketHeroSlot = GameManager.Instance.playerModel.BlackMarket.Slots.FirstOrDefault((BlackMarketHeroSlot x) => x.ActiveActorDefinitionID == actorDefinitionId);
			BlackMarketShopController.Instance.RefreshedHero(blackMarketHeroSlot?.ActiveActorDefinitionID);
		}
	}

	private void UpdateAdButtonDetails()
	{
		if (CanUpdateUI())
		{
			UIButton component = base.gameObject.GetComponent<UIButton>();
			if (GameManager.Instance.ShouldAskForAdConsent() || SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(adUsage))
			{
				string adAvailableText = GetAdAvailableText();
				HelpersUI.SetContentToLabel(speedUpAdLabel, adAvailableText);
				HelpersUI.SetButtonState(component, UIButtonColor.State.Normal);
				HelpersUI.SetSprite(adIconSprite, enabledAdIconName);
			}
			else
			{
				HelpersUI.SetContentToLabel(speedUpAdLabel, Helpers.FormatTime(GetAdRechargeTimeLeft()));
				HelpersUI.SetButtonState(component, UIButtonColor.State.Disabled);
				HelpersUI.SetSprite(adIconSprite, disabledAdIconName);
			}
		}
	}

	private long GetAdRechargeTimeLeft()
	{
		long videoAdAvailabilityTimeByType = GameManager.Instance.playerModel.GetVideoAdAvailabilityTimeByType(adUsage);
		if (videoAdAvailabilityTimeByType <= 0)
		{
			return 0L;
		}
		return videoAdAvailabilityTimeByType;
	}

	private bool CanUpdateUI()
	{
		if (adUsage == AdUsage.BuildUpgradeSpeedUp)
		{
			if (currentUpgradedBuildingModel.NeedSpeedUpButton())
			{
				return GameManager.Instance.gameEconomyData.ConfigData.AdsBuildingSpeedUpEnabled;
			}
			return false;
		}
		if (adUsage == AdUsage.RefreshBlackMarketSlot)
		{
			return GameManager.Instance.gameEconomyData.ConfigData.AdsBlackMarketRefreshEnabled;
		}
		return false;
	}

	private string GetAdAvailableText()
	{
		string result = string.Empty;
		if (adUsage == AdUsage.BuildUpgradeSpeedUp)
		{
			long milliSeconds = (long)(currentUpgradedBuildingModel.OriginalUpgradeTimer * GameManager.Instance.gameEconomyData.ConfigData.AdsBuildingSpeedUpMultiplier);
			result = "-" + Helpers.FormatTime(milliSeconds);
		}
		if (adUsage == AdUsage.RefreshBlackMarketSlot)
		{
			result = LocalizationManager.GetText("Popup.ProgressionUpdate.Button.Video");
		}
		return result;
	}
}
