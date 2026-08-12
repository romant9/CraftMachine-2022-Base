using UnityEngine;

public class EffectTempSpawnRBD : MonoBehaviour
{
	public GameObject SpawnGameObject;

	public float SpawnInterval = 1f;

	private float spawnCounter;

	private void Update()
	{
		if (spawnCounter > SpawnInterval)
		{
			GameObject obj = Object.Instantiate(SpawnGameObject);
			obj.transform.parent = base.transform;
			obj.transform.localPosition = new Vector3(0f, 0f, 0f);
			obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			obj.SetActive(value: true);
			spawnCounter = 0f;
		}
		spawnCounter += Time.deltaTime;
	}
}
