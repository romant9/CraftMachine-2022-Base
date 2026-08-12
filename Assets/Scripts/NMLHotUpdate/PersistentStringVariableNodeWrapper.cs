using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class PersistentStringVariableNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public PersistentStringVariableNode NodeBaseInternal = new PersistentStringVariableNode();
}
