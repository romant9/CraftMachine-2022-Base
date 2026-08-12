using System.Collections.Generic;
using UnityEngine;

public class MapVisualData : ScriptableObject
{
	public List<WorldMapItemVisualParams> WorldMapItems;

	[Tooltip("Prefab for mission icon.")]
	public GameObject MissionIconPrefabStory;

	[Tooltip("Prefab for special mission icon.")]
	public GameObject MissionIconPrefabSpecial;

	[Tooltip("Prefab for survival mission icon.")]
	public GameObject MissionIconPrefabSurvival;

	[Tooltip("Prefab for survival mission large icon.")]
	public GameObject MissionIconPrefabSurvivalLarge;

	[Tooltip("Prefab for mission icon.")]
	public GameObject MissionIconPrefabGrind;

	[Tooltip("Prefab for final mission icon.")]
	public GameObject MissionIconPrefabFinalEpisode;

	[Tooltip("Prefab for challenge mission icon.")]
	public GameObject MissionIconPrefabChallenge;

	[Tooltip("Prefab for challenge Master mission icon.")]
	public GameObject MissionIconPrefabChallengeMaster;

	[Tooltip("Prefab for Season Trial mission icon.")]
	public GameObject MissionIconPrefabTrial;

	public GameObject GetDetailMapItemPrefab(string itemId)
	{
		for (int i = 0; i < WorldMapItems.Count; i++)
		{
			if (WorldMapItems[i].ItemId == itemId)
			{
				return WorldMapItems[i].DetailBackgroundPrefab;
			}
		}
		return null;
	}
}
