using TWDModel;

public class ActionCameraNode : ClientNodeBase
{
	[GraphItInput("Enable", "")]
	public void EnableActionCamera()
	{
		ActionCamera.Instance.AllowedToActivate = true;
	}

	[GraphItInput("Disable", "")]
	public void DisableActionCamera()
	{
		ActionCamera.Instance.AllowedToActivate = false;
	}
}
