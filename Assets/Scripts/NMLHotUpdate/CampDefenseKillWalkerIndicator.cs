using TWDModel;

public class CampDefenseKillWalkerIndicator : HUDElementFollowTarget
{
	public ActorModel Walker { get; set; }

	public void OnClick()
	{
		if (TutorialView.Allowed("DefenseWalker"))
		{
			if (Walker != null)
			{
				Helpers.ExecuteCommand(new CampDefenseKillWalkerCommand(Walker));
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/walker_kill");
			}
			EventManager.NotifyClick("DefenseWalker");
		}
	}
}
