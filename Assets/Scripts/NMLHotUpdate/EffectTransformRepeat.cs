using UnityEngine;

public class EffectTransformRepeat : MonoBehaviour
{
	private Vector3 startPos;

	private Vector3 startRot;

	public Vector3 posSpeed;

	public Vector3 rotSpeed;

	public float speed = 1f;

	public float repeatTime = 1f;

	private void Start()
	{
		startPos = base.transform.localPosition;
		startRot = base.transform.localEulerAngles;
	}

	private void Update()
	{
		float num = Mathf.Repeat(Time.time, repeatTime);
		base.transform.localPosition = startPos + num * speed * posSpeed;
		base.transform.localEulerAngles = startRot + num * speed * rotSpeed;
	}
}
