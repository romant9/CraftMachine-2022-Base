using BaseModel;

namespace TWDModel
{
	public class MissionLogicModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public MissionLogicModel Model { get; set; }

		public bool HasFired { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(MissionLogicModel model)
		{
			Model = model;
			HasFired = model.HasFired;
		}

		public void BackUp()
		{
			Model.HasFired = HasFired;
		}
	}
}
