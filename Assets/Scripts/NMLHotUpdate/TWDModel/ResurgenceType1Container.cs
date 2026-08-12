using System.Collections.Generic;

namespace TWDModel
{
	public sealed class ResurgenceType1Container
	{
		public List<ResurgenceType1Info> ResurgenceType1InfoRecords { get; set; }

		public void Backup(ResurgenceType1ContainerBackup resurgenceType1ContainerBackup)
		{
			ResurgenceType1InfoRecords = new List<ResurgenceType1Info>();
			if (resurgenceType1ContainerBackup.ResurgenceType1InfoRecordsBackup == null)
			{
				return;
			}
			foreach (ResurgenceType1InfoBackup item in resurgenceType1ContainerBackup.ResurgenceType1InfoRecordsBackup)
			{
				ResurgenceType1Info resurgenceType1Info = new ResurgenceType1Info();
				resurgenceType1Info.Source = item.Source;
				resurgenceType1Info.ThisAbilityActionBearerTriggedRestoreAP = item.ThisAbilityActionBearerTriggedRestoreAP;
				resurgenceType1Info.TurnStartFactionActorNums = item.TurnStartFactionActorNums;
				resurgenceType1Info.UsedChargeAttackActors = ((item.UsedChargeAttackActors == null) ? null : new List<ActorModel>(item.UsedChargeAttackActors));
				resurgenceType1Info.ThisTurnAlreadyTiggerTimes = item.ThisTurnAlreadyTiggerTimes;
				ResurgenceType1InfoRecords.Add(resurgenceType1Info);
			}
		}
	}
}
