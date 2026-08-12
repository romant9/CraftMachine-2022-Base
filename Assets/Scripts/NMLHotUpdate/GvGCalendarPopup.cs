using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GvGCalendarPopup : HUDElement
{
	private GuildWarModel guildWar;

	[SerializeField]
	private GvGCalendarWarDayDetails gvgCalendarWarDayDetails;

	[SerializeField]
	private GvGCalendarGrid gvgCalendarGrid;

	private GvGCalendarWarDay selectedCalendarWarDay;

	public override void OpenWithStateData(object data)
	{
		base.Open();
		Init();
		SelectDay((data == null) ? (-1) : ((long)data));
	}

	public override void Open()
	{
		OpenWithStateData(null);
	}

	private void Init()
	{
		guildWar = GuildWarHelper.GetGuildWarModel();
		if (guildWar != null)
		{
			int num = 0;
			foreach (KeyValuePair<long, List<string>> timeSlot in guildWar.RegisteredPlayersForBattleSlot)
			{
				int j = num;
				gvgCalendarGrid.SetButtonsClickCallback(num, delegate
				{
					GuildWarDateSelected(timeSlot.Key, j, gvgCalendarGrid.GetGvGCalendarWarDay(j));
				});
				num++;
			}
		}
		BlackboardUISeenToggle.TryToOpen(UIType.GvGCalendarInfoPopup, "HasSeenGvGCalendarInfo", new UIType[1] { UIType.GuildBattleEndPopup });
	}

	private void SelectDay(long timeSlot)
	{
		if (timeSlot == -1)
		{
			timeSlot = guildWar.RegisteredPlayersForBattleSlot.ElementAt(0).Key;
			long utcTimeStamp = GameManager.Instance.playerModel.UtcTimeStamp;
			if (guildWar.CurrentBattle != null && guildWar.CurrentBattle.IsOngoing(utcTimeStamp))
			{
				timeSlot = guildWar.CurrentBattle.TimeSlot;
			}
			else
			{
				foreach (KeyValuePair<long, List<string>> item in guildWar.RegisteredPlayersForBattleSlot)
				{
					if (item.Key > utcTimeStamp)
					{
						timeSlot = item.Key;
						break;
					}
				}
			}
		}
		int warDayIndexByTimeslot = GuildWarHelper.GetWarDayIndexByTimeslot(timeSlot);
		GuildWarDateSelected(timeSlot, warDayIndexByTimeslot, gvgCalendarGrid.GetGvGCalendarWarDay(warDayIndexByTimeslot));
	}

	private void GuildWarDateSelected(long timeSlot, int index, GvGCalendarWarDay calendarWarDay)
	{
		selectedCalendarWarDay?.SelectGuildWarDay(selected: false);
		selectedCalendarWarDay = calendarWarDay;
		selectedCalendarWarDay?.SelectGuildWarDay(selected: true);
		gvgCalendarWarDayDetails.SetupDayRegisteredPlayers(timeSlot, index + 1);
	}
}
