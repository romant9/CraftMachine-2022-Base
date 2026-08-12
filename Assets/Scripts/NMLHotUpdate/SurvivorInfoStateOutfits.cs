public class SurvivorInfoStateOutfits : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorOutfits;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}

	public override void Enter()
	{
		base.Enter();
		if (base.SurvivorModel != null && base.SurvivorOutfitsView != null)
		{
			base.SurvivorOutfitsView.Show(base.SurvivorModel.OutfitDefinitionID);
		}
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Hide);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Hide);
	}

	public override void Exit()
	{
		base.Exit();
		if (base.SurvivorOutfitsView != null && base.SurvivorOutfitsView.CurrentOutfitDefinition != null && base.SurvivorOutfitsView.CurrentOutfitDefinition.ID != base.SurvivorModel.OutfitDefinitionID)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.close();
		}
	}
}
