using System;
using System.Collections.Generic;
using TWDModel;

public class MapGrid
{
	public FixedVec2 GridCellSize = new FixedVec2(0L, 0L);

	public Dictionary<string, IMapGridItem> Grid = new Dictionary<string, IMapGridItem>();

	private Callback onPositionChangeDelegate;

	public MapGrid(FixedPoint sizeX, FixedPoint sizeY)
	{
		SetGridSize(sizeX, sizeY);
	}

	public void SetGridSize(FixedPoint x, FixedPoint y)
	{
		GridCellSize.X = x;
		GridCellSize.Y = y;
	}

	public void AssignItemTo(IMapGridItem item, FixedPoint x, FixedPoint y)
	{
		if (item != null && GetAt(x, y) == null)
		{
			string key = "";
			FormatKey(x, y, out key);
			Grid[key] = item;
			item.x = x;
			item.y = y;
			item.AddedToGrid(this);
		}
	}

	public IMapGridItem GetAt(FixedPoint x, FixedPoint y)
	{
		string key = "";
		FormatKey(x, y, out key);
		if (Grid.ContainsKey(key))
		{
			return Grid[key];
		}
		return null;
	}

	public void PositionItems()
	{
		foreach (KeyValuePair<string, IMapGridItem> item in Grid)
		{
			if (item.Value != null)
			{
				item.Value.Position();
			}
		}
		if (onPositionChangeDelegate != null)
		{
			onPositionChangeDelegate();
		}
	}

	public void AddOnPositionCallback(Callback callback)
	{
		if (onPositionChangeDelegate != null)
		{
			onPositionChangeDelegate = (Callback)Delegate.Remove(onPositionChangeDelegate, callback);
			onPositionChangeDelegate = (Callback)Delegate.Combine(onPositionChangeDelegate, callback);
		}
		else
		{
			onPositionChangeDelegate = callback;
		}
	}

	public void RemoveOnPositionCallback(Callback callback)
	{
		if (onPositionChangeDelegate != null)
		{
			onPositionChangeDelegate = (Callback)Delegate.Remove(onPositionChangeDelegate, callback);
		}
	}

	public void GetAsListOf<T>(ref List<T> list) where T : class
	{
		if (list == null)
		{
			list = new List<T>();
		}
		else
		{
			list.Clear();
		}
		foreach (KeyValuePair<string, IMapGridItem> item in Grid)
		{
			if (item.Value != null)
			{
				list.Add(item.Value as T);
			}
		}
	}

	public void Clear()
	{
		if (Grid == null)
		{
			return;
		}
		foreach (KeyValuePair<string, IMapGridItem> item in Grid)
		{
			if (item.Value != null)
			{
				item.Value.Clear();
			}
		}
		onPositionChangeDelegate = null;
		Grid.Clear();
	}

	public static void FormatKey(FixedPoint x, FixedPoint y, out string key)
	{
		key = x.ToString() + "_" + y.ToString();
	}
}
