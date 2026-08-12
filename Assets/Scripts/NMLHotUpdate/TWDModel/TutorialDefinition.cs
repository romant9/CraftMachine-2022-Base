using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class TutorialDefinition
	{
		public List<TutorialPartDefinition> Parts;

		public TutorialDefinition()
		{
			Parts = new List<TutorialPartDefinition>();
		}
	}
}
