public class GvGInfoAttacksLocalizationLabel : LocalizationUIUpdaterWithParams
{
	protected override void Awake()
	{
		base.Awake();
		parameters = new string[1] { GameManager.Instance.gameEconomyData.GuildWarConfig.KeysPerBattle.ToString() };
	}
}
