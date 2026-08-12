using BaseModel;

namespace TWDModel
{
	public class SevenDayLoginRemedyCommand : ConsumeCurrencyCommand
	{
		public int Day { get; set; }

		public SevenDayLoginRemedyCommand(int day)
		{
			Day = day;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player.SevenDayLoginManager == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			SevenDayLoginPeriodModel currentPeriodModel = tWDModelManager.Player.SevenDayLoginManager.CurrentPeriodModel;
			if (currentPeriodModel == null)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (tWDModelManager.Player.SevenDayLoginManager.CurrentPeriodId != currentPeriodModel.PeriodId)
			{
				return new NGModelCommandRespond(this, result);
			}
			if (currentPeriodModel.TryRemedy(Day))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
