namespace TWDModel
{
	public class DebuffDamagePerRoundTimedEffect : CoexistTimedEffectAbstract
	{
		public int Param0 { get; private set; }

		public int Param1 { get; private set; }

		public int Param2 { get; private set; }

		public FixedPoint Param3 { get; private set; }

		public override bool TurnCheck => false;

		public DebuffDamagePerRoundTimedEffect()
		{
		}

		public DebuffDamagePerRoundTimedEffect(DebuffDamagePerRoundTimedEffect debuffDamagePerRoundTimedEffect)
			: base(debuffDamagePerRoundTimedEffect)
		{
			Param0 = debuffDamagePerRoundTimedEffect.Param0;
			Param1 = debuffDamagePerRoundTimedEffect.Param1;
			Param2 = debuffDamagePerRoundTimedEffect.Param2;
			Param3 = debuffDamagePerRoundTimedEffect.Param3;
		}

		public DebuffDamagePerRoundTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, int param0, int param1, int param2, FixedPoint param3)
			: base(CoexistTimedEffectType.DebuffDamagePerRound, duration, counter, instigator, target)
		{
			Param0 = param0;
			Param1 = param1;
			Param2 = param2;
			Param3 = param3;
		}

		public override void PostNewTimedEffect()
		{
			base.Target?.NotifyChange("AbilityVisited", new object[2] { "ActorDebuffDamagePerRoundUpdate", false });
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
		}

		public override void OnFactionChanged(Faction currentFaction, Faction newFaction)
		{
			if (!(base.Target is ActorModel { IsDead: false } actorModel) || newFaction != actorModel.Faction)
			{
				return;
			}
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null && turnManager.TurnCount >= Param0 && (turnManager.TurnCount - Param0) % Param1 == 0)
			{
				int num = Param2 + (int)(actorModel.MaxHitPoints * Param3);
				if (!actorModel.OnRedHealthBar && num >= actorModel.Hitpoints)
				{
					actorModel.SetHitPoints(actorModel.MaxHitPoints, actorModel.MaxHitPoints);
					actorModel.OnRedHealthBar = true;
					actorModel.StrugglesLeft--;
					actorModel.EndFortifications(interrupted: true);
				}
				else
				{
					actorModel.DealDamage(num, null, DamageType.DebuffDamagePerRound);
				}
				actorModel.NotifyChange("CommnDamageChanged", num);
				actorModel.NotifyChange("ActorHealthChanged");
				actorModel.NotifyChange("ActorDebuffDamagePerRoundGetDamage");
			}
		}

		public override void PostFinishTimedEffect()
		{
		}
	}
}
