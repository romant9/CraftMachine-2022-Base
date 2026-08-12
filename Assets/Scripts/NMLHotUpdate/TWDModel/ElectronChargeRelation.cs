namespace TWDModel
{
	public class ElectronChargeRelation : FactionToActorRelation
	{
		public int MaxLayer { get; private set; }

		public int CurrentLayer { get; private set; }

		public int LeftTurns { get; set; }

		public override FactionToActorRelationType Type => FactionToActorRelationType.ElectronCharge;

		public ElectronChargeRelation()
		{
		}

		public ElectronChargeRelation(ElectronChargeRelation electronChargeRelation)
			: base(electronChargeRelation)
		{
			MaxLayer = electronChargeRelation.MaxLayer;
			CurrentLayer = electronChargeRelation.CurrentLayer;
			LeftTurns = electronChargeRelation.LeftTurns;
		}

		public ElectronChargeRelation(ActorModel target, Faction foundingFaction, int expiryTurn, int maxLayer, int leftTurns)
			: base(target, foundingFaction, expiryTurn)
		{
			MaxLayer = maxLayer;
			CurrentLayer = 1;
			LeftTurns = leftTurns;
		}

		public void AddCurrentLayer()
		{
			int currentLayer = CurrentLayer + 1;
			CurrentLayer = currentLayer;
		}

		public void SubtractLeftTurns()
		{
			LeftTurns--;
		}
	}
}
