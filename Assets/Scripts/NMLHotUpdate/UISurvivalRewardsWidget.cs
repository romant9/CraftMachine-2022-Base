using UnityEngine;

public class UISurvivalRewardsWidget : ListWidgetBase
{
	[SerializeField]
	private UILabel completionCountLabel;

	[SerializeField]
	private UISprite completionCountSprite;

	[SerializeField]
	private UISprite currencySprite;

	[SerializeField]
	private UILabel currencyLabel;

	[SerializeField]
	private UILabel previousSurvivalLabel;

	private LootEntry internalEntry;

	public LootEntry lootEntry => internalEntry;

	public void UpdateUI(LootEntry entry)
	{
		if (entry != null)
		{
			internalEntry = entry;
			HelpersUI.SetContentToLabel(currencyLabel, internalEntry.RewardedAmount.ToString());
			HelpersUI.SetSprite(currencySprite, HelpersGfx.GetCurrencyIconName(internalEntry.RewardedCurrency));
			bool flag = internalEntry.Control > 0;
			if (flag)
			{
				completionCountLabel.text = internalEntry.Control.ToString();
			}
			Helpers.GameObjectSetActive(completionCountSprite, flag);
			Helpers.GameObjectSetActive(completionCountLabel, flag);
			Helpers.GameObjectSetActive(previousSurvivalLabel, !flag);
			if (previousSurvivalLabel != null)
			{
				previousSurvivalLabel.text = LocalizationManager.GetText("Popup.Survival.Intro.Stats.LastWeek");
			}
		}
	}
}
