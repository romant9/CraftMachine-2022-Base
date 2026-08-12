public class UIGuildSeasonTimer : UIGuildBattleTimer
{
	protected override bool IsOnGoing()
	{
		return GuildWarHelper.IsSeasonOngoing();
	}

	protected override string GetFormatedTime()
	{
		if (BattleSetting == BattleEnum.Ongoing)
		{
			return GuildWarHelper.GetFormatedTimeLeftToCurrentSeasonEnd();
		}
		if (BattleSetting == BattleEnum.Next)
		{
			return GuildWarHelper.GetFormatedTimeLeftToNextSeason();
		}
		return "";
	}

	protected override void SetContentInLabel()
	{
		if (battleContentLabel != null)
		{
			HelpersUI.SetContentToLabel(battleContentLabel, "");
		}
	}
}
