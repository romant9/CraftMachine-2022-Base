using BaseModel;

namespace TWDModel
{
	public class SetMissionObjectiveModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public SetMissionObjectiveModel Model { get; set; }

		public bool IsTriggered { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(SetMissionObjectiveModel model)
		{
			Model = model;
			IsTriggered = model.IsTriggered;
		}

		public void BackUp()
		{
			Model.IsTriggered = IsTriggered;
		}
	}
}
