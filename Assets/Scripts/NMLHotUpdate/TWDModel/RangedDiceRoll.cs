namespace TWDModel
{
	public class RangedDiceRoll : DiceRollLog
	{
		public int Min;

		public int Max;

		public int Roll;

		public override string ToString()
		{
			return "Roll(" + RollDiceType.ToString() + ") " + Roll + " [" + Min + ".." + Max + "]";
		}
	}
}
