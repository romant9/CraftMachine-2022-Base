using System.Collections.Generic;
using UnityEngine;

public class HwachaArcherController : MonoBehaviour
{
	[SerializeField]
	private GameObject HwachaArrowPrefab;

	[SerializeField]
	[Range(0f, 10f)]
	public float SpreadFactor = 0.5f;

	[SerializeField]
	[Range(0f, 0.4f)]
	private float SpreadFactorDistanceImpact = 0.1f;

	[SerializeField]
	private float HeightMultiplier = 2f;

	[SerializeField]
	private float ArrowFlightSpeed = 6f;

	private ICollection<GameObject> instantiatedArrows;

	public void Initialize(int maxArrowCount)
	{
		SingularityMonoBehaviour<ObjectPoolManager>.Instance.SetupCacheForObject(HwachaArrowPrefab, maxArrowCount);
		instantiatedArrows = new List<GameObject>(maxArrowCount);
	}

	public void ShootArrow(Vector3 target, float speedDelta = 0f)
	{
		float num = Vector3.Distance(base.transform.position, target);
		float num2 = SpreadFactor * (1f + SpreadFactorDistanceImpact * num);
		Vector3 target2 = Random.insideUnitSphere * num2 + target;
		GameObject gameObject = SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(HwachaArrowPrefab);
		gameObject.transform.position = base.transform.position;
		gameObject.transform.rotation = base.transform.rotation;
		gameObject.name = "HwachaArrow";
		gameObject.GetComponent<HwachaArrowController>().Shoot(target2, base.gameObject, ArrowFlightSpeed + speedDelta, HeightMultiplier);
		instantiatedArrows.Add(gameObject);
	}

	public void ClearArrows()
	{
		foreach (GameObject instantiatedArrow in instantiatedArrows)
		{
			SingularityMonoBehaviour<ObjectPoolManager>.Instance.ReturnObjectToPool(instantiatedArrow);
		}
		instantiatedArrows.Clear();
	}
}
