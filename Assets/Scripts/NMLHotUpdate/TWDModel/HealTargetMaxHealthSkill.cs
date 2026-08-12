namespace TWDModel
{
	public sealed class HealTargetMaxHealthSkill : BaseCommandSkill
	{
		public FixedPoint Parameter0 { get; private set; }

		public int Parameter1 { get; private set; }

		public int Parameter2 { get; private set; }

		public FixedPoint Parameter3 { get; private set; }

		public int Parameter4 { get; private set; }

		public int CombatUsedTimes { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillHealTargetMaxHealth;

		public HealTargetMaxHealthSkill()
		{
		}

		public HealTargetMaxHealthSkill(HealTargetMaxHealthSkill skill)
			: base(skill)
		{
			Parameter0 = skill.Parameter0;
			Parameter1 = skill.Parameter1;
			Parameter2 = skill.Parameter2;
			Parameter3 = skill.Parameter3;
			Parameter4 = skill.Parameter4;
			CombatUsedTimes = skill.CombatUsedTimes;
		}

		public HealTargetMaxHealthSkill(FixedPoint parameter0, int parameter1, int parameter2, FixedPoint parameter3, int parameter4)
		{
			Parameter0 = parameter0;
			Parameter1 = parameter1;
			Parameter2 = parameter2;
			Parameter3 = parameter3;
			Parameter4 = parameter4;
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCell);
			if (occupier != null)
			{
				occupier.NotifyChange("AbilityVisited", new object[2] { "Heal", false });
				bool flag = false;
				if (occupier.IsStruggling && CombatUsedTimes < Parameter4)
				{
					occupier.ExclusiveTimedEffect?.Instigator?.FinishTimedEffect(interrupted: true);
					HealDamageUpHP(occupier);
					flag = true;
					CombatUsedTimes++;
				}
				int amountHealed = (int)(occupier.MaxHitPoints * Parameter0) + Parameter1;
				base.manager.ExecuteAction(new HealAction(base.OwnActorModel, occupier, amountHealed));
				base.manager.ExecuteAction(new CommandSkillRemoveNegativeEffectAction(base.OwnActorModel, occupier, base.Definition, Parameter2));
				if (flag)
				{
					occupier.NotifyChange("ActorHealthChanged", "HealRedHealth");
				}
			}
		}

		public void HealDamageUpHP(ActorModel actorModel)
		{
			actorModel.OnRedHealthBar = false;
			actorModel.StrugglesLeft = 1;
			FixedPoint fixedPoint = actorModel.MaxHitPoints * Parameter3;
			actorModel.HealUpHitpoint((int)fixedPoint);
		}
	}
}
