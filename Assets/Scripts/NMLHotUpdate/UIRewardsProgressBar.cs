using System.Collections.Generic;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class UIRewardsProgressBar : MonoBehaviourExtended
{
	[SerializeField]
	private UIProgressBarWaypoints progressBar;

	[SerializeField]
	private WeeklyChallengeRewardIcon rewardIcon;

	private int rewardsPerBatchPersonal = 4;

	private int rewardsPerBatchGuild = 2;

	private List<List<WeeklyChallengeReward>> rewardsBatches = new List<List<WeeklyChallengeReward>>();

	private int currentSeenValue;

	private int currentNewValue;

	private GameObject tempIcon;

	private float runTweensDelay;

	private bool delayActive;

	public bool IsAnimating
	{
		get
		{
			if (progressBar != null)
			{
				return progressBar.IsAnimating;
			}
			return false;
		}
	}

	public void ShowProgress(bool personal)
	{
		rewardsBatches = new List<List<WeeklyChallengeReward>>();
		int num = 0;
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null)
		{
			int bitmask = 0;
			if (personal)
			{
				currentNewValue = weeklyChallengeModel.NumberStars;
				num = rewardsPerBatchPersonal;
				UtilsMath.BitmaskSet(2, ref bitmask);
			}
			else
			{
				currentNewValue = weeklyChallengeModel.NumberStarsGuild;
				num = rewardsPerBatchGuild;
				UtilsMath.BitmaskSet(4, ref bitmask);
			}
			currentSeenValue = currentNewValue;
			int batchCount = 0;
			int minStarCount = currentNewValue;
			int maxStarCount = currentNewValue + 1;
			weeklyChallengeModel.ReturnRewardsInBatches(bitmask, minStarCount, maxStarCount, num, out rewardsBatches, out batchCount, -1);
			ShowProgressionInBatch(rewardsBatches[rewardsBatches.Count - 1], currentNewValue, batchCount);
		}
	}

	public void ShowProgressFromLastSeenToCurrent(bool personal, float tweensDelay = 0f)
	{
		int num = 0;
		WeeklyChallengeModel weeklyChallengeModel = WeeklyChallengeHelper.GetWeeklyChallengeModel();
		if (weeklyChallengeModel != null && !IsAnimating && !delayActive)
		{
			runTweensDelay = tweensDelay;
			rewardsBatches = new List<List<WeeklyChallengeReward>>();
			int bitmask = 0;
			if (personal)
			{
				currentNewValue = weeklyChallengeModel.NumberStars;
				currentSeenValue = weeklyChallengeModel.LastSeenNumberStars;
				num = rewardsPerBatchPersonal;
				UtilsMath.BitmaskSet(2, ref bitmask);
			}
			else
			{
				currentNewValue = weeklyChallengeModel.NumberStarsGuild;
				currentSeenValue = weeklyChallengeModel.LastSeenNumberOfGuildStars;
				num = rewardsPerBatchGuild;
				UtilsMath.BitmaskSet(4, ref bitmask);
			}
			int batchCount = 0;
			int num2 = currentNewValue;
			int num3 = currentNewValue + 1;
			if (currentNewValue > currentSeenValue)
			{
				num2 = currentSeenValue;
				num3 = currentNewValue + 1;
				weeklyChallengeModel.ReturnRewardsInBatches(bitmask, num2, num3, num, out rewardsBatches, out batchCount, -1);
				TweenProgressionInBatch(rewardsBatches[rewardsBatches.Count - 1], currentNewValue, currentSeenValue, batchCount);
			}
			else
			{
				num2 = currentNewValue;
				num3 = currentNewValue + 1;
				weeklyChallengeModel.ReturnRewardsInBatches(bitmask, num2, num3, num, out rewardsBatches, out batchCount, -1);
				ShowProgressionInBatch(rewardsBatches[rewardsBatches.Count - 1], currentNewValue, batchCount);
			}
		}
	}

	public void ShowApocalypticProgressFromLastSeenToCurrent(bool personal, float tweensDelay = 0f)
	{
		int rewardsPerBatch = 0;
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		if (weeklyApocalypticChallengeModel != null && !IsAnimating && !delayActive)
		{
			runTweensDelay = tweensDelay;
			rewardsBatches = new List<List<WeeklyChallengeReward>>();
			int bitmask = 0;
			if (personal)
			{
				currentNewValue = weeklyApocalypticChallengeModel.NumberStars;
				currentSeenValue = weeklyApocalypticChallengeModel.LastSeenNumberStars;
				rewardsPerBatch = rewardsPerBatchPersonal;
				UtilsMath.BitmaskSet(64, ref bitmask);
			}
			int batchCount = 0;
			int num = currentNewValue;
			int num2 = currentNewValue + 1;
			if (currentNewValue > currentSeenValue)
			{
				num = currentSeenValue;
				num2 = currentNewValue + 1;
				weeklyApocalypticChallengeModel.ReturnRewardsInBatches(bitmask, num, num2, rewardsPerBatch, out rewardsBatches, out batchCount, -1);
				TweenProgressionInBatch(rewardsBatches[rewardsBatches.Count - 1], currentNewValue, currentSeenValue, batchCount);
			}
			else
			{
				num = currentNewValue;
				num2 = currentNewValue + 1;
				weeklyApocalypticChallengeModel.ReturnRewardsInBatches(bitmask, num, num2, rewardsPerBatch, out rewardsBatches, out batchCount, -1);
				ShowProgressionInBatch(rewardsBatches[rewardsBatches.Count - 1], currentNewValue, batchCount);
			}
		}
	}

	private void ShowProgressionInBatch(List<WeeklyChallengeReward> batch, int control, int currentBatchIndex)
	{
		if (batch == null || batch.Count <= 0 || !(rewardIcon != null) || !(rewardIcon.gameObject != null))
		{
			return;
		}
		progressBar.Reset();
		float min = ((currentBatchIndex <= 0) ? 0f : ((float)batch[0].Control));
		float max = batch[batch.Count - 1].Control;
		float progress = CalculateProgression(min, max, control);
		progressBar.SetProgress(progress);
		for (int i = 0; i < batch.Count; i++)
		{
			WeeklyChallengeReward weeklyChallengeReward = batch[i];
			if (weeklyChallengeReward == null)
			{
				continue;
			}
			float progress2 = CalculateProgression(min, max, weeklyChallengeReward.Control);
			tempIcon = progressBar.CreateWaypointIconAt(progress2, rewardIcon.gameObject, setActivate: true, positionNow: true);
			if (tempIcon != null && tempIcon.GetComponent<WeeklyChallengeRewardIcon>() != null)
			{
				WeeklyChallengeRewardIcon component = tempIcon.GetComponent<WeeklyChallengeRewardIcon>();
				component.SetReward(weeklyChallengeReward, control);
				if (weeklyChallengeReward.Control > control)
				{
					component.Show();
				}
				else
				{
					component.Hide();
				}
			}
		}
	}

	private void TweenProgressionInBatch(List<WeeklyChallengeReward> batch, int control, int controlOld, int currentBatchIndex)
	{
		if (batch == null || batch.Count <= 0 || !(rewardIcon != null) || !(rewardIcon.gameObject != null) || delayActive)
		{
			return;
		}
		progressBar.Reset();
		float min = ((currentBatchIndex <= 0) ? 0f : ((float)batch[0].Control));
		float max = batch[batch.Count - 1].Control;
		float num = CalculateProgression(min, max, controlOld);
		float num2 = CalculateProgression(min, max, control);
		float num3 = num;
		bool flag = false;
		progressBar.SetProgress(num);
		for (int i = 0; i < batch.Count; i++)
		{
			WeeklyChallengeReward weeklyChallengeReward = batch[i];
			if (weeklyChallengeReward == null)
			{
				continue;
			}
			float num4 = CalculateProgression(min, max, weeklyChallengeReward.Control);
			tempIcon = progressBar.CreateWaypointIconAt(num4, rewardIcon.gameObject, setActivate: true, positionNow: true);
			if (tempIcon != null && tempIcon.GetComponent<WeeklyChallengeRewardIcon>() != null)
			{
				WeeklyChallengeRewardIcon component = tempIcon.GetComponent<WeeklyChallengeRewardIcon>();
				component.SetReward(weeklyChallengeReward, control);
				ProgressBarWaypoint progressBarWaypoint = new ProgressBarWaypoint();
				progressBarWaypoint.id = i.ToString();
				progressBarWaypoint.duration = 1f;
				progressBarWaypoint.completionDelay = 0f;
				progressBarWaypoint.Easing = Easing.All.Linear;
				progressBarWaypoint.from = num3;
				progressBarWaypoint.to = num4;
				progressBarWaypoint.CurrentAsStartValue = false;
				if (num4 < num)
				{
					component.Hide();
				}
				else if (num4 <= num2)
				{
					num3 = num4;
					component.Show();
					progressBarWaypoint.ActivateObject = component;
					progressBar.AddAnimationWaypoint(progressBarWaypoint);
				}
				else if (!flag)
				{
					flag = true;
					progressBarWaypoint.to = num2;
					progressBarWaypoint.Easing = Easing.All.CubicEaseOut;
					progressBar.AddAnimationWaypoint(progressBarWaypoint);
					component.Show();
				}
				else
				{
					component.Show();
				}
			}
		}
		delayActive = true;
		Invoke("startWaypointsAfterDelay", runTweensDelay);
	}

	private void startWaypointsAfterDelay()
	{
		if (progressBar != null)
		{
			progressBar.StartWaypoints();
			Invoke("progressTweensDone", progressBar.GetWaypointsTotalDuration());
		}
	}

	private void progressTweensDone()
	{
		delayActive = false;
	}

	private float CalculateProgression(float min, float max, float current)
	{
		return Mathf.Clamp01(Mathf.Clamp01(Mathf.InverseLerp(min, max, current)));
	}
}
