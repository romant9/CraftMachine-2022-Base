using System.Linq;
using UnityEngine;

public class EffectGoreSpawn : MonoBehaviour
{
	public Vector3 ImpulseBase = new Vector3(0f, 0f, 0f);

	public Vector3 ImpulseVariation = new Vector3(0f, 0f, 0f);

	public Vector3 TorqueBase = new Vector3(0f, 0f, 0f);

	public Vector3 TorqueVariation = new Vector3(0f, 0f, 0f);

	public bool Rand;

	private Vector3 Impulse;

	private Vector3 Torque;

	private void Start()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.IsGoreDisabled)
		{
			return;
		}
		GoreData goreData = UnityUtils.LoadFromAssetBundle<GoreData>("Combat/GoreData", "scriptableobjects");
		int count = goreData.SpawnGameObjects.Count;
		GameObject obj = Object.Instantiate(Enumerable.ElementAt(index: Mathf.Min(Mathf.FloorToInt(Random.value * (float)count), count - 1), source: goreData.SpawnGameObjects).GetPrefab());
		obj.transform.parent = base.transform;
		obj.transform.localPosition = new Vector3(0f, 0f, 0f);
		obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		obj.SetActive(value: true);
		Rigidbody component = obj.GetComponent<Rigidbody>();
		if (component != null)
		{
			if (Rand)
			{
				Impulse = new Vector3(ImpulseBase.x + ImpulseVariation.x * (Random.value - 0.5f), ImpulseBase.y + ImpulseVariation.y * (Random.value - 0.5f), ImpulseBase.z + ImpulseVariation.z * (Random.value - 0.5f));
				Torque = new Vector3(TorqueBase.x + TorqueVariation.x * (Random.value - 0.5f), TorqueBase.y + TorqueVariation.y * (Random.value - 0.5f), TorqueBase.z + TorqueVariation.z * (Random.value - 0.5f));
			}
			component.AddForce(Impulse, ForceMode.Impulse);
			component.AddTorque(Torque, ForceMode.Impulse);
		}
	}
}
