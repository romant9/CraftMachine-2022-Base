using System.Collections.Generic;
using TWDModel;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class TimeUntilNextGvGPlayerBattle : MonoBehaviour
{
	private UILabel label;

	private GuildWarModel guildWar;

	private const string BattleStartsLocalizationKey = "GvG.Hub.YourBattleStarts{time}";

	private const string BattleEndsLocalizationKey = "GvG.Hub.YourBattleEnds{time}";

	private float refreshRate = 1f;

	private float timer = 1f;

	private void Awake()
	{
		label = GetComponent<UILabel>();
		guildWar = GuildWarHelper.GetGuildWarModel();
	}

	private void OnEnable()
	{
		UpdateTimer();
		if (guildWar != null)
		{
			guildWar.Changed += OnGuildWarModelChangedEventHandler;
		}
	}

	private void OnDisable()
	{
		if (guildWar != null)
		{
			guildWar.Changed -= OnGuildWarModelChangedEventHandler;
		}
	}

	private void OnGuildWarModelChangedEventHandler(TWDGroupModelChild model, string changed, object args)
	{
		if (!(changed != "GuildBattlePlayerRegistered") || !(changed != "GuildBattlePlayerResigned") || !(changed != "GuildBattleStarted") || !(changed != "GuildBattleEnded"))
		{
			UpdateTimer();
		}
	}

	private void Update()
	{
		if (timer < 0f)
		{
			timer += refreshRate;
			UpdateTimer();
		}
		timer -= Time.deltaTime;
	}

	private void UpdateTimer()
	{
		if (guildWar == null)
		{
			return;
		}
		if (GuildWarHelper.IsBattleOngoingAndPlayerRegistered())
		{
			label.text = LocalizationManager.GetText("GvG.Hub.YourBattleEnds{time}", Helpers.FormatTime(GuildWarHelper.GetTimeLeftToCurrentBattleEnd()));
		}
		foreach (KeyValuePair<long, List<string>> item in guildWar.RegisteredPlayersForBattleSlot)
		{
			if (item.Value.Contains(GameManager.Instance.playerModel.HashedId) && item.Key >= GameManager.Instance.playerModel.UtcTimeStamp)
			{
				label.text = LocalizationManager.GetText("GvG.Hub.YourBattleStarts{time}", Helpers.FormatTime(item.Key - GameManager.Instance.playerModel.UtcTimeStamp));
				return;
			}
		}
		label.text = "";
	}
}
