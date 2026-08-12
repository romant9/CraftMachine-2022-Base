using TWDModel;
using UnityEngine;

public class FeatureLockedPopup : HUDElement
{
	public enum FeatureType
	{
		Challenge = 0,
		Survival = 1,
		Outpost = 2,
		OutpostEdit = 3,
		GuildBattle = 4,
		EndlessMode = 5
	}

	[Header("Dynamic Content Labels")]
	[SerializeField]
	private UILabel challengeLockedCouncilLevelLabel;

	[SerializeField]
	private UILabel survivalLockedCouncilLevelLabel;

	[SerializeField]
	private UILabel outpostLockedCouncilLevelLabel;

	[SerializeField]
	private UILabel outpostEditLockedBuilingLevelLabel;

	[SerializeField]
	private UILabel guildBattleLockedCouncilLevelLabel;

	[SerializeField]
	private UILabel endlessModeLockedCouncilLevelLabel;

	[SerializeField]
	private GameObject challengeLockedContainer;

	[SerializeField]
	private GameObject challengeUnlockedContainer;

	[SerializeField]
	private GameObject survivalLockedContainer;

	[SerializeField]
	private GameObject survivalUnlockedContainer;

	[SerializeField]
	private GameObject outpostLockedContainer;

	[SerializeField]
	private GameObject outpostUnlockedContainer;

	[SerializeField]
	private GameObject outpostEditUnlockedContainer;

	[SerializeField]
	private GameObject outpostEditLockedContainer;

	[SerializeField]
	private GameObject guildBattleUnlockedContainer;

	[SerializeField]
	private GameObject guildBattleLockedContainer;

	[SerializeField]
	private GameObject endlessModeUnlockedContainer;

	[SerializeField]
	private GameObject endlessModeLockedContainer;

	private bool isClosing;

	public bool Locked { get; set; }

	public FeatureType Type { get; set; }

