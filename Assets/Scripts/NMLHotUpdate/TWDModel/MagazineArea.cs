namespace TWDModel
{
	public class MagazineArea : CombatAreaSingleGrid
	{
		public string RequiredTraitIdentifier { get; private set; }

		public override CombatAreaType Type => CombatAreaType.Magazine;

		public MagazineArea()
		{
		}

		public MagazineArea(GridCoordinate coordinate, Faction faction, int expiryTurn, string requiredTraitIdentifier)
			: base(coordinate, faction, expiryTurn, coordinate)
		{
			RequiredTraitIdentifier = requiredTraitIdentifier;
		}

		public MagazineArea(MagazineArea area)
			: base(area)
		{
			RequiredTraitIdentifier = area.RequiredTraitIdentifier;
		}
	}
}
