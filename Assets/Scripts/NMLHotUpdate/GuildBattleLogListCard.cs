using Client.Connectivity;
using System;
using System.Xml;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class GuildBattleLogListCard : NUIListItem<GvGSeasonModel.GuildBattleLogEntry>
{
	[Header("Generic")]
	[SerializeField]
	private UISprite resultHighlightSprite;

	[SerializeField]
	private Color victoryColor;

	[SerializeField]
	private Color defeatColor;

	[SerializeField]
	private Color drawColor;

	[SerializeField]
	private UILabel timerLabel;

	[Header("Ended Battle")]
	[SerializeField]
	private UILabel enemyGuildName;

	[SerializeField]
	private UILabel resultLabel;

	[SerializeField]
	private UILabel victoryPointsAmountLabel;

	public override void UpdateUI()
	{
		GvGSeasonModel.GuildBattleLogEntry data = GetData();
		HelpersUI.SetContentToLabel(enemyGuildName, GameManager.Instance.GetFilteredText(data.OpponentGuildName));
		if (data.IsVictory)
		{
			resultHighlightSprite.color = victoryColor;
			HelpersUI.SetContentToLabel(resultLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GuildBattleLog.Title.Victory"));
		}
		else if (data.IsDraw)
		{
			resultHighlightSprite.color = drawColor;
			HelpersUI.SetContentToLabel(resultLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Combat.Button.DrawOutpost"));
		}
		else
		{
			resultHighlightSprite.color = defeatColor;
			HelpersUI.SetContentToLabel(resultLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GuildBattleLog.Title.Defeat"));
		}
		long milliSeconds = GameManager.Instance.playerModel.UtcTimeStamp - data.EndedTimeStamp;
		HelpersUI.SetContentToLabel(timerLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Time.PostedAgo{TimeAgo}", Helpers.FormatTime(milliSeconds)));
		HelpersUI.SetContentToLabel(victoryPointsAmountLabel, data.VictoryPoints.ToString());
	}



	#region mycode
	public void OpenInviteGuild()
	{
		GvGSeasonModel.GuildBattleLogEntry data = GetData();
		if (data == null)
		{
			return;
		}

		var guildIDList = data.BattleId.Split('_');
		var guildID = guildIDList[1] == GameManager.Instance.playerModel.GuildId ? guildIDList[2] : guildIDList[1];

		MyTools.CopyToClipboard(data.OpponentGuildName);

		GuildInfoPopup.OpenForGuildId(guildID);
	}

	#endregion
}
