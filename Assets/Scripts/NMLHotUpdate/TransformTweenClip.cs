using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TransformTweenClip : PlayableAsset, ITimelineClipAsset
{
	public TransformTweenBehaviour Template = new TransformTweenBehaviour();

	public ExposedReference<Transform> StartLocation;

	public ExposedReference<Transform> EndLocation;

	public ClipCaps clipCaps => ClipCaps.Blending;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<TransformTweenBehaviour> scriptPlayable = ScriptPlayable<TransformTweenBehaviour>.Create(graph, Template);
		TransformTweenBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.StartLocation = StartLocation.Resolve(graph.GetResolver());
		behaviour.EndLocation = EndLocation.Resolve(graph.GetResolver());
		return scriptPlayable;
	}
}
