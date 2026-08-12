using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class AndNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public AndNode NodeBaseInternal = new AndNode();
}
