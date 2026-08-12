using System.Linq;
using UnityEngine;

public class EffectGoreSpawnAuto : MonoBehaviour
{
	public GoreData GD;

	public Vector3 ImpulseBase = new Vector3(0f, 0f, 0f);

	public Vector3 ImpulseVariation = new Vector3(0f, 0f, 0f);

	public Vector3 TorqueBase = new Vector3(0f, 0f, 0f);

	public Vector3 TorqueVariation = new Vector3(0f, 0f, 0f);

	public bool Rand;

	public float SpawnInterval = 1f;

	private float spawnCounter;

	private Vector3 Impulse;

	private Vector3 Torque;

	private void Update()
	{
		if (spawnCounter > SpawnInterval)
		{
			int count = GD.SpawnGameObjects.Count;
			int index = Mathf.FloorToInt(Random.value * (float)count);
			GameObject obj = Object.Instantiate(GD.SpawnGameObjects.ElementAt(index).GetPrefab());
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
			spawnCounter = 0f;
		}
		spawnCounter += Time.deltaTime;
	}
}
