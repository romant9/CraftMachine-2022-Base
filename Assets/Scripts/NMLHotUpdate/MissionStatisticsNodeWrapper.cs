using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Event)]
public class MissionStatisticsNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public MissionStatisticsNode NodeBaseInternal = new MissionStatisticsNode();
}
