using TWDModel;

public class GridCoordinateMovementDirection
{
	public GridCoordinate coordinate;

	public bool canMoveNE;

	public bool canMoveSE;

	public bool canMoveSW;

	public bool canMoveNW;

	public GridCoordinateMovementDirection(GridCoordinate coordinate)
	{
		this.coordinate = coordinate;
	}
}
