namespace TWDModel
{
	public class ShieldBreakerTimedEffect : CoexistTimedEffectAbstract
	{
		public ShieldBreakerTimedEffect()
		{
		}

		public ShieldBreakerTimedEffect(ShieldBreakerTimedEffect shieldBreakerTimedEffect)
			: base(shieldBreakerTimedEffect)
		{
		}

		public ShieldBreakerTimedEffect(int duration, int counter, ActorModel instigator, ActorModel target)
			: base(CoexistTimedEffectType.ShieldBreaker, duration, counter, instigator, target)
		{
		}

		public override void PostNewTimedEffect()
		{
			base.Target?.NotifyChange("AbilityVisited", new object[2] { "Equipment_Active_ShieldBreakerStrikeType2", false });
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (newTimedEffect is ShieldBreakerTimedEffect shieldBreakerTimedEffect && base.Target is ActorModel actorModel)
			{
				base.Instigator = shieldBreakerTimedEffect.Instigator;
				base.InstigatorFaction = shieldBreakerTimedEffect.InstigatorFaction;
				base.Counter = shieldBreakerTimedEffect.Counter;
				base.Duration = shieldBreakerTimedEffect.Duration;
				actorModel.NotifyChange("AbilityVisited", new object[2] { "Equipment_Active_ShieldBreakerStrikeType2", false });
			}
		}

		public override void PostFinishTimedEffect()
		{
			base.Target?.NotifyChange("ActorShieldBreakerUpdate");
		}
	}
}
