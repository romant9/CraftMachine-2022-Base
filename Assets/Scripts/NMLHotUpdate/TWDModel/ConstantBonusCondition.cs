namespace TWDModel
{
	public class ConstantBonusCondition : BaseBonusCondition
	{
		public ConstantBonusCondition(FixedPoint bonus)
			: base(bonus)
		{
		}

		public override bool Evaluate(ConditionContext context)
		{
			return true;
		}
	}
}
