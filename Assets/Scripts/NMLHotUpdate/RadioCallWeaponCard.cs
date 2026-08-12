using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RadioCallWeaponCard : RadioCallCardBase
{
	private RadioWeaponCard tokenCard;

	private bool overrideMyPosition;

	private Vector3 overrideMyPositionTo;

	private const float WeaponRevealFlipHalfDuration = 0.14f;

	[Tooltip("3D 结束并显示预告图标后，到第二次翻面开始之间的停顿（秒）。")]
	[SerializeField]
	private float weaponRevealPauseBeforeFlipSeconds = 1f;

	private Coroutine weaponRevealFlipRoutine;

	private void Awake()
	{
		DebugIdString = "RadioCallWeaponCard";
	}

	public override void InitWeaponCard(IReward reward)
	{
		base.InitWeaponCard(reward);
		overrideMyPosition = false;
		tokenCard = GetComponent<RadioWeaponCard>();
		if (tokenCard != null && tokenCard.GetButton() != null)
		{
			tokenCard.Init(reward);
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
	}

	public override void Select(bool selected)
	{
		base.Select(selected);
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

	public void SetWeaponRevealPauseBeforeFlipSeconds(float seconds)
	{
		weaponRevealPauseBeforeFlipSeconds = Mathf.Max(0f, seconds);
	}

	public float GetWeaponRevealPauseBeforeFlipSeconds()
	{
		return weaponRevealPauseBeforeFlipSeconds;
	}

	private static bool ShouldUseWeaponRevealFlipSequence(RadioCallWeaponCard card)
	{
		if (card == null || card.tokenCard == null)
		{
			return false;
		}
		if (!(card.tokenCard.GetCurrentReward() is RewardRemoldSkill { GivenRewardResult: var givenRewardResult }))
		{
			return false;
		}
		return givenRewardResult?.IsDuplicate ?? false;
	}

	protected override void AnimateIntroComplete()
	{
		if (!ShouldUseWeaponRevealFlipSequence(this))
		{
			base.AnimateIntroComplete();
			return;
		}
		if (WeaponEffectInternal != null)
		{
			WeaponEffectInternal.RequestHide();
		}
		if (EffectInternal != null)
		{
			EffectInternal.RequestHide();
		}
		ShowRewardCard();
		if (tokenCard != null)
		{
			tokenCard.ShowRevealTeaserOnly();
		}
		if (weaponRevealFlipRoutine != null)
		{
			StopCoroutine(weaponRevealFlipRoutine);
			weaponRevealFlipRoutine = null;
		}
		weaponRevealFlipRoutine = StartCoroutine(WeaponRevealFlipSequence());
	}

	private IEnumerator WeaponRevealFlipSequence()
	{
		if (weaponRevealPauseBeforeFlipSeconds > 0f)
		{
			yield return new WaitForSeconds(weaponRevealPauseBeforeFlipSeconds);
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_flip");
		}
		Vector3 origScale = base.transform.localScale;
		float t = 0f;
		while (t < 0.14f)
		{
			t += Time.deltaTime;
			float t2 = Mathf.Clamp01(t / 0.14f);
			base.transform.localScale = new Vector3(Mathf.Lerp(origScale.x, 0f, t2), origScale.y, origScale.z);
			yield return null;
		}
		base.transform.localScale = new Vector3(0f, origScale.y, origScale.z);
		if (tokenCard != null)
		{
			tokenCard.UpdateUI();
		}
		t = 0f;
		while (t < 0.14f)
		{
			t += Time.deltaTime;
			float t3 = Mathf.Clamp01(t / 0.14f);
			base.transform.localScale = new Vector3(Mathf.Lerp(0f, origScale.x, t3), origScale.y, origScale.z);
			yield return null;
		}
		base.transform.localScale = origScale;
		weaponRevealFlipRoutine = null;
		if (IntroCompleteCallaback != null)
		{
			IntroCompleteCallaback(this);
			IntroCompleteCallaback = null;
		}
	}
}
