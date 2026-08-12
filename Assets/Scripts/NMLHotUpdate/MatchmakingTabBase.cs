using UnityEngine;

public class MatchmakingTabBase : UIToggleContent
{
	[SerializeField]
	private UILabel DebugLabel;

	public MatchmakingPopup ParentPopup { get; set; }

	public void SetDebugText(string value)
	{
		if (DebugLabel != null)
		{
			DebugLabel.text = value;
		}
	}

	public virtual void OnClickClose()
	{
		if (ParentPopup != null)
		{
			ParentPopup.OnClickClose();
		}
		SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("global/match_search");
	}
}
