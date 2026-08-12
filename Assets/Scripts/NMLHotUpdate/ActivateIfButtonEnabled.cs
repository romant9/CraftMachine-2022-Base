using UnityEngine;

public class ActivateIfButtonEnabled : MonoBehaviourExtended
{
	[SerializeField]
	private UIButtonExtended Button;

	[SerializeField]
	private GameObject Target;

	private void Start()
	{
		UpdateInternal();
	}

	private void Update()
	{
		UpdateInternal();
	}

	private void UpdateInternal()
	{
		if (Button != null && Button.gameObject != null && Target != null)
		{
			Helpers.GameObjectSetActive(Target, Button.gameObject.activeSelf && Button.isEnabled && !Button.IsVisuallyDisabled);
		}
	}
}
