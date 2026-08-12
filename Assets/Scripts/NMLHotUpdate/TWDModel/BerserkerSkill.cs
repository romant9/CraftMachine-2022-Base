namespace TWDModel
{
	public sealed class BerserkerSkill : BaseCommandSkill
	{
		public int Parameter0 { get; private set; }

		public int Parameter1 { get; private set; }

		public int Parameter2 { get; private set; }

		public FixedPoint Parameter3 { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillBerserker;

		public BerserkerSkill()
		{
		}

		public BerserkerSkill(BerserkerSkill skill)
			: base(skill)
		{
			Parameter0 = skill.Parameter0;
			Parameter1 = skill.Parameter1;
			Parameter2 = skill.Parameter2;
			Parameter3 = skill.Parameter3;
		}

		public BerserkerSkill(int parameter0, int parameter1, int parameter2, FixedPoint parameter3)
		{
			Parameter0 = parameter0;
			Parameter1 = parameter1;
			Parameter2 = parameter2;
			Parameter3 = parameter3;
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCell);
			if (occupier != null)
			{
				base.manager.ExecuteAction(new BerserkRageAction(base.OwnActorModel, occupier, Parameter0, Parameter1, Parameter2, Parameter3));
			}
		}
	}
}
