using BaseModel;

namespace TWDModel
{
	public class MarkReturnLoginPopupShownCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.ReturnActivityManager?.ReturnLogin == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.ReturnActivityManager.ReturnLogin.MarkPopupShownOnCurrentLogin();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
