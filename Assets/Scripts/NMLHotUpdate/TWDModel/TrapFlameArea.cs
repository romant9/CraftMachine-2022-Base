namespace TWDModel
{
	public class TrapFlameArea : CombatAreaSingleGrid
	{
		public ActorModel Owner;

		public FixedPoint InjuryHPPercent;

		public override CombatAreaType Type => CombatAreaType.TrapFlame;

		public TrapFlameArea()
		{
		}

		public TrapFlameArea(ActorModel owner, GridCoordinate targetGridCoordinate, Faction faction, int expiryTurn, GridCoordinate effectiveAreaGridCoordinate, FixedPoint injuryHpPercent)
			: base(targetGridCoordinate, faction, expiryTurn, effectiveAreaGridCoordinate)
		{
			Owner = owner;
			InjuryHPPercent = injuryHpPercent;
		}

		public TrapFlameArea(TrapFlameArea area)
			: base(area)
		{
			Owner = area.Owner;
			InjuryHPPercent = area.InjuryHPPercent;
		}
	}
}
