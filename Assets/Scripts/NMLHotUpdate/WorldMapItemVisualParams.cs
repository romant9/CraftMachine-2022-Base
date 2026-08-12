using System;
using UnityEngine;

[Serializable]
public class WorldMapItemVisualParams
{
	[Tooltip("Identifier for this item")]
	public string ItemId;

	[Tooltip("Prefab for map detail view item")]
	public GameObject DetailBackgroundPrefab;
}
