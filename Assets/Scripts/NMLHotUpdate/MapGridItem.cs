using TWDModel;
using UnityEngine;

public class MapGridItem : MonoBehaviourExtended, IMapGridItem
{
	protected Vector3 newPosition;

	public MapGrid OwnerGrid { get; set; }

	public FixedPoint x { get; set; }

	public FixedPoint y { get; set; }

	public virtual void AddedToGrid(MapGrid grid)
	{
		OwnerGrid = grid;
	}

	public virtual void Position()
	{
		base.transform.localPosition = CalculateGridPosition();
	}

	public virtual Vector3 CalculateGridPosition()
	{
		newPosition = Helpers.staticVector3Zero;
		if (OwnerGrid != null)
		{
			newPosition.x = (long)(OwnerGrid.GridCellSize.X * x);
			newPosition.y = (long)(OwnerGrid.GridCellSize.Y * y);
		}
		return newPosition;
	}

	public string GetGridKey()
	{
		string key = "";
		MapGrid.FormatKey(x, y, out key);
		return key;
	}

	public override void Clear()
	{
		base.Clear();
		OwnerGrid = null;
		x = 0L;
		y = 0L;
	}
}
