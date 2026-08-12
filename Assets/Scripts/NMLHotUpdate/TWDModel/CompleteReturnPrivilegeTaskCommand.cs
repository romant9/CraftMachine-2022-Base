using BaseModel;

namespace TWDModel
{
	public class CompleteReturnPrivilegeTaskCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager?.Player?.ReturnActivityManager?.ReturnPrivilege == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			bool flag = tWDModelManager.Player.ReturnActivityManager.ReturnPrivilege.TryCompleteCurrentTask();
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
