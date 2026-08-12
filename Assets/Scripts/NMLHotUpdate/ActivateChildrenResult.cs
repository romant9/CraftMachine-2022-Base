using UnityEngine;

public class ActivateChildrenResult : ConditionalResult
{
	public bool ActivateChildrenOnTrue;

	public bool DeactivateChildrenOnTrue;

	public bool ActivateChildrenOnFalse;

	public bool DeactivateChildrenOnFalse = true;

	private void SetChildrenActive(bool active)
	{
		foreach (Transform item in base.transform)
		{
			Helpers.GameObjectSetActive(item.gameObject, active);
		}
	}

	public override void OnConditionTrue()
	{
		if (ActivateChildrenOnTrue)
		{
			SetChildrenActive(active: true);
		}
		if (DeactivateChildrenOnTrue)
		{
			SetChildrenActive(active: false);
		}
	}

	public override void OnConditionFalse()
	{
		if (ActivateChildrenOnFalse)
		{
			SetChildrenActive(active: true);
		}
		if (DeactivateChildrenOnFalse)
		{
			SetChildrenActive(active: false);
		}
	}
}
