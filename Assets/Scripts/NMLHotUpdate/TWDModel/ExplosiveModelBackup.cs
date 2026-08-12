using BaseModel;

namespace TWDModel
{
	public class ExplosiveModelBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public ExplosiveModel Model { get; set; }

		public bool HasExploded { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public void RecordStatus(ExplosiveModel model)
		{
			Model = model;
			HasExploded = model.HasExploded;
		}

		public void BackUp()
		{
			Model.HasExploded = HasExploded;
		}
	}
}
