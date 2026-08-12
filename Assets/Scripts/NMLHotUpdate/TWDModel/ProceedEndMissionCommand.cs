using BaseModel;

namespace TWDModel
{
	public class ProceedEndMissionCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.OK;
			TWDModelManager obj = manager as TWDModelManager;
			if (obj.Player.Combat == null)
			{
				manager.Debug.LogError("ProceedEndMission: Combat is null");
				result = TWDModelResult.Error;
			}
			if (obj.Player.Combat.CombatRetryChoicePendingState != MissionRetryState.Pending)
			{
				manager.Debug.LogError("ProceedEndMission: Combat is not at the correct state");
				result = TWDModelResult.Error;
			}
			obj.Player.Combat.ProceedEndCombat();
			return new NGModelCommandRespond(this, result);
		}
	}
}
