using System.Collections.Generic;

namespace TWDModel
{
	public sealed class AttackChainContainerBackup
	{
		public List<AttackChainSourceInfoBackup> AttackChainSourceInfoRecords { get; set; }

		public void RecordStatus(AttackChainContainer attackChainContainer)
		{
			AttackChainSourceInfoRecords = new List<AttackChainSourceInfoBackup>();
			if (attackChainContainer.AttackChainSourceInfoRecords == null)
			{
				return;
			}
			List<AttackChainSourceInfoBackup> list = new List<AttackChainSourceInfoBackup>();
			foreach (AttackChainSourceInfo attackChainSourceInfoRecord in attackChainContainer.AttackChainSourceInfoRecords)
			{
				AttackChainSourceInfoBackup attackChainSourceInfoBackup = new AttackChainSourceInfoBackup();
				attackChainSourceInfoBackup.RecordStatus(attackChainSourceInfoRecord);
				list.Add(attackChainSourceInfoBackup);
			}
			AttackChainSourceInfoRecords = list;
		}
	}
}
