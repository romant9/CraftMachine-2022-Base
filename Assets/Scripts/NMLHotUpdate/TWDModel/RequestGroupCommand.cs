using BaseModel;

namespace TWDModel
{
	public class RequestGroupCommand : ModelCommand
	{
		public string GuildId { get; set; }

		public JsonCommand Command { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.ServerService != null)
			{
				tWDModelManager.ServerService.SendGroupCommand(GuildId, Command);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
