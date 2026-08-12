using System.Collections.Generic;

namespace TWDModel
{
	public sealed class ResurgenceType2ContainerBackup
	{
		public List<ResurgenceType2InfoBackup> ResurgenceType2InfoRecordsBackup { get; set; }

		public void RecordStatus(ResurgenceType2Container resurgenceType2Container)
		{
			ResurgenceType2InfoRecordsBackup = new List<ResurgenceType2InfoBackup>();
			if (resurgenceType2Container.ResurgenceType2InfoRecords == null)
			{
				return;
			}
			foreach (ResurgenceType2Info resurgenceType2InfoRecord in resurgenceType2Container.ResurgenceType2InfoRecords)
			{
				ResurgenceType2InfoBackup resurgenceType2InfoBackup = new ResurgenceType2InfoBackup();
				resurgenceType2InfoBackup.Source = resurgenceType2InfoRecord.Source;
				resurgenceType2InfoBackup.ThisAbilityActionBearerTriggedRestoreAP = resurgenceType2InfoRecord.ThisAbilityActionBearerTriggedRestoreAP;
				resurgenceType2InfoBackup.TurnStartFactionActorNums = resurgenceType2InfoRecord.TurnStartFactionActorNums;
				resurgenceType2InfoBackup.UsedChargeAttackActors = ((resurgenceType2InfoRecord.UsedChargeAttackActors == null) ? null : new List<ActorModel>(resurgenceType2InfoRecord.UsedChargeAttackActors));
				resurgenceType2InfoBackup.NextCanTriggedRestoreAPTurn = resurgenceType2InfoRecord.NextCanTriggedRestoreAPTurn;
				ResurgenceType2InfoRecordsBackup.Add(resurgenceType2InfoBackup);
			}
		}
	}
}
