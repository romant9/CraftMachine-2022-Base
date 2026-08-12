using TWDModel;
using UnityEngine;

public class WeeklySurvivalRewardIcon : WaypointIconBase
{
	[SerializeField]
	private UISprite iconSprite;

	[SerializeField]
	private UIButtonExtended iconButton;

	[SerializeField]
	private int playTweenGroupOnClaim = 8;

	private WeeklySurvivalReward rewardReference;

	private SurvivalDifficulty difficultyOfReward;

	public void SetReward(WeeklySurvivalReward reward, int currentCompletionCount, SurvivalDifficulty difficulty)
	{
		if (IsNotNull(reward))
		{
			if (iconSprite != null)
			{
				iconSprite.alpha = 1f;
			}
			rewardReference = reward;
			difficultyOfReward = difficulty;
			UpdateUI();
		}
		else
		{
			Clear();
		}
	}

	public void UpdateUI()
	{
		if (rewardReference != null && rewardReference.RewardEntries != null && rewardReference.RewardEntries.Length > (int)difficultyOfReward && rewardReference.RewardEntries[(int)difficultyOfReward] != null)
		{
			IReward rewardAt = rewardReference.RewardEntries[(int)difficultyOfReward].GetRewardAt(0);
			if (rewardAt != null)
			{
				string spriteName = "";
				HelpersGfx.GetIconNameForIReward(rewardAt, out spriteName, null, null, null);
				HelpersUI.SetSprite(iconSprite, spriteName);
			}
		}
	}

	public override void Show()
	{
		base.Show();
		Helpers.GameObjectSetActive(iconSprite, value: true);
	}

	public override void Hide()
	{
		base.Hide();
		Helpers.GameObjectSetActive(iconSprite, value: false);
	}

	public override void CompleteTrigger()
	{
		base.CompleteTrigger();
		Show();
		TweenManager.PlayTweenGroup(base.gameObject, playTweenGroupOnClaim, forward: true, TweenClaimDone);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		AddListeners();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		RemoveListeners();
	}

	public override void Clear()
	{
		base.Clear();
		rewardReference = null;
		RemoveListeners();
	}

	private void TweenClaimDone()
	{
		Helpers.GameObjectSetActive(iconSprite, value: false);
	}

	private void OnClickIcon(UIButtonExtended button)
	{
	}

	private void AddListeners()
	{
		if (iconButton != null)
		{
			iconButton.SetClickCallback(OnClickIcon);
		}
	}

	private void RemoveListeners()
	{
		if (iconButton != null)
		{
			iconButton.Clear();
		}
	}
}
