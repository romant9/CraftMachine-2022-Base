using System.Collections.Generic;

namespace TWDModel
{
	public sealed class AttackChainSourceInfoBackup
	{
		public ActorModel Source { get; set; }

		public int ThisTurnEarnAttackChainNums { get; set; }

		public List<AttackChainTargetInfoBackup> AttackChainTargetInfoRecords { get; set; }

		public void RecordStatus(AttackChainSourceInfo attackChainSourceInfoRecord)
		{
			Source = attackChainSourceInfoRecord.Source;
			ThisTurnEarnAttackChainNums = attackChainSourceInfoRecord.ThisTurnEarnAttackChainNums;
			List<AttackChainTargetInfoBackup> list = new List<AttackChainTargetInfoBackup>();
			foreach (AttackChainTargetInfo attackChainTargetInfoRecord in attackChainSourceInfoRecord.AttackChainTargetInfoRecords)
			{
				AttackChainTargetInfoBackup attackChainTargetInfoBackup = new AttackChainTargetInfoBackup();
				attackChainTargetInfoBackup.RecordStatus(attackChainTargetInfoRecord);
				list.Add(attackChainTargetInfoBackup);
			}
			AttackChainTargetInfoRecords = list;
		}
	}
}
