using System.Collections.Generic;
using UnityEngine;

public class UIGridExtended : UIGrid
{
	public bool AllowActiveSelfUpdate = true;

	private List<Transform> activeChildList;

	public override void Reposition()
	{
		if (!AllowActiveSelfUpdate)
		{
			base.Reposition();
			return;
		}
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(base.gameObject))
		{
			Init();
		}
		if (TryToPopulateChildList(ref activeChildList))
		{
			ResetPosition(activeChildList);
			if (keepWithinPanel)
			{
				ConstrainWithinPanel();
			}
			if (onReposition != null)
			{
				onReposition();
			}
			activeChildList.Clear();
		}
	}

	public bool TryToPopulateChildList(ref List<Transform> list)
	{
		if (base.transform == null)
		{
			return false;
		}
		if (list == null)
		{
			list = new List<Transform>();
		}
		else
		{
			list.Clear();
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child != null && child.gameObject != null && (!hideInactive || child.gameObject.activeSelf))
			{
				list.Add(child);
			}
		}
		if (sorting != Sorting.None && arrangement != Arrangement.CellSnap)
		{
			if (sorting == Sorting.Alphabetic)
			{
				list.Sort(UIGrid.SortByName);
			}
			else if (sorting == Sorting.Horizontal)
			{
				list.Sort(UIGrid.SortHorizontal);
			}
			else if (sorting == Sorting.Vertical)
			{
				list.Sort(UIGrid.SortVertical);
			}
			else if (onCustomSort != null)
			{
				list.Sort(onCustomSort);
			}
			else
			{
				Sort(list);
			}
		}
		if (list != null)
		{
			return list.Count > 0;
		}
		return false;
	}
}
