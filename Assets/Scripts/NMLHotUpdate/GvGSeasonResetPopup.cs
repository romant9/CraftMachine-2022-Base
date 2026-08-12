using TWDModel;
using UnityEngine;

public class GvGSeasonResetPopup : HUDElement
{
	[SerializeField]
	private UISprite lastSeasonEmblemSprite;

	[SerializeField]
	private UILabel lastSeasonTierLabel;

	[SerializeField]
	private UISprite newSeasonEmblemSprite;

	[SerializeField]
	private UILabel newSeasonTierLabel;

	public override void Open()
	{
		base.Open();
		GvGSeasonModel.GvGSeasonStats currentSeasonStats = GameManager.Instance.playerModel.GvGSeasonModel.CurrentSeasonStats;
		if (currentSeasonStats == null)
		{
			Debug.LogError("Null current season stats");
			Close();
			return;
		}
		int currentTier = currentSeasonStats.CurrentTier;
		int lastSeasonTier = currentSeasonStats.LastSeasonTier;
		GuildTierDefinition guildTierDefinition = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(lastSeasonTier);
		GuildTierDefinition guildTierDefinition2 = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(currentTier);
		if (guildTierDefinition == null || guildTierDefinition2 == null)
		{
			Debug.LogError("Null current tier definition");
			Close();
		}
		else
		{
			SetTierVisuals(guildTierDefinition, lastSeasonEmblemSprite, lastSeasonTierLabel);
			SetTierVisuals(guildTierDefinition2, newSeasonEmblemSprite, newSeasonTierLabel);
		}
	}

	private void SetTierVisuals(GuildTierDefinition lastTierDefinition, UISprite uiSprite, UILabel uiLabel)
	{
		uiSprite.spriteName = lastTierDefinition.IconSprite;
		uiLabel.text = LocalizationManager.GetText(lastTierDefinition.NameLocalizationKey);
	}
}
