using TWDModel;
using UnityEngine;

public class EquipmentFavouriteVisibility : MonoBehaviour
{
	[SerializeField]
	private GameObject On;

	public void UpdateVisibility(EquipmentItemModel item)
	{
		On.SetActive(item?.IsFavourite ?? false);
	}
}
