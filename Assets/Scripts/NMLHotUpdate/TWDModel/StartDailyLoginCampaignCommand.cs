using BaseModel;

namespace TWDModel
{
	public class StartDailyLoginCampaignCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager)?.Player?.DailyLoginCalendar?.InitializeCampaign();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
