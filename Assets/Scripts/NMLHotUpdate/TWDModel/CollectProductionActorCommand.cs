using BaseModel;

namespace TWDModel
{
	public class CollectProductionActorCommand : ModelCommand
	{
		public CollectProductionActorCommand()
		{
		}

		public CollectProductionActorCommand(ActorModel actor)
			: base(actor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = manager.GetModel<ActorModel>(base.ModelId).CollectProduction();
			return new NGModelCommandRespond(this, result);
		}
	}
}
