using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class DropEquipmentsAndSurvivorsRaritiesDefinition
	{
		public DropRewardType RewardType;

		public DropType DropType;

		public int ControlLevelMin;

		public int ControlLevelMax;

		public DropEventDefinition.DropEventTag Tag;

		public DropEventDefinition.DropEventContext DropContext;

		public FixedPoint CommonProbability;

		public FixedPoint UncommonProbability;

		public FixedPoint RareProbability;

		public FixedPoint EpicProbability;

		public FixedPoint LegendaryProbability;

		private List<KeyValuePair<FixedPoint, int>> raritiesProbabilities = new List<KeyValuePair<FixedPoint, int>>();

		[JsonIgnore]
		public FixedPoint SumOfProbabilities => CommonProbability + UncommonProbability + RareProbability + EpicProbability + LegendaryProbability;

		public void PopulateRaritiesProbabilitiesList()
		{
			raritiesProbabilities.Clear();
			FixedPoint fixedPoint = 0L;
			if (CommonProbability > 0L)
			{
				raritiesProbabilities.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + CommonProbability, 0));
				fixedPoint += CommonProbability;
			}
			if (UncommonProbability > 0L)
			{
				raritiesProbabilities.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + UncommonProbability, 1));
				fixedPoint += UncommonProbability;
			}
			if (RareProbability > 0L)
			{
				raritiesProbabilities.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + RareProbability, 2));
				fixedPoint += RareProbability;
			}
			if (EpicProbability > 0L)
			{
				raritiesProbabilities.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + EpicProbability, 3));
				fixedPoint += EpicProbability;
			}
			if (LegendaryProbability > 0L)
			{
				raritiesProbabilities.Add(new KeyValuePair<FixedPoint, int>(fixedPoint + LegendaryProbability, 4));
				fixedPoint += LegendaryProbability;
			}
		}

		public int GetDropRarityForRandomNumber(FixedPoint number)
		{
			if (raritiesProbabilities.Count == 0)
			{
				PopulateRaritiesProbabilitiesList();
			}
			foreach (KeyValuePair<FixedPoint, int> raritiesProbability in raritiesProbabilities)
			{
				if (raritiesProbability.Key >= number)
				{
					return raritiesProbability.Value;
				}
			}
			return 0;
		}
	}
}
