using System.Collections.Generic;
using TWDModel;

public class RadioCallTableItem
{
	public string DropName;

	public string Description;

	public bool GuarateedHero;

	public bool FeaturedHero;

	public List<ItemAmountProbabilityData> SpecialCallProbabilities;

	public List<ItemAmountProbabilityData> Probabilities;

	public List<ItemAmountProbabilityData> SurvivorRarityAmounts;

	public List<ItemAmountProbabilityData> HeroRarityAmounts;
}
