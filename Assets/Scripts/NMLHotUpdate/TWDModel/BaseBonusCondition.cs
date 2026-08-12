namespace TWDModel
{
	public abstract class BaseBonusCondition : BonusCondition
	{
		private FixedPoint bonusValue;

		public FixedPoint BonusValue => bonusValue;

		public BaseBonusCondition(FixedPoint bonus)
		{
			bonusValue = bonus;
		}

		public abstract bool Evaluate(ConditionContext context);
	}
}
