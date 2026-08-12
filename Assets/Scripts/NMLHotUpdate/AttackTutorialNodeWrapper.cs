using TWDModel;
using UnityEngine;

public class AttackTutorialNodeWrapper : NodeBaseWrapper
{
	public GameObject TargetPositionMarker;

	[HideInInspector]
	public AttackTutorialNode NodeBaseInternal = new AttackTutorialNode();
}
