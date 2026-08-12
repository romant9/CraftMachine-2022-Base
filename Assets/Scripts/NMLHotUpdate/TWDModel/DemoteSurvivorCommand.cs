using BaseModel;

namespace TWDModel
{
	public class DemoteSurvivorCommand : ModelCommand
	{
		public DemoteSurvivorCommand()
		{
		}

		public DemoteSurvivorCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			TWDModelResult result = (manager.GetPlayer() as PlayerModel).SurvivorContainer.DemoteSurvivor(model);
			return new NGModelCommandRespond(this, result);
		}
	}
}
