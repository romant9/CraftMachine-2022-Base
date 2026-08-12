namespace TWDModel
{
	public sealed class ShieldType1Skill : BaseCommandSkill
	{
		public FixedPoint Parameter0 { get; private set; }

		public int Parameter1 { get; private set; }

		public int Parameter2 { get; private set; }

		public int Parameter3 { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillShieldType1;

		public ShieldType1Skill()
		{
		}

		public ShieldType1Skill(ShieldType1Skill shieldType1Skill)
			: base(shieldType1Skill)
		{
			Parameter0 = shieldType1Skill.Parameter0;
			Parameter1 = shieldType1Skill.Parameter1;
			Parameter2 = shieldType1Skill.Parameter2;
			Parameter3 = shieldType1Skill.Parameter3;
		}

		public ShieldType1Skill(FixedPoint parameter0, int parameter1, int parameter2, int parameter3)
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
				int shield = (int)(((base.OwnActorModel as SurvivorModel)?.GetDamageForPreferredWeapon() ?? 0) * Parameter0) + Parameter1;
				base.manager.ExecuteAction(new SkillShieldType1Action(base.OwnActorModel, occupier, Parameter2, shield));
				base.manager.ExecuteAction(new CommandSkillRemoveNegativeEffectAction(base.OwnActorModel, occupier, base.Definition, Parameter3));
			}
		}
	}
}
