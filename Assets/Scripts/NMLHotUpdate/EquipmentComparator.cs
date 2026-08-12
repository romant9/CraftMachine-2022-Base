using TWDModel;
using UnityEngine;

public class EquipmentComparator
{
	public int DamageIncrease { get; private set; }

	public Color DamageColor { get; private set; }

	public void SetItems(EquipmentItemModel baseItem, EquipmentItemModel newItem)
	{
		DamageIncrease = newItem.Damage;
		if (baseItem != null)
		{
			DamageIncrease -= baseItem.Damage;
		}
		if (DamageIncrease > 0)
		{
			DamageColor = Color.green;
		}
		else if (DamageIncrease < 0)
		{
			DamageColor = Color.red;
		}
		else
		{
			DamageColor = Color.black;
		}
	}
}
