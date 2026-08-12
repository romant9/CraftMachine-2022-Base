using System;
using BaseModel;

namespace TWDModel
{
	public class QuantunAction : StatusEffectAction
	{
		public FixedPoint BaseDamagePercentage { get; private set; }

		public FixedPoint AdditionalDamagePercentage { get; private set; }

		public int MaxLayer { get; private set; }

		public FixedPoint CanNotActionPercentage { get; private set; }

		public int Turns { get; private set; }

		public bool IgnoreSourceBeingDead { get; private set; }

		public QuantunAction(ActorModel sourceActor, ActorModel targetActor, int turns, FixedPoint baseDamagePercentage, FixedPoint additionalDamagePercentage, int maxLayer, FixedPoint canNotActionPercentage, bool ignoreSourceBeingDead = false, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(sourceActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			BaseDamagePercentage = baseDamagePercentage;
			AdditionalDamagePercentage = additionalDamagePercentage;
			MaxLayer = maxLayer;
			CanNotActionPercentage = canNotActionPercentage;
			base.Avoided = false;
			IgnoreSourceBeingDead = ignoreSourceBeingDead;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = ((TWDModelManager)manager).CombatModel;
			if (combatModel != null && base.SourceActor != null && base.SourceActor.IsValid() && base.TargetActor != null && base.TargetActor.IsValid())
			{
				if (!base.Avoided && !base.TargetActor.IsDead && (IgnoreSourceBeingDead || !base.SourceActor.IsDead))
				{
					base.TargetActor.StartQuantun(Turns, base.SourceActor, BaseDamagePercentage, AdditionalDamagePercentage, MaxLayer, CanNotActionPercentage);
				}
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("Quantun action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided + ", Turns = " + Turns;
		}
	}
}
