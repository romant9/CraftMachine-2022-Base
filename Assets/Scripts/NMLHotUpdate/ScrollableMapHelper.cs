using System.Collections.Generic;
using TWDModel;

public class ScrollableMapHelper
{
	public const int maxSlotsOnLayer = 6;

	public const int mapSlotSize = 1024;

	public const int halfMapSlotSize = 512;

	private static int GetSlotPositionX(int slot)
	{
		return (slot - 1) * 1024 + 512;
	}

	private static int GetSlotPositionY(int slot)
	{
		return 512;
	}

	private static ScrollableMapItemInstance CreateMapItemInstance(ScrollableMapItem item, MapLayer layer, int slot)
	{
		if (slot < 1 || slot > 6)
		{
			GameManager.Instance.playerModel.Debug.LogError("Scrollable map item slot out of range.");
			return null;
		}
		return new ScrollableMapItemInstance
		{
			Item = item,
			Layer = layer,
			SlotOnLayer = slot,
			GlobalX = GetSlotPositionX(slot) + item.X,
			GlobalY = GetSlotPositionY(slot) + item.Y
		};
	}

	public static bool GetMapItems(string mapName, ref List<ScrollableMapItemInstance> bgItems, ref List<ScrollableMapItemInstance> fgItems, ref List<ScrollableMapItemInstance> pathItems)
	{
		bool result = false;
		ScrollableMapItem[] scrollableMaps = GameManager.Instance.gameEconomyData.ScrollableMaps;
		for (int i = 0; i < scrollableMaps.Length; i++)
		{
			if (scrollableMaps[i].Map != mapName)
			{
				continue;
			}
			if (scrollableMaps[i].BG != 0)
			{
				ScrollableMapItemInstance scrollableMapItemInstance = CreateMapItemInstance(scrollableMaps[i], MapLayer.BG, scrollableMaps[i].BG);
				if (scrollableMapItemInstance != null)
				{
					bgItems.Add(scrollableMapItemInstance);
				}
			}
			else if (scrollableMaps[i].FG != 0)
			{
				ScrollableMapItemInstance scrollableMapItemInstance2 = CreateMapItemInstance(scrollableMaps[i], MapLayer.FG, scrollableMaps[i].FG);
				if (scrollableMapItemInstance2 != null)
				{
					fgItems.Add(scrollableMapItemInstance2);
				}
			}
			else if (scrollableMaps[i].Path != 0)
			{
				ScrollableMapItemInstance scrollableMapItemInstance3 = CreateMapItemInstance(scrollableMaps[i], MapLayer.FG, scrollableMaps[i].Path);
				if (scrollableMapItemInstance3 != null)
				{
					pathItems.Add(scrollableMapItemInstance3);
				}
			}
			else
			{
				GameManager.Instance.playerModel.Debug.LogError("Encountered a scrollable map items that was not on any layer (BG, FG or Path).");
			}
			result = true;
		}
		return result;
	}
}
