namespace TWDModel
{
	public sealed class PoisonRelation : ActorToActorRelation
	{
		public FixedPoint AttackerDamagePercentage { get; private set; }

		public int MaxLayerCount { get; private set; }

		public int CurrentLayerCount { get; private set; }

		public int LeftTurns { get; set; }

		public override RelationType Type => RelationType.Poison;

		public PoisonRelation()
		{
		}

		public PoisonRelation(PoisonRelation poisonRelation)
			: base(poisonRelation)
		{
			AttackerDamagePercentage = poisonRelation.AttackerDamagePercentage;
			MaxLayerCount = poisonRelation.MaxLayerCount;
			CurrentLayerCount = poisonRelation.CurrentLayerCount;
			LeftTurns = poisonRelation.LeftTurns;
		}

		public PoisonRelation(ActorModel source, ActorModel target, Faction foundingFaction, int expiryTurn, FixedPoint attackerDamagePercentage, int maxLayerCount, int leftTurns)
			: base(source, target, foundingFaction, expiryTurn)
		{
			AttackerDamagePercentage = attackerDamagePercentage;
			MaxLayerCount = maxLayerCount;
			CurrentLayerCount = 1;
			LeftTurns = leftTurns;
		}

		public PoisonRelation(ActorModel source, ActorModel target, Faction foundingFaction, int expiryTurn, FixedPoint attackerDamagePercentage, int maxLayerCount, int currentLayerCount, int leftTurns)
			: base(source, target, foundingFaction, expiryTurn)
		{
			AttackerDamagePercentage = attackerDamagePercentage;
			MaxLayerCount = maxLayerCount;
			CurrentLayerCount = currentLayerCount;
			LeftTurns = leftTurns;
		}

		public void AddLayerCount()
		{
			if (CurrentLayerCount < MaxLayerCount)
			{
				CurrentLayerCount++;
			}
		}

		public void SetCurrentLayerCount(int newCurrentLayerCount)
		{
			CurrentLayerCount = newCurrentLayerCount;
		}

		public void SubtractLeftTurns()
		{
			LeftTurns--;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
