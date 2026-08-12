using System;
using UnityEngine;

public class EffectRotationNoise : MonoBehaviour
{
	private Vector3 rotOffset;

	private Vector3 startRot;

	private float startTime;

	private float age;

	private DateTime startDate;

	public float Amount = 10f;

	public float Speed = 1f;

	public float Offset;

	private void Start()
	{
		if (Application.isPlaying)
		{
			startTime = Time.time;
		}
		else
		{
			startDate = DateTime.Now;
		}
		startRot = base.transform.localEulerAngles;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			age = Time.time - startTime;
		}
		else
		{
			TimeSpan timeSpan = DateTime.Now - startDate;
			age = 60f * (float)timeSpan.Minutes + (float)timeSpan.Seconds + 0.001f * (float)timeSpan.Milliseconds;
		}
		rotOffset.x = Amount * Mathf.Sin(age * Speed + Offset);
		rotOffset.y = Amount * Mathf.Cos(age * Speed + Offset);
		rotOffset.x = Amount * Mathf.Sin(age * 1.76f * Speed + Offset);
		base.transform.localEulerAngles = startRot + rotOffset;
	}
}
