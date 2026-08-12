using System.Collections.Generic;
using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class ApplyMediationDataCommand : ModelCommand
	{
		public string TransactionId;

		public ApplyMediationDataCommand()
		{
		}

		public ApplyMediationDataCommand(string transactionId)
		{
			TransactionId = transactionId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			string content = tWDModelManager.ContentService.GetContent(TransactionId);
			List<MediationData> list = tWDModelManager.GetMessageSerializer().DeserializeObject<List<MediationData>>(content);
			if (list.Count > 0)
			{
				tWDModelManager.MediationData = list[0];
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
