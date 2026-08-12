using BaseModel;

namespace TWDModel
{
	public class DoorModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public DoorModel Model { get; set; }

		public bool IsOpen { get; set; }

		public bool IsHidden { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(DoorModel model)
		{
			Model = model;
			IsOpen = model.IsOpen;
			IsHidden = model.IsHidden;
		}

		public void BackUp()
		{
			Model.IsOpen = IsOpen;
			Model.IsHidden = IsHidden;
		}
	}
}
