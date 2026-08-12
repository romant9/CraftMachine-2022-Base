using BaseModel;

namespace TWDModel
{
	public class BleedingOutAction : ModelAction
	{
		public ActorModel Target { get; set; }

		public ActorModel Source { get; private set; }

		public bool GiveFullHealth { get; private set; }

		public SupportModel SourceSupport { get; }

		public bool Avoided { get; set; }

		public BleedingOutAction(ActorModel actor, ActorModel target, bool giveFullHealth = true, SupportModel sourceSupport = null)
			: base(actor)
		{
			Source = actor;
			Target = target;
			GiveFullHealth = giveFullHealth;
			SourceSupport = sourceSupport;
		}

		public override bool Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			CombatModel combatModel = tWDModelManager.CombatModel;
			bool result = false;
			if (combatModel != null && Target != null && Target.IsValid())
			{
				if (Avoided)
				{
					return true;
				}
				result = combatModel.BleedOutActor(Source, Target, GiveFullHealth);
				if (result)
				{
					if (GiveFullHealth)
					{
						Target.StrugglesLeft--;
					}
					tWDModelManager.ExecuteAction(new PostStatusEffectAction(Source, Target, TimedEffectType.Bleeding, SourceSupport));
				}
				return result;
			}
			return result;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((Source != null) ? Source.DebugInfo : "null") + ", TargetActor = " + ((Target != null) ? Target.DebugInfo : "null");
		}
	}
}
