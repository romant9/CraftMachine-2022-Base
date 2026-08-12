using System;
using System.Collections;
using BaseModel.ContentTypes;
using UnityEngine;

namespace TWD.Externals
{
	public class VideoAdController : SingularityMonoBehaviour<VideoAdController>
	{
		public Action<UnityAdsShowCompletionState> onAdFinished;

		public Action onAdStarted;

		public Action<bool> onAdReady;

		public Action onUityAdsInitialized;

		private const string MaxSdkKey = "2gfrzsz5MUk5PiMTdgivPzEW0hVtq8Fspwhnmes77aCXIZ_wFhpef29fkfWTonOah5MSkDZsonO5KO0Z5pDlrM";

		private bool isPlayinging_AL;

		private int rewardedRetryAttempt;

		private string RewardedAdUnitId => GetRewardedAdUnitId();

		private void AdLog(string message)
		{
		}

		private void AdLogError(string message)
		{
		}

		public void Start()
		{
			AdLog("Start");
		}

		public void Initialize(bool testMode, string gameId, string consent)
		{
			AdLog("Initialize");
			ApplovinInit();
		}

		public void ShowVideoAd(string placementId)
		{
			AdLog("ShowVideoAd");
			ShowRewardedVideo(placementId);
		}

		private void ShowRewardedVideo(string _placementId)
		{
			AdLog("ShowRewardedVideo");
			if (Application.isEditor)
			{
				onAdStarted();
				onAdFinished(UnityAdsShowCompletionState.COMPLETED);
			}
		}

		public IEnumerator LoadPlacement(string placement)
		{
			yield return null;
		}

		public bool IsVideoPlaying()
		{
			return false;
		}

		public bool IsVideoAdReady(AdUsage adUsage)
		{
			bool result = false;
			AdLog("IsVideoAdReady:" + result);
			return result;
		}

		private string GetRewardedAdUnitId()
		{
			string text = "";
			switch (GameManager.ActiveBranch)
			{
			case "develop":
			case "feature":
			case "offline":
				return "bd0e414fd61ebc3a";
			case "staging":
			case "staging-pay":
				return "4db22aaada1dd509";
			case "test":
			case "test-lv":
				return "041a3bad71c140a7";
			case "release-lv":
				return "";
			default:
				return "0d5a00f41de18358";
			}
		}

		private void ApplovinInit()
		{
		}

		private void InitializeRewardedAds()
		{
		}
	}
}
