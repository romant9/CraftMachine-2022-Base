using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ComponentDropType
	{
		public const string ComponentMetal = "Metal";

		public const string ComponentBadge = "Badge";

		public const string ComponentCloth = "Cloth";

		public const string ComponentChemicals = "Chemicals";

		public const string ComponentFood = "Food";

		private List<KeyValuePair<FixedPoint, string>> componentProbabilities = new List<KeyValuePair<FixedPoint, string>>();

		public int ScavengerLevel;

		public DropEventDefinition.DropEventTag LootTag;

		public FixedPoint Probability;

		public FixedPoint Badge;

		public FixedPoint Metal;

		public FixedPoint Cloth;

		public FixedPoint Chemicals;

		public FixedPoint Food;

		public string EventControl;

		[JsonIgnore]
		public FixedPoint SumOfProbabilities => Metal + Badge + Cloth + Chemicals + Food;

		public void PopulateProbabilities()
		{
			componentProbabilities.Clear();
			FixedPoint fixedPoint = 0.0;
			if (Metal > 0.0)
			{
				componentProbabilities.Add(new KeyValuePair<FixedPoint, string>(fixedPoint + Metal, "Metal"));
				fixedPoint += Metal;
			}
			if (Badge > 0.0)
			{
				componentProbabilities.Add(new KeyValuePair<FixedPoint, string>(fixedPoint + Badge, "Badge"));
				fixedPoint += Badge;
			}
			if (Cloth > 0.0)
			{
				componentProbabilities.Add(new KeyValuePair<FixedPoint, string>(fixedPoint + Cloth, "Cloth"));
				fixedPoint += Cloth;
			}
			if (Chemicals > 0.0)
			{
				componentProbabilities.Add(new KeyValuePair<FixedPoint, string>(fixedPoint + Chemicals, "Chemicals"));
				fixedPoint += Chemicals;
			}
			if (Food > 0.0)
			{
				componentProbabilities.Add(new KeyValuePair<FixedPoint, string>(fixedPoint + Food, "Food"));
				fixedPoint += Food;
			}
		}

		public string GetDropComponentForRandomNumber(FixedPoint number)
		{
			if (componentProbabilities.Count == 0)
			{
				PopulateProbabilities();
			}
			foreach (KeyValuePair<FixedPoint, string> componentProbability in componentProbabilities)
			{
				if (componentProbability.Key >= number)
				{
					return componentProbability.Value;
				}
			}
			return null;
		}
	}
}
