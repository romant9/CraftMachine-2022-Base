namespace TWDModel
{
	public class AstheniaRelation : ActorToActorRelation
	{
		public int LeftTurns { get; set; }

		public FixedPoint MakeEnemyDecreaseAttackPercentage { get; set; }

		public FixedPoint MakeEnemyDecreaseDecreaseDamagePercentage { get; set; }

		public override RelationType Type => RelationType.Asthenia;

		public AstheniaRelation()
		{
		}

		public AstheniaRelation(AstheniaRelation astheniaRelation)
			: base(astheniaRelation)
		{
			LeftTurns = astheniaRelation.LeftTurns;
			MakeEnemyDecreaseAttackPercentage = astheniaRelation.MakeEnemyDecreaseAttackPercentage;
			MakeEnemyDecreaseDecreaseDamagePercentage = astheniaRelation.MakeEnemyDecreaseDecreaseDamagePercentage;
		}

		public AstheniaRelation(ActorModel source, ActorModel target, Faction foundingFaction, int expiryTurn, int leftTurns, FixedPoint makeEnemyDecreaseAttackPercentage, FixedPoint makeEnemyDecreaseDecreaseDamagePercentage)
			: base(source, target, foundingFaction, expiryTurn)
		{
			LeftTurns = leftTurns;
			MakeEnemyDecreaseAttackPercentage = makeEnemyDecreaseAttackPercentage;
			MakeEnemyDecreaseDecreaseDamagePercentage = makeEnemyDecreaseDecreaseDamagePercentage;
		}

		public void UpdateRelation(ActorModel source, Faction foundingFaction, int expiryTurn, FixedPoint makeEnemyDecreaseAttackPercentage, FixedPoint makeEnemyDecreaseDecreaseDamagePercentage)
		{
			base.SourceActor = source;
			base.FoundingFaction = foundingFaction;
			base.ExpiryTurn = expiryTurn;
			LeftTurns = expiryTurn - source.manager.CombatModel.TurnManager.TurnCount;
			MakeEnemyDecreaseAttackPercentage = makeEnemyDecreaseAttackPercentage;
			MakeEnemyDecreaseDecreaseDamagePercentage = makeEnemyDecreaseDecreaseDamagePercentage;
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
