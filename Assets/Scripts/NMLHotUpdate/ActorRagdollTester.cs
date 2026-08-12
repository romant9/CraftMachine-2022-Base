using System;
using TWDModel;
using UnityEngine;

public class ActorRagdollTester : MonoBehaviour
{
	public EquipmentType equipmentType;

	public float ImpactAngle;

	private CharacterAnimationController controller;

	public Vector3 ImpactDirection => new Vector3(0f - Mathf.Sin(ImpactAngle * (MathF.PI / 180f)), 0f, Mathf.Cos(ImpactAngle * (MathF.PI / 180f)));

	public ImpactProfile GetImpactProfile(bool isCritical)
	{
		ImpactProfileManager impactProfileManager = UnityEngine.Object.FindObjectOfType<ImpactProfileManager>();
		if (!(impactProfileManager != null))
		{
			return null;
		}
		return impactProfileManager.GetImpactProfile(equipmentType, "", isCritical);
	}

	private void Start()
	{
		controller = GetComponent<CharacterAnimationController>();
		controller.DisableRagdoll();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
		{
			ImpactProfile impactProfile = GetImpactProfile(Input.GetMouseButtonDown(2));
			if (impactProfile != null)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out var hitInfo))
				{
					ActorView componentInParent = hitInfo.collider.gameObject.GetComponentInParent<ActorView>();
					if (controller != null && componentInParent == GetComponent<ActorView>())
					{
						controller.EnableRagdoll();
						controller.ApplyImpactProfile(impactProfile, -ImpactDirection.normalized, Vector3.forward);
					}
				}
				Debug.DrawRay(ray.origin, ray.direction * 10f, Color.yellow);
			}
		}
		if (Input.GetMouseButtonDown(1) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo2))
		{
			ActorView componentInParent2 = hitInfo2.collider.gameObject.GetComponentInParent<ActorView>();
			if (componentInParent2 != null && componentInParent2 == GetComponent<ActorView>())
			{
				componentInParent2.BackFromRagdoll();
			}
		}
	}
}
