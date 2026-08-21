using System.Collections.Generic;
using UnityEngine;

public class SurvivorAnimationController : CharacterAnimationController
{
	private static bool VerboseDebug = false;

	private const int ManagementVariationUpgradeId = 10;

	private const int ManagementVariationRetireId = 11;

	private const int ManagementVariationUnlockId = 12;

	private static int PushParamNameHash = Animator.StringToHash("Push");

	private static int UseWeaponVariationParamNameHash = Animator.StringToHash("UseWeaponVariation");

	private static int UseWeaponParamNameHash = Animator.StringToHash("UseWeapon");

	private static int RaiseWeaponParamNameHash = Animator.StringToHash("RaiseWeapon");

	private static int LowerWeaponParamNameHash = Animator.StringToHash("LowerWeapon");

	private static int InteractParamNameHash = Animator.StringToHash("Interact");

	private static int InteractionCompletedParamNameHash = Animator.StringToHash("InteractionCompleted");

	private static int CustomAnimationParamNameHash = Animator.StringToHash("CustomAnimation");

	private static int UseWeaponFenceParamNameHash = Animator.StringToHash("UseWeaponFence");

	private static int UseWeaponChargeParamNameHash = Animator.StringToHash("UseWeaponCharge");

	protected static int IdleStanceParamNameHash = Animator.StringToHash("IdleStance");

	private static int ManagementInsertParamNameHash = Animator.StringToHash("ManagementInsert");

	private static int ManagementInsertVariationParamNameHash = Animator.StringToHash("ManagementInsertVariation");

	private static int CharacterManagementStateNameHash = Animator.StringToHash("Base Layer.CharacterManagement.BaseLoop");

	private static int EquipStateNameHash = Animator.StringToHash("Base Layer.Equip");

	private static int ChargeStateNameHash = Animator.StringToHash("Base Layer.Charge");

	private static int StartInteractionStateNameHash = Animator.StringToHash("Base Layer.StartInteraction");

	private static int EndInteractionStateNameHash = Animator.StringToHash("Base Layer.EndInteraction");

	private static int CompleteInteractionStateNameHash = Animator.StringToHash("Base Layer.CompleteInteraction");

	public const string StartInteractionAnimationEntryName = "Survivor_EnterLoot_Normal";

	public const string LoopInteractionAnimationEntryName = "Survivor_LoopLoot_Normal_A";

	public const string EndInteractionAnimationEntryName = "Survivor_LeaveLoot_Normal";

	public const string CompleteInteractionAnimationEntryName = "Survivor_OpenLoot_Normal_A";

	public static int StartInteractionAnimationEntryNameHash = Animator.StringToHash("Base Layer.Survivor_EnterLoot_Normal");

	public static int LoopInteractionAnimationEntryNameHash = Animator.StringToHash("Base Layer.Survivor_LoopLoot_Normal_A");

	public static int EndInteractionAnimationEntryNameHash = Animator.StringToHash("Base Layer.Survivor_LeaveLoot_Normal");

	public static int CompleteInteractionAnimationEntryNameHash = Animator.StringToHash("Base Layer.Survivor_OpenLoot_Normal_A");

	[SerializeField]
	[Tooltip("Collection of melee attack effects that can be triggered from animations.")]
	private List<GameObject> meleeAttackPrefabs;

	public List<AnimatorSetup> animators = new List<AnimatorSetup>();

	private RuntimeAnimatorController previousRuntimeAnimatorController;

	private AnimatorStateInfo previousStateInfo;

	private bool deactivatingCustomAnimation;

	private WeaponPose desiredWeaponPose;

	private bool weaponPoseUpdateDisabled;

	private float desiredWeaponPoseChangeTime;

	private float desiredWeaponPoseChangeReactionTime = 0.1f;

	private AnimationClip defaultStartInteractionClip;

	private AnimationClip defaultLoopInteractionClip;

	private AnimationClip defaultEndInteractionClip;

	private AnimationClip defaultCompletedInteractionClip;

