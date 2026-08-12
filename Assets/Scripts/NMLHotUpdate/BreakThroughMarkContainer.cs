using UnityEngine;

public class BreakThroughMarkContainer : MonoBehaviour
{
	[SerializeField]
	private GameObject Add;

	[SerializeField]
	private GameObject Item;

	[SerializeField]
	private UILabel amount;

	private void OnEnable()
	{
		UpdateUI();
	}

	public void UpdateUI()
	{
		SetEmpty();
	}

	public void SetEmpty()
	{
		Helpers.GameObjectSetActive(Add, value: true);
		Helpers.GameObjectSetActive(Item, value: false);
	}

	public void SetFill()
	{
		Helpers.GameObjectSetActive(Add, value: false);
		Helpers.GameObjectSetActive(Item, value: true);
	}
}
