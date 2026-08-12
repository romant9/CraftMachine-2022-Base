using System.Collections.Generic;
using TWDModel;

public class HeroSkinList : ScrollableListPanel<HeroSkinInfo>
{
	private SurvivorModel survivor;

	public void CreateItems(HeroSkinResourceEntry heroResource, SurvivorModel survivor)
	{
		this.survivor = survivor;
		SetCards(heroResource.HeroSkins);
		updateListState(survivor.CharacterPrefab);
	}

	public void CreateItemForPreview(HeroSkinInfo heroSkinInfo, SurvivorModel survivor)
	{
		this.survivor = survivor;
		List<HeroSkinInfo> items = new List<HeroSkinInfo> { heroSkinInfo };
		SetCards(items);
		updateListState(survivor.CharacterPrefab);
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewOutfitSeleted")
		{
			HeroSkinInfo heroSkinInfo = parameter as HeroSkinInfo;
			updateListState(heroSkinInfo.PrefabId);
		}
	}

	private void updateListState(string heroSkin)
	{
		for (int i = 0; i < GetCards().Count; i++)
		{
			HeroSkinListItem heroSkinListItem = GetCards()[i] as HeroSkinListItem;
			if (heroSkinListItem != null && heroSkinListItem.GetPrefabId() == heroSkin)
			{
				heroSkinListItem.Select();
			}
			else
			{
				heroSkinListItem.Deselect();
			}
			heroSkinListItem.UpdateUI();
		}
	}
}
