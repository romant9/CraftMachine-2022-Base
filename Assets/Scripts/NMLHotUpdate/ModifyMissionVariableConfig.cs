using TWDModel;

public class ModifyMissionVariableConfig : RunLocationItem
{
	public ModifyMissionVariableOperation VariableOperation;

	public string VariableName;

	public int Value;

	public override TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		if (VariableName != null && VariableName.Length > 0)
		{
			ModifyMissionVariableModel modifyMissionVariableModel = new ModifyMissionVariableModel();
			modifyMissionVariableModel.VariableHash = VariableName.GetHashCode();
			modifyMissionVariableModel.Value = Value;
			modifyMissionVariableModel.VariableOperation = VariableOperation;
			runLocation.AddModelObject(modifyMissionVariableModel);
			return modifyMissionVariableModel;
		}
		Debug.LogError("No variable name specified in ModifyMissionVariableConfig '" + this?.ToString() + "'!");
		return null;
	}
}
