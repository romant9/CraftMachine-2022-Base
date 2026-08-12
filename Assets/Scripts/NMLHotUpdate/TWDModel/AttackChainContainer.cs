using System.Collections.Generic;

namespace TWDModel
{
	public sealed class AttackChainContainer
	{
		public List<AttackChainSourceInfo> AttackChainSourceInfoRecords { get; set; }

		public void Backup(AttackChainContainerBackup attackChainContainerBackup)
		{
			AttackChainSourceInfoRecords = new List<AttackChainSourceInfo>();
			if (attackChainContainerBackup.AttackChainSourceInfoRecords == null)
			{
				return;
			}
			List<AttackChainSourceInfo> list = new List<AttackChainSourceInfo>();
			foreach (AttackChainSourceInfoBackup attackChainSourceInfoRecord in attackChainContainerBackup.AttackChainSourceInfoRecords)
			{
				AttackChainSourceInfo attackChainSourceInfo = new AttackChainSourceInfo();
				attackChainSourceInfo.Backup(attackChainSourceInfoRecord);
				list.Add(attackChainSourceInfo);
			}
			AttackChainSourceInfoRecords = list;
		}
	}
}
