using System.Collections.Generic;

namespace TWDModel
{
	public sealed class ResurgenceType1ContainerBackup
	{
		public List<ResurgenceType1InfoBackup> ResurgenceType1InfoRecordsBackup { get; set; }

		public void RecordStatus(ResurgenceType1Container resurgenceType1Container)
		{
			ResurgenceType1InfoRecordsBackup = new List<ResurgenceType1InfoBackup>();
			if (resurgenceType1Container.ResurgenceType1InfoRecords == null)
			{
				return;
			}
			foreach (ResurgenceType1Info resurgenceType1InfoRecord in resurgenceType1Container.ResurgenceType1InfoRecords)
			{
				ResurgenceType1InfoBackup resurgenceType1InfoBackup = new ResurgenceType1InfoBackup();
				resurgenceType1InfoBackup.Source = resurgenceType1InfoRecord.Source;
				resurgenceType1InfoBackup.ThisAbilityActionBearerTriggedRestoreAP = resurgenceType1InfoRecord.ThisAbilityActionBearerTriggedRestoreAP;
				resurgenceType1InfoBackup.TurnStartFactionActorNums = resurgenceType1InfoRecord.TurnStartFactionActorNums;
				resurgenceType1InfoBackup.UsedChargeAttackActors = ((resurgenceType1InfoRecord.UsedChargeAttackActors == null) ? null : new List<ActorModel>(resurgenceType1InfoRecord.UsedChargeAttackActors));
				resurgenceType1InfoBackup.ThisTurnAlreadyTiggerTimes = resurgenceType1InfoRecord.ThisTurnAlreadyTiggerTimes;
				ResurgenceType1InfoRecordsBackup.Add(resurgenceType1InfoBackup);
			}
		}
	}
}
