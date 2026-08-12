using BaseModel;

namespace TWDModel
{
	public class MovableModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public MovableModel Model { get; set; }

		public bool IsMoved { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(MovableModel model)
		{
			Model = model;
			IsMoved = model.IsMoved;
		}

		public void BackUp()
		{
			Model.IsMoved = IsMoved;
		}
	}
}
