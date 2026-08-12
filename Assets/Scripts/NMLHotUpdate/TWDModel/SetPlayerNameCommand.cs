using BaseModel;

namespace TWDModel
{
	public class SetPlayerNameCommand : ModelCommand
	{
		public string Name { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = (manager.GetPlayer() as PlayerModel).SetName(Name);
			return new NGModelCommandRespond(this, result);
		}
	}
}
