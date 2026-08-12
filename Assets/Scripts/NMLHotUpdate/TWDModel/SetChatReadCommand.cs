using BaseModel;

namespace TWDModel
{
	public class SetChatReadCommand : ModelCommand
	{
		public long ReadTime { get; set; }

		public SetChatReadCommand()
		{
		}

		public SetChatReadCommand(PlayerModel player)
			: base(player)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = manager.GetModel<PlayerModel>(base.ModelId).SetChatTime(ReadTime);
			return new NGModelCommandRespond(this, result);
		}
	}
}
