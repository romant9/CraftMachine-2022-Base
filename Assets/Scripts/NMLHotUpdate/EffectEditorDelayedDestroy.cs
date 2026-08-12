using System;
using UnityEngine;

[ExecuteInEditMode]
public class EffectEditorDelayedDestroy : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Destroy game object automatically after a delay")]
	public bool DestroyAfterDelay;

	[SerializeField]
	[Tooltip("The delay before the destruction.")]
	public float Delay = 2f;

	[SerializeField]
	[Tooltip("Destroy game object if it is disable.")]
	public bool DestroyOnDisable;

	private float age;

	private float startTime;

	private DateTime startDate;

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
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			startTime = Time.time;
		}
		else
		{
			startDate = DateTime.Now;
		}
	}

	private void CheckDeath()
	{
		if (!(this != null) || !DestroyAfterDelay)
		{
			return;
		}
		if (Application.isPlaying)
		{
			age = Time.time - startTime;
			if (age > Delay)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return;
		}
		TimeSpan timeSpan = DateTime.Now - startDate;
		age = 60f * (float)timeSpan.Minutes + (float)timeSpan.Seconds + 0.001f * (float)timeSpan.Milliseconds;
		if (age > Delay)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			CheckDeath();
		}
	}

	private void OnDisable()
	{
		if (DestroyOnDisable && Application.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
