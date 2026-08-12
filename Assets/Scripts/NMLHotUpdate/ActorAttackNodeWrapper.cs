using TWDModel;
using UnityEngine;

public class ActorAttackNodeWrapper : NodeBaseWrapper
{
	public GameObject TargetPositionMarker;

	[HideInInspector]
	public ActorAttackNode NodeBaseInternal = new ActorAttackNode();
}
