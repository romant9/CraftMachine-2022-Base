using System.Collections.Generic;
using TWDModel;

public class PhoneWeaponListPanel : ScrollableListPanel<EquipPrizeWheelDefinition>
{
	public void Init(List<EquipPrizeWheelDefinition> definitions)
	{
		SetCards(definitions);
	}
}
