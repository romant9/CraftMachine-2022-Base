using TWDModel;
using UnityEngine;

public class GuildBattleOverviewInfo : MonoBehaviour
{
	public GuildEmblemIcon EmblemIcon;

	public UILabel GuildNameLabel;

	public UILabel TierLevelLabel;

	public UILabel VpAmountLabel;

	public UILabel GasAmountLabel;

	public UILabel BattleKeysLabel;

	public GameObject GuildInfoContainer;

	public void UpdateUI()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (GuildWarHelper.IsGuildMember())
		{
			Helpers.GameObjectSetActive(GuildInfoContainer, value: true);
			GuildTierDefinition guildTierDefinition = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (GameManager.Instance.guildModel != null && GameManager.Instance.guildModel.GvGSeasonModel != null)
			{
				guildTierDefinition = GuildTierHelper.GetCurrentGuildTier();
				_ = guildTierDefinition.Tier;
				num = GameManager.Instance.guildModel.CurrentVictoryPoints;
				num2 = GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.GvGGas);
				num3 = GameManager.Instance.playerModel.GetCurrency(CurrencyType.BattlePass).Max;
				num4 = GameManager.Instance.playerModel.GetCurrency(CurrencyType.BattlePass).Value;
			}
			if (EmblemIcon != null)
			{
				EmblemIcon.UpdateUI(guildTierDefinition);
			}
			if (guildTierDefinition != null)
			{
				HelpersUI.SetContentToLabel(GuildNameLabel, GameManager.Instance.GetFilteredText(GameManager.Instance.guildModel.Name));
				HelpersUI.SetContentToLabel(TierLevelLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(guildTierDefinition.NameLocalizationKey));
				HelpersUI.SetContentToLabel(VpAmountLabel, num.ToString());
				HelpersUI.SetContentToLabel(GasAmountLabel, num2.ToString());
				HelpersUI.SetContentToLabel(BattleKeysLabel, num4 + "/" + num3);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(GuildInfoContainer, value: false);
		}
	}
}
