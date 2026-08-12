using BaseModel;

namespace TWDModel
{
	public class SocialPlatformConnected : ModelCommand
	{
		public bool GCConnected { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (GCConnected)
			{
				tWDModelManager?.Player.Blackboard.SetToggle("Toggle.GameCenterConnected");
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
