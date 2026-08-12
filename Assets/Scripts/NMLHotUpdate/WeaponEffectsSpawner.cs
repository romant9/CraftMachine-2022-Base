using TWDModel;
using UnityEngine;

internal class WeaponEffectsSpawner : MonoBehaviour
{
	[SerializeField]
	public string normalBulletTrail;

	[SerializeField]
	public string incendiaryBulletTrail;

	[SerializeField]
	public GameObject bulletSparks;

	[SerializeField]
	public bool isMuzzleFlashGore;

	[SerializeField]
	public GameObject muzzleFlash;

	[SerializeField]
	public GameObject onHitEffectPrefab;

	[SerializeField]
	public string onHitSoundEvent;

	[HideInInspector]
	public float BulletFlightTime;

	public Transform muzzleFlashPosition;

	public GameObject HideWhileReloading;

	public void SpawnFireEffects(Vector3 targetPosition, EquipmentItemModel weapon, bool spawnTrail = true)
	{
		if (muzzleFlashPosition == null && (!string.IsNullOrEmpty(incendiaryBulletTrail) || !string.IsNullOrEmpty(normalBulletTrail) || muzzleFlash != null))
		{
			Debug.LogWarning("Weapon '" + base.gameObject.name + "' does not have a proper effect setup!. You need have muzzleFlashPosition when using bullet trails or muzzleFlash.");
		}
		if (muzzleFlashPosition != null && spawnTrail && !string.IsNullOrEmpty(normalBulletTrail) && !string.IsNullOrEmpty(incendiaryBulletTrail))
		{
			GameObject gameObject = null;
			BulletTrailInstant bulletTrailInstant = null;
			if (weapon != null)
			{
				gameObject = (weapon.HasTrait("Equipment.Incendiary") ? Object.Instantiate(UnityUtils.LoadFromAssetBundle<PrefabResource>(incendiaryBulletTrail, "scriptableobjects").GetPrefab(), Vector3.zero, Quaternion.identity) : ((weapon.Definition.Category != EquipmentCategory.RangeWeapon || weapon.Definition.Type != EquipmentType.AssaultRifle) ? Object.Instantiate(UnityUtils.LoadFromAssetBundle<PrefabResource>(normalBulletTrail, "scriptableobjects").GetPrefab(), Vector3.zero, Quaternion.identity) : null));
			}
			bulletTrailInstant = ((gameObject != null) ? gameObject.GetComponent<BulletTrailInstant>() : null);
			if (bulletTrailInstant != null)
			{
				bulletTrailInstant.SetTrailCoordinates(muzzleFlashPosition.position, targetPosition);
			}
			else if (gameObject != null)
			{
				gameObject.transform.parent = muzzleFlashPosition;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.transform.localRotation = default(Quaternion);
			}
			BulletFlightTime = 0f;
		}
		if (muzzleFlashPosition != null && muzzleFlash != null && (!isMuzzleFlashGore || !GameManager.Instance.IsGoreDisabled))
		{
			GameObject obj = Object.Instantiate(muzzleFlash, Vector3.zero, Quaternion.identity);
			obj.transform.parent = muzzleFlashPosition;
			obj.transform.localPosition = new Vector3(0f, 0f, 0f);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.transform.localRotation = default(Quaternion);
		}
		if (bulletSparks != null)
		{
			GameObject obj2 = Object.Instantiate(bulletSparks, Vector3.zero, Quaternion.identity);
			obj2.transform.position = new Vector3(targetPosition.x, 0.1f, targetPosition.z);
			obj2.transform.localScale = new Vector3(1f, 1f, 1f);
			obj2.transform.localRotation = default(Quaternion);
		}
		Projectile projectile = base.gameObject.GetComponentInChildren(typeof(Projectile)) as Projectile;
		if (projectile != null)
		{
			projectile.enabled = true;
			projectile.Throw(targetPosition);
		}
	}

	public void SetReloadingMode(bool reloading)
	{
		Helpers.GameObjectSetActive(HideWhileReloading, !reloading);
	}
}
