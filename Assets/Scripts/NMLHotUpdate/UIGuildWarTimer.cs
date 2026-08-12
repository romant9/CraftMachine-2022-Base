using TWDModel;
using UnityEngine;

public class UIGuildWarTimer : UIGuildBattleTimer
{
	public enum WarEnum
	{
		Ongoing = 0,
		Next = 1
	}

	public static class UIGuildWarTimerLocalizations
	{
		public static string WarEnds = "GvG.OverviewPopup.TimerWarEnd";

		public static string WarStarts = "GvG.OverviewPopup.NextWar";

		public static string PreparationWeek = "GvG.OverviewPopup.PreparationWeek";
	}

	[Header("War Setting")]
	public WarEnum WarSetting;

	private bool isPreparationWeekOnState;

	protected override bool IsOnGoing()
	{
		if (WarSetting == WarEnum.Ongoing)
		{
			return GuildWarHelper.IsWarOngoing();
		}
		if (WarSetting == WarEnum.Next)
		{
			GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
			if (guildWarModel == null)
			{
				return false;
			}
			return guildWarModel.FindNextGuildWar(GameManager.Instance.playerModel.UtcTimeStamp) != null;
		}
		return false;
	}

	protected override void SetContentInLabel()
	{
		if (battleContentLabel != null)
		{
			if (WarSetting == WarEnum.Ongoing)
			{
				HelpersUI.SetContentToLabel(battleContentLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(UIGuildWarTimerLocalizations.WarEnds));
			}
			else if (WarSetting == WarEnum.Next)
			{
				HelpersUI.SetContentToLabel(battleContentLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(UIGuildWarTimerLocalizations.WarStarts));
			}
		}
	}

	protected override string GetFormatedTime()
	{
		if (WarSetting == WarEnum.Ongoing)
		{
			return GuildWarHelper.GetFormatedTimeLeftToCurrentWarEnd();
		}
		if (WarSetting == WarEnum.Next)
		{
			return GuildWarHelper.GetFormatedTimeLeftToNextWar();
		}
		return "-";
	}
}
