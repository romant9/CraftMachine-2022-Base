namespace TWDModel
{
	public sealed class NegativeEffectWithWeight : IWeightedItem
	{
		public string NegativeEffect { get; private set; }

		public int Weight { get; private set; }

		public NegativeEffectWithWeight(string negativeEffect, int weight)
		{
			NegativeEffect = negativeEffect;
			Weight = weight;
		}

		public int GetWeight()
		{
			return Weight;
		}
	}
}
