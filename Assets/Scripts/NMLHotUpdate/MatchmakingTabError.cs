public class MatchmakingTabError : MatchmakingTabBase
{
	public void OnClickTryAgain()
	{
		if (base.ParentPopup != null)
		{
			base.ParentPopup.QueryMatchMaking();
		}
	}

	public override void OnClickClose()
	{
		base.OnClickClose();
		MissionHubNavigation.TryOpenOutpost();
	}
}
