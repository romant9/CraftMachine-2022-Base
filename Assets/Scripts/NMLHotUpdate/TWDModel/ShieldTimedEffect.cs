namespace TWDModel
{
	public class ShieldTimedEffect : TimedEffect
	{
		public int Shield { get; set; }

		public ShieldTimedEffect(TimedEffectType type, int duration, int counter, Faction faction)
			: base(type, duration, counter, faction)
		{
			Shield = 0;
		}
	}
}
