using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ModelListReportCommand : ModelCommand
	{
		public List<string> ClientModelList;

		public ModelListReportCommand()
		{
		}

		public ModelListReportCommand(List<string> clientModels)
		{
			ClientModelList = clientModels;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager.ServerService != null)
			{
				string text = "ServerModelList: ";
				List<string> modelListReport = tWDModelManager.GetModelListReport();
				for (int i = 0; i < modelListReport.Count; i++)
				{
					text = text + modelListReport[i] + " : ";
				}
				tWDModelManager.Debug.Log(text);
				string text2 = "ClientModelList: ";
				if (ClientModelList != null)
				{
					for (int j = 0; j < ClientModelList.Count; j++)
					{
						text2 = text2 + ClientModelList[j] + " : ";
					}
				}
				tWDModelManager.Debug.Log(text2);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
