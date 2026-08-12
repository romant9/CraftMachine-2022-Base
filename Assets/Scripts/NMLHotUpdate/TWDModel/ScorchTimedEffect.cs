namespace TWDModel
{
	public class ScorchTimedEffect : TimedEffect
	{
		public int DamageChance { get; set; }

		public int Layers { get; set; }

		public int MaxLayers { get; set; }

		public ScorchTimedEffect(TimedEffectType type, int duration, int counter, Faction faction)
			: base(type, duration, counter, faction)
		{
			DamageChance = 0;
			Layers = 1;
		}
	}
}
