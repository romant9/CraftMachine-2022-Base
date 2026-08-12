using System.Collections.Generic;
using UnityEngine;

public class ConditionalNode : MonoBehaviour
{
	private static List<ConditionalNode> allStartedInstances = new List<ConditionalNode>();

	public static List<ConditionalNode> GetAllStartedInstances()
	{
		return allStartedInstances;
	}

	private void Start()
	{
		allStartedInstances.Add(this);
	}

	private void OnDestroy()
	{
		allStartedInstances.Remove(this);
	}

	protected virtual bool Check()
	{
		Debug.LogError("ConditionalNode.Check called. Only extending classes of ConditionalNode should be added as a component. Not the base class.");
		return false;
	}

	private void TriggerCondition(bool checkResult)
	{
		ConditionalResult[] components = base.gameObject.GetComponents<ConditionalResult>();
		for (int i = 0; i < components.Length; i++)
		{
			if (checkResult)
			{
				components[i].OnConditionTrue();
			}
			else
			{
				components[i].OnConditionFalse();
			}
		}
	}

	public void OnCombatStarted()
	{
		TriggerCondition(Check());
	}
}
