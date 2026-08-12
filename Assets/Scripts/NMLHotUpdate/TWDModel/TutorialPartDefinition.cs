using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TutorialPartDefinition
	{
		public string Id;

		public List<TutorialStepDefinition> Steps;

		[JsonIgnore]
		public bool IsForCombat
		{
			get
			{
				for (int i = 0; i < Steps.Count; i++)
				{
					if (Steps[i].IsForCombat)
					{
						return true;
					}
				}
				return false;
			}
		}

		public TutorialPartDefinition()
		{
			Steps = new List<TutorialStepDefinition>();
		}
	}
}
