using System;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingObject : MonoBehaviour
{
	[SerializeField]
	private float destroyAfterSeconds = 4f;

	[SerializeField]
	public float defaultExplosionForce = 15f;

	[SerializeField]
	private GameObject partsExplodedRoot;

	[SerializeField]
	private GameObject splashEffect;

	private Dictionary<Transform, Vector3> orginalPositions = new Dictionary<Transform, Vector3>();

	private Dictionary<Transform, Quaternion> orginalRotations = new Dictionary<Transform, Quaternion>();

	private bool hasExploded;

	public bool HasExploded => hasExploded;

	private void Awake()
	{
		foreach (Transform item in partsExplodedRoot.transform)
		{
			orginalPositions[item] = item.localPosition;
			orginalRotations[item] = item.localRotation;
		}
		Explode();
	}

	public void Explode(Vector3? forceVector = null)
	{
		if (hasExploded)
		{
			return;
		}
		float num = (forceVector.HasValue ? forceVector.Value.magnitude : defaultExplosionForce);
		Vector3 vector = forceVector ?? Vector3.zero;
		partsExplodedRoot?.SetActive(value: true);
		foreach (Transform key in orginalPositions.Keys)
		{
			if (key.TryGetComponent<MeshCollider>(out var component))
			{
				component.convex = true;
			}
			Vector3 vector2 = key.position - base.transform.position;
			float magnitude = vector2.magnitude;
			Vector3 vector3 = ((magnitude > 0f) ? (vector2 / magnitude) : key.forward);
			Vector3 vector4 = 0.5f * vector3 * num;
			if (key.TryGetComponent<Rigidbody>(out var component2))
			{
				component2.isKinematic = false;
				component2.velocity = Vector3.zero;
				component2.angularVelocity = Vector3.zero;
				component2.AddForce(vector + vector4);
				if (partsExplodedRoot != null)
				{
					component2.AddTorque(Vector3.Cross(partsExplodedRoot.transform.up, vector3));
				}
			}
		}
		splashEffect.transform.rotation = Quaternion.identity;
		ParticleSystem[] componentsInChildren = splashEffect.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.Clear();
			if (forceVector.HasValue)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
				velocityOverLifetime.enabled = true;
				Vector3 vector5 = forceVector.Value / num;
				velocityOverLifetime.x = vector5.x;
				velocityOverLifetime.y = vector5.y;
				velocityOverLifetime.z = vector5.z;
			}
			particleSystem.Play();
		}
		if (destroyAfterSeconds > 0f)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(destroyAfterSeconds);
			GameManager.Instance.TimingManager.Timer(timeSpan, Deactivate);
		}
		hasExploded = true;
	}

	public void Reset()
	{
		foreach (Transform key in orginalPositions.Keys)
		{
			key.localPosition = orginalPositions[key];
			key.localRotation = orginalRotations[key];
		}
		partsExplodedRoot.SetActive(value: false);
		if (base.gameObject.TryGetComponent<Renderer>(out var component) && base.gameObject.TryGetComponent<Collider>(out var component2))
		{
			component.enabled = true;
			component2.enabled = true;
		}
		hasExploded = false;
	}

	private void Deactivate()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
