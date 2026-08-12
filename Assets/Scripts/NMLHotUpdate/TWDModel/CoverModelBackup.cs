using BaseModel;

namespace TWDModel
{
	public class CoverModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public CoverModel Model { get; set; }

		public bool IsActive { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(CoverModel model)
		{
			Model = model;
			IsActive = model.IsActive;
		}

		public void BackUp()
		{
			Model.IsActive = IsActive;
		}
	}
}
