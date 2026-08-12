using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class MissionModel : TWDModelObject, IRunLocationItemContainer
	{
		public string Id { get; set; }

		public string MissionName { get; set; }

		public MissionFactionNames[] FactionNames { get; set; }

		public string DisplayTextID { get; set; }

		public MissionType TypeOfMission { get; set; }

		public int InitialTurnCountToWave { get; set; }

		public int InitialThreatLevel { get; set; }

		public int OptionalLootKeys { get; set; }

		public int CompletionBonusLootKeys { get; set; }

		public bool IsDeadly { get; set; }

		public int PvPAfterAlarmTurns { get; set; }

		public PvPMissionType PVPType { get; set; }

		public IncrementalDifficultyMissionType IncrementalDifficultyType { get; set; }

		public List<TWDModelObject> MissionTargetObjects { get; set; }

		public List<int> MissionTags { get; set; }

		public DropEventDefinition.DropEventTag LootTag { get; set; }

		public int SurvivorLevelRequirementOffset { get; set; }

		public MissionStarCondition[] MissionStarConditions { get; set; }

		public int MaxTeamSize { get; set; }

		public List<TWDModelObject> Models { get; private set; }

		public List<OutpostSliceModel> OutpostSlices { get; private set; }

		public MissionModel()
		{
			Models = new List<TWDModelObject>();
			OutpostSlices = new List<OutpostSliceModel>();
			MissionTargetObjects = new List<TWDModelObject>();
		}

		public void AddModelObject(TWDModelObject objectToAdd)
		{
			Models.Add(objectToAdd);
		}

		public void AddMission(MissionModel model)
		{
			throw new NotSupportedException("Missions not supported below another mission - move to scenario.");
		}

		public void AddSlice(OutpostSliceModel sliceModel)
		{
			OutpostSlices.Add(sliceModel);
		}

		public override bool IsValid()
		{
			return true;
		}

		public OutpostSliceModel GetOutpostSlice(string outpostSliceViewId)
		{
			foreach (OutpostSliceModel outpostSlice in OutpostSlices)
			{
				if (outpostSlice.ViewId == outpostSliceViewId)
				{
					return outpostSlice;
				}
			}
			return null;
		}

		public List<ActorSpawnPointModel> GetActorSpawnPoints()
		{
			List<ActorSpawnPointModel> list = new List<ActorSpawnPointModel>();
			foreach (TWDModelObject model in Models)
			{
				if (model is ActorSpawnPointModel)
				{
					list.Add(model as ActorSpawnPointModel);
				}
			}
			return list;
		}
	}
}
