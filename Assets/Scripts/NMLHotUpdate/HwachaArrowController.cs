using System.Collections.Generic;
using UnityEngine;

public class HwachaArrowController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField]
	private TrailRenderer trailRenderer;

	[SerializeField]
	private HwachaArrowSO HwachaArrowSo;

	[SerializeField]
	private GameObject MeshParent;

	[SerializeField]
	private List<HitEffectInformation> hitEffectInformations;

	private float flightSpeed;

	private float heightMultiplier;

	private float flightTimer;

	private float targetDistance;

	private float speedToDistance;

	private Vector3 startPoint;

	private Vector3 target;

	private Vector3 lastPosition;

	private bool readyToFly;

	private bool isInitialized;

	private Collider ownerCollider;

	private void Awake()
	{
		EnableRenderers(state: false);
	}

	private void FixedUpdate()
	{
		if (isInitialized)
		{
			EnableRenderers(state: true);
			readyToFly = true;
			isInitialized = false;
		}
		if (readyToFly && target != Vector3.zero)
		{
			flightTimer += Time.deltaTime;
			base.transform.position = CalculateParabola(startPoint, target, targetDistance / 5f * heightMultiplier, flightTimer * speedToDistance);
			Vector3 vector = base.transform.position - lastPosition;
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(new Ray(lastPosition, vector), out hitInfo, vector.magnitude, HwachaArrowSo.CollisionLayerMask, QueryTriggerInteraction.Ignore) && !hitInfo.collider.CompareTag("Projectile") && hitInfo.collider != ownerCollider)
			{
				Arrive(hitInfo);
				return;
			}
			base.transform.rotation = Quaternion.LookRotation(vector);
			lastPosition = base.transform.position;
		}
	}

	public void Shoot(Vector3 target, GameObject owner, float flightSpeed, float heightMultiplier)
	{
		startPoint = base.transform.position;
		lastPosition = base.transform.position;
		targetDistance = Vector3.Distance(base.transform.position, target);
		speedToDistance = flightSpeed / targetDistance * flightSpeed;
		ownerCollider = owner.GetComponent<Collider>();
		this.target = target;
		this.flightSpeed = flightSpeed;
		this.heightMultiplier = heightMultiplier;
		isInitialized = true;
	}

	private void Arrive(RaycastHit hit)
	{
		readyToFly = false;
		trailRenderer.time = HwachaArrowSo.TrailFadeoutTime;
		Invoke("DisableTrailEmission", HwachaArrowSo.DisableTrailEmissionTime);
		Invoke("DisableTrail", HwachaArrowSo.DisableTrailTime);
		base.transform.position = (hit.point += base.transform.forward * Random.Range(HwachaArrowSo.StuckDepthMin, HwachaArrowSo.StuckDepthMax));
		MakeChildOfHitObject(hit.transform);
		if (hitEffectInformations.Count > 0)
		{
			GameObject gameObject = FindHitEffectByLayer(hit.transform.gameObject);
			if (gameObject != null)
			{
				Object.Instantiate(gameObject, base.transform.position, hit.transform.rotation);
			}
		}
	}

	private void MakeChildOfHitObject(Transform parentTransform)
	{
		if (IsSuitedParent(parentTransform))
		{
			Quaternion rotation = base.transform.rotation;
			base.transform.rotation = default(Quaternion);
			base.transform.SetParent(parentTransform, worldPositionStays: true);
			MeshParent.transform.rotation = rotation;
		}
	}

	private bool IsSuitedParent(Transform parent)
	{
		if (IsUniformScaled(parent))
		{
			return IsUniformRotated(parent);
		}
		return false;
	}

	private bool IsUniformScaled(Transform parent)
	{
		if (Mathf.Approximately(parent.localScale.x, parent.localScale.y))
		{
			return Mathf.Approximately(parent.localScale.x, parent.localScale.z);
		}
		return false;
	}

	private bool IsUniformRotated(Transform parent)
	{
		Vector3 eulerAngles = parent.rotation.eulerAngles;
		if (Mathf.Approximately(parent.rotation.x, parent.rotation.y) && Mathf.Approximately(parent.rotation.x, parent.rotation.z))
		{
			return true;
		}
		if (Mathf.Approximately(Mathf.Round(eulerAngles.x) % 90f, 0f) && Mathf.Approximately(Mathf.Round(eulerAngles.y) % 90f, 0f))
		{
			return Mathf.Approximately(Mathf.Round(eulerAngles.z) % 90f, 0f);
		}
		return false;
	}

	private void EnableRenderers(bool state)
	{
		MeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = state;
		}
	}

	public static Vector3 CalculateParabola(Vector3 start, Vector3 end, float height, float t)
	{
		float x = 0f;
		float z = 0f;
		if (t > 0f)
		{
			x = (end.x - start.x) * t + start.x;
			z = (end.z - start.z) * t + start.z;
		}
		return new Vector3(x, f(t) + Mathf.Lerp(start.y, end.y, t), z);
		float f(float xPos)
		{
			return -4f * height * xPos * xPos + 4f * height * xPos;
		}
	}

	private void DisableTrailEmission()
	{
		trailRenderer.emitting = false;
	}

	private void DisableTrail()
	{
		trailRenderer.enabled = false;
	}

	private GameObject FindHitEffectByLayer(GameObject hitObject)
	{
		foreach (HitEffectInformation hitEffectInformation in hitEffectInformations)
		{
			if (IsInLayerMask(hitObject.layer, hitEffectInformation.CollisionLayerMask))
			{
				return hitEffectInformation.EffectPrefab;
			}
		}
		return null;
		static bool IsInLayerMask(int value, LayerMask layerMask)
		{
			return (layerMask.value & (1 << value)) > 0;
		}
	}
}
