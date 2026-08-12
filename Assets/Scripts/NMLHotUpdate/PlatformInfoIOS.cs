using UnityEngine;

public class PlatformInfoIOS : PlatformInfoBase
{
	protected override bool IsLowMemoryDevice()
	{
		return false;
	}

	protected override bool IsSDResolutionDevice()
	{
		if (Screen.width <= 1024)
		{
			return Screen.height <= 1024;
		}
		return false;
	}

	protected override bool IsSlowCPUDevice()
	{
		return false;
	}

	protected override bool IsSlowGPUDevice()
	{
		return false;
	}

	protected override bool SupportsStencil()
	{
		return true;
	}
}
