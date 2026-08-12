using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TutorialStepDefinition
	{
		public int Id;

		public bool ShowMapButton;

		public bool AllowCampSelect;

		public List<string> Actions;

		[JsonIgnore]
		public bool IsForCombat
		{
			get
			{
				foreach (string action in Actions)
				{
					if (action.ToLower().StartsWith("combat") || action.ToLower().StartsWith("video"))
					{
						return true;
					}
				}
				return false;
			}
		}

		public TutorialStepDefinition()
		{
			Actions = new List<string>();
		}
	}
}
