using BaseModel;

namespace TWDModel
{
	public class SpawnHarderLevelDetailMap : ModelCommand
	{
		public SpawnHarderLevelDetailMap()
		{
		}

		public SpawnHarderLevelDetailMap(MapMissionGroupModel mapMissionGroupModel)
			: base(mapMissionGroupModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel playerModel = (PlayerModel)manager.GetPlayer();
			MapMissionGroupModel model = manager.GetModel<MapMissionGroupModel>(base.ModelId);
			if (model == null)
			{
				tWDModelManager.Debug.LogError("SpawnHarderLevelDetailMap map mission group " + base.ModelId + " not found!");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (model.Missions.Count != 0)
			{
				tWDModelManager.Debug.LogError("SpawnHarderLevelDetailMap map mission group " + base.ModelId + " is empty");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!playerModel.MapContainerModel.SpawnMissionsForGroup(model.MissionSpawnPointGroup))
			{
				tWDModelManager.Debug.LogError("SpawnHarderLevelDetailMap map mission group " + base.ModelId + " spawning failed!");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
