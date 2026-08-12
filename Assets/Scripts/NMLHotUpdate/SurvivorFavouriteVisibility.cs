using TWDModel;
using UnityEngine;

public class SurvivorFavouriteVisibility : MonoBehaviour
{
	[SerializeField]
	private GameObject On;

	public void UpdateVisibility(SurvivorModel item)
	{
		On.SetActive(item?.IsFavourite ?? false);
	}
}
