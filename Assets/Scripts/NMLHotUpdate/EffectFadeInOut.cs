using System;
using Client.Constants;
using UnityEngine;

public class EffectFadeInOut : MonoBehaviour
{
	[Tooltip("Complete duration")]
	public float Duration = 4f;

	[Tooltip("Fade duration")]
	public float Fade = 0.5f;

	private float age;

	private float startTime;

	private DateTime startDate;

	private Material mat;

	private bool initialized;

	private void Start()
	{
		InitFade();
	}

	private void OnEnable()
	{
		InitFade();
	}

	private void InitFade()
	{
		if (!initialized)
		{
			mat = base.gameObject.GetComponentInChildren<MeshRenderer>().material;
			if (Application.isPlaying)
			{
				startTime = Time.time;
			}
			else
			{
				startDate = DateTime.Now;
			}
			initialized = true;
		}
	}

	private void UpdateFade()
	{
		InitFade();
		if (Application.isPlaying)
		{
			age = Time.time - startTime;
		}
		else
		{
			TimeSpan timeSpan = DateTime.Now - startDate;
			age = 60f * (float)timeSpan.Minutes + (float)timeSpan.Seconds + 0.001f * (float)timeSpan.Milliseconds;
		}
		int nameID;
		if (mat.HasProperty(MaterialParameters.TintColor))
		{
			nameID = MaterialParameters.TintColor;
		}
		else
		{
			if (!mat.HasProperty(MaterialParameters.Color))
			{
				return;
			}
			nameID = MaterialParameters.Color;
		}
		Color color = mat.GetColor(nameID);
		if (age <= Fade)
		{
			float t = age / Fade;
			color.a = Mathf.SmoothStep(0f, 1f, t);
		}
		if (age > Fade && age < Duration - Fade)
		{
			color.a = 1f;
		}
		if (age >= Duration - Fade)
		{
			float t2 = (age - Duration + Fade) / Fade;
			color.a = Mathf.SmoothStep(1f, 0f, t2);
		}
		mat.SetColor(nameID, color);
	}

	private void Update()
	{
		UpdateFade();
	}
}
