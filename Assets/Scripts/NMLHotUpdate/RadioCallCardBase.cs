using System;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class RadioCallCardBase : MonoBehaviourExtended
{
	public delegate void Callback(RadioCallCardBase cardBase);

	protected Callback IntroCompleteCallaback;

	protected Callback CollectAnimationCompleteCallback;

	protected RadioCallCardEffect EffectInternal;

	protected RadioWeaponCallCardEffect WeaponEffectInternal;

	protected Tweener ScaleTwener = new Tweener();

	protected Vector3 ScaleVector = Vector3.one;

	protected Tweener PositionTweener = new Tweener();

	protected Vector3 InitLocalPosition = Vector3.one;

	protected UIWidget widget;

	private LootEntry entry;

	private int lootEntryIndex;

	[NonSerialized]
	public bool ForRerolling;

	[NonSerialized]
	public bool DisableSelectVisualization;

	protected int AnimatorHashGetToken = Animator.StringToHash("GetToken");

	protected int AnimatorHashGetSurvivor = Animator.StringToHash("GetSurvivor");

	protected int AnimatorHashGetTokenInPlace = Animator.StringToHash("GetTokenInPlace");

	protected int AnimatorHashGetSurvivorInPlace = Animator.StringToHash("GetSurvivorInPlace");

	public virtual Vector2 localSize
	{
		get
		{
			if (widget != null)
			{
				return widget.localSize;
			}
			return Vector2.zero;
		}
	}

	public LootEntry GetLootEntry()
	{
		return entry;
	}

	public void SetLootEntry(LootEntry lootEntry, int lootEntryIndex)
	{
		entry = lootEntry;
		this.lootEntryIndex = lootEntryIndex;
	}

	public int GetLootEntryIndex()
	{
		return lootEntryIndex;
	}

	public virtual void UpdateUI()
	{
	}

	public virtual void Update()
	{
		if (PositionTweener != null && PositionTweener.animating)
		{
			PositionTweener.update();
			SetPosition(PositionTweener.progression);
		}
		if (ScaleTwener != null && base.transform != null && ScaleTwener.animating)
		{
			ScaleTwener.update();
			ScaleVector.x = ScaleTwener.progression.x;
			ScaleVector.y = ScaleTwener.progression.y;
			ScaleVector.z = ScaleTwener.progression.z;
			base.transform.localScale = ScaleVector;
		}
	}

	public virtual void SetIntroCompleteCallaback(Callback callback)
	{
		IntroCompleteCallaback = (Callback)Delegate.Remove(IntroCompleteCallaback, callback);
		IntroCompleteCallaback = (Callback)Delegate.Combine(IntroCompleteCallaback, callback);
	}

	public virtual void FakeEffectClicked()
	{
		if (EffectInternal != null)
		{
			EffectInternal.FakeUserClick();
		}
		if (WeaponEffectInternal != null)
		{
			WeaponEffectInternal.FakeUserClick();
		}
	}

	public virtual void SetInitPosition(Vector3 newPosition)
	{
		InitLocalPosition = newPosition;
		SetPosition(newPosition);
	}

	public virtual void Select(bool selected)
	{
		DebugTWD.Log("RadioCallCardBase Select", DebugType.ActivateObject);

		if (ScaleTwener != null)
		{
			Vector4 vector = base.transform.localScale;
			Vector4 one = Vector4.one;
			one = ((!selected || ForRerolling || DisableSelectVisualization) ? Vector4.one : (Vector4.one * 1.1f));
			ScaleTwener.easeFromTo(vector, one, 0.3f, EasingFunctions.BackEaseOut);
		}
	}

	public virtual void CollectCard(Callback collectAnimationComplete, SelectSurvivorsPopup.SelectedRewardType rewardType, bool animate, bool doInPlaceAnimation)
	{
		SetCollectCompleteCallaback(collectAnimationComplete);
	}

	public virtual void CollectCardComplete()
	{
		if (CollectAnimationCompleteCallback != null)
		{
			CollectAnimationCompleteCallback(this);
			CollectAnimationCompleteCallback = null;
		}
	}

	public virtual void HideRewardCard()
	{
		if (widget != null)
		{
			widget.alpha = 0f;
		}
	}

	public virtual void ShowRewardCard()
	{
		if (widget != null)
		{
			widget.alpha = 1f;
		}
	}

	public virtual void SetPosition(Vector3 newPosition)
	{
		if (EffectInternal != null)
		{
			EffectInternal.transform.localPosition = newPosition;
		}
		if (WeaponEffectInternal != null)
		{
			WeaponEffectInternal.transform.localPosition = newPosition;
		}
		base.transform.localPosition = newPosition;
	}

	public virtual void InitSurvivorCard(SurvivorModel model, int lootEntryIndex)
	{
	}

	public virtual void InitTokenCard(LootEntry entry, int lootEntryIndex, string buttonIndex, UIButtonExtended.OnClickCallback clickCallback)
	{
	}

	public virtual void InitWeaponCard(IReward reward)
	{
	}

	public virtual void AnimateIntro()
	{
		SetCardLocked(value: true, introAnimationLock: true);
		if (GetLootEntry() != null && GetLootEntry().Opened)
		{
			HideAnimation();
			ShowRewardCard();
			if (IntroCompleteCallaback != null)
			{
				IntroCompleteCallaback(this);
				IntroCompleteCallaback = null;
			}
		}
		else
		{
			if (!OfflineManager.IsNoEffects) ShowAnimation();
			HideRewardCard();
		}
	}

	public virtual void SetCardLocked(bool value, bool introAnimationLock)
	{
	}

	public void InitRerollButtons()
	{
		SurvivorCard component = base.gameObject.GetComponent<SurvivorCard>();
		if (component != null)
		{
			SurvivorCardRerollLocking survivorCardRerollLocking = component.GetSurvivorCardRerollLocking();
			if (survivorCardRerollLocking != null)
			{
				survivorCardRerollLocking.LootIndex = lootEntryIndex;
			}
		}
		TokenCard component2 = base.gameObject.GetComponent<TokenCard>();
		if (component2 != null)
		{
			TokenCardRerollLocking tokenCardRerollLocking = component2.GetTokenCardRerollLocking();
			if (tokenCardRerollLocking != null)
			{
				tokenCardRerollLocking.LootIndex = lootEntryIndex;
			}
		}
	}

	public void InitEffects(GameObject prefab, SurvivorClass survivorClass, int survivorRarity, DropType drop, bool isToken)
	{
		if (EffectInternal == null && prefab != null)
		{
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(prefab, base.gameObject.transform.parent.gameObject);
			if (gameObject != null)
			{
				gameObject.transform.localPosition = Vector3.zero;
				EffectInternal = gameObject.GetComponent<RadioCallCardEffect>();
				EffectInternal.SetSurvivorClass(survivorClass);
				EffectInternal.SetSurvivorRarityLevel(survivorRarity);
				EffectInternal.SetDropType(drop);
				EffectInternal.SetIsToken(isToken);
				EffectInternal.AddCompleteCallback(AnimateIntroComplete);
			}
		}
	}

	public void InitEffects(GameObject prefab, int rarity)
	{
		if (WeaponEffectInternal == null && prefab != null)
		{
			GameObject gameObject = Helpers.InstantiateToParentAndLayer(prefab, base.gameObject.transform.parent.gameObject);
			if (gameObject != null)
			{
				gameObject.transform.localPosition = Vector3.zero;
				WeaponEffectInternal = gameObject.GetComponent<RadioWeaponCallCardEffect>();
				WeaponEffectInternal.SetWeaponRarityLevel(rarity);
				WeaponEffectInternal.AddCompleteCallback(AnimateIntroComplete);
			}
		}
	}

	public void TweenToPosition(Vector3 newPosition, Vector3 newScale, float duration, Tweener.CallBackDelegate callback = null)
	{
		if (PositionTweener != null && ScaleTwener != null)
		{
			PositionTweener = new Tweener();
			PositionTweener.easeFromTo(base.transform.localPosition, newPosition, duration, EasingFunctions.SineEaseOut, callback);
			if (newScale.x < 0f)
			{
				newScale = base.transform.localScale;
			}
			ScaleTwener = new Tweener();
			ScaleTwener.easeFromTo(base.transform.localScale, newScale, duration, EasingFunctions.SineEaseOut);
			if (duration <= 0f)
			{
				Update();
			}
		}
	}

	public void TweenOffsetToPosition(Vector3 offsetPosition, Vector3 newScale, float duration, Tweener.CallBackDelegate callback = null)
	{
		Vector3 newPosition = InitLocalPosition + offsetPosition;
		TweenToPosition(newPosition, newScale, duration, callback);
	}

	public override void Clear()
	{
		base.Clear();
		if (EffectInternal != null)
		{
			EffectInternal.Clear();
			EffectInternal = null;
		}
		if (WeaponEffectInternal != null)
		{
			WeaponEffectInternal.Clear();
			WeaponEffectInternal = null;
		}
	}

	protected virtual void AnimateIntroComplete()
	{
		if (EffectInternal != null)
		{
			EffectInternal.RequestHide();
		}
		if (WeaponEffectInternal != null)
		{
			WeaponEffectInternal.RequestHide();
		}
		if (IntroCompleteCallaback != null)
		{
			IntroCompleteCallaback(this);
			IntroCompleteCallaback = null;
		}
		ShowRewardCard();
	}

	public void HideAnimation()
	{
		if (EffectInternal != null)
		{
			EffectInternal.Hide();
		}
		if (WeaponEffectInternal != null)
		{
			WeaponEffectInternal.Hide();
		}
	}

	public void ShowAnimation()
	{
		if (EffectInternal != null)
		{
			EffectInternal.Show();
		}
		if (WeaponEffectInternal != null)
		{
			WeaponEffectInternal.Show();
		}
	}

	private void SetCollectCompleteCallaback(Callback callback)
	{
		if (callback != null)
		{
			CollectAnimationCompleteCallback = (Callback)Delegate.Remove(CollectAnimationCompleteCallback, callback);
			CollectAnimationCompleteCallback = (Callback)Delegate.Combine(CollectAnimationCompleteCallback, callback);
		}
	}



	#region mycode
	public virtual void InitSurvivorCard(SurvivorModel model, int lootEntryIndex, UIButtonExtended.OnClickCallback clickCallback)
	{
	}
	#endregion
}
