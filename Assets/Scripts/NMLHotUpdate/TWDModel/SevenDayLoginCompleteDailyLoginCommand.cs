using BaseModel;

namespace TWDModel
{
	public class SevenDayLoginCompleteDailyLoginCommand : ModelCommand
	{
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
			tWDModelManager.Player.DailyLoginCalendar.DebugSetCompleted();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
