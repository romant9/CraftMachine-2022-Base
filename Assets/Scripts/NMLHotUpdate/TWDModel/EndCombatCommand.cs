using BaseModel;

namespace TWDModel
{
	public class EndCombatCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				result = combatModel.EndCombat();
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
