using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class ActorVariableNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public ActorVariableNode NodeBaseInternal = new ActorVariableNode();
}
