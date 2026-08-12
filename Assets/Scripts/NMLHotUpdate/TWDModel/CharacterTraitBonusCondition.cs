namespace TWDModel
{
	public class CharacterTraitBonusCondition : BaseBonusCondition
	{
		private string traitId;

		public CharacterTraitBonusCondition(FixedPoint bonus, string traitIdentifier)
			: base(bonus)
		{
			traitId = traitIdentifier;
		}

		public override bool Evaluate(ConditionContext context)
		{
			return context.GetBadgeOwner()?.HasAnyLevelTrait(traitId) ?? false;
		}
	}
}
