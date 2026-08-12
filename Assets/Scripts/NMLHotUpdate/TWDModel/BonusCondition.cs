namespace TWDModel
{
	public interface BonusCondition
	{
		FixedPoint BonusValue { get; }

		bool Evaluate(ConditionContext context);
	}
}
