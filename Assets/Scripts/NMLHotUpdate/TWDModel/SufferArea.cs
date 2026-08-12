namespace TWDModel
{
	public class SufferArea : CombatArea
	{
		public ActorModel Owner;

		public override CombatAreaType Type => CombatAreaType.Suffer;

		public SufferArea()
		{
		}

		public SufferArea(FixedPoint radius, GridCoordinate gridCoordinate, Faction faction, int expiryTurn, ActorModel owner)
			: base(radius, gridCoordinate, faction, expiryTurn)
		{
			Owner = owner;
		}

		public SufferArea(SufferArea area)
			: base(area)
		{
			Owner = area.Owner;
		}
	}
}
