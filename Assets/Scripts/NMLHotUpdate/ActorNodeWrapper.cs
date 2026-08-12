using TWDModel;
using UnityEngine;

public class ActorNodeWrapper : NodeBaseWrapper
{
	public int ActorTagHash;

	public Faction ActorFaction;

	public bool ActorIsBoss;

	[HideInInspector]
	public ActorNode NodeBaseInternal = new ActorNode();
}
