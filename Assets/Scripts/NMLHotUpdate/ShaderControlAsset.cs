using UnityEngine;
using UnityEngine.Playables;

public class ShaderControlAsset : PlayableAsset
{
	public float FloatVal;

	public Vector4 VectorVal = Vector4.zero;

	public Color ColorVal = Color.black;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<ShaderPlayable> scriptPlayable = ScriptPlayable<ShaderPlayable>.Create(graph);
		ShaderPlayable behaviour = scriptPlayable.GetBehaviour();
		behaviour.FloatVal = FloatVal;
		behaviour.VectorVal = VectorVal;
		behaviour.ColorVal = ColorVal;
		return scriptPlayable;
	}
}
