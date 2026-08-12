using System;
using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class OutpostVersionMigration
	{
		private static bool FixRemovedHotspots(PlayerModel player, RunLocationModel outpostTemplateModel)
		{
			bool result = false;
			OutpostLevelModel levelModel = player.OutpostModel.StoredLevelModel;
			if (levelModel.HotspotInfos == null)
			{
				return false;
			}
			List<HotspotInfo> list = levelModel.HotspotInfos.ToList();
			Random random = new Random();
			for (int i = 0; i < list.Count; i++)
			{
				HotspotInfo hotspotInfo = list[i];
				OutpostSliceModel sliceModel = outpostTemplateModel.GetSliceModel(hotspotInfo.SliceViewId);
				if (sliceModel == null || sliceModel.GetHotspotModel(hotspotInfo.HotspotViewId) != null)
				{
					continue;
				}
				List<OutpostHotspotModel> list2 = (from x in sliceModel.GetHotspotModels()
					where !levelModel.HotspotInfos.Any((HotspotInfo info) => info.HotspotViewId == x.ViewId)
					select x).ToList();
				if (list2.Count != 0)
				{
					OutpostHotspotModel outpostHotspotModel = list2[random.Next() % list2.Count];
					levelModel.SetHotspotInfo(hotspotInfo.SliceViewId, outpostHotspotModel.ViewId, hotspotInfo.State, hotspotInfo.WalkerType, hotspotInfo.Count, hotspotInfo.DefensiveMode);
					levelModel.SetHotspotInfo(hotspotInfo.SliceViewId, hotspotInfo.HotspotViewId, HotspotState.None, WalkerType.WalkerNormal, 0, AIMode.None);
					result = true;
					player.manager.Debug.Log("FixRemovedHotspot: Replaced unknown " + hotspotInfo.State.ToString() + " hotspot " + hotspotInfo.SliceViewId + " with " + outpostHotspotModel.ViewId);
				}
			}
			return result;
		}

		public static bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			string text = null;
			if (player != null && player.HasValidOutpost && player.OutpostModel.PublishedLevelDataVersion != manager.GameEconomyData.ConfigData.OutpostLevelDataVersion)
			{
				if (manager.ServerService != null)
				{
					string playerJsonByPlayerId = manager.ServerService.GetPlayerJsonByPlayerId(player.OutpostModel.StoredLevelModel.BaseRunLocationID);
					if (playerJsonByPlayerId != null)
					{
						RunLocationModel runLocationModel = manager.GetMessageSerializer().DeserializeObject<RunLocationModel>(playerJsonByPlayerId);
						if (runLocationModel != null)
						{
							player.SetOutpostTemplateByMissionId(player.OutpostModel.StoredLevelModel.BaseRunLocationID, runLocationModel);
							if (FixRemovedHotspots(player, runLocationModel))
							{
								manager.SetModelHotfixApplied();
							}
						}
						else
						{
							text = "FixPublishedOutpostLevelData() -> RunLocationModel could not be deserialized, cannot load outpost template!";
						}
					}
					else
					{
						text = "FixPublishedOutpostLevelData() -> Could not get level dictionary json from ServerService with ID = '" + player.OutpostModel.StoredLevelModel.BaseRunLocationID + "', cannot load outpost template!";
					}
				}
				else
				{
					text = "FixPublishedOutpostLevelData() -> No ServerService, cannot load outpost template!";
				}
				if (PublishOutpostCommand.PublishOutpost(manager) == TWDModelResult.OK)
				{
					return true;
				}
				text = "FixPublishedOutpostLevelData() -> Publish failed!";
				if (text != null)
				{
					manager.Debug.LogWarning(text);
				}
				else
				{
					manager.Debug.Log("FixPublishedOutpostLevelData() -> Player Outpost level data updated successfully!");
				}
			}
			return false;
		}
	}
}
