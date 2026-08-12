using TWDModel;
using UnityEngine;

public class FactionColorsResource : ScriptableObject
{
	public FactionColorEntry[] resources;

	public FactionColorEntry GetFactionColor(Faction faction)
	{
		FactionColorEntry result = null;
		for (int i = 0; i < resources.Length; i++)
		{
			FactionColorEntry factionColorEntry = resources[i];
			if (factionColorEntry.Faction == faction)
			{
				return factionColorEntry;
			}
			if (factionColorEntry.Faction == Faction.Any)
			{
				result = factionColorEntry;
			}
		}
		return result;
	}
}
