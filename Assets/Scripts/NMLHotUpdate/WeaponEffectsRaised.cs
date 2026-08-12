using UnityEngine;

public class WeaponEffectsRaised : MonoBehaviour
{
	[SerializeField]
	private GameObject projectile;

	public void ActivateProjectile(bool activate)
	{
		Helpers.GameObjectSetActive(projectile, activate);
	}
}
