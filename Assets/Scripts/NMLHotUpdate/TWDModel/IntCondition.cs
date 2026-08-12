namespace TWDModel
{
	public class IntCondition
	{
		public static bool Compare(ConditionOperator op, int a, int b)
		{
			return op switch
			{
				ConditionOperator.Equal => a == b, 
				ConditionOperator.GreaterThan => a > b, 
				ConditionOperator.GreaterThanOrEqual => a >= b, 
				ConditionOperator.LessThan => a < b, 
				ConditionOperator.LessThanOrEqual => a <= b, 
				ConditionOperator.NotEqual => a != b, 
				_ => false, 
			};
		}
	}
}
