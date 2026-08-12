using TWDModel;

public class PersistentIntConditionalNode : ConditionalNode
{
	public string ConditionVariableName = "";

	public ConditionOperator Operator;

	public int ConditionValue;

	protected override bool Check()
	{
		TWDModelManager modelManager = GameManager.Instance.modelManager;
		if (modelManager == null || modelManager.Player == null || modelManager.Player.Combat == null)
		{
			return false;
		}
		PersistentMissionVariableManager persistentMissionVariableManager = modelManager.Player.Combat.PersistentMissionVariableManager;
		if (persistentMissionVariableManager == null)
		{
			return false;
		}
		if (!persistentMissionVariableManager.DoesVariableExist(ConditionVariableName))
		{
			Debug.LogError("ConditionVariableName value invalid, no persistent int variable with name \"" + ConditionVariableName + "\" exists.");
			return false;
		}
		int intVariable = persistentMissionVariableManager.GetIntVariable(ConditionVariableName, 0);
		return IntCondition.Compare(Operator, intVariable, ConditionValue);
	}
}
