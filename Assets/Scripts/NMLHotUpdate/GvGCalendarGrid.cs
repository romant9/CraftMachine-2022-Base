using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GvGCalendarGrid : MonoBehaviour
{
	[SerializeField]
	private GvGCalendarWarDay[] gvgCalendarWarDays;

	private GuildWarModel guildWar;

	private void OnEnable()
	{
		guildWar = GuildWarHelper.GetGuildWarModel();
		if (guildWar != null)
		{
			guildWar.Changed -= OnGuildWarModelChanged;
			guildWar.Changed += OnGuildWarModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
		UpdateUI();
	}

	private void OnDisable()
	{
		if (guildWar != null)
		{
			guildWar.Changed -= OnGuildWarModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.GuildBattleLockdownTimeEvent)
		{
			UpdateUI();
		}
	}

	private void OnGuildWarModelChanged(TWDGroupModelChild model, string changed, object args)
	{
		switch (changed)
		{
		case "GuildBattlePlayerRegistered":
		case "GuildBattlePlayerResigned":
		case "GuildBattleStarted":
		case "GuildBattleEnded":
			UpdateUI();
			break;
		}
	}

	private void UpdateUI()
	{
		if (guildWar == null)
		{
			return;
		}
		int num = 0;
		foreach (KeyValuePair<long, List<string>> item in guildWar.RegisteredPlayersForBattleSlot)
		{
			gvgCalendarWarDays[num].FillWarDay(item);
			num++;
		}
	}

	public void SetButtonsClickCallback(int index, UIButtonExtended.OnClickCallback callback)
	{
		gvgCalendarWarDays[index].button.Clear();
		gvgCalendarWarDays[index].button.SetClickCallback(callback);
	}

	public GvGCalendarWarDay GetGvGCalendarWarDay(int index)
	{
		GvGCalendarWarDay[] array = gvgCalendarWarDays;
		if (array == null || array.Length <= index)
		{
			return null;
		}
		return gvgCalendarWarDays[index];
	}
}
