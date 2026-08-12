using BaseModel;

namespace TWDModel
{
	public class SetDefenseLogSeenCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (tWDModelManager != null && tWDModelManager.Player != null)
			{
				tWDModelManager.Player.SetDefenseLogSeen();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
