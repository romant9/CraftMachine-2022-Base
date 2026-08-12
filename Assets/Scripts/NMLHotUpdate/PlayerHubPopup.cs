using System.Collections.Generic;
using UnityEngine;

public class PlayerHubPopup : HUDElement
{
	[SerializeField]
	private ThingsToDoIndicator socialNotifications;

	[SerializeField]
	private ThingsToDoIndicator socialShareNotifications;

	[SerializeField]
	private UILabel socialShareLabel;

	[SerializeField]
	[Header("For profile button hiding")]
	private UIToggleMenu toggleMenu;

	private float _timeUntilNextCheckUnreadMessageCount;

	private const float _checkUnreadMessageCountIntervalSeconds = 1f;

	private void Awake()
	{
		UpdateDiscord();
	}

	public void UpdateDiscord()
	{
		Dictionary<ShareType, ShareModel> obtainedRewards = GameManager.Instance.playerModel.ShareManagerModel.ObtainedRewards;
		bool active = false;
		if (obtainedRewards.Count == 0)
		{
			active = true;
			socialShareLabel.text = LocalizationManager.GetText("Generic.Free");
		}
		socialShareNotifications.gameObject.SetActive(active);
	}

	public override void Open()
	{
		base.Open();
		UpdateIndicators();
	}

	public override void Close()
	{
		base.Close();
		CampView.Instance.Hud.UpdateIndicators();
	}

	public void UpdateIndicators(int forceAmount = -1)
	{
		socialNotifications.SetNumber((forceAmount == -1) ? SingularityMonoBehaviour<SDKManager>.Instance.ZendeskManager.UnreadMessageCount : forceAmount);
	}

	private new void Update()
	{
		_timeUntilNextCheckUnreadMessageCount -= Time.deltaTime;
		if (_timeUntilNextCheckUnreadMessageCount <= 0f)
		{
			_timeUntilNextCheckUnreadMessageCount = 1f;
			UpdateIndicators();
		}
	}
}
