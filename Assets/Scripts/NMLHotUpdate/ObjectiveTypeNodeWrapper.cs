using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Event)]
public class ObjectiveTypeNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public ObjectiveTypeNode NodeBaseInternal = new ObjectiveTypeNode();
}
