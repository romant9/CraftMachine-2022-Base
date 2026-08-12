using BaseModel;

namespace TWDModel
{
	public class PostStatusEffectAction : ModelActorAction
	{
		public TimedEffectType Type { get; }

		public ActorModel SourceActor { get; }

		public int Turns { get; }

		public SupportModel SourceSupport { get; }

		public string CausedByTrait { get; }

		public PostStatusEffectAction(ActorModel sourceActor, ActorModel target, TimedEffectType type, SupportModel sourceSupport, int turns = -1, string causedByTrait = null)
			: base(target)
		{
			SourceActor = sourceActor;
			Type = type;
			Turns = turns;
			SourceSupport = sourceSupport;
			CausedByTrait = causedByTrait;
		}

		public override bool Execute(ModelManager manager)
		{
			return true;
		}
	}
}
