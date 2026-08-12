using BaseModel;

namespace TWDModel
{
	public class SetPlayerEmblemCommand : ModelCommand
	{
		public PlayerEmblem NewEmblem { get; private set; }

		public SetPlayerEmblemCommand()
		{
		}

		public SetPlayerEmblemCommand(PlayerEmblem newEmblem)
		{
			NewEmblem = newEmblem;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = (manager.GetPlayer() as PlayerModel).SetPlayerEmblem(NewEmblem);
			return new NGModelCommandRespond(this, result);
		}
	}
}
