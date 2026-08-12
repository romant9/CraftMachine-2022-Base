using UnityEngine;

public class WalkerSpawnWarning : MonoBehaviour
{
	public float Duration = 1f;

	public float AlphaSpeed = 2f;

	public float TimeOffsetSpeed = 2f;

	public float StartScale = 0.5f;

	public float EndScale = 2f;

	private float alpha;

	private float startTime;

	private float age;

	private float idRand;

	private Vector3 origScale;

	private int id;

	private void Start()
	{
		id = base.gameObject.GetInstanceID();
		Random.InitState(id);
		idRand = Random.value;
		origScale = base.transform.localScale;
		startTime = Time.time;
		alpha = 0f;
		GetComponent<Renderer>().material.color = new Color(alpha, alpha, alpha, alpha);
	}

	private void Update()
	{
		alpha = 3f * Mathf.PerlinNoise(11.77f + (float)id, AlphaSpeed * Time.time);
		age = Mathf.Clamp01((Time.time - startTime) / Duration);
		base.transform.localScale = origScale * Mathf.SmoothStep(StartScale, EndScale, age);
		alpha *= Mathf.SmoothStep(1f, 0f, age) * Mathf.SmoothStep(0f, 1f, age);
		GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, alpha);
		GetComponent<Renderer>().material.SetVector("_Offset", new Vector4(TimeOffsetSpeed * alpha * age + startTime, 0f, TimeOffsetSpeed * alpha * age + idRand, 0f));
		if (age >= 0.99f)
		{
			startTime = Time.time;
		}
	}
}
