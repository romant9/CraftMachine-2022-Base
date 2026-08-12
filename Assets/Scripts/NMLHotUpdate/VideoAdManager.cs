using System;
using System.Collections;
using BaseModel;
using BaseModel.ContentTypes;
using Client.Connectivity;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class VideoAdManager : SingularityMonoBehaviour<VideoAdManager>
{
	public struct AdResult
	{
		public AdStatus Status;

		public string Message;

		public AdProvider Provider;

		public AdUsage Usage;
	}

	private string playerId;

	private bool initialized;

	private bool adsAvailable;

	private bool musicWasOn;

	private bool soundWasOn;

	private bool iPodWasOn;

	private float nextRequestTime;

	private float requestRetryTime = 60f;

	private float lastRequestTime;

	private int numberRequest;

	private int numberAnswer;

	private int lastSessionCount;

	private static int adsWatchedDuringSession;

	private static readonly float adNotAvailableTime = 50f;

	private const string PREFS_START_VIDEO_STATUS = "START_VIDEO_STATUS";

	private AdUsage currentAdUsage;

	private int lastCompletedCommandSequenceId = -1;

	public bool IsPlaying { get; private set; }

	public string DebugString => "adsAvailable: " + adsAvailable + "\nTime request: " + nextRequestTime + "/" + Time.time + "\nNumRequest: " + numberRequest + "\nnumberAnswer: " + numberAnswer + "\n";

	private bool IsInitialized => initialized;

	public event Action<bool> OnVideoClose;

	public void Init()
	{
		IsPlaying = false;
		if (initialized)
		{
			return;
		}
		TrySendEndVideoCommand();
		numberRequest = 0;
		numberAnswer = 0;
		initialized = true;
		playerId = GameManager.Instance.playerModel.HashedId;
		if (string.IsNullOrEmpty(playerId) && GameConfiguration.Instance.Config.OnlineLevel != BuildGameConfiguration.OnlineLevelType.Offline)
		{
			Debug.LogError("Can't initialize mediation because Player ID is empty");
		}
		else
		{
			if (string.IsNullOrEmpty(playerId))
			{
				playerId = "OfflineTestUser";
			}
			string gameId = GameManager.Instance.UnityAdsIds.UnityAdsGameIdAndroid;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				gameId = ((!Application.identifier.Contains("-lv")) ? GameManager.Instance.UnityAdsIds.UnityAdsGameIdIOS : GameManager.Instance.UnityAdsIds.UnityAdsGameIdIOSKorea);
			}
			SingularityMonoBehaviour<VideoAdController>.Instance.Initialize(!BuildConfiguration.Active.Branch.Contains("release"), gameId, GetConsent());
			VideoAdController videoAdController = SingularityMonoBehaviour<VideoAdController>.Instance;
			videoAdController.onUityAdsInitialized = (Action)Delegate.Combine(videoAdController.onUityAdsInitialized, (Action)delegate
			{
				StartCoroutine(AdsInitialized());
			});
			VideoAdController videoAdController2 = SingularityMonoBehaviour<VideoAdController>.Instance;
			videoAdController2.onAdReady = (Action<bool>)Delegate.Combine(videoAdController2.onAdReady, new Action<bool>(HandleAdAvailableToShow));
			VideoAdController videoAdController3 = SingularityMonoBehaviour<VideoAdController>.Instance;
			videoAdController3.onAdStarted = (Action)Delegate.Combine(videoAdController3.onAdStarted, new Action(HandleAdStarted));
			VideoAdController videoAdController4 = SingularityMonoBehaviour<VideoAdController>.Instance;
			videoAdController4.onAdFinished = (Action<UnityAdsShowCompletionState>)Delegate.Combine(videoAdController4.onAdFinished, (Action<UnityAdsShowCompletionState>)delegate(UnityAdsShowCompletionState result)
			{
				AdStatus status = AdStatus.OK;
				string message = null;
				if (result == UnityAdsShowCompletionState.SKIPPED)
				{
					status = AdStatus.Aborted;
					message = "UnityAds skipped";
				}
				if (result == UnityAdsShowCompletionState.UNKNOWN)
				{
					status = AdStatus.Error;
					message = "UnityAds failed to show video";
				}
				HandleAdFinished(new AdResult
				{
					Message = message,
					Status = status,
					Provider = AdProvider.UnityAds,
					Usage = currentAdUsage
				});
			});
			VideoAdController videoAdController5 = SingularityMonoBehaviour<VideoAdController>.Instance;
			videoAdController5.onAdReady = (Action<bool>)Delegate.Combine(videoAdController5.onAdReady, (Action<bool>)delegate(bool b)
			{
				HandleAdAvailability(AdProvider.UnityAds, b);
			});
		}
		if (GameManager.Instance.gameEconomyData.GetFeature("AdsShorterRequestRetryTime").Enabled)
		{
			requestRetryTime = 30f;
		}
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Combat != null && GameManager.Instance.playerModel.Combat.MissionCompleted)
		{
			nextRequestTime = Time.time;
		}
		else
		{
			nextRequestTime = Time.time + 20f;
		}
	}

	private IEnumerator AdsInitialized()
	{
		yield return StartCoroutine(SingularityMonoBehaviour<VideoAdController>.Instance.LoadPlacement(GetPlacementIdByUsageType(AdUsage.CinemaReward)));
		yield return StartCoroutine(SingularityMonoBehaviour<VideoAdController>.Instance.LoadPlacement(GetPlacementIdByUsageType(AdUsage.BuildUpgradeSpeedUp)));
		yield return StartCoroutine(SingularityMonoBehaviour<VideoAdController>.Instance.LoadPlacement(GetPlacementIdByUsageType(AdUsage.RefreshBlackMarketSlot)));
	}

	private string GetConsent()
	{
		return "true";
	}

	public bool IsAdAvailable(AdUsage usage)
	{
		if (!initialized)
		{
			return false;
		}
		if (!GameManager.Instance.HasAnsweredTargetedAdsConsentQuestion())
		{
			return false;
		}
		if (!GameManager.Instance.playerModel.IsVideoAdRewardAvailable(usage))
		{
			return false;
		}
		return SingularityMonoBehaviour<VideoAdController>.Instance.IsVideoAdReady(usage);
	}

	public void ClearAdAvailable()
	{
		adsAvailable = false;
	}

	public bool GetAdAvailabilityWithoutCaps(AdUsage usage)
	{
		if (!initialized)
		{
			return false;
		}
		if (!GameManager.Instance.HasAnsweredTargetedAdsConsentQuestion())
		{
			return false;
		}
		return SingularityMonoBehaviour<VideoAdController>.Instance.IsVideoAdReady(usage);
	}

	private bool IsVideoReady(AdUsage usage)
	{
		return SingularityMonoBehaviour<VideoAdController>.Instance.IsVideoAdReady(usage);
	}

	public bool IsVideoReadyForServe(AdUsage usage)
	{
		if (IsInitialized)
		{
			return IsVideoReady(usage);
		}
		return false;
	}

	public void PlayAd(AdUsage usage)
	{
		if (!IsAdAvailable(usage))
		{
			Debug.LogError("Failed to play ad: no ad available");
			OnAdClosed(completedAd: false, usage);
		}
		else if (!GameManager.Instance.HasAnsweredTargetedAdsConsentQuestion())
		{
			Debug.LogError("Tried to play ad without having player consent");
			OnAdClosed(completedAd: false, usage);
		}
		else if (!SingularityMonoBehaviour<VideoAdController>.Instance.IsVideoPlaying())
		{
			int counter = GameManager.Instance.playerModel.Blackboard.GetCounter("Counter.SessionPlayed");
			if (counter != lastSessionCount)
			{
				lastSessionCount = counter;
				adsWatchedDuringSession = 0;
			}
			lastRequestTime = 0f;
			adsWatchedDuringSession++;
			currentAdUsage = usage;
			ShowVideo(usage);
		}
		else
		{
			Debug.LogError("Trying to play ad while already playing.");
			OnAdClosed(completedAd: false, usage);
		}
	}

	public void CancelAd(AdUsage adUsage)
	{
		RestoreAudio();
		Helpers.ExecuteCommand(new CancelVideoAdRewardCommand(GameManager.Instance.playerModel, adUsage));
		IsPlaying = false;
	}

	public void FadeOutAudio()
	{
		if (!GameManager.Instance.Settings.IPodPlaying)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.SetMute(mute: true, "all");
			return;
		}
		GameManager.PauseIPodMusic();
		iPodWasOn = true;
	}

	public void RestoreAudio()
	{
		if (!iPodWasOn)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.SetMute(mute: false, "all");
			SingularityMonoBehaviour<AudioManager>.Instance.SetMute(!GameManager.Instance.Settings.MusicOn, "music");
			SingularityMonoBehaviour<AudioManager>.Instance.SetMute(!GameManager.Instance.Settings.SoundFxOn, "ambience");
		}
		else
		{
			GameManager.ResumeIPodMusic();
		}
	}

	private void OnAdClosed(bool completedAd, AdUsage adUsage)
	{
		if (completedAd)
		{
			SingularityMonoBehaviour<SDKManager>.Instance.AdsWatchClient(adUsage, "reward");
		}
		nextRequestTime = Time.time + requestRetryTime;
		IsPlaying = false;
		adUsage = AdUsage.Unknown;
		GameManager.Instance.IsReturningFromAds = false;
		if (!completedAd)
		{
			Helpers.ExecuteCommand(new CancelVideoAdRewardCommand(GameManager.Instance.playerModel, adUsage));
		}
		if (this.OnVideoClose != null)
		{
			this.OnVideoClose(completedAd);
		}
	}

	private void ShowVideo(AdUsage usage)
	{
		GameManager.Instance.IsReturningFromAds = true;
		PlayVideoAdCommand playVideoAdCommand = new PlayVideoAdCommand(GameManager.Instance.playerModel);
		playVideoAdCommand.Provider = AdProvider.UnityAds;
		playVideoAdCommand.AdsWatched = adsWatchedDuringSession;
		playVideoAdCommand.Usage = usage;
		SaveStartVideoStatus(playVideoAdCommand);
		if (Helpers.ExecuteCommand(playVideoAdCommand) != TWDModelResult.Error)
		{
			VideoAdWatchedCommand cmdToWait = new VideoAdWatchedCommand(GameManager.Instance.playerModel)
			{
				Usage = usage
			};
			GameManager.Instance.StartCoroutine(StartVideoAfterExecutingCommand(cmdToWait, usage));
		}
	}

	private void SaveStartVideoStatus(PlayVideoAdCommand command)
	{
		string value = GameManager.Instance.jsonSerializer.Serialize(command);
		TWDPlayerPrefs.SetString("START_VIDEO_STATUS", value);
		TWDPlayerPrefs.Save();
	}

	private void RemoveStartVideoStatus()
	{
		if (TWDPlayerPrefs.HasKey("START_VIDEO_STATUS"))
		{
			TWDPlayerPrefs.DeleteKey("START_VIDEO_STATUS");
			TWDPlayerPrefs.Save();
		}
	}

	private void TrySendEndVideoCommand()
	{
		if (TWDPlayerPrefs.HasKey("START_VIDEO_STATUS"))
		{
			PlayVideoAdCommand playVideoAdCommand = GameManager.Instance.jsonSerializer.Deserialize<PlayVideoAdCommand>(TWDPlayerPrefs.GetString("START_VIDEO_STATUS"));
			VideoAdFinishedAnalyticsCommand videoAdFinishedAnalyticsCommand = new VideoAdFinishedAnalyticsCommand();
			videoAdFinishedAnalyticsCommand.Provider = playVideoAdCommand.Provider;
			videoAdFinishedAnalyticsCommand.Status = AdStatus.Disconnected;
			videoAdFinishedAnalyticsCommand.Usage = playVideoAdCommand.Usage;
			GameManager.Instance.modelManager.ExecuteCommand(videoAdFinishedAnalyticsCommand);
			RemoveStartVideoStatus();
		}
	}

	private void HandleOnCommandCompletedMessage(int responsecode, int sequenceid)
	{
		lastCompletedCommandSequenceId = sequenceid;
	}

	private IEnumerator StartVideoAfterExecutingCommand(ModelCommand cmdToWait, AdUsage adUsage)
	{
		SignalRClient.Instance.OnCommandCompletedMessage += HandleOnCommandCompletedMessage;
		lastCompletedCommandSequenceId = -1;
		if (Helpers.ExecuteCommand(cmdToWait) == TWDModelResult.OK)
		{
			while (SignalRClient.Instance.IsConnected && lastCompletedCommandSequenceId < cmdToWait.SequenceId)
			{
				yield return null;
			}
			SignalRClient.Instance.OnCommandCompletedMessage -= HandleOnCommandCompletedMessage;
			if (!SingularityMonoBehaviour<VideoAdController>.Instance.IsVideoAdReady(adUsage))
			{
				OnAdClosed(completedAd: false, adUsage);
				yield break;
			}
			string placementIdByUsageType = GetPlacementIdByUsageType(adUsage);
			SingularityMonoBehaviour<VideoAdController>.Instance.ShowVideoAd(placementIdByUsageType);
			SingularityMonoBehaviour<SDKManager>.Instance.AdsWatchClient(adUsage, "start");
		}
		else
		{
			CancelAd(adUsage);
		}
	}

	private void HandleAdAvailableToShow(bool isReady)
	{
		numberAnswer++;
		adsAvailable = isReady;
	}

	private void HandleAdAvailability(AdProvider provider, bool available)
	{
		GameManager.Instance.StartCoroutine(DelayedSendAnalyticsEvent(provider, available));
	}

	public string GetPlacementIdByUsageType(AdUsage adUsage)
	{
		string result = GameManager.Instance.UnityAdsIds.UnityAdsAndroidPlacementData.Find((UnityAdsPlacementData x) => x.UsageType == adUsage).PlacementId;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			result = ((!Application.identifier.Contains("-lv")) ? GameManager.Instance.UnityAdsIds.UnityAdsIOSPlacementData.Find((UnityAdsPlacementData x) => x.UsageType == adUsage).PlacementId : GameManager.Instance.UnityAdsIds.UnityAdsIOSKoreaPlacementData.Find((UnityAdsPlacementData x) => x.UsageType == adUsage).PlacementId);
		}
		return result;
	}

	private IEnumerator DelayedSendAnalyticsEvent(AdProvider provider, bool available)
	{
		yield return null;
		VideoAdFillAnalyticsCommand videoAdFillAnalyticsCommand = new VideoAdFillAnalyticsCommand();
		videoAdFillAnalyticsCommand.Provider = provider;
		videoAdFillAnalyticsCommand.Available = available;
		GameManager.Instance.modelManager.ExecuteCommand(videoAdFillAnalyticsCommand);
	}

	private void HandleAdStarted()
	{
		IsPlaying = true;
	}

	private void HandleAdFinished(AdResult result)
	{
		if (result.Status == AdStatus.Error)
		{
			Debug.LogError("HandleAdFinished result=" + result.Status.ToString() + " message=" + result.Message);
			OnAdClosed(completedAd: true, result.Usage);
		}
		else if (result.Status == AdStatus.OK)
		{
			OnAdClosed(completedAd: true, result.Usage);
		}
		else if (result.Status == AdStatus.Aborted)
		{
			OnAdClosed(completedAd: false, result.Usage);
		}
		else
		{
			OnAdClosed(completedAd: true, result.Usage);
		}
		VideoAdFinishedAnalyticsCommand videoAdFinishedAnalyticsCommand = new VideoAdFinishedAnalyticsCommand();
		videoAdFinishedAnalyticsCommand.Provider = result.Provider;
		videoAdFinishedAnalyticsCommand.Status = result.Status;
		videoAdFinishedAnalyticsCommand.Usage = result.Usage;
		GameManager.Instance.modelManager.ExecuteCommand(videoAdFinishedAnalyticsCommand);
		RemoveStartVideoStatus();
	}
}
