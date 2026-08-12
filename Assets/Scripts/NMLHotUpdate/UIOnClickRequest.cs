using TWD.Externals;
using UnityEngine;

public class UIOnClickRequest : MonoBehaviour
{
	public enum TriggerStates
	{
		Disabled = 0,
		Trigger = 1,
		UIEvent = 2
	}

	[Header("Required")]
	[Header("Click trigger")]
	public TriggerStates Trigger;

	[Header("On click action")]
	public DeepLinkActions.ActionsEnum DeepLinkAction;

	[Space(25f)]
	[Header("Optional")]
	[Header("On click close popup")]
	public UIType OnClickClose = UIType.None; //GuildBattleHighscorePopup

	[Header("Set from this at Awake()")]
	public UIButtonExtended Button;

	private void Awake()
	{
		if (Button == null)
		{
			Button = GetComponent<UIButtonExtended>();
		}
	}

	private void OnEnable()
	{
		if (Button != null)
		{
			Button.SetClickCallback(OnClickCallback);
		}
	}

	private void OnDisable()
	{
		if (Button != null)
		{
			Button.Clear();
		}
	}

	protected virtual void OnClickCallback(UIButtonExtended target)
	{
		if (Trigger == TriggerStates.Trigger)
		{
			if (DeepLinkNavigation.HandleDeepLink(DeepLinkAction.ToString()))
			{
				CallOnClickClose();
			}
		}
		else if (Trigger == TriggerStates.UIEvent)
		{
			UIEvent.Send("OnClickedRequest", this);
			CallOnClickClose();
		}
	}

	protected virtual void CallOnClickClose()
	{
		HUDManager.TryClosePopup(OnClickClose);
	}
}
