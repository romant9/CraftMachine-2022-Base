using BaseModel;

namespace TWDModel
{
	public class FleeStep2Command : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = (manager as TWDModelManager).CombatModel.FleeStep2();
			return new NGModelCommandRespond(this, result);
		}
	}
}
