using System.Collections.Generic;

namespace TWDModel
{
	public sealed class AttackChainSourceInfo
	{
		public ActorModel Source { get; set; }

		public int ThisTurnEarnAttackChainNums { get; set; }

		public List<AttackChainTargetInfo> AttackChainTargetInfoRecords { get; set; }

		public void Backup(AttackChainSourceInfoBackup attackChainSourceInfoBackup)
		{
			Source = attackChainSourceInfoBackup.Source;
			ThisTurnEarnAttackChainNums = attackChainSourceInfoBackup.ThisTurnEarnAttackChainNums;
			List<AttackChainTargetInfo> list = new List<AttackChainTargetInfo>();
			foreach (AttackChainTargetInfoBackup attackChainTargetInfoRecord in attackChainSourceInfoBackup.AttackChainTargetInfoRecords)
			{
				AttackChainTargetInfo attackChainTargetInfo = new AttackChainTargetInfo();
				attackChainTargetInfo.Backup(attackChainTargetInfoRecord);
				list.Add(attackChainTargetInfo);
			}
			AttackChainTargetInfoRecords = list;
		}
	}
}
