using BaseModel;

namespace TWDModel
{
	public class CombatExitModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public CombatExitModel Model { get; set; }

		public bool Enabled { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(CombatExitModel model)
		{
			Model = model;
			Enabled = model.Enabled;
		}

		public void BackUp()
		{
			Model.Enabled = Enabled;
		}
	}
}
