using BaseModel;

namespace TWDModel
{
	public class ClearOutpostCommand : ModelCommand
	{
		public string OutpostTemplateID { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				if (tWDModelManager.Player.OutpostModel.EditLevelModel != null)
				{
					tWDModelManager.Player.OutpostModel.EditLevelModel.ClearAllHotspotInfos();
				}
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
