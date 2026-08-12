using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ApplyRunLocationCommand : ModelCommand
	{
		public string TransactionId;

		public VisitMode VisitMode;

		public ApplyRunLocationCommand()
		{
		}

		public ApplyRunLocationCommand(string transactionId, VisitMode visitMode)
		{
			TransactionId = transactionId;
			VisitMode = visitMode;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager obj = manager as TWDModelManager;
			string content = obj.ContentService.GetContent(TransactionId);
			obj.ApplyRunLocation(runLocation: obj.GetMessageSerializer().DeserializeObject<List<RunLocationModel>>(content)[0], visitMode: VisitMode, defendingPlayer: null);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
