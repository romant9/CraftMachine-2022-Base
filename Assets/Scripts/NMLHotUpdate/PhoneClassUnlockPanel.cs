using TWDModel;
using UnityEngine;

public class PhoneClassUnlockPanel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel MainLabel;

	[SerializeField]
	private UITexture SurvivorTexture;

	[SerializeField]
	private UIButtonExtended GoButton;

	private void Start()
	{
		if (!(GameManager.Instance == null) && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.SurvivorContainer != null && GameManager.Instance.playerModel.SurvivorContainer.StoryTeller != null && GoButton != null)
		{
			GoButton.isEnabled = GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.FirstQuestAccepted;
		}
	}

	public void AddClickListener(UIButtonExtended.OnClickCallback callback, string buttonId = "")
	{
		if (GoButton != null)
		{
			GoButton.id = buttonId;
			GoButton.SetClickCallback(callback);
		}
	}

	public void RemoveListeners()
	{
		if (GoButton != null)
		{
			GoButton.Clear();
		}
	}

	public void UpdateUI()
	{
		if (GameManager.Instance == null || GameManager.Instance.modelManager == null)
		{
			return;
		}
		QuestDefinition nextUnlockSurvivorClassQuest = QuestUtils.GetNextUnlockSurvivorClassQuest(GameManager.Instance.modelManager);
		if (nextUnlockSurvivorClassQuest != null)
		{
			Rewards rewards = nextUnlockSurvivorClassQuest.GetRewards();
			if (rewards != null)
			{
				RewardSurvivorClass survivorClassReward = rewards.GetSurvivorClassReward();
				if (survivorClassReward != null)
				{
					SurvivorClass survivorClass = survivorClassReward.SurvivorClass;
					if (survivorClass != SurvivorClass.None)
					{
						MapMissionGroupModel unlockedEpisode = nextUnlockSurvivorClassQuest.GetUnlockedEpisode(GameManager.Instance.modelManager);
						if (unlockedEpisode != null)
						{
							string text = LocalizationManager.GetText("Popup.StartPhoneCall.UnlockClass{EpisodeName}{ClassName}", HelpersLocalization.GetEpisodeTitle(unlockedEpisode), HelpersLocalization.GetSurvivorClassName(survivorClass));
							HelpersUI.SetContentToLabel(MainLabel, text);
							HelpersGfx.SetSurvivorClassMaterial(SurvivorTexture, survivorClass);
							Helpers.GameObjectSetActive(base.gameObject, value: true);
							return;
						}
					}
				}
			}
		}
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}
}
