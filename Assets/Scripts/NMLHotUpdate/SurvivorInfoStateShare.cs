public class SurvivorInfoStateShare : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorShare;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(base.CloseButton, value: false);
	}

	public override void Enter()
	{
		base.Enter();
	}
}
