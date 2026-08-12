using TWDModel;
using UnityEngine;

public class UIApocalypticChallengeRewardsWidget : ListWidgetBase
{
	[SerializeField]
	private UISprite currencySprite;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UILabel currencyLabel;

	private LootEntry internalEntry;

	public LootEntry lootEntry => internalEntry;

	public void UpdateUI(LootEntry entry)
	{
		if (entry != null)
		{
			internalEntry = entry;
			PlayerModel playerModel = GameManager.Instance.playerModel;
			HelpersUI.SetContentToLabel(currencyLabel, internalEntry.RewardedAmount.ToString());
			if (internalEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Consumable)
			{
				consumableTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(internalEntry.GeneratedEquipment ?? internalEntry.RewardedEquipment);
				Helpers.GameObjectSetActive(currencySprite, value: false);
				Helpers.GameObjectSetActive(consumableTexture, value: true);
			}
			else if (internalEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon || internalEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor)
			{
				consumableTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(internalEntry.RewardedEquipment);
				HelpersUI.SetContentToLabel(currencyLabel, "1");
				Helpers.GameObjectSetActive(currencySprite, value: false);
				Helpers.GameObjectSetActive(consumableTexture, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(consumableTexture, value: false);
				HelpersUI.SetSprite(currencySprite, HelpersGfx.GetCurrencyIconName(internalEntry.RewardedCurrency, playerModel));
			}
		}
	}
}
