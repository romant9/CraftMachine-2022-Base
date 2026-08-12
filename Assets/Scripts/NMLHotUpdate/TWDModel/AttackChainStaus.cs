namespace TWDModel
{
	public class AttackChainStaus
	{
		public bool IsAttackChain;

		public FixedPoint UpCriticalDamagePercentage;

		public FixedPoint UpSpecialActorDamagePercentage;

		public void RecordStatus(AttackChainStaus attackChainStaus)
		{
			IsAttackChain = attackChainStaus.IsAttackChain;
			UpCriticalDamagePercentage = attackChainStaus.UpCriticalDamagePercentage;
			UpSpecialActorDamagePercentage = attackChainStaus.UpSpecialActorDamagePercentage;
		}
	}
}
