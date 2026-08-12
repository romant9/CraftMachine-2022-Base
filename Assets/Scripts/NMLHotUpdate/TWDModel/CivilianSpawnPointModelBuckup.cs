namespace TWDModel
{
	public class CivilianSpawnPointModelBuckup : ActorSpawnPointModelBackup
	{
		public string ActorClassID { get; set; }

		public string ActorID { get; set; }

		public bool CivilianCanStruggle { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void RecordStatus(ActorSpawnPointModel model)
		{
			base.RecordStatus(model);
			ActorClassID = (base.Model as CivilianSpawnPointModel).ActorClassID;
			ActorID = (base.Model as CivilianSpawnPointModel).ActorID;
			CivilianCanStruggle = (base.Model as CivilianSpawnPointModel).CivilianCanStruggle;
		}

		public override void BackUp()
		{
			base.BackUp();
			(base.Model as CivilianSpawnPointModel).ActorClassID = ActorClassID;
			(base.Model as CivilianSpawnPointModel).ActorID = ActorID;
			(base.Model as CivilianSpawnPointModel).CivilianCanStruggle = CivilianCanStruggle;
		}
	}
}
