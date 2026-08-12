using UnityEngine;

public class SPRemoldTraitsUpdatedPopup : HUDElement
{
	[SerializeField]
	private SPRemoldTraitsUpdatedItem leftItem;

	[SerializeField]
	private SPRemoldTraitsUpdatedItem rightItem;

	public void InitData(string oldtraidID, string newtraidID)
	{
		leftItem.Setup(oldtraidID);
		rightItem.Setup(newtraidID);
	}
}
