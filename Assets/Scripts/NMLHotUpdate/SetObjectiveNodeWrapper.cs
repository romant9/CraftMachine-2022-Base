using TWDModel;
using UnityEngine;

public class SetObjectiveNodeWrapper : NodeBaseWrapper
{
	public string ObjectiveText;

	public string CustomText1;

	public string CustomText2;

	[HideInInspector]
	public SetObjectiveNode NodeBaseInternal = new SetObjectiveNode();
}
