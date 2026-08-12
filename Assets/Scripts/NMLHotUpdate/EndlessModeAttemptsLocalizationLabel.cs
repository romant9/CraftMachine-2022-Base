public class EndlessModeAttemptsLocalizationLabel : LocalizationUIUpdaterWithParams
{
	protected override void Awake()
	{
		base.Awake();
		parameters = new string[1] { GameManager.Instance.gameEconomyData.EndlessModeConfig.AttemptsToSumForFinalScoreNormal.ToString() };
	}
}
