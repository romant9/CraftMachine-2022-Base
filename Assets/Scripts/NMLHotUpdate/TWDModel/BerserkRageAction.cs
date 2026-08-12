using System;
using BaseModel;

namespace TWDModel
{
	public class BerserkRageAction : StatusEffectAction
	{
		public int Turns { get; private set; }

		public int Layer { get; private set; }

		public int BaseRageLayer { get; private set; }

		public FixedPoint AdditionDamageMultiplier { get; private set; }

		public BerserkRageAction(ActorModel sourceActor, ActorModel targetActor, int turns, int layer, int baseRageLayer, FixedPoint additionDamageMultiplier, SupportModel sourceSupport = null, Func<int> damage = null)
			: base(targetActor, targetActor, sourceSupport, damage)
		{
			Turns = turns;
			Layer = layer;
			BaseRageLayer = baseRageLayer;
			AdditionDamageMultiplier = additionDamageMultiplier;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = ((TWDModelManager)manager).CombatModel;
			if (combatModel != null && base.TargetActor != null && base.TargetActor.IsValid() && !base.TargetActor.IsDead)
			{
				base.TargetActor.StartBerserkRage(Turns, base.SourceActor, Layer, BaseRageLayer, AdditionDamageMultiplier);
				return true;
			}
			(manager as TWDModelManager).Debug.LogError("BerserkRage action failed - CombatModel: " + ((combatModel != null) ? "not null" : "NULL") + " Source Actor: " + ((base.SourceActor != null) ? "not null" : "NULL") + " Target Actor: " + ((base.TargetActor != null) ? "not null" : "NULL"));
			return false;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.SourceActor != null) ? base.SourceActor.DebugInfo : "null") + ", TargetActor = " + ((base.TargetActor != null) ? base.TargetActor.DebugInfo : "null") + ", Avoided = " + base.Avoided;
		}
	}
}
