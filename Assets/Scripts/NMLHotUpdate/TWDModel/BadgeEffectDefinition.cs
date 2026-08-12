using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class BadgeEffectDefinition : TypeIndexDefinition
	{
		public string TraitId;

		public int Level;

		public bool IsRelative;

		public string Category;

		[GEDListFromColumns]
		public List<int> Strengths;

		public List<int> GetStrengthForRarity(int rarity)
		{
			if (rarity < 0 || Strengths.Count < 2)
			{
				return new List<int> { 1, 1 };
			}
			int num = Math.Min(rarity, Strengths.Count / 2 - 1);
			return new List<int>
			{
				Strengths[num * 2],
				Strengths[num * 2 + 1]
			};
		}
	}
}
