using TWDModel;
using UnityEngine;

public static class TeamPresetClientHelpers
{
	public static void SavePreset(int index, ITeamPresetData presetData)
	{
		if (index >= 0 && presetData != null)
		{
			Helpers.ExecuteCommand(new SaveTeamPresetCommand(index, presetData));
		}
	}

	public static void LockTooltip(TeamPresetData[] presetData, GameObject button, int index)
	{
		TooltipManager.OpenTextBoxWithText(button, LocalizationManager.GetText("Popup.TeamSelection.TeamPreset.LockedTooltip", presetData[index].RequiredLevel));
	}
}
