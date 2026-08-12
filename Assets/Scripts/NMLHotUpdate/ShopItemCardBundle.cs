using UnityEngine;

public class ShopItemCardBundle : ShopItemCard
{
	[Header("Bundle Items List")]
	[SerializeField]
	private NUIScrollableList scrollableList;

	public const string defaultItemPrefabName = "Bundle_List_Item";

	public const string defaultEquipmentPrefabName = "Bundle_List_Equipment";

	public const string defaultConsumablePrefabName = "Bundle_List_Consumable";

	[SerializeField]
	private GameObject shinyEffect;

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(shinyEffect, value: false);
		if (base.storeDefinition != null && base.storeDefinition.Shiny)
		{
			Helpers.GameObjectSetActive(shinyEffect, value: true);
		}
		if (contentDefinition == null || contentDefinition.RewardEntries == null || !(scrollableList != null))
		{
			return;
		}
		scrollableList.Clear();
		NUIListItem<IReward> nUIListItem = null;
		for (int i = 0; i < contentDefinition.RewardEntries.RewardsList.Count; i++)
		{
			IReward reward = contentDefinition.RewardEntries.RewardsList[i];
			if (reward == null)
			{
				continue;
			}
			if (reward.Type == RewardType.Equipment || reward.Type == RewardType.RandomEquipment || reward.Type == RewardType.EquipToken)
			{
				if (reward.Type == RewardType.Equipment)
				{
					RewardEquipment obj = reward as RewardEquipment;
					if (obj != null && obj.IsConsumableReward(GameManager.Instance.modelManager))
					{
						nUIListItem = scrollableList.InstantiateAdd("Bundle_List_Consumable") as NUIListItem<IReward>;
						goto IL_011b;
					}
				}
				nUIListItem = scrollableList.InstantiateAdd("Bundle_List_Equipment") as NUIListItem<IReward>;
			}
			else
			{
				nUIListItem = scrollableList.InstantiateAdd("Bundle_List_Item") as NUIListItem<IReward>;
			}
			goto IL_011b;
			IL_011b:
			if (nUIListItem != null)
			{
				nUIListItem.SetData(reward);
			}
		}
		scrollableList.SortAndReset();
		for (int j = 0; j < scrollableList.currentItemsList.Count; j++)
		{
			NestedUIDragScrollView nestedUIDragScrollView = Helpers.AddComponent<NestedUIDragScrollView>(scrollableList.currentItemsList[j].gameObject);
			if (nestedUIDragScrollView != null)
			{
				nestedUIDragScrollView.target = GetComponent<UIDragScrollView>();
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (scrollableList != null)
		{
			scrollableList.Clear();
		}
	}
}
