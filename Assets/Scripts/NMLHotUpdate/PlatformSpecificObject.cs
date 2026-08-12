using UnityEngine;

public class PlatformSpecificObject : MonoBehaviour
{
	[Tooltip("Which platform type to check against")]
	public PlatformFlag PlatformCondition;

	[Tooltip("Should the object be disabled when platform matches?")]
	public bool DisabledOnPlatform;

	public void Start()
	{
		if (PlatformInfo.HasFlag(PlatformCondition) && DisabledOnPlatform)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
