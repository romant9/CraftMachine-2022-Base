using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GuildWar/GuildBattleMapConfig", fileName = "GuildBattleMapConfig")]
public class GuildBattleMapConfigResourcesMap : ResourcesMap<GuildBattleMapResourceEntry>
{
	private Dictionary<string, string> dict = new Dictionary<string, string>();

	internal void Init()
	{
		dict.Clear();
		GuildBattleMapResourceEntry[] array = resources;
		foreach (GuildBattleMapResourceEntry guildBattleMapResourceEntry in array)
		{
			if (dict.ContainsKey(guildBattleMapResourceEntry.Identifier))
			{
				Debug.LogError("Duplicate index " + guildBattleMapResourceEntry.Identifier + " for card pack image: " + guildBattleMapResourceEntry.MaterialName + " not found in the asset list");
			}
			else
			{
				dict[guildBattleMapResourceEntry.Identifier] = guildBattleMapResourceEntry.MaterialName;
			}
		}
	}

	internal GuildBattleMapResourceEntry GetEntryByIndex(int index)
	{
		if (0 <= index && index < resources.Length)
		{
			return resources[index];
		}
		return null;
	}
}
