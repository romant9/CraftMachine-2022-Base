namespace TWDModel
{
	public sealed class AdrenalineSkill : BaseCommandSkill
	{
		public FixedPoint Parameter0 { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillAdrenaline;

		public AdrenalineSkill()
		{
		}

		public AdrenalineSkill(AdrenalineSkill skill)
			: base(skill)
		{
			Parameter0 = skill.Parameter0;
		}

		public AdrenalineSkill(FixedPoint parameter0)
		{
			Parameter0 = parameter0;
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCell);
			if (occupier != null)
			{
				occupier.NotifyChange("Adrenaline");
				if (!occupier.IsStunned && !occupier.IsStruggling && !occupier.IsElectricShocked)
				{
					occupier.ResetActionPointsForExternal();
				}
				occupier.UsedChargeAttackThisTurn = false;
				int amountHealed = (int)((base.OwnActorModel as SurvivorModel).GetDamageForPreferredWeapon() * Parameter0);
				base.manager.ExecuteAction(new HealAction(base.OwnActorModel, occupier, amountHealed));
			}
		}
	}
}
