using System.Collections.Generic;

public class GoldRadioWeaponRightListPanel : ScrollableListPanel<string>
{
	public void Init(List<string> definitions)
	{
		Helpers.GameObjectSetActive(cardPrefab, value: true);
		SetCards(definitions);
		Helpers.GameObjectSetActive(cardPrefab, value: false);
	}
}
