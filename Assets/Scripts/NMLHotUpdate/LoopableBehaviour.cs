using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class LoopableBehaviour : PlayableBehaviour
{
	public TimelineAction Action;

	public Condition CurrentCondition;

	public string MarkerToJumpTo;

	public string MarkerLabel;

	public float TimeToJumpTo;

	[HideInInspector]
	public bool ClipExecuted;

	public bool ConditionMet()
	{
		return CurrentCondition switch
		{
			Condition.Input => Input.anyKeyDown, 
			Condition.Always => true, 
			Condition.Never => true, 
			_ => false, 
		};
	}
}
