using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class RandomOneOfThreeNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public RandomOneOfThreeNode NodeBaseInternal = new RandomOneOfThreeNode();
}
