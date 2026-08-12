using BaseModel;

namespace TWDModel
{
	public class StartEndlessCycleCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			EndlessModeCalendarDefinition currentEndlessModeCalendarDefinition = player.EndlessModeManager.CurrentEndlessModeCalendarDefinition;
			EndlessModeCalendarDefinition getActiveEndlessMode = player.EndlessModeManager.GetActiveEndlessMode;
			if (player.Combat != null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Skip);
			}
			if (getActiveEndlessMode == null)
			{
				tWDModelManager.Debug.Log("StartEndlessModeCommand next Endless definition is null");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (currentEndlessModeCalendarDefinition == getActiveEndlessMode)
			{
				tWDModelManager.Debug.Log("StartEndlessModeCommand current Endless definition is the same as the next one");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			player.EndlessModeManager.StartNewEndlessCycle(getActiveEndlessMode.Identifier);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
