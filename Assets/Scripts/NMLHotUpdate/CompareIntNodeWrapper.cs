using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Logic)]
public class CompareIntNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public CompareIntNode NodeBaseInternal = new CompareIntNode();
}
