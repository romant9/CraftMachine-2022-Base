namespace TWDModel
{
	public abstract class CombatAreaSingleGrid : CombatArea
	{
		public GridCoordinate EffectiveAreaGridCoordinate { get; private set; }

		public CombatAreaSingleGrid()
		{
		}

		public CombatAreaSingleGrid(GridCoordinate targetGridCoordinate, Faction faction, int expiryTurn, GridCoordinate effectiveAreaGridCoordinate)
		{
			Coordinate = targetGridCoordinate;
			Faction = faction;
			ExpiryTurn = expiryTurn;
			EffectiveAreaGridCoordinate = effectiveAreaGridCoordinate;
		}

		public CombatAreaSingleGrid(CombatAreaSingleGrid area)
			: base(area)
		{
			EffectiveAreaGridCoordinate = area.EffectiveAreaGridCoordinate;
		}

		public override bool IsValid()
		{
			return true;
		}

		public override bool IsInArea(GridCoordinate otherCoord)
		{
			return false;
		}
	}
}
