using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class LoopableClip : PlayableAsset, ITimelineClipAsset
{
	[HideInInspector]
	public LoopableBehaviour Template = new LoopableBehaviour();

	public TimelineAction Action;

	public Condition CurrentCondition;

	public string MarkerToJumpTo;

	public string MarkerLabel;

	public float TimeToJumpTo;

	public ClipCaps clipCaps => ClipCaps.None;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<LoopableBehaviour> scriptPlayable = ScriptPlayable<LoopableBehaviour>.Create(graph, Template);
		LoopableBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.MarkerToJumpTo = MarkerToJumpTo;
		behaviour.Action = Action;
		behaviour.CurrentCondition = CurrentCondition;
		behaviour.MarkerLabel = MarkerLabel;
		behaviour.TimeToJumpTo = TimeToJumpTo;
		return scriptPlayable;
	}
}
