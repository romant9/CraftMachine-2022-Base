using UnityEngine;

public class PlatformSpecificImageEffectsController : MonoBehaviour
{
	[Tooltip("Which platform type to check against")]
	public PlatformFlag PlatformCondition;

	[Tooltip("Should the object be disabled when platform matches?")]
	public bool DisabledOnPlatform;

	private AmplifyColorEffect imageEffect;

	public AmplifyColorEffect ImageEffect
	{
		get
		{
			if (imageEffect == null)
			{
				imageEffect = GetComponent<AmplifyColorEffect>();
			}
			return imageEffect;
		}
	}

	public void Start()
	{
		if (PlatformInfo.HasFlag(PlatformCondition) && DisabledOnPlatform)
		{
			ImageEffect.enabled = false;
		}
	}

	public bool EnableImageEffectIfAvailable(bool visibility)
	{
		bool result = !PlatformInfo.HasFlag(PlatformCondition) || !DisabledOnPlatform;
		if (ImageEffect == null)
		{
			result = false;
		}
		else
		{
			ImageEffect.enabled = visibility;
		}
		return result;
	}
}
