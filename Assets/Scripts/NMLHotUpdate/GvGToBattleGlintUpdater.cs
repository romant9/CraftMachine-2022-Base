using TWDModel;
using UnityEngine;

public class GvGToBattleGlintUpdater : MonoBehaviour
{
	[SerializeField]
	private GameObject glint;

	private GuildWarModel guildWar;

	private void OnEnable()
	{
		UpdateGlint();
		guildWar = GuildWarHelper.GetGuildWarModel();
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
		if (!(changed != "GuildBattleStarted") || !(changed != "GuildBattleEnded"))
		{
			UpdateGlint();
		}
	}

	private void UpdateGlint()
	{
		glint.SetActive(GuildWarHelper.IsBattleOngoingAndPlayerRegistered());
	}
}
