using TWDModel;
using UnityEngine;

public class WeaponExtraInfoPanel : MonoBehaviour
{
	public delegate void WeaponInfoClosedDelegate();

	public EquipmentItemModel EquipmentItem { get; set; }

	public static event WeaponInfoClosedDelegate OnWeaponInfoClosed;

	public void OnClicked()
	{
		if (WeaponExtraInfoPanel.OnWeaponInfoClosed != null)
		{
			WeaponExtraInfoPanel.OnWeaponInfoClosed();
		}
	}
}