	private float CharacterManagementIdleTimer;

	private float CharacterManagementInsertMinDelay = 2f;

	private float CharacterManagementInsertMaxDelay = 8f;

	private float desiredIdleStance;

	public float IdleStanceBlendTime = 0.3f;

	private int resumeToState;

	public string ControllerId { get; protected set; }

	private bool WeaponSwitchNotificationSent { get; set; }

	public WeaponPose DesiredWeaponPose
	{
		get
		{
			if (!base.IsDeathRequested)
			{
				return desiredWeaponPose;
			}
			return WeaponPose.Lowered;
		}
		set
		{
			if (desiredWeaponPose != value && (value == WeaponPose.Lowered || value == WeaponPose.Raised))
			{
				desiredWeaponPose = value;
				desiredWeaponPoseChangeTime = Time.time;
				_ = VerboseDebug;
			}
		}
	}

	public WeaponPose CurrentWeaponPose
	{
		get
		{
			AnimatorStateInfo currentAnimatorStateInfo = base.Animator.GetCurrentAnimatorStateInfo(0);
			AnimatorStateInfo nextAnimatorStateInfo = base.Animator.GetNextAnimatorStateInfo(0);
			float normalizedTime = base.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if ((currentAnimatorStateInfo.fullPathHash == CharacterAnimationController.RaiseWeaponStateNameHash && normalizedTime >= 1f) || currentAnimatorStateInfo.fullPathHash == CharacterAnimationController.UseWeaponStateNameHash || currentAnimatorStateInfo.fullPathHash == ChargeStateNameHash)
			{
				return WeaponPose.Raised;
			}
			if (nextAnimatorStateInfo.fullPathHash == CharacterAnimationController.RaiseWeaponStateNameHash || (currentAnimatorStateInfo.fullPathHash == CharacterAnimationController.RaiseWeaponStateNameHash && normalizedTime < 1f))
			{
				return WeaponPose.BeingRaised;
			}
			if (currentAnimatorStateInfo.fullPathHash == CharacterAnimationController.LowerWeaponStateNameHash)
			{
				return WeaponPose.Lowered;
			}
			if (nextAnimatorStateInfo.fullPathHash == CharacterAnimationController.LowerWeaponStateNameHash)
			{
				return WeaponPose.BeingLowered;
			}
			return WeaponPose.Lowered;
		}
	}

	private bool IsSwitchingWeapon => base.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == EquipStateNameHash;

	public bool IsPushRequested => base.Animator.GetBool(PushParamNameHash);

	public bool IsBleedingOutRequested { get; set; }

	public bool IsInStartInteraction
	{
		get
		{
			if (base.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == StartInteractionStateNameHash)
			{
				return !base.Animator.IsInTransition(0);
			}
			return false;
		}
	}

	public bool IsInEndInteraction
	{
		get
		{
			if (base.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == EndInteractionStateNameHash)
			{
				return !base.Animator.IsInTransition(0);
			}
			return false;
		}
	}

	public bool IsInInteractionCompleted
	{
		get
		{
			if (base.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == CompleteInteractionStateNameHash)
			{
				return !base.Animator.IsInTransition(0);
			}
			return false;
		}
	}

	public bool IsInInteractionLoop
	{
		get
		{
			if (base.Animator.GetBool(InteractParamNameHash))
			{
				return !base.Animator.IsInTransition(0);
			}
			return false;
		}
	}

	public event WeaponSwitchHandler WeaponSwitched;

	public event WeaponRaisedHandler WeaponRaised;

	public event InteractionCompleteHandler InteractionCompleted;

	public SurvivorAnimationController()
	{
		previousRuntimeAnimatorController = null;
		deactivatingCustomAnimation = false;
	}

