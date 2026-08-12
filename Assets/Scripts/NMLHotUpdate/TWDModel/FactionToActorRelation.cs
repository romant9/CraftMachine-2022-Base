using BaseModel;

namespace TWDModel
{
	public abstract class FactionToActorRelation : TWDModelObject
	{
		[IgnoreModelProperty]
		public ActorModel TargetActor { get; protected set; }

		public Faction FoundingFaction { get; protected set; }

		public int ExpiryTurn { get; set; }

		public abstract FactionToActorRelationType Type { get; }

		public FactionToActorRelation()
		{
		}

		public FactionToActorRelation(FactionToActorRelation factionToActorRelation)
		{
			TargetActor = factionToActorRelation.TargetActor;
			FoundingFaction = factionToActorRelation.FoundingFaction;
			ExpiryTurn = factionToActorRelation.ExpiryTurn;
		}

		public FactionToActorRelation(ActorModel target, Faction foundingFaction, int expiryTurn)
		{
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
