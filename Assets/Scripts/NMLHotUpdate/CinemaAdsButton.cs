using BaseModel.ContentTypes;
using TWDModel;
using UnityEngine;

public class CinemaAdsButton : MonoBehaviour
{
	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private GameObject cinemaDisabledSprite;

	[SerializeField]
	private GameObject cinemaEnabledSprite;

	[SerializeField]
	private GameObject freeGameObject;

	private float refreshTimer;

	private CapData capData;

	private PlayerModel playerInternal;

	private bool isAdAvailable;

	private bool isCapDataSet;

	private PlayerModel player
	{
		get
		{
			if (playerInternal == null)
			{
				playerInternal = GameManager.Instance.playerModel;
			}
			return playerInternal;
		}
	}

	private bool HasVideoLoaded
	{
		get
		{
			if ((GameManager.Instance.ShouldAskForAdConsent() || SingularityMonoBehaviour<VideoAdManager>.Instance.GetAdAvailabilityWithoutCaps(AdUsage.CinemaReward)) && !TutorialView.Instance.Running)
			{
				return SingularityMonoBehaviour<VideoAdManager>.Instance.IsVideoReadyForServe(AdUsage.CinemaReward);
			}
			return false;
		}
	}

	private void OnEnable()
	{
		Helpers.GameObjectSetActive(timerContainer, value: false);
		CheckIfAdIsAvailable();
	}

	private void CheckIfAdIsAvailable()
	{
		if (capData == null)
		{
			capData = player.GetCapData();
		}
		isAdAvailable = player.IsVideoAdRewardAvailable(AdUsage.CinemaReward) && capData != null && HasVideoLoaded;
		Helpers.GameObjectSetActive(cinemaEnabledSprite, isAdAvailable);
		Helpers.GameObjectSetActive(freeGameObject, isAdAvailable);
		Helpers.GameObjectSetActive(cinemaDisabledSprite, !isAdAvailable);
		if (!isAdAvailable)
		{
			Helpers.GameObjectSetActive(timerContainer, capData != null && HasVideoLoaded);
			if (capData != null && HasVideoLoaded)
			{
				isCapDataSet = capData.TheaterSessionLength * 60 * 1000 > 0;
				HelpersUI.SetContentToLabel(timerLabel, Helpers.FormatTime(GetTimeLeft()));
			}
			else
			{
				isCapDataSet = false;
			}
		}
		else
		{
			Helpers.GameObjectSetActive(timerContainer, value: false);
		}
	}

	private void Update()
	{
		if (isAdAvailable)
		{
			return;
		}
		refreshTimer -= Time.deltaTime;
		if (!(refreshTimer <= 0f))
		{
			return;
		}
		refreshTimer = 1f;
		if (!isCapDataSet)
		{
			CheckIfAdIsAvailable();
			return;
		}
		long timeLeft = GetTimeLeft();
		if (timeLeft <= 0)
		{
			CheckIfAdIsAvailable();
		}
		HelpersUI.SetContentToLabel(timerLabel, Helpers.FormatTime(timeLeft));
	}

	public void OnClick()
	{
		if (isAdAvailable)
		{
			CampView.Instance.CampViewBuildings.UnselectBuilding();
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AdPopupView).Open();
		}
		else if (!isCapDataSet)
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Ads.NoAdsOlduser.Title"), LocalizationManager.GetText("Popup.Ads.NoAdsOlduser.Content"), LocalizationManager.GetText("Popup.TOS.PrivacyPolicy.Content.Ok"));
		}
		else
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Generic.Info"), LocalizationManager.GetText("Popup.Ads.TimeLeftForNewAds") + "\n[b]" + Helpers.FormatTime(GetTimeLeft()), LocalizationManager.GetText("Popup.TOS.PrivacyPolicy.Content.Ok"));
		}
	}

	private long GetTimeLeft()
	{
		long videoAdAvailabilityTimeByType = player.GetVideoAdAvailabilityTimeByType(AdUsage.CinemaReward);
		if (videoAdAvailabilityTimeByType <= 0)
		{
			return 0L;
		}
		return videoAdAvailabilityTimeByType;
	}
}
