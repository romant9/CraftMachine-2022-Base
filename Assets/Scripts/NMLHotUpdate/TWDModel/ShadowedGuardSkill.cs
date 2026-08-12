namespace TWDModel
{
	public class ShadowedGuardSkill : BaseCommandSkill
	{
		public override CommandSkillType Type => CommandSkillType.CommandSkillShadowedGuard;

		public ShadowedGuardSkill()
		{
		}

		public ShadowedGuardSkill(ShadowedGuardSkill shadowedGuardSkill)
			: base(shadowedGuardSkill)
		{
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCell);
			if (occupier == null || base.OwnActorModel.IsStunned || base.OwnActorModel.IsDead || base.OwnActorModel.IsStruggling)
			{
				return;
			}
			if (base.OwnActorModel is SurvivorModel { IsLeader: not false })
			{
				foreach (ActorModel survivor in base.manager.CombatModel.Survivors)
				{
					if (!survivor.IsDead)
					{
						base.manager.CombatModel.AddShadowedGuard(base.OwnActorModel, survivor);
						base.manager.CombatModel.AddShadowedGuardRefTrait(base.OwnActorModel, survivor);
						survivor.NotifyChange("UpdateShadowedGuardEvent");
					}
				}
			}
			else
			{
				base.manager.CombatModel.AddShadowedGuard(base.OwnActorModel, occupier);
				base.manager.CombatModel.AddShadowedGuardRefTrait(base.OwnActorModel, occupier);
				occupier.NotifyChange("UpdateShadowedGuardEvent");
			}
			base.OwnActorModel.ChargeNum = 0L;
			base.manager.CombatModel.NotifyChange("UpdateShadowedGuardEvent");
			base.OwnActorModel.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffShadowedGuard", false });
		}

		public override void PostExecute(GridCoordinate targetCell)
		{
			base.PostExecute(targetCell);
			foreach (ActorModel survivor in base.manager.CombatModel.Survivors)
			{
				survivor.NotifyChange("ActorHealthChanged");
			}
		}
	}
}
