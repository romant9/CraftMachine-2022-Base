using UnityEngine;

public class EffectTransformBlend : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Transform blend duration in seconds")]
	private float BlendDuration = 1f;

	[SerializeField]
	[Tooltip("Time remap gamma")]
	private float AgeGamma = 1f;

	[SerializeField]
	[Tooltip("Smooth time")]
	private bool SmoothTime = true;

	[SerializeField]
	[Tooltip("Translation start value")]
	private Vector3 BlendStartPosition;

	[SerializeField]
	[Tooltip("Rotation start value")]
	private Vector3 BlendStartRotation;

	[SerializeField]
	[Tooltip("Scale start value")]
	private Vector3 BlendStartScale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	[Tooltip("Translation end value")]
	private Vector3 BlendEndPosition;

	[SerializeField]
	[Tooltip("Rotation end value")]
	private Vector3 BlendEndRotation;

	[SerializeField]
	[Tooltip("Scale end value")]
	private Vector3 BlendEndScale = new Vector3(1f, 1f, 1f);

	private float startTime;

	private float age;

	private void Start()
	{
		startTime = Time.time;
	}

	private void onEnable()
	{
		startTime = Time.time;
		age = 0f;
	}

	private void Awake()
	{
		startTime = Time.time;
		age = 0f;
	}

	private void Update()
	{
		age = Mathf.Clamp01((Time.time - startTime) / BlendDuration);
		age = Mathf.Pow(age, AgeGamma);
		if (SmoothTime)
		{
			base.transform.localPosition = SmoothStepV(BlendStartPosition, BlendEndPosition, age);
			base.transform.localEulerAngles = SmoothStepV(BlendStartRotation, BlendEndRotation, age);
			base.transform.localScale = SmoothStepV(BlendStartScale, BlendEndScale, age);
		}
		else
		{
			base.transform.localPosition = Vector3.Lerp(BlendStartPosition, BlendEndPosition, age);
			base.transform.localEulerAngles = Vector3.Lerp(BlendStartRotation, BlendEndRotation, age);
			base.transform.localScale = Vector3.Lerp(BlendStartScale, BlendEndScale, age);
		}
	}

	private Vector3 SmoothStepV(Vector3 from, Vector3 to, float t)
	{
		return new Vector3(Mathf.SmoothStep(from.x, to.x, t), Mathf.SmoothStep(from.y, to.y, t), Mathf.SmoothStep(from.z, to.z, t));
	}
}
