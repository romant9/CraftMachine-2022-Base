using TWDModel;
using UnityEngine;

public class MoveTutorialNodeWrapper : NodeBaseWrapper
{
	public GameObject TargetPositionMarker;

	[HideInInspector]
	public MoveTutorialNode NodeBaseInternal = new MoveTutorialNode();
}
