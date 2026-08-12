using BaseModel;

namespace TWDModel
{
	public class CampDefenseKillWalkerCommand : ModelCommand
	{
		public CampDefenseKillWalkerCommand()
		{
		}

		public CampDefenseKillWalkerCommand(ActorModel actor)
			: base(actor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (model != null)
			{
				(manager as TWDModelManager).Player.Camp.CampDefenseModel.KillWalker(model);
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
		}
	}
}
