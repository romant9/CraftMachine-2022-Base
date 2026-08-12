using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class RunLocationModel : TWDModelObject, IRunLocationItemContainer
	{
		[JsonIgnore]
		public Dictionary<SlicePosition, List<string>> SliceViewIds;

		[JsonIgnore]
		private Dictionary<OutpostSliceModel, GridColliderData> CachedGridColliderData = new Dictionary<OutpostSliceModel, GridColliderData>();

		public string SceneName { get; set; }

		public string ContentPack { get; set; }

		public string BackgroundSceneName { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool IncludedInBuild { get; set; }

		public GridModel Grid { get; private set; }

		public List<TWDModelObject> Models { get; private set; }

		public List<MissionModel> Missions { get; set; }

		public List<int> GridVisibility { get; set; }

		public List<int> GridMovement { get; set; }

		public string ExportedVisibility { get; set; }

		public string ExportedMovement { get; set; }

		public string VersionInfo { get; set; }

		public bool IsOutpostModel
		{
			get
			{
				if (Missions != null && Missions.Count > 0)
				{
					if (Missions[0].OutpostSlices != null)
					{
						return Missions[0].OutpostSlices.Count > 0;
					}
					return false;
				}
				return false;
			}
		}

		public RunLocationModel()
		{
			Models = new List<TWDModelObject>();
			Missions = new List<MissionModel>();
		}

		public override void Initialize()
		{
			base.Initialize();
			IncludedInBuild = true;
			Grid = new GridModel();
			Grid.SetManager(base.manager);
			Grid.Initialize();
			base.Debug.Log("Initializing run location: " + SceneName);
		}

		public void SetGrid(FixedVec3 position, FixedVec2 cellSize, int width, int height)
		{
			Grid = new GridModel();
			Grid.SetManager(base.manager);
			Grid.SetPosition(position);
			Grid.SetCellSize(cellSize);
			Grid.SetWidth(width);
			Grid.SetHeight(height);
			Grid.Initialize();
		}

		public void AddModelObject(TWDModelObject objectToAdd)
		{
			Models.Add(objectToAdd);
		}

		public void AddMission(MissionModel model)
		{
			Missions.Add(model);
		}

		public void AddSlice(OutpostSliceModel sliceModel)
		{
			throw new NotImplementedException();
		}

		public MissionModel GetMission(string missionId)
		{
			if (missionId == null)
			{
				return Missions[0];
			}
			foreach (MissionModel mission in Missions)
			{
				if (mission.Id == missionId)
				{
					return mission;
				}
			}
			return null;
		}

		public void AddStartLocation(GridCoordinate coordinate)
		{
			throw new NotSupportedException();
		}

		public void SetSceneName(string sceneName)
		{
			SceneName = sceneName;
		}

		public void SetBackgroundSceneName(string sceneName)
		{
			BackgroundSceneName = sceneName;
		}

		public OutpostSliceModel GetSliceModel(string sliceViewId)
		{
			foreach (OutpostSliceModel outpostSlice in Missions[0].OutpostSlices)
			{
				if (outpostSlice.ViewId == sliceViewId)
				{
					return outpostSlice;
				}
			}
			return null;
		}

		public int GetMoveBlockedBits(GridCoordinate coordinate, string sliceViewId, bool staticBits)
		{
			int num = 0;
			OutpostSliceModel sliceModel = GetSliceModel(sliceViewId);
			if (sliceModel != null)
			{
				GridColliderData gridColliderData = GetGridColliderData(sliceModel);
				if (gridColliderData != null)
				{
					int num2 = ((!staticBits) ? 1 : 0);
					int num3 = ((!staticBits) ? gridColliderData.MovementColliderCount : 0);
					for (int i = num2; i <= num3; i++)
					{
						num |= (gridColliderData.IsBlocked(coordinate, i) ? 16 : 0);
					}
					for (int j = 0; j < 4; j++)
					{
						GridCoordinate coordinateNeighbor = Grid.GetCoordinateNeighbor(coordinate, j * 2);
						if (!coordinateNeighbor.IsValid)
						{
							continue;
						}
						for (int k = num2; k <= num3; k++)
						{
							if (gridColliderData.IsMovementBlocked(coordinate, coordinateNeighbor, k))
							{
								num |= 1 << j;
							}
						}
					}
				}
			}
			return num;
		}

		public int GetVisibilityBlockedBits(GridCoordinate coordinate, string sliceViewId, bool staticBits)
		{
			int num = 0;
			OutpostSliceModel sliceModel = GetSliceModel(sliceViewId);
			if (sliceModel != null)
			{
				GridColliderData gridColliderData = GetGridColliderData(sliceModel);
				if (gridColliderData != null)
				{
					int num2 = ((!staticBits) ? 1 : 0);
					int num3 = ((!staticBits) ? gridColliderData.VisibilityColliderCount : 0);
					for (int i = num2; i <= num3; i++)
					{
						num |= (gridColliderData.IsVisibilityBlocked(coordinate, i) ? 16 : 0);
					}
					for (int j = 0; j < 4; j++)
					{
						GridCoordinate coordinateNeighbor = Grid.GetCoordinateNeighbor(coordinate, j * 2);
						if (!coordinateNeighbor.IsValid)
						{
							continue;
						}
						for (int k = num2; k <= num3; k++)
						{
							if (gridColliderData.IsVisibilityBlocked(coordinate, coordinateNeighbor, k))
							{
								num |= 1 << j;
							}
						}
					}
				}
			}
			return num;
		}

		private GridColliderData GetGridColliderData(OutpostSliceModel sliceModel)
		{
			if (!CachedGridColliderData.ContainsKey(sliceModel))
			{
				List<CombatColliderModel> list = new List<CombatColliderModel>();
				List<CombatColliderModel> list2 = new List<CombatColliderModel>();
				foreach (TWDModelObject model in sliceModel.Models)
				{
					if (model is CombatColliderModel { IsDynamic: not false } combatColliderModel)
					{
						if (combatColliderModel.BlockMovement)
						{
							list.Add(combatColliderModel);
						}
						if (combatColliderModel.BlockVision)
						{
							list2.Add(combatColliderModel);
						}
					}
				}
				GridColliderData value = new GridColliderData(Grid, list2.Count, list.Count, sliceModel.ExportedVisibility, sliceModel.ExportedMovement);
				CachedGridColliderData.Add(sliceModel, value);
			}
			return CachedGridColliderData[sliceModel];
		}

		public List<string> GetSliceViewIds(SlicePosition slicePosition)
		{
			if (SliceViewIds == null)
			{
				SliceViewIds = new Dictionary<SlicePosition, List<string>>();
			}
			if (!SliceViewIds.ContainsKey(slicePosition))
			{
				List<string> list = new List<string>();
				if (Missions.Count > 0)
				{
					foreach (OutpostSliceModel outpostSlice in Missions[0].OutpostSlices)
					{
						if (outpostSlice.SlicePosition == slicePosition)
						{
							list.Add(outpostSlice.ViewId);
						}
					}
				}
				SliceViewIds.Add(slicePosition, list);
			}
			return SliceViewIds[slicePosition];
		}

		public bool GetSliceIndexAndCount(string sliceViewId, out int index, out int count)
		{
			index = -1;
			count = -1;
			OutpostSliceModel sliceModel = GetSliceModel(sliceViewId);
			List<string> sliceViewIds = GetSliceViewIds(sliceModel.SlicePosition);
			for (int i = 0; i < sliceViewIds.Count; i++)
			{
				if (sliceViewIds[i] == sliceViewId)
				{
					index = i;
					count = sliceViewIds.Count;
					return true;
				}
			}
			return false;
		}

		public List<GridCoordinate> GetOutpostStartLocations()
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			foreach (TWDModelObject model in Missions[0].Models)
			{
				if (model is CombatStartLocationModel combatStartLocationModel)
				{
					list.Add(combatStartLocationModel.Location);
				}
			}
			return list;
		}

		public List<GridCoordinate> GetOutpostObjectiveLocations(string sliceViewId, OutpostObjectiveType objectiveType)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			foreach (OutpostSliceModel outpostSlice in Missions[0].OutpostSlices)
			{
				if (!(outpostSlice.ViewId == sliceViewId))
				{
					continue;
				}
				foreach (TWDModelObject model in outpostSlice.Models)
				{
					if (!(model is InteractiveObjectModel interactiveObjectModel))
					{
						continue;
					}
					foreach (InteractionReceiver receiver in interactiveObjectModel.receivers)
					{
						if (receiver is OutpostObjectiveModel outpostObjectiveModel && outpostObjectiveModel.OutpostObjectiveType == objectiveType)
						{
							list.Add(interactiveObjectModel.Location.Coordinate);
						}
					}
				}
			}
			return list;
		}

		public WalkerSpawnPointModel GetOutpostThreatSpawn()
		{
			foreach (TWDModelObject model in Missions[0].Models)
			{
				if (model is WalkerSpawnPointModel { ActivationType: ActivationType.Threat } walkerSpawnPointModel)
				{
					return walkerSpawnPointModel;
				}
			}
			return null;
		}

		public OutpostHotspotModel GetHotspotAt(string sliceViewId, GridCoordinate coordinate)
		{
			if (Missions.Count > 0)
			{
				foreach (OutpostSliceModel outpostSlice in Missions[0].OutpostSlices)
				{
					if (!(outpostSlice.ViewId == sliceViewId))
					{
						continue;
					}
					foreach (TWDModelObject model in outpostSlice.Models)
					{
						if (!(model is OutpostHotspotModel outpostHotspotModel))
						{
							continue;
						}
						if (outpostHotspotModel.DefenderModel != null && outpostHotspotModel.DefenderModel.Location.Contains(coordinate))
						{
							return outpostHotspotModel;
						}
						if (outpostHotspotModel.SpawnPointModel != null && outpostHotspotModel.SpawnPointModel.Location.Contains(coordinate))
						{
							return outpostHotspotModel;
						}
						if (outpostHotspotModel.FlagModels != null)
						{
							for (int i = 0; i < outpostHotspotModel.FlagModels.Count; i++)
							{
								if (outpostHotspotModel.FlagModels[i] is InteractiveObjectModel interactiveObjectModel && interactiveObjectModel.Location.Contains(coordinate))
								{
									return outpostHotspotModel;
								}
							}
						}
						if (outpostHotspotModel.ResourceContainerModels == null)
						{
							continue;
						}
						for (int j = 0; j < outpostHotspotModel.ResourceContainerModels.Count; j++)
						{
							if (outpostHotspotModel.ResourceContainerModels[j] is InteractiveObjectModel interactiveObjectModel2 && interactiveObjectModel2.Location.Contains(coordinate))
							{
								return outpostHotspotModel;
							}
						}
					}
					return null;
				}
			}
			return null;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
