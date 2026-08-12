namespace TWDModel
{
	public sealed class AttackChainTargetInfo
	{
		public ActorModel Target { get; set; }

		public int AttackNums { get; set; }

		public void Backup(AttackChainTargetInfoBackup attackChainTargetInfoBackup)
		{
			Target = attackChainTargetInfoBackup.Target;
			AttackNums = attackChainTargetInfoBackup.AttackNums;
		}
	}
}
