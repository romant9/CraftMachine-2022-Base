namespace TWDModel
{
	public class EmitArea : CombatArea
	{
		public FixedPoint Multiplier;

		public override CombatAreaType Type => CombatAreaType.Emitter;

		public EmitArea()
		{
		}

		public EmitArea(FixedPoint radius, GridCoordinate gridCoordinate, Faction faction, int expiryTurn, FixedPoint multiplier)
			: base(radius, gridCoordinate, faction, expiryTurn)
		{
			Multiplier = multiplier;
		}

		public EmitArea(EmitArea area)
			: base(area)
		{
			Multiplier = area.Multiplier;
		}
	}
}
