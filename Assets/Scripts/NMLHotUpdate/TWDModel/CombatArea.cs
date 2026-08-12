namespace TWDModel
{
	public abstract class CombatArea : TWDModelObject
	{
		public FixedPoint Radius;

		public FixedPoint XLength;

		public FixedPoint YLength;

		public GridCoordinate Coordinate;

		public Faction Faction;

		public int ExpiryTurn;

		public abstract CombatAreaType Type { get; }

		public CombatArea()
		{
		}

		public CombatArea(FixedPoint xLength, FixedPoint yLength, FixedPoint radius, GridCoordinate gridCoordinate, Faction faction, int expiryTurn)
		{
			Radius = radius;
			XLength = xLength;
			YLength = yLength;
			Coordinate = gridCoordinate;
			Faction = faction;
			ExpiryTurn = expiryTurn;
		}

		public CombatArea(CombatArea area)
		{
			Radius = area.Radius;
			XLength = area.XLength;
			YLength = area.YLength;
			Coordinate = area.Coordinate;
			Faction = area.Faction;
			ExpiryTurn = area.ExpiryTurn;
		}

		public CombatArea(FixedPoint radius, GridCoordinate gridCoordinate, Faction faction, int expiryTurn)
		{
			Radius = radius;
			Coordinate = gridCoordinate;
			Faction = faction;
			ExpiryTurn = expiryTurn;
		}

		public virtual bool IsInArea(GridCoordinate otherCoord)
		{
			return otherCoord.SquaredDistanceTo(Coordinate) < Radius * Radius;
		}

		public virtual bool IsNearAreaGrid(GridCoordinate otherCoord)
		{
			return false;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
