using System.Linq;
using BaseModel;

namespace TWDModel
{
	public static class SupportHelpers
	{
		public const int MaxEquippedSupportCount = 3;

		public static bool AreSupportsFixed(MapMissionModel mapMissionModel)
		{
			return mapMissionModel?.IsFixedSurvivorSeasonMission ?? false;
		}

		public static SupportModel GetMissionSupport(MapMissionModel mapMissionModel, PlayerModel player, int equippedIndex)
		{
			if (AreSupportsFixed(mapMissionModel))
			{
				return mapMissionModel.GetFixedSupport(equippedIndex);
			}
			string text = player.EquippedSupportIds[equippedIndex];
			if (!string.IsNullOrEmpty(text))
			{
				return player.GetSupportModel(text);
			}
			return null;
		}

		public static SupportTalentNodeTrunkModel GetFirstTalentNodeByTrunkNodes(ModelList<SupportTalentNodeTrunkModel> trunkNodes)
		{
			foreach (SupportTalentNodeTrunkModel trunkNode in trunkNodes)
			{
				if (trunkNode.GetRequireTrunkId() == 0)
				{
					return trunkNode;
				}
			}
			return null;
		}

		public static int GetUpgradableSupportCount(this PlayerModel playerModel)
		{
			return playerModel.SupportModels.Count((SupportModel model) => model.CanUpgrade);
		}
	}
}
