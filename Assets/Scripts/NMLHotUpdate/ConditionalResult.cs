using UnityEngine;

public class ConditionalResult : MonoBehaviour
{
	public virtual void OnConditionTrue()
	{
		Debug.LogError("ConditionalResult.OnConditionTrue called. Only extending classes of this script should be added as components.");
	}

	public virtual void OnConditionFalse()
	{
		Debug.LogError("ConditionalResult.OnConditionFalse called. Only extending classes of this script should be added as components.");
	}
}
