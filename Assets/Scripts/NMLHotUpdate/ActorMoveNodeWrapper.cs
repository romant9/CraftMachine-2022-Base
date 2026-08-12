using TWDModel;
using UnityEngine;

public class ActorMoveNodeWrapper : NodeBaseWrapper
{
	public GameObject TargetPositionMarker;

	[HideInInspector]
	public ActorMoveNode NodeBaseInternal = new ActorMoveNode();
}
