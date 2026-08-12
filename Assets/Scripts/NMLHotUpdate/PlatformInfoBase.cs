using UnityEngine;

public abstract class PlatformInfoBase
{
	public bool HasFlag(PlatformFlag flag)
	{
		return flag switch
		{
			PlatformFlag.Editor => Application.isEditor, 
			PlatformFlag.LowMemory => IsLowMemoryDevice(), 
			PlatformFlag.SDResolution => IsSDResolutionDevice(), 
			PlatformFlag.SlowCPU => IsSlowCPUDevice(), 
			PlatformFlag.SlowGPU => IsSlowGPUDevice(), 
			PlatformFlag.SupportsStencil => SupportsStencil(), 
			PlatformFlag.IPhoneX => IsIPhoneX(), 
			_ => false, 
		};
	}

	public virtual bool GetLimitedScreenSize(out int w, out int h)
	{
		w = (h = 0);
		return false;
	}

	protected abstract bool IsLowMemoryDevice();

	protected abstract bool IsSDResolutionDevice();

	protected abstract bool IsSlowCPUDevice();

	protected abstract bool IsSlowGPUDevice();

	protected abstract bool SupportsStencil();

	protected bool IsIPhoneX()
	{
		return false;
	}
}
