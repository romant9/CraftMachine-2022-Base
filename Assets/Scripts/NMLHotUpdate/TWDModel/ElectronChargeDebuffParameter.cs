namespace TWDModel
{
	public class ElectronChargeDebuffParameter : DebuffParameterAbstract<int>
	{
		public override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		public override bool IgnoreExpiryTurn => false;

		public ElectronChargeDebuffParameter(ElectronChargeDebuffParameter electronChargeDebuffParameter)
			: base((DebuffParameterAbstract<int>)electronChargeDebuffParameter)
		{
		}

		public ElectronChargeDebuffParameter()
		{
		}

		public ElectronChargeDebuffParameter(string parameterKey, int parameterValue, int expiryTurn)
			: base(parameterKey, parameterValue, expiryTurn)
		{
		}
	}
}
