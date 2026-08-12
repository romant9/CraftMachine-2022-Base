using System;

namespace TWDModel
{
	[Serializable]
	public class SPTraitWithWeight
	{
		public SPTraitsRemoldDefinitions TraitDef { get; set; }

		public int Weight { get; set; }

		public SPTraitWithWeight(SPTraitsRemoldDefinitions traitDef, int weight)
		{
			TraitDef = traitDef;
			Weight = weight;
		}
	}
}
