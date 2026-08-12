namespace TWDModel
{
	public class SurvivalGameSkill : BaseCommandSkill
	{
		public override CommandSkillType Type => CommandSkillType.CommandSkillSurvivalGame;

		public SurvivalGameSkill()
		{
		}

		public SurvivalGameSkill(SurvivalGameSkill survivalGameSkill)
			: base(survivalGameSkill)
		{
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCell);
			if (occupier != null)
			{
				base.manager.CombatModel.SetNewSurvivalGame(base.OwnActorModel, occupier);
			}
		}
	}
}
