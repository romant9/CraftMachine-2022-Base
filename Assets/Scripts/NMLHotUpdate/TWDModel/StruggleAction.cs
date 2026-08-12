using BaseModel;

namespace TWDModel
{
	public class StruggleAction : ModelAction
	{
		public ActorModel Target { get; set; }

		public ActorModel Source { get; private set; }

		public bool Avoided { get; set; }

		public StruggleAction(ActorModel actor, ActorModel target)
			: base(actor)
		{
			Source = actor;
			Target = target;
		}

		public override bool Execute(ModelManager manager)
		{
			if (Avoided)
			{
				return true;
			}
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			bool result = false;
			if (combatModel != null && Source != null)
			{
				result = combatModel.StruggleActor(Source, Target);
				if (result)
				{
					Target.StrugglesLeft--;
					if (Target.Faction == Faction.Survivor)
					{
						combatModel.MissionStatistics.AddStruggleCount();
					}
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
