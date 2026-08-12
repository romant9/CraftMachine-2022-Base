using UnityEngine;
using UnityEngine.Playables;

public class UITweenMixer : PlayableBehaviour
{
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		Transform transform = playerData as Transform;
		if (transform == null)
		{
			return;
		}
		Vector3 localScale = transform.localScale;
		Quaternion rotation = transform.rotation;
		int inputCount = playable.GetInputCount();
		float num = 0f;
		float num2 = 0f;
		Vector3 zero = Vector3.zero;
		Quaternion quaternion = new Quaternion(0f, 0f, 0f, 0f);
		for (int i = 0; i < inputCount; i++)
		{
			ScriptPlayable<UITweenPlayable> playable2 = (ScriptPlayable<UITweenPlayable>)playable.GetInput(i);
			UITweenPlayable behaviour = playable2.GetBehaviour();
			float inputWeight = playable.GetInputWeight(i);
			float time = (float)(playable2.GetTime() * (double)behaviour.InverseDuration);
			if (behaviour.TweenScale)
			{
				float t = behaviour.ScaleAnimationCurve.Evaluate(time);
				num += inputWeight;
				zero += Vector3.Lerp(new Vector3(behaviour.FromScale.x, behaviour.FromScale.y, localScale.z), new Vector3(behaviour.ToScale.x, behaviour.ToScale.y, localScale.z), t) * inputWeight;
			}
			if (behaviour.TweenRotation)
			{
				float t2 = behaviour.RotationAnimationCurve.Evaluate(time);
				num2 += inputWeight;
				Quaternion rotation2 = Quaternion.Lerp(Quaternion.Euler(0f, 0f, behaviour.FromRotation), Quaternion.Euler(0f, 0f, behaviour.ToRotation), t2);
				rotation2 = QuaternionHelpers.NormalizeQuaternion(rotation2);
				if (Quaternion.Dot(quaternion, rotation2) < 0f)
				{
					rotation2 = QuaternionHelpers.ScaleQuaternion(rotation2, -1f);
				}
				rotation2 = QuaternionHelpers.ScaleQuaternion(rotation2, inputWeight);
				quaternion = QuaternionHelpers.AddQuaternions(quaternion, rotation2);
			}
		}
		zero += localScale * (1f - num);
		Quaternion second = QuaternionHelpers.ScaleQuaternion(rotation, 1f - num2);
		quaternion = QuaternionHelpers.AddQuaternions(quaternion, second);
		transform.localScale = zero;
		transform.localRotation = quaternion;
	}
}
