using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TransformTweenBehaviour : PlayableBehaviour
{
	public Transform StartLocation;

	public Transform EndLocation;

	public bool TweenPosition = true;

	public bool TweenRotation = true;

	public float InverseDuration;

	public Vector3 StartingPosition;

	public Quaternion StartingRotation;

	public AnimationCurve CurrentCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public override void OnGraphStart(Playable playable)
	{
		double duration = playable.GetDuration();
		if (Mathf.Approximately((float)duration, 0f))
		{
			throw new UnityException("A TransformTween cannot have a duration of zero.");
		}
		InverseDuration = 1f / (float)duration;
		if ((bool)StartLocation)
		{
			StartingPosition = StartLocation.position;
			StartingRotation = StartLocation.rotation;
		}
	}
}
