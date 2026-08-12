using System;

namespace TWDModel
{
	public class CivilianSpawnPointModel : ActorSpawnPointModel
	{
		public string ActorClassID { get; set; }

		public string ActorID { get; set; }

		public bool CivilianCanStruggle { get; set; }

		public CivilianSpawnPointModel()
		{
		}

		public CivilianSpawnPointModel(string viewId)
			: base(viewId)
		{
		}

		protected override int InternalSpawn(ActorModel instigator)
		{
			CombatModel combatModel = base.manager.CombatModel;
			MissionGenerationData missionGenerationData = base.gameEconomyData.GetMissionGenerationData(base.manager.Player.SelectedMissionDifficulty);
			int level = Math.Max(1, base.manager.Player.PlayerRandom.GetRandomInRange(missionGenerationData.MinWalkerLevel, missionGenerationData.MaxWalkerLevel) + base.LevelOffset);
			ActorModel actorModel = combatModel.CreateActor(base.Location.Coordinate, Faction.Civilian, level, base.SpawnTag, ActorClassID, ActorID, base.Gender);
			actorModel.MissionFailCondition = base.MissionFailCondition;
			actorModel.CivilianCanStruggle = CivilianCanStruggle;
			actorModel.SetupForCombat(combatModel);
			return 1;
		}
	}
}
