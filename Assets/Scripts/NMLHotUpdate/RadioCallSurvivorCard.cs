using TWDModel;
using UnityEngine;

public class RadioCallSurvivorCard : RadioCallCardBase
{
	private SurvivorCard SurvivorCardInternal;

	private AnimatedUIButtonExtended AnimatedButton;

	private bool overrideMyPosition;

	private Vector3 overrideMyPositionTo;

	public SurvivorModel Item
	{
		get
		{
			if (SurvivorCardInternal != null)
			{
				return SurvivorCardInternal.Item;
			}
			return null;
		}
	}

	public SurvivorCard card => SurvivorCardInternal;

	private void Awake()
	{
		DebugIdString = "RadioCallSurvivorCard";
	}

	public static bool TryAddComponent(GameObject target, out RadioCallSurvivorCard card)
	{
		if (target != null)
		{
			card = target.GetComponent<RadioCallSurvivorCard>();
			if (card == null)
			{
				card = target.AddComponent<RadioCallSurvivorCard>();
			}
			if (card != null)
			{
				return true;
			}
			Debug.LogWarning("Could not AddComponent: RadioCallSurvivorCard!");
			return false;
		}
		Debug.LogError("Cant attach to NULL target!!");
		card = null;
		return false;
	}

	public override void InitSurvivorCard(SurvivorModel model, int lootEntryIndex, UIButtonExtended.OnClickCallback clickCallback)
	{
		base.InitSurvivorCard(model, lootEntryIndex, clickCallback);
		overrideMyPosition = false;
		SurvivorCardInternal = GetComponent<SurvivorCard>();
		if (SurvivorCardInternal != null && model != null)
		{
			AnimatedButton = SurvivorCardInternal.GetComponent<AnimatedUIButtonExtended>();
			widget = SurvivorCardInternal.widget;
			SurvivorCardInternal.Type = (ForRerolling ? SurvivorCard.CardType.RadioPhoneForReroll : SurvivorCard.CardType.RadioPhone);
			if (clickCallback != null) SurvivorCardInternal.GetComponent<UIButtonExtended>().SetClickCallback(clickCallback);

			SurvivorCardInternal.Item = model;
			SurvivorCardInternal.SetLootIndex(lootEntryIndex);
		}
		else
		{
			DebugLogError("Cant accept NULL parameters!!");
		}
	}

	public override void Select(bool selected)
	{
		if (!OfflineManager.IsLoadDataManager) base.Select(selected);
		if (SurvivorCardInternal != null)
		{
			SurvivorCardInternal.Selected = selected;
		}
	}

	public override void CollectCard(Callback collectAnimationComplete, SelectSurvivorsPopup.SelectedRewardType rewardType, bool animate, bool doInPlaceAnimation)
	{
		base.CollectCard(collectAnimationComplete, rewardType, animate, doInPlaceAnimation);
		if (!animate)
		{
			DebugTWD.LogError("Called CollectCard for a survivor card, with animate=false parameter, that is unsupported.");
			if (OfflineManager.IsLoadDataManager)
			{
				overrideMyPosition = true;
				overrideMyPositionTo = base.gameObject.transform.position;
				if (AnimatedButton != null) AnimatedButton.SetTriggerToAnimation(AnimatorHashGetTokenInPlace);
				return;
			}
		}
		if (!(AnimatedButton != null))
		{
			return;
		}
		AnimatedButton.SetCompleteCallback(CollectAnimationComplete);
		switch (rewardType)
		{
		case SelectSurvivorsPopup.SelectedRewardType.Survivor:
			if (doInPlaceAnimation)
			{
				overrideMyPosition = true;
				overrideMyPositionTo = base.gameObject.transform.position;
				AnimatedButton.SetTriggerToAnimation(AnimatorHashGetSurvivorInPlace);
			}
			else
			{
				AnimatedButton.SetTriggerToAnimation(AnimatorHashGetSurvivor);
			}
			break;
		case SelectSurvivorsPopup.SelectedRewardType.ClassToken:
			if (doInPlaceAnimation)
			{
				overrideMyPosition = true;
				overrideMyPositionTo = base.gameObject.transform.position;
				AnimatedButton.SetTriggerToAnimation(AnimatorHashGetTokenInPlace);
			}
			else
			{
				AnimatedButton.SetTriggerToAnimation(AnimatorHashGetToken);
			}
			break;
		}
	}

	public void LateUpdate()
	{
		if (overrideMyPosition)
		{
			base.gameObject.transform.position = new Vector3(overrideMyPositionTo.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
		}
	}

	public override void CollectCardComplete()
	{
		base.CollectCardComplete();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (SurvivorCardInternal != null)
		{
			SurvivorCardInternal.UpdateUI();
		}
	}

	public override void HideRewardCard()
	{
		base.HideRewardCard();
		SetUIButtonDisabledAlpha(0f);
	}

	protected override void AnimateIntroComplete()
	{
		base.AnimateIntroComplete();
		if (Item != null && SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/found_" + HelpersUI.GetRarityName(Item.SurvivorRarityLevel).ToLower());
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/found_" + Item.SurvivorClass.ToString().ToLower());
		}
		SetUIButtonDisabledAlpha(1f);
	}

	public override void SetCardLocked(bool value, bool introAnimationLock)
	{
		base.SetCardLocked(value, introAnimationLock);
		if (SurvivorCardInternal != null)
		{
			SurvivorCardInternal.Locked = value;
			SurvivorCardInternal.UpdateUI();
		}
	}

	private void CollectAnimationComplete(UIButtonExtended button)
	{
		if (AnimatedButton != null)
		{
			AnimatedButton.RemoveOnCompleteCallback(CollectAnimationComplete);
			CollectCardComplete();
		}
	}

	private void SetUIButtonDisabledAlpha(float alpha)
	{
		UIButton component = SurvivorCardInternal.gameObject.GetComponent<UIButton>();
		if (component != null)
		{
			Color disabledColor = component.disabledColor;
			disabledColor.a = alpha;
			component.disabledColor = disabledColor;
			disabledColor = component.pressed;
			disabledColor.a = alpha;
			component.pressed = disabledColor;
			disabledColor = component.hover;
			disabledColor.a = alpha;
			component.hover = disabledColor;
			disabledColor = component.defaultColor;
			disabledColor.a = alpha;
			component.defaultColor = disabledColor;
		}
	}
}
