using System.Collections.Generic;

namespace TWDModel
{
	public sealed class ResurgenceType2Container
	{
		public List<ResurgenceType2Info> ResurgenceType2InfoRecords { get; set; }

		public void Backup(ResurgenceType2ContainerBackup resurgenceType2ContainerBackup)
		{
			ResurgenceType2InfoRecords = new List<ResurgenceType2Info>();
			if (resurgenceType2ContainerBackup.ResurgenceType2InfoRecordsBackup == null)
			{
				return;
			}
			foreach (ResurgenceType2InfoBackup item in resurgenceType2ContainerBackup.ResurgenceType2InfoRecordsBackup)
			{
				ResurgenceType2Info resurgenceType2Info = new ResurgenceType2Info();
				resurgenceType2Info.Source = item.Source;
				resurgenceType2Info.ThisAbilityActionBearerTriggedRestoreAP = item.ThisAbilityActionBearerTriggedRestoreAP;
				resurgenceType2Info.TurnStartFactionActorNums = item.TurnStartFactionActorNums;
				resurgenceType2Info.UsedChargeAttackActors = ((item.UsedChargeAttackActors == null) ? null : new List<ActorModel>(item.UsedChargeAttackActors));
				resurgenceType2Info.NextCanTriggedRestoreAPTurn = item.NextCanTriggedRestoreAPTurn;
				ResurgenceType2InfoRecords.Add(resurgenceType2Info);
			}
		}
	}
}
