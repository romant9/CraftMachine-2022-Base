using TWDModel;

public class EndCombatVisualizationTask : VisualizationTask
{
	public override bool IsGlobalBlocker => true;

	private float DelayTimer { get; set; }

	private ECombatResult Result { get; set; }

	public EndCombatVisualizationTask(ECombatResult result, float delay = 1f)
		: base(null)
	{
		Result = result;
		DelayTimer = delay;
	}

	public override bool Update(float deltaTime)
	{
		DelayTimer -= deltaTime;
		if (DelayTimer <= 0f)
		{
			CombatView.Instance.RequestEndCombat(Result);
			return false;
		}
		return true;
	}
}
