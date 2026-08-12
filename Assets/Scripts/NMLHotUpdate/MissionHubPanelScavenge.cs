using System.Collections.Generic;
using TWDModel;

public class MissionHubPanelScavenge : MissionHubGameModePanel
{
	public override void Init(MissionHubContent content, HUDElement parent)
	{
		base.Init(content, parent);
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		if (GameManager.Instance.playerModel.Tutorial.HasCompletedPart("EndTutorial"))
		{
			base.ButtonMainClicked(button);
			EventManager.NotifyClick("Scavenge");
		}
	}

	protected override void OpenDialog()
	{
		MissionHubNavigation.OpenScavenge();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.ScavengeUnlockAfterTutorial"));
		CheckLockedState();
		List<DropEventDefinition.DropEventTag> list = new List<DropEventDefinition.DropEventTag>();
		GrindButtonDefinition[] grindButtonDefinitions = GameManager.Instance.gameEconomyData.GrindButtonDefinitions;
		if (grindButtonDefinitions != null)
		{
			for (int i = 0; i < grindButtonDefinitions.Length; i++)
			{
				if (grindButtonDefinitions[i] != null && !list.Contains(grindButtonDefinitions[i].LootTag))
				{
					list.Add(grindButtonDefinitions[i].LootTag);
				}
			}
		}
		PreviewRewardsList(list);
		Helpers.GameObjectSetActive(unlockedEffect, FeatureUIHighlights.IsActive(FeatureUIHighlights.FeaturesIds.ScavengeModeUnlocked));
	}

	public override void CheckLockedState()
	{
		UpdateLockedState(!GameManager.Instance.playerModel.Tutorial.HasCompletedPart("EndTutorial"));
	}
}
