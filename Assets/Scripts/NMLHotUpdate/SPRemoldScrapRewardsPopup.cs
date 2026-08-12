using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldScrapRewardsPopup : HUDElement
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	private List<EquipmentItemModel> equipmentItemModels;

	private Rewards rewards;

	private readonly List<GameObject> Entries = new List<GameObject>();

	public void Setup(List<EquipmentItemModel> equipmentItemModels)
	{
		this.equipmentItemModels = equipmentItemModels;
		rewards = null;
		if (equipmentItemModels.Count > 0)
		{
			UpdateUI();
		}
	}

	public void SetupRewards(Rewards rewards)
	{
		this.rewards = rewards;
		equipmentItemModels = null;
		if (rewards != null && rewards.RewardsList != null && rewards.RewardsList.Count > 0)
		{
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		FreshListData();
	}

	private void FreshListData()
	{
		ClearEntries();
		Rewards rewards = this.rewards;
		UITable component = EntryContainer.GetComponent<UITable>();
		int count = rewards.RewardsList.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<SPRemoldScrapItem>(out var component2))
			{
				component2.Setup(rewards.RewardsList[i]);
			}
			Entries.Add(gameObject);
		}
		component.Reposition();
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}
}
