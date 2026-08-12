using BaseModel;
using TWDModel;

public class GuildBattleOverviewPopup : HUDElement
{
	public GuildBattleOverviewInfo OverviewInfo;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (OverviewInfo != null)
		{
			OverviewInfo.UpdateUI();
		}
	}

	private void OnEnable()
	{
		if (!(GameManager.Instance == null))
		{
			GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed += OnGuildBattlePlayerChange;
			GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
			if (guildWarModel != null && guildWarModel.CurrentBattle != null)
			{
				guildWarModel.CurrentBattle.Changed += OnGuildBattleModelChange;
			}
			UpdateUI();
		}
	}

	private void OnDisable()
	{
		if (!(GameManager.Instance == null))
		{
			GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
			GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
			if (guildWarModel != null && guildWarModel.CurrentBattle != null)
			{
				guildWarModel.CurrentBattle.Changed -= OnGuildBattleModelChange;
			}
		}
	}

	private void OnGuildBattlePlayerChange(ModelObject m, string changed, object arg)
	{
		UpdateUI();
	}

	private void OnGuildBattleModelChange(TWDGroupModelChild modelObject, string changed, object args)
	{
		UpdateUI();
	}

	public static bool CanShowSeasonPopup()
	{
		bool num = GuildWarHelper.IsSeasonOngoing();
		bool flag = GuildWarHelper.IsWarOngoing();
		bool flag2 = GuildWarHelper.GetTimeLeftToNextWar() <= 0;
		bool flag3 = GuildWarHelper.HasSeenGvGSeasonStart();
		bool flag4 = GameManager.Instance.playerModel.GvGSeasonModelPlayer.HasGvGSeasonStarted();
		bool flag5 = GuildWarHelper.IsLockedByCouncilLevelOrTutorial();
		if (num && !flag && !flag3 && flag4 && !flag2)
		{
			return !flag5;
		}
		return false;
	}
}
