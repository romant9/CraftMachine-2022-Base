using BaseModel;

namespace TWDModel
{
	public class LootModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public LootModel Model { get; set; }

		public bool IsOpened { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(LootModel model)
		{
			Model = model;
			IsOpened = model.IsOpened;
		}

		public void BackUp()
		{
			Model.IsOpened = IsOpened;
		}
	}
}
