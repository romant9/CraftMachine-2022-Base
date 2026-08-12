using UnityEngine;

public class EffectTransformContinuous : MonoBehaviour
{
	public Vector3 posSpeed;

	public Vector3 rotSpeed;

	public float speed = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Translate(posSpeed * speed * Time.deltaTime);
		base.transform.Rotate(rotSpeed * speed * Time.deltaTime);
	}
}
