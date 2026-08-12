using BaseModel.ContentTypes;
using TWDModel;
using UnityEngine;

public class AdAvailableIndicator : HUDElementFollowTarget
{
	[SerializeField]
	private GameObject nextAdTimer;

	[SerializeField]
	private GameObject adAvailable;

	[SerializeField]
	private UILabel timer;

	private bool isCapDataSet;

	private float refreshTimer;

	private bool isAdAvailable = true;

	private PlayerModel playerInternal;

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

	public void OnClick()
	{
		CampView.Instance.CampViewBuildings.UnselectBuilding();
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AdPopupView).Open();
	}

	public void OnTimerClick()
	{
		AlertPopup.ShowPopup(LocalizationManager.GetText("Generic.Info"), LocalizationManager.GetText("Popup.Ads.TimeLeftForNewAds") + "\n[b]" + Helpers.FormatTime(GetTimeLeft()), LocalizationManager.GetText("Popup.TOS.PrivacyPolicy.Content.Ok"));
	}

	private void OnEnable()
	{
		CheckIfAdIsAvailable();
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
			HelpersUI.SetContentToLabel(timer, Helpers.FormatTime(0L));
			return;
		}
		long timeLeft = GetTimeLeft();
		if (timeLeft <= 0)
		{
			CheckIfAdIsAvailable();
		}
		HelpersUI.SetContentToLabel(timer, Helpers.FormatTime(timeLeft));
	}

	private void CheckIfAdIsAvailable()
	{
		CapData capData = player.GetCapData();
		isAdAvailable = player.IsVideoAdRewardAvailable(AdUsage.CinemaReward) && capData != null && SingularityMonoBehaviour<VideoAdManager>.Instance.IsVideoReadyForServe(AdUsage.CinemaReward);
		bool flag = !isAdAvailable && capData != null;
		Helpers.GameObjectSetActive(adAvailable, isAdAvailable);
		Helpers.GameObjectSetActive(nextAdTimer, flag);
		if (flag)
		{
			isCapDataSet = capData.TheaterSessionLength * 60 * 1000 > 0;
			HelpersUI.SetContentToLabel(timer, Helpers.FormatTime(GetTimeLeft()));
		}
		else if (isAdAvailable)
		{
			TweenManager.PlayTweenGroup(base.gameObject, 1);
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
