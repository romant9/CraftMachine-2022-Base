using TWDModel;
using UnityEngine;

public class StartDialogNodeWrapper : NodeBaseWrapper
{
	public CombatDialogPlayerView DialogPlayerView;

	[HideInInspector]
	public StartDialogNode NodeBaseInternal = new StartDialogNode();
}
