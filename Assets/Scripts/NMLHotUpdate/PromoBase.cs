using TWD.Externals;
using UnityEngine;

public class PromoBase : MonoBehaviourExtended
{
	[SerializeField]
	private UIButtonExtended mainButton;

	protected PlayerHubNewsItem cachedItem;

	public PlayerHubNewsItem CachedItem => cachedItem;

	public virtual void Awake()
	{
		DebugIdString = "PromoBase";
	}

	public virtual void OnEnable()
	{
		if ((bool)mainButton)
		{
			mainButton.SetClickCallback(OnButtonClicked);
		}
	}

	public virtual void OnDisable()
	{
		if (mainButton != null)
		{
			mainButton.Clear();
		}
	}

	public virtual void UpdateUIWithItem(PlayerHubNewsItem item)
	{
		if (item != null && cachedItem != item)
		{
			cachedItem = item;
		}
		UpdateUI();
	}

	public virtual void UpdateUI()
	{
		_ = cachedItem;
	}

	protected virtual void OnButtonClicked(UIButtonExtended button)
	{
		DebugLog("OnButtonClicked");
		if (cachedItem != null)
		{
			if (cachedItem.NavigationLink.Contains("youtube") || cachedItem.NavigationLink.Contains("youtu.be"))
			{
				string episodeId = GameManager.Instance.gameEconomyData.ConfigData.CurrentCampaign.ToString();
				GameManager.Instance.modelManager.Metrics.AddStart().AddSeasonVideo(episodeId, "mission_hub").Send();
			}
			DeepLinkNavigation.HandleItemDeepLinkClick(cachedItem);
		}
	}
}
