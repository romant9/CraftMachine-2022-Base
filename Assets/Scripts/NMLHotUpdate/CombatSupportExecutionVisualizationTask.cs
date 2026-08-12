using TWDModel;

public class CombatSupportExecutionVisualizationTask : VisualizationTask
{
	private readonly ExecuteCombatSupportAction executeCombatSupportAction;

	public CombatSupportExecutionVisualizationTask(ExecuteCombatSupportAction action)
		: base(action)
	{
		executeCombatSupportAction = action;
	}

	public override void Start()
	{
		if (GameManager.Instance.modelManager.CombatModel.SupportManager.TryGetSupport(executeCombatSupportAction.EquipIndex, out var combatSupportModel))
		{
			CombatSupportsView.Instance.SupportExecuted(combatSupportModel.SupportModel, executeCombatSupportAction.Target, executeCombatSupportAction.Targets);
		}
	}
}
