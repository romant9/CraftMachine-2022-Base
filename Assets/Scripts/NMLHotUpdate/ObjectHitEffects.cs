using System;
using System.Collections;
using UnityEngine;

internal class ObjectHitEffects : MonoBehaviour
{
	[SerializeField]
	private Vector3 ShakeAxis;

	[SerializeField]
	private float ShakeDegrees;

	[SerializeField]
	private float ShakeDuration;

	[SerializeField]
	private float ShakeFrequency;

	private float CurrentShakeMagnitude;

	public void ShakeObject(float magnitude)
	{
		StopCoroutine("StartShaking");
		CurrentShakeMagnitude = magnitude;
		StartCoroutine("StartShaking");
	}

	private IEnumerator StartShaking()
	{
		float currentTime = 0f;
		Quaternion originalRotation = base.transform.localRotation;
		Quaternion shakenRotation = base.transform.localRotation * Quaternion.AngleAxis(ShakeDegrees * CurrentShakeMagnitude, ShakeAxis);
		while (currentTime < ShakeDuration)
		{
			float num = (Mathf.Cos((currentTime / ShakeDuration - 0.5f) * MathF.PI * 2f) + 1f) / 2f;
			currentTime += Time.deltaTime;
			float t = (Mathf.Cos(currentTime * ShakeFrequency) + 1f) / 2f * num;
			base.transform.localRotation = Quaternion.Lerp(originalRotation, shakenRotation, t);
			yield return null;
		}
		base.transform.localRotation = originalRotation;
	}
}
