namespace TWDModel
{
	public sealed class QuantunTimedEffect : CoexistTimedEffectAbstract
	{
		public FixedPoint BaseDamagePercentage { get; private set; }

		public FixedPoint AdditionalDamagePercentage { get; private set; }

		public int MaxLayer { get; private set; }

		public FixedPoint CanNotActionPercentage { get; private set; }

		public int CurrentLayer { get; private set; }

		public QuantunTimedEffect()
		{
		}

		public QuantunTimedEffect(QuantunTimedEffect quantunTimedEffect)
			: base(quantunTimedEffect)
		{
			BaseDamagePercentage = quantunTimedEffect.BaseDamagePercentage;
			AdditionalDamagePercentage = quantunTimedEffect.AdditionalDamagePercentage;
			MaxLayer = quantunTimedEffect.MaxLayer;
			CanNotActionPercentage = quantunTimedEffect.CanNotActionPercentage;
			CurrentLayer = quantunTimedEffect.CurrentLayer;
		}

		public QuantunTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target, FixedPoint baseDamagePercentage, FixedPoint additionalDamagePercentage, int maxLayer, FixedPoint canNotActionPercentage)
			: base(CoexistTimedEffectType.Quantun, duration, counter, instigator, target)
		{
			CurrentLayer = 1;
			BaseDamagePercentage = baseDamagePercentage;
			AdditionalDamagePercentage = additionalDamagePercentage;
			MaxLayer = maxLayer;
			CanNotActionPercentage = canNotActionPercentage;
		}

		public override void PostNewTimedEffect()
		{
			base.Target?.NotifyChange("AbilityVisited", new object[2] { "Quantun", false });
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is QuantunTimedEffect quantunTimedEffect)
			{
				base.InstigatorFaction = quantunTimedEffect.InstigatorFaction;
				base.Counter = quantunTimedEffect.Counter;
				base.Duration = quantunTimedEffect.Duration;
				if (CurrentLayer < MaxLayer)
				{
					CurrentLayer++;
				}
			}
		}

		public override void PostFinishTimedEffect()
		{
			base.Target?.NotifyChange("ActorQuantunUpdate");
		}

		public override void OnFactionChanged(Faction currentFaction, Faction newFaction)
		{
			if (!(base.Target is ActorModel { IsDead: false } actorModel))
			{
				return;
			}
			if (newFaction == actorModel.Faction && CurrentLayer > 0)
			{
				int num = 0;
				IChallengeDebuffProvider challengeDebuffProvider = MapMissionDebuffHelper.CanUseDebuffMission(base.manager);
				if (challengeDebuffProvider != null)
				{
					num = (int)ChallengeDebufHelps.GetDebufTotalFirstParam(challengeDebuffProvider.GetChallengeDebuffs(), ChallengeDebuffType.DebuffQuantunDmgReduction);
				}
				FixedPoint fixedPoint = BaseDamagePercentage + (CurrentLayer - 1) * AdditionalDamagePercentage - num / 100;
				fixedPoint = ((fixedPoint > 0L) ? fixedPoint : ((FixedPoint)0L));
				FixedPoint fixedPoint2 = actorModel.MaxHitPoints * fixedPoint;
				CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, actorModel, (int)fixedPoint2, 0, DamageType.Qunantun, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
			}
			if (newFaction == actorModel.Faction && base.manager.Player.RollDice(RollDiceType.QuantunCanNotMove, CanNotActionPercentage, 0L) != PlayerRandomChanceResult.Failed)
			{
				base.Target?.NotifyChange("AbilityVisited", new object[2] { "Quantun", false });
				base.manager.ExecuteAction(new QuantunCanNotMoveAction(null, actorModel, 1));
			}
		}
	}
}
