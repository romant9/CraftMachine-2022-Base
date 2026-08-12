using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Event)]
public class TriggerNodeWrapper : NodeBaseWrapper
{
	public TriggerView TriggerView;

	[HideInInspector]
	public TriggerNode NodeBaseInternal = new TriggerNode();
}
