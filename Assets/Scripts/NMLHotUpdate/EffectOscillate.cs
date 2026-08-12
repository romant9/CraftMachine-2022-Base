using UnityEngine;

public class EffectOscillate : MonoBehaviour
{
	public Vector3 oscillateMoveScale;

	public Vector3 oscillateSizeScale;

	public float oscillateSpeed = 1f;

	private Vector3 origPos;

	private Vector3 origSize;

	private void Start()
	{
		origPos = base.transform.localPosition;
		origSize = base.transform.localScale;
	}

	private void Update()
	{
		base.transform.localPosition = origPos + Mathf.Sin(oscillateSpeed * Time.time) * oscillateMoveScale;
		base.transform.localScale = origSize + Mathf.Sin(oscillateSpeed * Time.time) * oscillateSizeScale;
	}
}
