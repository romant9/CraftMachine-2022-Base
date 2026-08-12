using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Event)]
public class SurvivorNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public SurvivorNode NodeBaseInternal = new SurvivorNode();
}
