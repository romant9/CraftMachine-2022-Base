using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EffectEditModeUpdate : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Do we want child particle systems to update in edit mode")]
	public bool updateParticleSystems;

	[SerializeField]
	[Tooltip("Do we want child components to update in edit mode")]
	public bool updateComponents;

	[SerializeField]
	[Tooltip("List of effects components to update in edit mode")]
	private List<Component> EffectComponents;

	private float age;

	private float startTime;

	private DateTime startDate;

	private List<ParticleSystem> ParticleSystems = new List<ParticleSystem>();

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
		if (updateParticleSystems)
		{
			ParticleSystems.AddRange(base.gameObject.GetComponentsInChildren<ParticleSystem>());
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

	private void EditModeUpdate()
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
		if (updateParticleSystems && ParticleSystems != null)
		{
			foreach (ParticleSystem particleSystem in ParticleSystems)
			{
				if (particleSystem != null)
				{
					particleSystem.Simulate(age);
				}
			}
		}
		if (updateComponents)
		{
			BroadcastMessage("Update");
		}
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
	}
}
