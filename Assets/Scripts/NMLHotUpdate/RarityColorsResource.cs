using UnityEngine;

public class RarityColorsResource : ScriptableObject
{
	public ColorEntry[] resources;

	public ColorEntry GetRarityColor(int rarityLevel)
	{
		ColorEntry result = null;
		ColorEntry result2 = null;
		for (int i = 0; i < resources.Length; i++)
		{
			ColorEntry colorEntry = resources[i];
			if (colorEntry.RarityLevel == 0)
			{
				result = colorEntry;
			}
			else if (colorEntry.RarityLevel == 4)
			{
				result2 = colorEntry;
			}
			if (colorEntry.RarityLevel == rarityLevel)
			{
				return colorEntry;
			}
		}
		if (rarityLevel > 4)
		{
			return result2;
		}
		return result;
	}
}
