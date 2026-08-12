using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class AbilityModifierDefinition
	{
		public string Type;

		public List<string> ConstructionParameters;

		public AbilityModifierDefinition()
		{
		}

		public AbilityModifierDefinition(string type, List<string> parameters)
		{
			Type = type;
			ConstructionParameters = new List<string>();
			foreach (string parameter in parameters)
			{
				ConstructionParameters.Add(parameter);
			}
		}
	}
}
