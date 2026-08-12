using UnityEngine;
using UnityEngine.Playables;

public class UITweenPlayable : PlayableBehaviour
{
	public bool TweenScale;

	public bool TweenRotation;

	public Vector2 FromScale;

	public Vector2 ToScale;

	public AnimationCurve ScaleAnimationCurve;

	public float FromRotation;

	public float ToRotation;

	public AnimationCurve RotationAnimationCurve;

	public float InverseDuration;

	public override void OnGraphStart(Playable playable)
	{
		double duration = playable.GetDuration();
		if (Mathf.Approximately((float)duration, 0f))
		{
			throw new UnityException("A TransformTween cannot have a duration of zero.");
		}
		InverseDuration = 1f / (float)duration;
	}
}
