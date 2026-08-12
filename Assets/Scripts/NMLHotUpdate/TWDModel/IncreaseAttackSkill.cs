namespace TWDModel
{
	public sealed class IncreaseAttackSkill : BaseCommandSkill
	{
		public FixedPoint Parameter0 { get; private set; }

		public FixedPoint Parameter1 { get; private set; }

		public int Parameter2 { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillIncreaseAttack;

		public IncreaseAttackSkill()
		{
		}

		public IncreaseAttackSkill(IncreaseAttackSkill increaseAttackSkill)
			: base(increaseAttackSkill)
		{
			Parameter0 = increaseAttackSkill.Parameter0;
			Parameter1 = increaseAttackSkill.Parameter1;
			Parameter2 = increaseAttackSkill.Parameter2;
		}

		public IncreaseAttackSkill(FixedPoint parameter0, FixedPoint parameter1, int parameter2)
		{
			Parameter0 = parameter0;
			Parameter1 = parameter1;
			Parameter2 = parameter2;
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCell);
			if (occupier != null)
			{
				base.manager.ExecuteAction(new SkillIncreaseAttackAction(base.OwnActorModel, occupier, Parameter0, Parameter1, Parameter2));
			}
		}
	}
}
