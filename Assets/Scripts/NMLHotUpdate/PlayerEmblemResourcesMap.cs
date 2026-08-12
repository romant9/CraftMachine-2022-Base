using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerEmblemResourcesMap : ScriptableObject
{
	public List<string> IconSprites;

	public List<string> Borders;

	public List<Color> BackgroundColors;

	public int IconCount => IconSprites.Count;

	public int BorderCount => Borders.Count;

	public int ColorsCount => BackgroundColors.Count;

	public string GetIcon(int index)
	{
		if (index < 0 || index >= IconCount)
		{
			Debug.LogError("PlayerEmblem icon not found at index " + index + " Returning Default");
			return IconSprites[0];
		}
		return IconSprites[index];
	}

	public string GetBorder(int index)
	{
		if (index < 0 || index >= BorderCount)
		{
			Debug.LogError("PlayerEmblem border not found at index " + index + " Returning Default");
			return Borders[0];
		}
		return Borders[index];
	}

	public Color GetColor(int index)
	{
		if (index < 0 || index >= ColorsCount)
		{
			Debug.LogError("PlayerEmblem color not found at index " + index + " Returning Default");
			return BackgroundColors[0];
		}
		return BackgroundColors[index];
	}
}
