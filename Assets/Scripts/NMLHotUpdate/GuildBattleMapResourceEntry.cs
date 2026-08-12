using System;
using UnityEngine;

[Serializable]
public class GuildBattleMapResourceEntry : ResourceEntry
{
	public string MaterialName;

	[NonSerialized]
	public Material Material;
}
