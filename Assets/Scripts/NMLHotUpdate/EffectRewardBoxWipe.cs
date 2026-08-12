using System.Collections.Generic;
using UnityEngine;

public class EffectRewardBoxWipe : MonoBehaviour
{
	public List<GameObject> WipeObjects = new List<GameObject>();

	public float WipeSpeed = 0.4f;

	private List<Material> wipeMaterials = new List<Material>();

	private float startTime;

	public void Start()
	{
		startTime = Time.time;
		for (int i = 0; i < WipeObjects.Count; i++)
		{
			Material[] materials = WipeObjects[i].GetComponent<MeshRenderer>().materials;
			for (int j = 0; j < materials.Length; j++)
			{
				if (materials[j] != null)
				{
					wipeMaterials.Add(materials[j]);
				}
				else
				{
					Debug.LogWarning("RewardBoxWipe did not find material " + WipeObjects[i].name);
				}
			}
		}
	}

	public void Update()
	{
		for (int i = 0; i < wipeMaterials.Count; i++)
		{
			wipeMaterials[i].SetFloat("_Cutoff", WipeSpeed * (Time.time - startTime));
		}
	}
}
