using Client.Utils;
using TWDModel;
using UnityEngine;

public class ActorHitEffects : MonoBehaviour
{
	public GameObject BloodSplat;

	public GameObject HealEffect;

	public GameObject ChargeAbilityEffect;

	public GameObject GunParent;

	public void SpawnHealEffects(ActorModel healer)
	{
		if (HealEffect != null)
		{
			Helpers.InstantiateToParent(HealEffect, base.gameObject);
		}
	}

	public void SpawnHitEffects(ActorModel damager, float sizeMultiplier = 1f)
	{
		if ((!(GameManager.Instance != null) || GameManager.Instance.playerModel == null || !GameManager.Instance.IsGoreDisabled) && BloodSplat != null)
		{
			Vector3 vector = new Vector3(0f, 0f, 0f);
			if (damager != null)
			{
				GridView.Instance.GetPosition(damager.GridCoordinate).ToVector3();
			}
			GameObject obj = Object.Instantiate(BloodSplat);
			obj.transform.rotation = Quaternion.LookRotation((base.transform.position - vector).normalized, Vector3.up);
			obj.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f);
			ParticleSystem.MainModule main = obj.GetComponent<ParticleSystem>().main;
			main.startSize = main.startSize.constant * sizeMultiplier;
		}
	}

	public void SpawnGenericChargeAbilityEffect(Vector3 targetPos)
	{
		if (GunParent != null && ChargeAbilityEffect != null)
		{
			GameObject obj = Object.Instantiate(ChargeAbilityEffect);
			obj.transform.rotation = Quaternion.LookRotation((base.transform.position - targetPos).normalized, Vector3.up);
			obj.transform.position = targetPos;
		}
	}
}
