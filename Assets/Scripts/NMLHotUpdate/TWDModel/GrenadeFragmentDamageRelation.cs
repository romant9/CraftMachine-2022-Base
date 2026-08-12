namespace TWDModel
{
	public class GrenadeFragmentDamageRelation : ActorToActorRelation
	{
		public FixedPoint AdditionDamagePercentage { get; private set; }

		public FixedPoint AdditionAddDamagePercentage { get; private set; }

		public override RelationType Type => RelationType.GrenadeFragmentDamage;

		public GrenadeFragmentDamageRelation()
		{
		}

		public GrenadeFragmentDamageRelation(GrenadeFragmentDamageRelation grenadeFragmentDamageRelation)
			: base(grenadeFragmentDamageRelation)
		{
			AdditionDamagePercentage = grenadeFragmentDamageRelation.AdditionDamagePercentage;
			AdditionAddDamagePercentage = grenadeFragmentDamageRelation.AdditionAddDamagePercentage;
		}

		public GrenadeFragmentDamageRelation(ActorModel source, ActorModel target, Faction foundingFaction, int expiryTurn, FixedPoint additionDamagePercentage, FixedPoint additionAddDamagePercentage)
			: base(source, target, foundingFaction, expiryTurn)
		{
			AdditionDamagePercentage = additionDamagePercentage;
			AdditionAddDamagePercentage = additionAddDamagePercentage;
		}

		public void UpdateRelation(ActorModel source, Faction foundingFaction, FixedPoint additionDamagePercentage, FixedPoint additionAddDamagePercentage)
		{
			base.SourceActor = source;
			base.FoundingFaction = foundingFaction;
			AdditionDamagePercentage = additionDamagePercentage;
			AdditionAddDamagePercentage = additionAddDamagePercentage;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
