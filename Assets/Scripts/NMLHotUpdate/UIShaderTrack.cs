using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(ShaderControlAsset))]
[TrackBindingType(typeof(UIWidget))]
public class UIShaderTrack : TrackAsset
{
	public string ShaderVarName;

	public ShaderControlType VariableType;

	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		ScriptPlayable<UIShaderMixer> scriptPlayable = ScriptPlayable<UIShaderMixer>.Create(graph, inputCount);
		scriptPlayable.GetBehaviour().ShaderVarName = ShaderVarName;
		scriptPlayable.GetBehaviour().VariableType = VariableType;
		return scriptPlayable;
	}
}
