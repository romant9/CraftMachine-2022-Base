namespace TWDModel
{
	public class IntDiceRollLog : DiceRollLog
	{
		public int SuccessProbability;

		public int SuccessProbabilityExtension;

		public int Roll;

		public override string ToString()
		{
			string[] obj = new string[12]
			{
				"Roll(",
				RollDiceType.ToString(),
				") ",
				Roll.ToString(),
				" < (",
				SuccessProbability.ToString(),
				" + ",
				SuccessProbabilityExtension.ToString(),
				" = ",
				null,
				null,
				null
			};
			int num = (SuccessProbability *= 1 + SuccessProbabilityExtension);
			obj[9] = num.ToString();
			obj[10] = ") results in ";
			obj[11] = Result.ToString();
			return string.Concat(obj);
		}
	}
}
