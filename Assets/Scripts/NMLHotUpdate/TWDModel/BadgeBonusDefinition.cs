using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class BadgeBonusDefinition : TypeIndexDefinition
	{
		public string ConditionClassName;

		public List<string> ConstructionParameters;
	}
}
