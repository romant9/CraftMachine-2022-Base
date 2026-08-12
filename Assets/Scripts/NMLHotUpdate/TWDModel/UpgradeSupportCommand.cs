using BaseModel;

namespace TWDModel
{
	public class UpgradeSupportCommand : ModelCommand
	{
		public string SupportId;

		public UpgradeSupportCommand()
		{
		}

		public UpgradeSupportCommand(string supportId)
		{
			SupportId = supportId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.Player.GetSupportModel(SupportId).Upgrade())
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
