using BaseModel;

namespace TWDModel
{
	public class CreateOutpostCommand : ModelCommand
	{
		public string OutpostTemplateID { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				tWDModelManager.Player.OutpostModel.EditLevelModel = new OutpostLevelModel();
				tWDModelManager.Player.OutpostModel.EditLevelModel.SetManager(tWDModelManager);
				OutpostTemplateDefinition outpostTemplateDefinition = tWDModelManager.GameEconomyData.GetOutpostTemplateDefinition(tWDModelManager.Player.SelectedOutpostTemplateDefinitionId);
				if (outpostTemplateDefinition != null)
				{
					tWDModelManager.Player.OutpostModel.EditLevelModel.BaseRunLocationID = outpostTemplateDefinition.MissionID;
					result = TWDModelResult.OK;
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
