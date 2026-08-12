using System;
using TWDModel;

[Serializable]
public class HeroSkinResourceEntry
{
	public string HeroDefinitionID;

	public HeroSkinInfo[] HeroSkins;

	public HeroSkinInfo GetHeroSkinInfoForSurvivor(SurvivorModel survivor)
	{
		HeroSkinInfo[] heroSkins = HeroSkins;
		foreach (HeroSkinInfo heroSkinInfo in heroSkins)
		{
			if (heroSkinInfo.PrefabId == survivor.CharacterPrefab)
			{
				return heroSkinInfo;
			}
		}
		return null;
	}
}
