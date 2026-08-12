using System.Collections.Generic;
using TWDModel;

public class GoldRadioWeaponListPanel : ScrollableListPanel<EquipPrizeWheelDefinition>
{
	public void Init(List<EquipPrizeWheelDefinition> definitions)
	{
		Helpers.GameObjectSetActive(cardPrefab, value: true);
		SetCards(definitions);
		Helpers.GameObjectSetActive(cardPrefab, value: false);
		Reposition(resetScrollView: true);
	}

	public void Reposition(bool resetScrollView = false)
	{
		PositionCards(resetScrollView);
	}
}
