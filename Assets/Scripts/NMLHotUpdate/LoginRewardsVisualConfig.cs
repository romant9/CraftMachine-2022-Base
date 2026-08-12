using System.Collections.Generic;
using UnityEngine;

public class LoginRewardsVisualConfig : ScriptableObject
{
	public RewardVisualizationEntry Hero;

	public RewardVisualizationEntry Currency;

	public List<RarityVisualizationEntry> rarity = new List<RarityVisualizationEntry>();

	public RarityVisualizationEntry GetRarityVisualization(int rarityLevel)
	{
		for (int i = 0; i < rarity.Count; i++)
		{
			if (rarity[i].RarityLevel == rarityLevel)
			{
				return rarity[i];
			}
		}
		return null;
	}
}
