using System.Linq;

namespace TWDModel
{
	public static class TeamPresetHelpers
	{
		public static bool AreEquivalent(this ITeamPresetData teamPresetData, ITeamPresetData other)
		{
			if (teamPresetData.Survivors.Length != other.Survivors.Length || teamPresetData.Supports.Length != other.Supports.Length)
			{
				return false;
			}
			for (int i = 0; i < teamPresetData.Survivors.Length; i++)
			{
				if (teamPresetData.Survivors[i] != other.Survivors[i])
				{
					return false;
				}
			}
			for (int j = 0; j < teamPresetData.Supports.Length; j++)
			{
				if (teamPresetData.Supports[j] != other.Supports[j])
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsValid(this ITeamPresetData presetData)
		{
			if (presetData != null && presetData.Survivors.Length == 3)
			{
				return presetData.Survivors.All((SurvivorModel survivor) => survivor != null);
			}
			return false;
		}

		public static bool IsPresetSlotUnlocked(PlayerModel player, int index)
		{
			return player.CouncilLevel >= player.gameEconomyData.TeamPresets[index].RequiredLevel;
		}

		public static bool IsFeatureUnlocked(PlayerModel player)
		{
			return IsPresetSlotUnlocked(player, 0);
		}
	}
}
