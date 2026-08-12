using System.Collections.Generic;
using BaseModel;
using TWDModel;

public class LootEntryGenParams
{
	public DropEventDefinition dropEventDefinition;

	public DropEventDefinition.DropEventType eventType;

	public int targetLevel;

	public DropEventDefinition.DropEventTag tag;

	public ModelRandom random;

	public DropType dropType;

	public DropCurrenciesProbabilitiesDefinition.DropCurrency forcedCurrency = DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency;

	public DropEventDefinition.DropEventContext context;

	public bool ignoreCumulativeProbability;

	public Dictionary<DropCurrenciesProbabilitiesDefinition.DropCurrency, DropCurrencyTraitModifier> TraitModifiers;

	public void SetTraitModifier(string traitId, FixedPoint modifier, DropCurrenciesProbabilitiesDefinition.DropCurrency currency)
	{
		if (TraitModifiers == null)
		{
			TraitModifiers = new Dictionary<DropCurrenciesProbabilitiesDefinition.DropCurrency, DropCurrencyTraitModifier>();
		}
		TraitModifiers[currency] = new DropCurrencyTraitModifier
		{
			TraitId = traitId,
			Modifier = modifier
		};
	}

	public DropCurrencyTraitModifier GetTraitModifier(DropCurrenciesProbabilitiesDefinition.DropCurrency currency)
	{
		if (TraitModifiers != null && TraitModifiers.ContainsKey(currency))
		{
			return TraitModifiers[currency];
		}
		return null;
	}
}
