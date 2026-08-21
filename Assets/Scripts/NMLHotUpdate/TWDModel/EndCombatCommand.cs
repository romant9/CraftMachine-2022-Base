using BaseModel;

namespace TWDModel
{
	public class EndCombatCommand : ModelCommand
	{
		public bool ForceFailure { get; set; }

		public EndCombatCommand()
		{
		}

		public EndCombatCommand(bool forceFailure)
		{
			ForceFailure = forceFailure;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				result = combatModel.EndCombat(ForceFailure);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
