using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
	protected static int MoveParamNameHash = Animator.StringToHash("Move");

	protected static int UseStopMoveParamNameHash = Animator.StringToHash("UseStopMove");

	protected static int StruggleParamNameHash = Animator.StringToHash("Struggle");

	protected static int TakeDamageParamNameHash = Animator.StringToHash("TakeDamage");

	protected static int MoveSpeedParamNameHash = Animator.StringToHash("MoveSpeed");

	protected static int DamageVariationParamNameHash = Animator.StringToHash("DamageVariation");

	protected static int DamageDirectionParamNameHash = Animator.StringToHash("DamageDirection");

	protected static int CriticalDamageParamNameHash = Animator.StringToHash("CriticalDamage");

	protected static int DeathParamNameHash = Animator.StringToHash("Death");

	protected static int StruggleDeathParamNameHash = Animator.StringToHash("StruggleDeath");

	protected static int StruggleVariationParamNameHash = Animator.StringToHash("StruggleVariation");

	protected static int RandomParamNameHash = Animator.StringToHash("Random");

	private static int QuickHitDirectionAParamNameHash = Animator.StringToHash("QuickHitDirectionA");

	private static int QuickHitDirectionBParamNameHash = Animator.StringToHash("QuickHitDirectionB");

	private static int QuickHitParamNameHash = Animator.StringToHash("QuickHit");

	protected static int ReloadParamNameHash = Animator.StringToHash("Reload");

	protected static int ForceIdleParamNameHash = Animator.StringToHash("ForceIdle");

	public static int QuickHitAStateNameHash = Animator.StringToHash("TakeHit.QuickHitA");

	public static int QuickHitBStateNameHash = Animator.StringToHash("TakeHit.QuickHitB");

	protected static int DeathStateNameHash = Animator.StringToHash("Base Layer.Death");

	protected static int IdleStateNameHash = Animator.StringToHash("Base Layer.Idle");

	protected static int StruggleStateNameHash = Animator.StringToHash("Base Layer.Struggle");

	protected static int EnterStruggleStateNameHash = Animator.StringToHash("Base Layer.EnterStruggle");

	protected static int UseWeaponStateNameHash = Animator.StringToHash("Base Layer.UseWeapon");

	protected static int RaiseWeaponStateNameHash = Animator.StringToHash("Base Layer.RaiseWeapon");

	protected static int LowerWeaponStateNameHash = Animator.StringToHash("Base Layer.LowerWeapon");

	protected static int StopMoveStateNameHash = Animator.StringToHash("Base Layer.StopMove");

	protected static int ReloadingStateNameHash = Animator.StringToHash("Base Layer.Reloading");

	private static string ChargeEquipmentBaseClipName = "Chainsaw_UseWeaponCritical";

	protected bool hasChargeAnimation;

	public float MoveSpeedInterpTime = 0.3f;

	public bool blendDirectionalDamageAnimations;

	protected Dictionary<string, Rigidbody> bodyPartMap = new Dictionary<string, Rigidbody>();

	private float ragdollStartTime;

	public float struggleVariationTarget;

	public Vector3 LastDeltaMovement;

	public float LastDeltaMovementMagnitude;

	public Quaternion LastDeltaRotation;

	private List<SkinnedMeshRenderer> blendShapeRenderers;

	private ActorView actorView;

	private Animator animator;

	private bool enablePortraitShapeKey;

	private float portraitShapeWeight;

	public ActorView ActorView
	{
		get
		{
			if (actorView == null)
			{
				actorView = GetComponent<ActorView>();
			}
			return actorView;
		}
	}

	protected AnimatorOverrideController AnimatorOverrideController => Animator.runtimeAnimatorController as AnimatorOverrideController;

	public bool IsValid
	{
		get
		{
			if (Animator != null)
			{
				return Animator.gameObject.activeInHierarchy;
			}
			return false;
		}
	}

	protected Animator Animator
	{
		get
		{
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			return animator;
		}
		private set
		{
			animator = value;
		}
	}

	private bool UseWeaponNotified { get; set; }

	protected float MoveSpeed { get; set; }

	protected float TargetMoveSpeed { get; set; }

	public ControlState ControlState
	{
		get
		{
			if (!Animator.enabled)
			{
				foreach (Rigidbody value in bodyPartMap.Values)
				{
					if (value.isKinematic)
					{
						return ControlState.None;
					}
				}
				return ControlState.Ragdoll;
			}
			return ControlState.Animation;
		}
	}

	public bool IsRagdollSleeping
	{
		get
		{
			if (ControlState != ControlState.Ragdoll)
			{
				return false;
			}
			foreach (Rigidbody value in bodyPartMap.Values)
			{
				if (!value.IsSleeping())
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool IsStopping
	{
		get
		{
			if (Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != StopMoveStateNameHash && Animator.GetNextAnimatorStateInfo(0).fullPathHash != StopMoveStateNameHash)
			{
				if (Animator.GetNextAnimatorStateInfo(0).fullPathHash == IdleStateNameHash)
				{
					return Animator.IsInTransition(0);
				}
				return false;
			}
			return true;
		}
	}

	public bool IsInStopMove
	{
		get
		{
			if (Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == StopMoveStateNameHash)
			{
				return !Animator.IsInTransition(0);
			}
			return false;
		}
	}

	public bool IsInTransition => Animator.IsInTransition(0);

	public bool HasForceEndPosition => Animator.isMatchingTarget;

	public bool IsStruggling => Animator.GetBool(StruggleParamNameHash);

	public bool IsDeathRequested
	{
		get
		{
			if (Animator.GetNextAnimatorStateInfo(0).fullPathHash != DeathStateNameHash)
			{
				return Animator.GetBool(DeathParamNameHash);
			}
			return true;
		}
	}

	public bool IsInDeath
	{
		get
		{
			bool num = Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == DeathStateNameHash;
			bool flag = Animator.IsInTransition(0);
			if (!num || flag)
			{
				return ControlState != ControlState.Animation;
			}
			return true;
		}
	}

	public virtual bool IsIdle
	{
		get
		{
			if (Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == IdleStateNameHash)
			{
				return !Animator.IsInTransition(0);
			}
			return false;
		}
	}

	public bool IsReloading
	{
		get
		{
			if (Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == ReloadingStateNameHash)
			{
				return !Animator.IsInTransition(0);
			}
			return false;
		}
	}

	public bool IsMoveRequested => Animator.GetBool(MoveParamNameHash);

	public event UseWeaponHandler OnUseWeapon;

	public event QuickHitHandler OnTakeQuickHit;

	public event MoveHandler OnMove;

	protected void SetAnimator(AnimatorOverrideController newAnimator)
	{
		Animator.runtimeAnimatorController = newAnimator;
		CheckForChargeAnimation(newAnimator);
	}

	protected void SetAnimator(RuntimeAnimatorController newAnimator)
	{
		Animator.runtimeAnimatorController = newAnimator;
		CheckForChargeAnimation(newAnimator as AnimatorOverrideController);
	}

	private void CheckForChargeAnimation(AnimatorOverrideController overrideController)
	{
		if (!(overrideController != null))
		{
			return;
		}
		List<KeyValuePair<AnimationClip, AnimationClip>> list = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
		overrideController.GetOverrides(list);
		foreach (KeyValuePair<AnimationClip, AnimationClip> item in list)
		{
			if (item.Key.name == ChargeEquipmentBaseClipName && item.Value != null)
			{
				hasChargeAnimation = true;
				break;
			}
		}
	}

	private void Start()
	{
		blendShapeRenderers = null;
		UpdateBlendShapeRenderers();
	}

	public void UpdateBlendShapeRenderers()
	{
		blendShapeRenderers?.Clear();
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].sharedMesh != null && componentsInChildren[i].sharedMesh.blendShapeCount > 0)
			{
				if (blendShapeRenderers == null)
				{
					blendShapeRenderers = new List<SkinnedMeshRenderer>();
				}
				if (!blendShapeRenderers.Contains(componentsInChildren[i]))
				{
					blendShapeRenderers.Add(componentsInChildren[i]);
				}
			}
		}
	}

	private void NotifyUseWeapon(bool preImpact)
	{
		if (!UseWeaponNotified)
		{
			if (!preImpact)
			{
				UseWeaponNotified = true;
			}
			this.OnUseWeapon?.Invoke(preImpact);
		}
	}

	private void NotifyMove(bool moving)
	{
		this.OnMove?.Invoke(moving);
	}

	public virtual void QuickHit(string direction, EquipmentType equipmentType, Vector3 attackerPosition, DamageAction action, string subCategory = null)
	{
		float num = 0f;
		if (direction.ToLower() == "right")
		{
			num = 0.25f;
		}
		else if (direction.ToLower() == "back")
		{
			num = 0.5f;
		}
		else if (direction.ToLower() == "left")
		{
			num = 0.75f;
		}
		Vector3 normalized = (base.gameObject.transform.position - attackerPosition).normalized;
		if (direction.ToLower() != "none")
		{
			float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-base.gameObject.transform.forward, normalized), -1f, 1f));
			if (Vector3.Dot(base.gameObject.transform.right, normalized) >= 0f)
			{
				num2 = MathF.PI * 2f - num2;
			}
			num -= num2 / (MathF.PI * 2f);
			if (num < 0f)
			{
				num += 1f;
			}
			if (Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != QuickHitAStateNameHash && Animator.GetNextAnimatorStateInfo(0).fullPathHash != QuickHitAStateNameHash)
			{
				Animator.SetFloat(QuickHitDirectionAParamNameHash, num);
			}
			else
			{
				Animator.SetFloat(QuickHitDirectionBParamNameHash, num);
			}
			Animator.SetTrigger(QuickHitParamNameHash);
		}
		Vector3 impactDirection = normalized;
		QuickHitProfile quickHitProfile = ImpactProfileManager.Instance.GetQuickHitProfile(equipmentType, subCategory);
		if (quickHitProfile != null && quickHitProfile.effectPrefabResource != null && (action == null || !action.Dodged || action.Critical))
		{
			SpawnQuickHitEffect(quickHitProfile.effectPrefabResource.GetPrefab(), impactDirection, quickHitProfile);
		}
		if (quickHitProfile != null && quickHitProfile.ricochetPrefabResource != null && ActorView != null && ActorView.Model.IsImpenetrable)
		{
			SpawnQuickHitEffect(quickHitProfile.ricochetPrefabResource.GetPrefab(), impactDirection, quickHitProfile);
		}
	}

	private void SpawnQuickHitEffect(GameObject prefab, Vector3 impactDirection, QuickHitProfile profile)
	{
		Vector3 position = base.gameObject.transform.position;
		Quaternion rotation = base.gameObject.transform.rotation;
		switch (profile.effectSpawnDirection)
		{
		case EffectSpawnDirection.CharacterDirection:
			rotation = Quaternion.LookRotation(base.gameObject.transform.forward);
			break;
		case EffectSpawnDirection.NegativeCharacterDirection:
			rotation = Quaternion.LookRotation(-base.gameObject.transform.forward);
			break;
		case EffectSpawnDirection.ImpactDirection:
			rotation = Quaternion.LookRotation(impactDirection);
			break;
		case EffectSpawnDirection.NegativeImpactDirection:
			rotation = Quaternion.LookRotation(-impactDirection);
			break;
		}
		UnityEngine.Object.Instantiate(prefab, position, rotation);
	}

	private void NotifyQuickHit(string direction)
	{
		this.OnTakeQuickHit?.Invoke(direction);
	}

	private void OnWeaponPreImpact()
	{
		NotifyUseWeapon(preImpact: true);
	}

	private void OnWeaponImpact()
	{
		NotifyUseWeapon(preImpact: false);
	}

	private void OnQuickHit(string direction)
	{
		NotifyQuickHit(direction);
	}

	private void OnSetPortraitShapeKeyOn()
	{
		enablePortraitShapeKey = true;
	}

	private void OnSetPortraitShapeKeyOff()
	{
		enablePortraitShapeKey = false;
	}

	private void UpdatePortraitShapeKey()
	{
		float num = 200f;
		float num2 = portraitShapeWeight;
		if (enablePortraitShapeKey && portraitShapeWeight < 100f)
		{
			portraitShapeWeight = Math.Min(portraitShapeWeight + num * Time.deltaTime, 100f);
		}
		if (!enablePortraitShapeKey && portraitShapeWeight > 0f)
		{
			portraitShapeWeight = Math.Max(portraitShapeWeight - num * Time.deltaTime, 0f);
		}
		if (portraitShapeWeight != num2)
		{
			for (int i = 0; i < blendShapeRenderers.Count; i++)
			{
				blendShapeRenderers[i].SetBlendShapeWeight(0, portraitShapeWeight);
			}
			num2 = portraitShapeWeight;
		}
	}

	private void OnPlaySound(string soundEventName)
	{
		if (GameManager.Instance.playerModel.Combat != null && SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(soundEventName, base.gameObject);
		}
	}

	private void OnPlaySoundType(string typeName)
	{
		if (!(SingularityMonoBehaviour<AudioManager>.Instance != null) || !SingularityMonoBehaviour<AudioManager>.Instance.combatSfxLoaded)
		{
			return;
		}
		ActorModel model = ActorView.Model;
		bool flag = false;
		if (model == null)
		{
			Debug.LogError("CharacterAnimationController: OnPlaySoundType '" + typeName + "' - actorModel is NULL!");
			return;
		}
		string text = Enum.GetName(typeof(Faction), model.Faction);
		if (text == "Raider")
		{
			text = "Survivor";
		}
		string text2 = Enum.GetName(typeof(ActorGender), model.Gender);
		string text3 = "";
		string text4 = ActorView.EquipmentTypeSoundOverride;
		EquipmentItemModel selectedEquipment = model.SelectedEquipment;
		bool flag2 = false;
		FireWeaponVisualizationTask getMostRecentFireWeaponVisualizationTask = ActorView.GetMostRecentFireWeaponVisualizationTask;
		if (getMostRecentFireWeaponVisualizationTask != null && getMostRecentFireWeaponVisualizationTask.WeaponAbility.Definition != null && getMostRecentFireWeaponVisualizationTask.WeaponAbility.IsChargeAttack)
		{
			flag2 = true;
		}
		if (flag2 && !string.IsNullOrEmpty(ActorView.ChargedEquipmentTypeSoundOverride))
		{
			text4 = ActorView.ChargedEquipmentTypeSoundOverride;
		}
		if (!string.IsNullOrEmpty(text4))
		{
			text3 = text4;
		}
		else if (selectedEquipment != null)
		{
			text3 = Enum.GetName(typeof(EquipmentType), selectedEquipment.Definition.Type);
		}
		string text5 = "";
		try
		{
			switch ((SoundType)Enum.Parse(typeof(SoundType), typeName))
			{
			case SoundType.UseWeapon:
				text5 = "combat_weapon/" + text + "_" + text3 + "_use";
				ActorView.PlayEquipmentSound(active: false);
				break;
			case SoundType.UseWeaponCritical:
				text5 = "combat_weapon/" + text + "_" + text3 + "_use_critical";
				break;
			case SoundType.HitWeapon:
				if (ActorView.IsTargetInCover)
				{
					text5 = "combat_weapon/ranged_cover_hit";
					break;
				}
				text5 = "combat_weapon/" + text + "_" + text3 + "_hit";
				if (!ActorView.IsTargetHuman && model.Faction != Faction.Walker && !ActorView.IsTargetEnvironmentalActor)
				{
					flag = true;
				}
				break;
			case SoundType.HitWeaponCritical:
				text5 = "combat_weapon/" + text + "_" + text3 + "_hit_critical";
				if (!ActorView.IsTargetHuman && model.Faction != Faction.Walker && !ActorView.IsTargetEnvironmentalActor)
				{
					flag = true;
				}
				break;
			case SoundType.FootStep:
			{
				string text6 = "gravel";
				if (CombatView.Instance != null)
				{
					text6 = Enum.GetName(typeof(GroundType), CombatView.Instance.CurrentMissionGroundType);
				}
				text5 = "combat_" + text + "/footstep_" + text6;
				break;
			}
			case SoundType.Push:
			{
				string text6 = "gravel";
				if (CombatView.Instance != null)
				{
					text6 = Enum.GetName(typeof(GroundType), CombatView.Instance.CurrentMissionGroundType);
				}
				text5 = "combat_survivor/survivor_push_" + text6.ToLower();
				break;
			}
			case SoundType.HitSurvivor:
				if (ActorView.IsTargetInCover)
				{
					return;
				}
				text5 = "combat_survivor/survivor_" + text2 + "_hit";
				break;
			case SoundType.HitSurvivorCritical:
				text5 = "combat_survivor/survivor_" + text2 + "_hit_critical";
				break;
			}
		}
		catch (ArgumentException)
		{
			Debug.LogWarning("Failed to parse SoundType '" + typeName + "'");
		}
		if (!ActorView.IsAttackDodged)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(text5.ToLower(), base.gameObject);
		}
		if (flag)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_weapon/gore_hit", base.gameObject);
		}
	}

	private void OnPushStart()
	{
	}

	private void OnAnimatorMove()
	{
		LastDeltaMovement = Animator.deltaPosition;
		LastDeltaMovementMagnitude = LastDeltaMovement.magnitude;
		LastDeltaRotation = Animator.deltaRotation;
	}

	private void Awake()
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			bodyPartMap.Add(componentsInChildren[i].gameObject.name, componentsInChildren[i]);
		}
		DisableRagdoll(enableAnimator: true, disableCollisions: true);
	}

	public void EnableRagdoll()
	{
		foreach (Rigidbody value in bodyPartMap.Values)
		{
			value.isKinematic = false;
			value.GetComponent<Collider>().enabled = true;
			value.detectCollisions = true;
		}
		Animator.enabled = false;
		ragdollStartTime = Time.time;
	}

	public void DisableRagdoll(bool enableAnimator = true, bool disableCollisions = false)
	{
		foreach (Rigidbody value in bodyPartMap.Values)
		{
			value.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			value.isKinematic = true;
			value.GetComponent<Collider>().enabled = !disableCollisions;
			value.detectCollisions = !disableCollisions;
		}
		Animator.enabled = enableAnimator;
	}

	public void ApplyImpactProfile(ImpactProfile profile, Vector3 impactDirection, Vector3 attackDirection)
	{
		for (int i = 0; i < profile.ImpactConfigurations.Count; i++)
		{
			if (!bodyPartMap.ContainsKey(profile.ImpactConfigurations[i].bodyPartName))
			{
				continue;
			}
			Rigidbody rigidbody = bodyPartMap[profile.ImpactConfigurations[i].bodyPartName];
			if (profile.ImpactConfigurations[i].detachBodyPartType != DetachType.DontDetach && !GameManager.Instance.IsGoreDisabled)
			{
				rigidbody.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
				rigidbody.gameObject.GetComponent<Collider>().enabled = false;
				if (profile.ImpactConfigurations[i].detachBodyPartType == DetachType.DetachSpawnReplacement)
				{
					GameObject detachmentPrefab = ActorView.GetDetachmentPrefab(profile.ImpactConfigurations[i].bodyPartName);
					if (detachmentPrefab != null)
					{
						GameObject obj = UnityEngine.Object.Instantiate(detachmentPrefab);
						obj.transform.position = rigidbody.transform.position;
						obj.transform.rotation = rigidbody.transform.rotation;
						obj.transform.localScale = new Vector3(1f, 1f, 1f);
						Rigidbody componentInChildren = obj.GetComponentInChildren<Rigidbody>();
						if (componentInChildren != null)
						{
							rigidbody = componentInChildren;
						}
					}
				}
			}
			Vector3 vector = impactDirection;
			switch (profile.ImpactConfigurations[i].forceDirectionType)
			{
			case ForceDirectionType.ImpactDirection:
				vector = impactDirection;
				break;
			case ForceDirectionType.CharacterSpace:
				vector = base.transform.localToWorldMatrix.MultiplyVector(profile.ImpactConfigurations[i].forceDirection);
				break;
			case ForceDirectionType.WorldSpace:
				vector = profile.ImpactConfigurations[i].forceDirection;
				break;
			case ForceDirectionType.AttackDirection:
				vector = attackDirection;
				break;
			}
			Vector3 position = rigidbody.transform.position + base.transform.localToWorldMatrix.MultiplyVector(profile.ImpactConfigurations[i].forceOffset);
			float num = 0.3f;
			Vector3 vector2 = new Vector3(num * (UnityEngine.Random.value - 0.5f), 0f, num * (UnityEngine.Random.value - 0.5f));
			vector += vector2;
			if (profile.ImpactConfigurations[i].effectPrefabResource != null)
			{
				Vector3 position2 = base.gameObject.transform.position;
				Quaternion rotation = base.gameObject.transform.rotation;
				switch (profile.ImpactConfigurations[i].effectSpawnDirection)
				{
				case EffectSpawnDirection.CharacterDirection:
					rotation = Quaternion.LookRotation(base.gameObject.transform.forward + vector2);
					break;
				case EffectSpawnDirection.NegativeCharacterDirection:
					rotation = Quaternion.LookRotation(-base.gameObject.transform.forward + vector2);
					break;
				case EffectSpawnDirection.ImpactDirection:
					rotation = Quaternion.LookRotation(impactDirection.normalized + vector2);
					break;
				case EffectSpawnDirection.NegativeImpactDirection:
					rotation = Quaternion.LookRotation(-impactDirection.normalized + vector2);
					break;
				case EffectSpawnDirection.AttackDirection:
					rotation = Quaternion.LookRotation(attackDirection + vector2);
					break;
				case EffectSpawnDirection.NegativeAttackDirection:
					rotation = Quaternion.LookRotation(-attackDirection + vector2);
					break;
				}
				if (profile.ImpactConfigurations[i].effectSpawnLocation == EffectSpawnLocation.HitBodypart)
				{
					position2 = rigidbody.transform.position;
				}
				UnityEngine.Object.Instantiate(profile.ImpactConfigurations[i].effectPrefabResource.GetPrefab(), position2, rotation);
			}
			rigidbody.AddForceAtPosition(vector * profile.ImpactConfigurations[i].forceMagnitude, position, ForceMode.Impulse);
		}
	}

	public void AddImpulseToRagdoll(string bodyPartName, Vector3 position, Vector3 impulse)
	{
		if (bodyPartMap.ContainsKey(bodyPartName))
		{
			Rigidbody rigidbody = bodyPartMap[bodyPartName];
			if (rigidbody != null)
			{
				rigidbody.AddForceAtPosition(impulse, position, ForceMode.Impulse);
			}
		}
	}

	protected virtual void Update()
	{
		if (IsMoveRequested)
		{
			if (MoveSpeed < TargetMoveSpeed)
			{
				MoveSpeed += Time.deltaTime / MoveSpeedInterpTime;
				if (MoveSpeed > TargetMoveSpeed)
				{
					MoveSpeed = TargetMoveSpeed;
				}
			}
			if (MoveSpeed > TargetMoveSpeed)
			{
				MoveSpeed -= Time.deltaTime / MoveSpeedInterpTime;
				if (MoveSpeed < TargetMoveSpeed)
				{
					MoveSpeed = TargetMoveSpeed;
				}
			}
			Animator.SetFloat(MoveSpeedParamNameHash, MoveSpeed);
		}
		if (blendShapeRenderers != null)
		{
			UpdatePortraitShapeKey();
		}
		if (IsStruggling)
		{
			float num = Animator.GetFloat(StruggleVariationParamNameHash);
			if (num < struggleVariationTarget)
			{
				num += Time.deltaTime / 0.5f;
			}
			else if (num > struggleVariationTarget)
			{
				num -= Time.deltaTime / 0.5f;
			}
			Animator.SetFloat(StruggleVariationParamNameHash, Mathf.Clamp(num, 0f, 1f));
		}
		AnimatorStateInfo currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
		AnimatorStateInfo nextAnimatorStateInfo = Animator.GetNextAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.fullPathHash == UseWeaponStateNameHash && nextAnimatorStateInfo.fullPathHash == LowerWeaponStateNameHash)
		{
			NotifyUseWeapon(preImpact: false);
		}
		if (ControlState == ControlState.Ragdoll && IsRagdollSleeping && Time.time - ragdollStartTime > 1f)
		{
			DisableRagdoll(enableAnimator: false, disableCollisions: true);
		}
	}

	public virtual void UseWeapon(bool criticalDamage, bool useFenceAttack, bool useChargeAttack)
	{
		UseWeaponNotified = false;
	}

	public virtual void MeleeDamage(bool criticalDamage, Vector3 direction)
	{
		if (IsIdle || IsReloading)
		{
			Animator.SetTrigger(ForceIdleParamNameHash);
			float value = (criticalDamage ? 1f : 0f);
			Animator.SetFloat(CriticalDamageParamNameHash, value);
			float value2 = (int)(UnityEngine.Random.value * 1000f) % 3;
			Animator.SetFloat(DamageVariationParamNameHash, value2);
			float num = 0f;
			float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-base.gameObject.transform.forward, direction), -1f, 1f));
			if (Vector3.Dot(base.gameObject.transform.right, direction) >= 0f)
			{
				num2 = MathF.PI * 2f - num2;
			}
			num -= num2 / (MathF.PI * 2f);
			if (num < 0f)
			{
				num += 1f;
			}
			Animator.SetFloat(DamageDirectionParamNameHash, num);
			Animator.SetTrigger(TakeDamageParamNameHash);
		}
	}

	public void SetTargetMoveSpeed(float moveSpeed)
	{
		if (TargetMoveSpeed != moveSpeed)
		{
			TargetMoveSpeed = moveSpeed;
		}
	}

	public float GetTargetMoveSpeed()
	{
		return TargetMoveSpeed;
	}

	public void StartMove(float moveSpeed)
	{
		if (!IsMoveRequested)
		{
			EnsureIdle();
			Animator.SetBool(MoveParamNameHash, value: true);
			SetTargetMoveSpeed(moveSpeed);
			MoveSpeed = 0f;
			Animator.SetFloat(MoveSpeedParamNameHash, MoveSpeed);
			NotifyMove(moving: true);
		}
	}

	public void SetReloading(bool isReloading)
	{
		Animator.SetBool(ReloadParamNameHash, isReloading);
		if (isReloading)
		{
			Animator.ResetTrigger(ForceIdleParamNameHash);
		}
	}

	public virtual void ForceIdle()
	{
		if (!IsDeathRequested && !IsInDeath)
		{
			EnsureIdle();
			Animator.CrossFade(IdleStateNameHash, 0f);
		}
	}

	public virtual void SetIdleStance(IdleStance stance)
	{
	}

	public void StopMove(bool useStopMove = true)
	{
		Animator.SetBool(UseStopMoveParamNameHash, useStopMove);
		Animator.SetBool(MoveParamNameHash, value: false);
		TargetMoveSpeed = 0f;
		NotifyMove(moving: false);
	}

	public void ForceEndPosition(Vector3 position, Quaternion rotation, float endNormalizedTime = 0.95f)
	{
		Animator.MatchTarget(position, rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), 0f, endNormalizedTime);
	}

	public void ClearEndPosition()
	{
		Animator.InterruptMatchTarget(completeMatch: false);
	}

	public virtual void EnsureIdle()
	{
		if (!IsIdle)
		{
			ClearEndPosition();
			Animator.SetBool(MoveParamNameHash, value: false);
			Animator.SetBool(StruggleParamNameHash, value: false);
			Animator.SetTrigger(ForceIdleParamNameHash);
		}
	}

	public void EnterStruggle()
	{
		EnsureIdle();
		Animator.SetBool(StruggleParamNameHash, value: true);
		Animator.SetFloat(StruggleVariationParamNameHash, 0f);
		struggleVariationTarget = 0f;
	}

	public void SetSeriousStruggle(bool serious)
	{
		struggleVariationTarget = (serious ? 1f : 0f);
	}

	public void LeaveStruggle()
	{
		Animator.SetBool(StruggleParamNameHash, value: false);
	}

	public void Die(bool struggleDeath)
	{
		Animator.SetTrigger(ForceIdleParamNameHash);
		Animator.SetFloat(StruggleDeathParamNameHash, struggleDeath ? 1f : 0f);
		Animator.SetFloat(RandomParamNameHash, UnityEngine.Random.Range(0, 1000) % 3);
		Animator.SetBool(DeathParamNameHash, value: true);
	}

	public void Die(ImpactProfile profile, Vector3 impactDirection, Vector3 attackDirection)
	{
		EnableRagdoll();
		ApplyImpactProfile(profile, impactDirection, attackDirection);
	}
}
