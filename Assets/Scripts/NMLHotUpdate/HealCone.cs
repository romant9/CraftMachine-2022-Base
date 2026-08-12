using UnityEngine;

public class HealCone : MonoBehaviour
{
	public float rotSpeed = 100f;

	public float duration = 3f;

	public float alphaSpeed = 2f;

	private float alpha;

	private float startTime;

	private float age;

	private void Start()
	{
		startTime = Time.time;
		alpha = 0f;
		GetComponent<Renderer>().material.color = new Color(alpha, alpha, alpha, alpha);
	}

	private void Update()
	{
		base.transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
		int instanceID = base.gameObject.GetInstanceID();
		alpha = 4f * Mathf.PerlinNoise(11.77f + (float)instanceID, alphaSpeed * Time.time);
		age = Mathf.Clamp01((Time.time - startTime) / duration);
		alpha *= Mathf.SmoothStep(1f, 0f, age) * Mathf.SmoothStep(0f, 1f, age);
		GetComponent<Renderer>().material.color = new Color(alpha, alpha, alpha, alpha);
		if (age >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
