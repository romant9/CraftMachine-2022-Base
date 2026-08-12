using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class OutpostLevelModel : TWDModelObject
	{
		public const string ChosenSliceChanged = "ChosenSliceChanged";

		public const string HotspotInfoChanged = "HotspotInfoChanged";

		public string BaseRunLocationID { get; set; }

		public List<SliceConfiguration> ChosenSlices { get; set; }

		public List<HotspotInfo> HotspotInfos { get; set; }

		[JsonIgnore]
		public bool HasFlag
		{
			get
			{
				for (int i = 0; i < HotspotInfos.Count; i++)
				{
					if (HotspotInfos[i].State == HotspotState.Flag && HasChosenSlice(HotspotInfos[i].SliceViewId))
					{
						return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasResourceContainer
		{
			get
			{
				for (int i = 0; i < HotspotInfos.Count; i++)
				{
					if (HotspotInfos[i].State == HotspotState.ResourceContainer && HasChosenSlice(HotspotInfos[i].SliceViewId))
					{
						return true;
					}
				}
				return false;
			}
		}

		public OutpostLevelModel()
		{
			ChosenSlices = new List<SliceConfiguration>();
			HotspotInfos = new List<HotspotInfo>();
		}

		public int GetDefenderCount()
		{
			int num = 0;
			for (int i = 0; i < HotspotInfos.Count; i++)
			{
				if (HotspotInfos[i].IsDefenderSpawn && HasChosenSlice(HotspotInfos[i].SliceViewId))
				{
					num++;
				}
			}
			return num;
		}

		public bool HasDefender(HotspotState state)
		{
			for (int i = 0; i < HotspotInfos.Count; i++)
			{
				if (HotspotInfos[i].State == state && HasChosenSlice(HotspotInfos[i].SliceViewId))
				{
					return true;
				}
			}
			return false;
		}

		public HotspotState GetFirstFreeDefenderState()
		{
			if (!HasDefender(HotspotState.DefenderSpawn_0))
			{
				return HotspotState.DefenderSpawn_0;
			}
			if (!HasDefender(HotspotState.DefenderSpawn_1))
			{
				return HotspotState.DefenderSpawn_1;
			}
			if (!HasDefender(HotspotState.DefenderSpawn_2))
			{
				return HotspotState.DefenderSpawn_2;
			}
			return HotspotState.None;
		}

		public string GetChosenSliceViewId(SlicePosition slicePosition)
		{
			for (int i = 0; i < ChosenSlices.Count; i++)
			{
				if (ChosenSlices[i].Position == slicePosition)
				{
					return ChosenSlices[i].ViewId;
				}
			}
			return null;
		}

		public SliceConfiguration GetSliceConfigById(string ViewId)
		{
			for (int i = 0; i < ChosenSlices.Count; i++)
			{
				if (ChosenSlices[i].ViewId == ViewId)
				{
					return ChosenSlices[i];
				}
			}
			return null;
		}

		public void SetHotspotInfo(string sliceViewId, string hotspotViewId, HotspotState state, WalkerType walkerType, int count, AIMode defensiveMode)
		{
			bool flag = false;
			for (int i = 0; i < HotspotInfos.Count; i++)
			{
				if (HotspotInfos[i].HotspotViewId == hotspotViewId)
				{
					if (state == HotspotState.None)
					{
						HotspotInfos.RemoveAt(i);
					}
					else
					{
						HotspotInfos[i].State = state;
						HotspotInfos[i].WalkerType = walkerType;
						HotspotInfos[i].Count = count;
						HotspotInfos[i].DefensiveMode = defensiveMode;
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				HotspotInfos.Add(new HotspotInfo
				{
					HotspotViewId = hotspotViewId,
					State = state,
					WalkerType = walkerType,
					Count = count,
					SliceViewId = sliceViewId,
					DefensiveMode = defensiveMode
				});
			}
			NotifyChange("HotspotInfoChanged", hotspotViewId);
		}

		public int CanAffordHotspotModifiaction(string sliceViewId, string hotspotViewId, HotspotState state, WalkerType walkerType, int count)
		{
			if (state == HotspotState.None)
			{
				return 0;
			}
			string selectedOutpostTemplateDefinitionId = base.manager.Player.SelectedOutpostTemplateDefinitionId;
			SliceConfiguration sliceConfigById = GetSliceConfigById(sliceViewId);
			OutpostTemplateDefinition outpostTemplateDefinition = base.manager.GameEconomyData.GetOutpostTemplateDefinition(selectedOutpostTemplateDefinitionId);
			int deploymentCostForHotspot = GetDeploymentCostForHotspot(state, walkerType);
			deploymentCostForHotspot *= count;
			int num = 0;
			for (int i = 0; i < HotspotInfos.Count; i++)
			{
				if (HotspotInfos[i].HotspotViewId == hotspotViewId)
				{
					num = GetDeploymentCostForHotspot(HotspotInfos[i].State, HotspotInfos[i].WalkerType);
					num *= HotspotInfos[i].Count;
					break;
				}
			}
			int totalUsedDeploymentForSlice = GetTotalUsedDeploymentForSlice(sliceViewId);
			int maxDeploymentForSlice = GetMaxDeploymentForSlice(sliceConfigById.Position, outpostTemplateDefinition);
			int num2 = Math.Max(0, totalUsedDeploymentForSlice - num + deploymentCostForHotspot);
			return Math.Max(0, num2 - maxDeploymentForSlice);
		}

		public bool HasExceededTotalType(OutpostModel outpostModel, WalkerType walkerType)
		{
			int totalWalkersAssigned = GetTotalWalkersAssigned(walkerType);
			OutpostWalkerModel walkerModel = outpostModel.GetWalkerModel(walkerType.ToString());
			if (walkerModel != null)
			{
				return totalWalkersAssigned >= walkerModel.Amount;
			}
			return false;
		}

		public string GetSliceViewIdForHotspot(RunLocationModel outpostTemplateModel, string hotspotViewId)
		{
			foreach (OutpostSliceModel outpostSlice in outpostTemplateModel.Missions[0].OutpostSlices)
			{
				foreach (TWDModelObject model in outpostSlice.Models)
				{
					if (model is OutpostHotspotModel outpostHotspotModel && outpostHotspotModel.ViewId == hotspotViewId)
					{
						return outpostSlice.ViewId;
					}
				}
			}
			return null;
		}

		public int GetTotalUsedDeploymentForSlice(string sliceViewId)
		{
			int num = 0;
			int num2 = 0;
			foreach (HotspotInfo hotspotInfo in HotspotInfos)
			{
				if (hotspotInfo.SliceViewId == sliceViewId)
				{
					num2 = GetDeploymentCostForHotspot(hotspotInfo.State, hotspotInfo.WalkerType);
					num2 *= hotspotInfo.Count;
					num += num2;
				}
			}
			return num;
		}

		public int GetDeploymentCostForHotspot(HotspotState hotspotState, WalkerType walkerType)
		{
			switch (hotspotState)
			{
			case HotspotState.DefenderSpawn_0:
			case HotspotState.DefenderSpawn_1:
			case HotspotState.DefenderSpawn_2:
				return base.manager.GameEconomyData.ConfigData.OutpostSurvivorDeploymentCost;
			case HotspotState.Flag:
				return base.manager.GameEconomyData.ConfigData.OutpostFlagDeploymentCost;
			case HotspotState.ResourceContainer:
				return base.manager.GameEconomyData.ConfigData.OutpostResourceContainerDeploymentCost;
			case HotspotState.Walker:
				return walkerType switch
				{
					WalkerType.WalkerArmored => base.manager.GameEconomyData.ConfigData.OutpostWalkerArmoredDeploymentCost, 
					WalkerType.WalkerExplosive => base.manager.GameEconomyData.ConfigData.OutpostWalkerNormalDeploymentCost, 
					WalkerType.WalkerNormal => base.manager.GameEconomyData.ConfigData.OutpostWalkerNormalDeploymentCost, 
					WalkerType.WalkerTank => base.manager.GameEconomyData.ConfigData.OutpostWalkerTankDeploymentCost, 
					_ => base.manager.GameEconomyData.ConfigData.OutpostWalkerNormalDeploymentCost, 
				};
			default:
				return 0;
			}
		}

		public int GetMaxDeploymentForSlice(SlicePosition position, OutpostTemplateDefinition template)
		{
			if (template != null)
			{
				switch (position)
				{
				case SlicePosition.First:
					return template.FirstSliceDeploymentPoints;
				case SlicePosition.Second:
					return template.SecondSliceDeploymentPoints;
				case SlicePosition.Third:
					return template.ThirdSliceDeploymentPoints;
				}
			}
			return 0;
		}

		public int CalculateDeploymentPointsLeftForSlice(string SliceViewId)
		{
			string selectedOutpostTemplateDefinitionId = base.manager.Player.SelectedOutpostTemplateDefinitionId;
			OutpostTemplateDefinition outpostTemplateDefinition = base.manager.GameEconomyData.GetOutpostTemplateDefinition(selectedOutpostTemplateDefinitionId);
			if (outpostTemplateDefinition == null)
			{
				base.Debug.LogError("Could not find template '" + selectedOutpostTemplateDefinitionId + "'");
				return 0;
			}
			int result = 0;
			if (outpostTemplateDefinition != null)
			{
				SliceConfiguration sliceConfigById = GetSliceConfigById(SliceViewId);
				if (sliceConfigById != null)
				{
					int maxDeploymentForSlice = GetMaxDeploymentForSlice(sliceConfigById.Position, outpostTemplateDefinition);
					int totalUsedDeploymentForSlice = GetTotalUsedDeploymentForSlice(SliceViewId);
					result = UtilsMath.Max(0, maxDeploymentForSlice - totalUsedDeploymentForSlice);
				}
			}
			return result;
		}

		public int GetTotalWalkersAssigned(WalkerType walkerType)
		{
			int num = 0;
			for (int i = 0; i < HotspotInfos.Count; i++)
			{
				HotspotInfo hotspotInfo = HotspotInfos[i];
				if (hotspotInfo.IsWalkerSpawn && hotspotInfo.WalkerType == walkerType)
				{
					num++;
				}
			}
			return num;
		}

		public void RemoveDefenderHotspots()
		{
			List<HotspotInfo> list = new List<HotspotInfo>();
			foreach (HotspotInfo hotspotInfo in HotspotInfos)
			{
				if (hotspotInfo.IsDefenderSpawn)
				{
					list.Add(hotspotInfo);
				}
			}
			foreach (HotspotInfo item in list)
			{
				HotspotInfos.Remove(item);
			}
		}

		public void RemoveWalkerHotspots()
		{
			List<HotspotInfo> list = new List<HotspotInfo>();
			foreach (HotspotInfo hotspotInfo in HotspotInfos)
			{
				if (hotspotInfo.IsWalkerSpawn)
				{
					list.Add(hotspotInfo);
				}
			}
			foreach (HotspotInfo item in list)
			{
				HotspotInfos.Remove(item);
			}
		}

		public void ClearHotspotInfo(string sliceViewId)
		{
			List<HotspotInfo> list = new List<HotspotInfo>();
			foreach (HotspotInfo hotspotInfo in HotspotInfos)
			{
				if (hotspotInfo.SliceViewId == sliceViewId)
				{
					list.Add(hotspotInfo);
				}
			}
			foreach (HotspotInfo item in list)
			{
				HotspotInfos.Remove(item);
			}
		}

		public void ClearAllHotspotInfos()
		{
			HotspotInfos.Clear();
		}

		public void SetSlice(SlicePosition slicePosition, string viewId)
		{
			bool flag = false;
			for (int i = 0; i < ChosenSlices.Count; i++)
			{
				if (ChosenSlices[i].Position == slicePosition)
				{
					ChosenSlices[i].ViewId = viewId;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ChosenSlices.Add(new SliceConfiguration
				{
					Position = slicePosition,
					ViewId = viewId
				});
			}
			NotifyChange("ChosenSliceChanged", slicePosition);
		}

		public bool HasChosenSlice(string viewId)
		{
			for (int i = 0; i < ChosenSlices.Count; i++)
			{
				if (ChosenSlices[i].ViewId == viewId)
				{
					return true;
				}
			}
			return false;
		}

		public HotspotInfo GetHotspotInfoForDefender(HotspotState state)
		{
			foreach (HotspotInfo hotspotInfo in HotspotInfos)
			{
				if (hotspotInfo.State == state && HasChosenSlice(hotspotInfo.SliceViewId))
				{
					return hotspotInfo;
				}
			}
			return null;
		}

		public HotspotInfo FindHotspotInfo(string viewId)
		{
			foreach (HotspotInfo hotspotInfo in HotspotInfos)
			{
				if (hotspotInfo.HotspotViewId == viewId)
				{
					return hotspotInfo;
				}
			}
			return null;
		}

		public RunLocationModel GenerateOutpost(RunLocationModel outpostTemplateModel)
		{
			RunLocationModel runLocationModel = new RunLocationModel();
			IMessageSerializer messageSerializer2;
			if (base.manager == null)
			{
				IMessageSerializer messageSerializer = new MessageSerializer();
				messageSerializer2 = messageSerializer;
			}
			else
			{
				messageSerializer2 = base.manager.GetMessageSerializer();
			}
			string value = messageSerializer2.SerializeObject(outpostTemplateModel);
			runLocationModel = messageSerializer2.DeserializeObject<RunLocationModel>(value);
			if (runLocationModel.Missions.Count > 0)
			{
				List<OutpostSliceModel> list = new List<OutpostSliceModel>();
				foreach (OutpostSliceModel outpostSlice in runLocationModel.Missions[0].OutpostSlices)
				{
					if (!HasChosenSlice(outpostSlice.ViewId))
					{
						list.Add(outpostSlice);
						continue;
					}
					List<TWDModelObject> list2 = new List<TWDModelObject>();
					foreach (TWDModelObject model in outpostSlice.Models)
					{
						if (!(model is OutpostHotspotModel outpostHotspotModel))
						{
							continue;
						}
						HotspotInfo hotspotInfo = FindHotspotInfo(outpostHotspotModel.ViewId);
						if (hotspotInfo == null || (!hotspotInfo.IsDefenderSpawn && outpostHotspotModel.DefenderModel != null))
						{
							list2.Add(outpostHotspotModel.DefenderModel);
						}
						if (hotspotInfo == null || (!hotspotInfo.IsWalkerSpawn && outpostHotspotModel.SpawnPointModel != null))
						{
							list2.Add(outpostHotspotModel.SpawnPointModel);
						}
						if (hotspotInfo == null || (hotspotInfo.State != HotspotState.Flag && outpostHotspotModel.FlagModels != null))
						{
							list2.AddRange(outpostHotspotModel.FlagModels);
						}
						if (hotspotInfo == null || (hotspotInfo.State != HotspotState.ResourceContainer && outpostHotspotModel.ResourceContainerModels != null))
						{
							list2.AddRange(outpostHotspotModel.ResourceContainerModels);
						}
						if (hotspotInfo != null)
						{
							if (hotspotInfo.IsDefenderSpawn)
							{
								outpostHotspotModel.DefenderModel.DefenderIndex = hotspotInfo.GetDefenderIndex();
								outpostHotspotModel.DefenderModel.DefensiveMode = hotspotInfo.DefensiveMode;
								outpostHotspotModel.DefenderModel.State = PvPDefenderSpawnState.Enabled;
							}
							else if (hotspotInfo.IsWalkerSpawn)
							{
								outpostHotspotModel.SpawnPointModel.TotalSpawnCount = hotspotInfo.Count;
								outpostHotspotModel.SpawnPointModel.SpawnCountPerAction = hotspotInfo.Count;
								(outpostHotspotModel.SpawnPointModel as WalkerSpawnPointModel).OverrideWalkerType = hotspotInfo.WalkerType;
							}
						}
						list2.Add(outpostHotspotModel);
					}
					foreach (TWDModelObject item in list2)
					{
						outpostSlice.Models.Remove(item);
					}
				}
				foreach (OutpostSliceModel item2 in list)
				{
					runLocationModel.Missions[0].OutpostSlices.Remove(item2);
				}
			}
			List<CombatColliderModel> list3 = new List<CombatColliderModel>();
			if (runLocationModel.Missions.Count > 0)
			{
				foreach (TWDModelObject model2 in runLocationModel.Missions[0].Models)
				{
					if (model2 is CombatColliderModel { IsDynamic: not false } combatColliderModel)
					{
						list3.Add(combatColliderModel);
					}
				}
				foreach (OutpostSliceModel outpostSlice2 in runLocationModel.Missions[0].OutpostSlices)
				{
					foreach (TWDModelObject model3 in outpostSlice2.Models)
					{
						if (model3 is CombatColliderModel { IsDynamic: not false } combatColliderModel2)
						{
							list3.Add(combatColliderModel2);
						}
					}
				}
			}
			int num = 0;
			int num2 = 0;
			foreach (CombatColliderModel item3 in list3)
			{
				if (item3.BlockVision)
				{
					num++;
				}
				if (item3.BlockMovement)
				{
					num2++;
				}
			}
			BuildCombinedColliderData(runLocationModel);
			return runLocationModel;
		}

		protected void BuildCombinedColliderData(RunLocationModel runLocation)
		{
			MissionModel missionModel = runLocation.Missions[0];
			base.manager.Debug.Log("Exporting movement with " + runLocation.Missions.Count + " missions and " + missionModel.OutpostSlices.Count + " slices");
			IEnumerable<CombatColliderModel> source = from x in runLocation.Models
				where x is CombatColliderModel
				select (CombatColliderModel)x;
			List<CombatColliderModel> combinedMovementColliders = source.Where((CombatColliderModel x) => x.BlockMovement && x.IsDynamic).ToList();
			List<CombatColliderModel> combinedVisionColliders = source.Where((CombatColliderModel x) => x.BlockVision && x.IsDynamic).ToList();
			base.manager.Debug.Log("RunLoc Has " + combinedVisionColliders.Count + " / " + combinedMovementColliders.Count + " dynamic colliders");
			foreach (OutpostSliceModel outpostSlice in missionModel.OutpostSlices)
			{
				IEnumerable<CombatColliderModel> source2 = from x in outpostSlice.Models
					where x is CombatColliderModel
					select (CombatColliderModel)x;
				List<CombatColliderModel> list = source2.Where((CombatColliderModel x) => x.BlockMovement && x.IsDynamic).ToList();
				List<CombatColliderModel> list2 = source2.Where((CombatColliderModel x) => x.BlockVision && x.IsDynamic).ToList();
				base.manager.Debug.Log("Slice Has " + list2.Count + " / " + list.Count + " dynamic colliders");
				combinedMovementColliders.AddRange(list.Where((CombatColliderModel x) => !combinedMovementColliders.Contains(x)));
				combinedVisionColliders.AddRange(list2.Where((CombatColliderModel x) => !combinedVisionColliders.Contains(x)));
			}
			combinedMovementColliders.StableSort((CombatColliderModel a, CombatColliderModel b) => a.ViewId.CompareTo(b.ViewId));
			combinedVisionColliders.StableSort((CombatColliderModel a, CombatColliderModel b) => a.ViewId.CompareTo(b.ViewId));
			base.manager.Debug.Log("Combined Has  " + combinedVisionColliders.Count + " / " + combinedMovementColliders.Count + " dynamic colliders");
			GridColliderData gridColliderData = new GridColliderData(runLocation.Grid, combinedVisionColliders.Count, combinedMovementColliders.Count);
			foreach (OutpostSliceModel outpostSlice2 in missionModel.OutpostSlices)
			{
				IEnumerable<CombatColliderModel> source3 = from x in outpostSlice2.Models
					where x is CombatColliderModel
					select (CombatColliderModel)x;
				List<CombatColliderModel> list3 = source3.Where((CombatColliderModel x) => x.BlockMovement && x.IsDynamic).ToList();
				list3.Sort((CombatColliderModel a, CombatColliderModel b) => a.ViewId.CompareTo(b.ViewId));
				List<CombatColliderModel> list4 = source3.Where((CombatColliderModel x) => x.BlockVision && x.IsDynamic).ToList();
				list4.Sort((CombatColliderModel a, CombatColliderModel b) => a.ViewId.CompareTo(b.ViewId));
				GridColliderData other = new GridColliderData(runLocation.Grid, list4.Count, list3.Count, outpostSlice2.ExportedVisibility, outpostSlice2.ExportedMovement);
				int[] movementBitMapping = list3.Select((CombatColliderModel x) => combinedMovementColliders.IndexOf(x)).ToArray();
				int[] visibilityBitMapping = list4.Select((CombatColliderModel x) => combinedVisionColliders.IndexOf(x)).ToArray();
				gridColliderData.Combine(other, visibilityBitMapping, movementBitMapping);
				outpostSlice2.ExportedVisibility = null;
				outpostSlice2.ExportedMovement = null;
			}
			runLocation.ExportedVisibility = gridColliderData.GetVisibilityAsString();
			runLocation.ExportedMovement = gridColliderData.GetMovementAsString();
		}

		private static BitArray BitArrayFromBase64String(string s)
		{
			return new BitArray(Convert.FromBase64String(s));
		}

		public static string BitArrayToBase64String(BitArray a)
		{
			byte[] array = new byte[a.Length / 8 + 1];
			a.CopyTo(array, 0);
			return Convert.ToBase64String(array);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
