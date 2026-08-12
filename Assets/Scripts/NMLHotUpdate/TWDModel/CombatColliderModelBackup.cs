using BaseModel;

namespace TWDModel
{
	public class CombatColliderModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public CombatColliderModel Model { get; set; }

		public bool IsEnabled { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(CombatColliderModel model)
		{
			Model = model;
			IsEnabled = model.IsEnabled;
		}

		public void BackUp()
		{
			Model.IsEnabled = IsEnabled;
		}
	}
}