	private void SpawnMeleeEffects(Transform parent, Quaternion relativeOrientation, Vector3 localScaling, Vector3 position, int effectIndex)
	{
		if (effectIndex >= meleeAttackPrefabs.Count)
		{
			return;
		}
		GameObject gameObject = meleeAttackPrefabs[effectIndex];
		if (gameObject != null)
		{
			Transform transform = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity).transform;
			if (parent != null)
			{
				transform.SetParent(parent, worldPositionStays: false);
				transform.localRotation = relativeOrientation * Quaternion.Euler(new Vector3(90f, 0f, 0f));
				transform.localScale = localScaling;
				transform.position = position;
			}
		}
	}

	public void OnMeleeAttackPlayEffect(int index)
	{
		SpawnMeleeEffects(base.transform, Quaternion.identity, new Vector3(-1f, 1f, 1f), base.transform.position, index);
	}

	private RuntimeAnimatorController GetController(string animationId)
	{
		for (int i = 0; i < animators.Count; i++)
		{
			if (animators[i].Id == animationId)
			{
				return animators[i].Controller;
			}
		}
		return null;
	}

	public void CharacterManagement()
	{
		base.Animator.CrossFade(CharacterManagementStateNameHash, 0f, 0);
		CharacterManagementIdleTimer = Random.Range(CharacterManagementInsertMinDelay, CharacterManagementInsertMaxDelay);
	}

	public void UpdateCharacterManagement()
	{
		if (base.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != CharacterManagementStateNameHash)
		{
			return;
		}
		CharacterManagementIdleTimer -= Time.deltaTime;
		if (!(CharacterManagementIdleTimer <= 0f))
		{
			return;
		}
		CharacterManagementIdleTimer = Random.Range(CharacterManagementInsertMinDelay, CharacterManagementInsertMaxDelay);
		int integer = base.Animator.GetInteger(ManagementInsertVariationParamNameHash);
		int num = Random.Range(0, 100) % 3;
		for (int i = 0; i < 10; i++)
		{
			if (integer != num)
			{
				break;
			}
			num = Random.Range(0, 100) % 3;
		}
		base.Animator.SetInteger(ManagementInsertVariationParamNameHash, num);
		base.Animator.ResetTrigger(ManagementInsertParamNameHash);
		base.Animator.SetTrigger(ManagementInsertParamNameHash);
	}

	public void CharacterManagementUpgrade()
	{
		base.Animator.SetInteger(ManagementInsertVariationParamNameHash, 10);
		base.Animator.ResetTrigger(ManagementInsertParamNameHash);
		base.Animator.SetTrigger(ManagementInsertParamNameHash);
		CharacterManagementIdleTimer = CharacterManagementInsertMaxDelay;
	}

	public void CharacterManagementRetire()
	{
		base.Animator.SetInteger(ManagementInsertVariationParamNameHash, 11);
		base.Animator.ResetTrigger(ManagementInsertParamNameHash);
		base.Animator.SetTrigger(ManagementInsertParamNameHash);
		CharacterManagementIdleTimer = CharacterManagementInsertMaxDelay;
	}

	public void CharacterManagementUnlock()
	{
		base.Animator.SetInteger(ManagementInsertVariationParamNameHash, 12);
		base.Animator.ResetTrigger(ManagementInsertParamNameHash);
		base.Animator.SetTrigger(ManagementInsertParamNameHash);
		CharacterManagementIdleTimer = CharacterManagementInsertMaxDelay;
	}

	public void AddController(string animationId, RuntimeAnimatorController controller)
	{
		AnimatorSetup item = new AnimatorSetup
		{
			Controller = controller,
			Id = animationId
		};
		animators.Add(item);
	}

	public void SetController(string animationId, string fallbackAnimationId = "")
	{
		RuntimeAnimatorController controller = GetController(animationId);
		ControllerId = animationId;
		if (controller == null && animationId != fallbackAnimationId && fallbackAnimationId != "")
		{
			controller = GetController(fallbackAnimationId);
			ControllerId = fallbackAnimationId;
		}
		if (controller == null && animators.Count > 0)
		{
			Debug.LogWarning("Could not find controller with ID = '" + animationId + "' defaulting to first controller in list.");
			controller = animators[0].Controller;
			ControllerId = animators[0].Id;
		}
		if (controller != null)
		{
			SetAnimator(controller);
		}
		else
		{
			Debug.LogError("Could not find controller for animationId '" + animationId + "'");
		}
	}

	public void NotifyWeaponSwitch()
	{
		if (!WeaponSwitchNotificationSent)
		{
			WeaponSwitchNotificationSent = true;
			this.WeaponSwitched?.Invoke();
		}
	}

	public void NotifyWeaponRaised(bool raised)
	{
		this.WeaponRaised?.Invoke(raised);
	}

	protected override void Update()
	{
		base.Update();
		if (base.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == CharacterAnimationController.IdleStateNameHash && !base.Animator.IsInTransition(0) && deactivatingCustomAnimation)
		{
			RestoreState();
			deactivatingCustomAnimation = false;
		}
		UpdateWeaponPose();
		UpdateIdleStance();
		UpdateCharacterManagement();
	}

	public override void SetIdleStance(IdleStance stance)
	{
		switch (stance)
		{
		case IdleStance.Stand:
			desiredIdleStance = 0f;
			break;
		case IdleStance.HalfCover:
			desiredIdleStance = 1f;
			break;
		}
	}

	private void StoreState()
	{
		if (previousRuntimeAnimatorController == null)
		{
			previousRuntimeAnimatorController = base.Animator.runtimeAnimatorController;
			previousStateInfo = base.Animator.GetCurrentAnimatorStateInfo(0);
		}
	}

	private void RestoreState()
	{
		if (previousRuntimeAnimatorController != null)
		{
			SetAnimator(previousRuntimeAnimatorController);
			base.Animator.CrossFade(previousStateInfo.fullPathHash, 0f);
			previousRuntimeAnimatorController = null;
		}
	}

	private void RaiseWeapon()
	{
		WeaponPose currentWeaponPose = CurrentWeaponPose;
		if (currentWeaponPose != WeaponPose.Raised && currentWeaponPose != WeaponPose.BeingRaised)
		{
			desiredWeaponPoseChangeTime = Time.time;
			if (base.IsReloading)
			{
				ForceRaiseWeapon();
				return;
			}
			NotifyWeaponRaised(raised: true);
			base.Animator.ResetTrigger(RaiseWeaponParamNameHash);
			base.Animator.SetTrigger(RaiseWeaponParamNameHash);
			base.Animator.ResetTrigger(LowerWeaponParamNameHash);
			_ = VerboseDebug;
		}
	}

	private void LowerWeapon()
	{
		WeaponPose currentWeaponPose = CurrentWeaponPose;
		if (currentWeaponPose != WeaponPose.Lowered && currentWeaponPose != WeaponPose.BeingLowered)
		{
			desiredWeaponPoseChangeTime = Time.time;
			NotifyWeaponRaised(raised: false);
			base.Animator.ResetTrigger(LowerWeaponParamNameHash);
			base.Animator.SetTrigger(LowerWeaponParamNameHash);
			base.Animator.ResetTrigger(RaiseWeaponParamNameHash);
			_ = VerboseDebug;
		}
	}

	public void SetWeaponPoseUpdate(bool enabled)
	{
		weaponPoseUpdateDisabled = !enabled;
	}

	private void UpdateWeaponPose()
	{
		if (!weaponPoseUpdateDisabled && Time.time - desiredWeaponPoseChangeTime >= desiredWeaponPoseChangeReactionTime)
		{
			if (!IsSwitchingWeapon && DesiredWeaponPose == WeaponPose.Raised && CurrentWeaponPose != WeaponPose.Raised && CurrentWeaponPose != WeaponPose.BeingRaised)
			{
				RaiseWeapon();
			}
			else if (DesiredWeaponPose == WeaponPose.Lowered && CurrentWeaponPose != WeaponPose.Lowered && CurrentWeaponPose != WeaponPose.BeingLowered)
			{
				LowerWeapon();
			}
		}
	}

	private void UpdateIdleStance()
	{
		float num = base.Animator.GetFloat(IdleStanceParamNameHash);
		if (num < desiredIdleStance)
		{
			num = Mathf.Clamp01(num + Time.deltaTime / IdleStanceBlendTime);
		}
		else if (num > desiredIdleStance)
		{
			num = Mathf.Clamp01(num - Time.deltaTime / IdleStanceBlendTime);
		}
		base.Animator.SetFloat(IdleStanceParamNameHash, num);
	}

	public override void UseWeapon(bool criticalDamage, bool useFenceAttack, bool useChargeAttack)
	{
		base.UseWeapon(criticalDamage: false, useFenceAttack: false, useChargeAttack: false);
		float value = (int)(Random.value * 1000f) % 3;
		float value2 = (criticalDamage ? 1f : 0f);
		base.Animator.SetFloat(UseWeaponVariationParamNameHash, value);
		base.Animator.SetTrigger(UseWeaponParamNameHash);
		base.Animator.SetFloat(CharacterAnimationController.CriticalDamageParamNameHash, value2);
		base.Animator.SetBool(UseWeaponFenceParamNameHash, useFenceAttack);
		base.Animator.SetBool(UseWeaponChargeParamNameHash, useChargeAttack && hasChargeAnimation && !useFenceAttack);
		_ = VerboseDebug;
	}

	public void StartPush()
	{
		if (!IsPushRequested)
		{
			base.Animator.SetBool(PushParamNameHash, value: true);
		}
	}

	public void StopPush()
	{
		base.Animator.SetBool(PushParamNameHash, value: false);
	}

	public override void EnsureIdle()
	{
		base.EnsureIdle();
		if (!IsIdle)
		{
			EndEnvironmentAnimation(completed: false);
			LowerWeapon();
			base.Animator.SetBool(CustomAnimationParamNameHash, value: false);
		}
		DesiredWeaponPose = WeaponPose.Lowered;
	}

	public bool IsCustomAnimationPlaying()
	{
		return base.Animator.GetBool(CustomAnimationParamNameHash);
	}

	public void StartCustomAnimation(AnimationClip customAnimationClip)
	{
		EnsureIdle();
		StoreState();
		RuntimeAnimatorController runtimeAnimatorController = base.Animator.runtimeAnimatorController;
		AnimatorOverrideController animatorOverrideController = runtimeAnimatorController as AnimatorOverrideController;
		if (animatorOverrideController != null)
		{
			runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
		}
		AnimatorOverrideController animatorOverrideController2 = new AnimatorOverrideController();
		animatorOverrideController2.runtimeAnimatorController = runtimeAnimatorController;
		animatorOverrideController2["Survivor_CustomAnimation_Template"] = customAnimationClip;
		SetAnimator(animatorOverrideController2);
		base.Animator.SetBool(CustomAnimationParamNameHash, value: true);
		base.Animator.speed *= Random.Range(0.7f, 1f);
	}

	public void StopCustomAnimation()
	{
		base.Animator.SetBool(CustomAnimationParamNameHash, value: false);
		deactivatingCustomAnimation = true;
	}

	public void SwitchWeapon()
	{
		EnsureIdle();
		WeaponSwitchNotificationSent = false;
		base.Animator.ResetTrigger(RaiseWeaponParamNameHash);
		DesiredWeaponPose = WeaponPose.Lowered;
		_ = VerboseDebug;
	}

	public void ForceRaiseWeapon()
	{
		base.Animator.ResetTrigger(LowerWeaponParamNameHash);
		base.Animator.ResetTrigger(RaiseWeaponParamNameHash);
		base.Animator.CrossFade(CharacterAnimationController.RaiseWeaponStateNameHash, 0f);
		NotifyWeaponRaised(raised: true);
		_ = VerboseDebug;
	}

	public void StartEnvironmentAnimation(EnvironmentAnimation environmentAnimation)
	{
		if (!base.Animator || base.Animator.runtimeAnimatorController == null)
		{
			return;
		}
		AnimatorOverrideController animatorOverrideController = base.Animator.runtimeAnimatorController as AnimatorOverrideController;
		if (!animatorOverrideController)
		{
			return;
		}
		AnimatorOverrideController animatorOverrideController2 = new AnimatorOverrideController();
		animatorOverrideController2.runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
		if (defaultStartInteractionClip == null)
		{
			defaultStartInteractionClip = animatorOverrideController["Survivor_EnterLoot_Normal"];
			defaultLoopInteractionClip = animatorOverrideController["Survivor_LoopLoot_Normal_A"];
			defaultEndInteractionClip = animatorOverrideController["Survivor_LeaveLoot_Normal"];
			defaultCompletedInteractionClip = animatorOverrideController["Survivor_OpenLoot_Normal_A"];
		}
		AnimationClip animationClip = ((environmentAnimation != null) ? environmentAnimation.GetAnimationClip(EnvironmentAnimationType.StartInteraction) : null);
		AnimationClip animationClip2 = ((environmentAnimation != null) ? environmentAnimation.GetAnimationClip(EnvironmentAnimationType.LoopInteraction) : null);
		AnimationClip animationClip3 = ((environmentAnimation != null) ? environmentAnimation.GetAnimationClip(EnvironmentAnimationType.EndInteraction) : null);
		AnimationClip animationClip4 = ((environmentAnimation != null) ? environmentAnimation.GetAnimationClip(EnvironmentAnimationType.CompleteInteraction) : null);
		List<KeyValuePair<AnimationClip, AnimationClip>> list = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
		animatorOverrideController.GetOverrides(list);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			KeyValuePair<AnimationClip, AnimationClip> keyValuePair = list[i];
			if (keyValuePair.Key.name == "Survivor_EnterLoot_Normal")
			{
				list[i] = new KeyValuePair<AnimationClip, AnimationClip>(keyValuePair.Key, (animationClip != null) ? animationClip : defaultStartInteractionClip);
			}
			else if (keyValuePair.Key.name == "Survivor_LoopLoot_Normal_A")
			{
				list[i] = new KeyValuePair<AnimationClip, AnimationClip>(keyValuePair.Key, (animationClip2 != null) ? animationClip2 : defaultLoopInteractionClip);
			}
			else if (keyValuePair.Key.name == "Survivor_LeaveLoot_Normal")
			{
				list[i] = new KeyValuePair<AnimationClip, AnimationClip>(keyValuePair.Key, (animationClip3 != null) ? animationClip3 : defaultEndInteractionClip);
			}
			else if (keyValuePair.Key.name == "Survivor_OpenLoot_Normal_A")
			{
				list[i] = new KeyValuePair<AnimationClip, AnimationClip>(keyValuePair.Key, (animationClip4 != null) ? animationClip4 : defaultCompletedInteractionClip);
			}
		}
		animatorOverrideController2.ApplyOverrides(list);
		SetAnimator(animatorOverrideController2);
		base.Animator.SetBool(InteractParamNameHash, value: true);
	}

	public void EndEnvironmentAnimation(bool completed)
	{
		base.Animator.SetBool(InteractionCompletedParamNameHash, completed);
		base.Animator.SetBool(InteractParamNameHash, value: false);
	}

	public void NotifyInteractionComplete()
	{
		this.InteractionCompleted?.Invoke();
	}

	private void OnInteractionComplete()
	{
		NotifyInteractionComplete();
		_ = VerboseDebug;
	}

	public void PlaySound(string eventName)
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName);
		}
	}

	public void PlayGoreSound(string eventName)
	{
		if (!GameManager.Instance.IsGoreDisabled && SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName);
		}
	}

	public override void ForceIdle()
	{
		resumeToState = base.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		base.Animator.Play(CharacterAnimationController.IdleStateNameHash);
		base.Animator.Update(1f);
	}

	public void ResumeAfterForce()
	{
		if (resumeToState != 0)
		{
			base.Animator.Play(resumeToState);
			base.Animator.Update(1f);
			resumeToState = 0;
		}
	}
}
