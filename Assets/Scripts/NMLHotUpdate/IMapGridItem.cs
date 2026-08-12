using TWDModel;

public interface IMapGridItem
{
	MapGrid OwnerGrid { get; set; }

	FixedPoint x { get; set; }

	FixedPoint y { get; set; }

	void AddedToGrid(MapGrid grid);

	void Position();

	void Clear();
}
