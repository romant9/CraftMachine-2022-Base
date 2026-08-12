using System.Collections.Generic;
using UnityEngine;

public class RecycleWeaponRewardsPopup : HUDElement
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	private Rewards rewards;

	private readonly List<GameObject> Entries = new List<GameObject>();

	public void SetupRewards(Rewards rewards)
	{
		this.rewards = rewards;
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
			if (gameObject.TryGetComponent<RouletteRewardCard>(out var component2))
			{
				component2.Bind(rewards.RewardsList[i]);
			}
			Entries.Add(gameObject);
		}
		Helpers.GameObjectSetActive(EntryPrefab, value: false);
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
