namespace TWDModel
{
	public class TraitApplyEntry
	{
		public string TraitId;

		public FixedPoint Chance;

		public int Turns;

		public bool HasChanceOverride => Chance > 0L;

		public bool HasTurns => Turns > 0;
	}
}
