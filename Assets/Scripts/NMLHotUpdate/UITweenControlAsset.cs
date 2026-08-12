using UnityEngine;
using UnityEngine.Playables;

public class UITweenControlAsset : PlayableAsset
{
	public bool TweenScale;

	public bool TweenRotation;

	public Vector2 FromScale;

	public Vector2 ToScale;

	public AnimationCurve ScaleAnimationCurve;

	public float FromRotation;

	public float ToRotation;

	public AnimationCurve RotationAnimationCurve;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<UITweenPlayable> scriptPlayable = ScriptPlayable<UITweenPlayable>.Create(graph);
		UITweenPlayable behaviour = scriptPlayable.GetBehaviour();
		behaviour.TweenScale = TweenScale;
		behaviour.TweenRotation = TweenRotation;
		behaviour.FromScale = FromScale;
		behaviour.ToScale = ToScale;
		behaviour.ScaleAnimationCurve = ScaleAnimationCurve;
		behaviour.FromRotation = FromRotation;
		behaviour.ToRotation = ToRotation;
		behaviour.RotationAnimationCurve = RotationAnimationCurve;
		return scriptPlayable;
	}
}
