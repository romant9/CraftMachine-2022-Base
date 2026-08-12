using UnityEngine;
using UnityEngine.Playables;

public class TransformTweenMixerBehaviour : PlayableBehaviour
{
	private bool initialized;

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		Transform transform = playerData as Transform;
		if (transform == null)
		{
			return;
		}
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		int inputCount = playable.GetInputCount();
		float num = 0f;
		float num2 = 0f;
		Vector3 zero = Vector3.zero;
		Quaternion quaternion = new Quaternion(0f, 0f, 0f, 0f);
		for (int i = 0; i < inputCount; i++)
		{
			ScriptPlayable<TransformTweenBehaviour> playable2 = (ScriptPlayable<TransformTweenBehaviour>)playable.GetInput(i);
			TransformTweenBehaviour behaviour = playable2.GetBehaviour();
			if (behaviour.EndLocation == null)
			{
				continue;
			}
			float inputWeight = playable.GetInputWeight(i);
			if (!initialized && !behaviour.StartLocation)
			{
				behaviour.StartingPosition = position;
				behaviour.StartingRotation = rotation;
				initialized = true;
			}
			float time = (float)(playable2.GetTime() * (double)behaviour.InverseDuration);
			float t = behaviour.CurrentCurve.Evaluate(time);
			if (behaviour.TweenPosition)
			{
				num += inputWeight;
				zero += Vector3.Lerp(behaviour.StartingPosition, behaviour.EndLocation.position, t) * inputWeight;
			}
			if (behaviour.TweenRotation)
			{
				num2 += inputWeight;
				Quaternion rotation2 = Quaternion.Lerp(behaviour.StartingRotation, behaviour.EndLocation.rotation, t);
				rotation2 = QuaternionHelpers.NormalizeQuaternion(rotation2);
				if (Quaternion.Dot(quaternion, rotation2) < 0f)
				{
					rotation2 = QuaternionHelpers.ScaleQuaternion(rotation2, -1f);
				}
				rotation2 = QuaternionHelpers.ScaleQuaternion(rotation2, inputWeight);
				quaternion = QuaternionHelpers.AddQuaternions(quaternion, rotation2);
			}
		}
		zero += position * (1f - num);
		Quaternion second = QuaternionHelpers.ScaleQuaternion(rotation, 1f - num2);
		quaternion = QuaternionHelpers.AddQuaternions(quaternion, second);
		transform.position = zero;
		transform.rotation = quaternion;
	}
}
