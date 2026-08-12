using System;

namespace TWDModel
{
	[Serializable]
	public class TokenToRarityAmount
	{
		public CurrencyType Type;

		public int Common;

		public int Uncommon;

		public int Rare;

		public int Epic;

		public int Legendary;
	}
}
