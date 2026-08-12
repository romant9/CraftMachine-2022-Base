using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class PersistentIntVariableNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public PersistentIntVariableNode NodeBaseInternal = new PersistentIntVariableNode();
}
