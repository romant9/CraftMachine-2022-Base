using System;
using TWDModel;
using UnityEngine;

public class ReturnLoginThreeDayRewardItem : MonoBehaviour
{
	private Rewards _rewards;

	private int _rewardIndex;

	private ReturnThreeDayRewardStatus _status;

	private Action _onClaimed;

	private UILabel _amountLabel;

	private UISprite _rewardIcon;

	private GameObject _locked;

	private GameObject _claimed;

	private GameObject _claimable;

	private UIButton _button;

	public void Bind(Rewards rewards, ReturnThreeDayRewardStatus status, int rewardIndex, Action onClaimed)
	{
		_rewards = rewards;
		_status = status;
		_rewardIndex = rewardIndex;
		_onClaimed = onClaimed;
		ResolveReferences();
		Refresh();
	}

	private void ResolveReferences()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			string text = componentsInChildren[i].name.ToLowerInvariant();
			if (_locked == null && text.Contains("lock"))
			{
				_locked = componentsInChildren[i].gameObject;
			}
			else if (_claimed == null && (text.Contains("claimed") || text.Contains("completed")))
			{
				_claimed = componentsInChildren[i].gameObject;
			}
			else if (_claimable == null && (text.Contains("reward_button") || text.Contains("claim")))
			{
				_claimable = componentsInChildren[i].gameObject;
			}
		}
		_button = GetComponentInChildren<UIButton>(includeInactive: true);
		if (_button != null)
		{
			EventDelegate.Set(_button.onClick, OnClaimClicked);
		}
		_amountLabel = GetComponentInChildren<UILabel>(includeInactive: true);
		UISprite[] componentsInChildren2 = GetComponentsInChildren<UISprite>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			string text2 = componentsInChildren2[j].name.ToLowerInvariant();
			if (text2.Contains("icon") || text2.Contains("reward"))
			{
				_rewardIcon = componentsInChildren2[j];
				break;
			}
		}
	}

	private void Refresh()
	{
		IReward reward = ((_rewards != null && _rewards.Count > 0) ? _rewards.GetRewardAt(0) : null);
		if (reward != null)
		{
			HelpersGfx.GetIconNameForIReward(reward, out var spriteName, null, null, null);
			HelpersUI.SetSprite(_rewardIcon, spriteName);
			int numsForIReward = Helpers.GetNumsForIReward(reward);
			HelpersUI.SetContentToLabel(_amountLabel, (numsForIReward > 1) ? ("x" + numsForIReward) : string.Empty);
		}
		Helpers.GameObjectSetActive(_locked, _status == ReturnThreeDayRewardStatus.Lock);
		Helpers.GameObjectSetActive(_claimed, _status == ReturnThreeDayRewardStatus.Rewarded);
		Helpers.GameObjectSetActive(_claimable, _status == ReturnThreeDayRewardStatus.Unlock);
		if (_button != null)
		{
			_button.isEnabled = _status == ReturnThreeDayRewardStatus.Unlock;
		}
	}

	public void OnClaimClicked()
	{
		if (_status == ReturnThreeDayRewardStatus.Unlock && Helpers.ExecuteCommand(new ReturnThreeDayRewardCommand(_rewardIndex)) == TWDModelResult.OK)
		{
			BuildingsHUD buildingsHUD = BuildingsHUD.Get();
			if (buildingsHUD != null && _rewards != null)
			{
				buildingsHUD.CreateCollectAnim(_rewards, base.gameObject);
			}
			_onClaimed?.Invoke();
		}
	}
}
