using UnityEngine;

public class SetupGlobalVisuals : MonoBehaviour
{
	private Vector4 wind = new Vector4(0.85f, 0.075f, 0.4f, 0.5f);

	private float windFrequency = 0.75f;

	private float grassWindFrequency = 1.5f;

	private void Start()
	{
		Shader.SetGlobalColor("_Wind", wind);
		Shader.SetGlobalColor("_GrassWind", wind);
	}

	private void Update()
	{
		Color value = wind * Mathf.Sin(Time.realtimeSinceStartup * windFrequency);
		value.a = wind.w;
		Color value2 = wind * Mathf.Sin(Time.realtimeSinceStartup * grassWindFrequency);
		value2.a = wind.w;
		Shader.SetGlobalColor("_Wind", value);
		Shader.SetGlobalColor("_GrassWind", value2);
	}
}
