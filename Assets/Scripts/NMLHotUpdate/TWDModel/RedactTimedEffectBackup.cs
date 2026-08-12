using BaseModel;

namespace TWDModel
{
	public class RedactTimedEffectBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public RedactTimedEffect Model { get; set; }

		public int Layers { get; set; }

		public int IncreaseDamageRatio { get; set; }

		public FixedPoint ReduceHpChance { get; set; }

		public int ReducedHpRatio { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(RedactTimedEffect timedEffect)
		{
			Model = timedEffect;
			Layers = timedEffect.Layers;
			IncreaseDamageRatio = timedEffect.IncreaseDamageRatio;
			ReduceHpChance = timedEffect.ReduceHpChance;
			ReducedHpRatio = timedEffect.ReducedHpRatio;
		}

		public void BackUp()
		{
			Model.Layers = Layers;
			Model.IncreaseDamageRatio = IncreaseDamageRatio;
			Model.ReduceHpChance = ReduceHpChance;
			Model.ReducedHpRatio = ReducedHpRatio;
		}
	}
}
