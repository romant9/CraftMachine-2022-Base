using BaseModel;

namespace TWDModel
{
	public class PerformCommandSkillCommand : ModelCommand
	{
		public GridCoordinate TargetCell { get; private set; }

		public PerformCommandSkillCommand()
		{
		}

		public PerformCommandSkillCommand(int modelId, GridCoordinate targetCell)
			: base(modelId)
		{
			TargetCell = targetCell;
		}

		public static bool PerformCommandSkill(TWDModelManager manager, BaseCommandSkill commandSkill, GridCoordinate targetCell)
		{
			if (manager?.CombatModel == null || commandSkill == null)
			{
				return false;
			}
			return commandSkill.ReleaseSkillToTargetCell(targetCell);
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			BaseCommandSkill model = manager.GetModel<BaseCommandSkill>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			bool flag = PerformCommandSkill(manager as TWDModelManager, model, TargetCell);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
