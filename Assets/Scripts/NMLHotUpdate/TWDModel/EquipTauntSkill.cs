namespace TWDModel
{
	public sealed class EquipTauntSkill : BaseCommandSkill
	{
		public FixedPoint Parameter0 { get; private set; }

		public int Parameter1 { get; private set; }

		public int Parameter2 { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillEquipTaunt;

		public EquipTauntSkill()
		{
		}

		public EquipTauntSkill(EquipTauntSkill skill)
			: base(skill)
		{
			Parameter0 = skill.Parameter0;
			Parameter1 = skill.Parameter1;
			Parameter2 = skill.Parameter2;
		}

		public EquipTauntSkill(FixedPoint parameter0, int parameter1, int parameter2)
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
				occupier.NotifyChange("AbilityVisited", new object[2] { "CommandSkillEquipTauntSkill", false });
				int shield = (int)(occupier.MaxHitPoints * Parameter0);
				base.manager.ExecuteAction(new CommandSkillEquipTauntAction(occupier, Parameter2, Parameter1, shield));
			}
		}
	}
}
