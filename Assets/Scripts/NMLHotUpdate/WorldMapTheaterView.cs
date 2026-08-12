using BaseModel.ContentTypes;
using UnityEngine;

public class WorldMapTheaterView : MonoBehaviour
{
	[SerializeField]
	private GameObject availabGameObject;

	[SerializeField]
	private GameObject adAvailableGlow;

	[SerializeField]
	private GameObject adIndicatorPrefab;

	private AdAvailableIndicator indicator;

	private const float createIndicatorDelay = 1f;

	private float currentFloatDelay;

	private bool isAvailable
	{
		get
		{
			if (GameManager.Instance.ShouldAskForAdConsent() || SingularityMonoBehaviour<VideoAdManager>.Instance.GetAdAvailabilityWithoutCaps(AdUsage.CinemaReward))
			{
				return !TutorialView.Instance.Running;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		SetAvailable(isAvailable);
	}

	public void SetAvailable(bool available)
	{
		if (availabGameObject != null)
		{
			availabGameObject.SetActive(available);
		}
		if (adAvailableGlow != null)
		{
			adAvailableGlow.SetActive(GameManager.Instance.playerModel.IsVideoAdRewardAvailable(AdUsage.CinemaReward));
		}
		if (available)
		{
			CreateIndicator();
		}
		else if (indicator != null && indicator.gameObject != null)
		{
			indicator.gameObject.SetActive(value: false);
		}
	}

	private void CreateIndicator()
	{
		if (indicator == null && CampView.Instance != null)
		{
			_ = CampView.Instance.BuildingsHud != null;
		}
		if (indicator != null && indicator.gameObject != null)
		{
			indicator.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		currentFloatDelay -= Time.deltaTime;
		if (currentFloatDelay < 0f)
		{
			currentFloatDelay = 1f;
			SetAvailable(isAvailable);
		}
		if (indicator != null)
		{
			indicator.UpdateFollowTarget();
		}
	}
}
