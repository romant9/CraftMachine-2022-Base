using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class BadgeRecipe
	{
		public string Component1;

		public string Component2;

		public string Component3;

		public string Component4;

		public string Results;

		public string GetRandomEffect(ModelRandom random)
		{
			List<string> list = Results.Split(',').ToList();
			return random.GetRandomElement(list, remove: false);
		}

		public bool CanBeBuiltWith(List<CurrencyType> selectedComponents)
		{
			List<string> list = new List<string> { Component1, Component2, Component3, Component4 };
			for (int i = 1; i < selectedComponents.Count; i++)
			{
				if (!list.Remove(ComponentHelper.GetComponentBaseCurrency(selectedComponents[i]).ToString()))
				{
					return false;
				}
			}
			return list.Count == 0;
		}
	}
}
