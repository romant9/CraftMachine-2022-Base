public class WarPlannerPopUpLocalizationLabel : LocalizationUIUpdaterWithParams
{
	protected override void Awake()
	{
		base.Awake();
		parameters = new string[2]
		{
			GameManager.Instance.gameEconomyData.GuildWarConfig.MinPlayersToStartBattle.ToString(),
			GameManager.Instance.gameEconomyData.GuildWarConfig.BattlePassRefreshAmount.ToString()
		};
	}
}
