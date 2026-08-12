using System;

namespace TWDModel
{
	[Serializable]
	public class GrindButtonDefinition
	{
		public enum Difficulty
		{
			None = 0,
			Normal = 1,
			Hard = 2,
			Count = 3
		}

		public int Id;

		public int DisplayOrder;

		public DropEventDefinition.DropEventTag LootTag;

		public DropEventDefinition.DropEventContext DropContext;

		public Difficulty GrindDifficulty;

		public int MissionLevelOffset;

		public int LegendaryRarityModifier;

		public string PrefabName;

		public string TitleLocalizationKey;

		public string IconSpriteOverride;

		public int GetMissionLevel(PlayerModel playerModel)
		{
			int result = 1;
			if (playerModel != null)
			{
				int num = playerModel.SurvivorContainer.GetHighestLevelSurvivor() * 3;
				int num2 = Math.Max(playerModel.SurvivorContainer.GetHighestSurvivorRarity() - 4, 0) * LegendaryRarityModifier;
				result = num + num2 + MissionLevelOffset;
				int val = Math.Max(playerModel.gameEconomyData.GetMaxAvailableDifficulty(), 0);
				result = Math.Min(result, val);
				result = Math.Max(result, 1);
			}
			return result;
		}
	}
}
