using UnityEngine;

public class PlatformSpecificParticles : MonoBehaviour
{
	public PlatformFlag PlatformCondition;

	[Header("DO NOT USE CURVES IN EMITTER")]
	public float EmissionRateMultiplier = 1f;

	public float StartSizeMultiplier = 1f;

	public void Start()
	{
		if (!PlatformInfo.HasFlag(PlatformCondition))
		{
			return;
		}
		ParticleSystem component = GetComponent<ParticleSystem>();
		if (!(component != null))
		{
			return;
		}
		ParticleSystem.EmissionModule emission = component.emission;
		if (emission.burstCount > 0)
		{
			ParticleSystem.Burst[] array = new ParticleSystem.Burst[emission.burstCount];
			if (EmissionRateMultiplier <= 0f)
			{
				array = new ParticleSystem.Burst[0];
			}
			else
			{
				emission.GetBursts(array);
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i].maxCount = (short)((float)array[i].maxCount * EmissionRateMultiplier);
				array[i].minCount = (short)((float)array[i].minCount * EmissionRateMultiplier);
			}
			emission.SetBursts(array);
			if (EmissionRateMultiplier == 1f && StartSizeMultiplier == 1f)
			{
			}
		}
		else
		{
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			rateOverTime.constantMax *= EmissionRateMultiplier;
			emission.rateOverTime = rateOverTime;
			if (EmissionRateMultiplier == 1f)
			{
				_ = StartSizeMultiplier;
				_ = 1f;
			}
		}
		ParticleSystem.MainModule main = component.main;
		main.startSizeMultiplier = StartSizeMultiplier;
	}
}
