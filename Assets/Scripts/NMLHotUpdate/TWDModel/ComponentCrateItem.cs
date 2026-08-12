using System;

namespace TWDModel
{
	[Serializable]
	public class ComponentCrateItem
	{
		public string Type;

		public int Rarity;

		public int Count;

		public ComponentCrateItem(string type, int rarity, int count)
		{
			Count = count;
			Type = type;
			Rarity = rarity;
		}

		public bool IsFixedRarity()
		{
			return Rarity != -1;
		}

		public bool IsFixedType()
		{
			return !string.IsNullOrEmpty(Type);
		}
	}
}
