using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class RandomChanceNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public RandomChanceNode NodeBaseInternal = new RandomChanceNode();
}
