using System.Collections;
using UnityEngine;

public class PanelSocialButtons : MonoBehaviour
{
	[SerializeField]
	private UILabel googlePlayLabel;

	[SerializeField]
	private UILabel gamecenterLabel;

	[SerializeField]
	private GameObject gameCenterButtonParent;

	[SerializeField]
	private GameObject googlePlayButtonParent;

	[SerializeField]
	private ThingsToDoIndicator inboxIndicator;

	protected bool openingFacebook;

	protected bool connectingFacebook;

	private float timeUntilNextAuthCheck;

	private const float AuthCheckIntervalSeconds = 1f;

	private IEnumerator UpdateUIDelayed_Coroutine()
	{
		yield return null;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (inboxIndicator != null)
		{
			inboxIndicator.SetNumber(SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount);
		}
	}

	public void OnGameCenterConnect()
	{
		StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(!GameManager.Instance.GameCenterManager.Authenticated, delegate
		{
			UpdateUI();
		}));
	}

	public void OnGooglePlayConnect()
	{
		StartCoroutine(GameManager.Instance.GameCenterManager.ToggleConnect_Coroutine(!GameManager.Instance.GameCenterManager.Authenticated, delegate
		{
			UpdateUI();
		}));
	}

	private void Update()
	{
		timeUntilNextAuthCheck -= Time.deltaTime;
		if (timeUntilNextAuthCheck <= 0f)
		{
			GameManager.Instance.GameCenterManager.CheckAuthentication();
			UpdateUI();
			timeUntilNextAuthCheck = 1f;
		}
	}

	public void OnHelpshiftInbox()
	{
		SingularityMonoBehaviour<SDKManager>.Instance.ShowFAQs();
	}
}
