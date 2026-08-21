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

	public PhoneCallDefinition CallDefinition { get; private set; }

	public PhoneCallVisual CallVisual { get; private set; }

	public string HeroUp => CallVisual.HeroUp;

	public List<string> AmountEffect => CallVisual.AmountEffect;

	public RadioCallTableItem(PhoneCallDefinition definition)
	{
		CallDefinition = definition;
		CallVisual = GameManager.Instance.gameEconomyData.GetPhoneCallVisual(definition);
	}
}
