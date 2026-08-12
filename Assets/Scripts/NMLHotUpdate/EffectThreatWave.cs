using UnityEngine;

public class EffectThreatWave : MonoBehaviour
{
	public float Duration = 1f;

	public float StartScale = 0.5f;

	public float EndScale = 2f;

	public float Brightness = 0.5f;

	private float alpha;

	private float startTime;

	private Vector3 origScale;

	private bool Inited;

	public float CurrentScale { get; private set; }

	public float Age { get; private set; }

	public void Begin()
	{
		origScale = base.transform.localScale;
		startTime = Time.time;
		Inited = true;
	}

	private void Start()
	{
		alpha = 0f;
		GetComponent<Renderer>().material.color = new Color(alpha, alpha, alpha, alpha);
	}

	private void Update()
	{
		if (Inited)
		{
			alpha = 2f;
			Age = Mathf.Clamp01((Time.time - startTime) / Duration);
			CurrentScale = Mathf.SmoothStep(StartScale, EndScale, Age);
			base.transform.localScale = origScale * CurrentScale;
			alpha *= Mathf.SmoothStep(1f, 0f, Age) * Mathf.SmoothStep(0f, 1f, Age);
			if (GetComponent<Renderer>().material.HasProperty("_TintColor"))
			{
				GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha * Brightness));
			}
			else
			{
				GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, alpha * Brightness);
			}
		}
	}
}
