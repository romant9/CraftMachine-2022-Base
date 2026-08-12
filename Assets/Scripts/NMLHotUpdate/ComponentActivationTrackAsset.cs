using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(ComponentTrackControlAsset))]
[TrackBindingType(typeof(MonoBehaviour))]
public class ComponentActivationTrackAsset : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		return ScriptPlayable<ComponentActivationMixerPlayable>.Create(graph, inputCount);
	}

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		base.GatherProperties(director, driver);
	}

	protected override void OnCreateClip(TimelineClip clip)
	{
		clip.displayName = "Component Active";
		base.OnCreateClip(clip);
	}
}
