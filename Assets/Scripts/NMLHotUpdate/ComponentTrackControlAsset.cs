using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ComponentTrackControlAsset : PlayableAsset
{
	public ActivationTrack.PostPlaybackState PostPlaybackState;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<ComponentActivationMixerPlayable> scriptPlayable = ScriptPlayable<ComponentActivationMixerPlayable>.Create(graph);
		scriptPlayable.GetBehaviour().PostPlaybackState = PostPlaybackState;
		return scriptPlayable;
	}
}
