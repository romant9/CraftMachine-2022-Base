using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BadgeRarityResult
	{
		public int Total;

		public int MinRarity;

		public FixedPoint Common;

		public FixedPoint Uncommon;

		public FixedPoint Rare;

		public FixedPoint Epic;

		public FixedPoint Legendary;

		[JsonIgnore]
		public int MaxRarity
		{
			get
			{
				if (Legendary > 0L)
				{
					return 4;
				}
				if (Epic > 0L)
				{
					return 3;
				}
				if (Rare > 0L)
				{
					return 2;
				}
				if (Uncommon > 0L)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
