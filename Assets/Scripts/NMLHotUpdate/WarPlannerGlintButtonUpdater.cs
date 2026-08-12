using TWDModel;
using UnityEngine;

public class WarPlannerGlintButtonUpdater : MonoBehaviour
{
	[SerializeField]
	private GameObject glint;

	private GuildWarModel guildWar;

	private void OnEnable()
	{
		guildWar = GuildWarHelper.GetGuildWarModel();
		UpdateGlint();
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
		if (!(changed != "GuildBattlePlayerRegistered") || !(changed != "GuildBattlePlayerResigned"))
		{
			UpdateGlint();
		}
	}

	private void UpdateGlint()
	{
		if (guildWar != null)
		{
			int allValidRegisteredDaysForPlayer = guildWar.GetAllValidRegisteredDaysForPlayer(GameManager.Instance.playerModel.HashedId, GameManager.Instance.playerModel.UtcTimeStamp);
			glint.SetActive(allValidRegisteredDaysForPlayer == 0);
		}
	}
}
