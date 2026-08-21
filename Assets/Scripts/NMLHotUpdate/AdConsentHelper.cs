using System;
using System.Collections;

public static class AdConsentHelper
{
	public const bool IsMaxConsentFlowEnabled = true;

	public static bool IsAdsConsentHandledByUmp => true;

	public static bool IsMaxSdkInitialized => false;

	public static bool ShouldOfferPrivacyOptions
	{
		get
		{
			_ = IsMaxSdkInitialized;
			return false;
		}
	}

	public static void ShowPrivacyOptionsForExistingUser(Action onCompleted = null, bool reloadAds = true)
	{
	}

	private static void RunOnMainThread(Action action)
	{
		if (action == null)
		{
			return;
		}
		try
		{
			if (SingularityMonoBehaviour<SDKManager>.Instance != null && SingularityMonoBehaviour<SDKManager>.Instance.UnityMainThreadDispatcher != null)
			{
				SingularityMonoBehaviour<SDKManager>.Instance.UnityMainThreadDispatcher.Enqueue(action);
				return;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[Ads] Enqueue via UnityMainThreadDispatcher failed: " + ex);
		}
		try
		{
			if (GameManager.Instance != null)
			{
				GameManager.Instance.StartCoroutine(RunOnMainThreadNextFrame(action));
				return;
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("[Ads] StartCoroutine main-thread hop failed: " + ex2);
		}
		Debug.LogError("[Ads] CMP callback on background thread but no main-thread runner; skip Unity UI/ads work.");
	}

	private static IEnumerator RunOnMainThreadNextFrame(Action action)
	{
		yield return null;
		action();
	}
}
