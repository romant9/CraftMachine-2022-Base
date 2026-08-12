using TWDModel;

public class UIGuildBattleProgressBar : UIProgressBarExtended
{
	private void Awake()
	{
		DebugIdString = "UIGuildBattleProgressBar";
	}

	public override void OnEnable()
	{
		base.OnEnable();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GuildWarHelper.GetGuildWarModel() != null)
		{
			GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
			int registeredPlayersCountForBattleTimeSlot = GuildWarHelper.GetRegisteredPlayersCountForBattleTimeSlot();
			int maxPlayerCountInBattle = gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
			HelpersUI.SetContentToLabel(progressBarLabel, $"{registeredPlayersCountForBattleTimeSlot}/{maxPlayerCountInBattle}");
			if (progressBar != null)
			{
				bool flag = registeredPlayersCountForBattleTimeSlot >= gameEconomyData.GuildWarConfig.MinPlayersToStartBattle;
				progressBarLabel.color = (flag ? SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.ValidColor : SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.NotValidColor);
				progressBar.value = (float)registeredPlayersCountForBattleTimeSlot / (float)maxPlayerCountInBattle;
			}
			Helpers.GameObjectSetActive(base.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}

	public override void Clear()
	{
		base.Clear();
	}
}
