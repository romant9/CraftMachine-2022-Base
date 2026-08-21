using UnityEngine;

public class WorldBossPVPItem : NUIListItem<WorldBossPVPCellSlotData[]>
{
	[SerializeField]
	private WorldBossPVPItemItem item1;

	[SerializeField]
	private WorldBossPVPItemItem item2;

	[SerializeField]
	private WorldBossPVPItemItem item3;

	public override void SetData(WorldBossPVPCellSlotData[] data)
	{
		base.SetData(data);
		WorldBossPVPCellSlotData slotData = GetSlotData(0);
		WorldBossPVPCellSlotData slotData2 = GetSlotData(1);
		WorldBossPVPCellSlotData slotData3 = GetSlotData(2);
		Helpers.GameObjectSetActive(item1, slotData?.HasValue ?? false);
		Helpers.GameObjectSetActive(item2, slotData2?.HasValue ?? false);
		Helpers.GameObjectSetActive(item3, slotData3?.HasValue ?? false);
		UpdateUI();
	}

	public WorldBossPVPCellSlotData GetSlotData(int index)
	{
		WorldBossPVPCellSlotData[] data = GetData();
		if (data == null || index < 0 || index >= data.Length)
		{
			return null;
		}
		return data[index];
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (item1 != null)
		{
			item1.SetData(GetSlotData(0));
		}
		if (item2 != null)
		{
			item2.SetData(GetSlotData(1));
		}
		if (item3 != null)
		{
			item3.SetData(GetSlotData(2));
		}
	}

	public override void Clear()
	{
		base.Clear();
	}
}
