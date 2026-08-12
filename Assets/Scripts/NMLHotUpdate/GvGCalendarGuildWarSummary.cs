using BaseModel;
using TWDModel;
using UnityEngine;

public class GvGCalendarGuildWarSummary : MonoBehaviour
{
	[SerializeField]
	private UILabel guildEarnings;

	[SerializeField]
	private UILabel playerEarningsVP;

	[SerializeField]
	private UILabel playerEarningsRP;

	[SerializeField]
	private UILabel guildWarParticipants;

	private const string guildEarningsLocalizationString = "GvG.Hub.WarSummary.GuildEarnings{Amount}";

	private const string guildWarParticipantsLocalizationString = "GvG.Hub.WarSummary.ParticipatingMembers{ParticipatedCount}{MembersCount}";

	private void OnEnable()
	{
		GuildWarModelPlayer guildWarPlayer = GuildWarHelper.GetGuildWarPlayer();
		if (guildWarPlayer != null)
		{
			guildWarPlayer.Changed += OnModelChanged;
		}
		UpdateUI();
	}

	private void OnDisable()
	{
		GuildWarModelPlayer guildWarPlayer = GuildWarHelper.GetGuildWarPlayer();
		if (guildWarPlayer != null)
		{
			guildWarPlayer.Changed -= OnModelChanged;
		}
	}

	private void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (changed == "GuildBattleEnded")
		{
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		HelpersUI.SetContentToLabel(guildEarnings, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Hub.WarSummary.GuildEarnings{Amount}", playerModel.GuildModel.GvGSeasonModel.CalculateBattleLogTotalScoreForWar(playerModel.GuildModel.GuildWarModel.WarDefinitionId)));
		HelpersUI.SetContentToLabel(guildWarParticipants, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Hub.WarSummary.ParticipatingMembers{ParticipatedCount}{MembersCount}", playerModel.GuildModel.GvGSeasonModel.GuildWarModel.GetWarAndRegisteredCount(playerModel.UtcTimeStamp), playerModel.gameEconomyData.GuildWarConfig.GuildWarRegistrationLimit));
		HelpersUI.SetContentToLabel(playerEarningsRP, playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GetRPGainedInWar().ToString());
		HelpersUI.SetContentToLabel(playerEarningsVP, playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GetVPGainedInWarForGuild().ToString());
	}
}
