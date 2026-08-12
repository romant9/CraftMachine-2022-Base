using BaseModel;

namespace TWDModel
{
	public class SkipTurnCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = (manager as TWDModelManager).CombatModel.SkipTurn();
			return new NGModelCommandRespond(this, result);
		}
	}
}
