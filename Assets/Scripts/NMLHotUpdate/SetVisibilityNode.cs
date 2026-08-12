using TWDModel;
using UnityEngine;

public class SetVisibilityNode : ClientNodeBase
{
	public GameObject TargetObject;

	[GraphItInput("Enable", "")]
	public void Enable()
	{
		TargetObject.GetComponent<Renderer>().enabled = true;
	}

	[GraphItInput("Disable", "")]
	public void Disable()
	{
		TargetObject.GetComponent<Renderer>().enabled = false;
	}

	[GraphItInput("Flip", "")]
	public void Flip()
	{
		bool flag = TargetObject.GetComponent<Renderer>().enabled;
		TargetObject.GetComponent<Renderer>().enabled = !flag;
	}
}
