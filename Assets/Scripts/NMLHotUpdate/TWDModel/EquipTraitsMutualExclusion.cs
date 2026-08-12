using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class EquipTraitsMutualExclusion
	{
		public string Traits;

		public List<string> MutualExclusionTraits;
	}
}
