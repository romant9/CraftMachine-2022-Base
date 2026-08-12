using Fabric;
using UnityEngine;

[RequireComponent(typeof(AudioComponent))]
public class CustomAudioSourceSettings : MonoBehaviour
{
	public AudioSourceCurveType Type;

	public AnimationCurve Curve;

	private void Start()
	{
		AudioComponent component = base.gameObject.GetComponent<AudioComponent>();
		if (component != null && component.AudioSource != null)
		{
			component.AudioSource.SetCustomCurve(Type, Curve);
		}
	}
}
