using System.Collections.Generic;
using UnityEngine;

public class StaticCombiner : MonoBehaviour
{
	public List<GameObject> GOlist;

	private bool initialized;

	private void Start()
	{
		if (!initialized)
		{
			if (GOlist.Count > 0)
			{
				UnityUtils.CombineInHierarchy(GOlist.ToArray());
			}
			else
			{
				UnityUtils.CombineInHierarchy(base.gameObject);
			}
			initialized = true;
		}
	}
}
