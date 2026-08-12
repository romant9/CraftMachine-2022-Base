namespace TWDModel
{
	public sealed class AttackChainTargetInfoBackup
	{
		public ActorModel Target { get; set; }

		public int AttackNums { get; set; }

		public void RecordStatus(AttackChainTargetInfo attackChainTargetInfo)
		{
			Target = attackChainTargetInfo.Target;
			AttackNums = attackChainTargetInfo.AttackNums;
		}
	}
}
