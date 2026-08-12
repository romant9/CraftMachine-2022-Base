using BaseModel;

namespace TWDModel
{
	public class TriggerModelBackup : TWDModelObject
	{
		public int CurrentActivationCount;

		[IgnoreModelProperty]
		public TriggerModel Model { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(TriggerModel model)
		{
			Model = model;
			CurrentActivationCount = model.CurrentActivationCount;
		}

		public void BackUp()
		{
			Model.CurrentActivationCount = CurrentActivationCount;
		}
	}
}
