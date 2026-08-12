using TWDModel;

public static class BlackboardUISeenToggle
{
	public static bool TryToOpen(UIType popupToOpen, string toggleToCheck, UIType[] uiBlockers = null)
	{
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn(toggleToCheck))
		{
			int num = ((uiBlockers != null) ? uiBlockers.Length : 0);
			for (int i = 0; i < num; i++)
			{
				if (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(uiBlockers[i]))
				{
					return false;
				}
			}
			HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(popupToOpen);
			if (hUDElement != null)
			{
				hUDElement.OnClose += delegate
				{
					Helpers.ExecuteCommandDelayed(new SetBlackboardToggleCommand(toggleToCheck));
				};
				hUDElement.Open();
				return true;
			}
		}
		return false;
	}
}
