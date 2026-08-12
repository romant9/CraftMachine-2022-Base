using BaseModel;

namespace TWDModel
{
	public class ExecuteSupportActionCommand : ModelCommand
	{
		public int CombatSupportIndex;

		public GridCoordinate Target;

		public ExecuteSupportActionCommand()
		{
		}

		public ExecuteSupportActionCommand(int combatSupportIndex, GridCoordinate target)
		{
			CombatSupportIndex = combatSupportIndex;
			Target = target;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.CombatModel.SupportManager.TryGetSupport(CombatSupportIndex, out var combatSupportModel) && tWDModelManager.ExecuteAction(new ExecuteCombatSupportAction(combatSupportModel.AttachedSurvivor, CombatSupportIndex, Target)))
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
