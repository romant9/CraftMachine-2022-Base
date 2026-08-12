using BaseModel;

namespace TWDModel
{
	public class UnEquipmentModSkillCommand : ModelCommand
	{
		public int ModSkillModeID { get; set; }

		public UnEquipmentModSkillCommand()
		{
		}

		public UnEquipmentModSkillCommand(int modSkillModeID)
		{
			ModSkillModeID = modSkillModeID;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ModSkillManager modSkillManager = tWDModelManager.Player.ModSkillManager;
			if (modSkillManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ModSkillMode model = manager.GetModel<ModSkillMode>(ModSkillModeID);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult tWDModelResult = modSkillManager.UnEquipModSkill(model);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
