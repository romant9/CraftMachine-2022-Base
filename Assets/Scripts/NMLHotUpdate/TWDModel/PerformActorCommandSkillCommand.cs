using BaseModel;

namespace TWDModel
{
	public class PerformActorCommandSkillCommand : ModelCommand
	{
		public GridCoordinate TargetCell { get; private set; }

		public PerformActorCommandSkillCommand()
		{
		}

		public PerformActorCommandSkillCommand(int modelId, GridCoordinate targetCell)
			: base(modelId)
		{
			TargetCell = targetCell;
		}

		public static bool PerformActorCommandSkill(TWDModelManager manager, BaseCommandSkill commandSkill, GridCoordinate targetCell)
		{
			if (manager?.CombatModel == null || commandSkill == null)
			{
				return false;
			}
			return commandSkill.ReleaseSkillToTargetCell(targetCell);
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			BaseCommandSkill baseCommandSkill = manager.GetModel<ActorModel>(base.ModelId)?.CommandSkillModelManager?.ActorCommandSkill;
			if (baseCommandSkill == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			bool flag = PerformActorCommandSkill(manager as TWDModelManager, baseCommandSkill, TargetCell);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
