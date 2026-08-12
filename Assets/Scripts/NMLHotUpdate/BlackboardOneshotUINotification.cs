using TWDModel;
using UnityEngine;

public class BlackboardOneshotUINotification : MonoBehaviour
{
	private enum Prefix
	{
		None = 0,
		GuildBattleTutorialSeen = 1
	}

	[SerializeField]
	private Prefix prefix;

	[SerializeField]
	private string toggleId;

	[SerializeField]
	private GameObject toggleContent;

	[SerializeField]
	private bool triggerSeenOnEnable;

	private string blackboardId
	{
		get
		{
			if (prefix != Prefix.None && !string.IsNullOrEmpty(toggleId))
			{
				return prefix.ToString() + "." + toggleId;
			}
			return "";
		}
	}

	public void TriggerNotificationSeen()
	{
		if (!HasSeenNotification() && !string.IsNullOrEmpty(blackboardId))
		{
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand(blackboardId));
		}
	}

	private void OnEnable()
	{
		Helpers.GameObjectSetActive(toggleContent, !HasSeenNotification());
		if (triggerSeenOnEnable)
		{
			TriggerNotificationSeen();
		}
	}

	private bool HasSeenNotification()
	{
		if (string.IsNullOrEmpty(blackboardId))
		{
			return true;
		}
		if (GameManager.Instance == null)
		{
			return true;
		}
		return GameManager.Instance.playerModel.Blackboard.IsToggleOn(blackboardId);
	}
}
