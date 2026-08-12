using TWDModel;

public class MissionHubPanelOutpost : MissionHubGameModePanel
{
	public override void Start()
	{
		base.Start();
		HelpersUI.SetContentToLabel(lockedLabel, LocalizationManager.GetText("Popup.MissionHub.OutpostUnlockAtLevel{CouncilLevel}", GameManager.Instance.gameEconomyData.ConfigData.OutpostUnlockAtCouncilLevel));
	}

	public override void Update()
	{
		base.Update();
		Helpers.GameObjectSetActive(timerLabel, !base.isLocked && GameManager.Instance.playerModel.OutpostTutorialState == OutpostTutorialState.Done && GameManager.Instance.playerModel.HasValidOutpost);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		CheckLockedState();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		IReward reward = null;
		if (playerModel != null && !base.isLocked)
		{
			OutpostSeason outpostSeasonById = GameManager.Instance.gameEconomyData.GetOutpostSeasonById(playerModel.CurrentOutpostSeasonId);
			if (outpostSeasonById != null)
			{
				timeLabelLocalisation = LocalizationManager.GetText("Popup.MissionHub.OutpostSeasonEndsIn");
				OutpostTier outpostInfluenceTier = GameManager.Instance.gameEconomyData.GetOutpostInfluenceTier(GameManager.Instance.playerModel.RankingScore, outpostSeasonById.TierSetId);
				gameModeTimeLeft = outpostSeasonById.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
				if (outpostInfluenceTier.GetRewards().RewardsList.Count > 0)
				{
					reward = outpostInfluenceTier.GetRewards().RewardsList[0];
				}
			}
			else
			{
				timeLabelLocalisation = LocalizationManager.GetText("Popup.MissionHub.OutpostSeasonStartsIn");
				outpostSeasonById = GameManager.Instance.gameEconomyData.GetNextOutpostSeason(GameManager.Instance.playerModel.UtcTimeStamp);
				if (outpostSeasonById != null)
				{
					gameModeTimeLeft = outpostSeasonById.StartTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
				}
			}
		}
		if (!base.isLocked)
		{
			PreviewSingleReward(reward);
		}
		else
		{
			PreviewSingleReward(null);
		}
		if (progressBar != null)
		{
			progressBar.UpdateUI();
		}
	}

	protected override void ButtonMainClicked(UIButtonExtended button)
	{
		if (!TutorialView.Instance.Running || TutorialView.Instance.IsSuggesting || TutorialView.Instance.Model == null || !(TutorialView.Instance.Model.CurrentPartId != "OutpostMode"))
		{
			base.ButtonMainClicked(button);
			EventManager.NotifyClick("Outpost");
		}
	}

	protected override void OpenDialog()
	{
		MissionHubNavigation.TryOpenOutpost();
	}

	public override void CheckLockedState()
	{
		UpdateLockedState(!GameManager.Instance.playerModel.IsOutpostUnlocked);
	}
}
