using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.7366781f, 0.3261246f, 0.8529412f)]
[TrackClipType(typeof(LoopableClip))]
public class LoopableTrack : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		ScriptPlayable<LoopableMixerBehaviour> scriptPlayable = ScriptPlayable<LoopableMixerBehaviour>.Create(graph, inputCount);
		LoopableMixerBehaviour behaviour = scriptPlayable.GetBehaviour();
		behaviour.MarkerClips = new Dictionary<string, double>();
		foreach (TimelineClip clip in GetClips())
		{
			LoopableClip loopableClip = (LoopableClip)clip.asset;
			string displayName = clip.displayName;
			switch (loopableClip.Action)
			{
			case TimelineAction.Pause:
				displayName = "||";
				break;
			case TimelineAction.Marker:
				displayName = "● " + loopableClip.MarkerLabel;
				if (!behaviour.MarkerClips.ContainsKey(loopableClip.MarkerLabel))
				{
					behaviour.MarkerClips.Add(loopableClip.MarkerLabel, clip.start);
				}
				break;
			case TimelineAction.JumpToMarker:
				displayName = "↩\ufe0e  " + loopableClip.MarkerToJumpTo;
				break;
			case TimelineAction.JumpToTime:
				displayName = "↩ " + loopableClip.TimeToJumpTo;
				break;
			}
			clip.displayName = displayName;
		}
		return scriptPlayable;
	}
}
