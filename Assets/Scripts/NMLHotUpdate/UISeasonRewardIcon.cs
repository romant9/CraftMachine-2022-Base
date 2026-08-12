using TWDModel;

public class UISeasonRewardIcon : MonoBehaviourExtended
{
	public UISprite RewardIcon;

	public UILabel LabelOne;

	public UILabel LabelTwo;

	public void UpdateUI(SeasonDefinition seasonDefinition)
	{
		CurrencyType currencyType = seasonDefinition?.RewardCurrency ?? CurrencyType.None;
		if (currencyType == CurrencyType.None)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		long firstSeasonMissionUnlockTime = GameManager.Instance.gameEconomyData.GetFirstSeasonMissionUnlockTime(seasonDefinition);
		if (firstSeasonMissionUnlockTime != -1 && GameManager.Instance.playerModel.UtcTimeStamp < firstSeasonMissionUnlockTime)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
			return;
		}
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		HelpersUI.SetSprite(RewardIcon, HelpersGfx.GetCurrencyIconName(currencyType));
		HelpersUI.SetContentToLabel(LabelOne, LocalizationManager.GetText(seasonDefinition.RewardLocalisationTitle), !string.IsNullOrEmpty(seasonDefinition.RewardLocalisationTitle));
		HelpersUI.SetContentToLabel(LabelTwo, LocalizationManager.GetText(seasonDefinition.RewardLocalisationDesc), !string.IsNullOrEmpty(seasonDefinition.RewardLocalisationDesc));
	}
}
