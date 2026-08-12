using TWDModel;
using UnityEngine;

public class CollectSurvivalRewardButton : MonoBehaviourExtended
{
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
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		LootEntry lootEntry = null;
		if (weeklySurvivalModel != null)
		{
			lootEntry = weeklySurvivalModel.FirstCollectablePersonalReward;
			if (weeklySurvivalModel.Rewards != null && button != null)
			{
				button.SetContentToLabelOne(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Social.WeeklySurvival.UnclaimedRewards{Amount}", weeklySurvivalModel.Rewards.Count));
			}
		}
		Helpers.GameObjectSetActive(button, lootEntry != null);
	}

	private void OnButtonClicked(UIButtonExtended button)
	{
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel.CanCollectRewards)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(weeklySurvivalModel);
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