	public Callback Callback { get; set; }

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
		if (!isClosing)
		{
			isClosing = true;
			Callback?.Invoke();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(challengeLockedContainer, Type == FeatureType.Challenge && Locked);
		Helpers.GameObjectSetActive(challengeUnlockedContainer, Type == FeatureType.Challenge && !Locked);
		Helpers.GameObjectSetActive(survivalLockedContainer, Type == FeatureType.Survival && Locked);
		Helpers.GameObjectSetActive(survivalUnlockedContainer, Type == FeatureType.Survival && !Locked);
		Helpers.GameObjectSetActive(outpostLockedContainer, Type == FeatureType.Outpost && Locked);
		Helpers.GameObjectSetActive(outpostUnlockedContainer, Type == FeatureType.Outpost && !Locked);
		Helpers.GameObjectSetActive(outpostEditLockedContainer, Type == FeatureType.OutpostEdit && Locked);
		Helpers.GameObjectSetActive(outpostEditUnlockedContainer, Type == FeatureType.OutpostEdit && !Locked);
		Helpers.GameObjectSetActive(guildBattleUnlockedContainer, Type == FeatureType.GuildBattle && !Locked);
		Helpers.GameObjectSetActive(guildBattleLockedContainer, Type == FeatureType.GuildBattle && Locked);
		Helpers.GameObjectSetActive(endlessModeUnlockedContainer, Type == FeatureType.EndlessMode && !Locked);
		Helpers.GameObjectSetActive(endlessModeLockedContainer, Type == FeatureType.EndlessMode && Locked);
		if (Locked)
		{
			if (Type == FeatureType.Challenge)
			{
				if (WeeklyChallengeHelper.IsLockedByCouncilLevel())
				{
					int num = ((GameManager.Instance != null) ? GameManager.Instance.gameEconomyData.ConfigData.ChallengesUnlockAtCouncilLevel : (-1));
					string text = ((num == -1) ? "" : num.ToString());
					HelpersUI.SetContentToLabel(challengeLockedCouncilLevelLabel, LocalizationManager.GetText("Popup.FeatureLocked.CouncilLevelNeeded{Level}", text));
				}
				else if (WeeklyChallengeHelper.IsLockedByTutorial())
				{
					HelpersUI.SetContentToLabel(challengeLockedCouncilLevelLabel, LocalizationManager.GetText("Popup.FeatureLocked.TutorialEndBlockingChallenges"));
				}
			}
			else if (Type == FeatureType.Survival)
			{
				if (WeeklySurvivalHelper.IsLockedByCouncilLevel())
				{
					int num2 = ((GameManager.Instance != null) ? GameManager.Instance.gameEconomyData.ConfigData.SurvivalUnlockAtCouncilLevel : (-1));
					string text2 = ((num2 == -1) ? "" : num2.ToString());
					HelpersUI.SetContentToLabel(survivalLockedCouncilLevelLabel, LocalizationManager.GetText("Popup.FeatureLocked.CouncilLevelNeeded{Level}", text2));
				}
				else if (WeeklySurvivalHelper.IsLockedByTutorial())
				{
					HelpersUI.SetContentToLabel(survivalLockedCouncilLevelLabel, LocalizationManager.GetText("Popup.FeatureLocked.TutorialEndBlockingSurvival"));
				}
			}
			else if (Type == FeatureType.Outpost)
			{
				if (TutorialView.Instance.Running)
				{
					outpostLockedCouncilLevelLabel.text = LocalizationManager.GetText("Popup.FeatureLocked.TutorialRunning");
					return;
				}
				outpostLockedCouncilLevelLabel.text = LocalizationManager.GetText("Popup.FeatureLocked.CouncilLevelNeeded{Level}", GameManager.Instance.gameEconomyData.ConfigData.OutpostUnlockAtCouncilLevel);
			}
			else if (Type == FeatureType.OutpostEdit)
			{
				outpostEditLockedBuilingLevelLabel.text = LocalizationManager.GetText("Popup.FeatureLocked.Builing{Building}{Level}", HelpersLocalization.GetBuildingName("Outpost"), GameManager.Instance.gameEconomyData.ConfigData.OutpostUnlockEditingAtBuilingLevel);
			}
			else if (Type == FeatureType.GuildBattle)
			{
				int num3 = ((GameManager.Instance != null) ? GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarUnlockAtCouncilLevel : (-1));
				string text3 = ((num3 == -1) ? "" : num3.ToString());
				HelpersUI.SetContentToLabel(guildBattleLockedCouncilLevelLabel, LocalizationManager.GetText("Popup.FeatureLocked.CouncilLevelNeeded{Level}", text3));
			}
			else if (Type == FeatureType.EndlessMode)
			{
				int num4 = ((GameManager.Instance != null) ? EndlessModeHelpers.EndlessModeConfig.CouncilLockLevel : (-1));
				string text4 = ((num4 == -1) ? "" : num4.ToString());
				HelpersUI.SetContentToLabel(endlessModeLockedCouncilLevelLabel, LocalizationManager.GetText("Popup.FeatureLocked.CouncilLevelNeeded{Level}", text4));
			}
		}
		else if (Type == FeatureType.Challenge)
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.ChallengeUnlockedSeen"));
		}
		else if (Type == FeatureType.Survival)
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.SurvivalUnlockedSeen"));
		}
		else if (Type == FeatureType.Outpost)
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.ToggleOutpostUnlockedSeen"));
		}
		else if (Type == FeatureType.OutpostEdit)
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.ToggleOutpostEditUnlockedSeen"));
		}
		else if (Type == FeatureType.GuildBattle)
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.GuildBattleUnlockedSeen"));
		}
		else if (Type == FeatureType.EndlessMode)
		{
			Helpers.ExecuteCommand(new FeatureUnlockedSeenCommand("Toggle.EndlessModeUnlockedSeen"));
		}
	}

	public static void Open(FeatureType type, bool locked, Callback callback = null)
	{
		FeatureLockedPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LockedFeaturePopup) as FeatureLockedPopup;
		obj.Locked = locked;
		obj.Type = type;
		obj.Callback = callback;
		obj.Open();
	}
}
