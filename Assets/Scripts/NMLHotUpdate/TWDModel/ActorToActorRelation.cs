using BaseModel;

namespace TWDModel
{
	public abstract class ActorToActorRelation : TWDModelObject
	{
		[IgnoreModelProperty]
		public ActorModel SourceActor { get; protected set; }

		[IgnoreModelProperty]
		public ActorModel TargetActor { get; protected set; }

		public Faction FoundingFaction { get; protected set; }

		public int ExpiryTurn { get; set; }

		public abstract RelationType Type { get; }

		public ActorToActorRelation()
		{
		}

		public ActorToActorRelation(ActorToActorRelation actorToActorRelation)
		{
			SourceActor = actorToActorRelation.SourceActor;
			TargetActor = actorToActorRelation.TargetActor;
			FoundingFaction = actorToActorRelation.FoundingFaction;
			ExpiryTurn = actorToActorRelation.ExpiryTurn;
		}

		public ActorToActorRelation(ActorModel source, ActorModel target, Faction foundingFaction, int expiryTurn)
		{
			SourceActor = source;
			TargetActor = target;
			FoundingFaction = foundingFaction;
			ExpiryTurn = expiryTurn;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
