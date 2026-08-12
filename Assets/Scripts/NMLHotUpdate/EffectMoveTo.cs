using UnityEngine;

public class EffectMoveTo : MonoBehaviour
{
	private Vector3 startPoint;

	private Vector3 startRot;

	private float startTime;

	public Vector3 posOffset;

	public Vector3 rotOffset;

	public float speed;

	private void Start()
	{
		startTime = Time.time;
		startPoint = base.transform.localPosition;
		startRot = base.transform.localEulerAngles;
	}

	private void Update()
	{
		float t = (Time.time - startTime) * speed;
		base.transform.localPosition = Vector3.Lerp(startPoint, startPoint + posOffset, t);
		base.transform.localEulerAngles = Vector3.Lerp(startRot, startRot + rotOffset, t);
	}
}
