using BaseModel;

namespace TWDModel
{
	public class SendSkipOutpostMatchMetricCommand : ModelCommand
	{
		public MatchInfo MatchInfo { get; set; }

		public SendSkipOutpostMatchMetricCommand()
		{
		}

		public SendSkipOutpostMatchMetricCommand(MatchInfo matchInfo)
		{
			MatchInfo = matchInfo;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Metrics.AddSkip().AddPvp().AddPvpAttacker()
				.AddPvpDefender(MatchInfo)
				.Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
