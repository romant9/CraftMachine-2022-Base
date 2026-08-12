public class SurvivorInfoStateMissionAccept : SurvivorInfoStateOverviewLimited
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivoreMissionAccept;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(base.CloseButton, value: false);
		if (Helpers.GameObjectSetActive(base.AcceptFromMissionParent, value: true))
		{
			bool flag = GameManager.Instance.playerModel.SurvivorContainer.CanAddSurvivor();
			if (Helpers.GameObjectSetActive(base.AcceptButton, value: true))
			{
				base.AcceptButton.isEnabled = flag;
			}
			base.AcceptFromMissionParent.UpdateWithSurvivor(base.SurvivorModel, flag);
		}
	}

	public override bool AllowExit()
	{
		return false;
	}

	protected override void UpdateAndShowBadges()
	{
		if (base.SurvivorRightSidePanel != null)
		{
			base.SurvivorRightSidePanel.SetActiveButtons(value: false);
		}
	}
}
