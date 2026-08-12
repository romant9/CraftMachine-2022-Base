using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Event)]
public class AtTurnNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public AtTurnNode NodeBaseInternal = new AtTurnNode();
}
