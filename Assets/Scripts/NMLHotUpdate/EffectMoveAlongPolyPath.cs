using System;
using UnityEngine;

[ExecuteInEditMode]
public class EffectMoveAlongPolyPath : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Destroy game object automatically after a delay")]
	public bool DestroyAfterDelay;

	[SerializeField]
	[Tooltip("The delay before the destruction.")]
	public float Delay = 2f;

	[Tooltip("Destroy game object if it is disable.")]
	public bool DestroyOnDisable;

	[Tooltip("Movement speed in units/s")]
	public float Speed = 0.01f;

	[Tooltip("Movement speed in units/s")]
	public float RotSpeed = 10f;

	[Tooltip("Start position offset")]
	public float StartOffset;

	[Tooltip("Movement speed in units/s")]
	public Vector3 Offset = new Vector3(0f, 0f, 0.03f);

	public bool Loop;

	[Tooltip("Polyline Path to move along")]
	public PolylinePath Path;

	private float age;

	private float startTime;

	private float previousAge;

	private float deltaAge;

	private DateTime startDate;

	private PolylinePathIterator it;

	private Transform trans;

	private bool initialized;

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

	private void InitIterator()
	{
		if (!initialized)
		{
			it = new PolylinePathIterator(Path);
			trans = base.gameObject.transform;
			it.Advance(StartOffset);
			initialized = true;
		}
	}

	private void CheckDeath()
	{
		if (Application.isPlaying)
		{
			age = Time.time - startTime;
			if (age > Delay && DestroyAfterDelay)
			{
				Die();
			}
			return;
		}
		TimeSpan timeSpan = DateTime.Now - startDate;
		age = 60f * (float)timeSpan.Minutes + (float)timeSpan.Seconds + 0.001f * (float)timeSpan.Milliseconds;
		if (age > Delay && DestroyAfterDelay)
		{
			Die();
		}
	}

	private void Die()
	{
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Update()
	{
		InitIterator();
		previousAge = age;
		CheckDeath();
		deltaAge = age - previousAge;
		Vector3 position = it.Position;
		it.Advance(Speed * deltaAge);
		Vector3 position2 = it.Position;
		float magnitude = (position2 - position).magnitude;
		trans.position = position2 + Offset;
		trans.Rotate(Vector3.forward, RotSpeed * magnitude * 100f);
		if (it.AtEnd)
		{
			if (Loop)
			{
				it.Clear(Path);
			}
			else
			{
				Die();
			}
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
