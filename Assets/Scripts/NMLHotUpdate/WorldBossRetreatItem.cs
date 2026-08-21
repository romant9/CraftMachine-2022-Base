using UnityEngine;

public class WorldBossRetreatItem : NUIListItem<string>
{
	[SerializeField]
	private UILabel itemNameLabel;

	[SerializeField]
	private GameObject item1;

	[SerializeField]
	private GameObject item2;

	public override void SetData(string data)
	{
		base.SetData(data);
		UpdateUI();
	}

	public void Setup(string data1, string data2)
	{
		Helpers.GameObjectSetActive(item1, data1 != null);
		Helpers.GameObjectSetActive(item2, data2 != null);
		SetData(data1);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}

	public override void Clear()
	{
		base.Clear();
	}
}
