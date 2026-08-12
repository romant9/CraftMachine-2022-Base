using System;

namespace TWDModel
{
	[Serializable]
	public class ItemAmountProbabilityData
	{
		public string Name;

		public int ItemEnumValue;

		public Type ItemEnumType;

		public string Amount;

		public int Rarity;

		public FixedPoint Probability;

		public ItemAmountProbabilityData()
		{
			Rarity = -1;
		}

		public ItemAmountProbabilityData(ItemAmountProbabilityData other)
		{
			Name = other.Name;
			ItemEnumValue = other.ItemEnumValue;
			ItemEnumType = other.ItemEnumType;
			Amount = other.Amount;
			Rarity = other.Rarity;
			Probability = other.Probability;
		}
	}
}
