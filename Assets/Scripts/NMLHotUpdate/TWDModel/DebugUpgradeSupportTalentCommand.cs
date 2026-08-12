using BaseModel;

namespace TWDModel
{
	public class DebugUpgradeSupportTalentCommand : ModelCommand
	{
		public string SupportId { get; set; }

		public int TreeId { get; set; }

		public int NodeId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			SupportModel supportModel = (manager as TWDModelManager).Player.GetSupportModel(SupportId);
			if (supportModel == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			SupportTalentTreeModel supportTalentTreeModel = supportModel.SupportTalentTreeModels.Find((SupportTalentTreeModel x) => x.TreeId == TreeId);
			if (supportTalentTreeModel == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!supportTalentTreeModel.CanUpgradeNodeByNodeId(NodeId))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (supportTalentTreeModel.GetNodeModelByNodeId(NodeId) == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelResult = supportTalentTreeModel.UpgradeNodeByNodeId(NodeId);
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
