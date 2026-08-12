using UnityEngine;

internal class ProbeLightSampler : MonoBehaviour
{
	private Vector3 sampledPosition;

	private void Start()
	{
		UpdateLight();
		sampledPosition = base.transform.position;
		if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null && !GetComponent<Renderer>().material.HasProperty("_SampledLight"))
		{
			Debug.LogWarning("ProbeLightSampler on " + base.gameObject.name + " used without a ProbeLit material.");
		}
	}

	private void Update()
	{
		if (base.transform.position != sampledPosition)
		{
			UpdateLight();
			sampledPosition = base.transform.position;
		}
	}

	protected void UpdateLight()
	{
		if (!(GetComponent<Renderer>() == null))
		{
			Material material = GetComponent<Renderer>().material;
			if (!(material == null))
			{
				Vector3 vector = LightProbeUtil.SampleLightProbesUp(base.transform.position, GetComponent<Renderer>());
				material.SetVector("_SampledLight", vector);
			}
		}
	}
}
