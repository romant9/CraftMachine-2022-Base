using System;
using UnityEngine;

[Serializable]
public class GuildBattleMapLineAssets
{
	public GameObject LineRendererPrefab;

	public Material SomeUnlockedMateria;

	public Material AllUnlockedLineMateria;

	public Material NoneUnlockedLineMateria;

	public bool NotEmpty()
	{
		if (LineRendererPrefab != null && SomeUnlockedMateria != null && AllUnlockedLineMateria != null)
		{
			return NoneUnlockedLineMateria != null;
		}
		return false;
	}
}
