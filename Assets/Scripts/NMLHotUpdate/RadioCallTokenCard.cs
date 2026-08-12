using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RadioCallTokenCard : RadioCallCardBase
{
	private TokenCard tokenCard;

	private bool overrideMyPosition;

	private Vector3 overrideMyPositionTo;

	private void Awake()
	{
		DebugIdString = "RadioCallTokenCard";
	}

	public override void InitTokenCard(LootEntry entry, int lootEntryIndex, string buttonIndex, UIButtonExtended.OnClickCallback clickCallback)
	{
		base.InitTokenCard(entry, lootEntryIndex, buttonIndex, clickCallback);
		overrideMyPosition = false;
		tokenCard = GetComponent<TokenCard>();
		if (tokenCard != null && entry != null && tokenCard.GetButton() != null)
		{
			tokenCard.Init(entry, lootEntryIndex);
			tokenCard.GetButton().id = buttonIndex;
			tokenCard.GetButton().SetClickCallback(clickCallback);
			tokenCard.ForRerolling = ForRerolling;
			widget = tokenCard.widget;
		}
		else
		{
			DebugLogError("Cant accept NULL parameters!!");
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (tokenCard != null)
		{
			tokenCard.UpdateUI();
		}
	}

	public override void SetCardLocked(bool value, bool introAnimationLock)
	{
		base.SetCardLocked(value, introAnimationLock);
		if (tokenCard.GetButton() != null)
		{
			tokenCard.GetButton().isEnabled = !value;
		}
		if (!introAnimationLock && tokenCard.GetAcceptButton() != null)
		{
			Helpers.GameObjectSetActive(tokenCard.GetAcceptButton(), !value);
		}
	}

	public override void Select(bool selected)
	{
		if (!OfflineManager.IsLoadDataManager) base.Select(selected);
		if (tokenCard != null)
		{
			tokenCard.SetSeleted(selected);
		}
	}

	public void OnAnimationCompletedHandler()
	{
		base.CollectCardComplete();
	}

	private void TriggerCardAnimation(int animationHash)
	{
		Animator animator = null;
		List<Animator> list = ListPool<Animator>.Get();
		base.gameObject.GetComponentsInChildren(includeInactive: true, list);
		if (list.Count > 0)
		{
			animator = list[0];
		}
		if (animator != null)
		{
			animator.enabled = true;
			animator.SetTrigger(animationHash);
		}
		else
		{
			Debug.LogError("Could not find Animator in object: " + base.gameObject.name);
		}
		ListPool<Animator>.Release(list);
	}

	public override void CollectCard(Callback collectAnimationComplete, SelectSurvivorsPopup.SelectedRewardType rewardType, bool animate, bool doInPlaceAnimation)
	{
		base.CollectCard(collectAnimationComplete, rewardType, animate, doInPlaceAnimation);
		if (!(tokenCard != null))
		{
			return;
		}
		bool allowShowingUnlockButton = true;
		if (!GameManager.Instance.gameEconomyData.ConfigData.EnableHeroUnlockInMultiCardCall)
		{
			allowShowingUnlockButton = !animate;
		}
		tokenCard.CollectCard(allowShowingUnlockButton);
		if (animate)
		{
			if (doInPlaceAnimation)
			{
				overrideMyPosition = true;
				overrideMyPositionTo = base.gameObject.transform.position;
				TriggerCardAnimation(AnimatorHashGetTokenInPlace);
			}
			else
			{
				TriggerCardAnimation(AnimatorHashGetToken);
			}
		}
	}

	public void LateUpdate()
	{
		if (overrideMyPosition)
		{
			base.gameObject.transform.position = new Vector3(overrideMyPositionTo.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
		}
	}

	protected override void AnimateIntroComplete()
	{
		base.AnimateIntroComplete();
		LootEntry lootEntry = GetLootEntry();
		if (lootEntry != null && SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/found_" + HelpersUI.GetRarityName(lootEntry.RewardedRarityLevel).ToLower());
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/found_token");
		}
	}
}
