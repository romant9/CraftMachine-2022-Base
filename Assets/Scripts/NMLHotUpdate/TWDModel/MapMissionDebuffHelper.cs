using TWDModel.ContentTypes;

namespace TWDModel
{
	public static class MapMissionDebuffHelper
	{
		public static MapMissionModel CanUseDebuffMission(TWDModelManager manager)
		{
			if (manager == null)
			{
				return null;
			}
			if (manager.Player == null)
			{
				return null;
			}
			if (!(manager.Player.GetAttackTargetMissionModel() is MapMissionModel mapMissionModel))
			{
				return null;
			}
			if (mapMissionModel.IsInWeeklyChallenge)
			{
				return mapMissionModel;
			}
			if (mapMissionModel.IsInApocalyptiWeeklyChallenge)
			{
				return mapMissionModel;
			}
			if (mapMissionModel.IsEndlessMission && manager.Player.EndlessModeManager != null && manager.Player.EndlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert)
			{
				return mapMissionModel;
			}
			return null;
		}
	}
}
