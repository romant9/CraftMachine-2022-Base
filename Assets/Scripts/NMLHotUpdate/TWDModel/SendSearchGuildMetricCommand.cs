using BaseModel;

namespace TWDModel
{
	public class SendSearchGuildMetricCommand : ModelCommand
	{
		public GuildSearchInfo Info { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Metrics.AddEndSearchGuild(Info).Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
