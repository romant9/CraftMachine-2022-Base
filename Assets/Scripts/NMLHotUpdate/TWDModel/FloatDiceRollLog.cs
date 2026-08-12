namespace TWDModel
{
	public class FloatDiceRollLog : DiceRollLog
	{
		public FixedPoint SuccessProbability;

		public FixedPoint SuccessProbabilityExtension;

		public FixedPoint Roll;

		public override string ToString()
		{
			string[] obj = new string[12]
			{
				"Roll(",
				RollDiceType.ToString(),
				") ",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			FixedPoint roll = Roll;
			obj[3] = roll.ToString();
			obj[4] = " < (";
			roll = SuccessProbability;
			obj[5] = roll.ToString();
			obj[6] = " + ";
			roll = SuccessProbabilityExtension;
			obj[7] = roll.ToString();
			obj[8] = " = ";
			roll = (SuccessProbability *= 1.0 + SuccessProbabilityExtension);
			obj[9] = roll.ToString();
			obj[10] = ") results in ";
			obj[11] = Result.ToString();
			return string.Concat(obj);
		}
	}
}
