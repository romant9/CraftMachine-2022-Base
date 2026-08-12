using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class SetActorVariableNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public SetActorVariableNode NodeBaseInternal = new SetActorVariableNode();
}
