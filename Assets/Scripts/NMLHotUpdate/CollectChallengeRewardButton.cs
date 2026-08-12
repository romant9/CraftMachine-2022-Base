using TWDModel;
using UnityEngine;

public class CollectChallengeRewardButton : MonoBehaviourExtended
{
	[SerializeField]
	private bool collectPersonal;

	[SerializeField]
	private bool collectGuild;

	[SerializeField]
	private UIButtonWithLabelAndIcon button;

	public virtual void OnEnable()
	{
		UpdateUI();
		if (button != null)
		{
			button.SetClickCallback(OnButtonClicked);
		}
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public virtual void OnDisable()
	{
		if (button != null)
		{
			button.Clear();
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public virtual void UpdateUI()
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		LootEntry lootEntry = null;
		if (weeklyChallengeModel != null)
		{
			if (collectPersonal)
			{
				lootEntry = weeklyChallengeModel.FirstCollectablePersonalReward;
			}
			if (collectGuild && lootEntry == null)
			{
				lootEntry = weeklyChallengeModel.FirstCollectableGuildReward;
			}
			if (weeklyChallengeModel.Rewards != null && button != null)
			{
				button.SetContentToLabelOne(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Social.WeeklyChallenge.UnclaimedRewards{Amount}", weeklyChallengeModel.Rewards.Count));
			}
		}
		Helpers.GameObjectSetActive(button, value: false);
	}

	private void OnButtonClicked(UIButtonExtended button)
	{
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel.CanCollectRewards)
		{
			if (!WeeklyChallengeRewardListPopup.TryOpenForGuildGifts())
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(weeklyChallengeModel);
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/quest_complete");
			Helpers.GameObjectSetActive(button, value: false);
		}
	}

	private void OnUIEvent(string type, object parameter = null)
	{
		if (type == "OnPopUpClose")
		{
			UpdateUI();
		}
	}
}
