using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroSkinResources", menuName = "Hero Skins")]
public class HeroSkinResourcesMap : ScriptableObject
{
	public List<HeroSkinResourceEntry> Skins;
}
