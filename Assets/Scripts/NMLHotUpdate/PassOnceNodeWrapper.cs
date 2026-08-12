using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class PassOnceNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public PassOnceNode NodeBaseInternal = new PassOnceNode();
}
