namespace TWDModel
{
	public class ScorchTimedEffectBackup : TimeEffectBackup
	{
		public int DamageChance { get; set; }

		public int Layers { get; set; }

		public int MaxLayers { get; set; }

		public void RecordStatus(ScorchTimedEffect scorchTimedEffect)
		{
			RecordStatus((TimedEffect)scorchTimedEffect);
			DamageChance = scorchTimedEffect.DamageChance;
			Layers = scorchTimedEffect.Layers;
			MaxLayers = scorchTimedEffect.MaxLayers;
		}

		public override void BackUp()
		{
			base.BackUp();
			ScorchTimedEffect obj = base.Model as ScorchTimedEffect;
			obj.DamageChance = DamageChance;
			obj.Layers = Layers;
			obj.MaxLayers = MaxLayers;
		}
	}
}
