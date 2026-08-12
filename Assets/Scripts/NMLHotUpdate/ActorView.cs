using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Utils;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class ActorView : ModelView<ActorModel>
{
	private const string WeaponAttachBoneName = "Bind_RightGunParent";

	private const string OtherWeaponAttachBoneName = "Bind_LeftGunParent";

	private const string StunAttachBoneName = "Bind_Head";

	public float jogSpeed = 5f;

	public float walkSpeed = 1.5f;

	public float meleeSpeed = 1f;

	public float FadeOutTime = 1.5f;

	[Tooltip("Disable additional dynamically loaded animations")]
	public bool DisableAnimationLoading;

	private float FadeOutTimer;

	private float RagdollWaitTimer;

	private bool FadeOutRequested;

	private GameObject currentWeaponPrefab;

	private GameObject currentOtherWeaponPrefab;

	private Transform spineAttachTarget;

	private ShadowBlobOrient shadowBlob;

	[SerializeField]
	[Tooltip("Configuration for detaching body parts. Specify bone name and prefab to spawn when detachment happens for that particular bone. Note that only physics body mapped bones can be detached.")]
	private List<DetachmentConfiguration> detachmentConfigurations;

	[SerializeField]
	private List<GameObject> disableOnDeath;

	[Tooltip("Objects that are affected by mirroring (for left-handed characters)")]
	public List<GameObject> mirroredRoots;

	private bool hasDeathVisualizationStarted;

	private bool showHealthIndicator;

	private ActorProductionIndicator productionIndicator;

	private float healthValue = 1f;

	private bool newTurn;

	private bool aiOverwatchIndicator;

	private Vector3 previousMeleeWeaponTipPosition;

	private Vector3 meleeWeaponImpactDirection;

	private GameObject indicatorParent;

	private GameObject characterSelectionIndicator;

	private SelectionMesh characterSelectionMesh;

	private EffectRumble characterSelectionRumble;

	private ActionPointIndicator characterSelectionActionPointIndicator;

	private SelectionMesh characterSelectionAP1Mesh;

	private SelectionMesh characterSelectionAP2Mesh;

	private GameObject characterSelectionChangeIndicatorPrefab;

	private GameObject stunIndicator;

	private GameObject staggerIndicator;

	private GameObject remoteIndicator;

	private GameObject chargeSelectionIndicator;

	private GameObject commandSkillSelectableIndicator;

	private GameObject commandSkillSelectedIndicator;

	private HealthIndicator healthIndicator;

	private GameObject healthBarPosition;

	private SpeechBubble speechBubble;

	private GameObject turnCountPosition;

	private GameObject goreSpawnerPrefab;

	private GameObject explosionPrefab;

	private GameObject activeExplosion;

	private GameObject abilityRangeIndicator;

	private WeaponRangeVisualization _abilityRangeVisualizer;

	private GameObject activationRangeIndicator;

	private WeaponRangeVisualization _activationRangeVisualizer;

	private GameObject herdLineIndicator;

	private HerdVisualizationLine herdVisualizationLine;

	private HealthBarInjuryTypeColors healthBarInjuryTypeColorsConfigInternal;

	private bool useModelForInitialPosition = true;

	private Renderer[] renderers;

	private Renderer[] weaponRenderers;

	private Renderer[] otherWeaponRenderers;

	private ActorNotificationManager notificationManager;

	private bool isCombatActor;

	public bool IsInvisible;

	[HideInInspector]
	public bool IsDeathInfoVisualized;

	private GameObject bossAuraFX;

	private GameObject ABTestA2Indicator;

	public bool UseModelForInitialPosition
	{
		get
		{
			return useModelForInitialPosition;
		}
		set
		{
			useModelForInitialPosition = value;
		}
	}

	public bool CanUpdateVisibility { get; set; }

	public BuildingView BuildingToCollect { get; set; }

	public bool IsVisibleToSurvivors { get; private set; }

	public bool FlankedNotificationShown { get; set; }

	public bool EnableCarolRealTimeUpdateDirection { get; set; }

	public bool MarkedNotificationShown { get; set; }

	public string EquipmentTypeSoundOverride { get; private set; }

	public string ChargedEquipmentTypeSoundOverride { get; private set; }

	public bool HasActiveTimedEffect => base.Model.ExclusiveTimedEffect != null;

	public bool LightWeight { get; set; }

	public EquipmentItemModel CurrentWeapon { get; private set; }

	private EquipmentItemModel RequestedWeapon { get; set; }

	public Vector3 WeaponImpactDirection
	{
		get
		{
			if (IsMeleeWeaponEquipped)
			{
				return meleeWeaponImpactDirection;
			}
			return base.transform.forward;
		}
	}

	public Vector3 ChargeWeaponImpactDirection
	{
		get
		{
			if (CurrentWeapon.ChargeEquipment == null)
			{
				return WeaponImpactDirection;
			}
			if (CurrentWeapon.ChargeEquipment.Definition.Category == EquipmentCategory.MeleeWeapon)
			{
				return meleeWeaponImpactDirection;
			}
			return base.transform.forward;
		}
	}

	public GameObject IndicatorParent
	{
		get
		{
			if (indicatorParent == null)
			{
				indicatorParent = new GameObject("OffsetParent");
				indicatorParent.transform.parent = base.transform;
				indicatorParent.transform.localPosition = new Vector3(0f, 2f, 0f);
				indicatorParent.transform.localScale = new Vector3(1f, 1f, 1f);
				indicatorParent.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			return indicatorParent;
		}
	}

	public bool IsMeleeWeaponEquipped
	{
		get
		{
			if (CurrentWeapon != null)
			{
				return CurrentWeapon.Definition.Category == EquipmentCategory.MeleeWeapon;
			}
			return false;
		}
	}

	public bool IsRangedWeaponEquipped
	{
		get
		{
			if (CurrentWeapon != null)
			{
				return CurrentWeapon.Definition.Category == EquipmentCategory.RangeWeapon;
			}
			return false;
		}
	}

	public bool IsTargetInCover { get; set; }

	public bool IsTargetHuman { get; set; }

	public bool IsTargetEnvironmentalActor { get; set; }

	public bool IsAttackDodged { get; set; }

	public CharacterAnimationController CharacterAnimationController { get; private set; }

	public HealthIndicator HealthIndicator => healthIndicator;

	public WeaponRangeVisualization AbilityRangeVisualizer
	{
		get
		{
			if (abilityRangeIndicator != null && _abilityRangeVisualizer == null)
			{
				_abilityRangeVisualizer = abilityRangeIndicator.GetComponent<WeaponRangeVisualization>();
			}
			return _abilityRangeVisualizer;
		}
	}

	public WeaponRangeVisualization ActivationRangeVisualizer
	{
		get
		{
			if (activationRangeIndicator != null && _activationRangeVisualizer == null)
			{
				_activationRangeVisualizer = activationRangeIndicator.GetComponent<WeaponRangeVisualization>();
			}
			return _activationRangeVisualizer;
		}
	}

	private HealthBarInjuryTypeColors healthBarInjuryTypeColorsConfig
	{
		get
		{
			if (healthBarInjuryTypeColorsConfigInternal == null)
			{
				healthBarInjuryTypeColorsConfigInternal = UnityUtils.LoadFromAssetBundle<HealthBarInjuryTypeColors>("HealthBarInjuryTypeColorsConfig", "scriptableobjects");
			}
			return healthBarInjuryTypeColorsConfigInternal;
		}
	}

	public bool IsWeaponCarrying => base.Model.IsHuman;

	public bool IsBleeding => base.Model.HasTrait("Bleeding");

	public bool IsBurning => base.Model.HasTrait("Burning");

	public bool SwitchingWeapon => RequestedWeapon != null;

	private bool IsCurrentlyRequestedWeaponMelee
	{
		get
		{
			SwitchWeaponVisualizationTask mostRecentlyAddedTask = VisualizationQueue.Instance.GetMostRecentlyAddedTask<SwitchWeaponVisualizationTask>(base.Model);
			if (mostRecentlyAddedTask != null)
			{
				return mostRecentlyAddedTask.SwitchToMelee;
			}
			EquipmentItemModel equipmentOfCategory = base.Model.GetEquipmentOfCategory(EquipmentCategory.MeleeWeapon);
			return CurrentWeapon == equipmentOfCategory;
		}
	}

	private bool IsFireWeaponTaskInQueue => GetMostRecentFireWeaponVisualizationTask != null;

	public FireWeaponVisualizationTask GetMostRecentFireWeaponVisualizationTask => VisualizationQueue.Instance.GetMostRecentlyAddedTask<FireWeaponVisualizationTask>(base.Model);

	public bool CanShowProductionIndicator
	{
		get
		{
			if (base.Model == null || base.Model.Producer == null || !base.Model.CanCollectProduction)
			{
				if (BuildingToCollect != null)
				{
					return BuildingToCollect.Model.Producer.HasEnoughToCollect;
				}
				return false;
			}
			return true;
		}
	}

	public GameObject GetDetachmentPrefab(string bodyPartName)
	{
		if (detachmentConfigurations != null)
		{
			for (int i = 0; i < detachmentConfigurations.Count; i++)
			{
				if (detachmentConfigurations[i].bodyPartName == bodyPartName)
				{
					return detachmentConfigurations[i].detachmentPrefab;
				}
			}
		}
		return null;
	}

	private void UpdateMeleeWeaponImpactDirection()
	{
		if (IsMeleeWeaponEquipped)
		{
			GameObject gameObject = GetCurrentWeaponPrefab();
			if (gameObject != null)
			{
				Vector3 vector = gameObject.transform.position - gameObject.transform.right * 0.5f;
				meleeWeaponImpactDirection = Vector3.Normalize(vector - previousMeleeWeaponTipPosition);
				previousMeleeWeaponTipPosition = vector;
			}
		}
		else
		{
			meleeWeaponImpactDirection = new Vector3(0f, 0f, 0f);
			previousMeleeWeaponTipPosition = new Vector3(0f, 0f, 0f);
		}
	}

	public float GetMoveSpeed(MoveSpeed moveSpeed)
	{
		if (moveSpeed != MoveSpeed.Jog)
		{
			return walkSpeed;
		}
		return jogSpeed;
	}

	public GameObject GetCurrentWeaponPrefab()
	{
		if (!(currentOtherWeaponPrefab != null))
		{
			return currentWeaponPrefab;
		}
		return currentOtherWeaponPrefab;
	}

	public GameObject GetWeaponVisualizationPrefab()
	{
		if (CurrentWeapon == null)
		{
			if (!(currentOtherWeaponPrefab != null))
			{
				return currentWeaponPrefab;
			}
			return currentOtherWeaponPrefab;
		}
		if (HelpersGfx.GetEquipmentResourceEntry(CurrentWeapon).useOtherHandOnCharged && base.Model.UsedChargeAttackThisTurn && currentOtherWeaponPrefab != null)
		{
			return currentOtherWeaponPrefab;
		}
		if (!HelpersGfx.GetEquipmentResourceEntry(CurrentWeapon).useOtherHandOnCharged && currentOtherWeaponPrefab != null)
		{
			return currentOtherWeaponPrefab;
		}
		return currentWeaponPrefab;
	}

	public void SetMirrored(bool mirrored)
	{
		Vector3 localScale = (mirrored ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f));
		if (mirroredRoots != null)
		{
			for (int i = 0; i < mirroredRoots.Count; i++)
			{
				mirroredRoots[i].transform.localScale = localScale;
			}
		}
	}

	public void EnableModelChangeListener(bool enabled)
	{
		base.Model.Changed -= OnActorModelChanged;
		if (enabled)
		{
			base.Model.Changed += OnActorModelChanged;
		}
	}

	private void UpdateCurrentWeaponPrefab()
	{
		Transform transform = UnityUtils.FindChild(base.transform, "Bind_RightGunParent");
		if (transform != null && transform.childCount > 0)
		{
			currentWeaponPrefab = transform.GetChild(0).gameObject;
		}
		Transform transform2 = UnityUtils.FindChild(base.transform, "Bind_LeftGunParent");
		if (transform2 != null && transform2.childCount > 0)
		{
			currentOtherWeaponPrefab = transform2.GetChild(0).gameObject;
		}
	}

	public void RequestWeaponSwitch(bool meleeWeapon)
	{
		if (!IsWeaponCarrying || meleeWeapon == IsMeleeWeaponEquipped)
		{
			return;
		}
		RequestedWeapon = base.Model.GetEquipmentOfCategory((!meleeWeapon) ? EquipmentCategory.RangeWeapon : EquipmentCategory.MeleeWeapon);
		if (RequestedWeapon != null)
		{
			SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
			if (survivorAnimationController != null)
			{
				survivorAnimationController.SwitchWeapon();
				OnWeaponSwitched();
			}
		}
		else
		{
			Debug.LogError("Cannot find weapon of type " + (meleeWeapon ? "melee" : "ranged"));
		}
	}

	public void RequestSwitchEquipment(EquipmentItemModel equipment)
	{
		if (CurrentWeapon == equipment)
		{
			return;
		}
		RequestedWeapon = equipment;
		if (RequestedWeapon != null)
		{
			SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
			if (survivorAnimationController != null)
			{
				survivorAnimationController.SwitchWeapon();
			}
		}
		else
		{
			Debug.LogError("Cannot find equipment");
		}
	}

	public void SetVisible(bool visible)
	{
		if (!CanUpdateVisibility)
		{
			return;
		}
		bool updateAll = IsVisibleToSurvivors != visible;
		IsVisibleToSurvivors = visible;
		bool flag = false;
		if (renderers != null)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				if (renderers[i] != null)
				{
					renderers[i].enabled = visible;
				}
				else
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			renderers = GetComponentsInChildren<Renderer>();
			SetRenderersEnabled(renderers, visible);
		}
		if (weaponRenderers != null)
		{
			SetRenderersEnabled(weaponRenderers, visible);
		}
		if (otherWeaponRenderers != null)
		{
			SetRenderersEnabled(otherWeaponRenderers, visible);
		}
		Helpers.GameObjectSetActive(bossAuraFX, visible);
		RefreshUI(updateAll);
	}

	private void SetRenderersEnabled(Renderer[] renderers, bool visible)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = visible;
			}
		}
	}

	public void RefreshUI(bool updateAll)
	{
		if (healthIndicator != null)
		{
			bool flag = IsVisibleToSurvivors && showHealthIndicator;
			if (base.Model.IsFlare)
			{
				Helpers.GameObjectSetActive(healthIndicator, flag);
				Helpers.GameObjectSetActive(healthIndicator.HealthBar, value: false);
				Helpers.GameObjectSetActive(healthIndicator.ShieldHPBar, value: false);
				if (updateAll)
				{
					healthIndicator.NameLabel.text = base.Model.Name;
				}
			}
			else
			{
				bool flag2 = base.Model.Faction == Faction.Survivor;
				bool flag3 = base.Model.StrugglesLeft <= 0;
				bool flag4 = base.Model.Faction == Faction.Lure;
				float num = (float)base.Model.Hitpoints / (float)base.Model.MaxHitPoints;
				float num2 = (float)base.Model.ShieldHitPoints / (float)base.Model.MaxShieldHitPoints;
				healthIndicator.gameObject.SetActive(flag && !flag4);
				healthIndicator.HealthBar.gameObject.SetActive(flag && num > 0f && (flag2 || flag3 || num < 1f || num2 < 1f));
				healthIndicator.ShieldHPBar.gameObject.SetActive(flag && num2 > 0f && (flag2 || flag3 || num2 < 1f));
				if (healthIndicator.ChargePointContainer != null && num == 0f)
				{
					Helpers.GameObjectSetActive(healthIndicator.ChargePointContainer.gameObject, value: false);
				}
			}
			if (base.Model.IsHuman && updateAll)
			{
				bool flag5 = false;
				if (base.Model.manager != null && base.Model.manager.Player != null && base.Model.manager.Player.Combat != null && base.Model.manager.Player.Combat.IsPVPMission && base.Model.manager.Player.SurvivorContainer != null && base.Model.manager.Player.SurvivorContainer.Survivors != null)
				{
					flag5 = !GameManager.Instance.playerModel.SurvivorContainer.Survivors.Contains(base.Model as SurvivorModel);
				}
				healthIndicator.NameLabel.text = (flag5 ? GameManager.Instance.GetFilteredText(base.Model.Name) : base.Model.Name);
			}
			if (base.Model.Faction == Faction.Walker)
			{
				if (healthIndicator.ToughWalkerIcon != null)
				{
					healthIndicator.ToughWalkerIcon.gameObject.SetActive(base.Model.IsBoss && flag);
				}
				Helpers.GameObjectSetActive(healthIndicator.BossWalkerIcon, base.Model.IsBossWalker);
			}
		}
		SetShadowVisibility(IsVisibleToSurvivors);
		UpdateCharacterSelectionIndicator();
		BurningMan component = base.gameObject.GetComponent<BurningMan>();
		if (component != null)
		{
			component.SetVisibility(IsVisibleToSurvivors);
		}
		PoisonMan component2 = base.gameObject.GetComponent<PoisonMan>();
		if (component2 != null)
		{
			component2.SetVisibility(IsVisibleToSurvivors);
		}
		DebuffDamageMan debuffDamageMan = base.gameObject.GetComponent<DebuffDamageMan>();
		if (debuffDamageMan == null)
		{
			debuffDamageMan = base.gameObject.AddComponent<DebuffDamageMan>();
			debuffDamageMan.enabled = false;
		}
		debuffDamageMan.SetVisibility(IsVisibleToSurvivors);
		ElectricMan electricMan = base.gameObject.GetComponent<ElectricMan>();
		if (electricMan == null)
		{
			electricMan = base.gameObject.AddComponent<ElectricMan>();
			electricMan.enabled = false;
		}
		electricMan.SetVisibility(IsVisibleToSurvivors);
		CitadelLeaderMan citadelLeaderMan = base.gameObject.GetComponent<CitadelLeaderMan>();
		if (citadelLeaderMan == null)
		{
			citadelLeaderMan = base.gameObject.AddComponent<CitadelLeaderMan>();
			citadelLeaderMan.enabled = false;
		}
		citadelLeaderMan.SetVisibility(IsVisibleToSurvivors);
		SurvivalGameMan survivalGameMan = base.gameObject.GetComponent<SurvivalGameMan>();
		if (survivalGameMan == null)
		{
			survivalGameMan = base.gameObject.AddComponent<SurvivalGameMan>();
			survivalGameMan.enabled = false;
		}
		survivalGameMan.SetVisibility(IsVisibleToSurvivors);
		QuantunMan quantunMan = base.gameObject.GetComponent<QuantunMan>();
		if (quantunMan == null)
		{
			quantunMan = base.gameObject.AddComponent<QuantunMan>();
			quantunMan.enabled = false;
		}
		quantunMan.SetVisibility(IsVisibleToSurvivors);
		ElectricSurgedMan electricSurgedMan = base.gameObject.GetComponent<ElectricSurgedMan>();
		if (electricSurgedMan == null)
		{
			electricSurgedMan = base.gameObject.AddComponent<ElectricSurgedMan>();
			electricSurgedMan.enabled = false;
		}
		electricSurgedMan.SetVisibility(IsVisibleToSurvivors);
		if (staggerIndicator != null && base.Model.IsStaggered)
		{
			staggerIndicator.SetActive(IsVisibleToSurvivors);
		}
		if (remoteIndicator != null && base.Model.IsRemoteWeakened)
		{
			remoteIndicator.SetActive(IsVisibleToSurvivors);
			healthIndicator.UpdateRemote(isActive: true);
		}
		if (ABTestA2Indicator != null && ABTestA2Indicator.activeInHierarchy != IsVisibleToSurvivors && base.Model.IsABTesterA2ed)
		{
			ABTestA2Indicator.SetActive(IsVisibleToSurvivors);
		}
	}

	public void SetShadowVisibility(bool visible)
	{
		if (shadowBlob != null)
		{
			shadowBlob.enabled = visible;
		}
	}

	private void DisableOnDeathObjects()
	{
		foreach (GameObject item in disableOnDeath)
		{
			Helpers.GameObjectSetActive(item, value: false);
		}
	}

	private void FadeAndDestroyShadow()
	{
		if (shadowBlob != null)
		{
			shadowBlob.FadeAndDestroyShadow();
			shadowBlob = null;
		}
	}

	public void ShowHealthIndicator(bool visible)
	{
		showHealthIndicator = visible;
		RefreshUI(updateAll: true);
	}

	public void SetCoverIconState(CoverIconState coverState)
	{
		if (HealthIndicator != null)
		{
			HealthIndicator.SetCoverIconEnabled(coverState);
		}
	}

	public void SetHealthIndicatorValue(float value)
	{
		healthValue = value;
	}

	public void SetSpeechBubble(bool enabled)
	{
		if (speechBubble != null)
		{
			speechBubble.SetActive(enabled);
		}
	}

	public void SetOverwatchIndicator(bool enabled)
	{
		if (!(characterSelectionIndicator != null))
		{
			return;
		}
		ActionPointIndicator actionPointIndicator = characterSelectionActionPointIndicator;
		if (actionPointIndicator != null && actionPointIndicator.OverwatchIndicator != null)
		{
			SelectionMesh component = actionPointIndicator.OverwatchIndicator.GetComponent<SelectionMesh>();
			component.IsInactive = !enabled;
			component.IsSelected = false;
			if (actionPointIndicator.ActionPoint1 != null)
			{
				actionPointIndicator.ActionPoint1.SetActive(!enabled);
			}
			if (actionPointIndicator.ActionPoint2 != null)
			{
				actionPointIndicator.ActionPoint2.SetActive(!enabled);
			}
		}
	}

	public void SetAIOverwatchIndicator(bool enabled)
	{
		aiOverwatchIndicator = enabled;
	}

	public void GoToRagdoll()
	{
		CharacterAnimationController.EnableRagdoll();
	}

	public void BackFromRagdoll()
	{
		CharacterAnimationController.DisableRagdoll(enableAnimator: true, disableCollisions: true);
	}

	public void ApplyImpactProfile(ImpactProfile impactProfile, Vector3 impactDirection, Vector3 attackDirection)
	{
		CharacterAnimationController.ApplyImpactProfile(impactProfile, impactDirection, attackDirection);
	}

	public void AddNotification(ActorNotificationMessage message, bool dueLuck = false, ActorModel actorModel = null, Action onStarted = null, TimedEffectType timedEffectType = TimedEffectType.None, bool stackMultiple = false, bool wipeAllPreviousOfSameType = false)
	{
		if (IsVisibleToSurvivors && (timedEffectType == TimedEffectType.None || !notificationManager.GetIsNotificationAlreadyInQueueForEffect(timedEffectType)))
		{
			if (stackMultiple)
			{
				notificationManager.StackNotificationMessage(message);
			}
			else if (wipeAllPreviousOfSameType)
			{
				notificationManager.ClearSameNotificationTypes(message.MessageType);
			}
			notificationManager.AddNotification(message, dueLuck, actorModel, onStarted, timedEffectType);
		}
	}

	public void RemoveNotificationsForTrait(string traitIdentifier)
	{
		if (!(traitIdentifier == ""))
		{
			notificationManager.RemoveNotificationsForTrait(traitIdentifier);
		}
	}

	public void Stun(int counter, int maxDuration)
	{
		if (!base.Model.IsElectricShocked)
		{
			if (base.Model.IsWalker)
			{
				(CharacterAnimationController as WalkerAnimationController).SetStunned(astunned: true);
			}
			if (stunIndicator != null)
			{
				stunIndicator.SetActive(value: true);
			}
		}
	}

	public void SetStagger(bool enabled)
	{
		if (staggerIndicator != null)
		{
			staggerIndicator.SetActive(enabled);
		}
		if (healthIndicator != null)
		{
			healthIndicator.DisableMultipleTurnIndicator();
		}
		if (base.Model.IsWalker && !base.Model.IsDead)
		{
			WalkerAnimationController walkerAnimationController = CharacterAnimationController as WalkerAnimationController;
			if (walkerAnimationController != null)
			{
				walkerAnimationController.SetStaggered(enabled);
			}
		}
	}

	public void SetRemote(bool enabled)
	{
		if (remoteIndicator != null)
		{
			remoteIndicator.SetActive(enabled);
		}
	}

	public void Herd(int counter, int maxDuration)
	{
		if (herdLineIndicator == null)
		{
			CreateHerdLineIndicator();
		}
		if (base.Model != null && base.Model.ExclusiveTimedEffect != null && base.Model.ExclusiveTimedEffect.Instigator != null)
		{
			ActorView actorView = GameManager.Instance.GetViewForModel(base.Model.ExclusiveTimedEffect.Instigator) as ActorView;
			if (actorView != null)
			{
				herdVisualizationLine?.SetActorViewDependencies(this, actorView);
			}
		}
	}

	public void Die()
	{
		SetBurningState(enabled: false);
		SetPoisonState(enabled: false);
		SetDebuffDamageState(enabled: false);
		SetElectricState(enabled: false);
		SetSurvivalGameManEffect(enabled: false);
		SetCitadelState_LeaderBuffCitadel(enabled: false);
		SetQuantunState(enabled: false);
		DisableOnDeathObjects();
		FadeAndDestroyShadow();
		DestroySelectionIndicator();
		DestroyHealthIndicator();
		DestroyStunIndicator();
		DestroyStaggerIndicator();
		DestroyRemoteIndicator();
		DestroyChargeSelectionIndicator();
		DestroyHerdLineIndicator();
		DestroyBossAuraEffect();
		DestroyABTestA2Indicator();
	}

	public void EndStruggle()
	{
		if (CharacterAnimationController.IsStruggling)
		{
			if (!base.Model.IsDead)
			{
				CharacterAnimationController.LeaveStruggle();
			}
			SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("stop_group/struggle", base.gameObject);
		}
	}

	public void EndBleedingOut()
	{
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		if (survivorAnimationController != null && !base.Model.IsDead)
		{
			survivorAnimationController.IsBleedingOutRequested = false;
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("stop_group/bleeding_out", base.gameObject);
		}
	}

	private void OnAbilitySelected(AbilityModel ability, ActorModel sourceActor)
	{
		if (base.Model != sourceActor)
		{
			return;
		}
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		if (ability == null || !(survivorAnimationController != null))
		{
			return;
		}
		if (sourceActor.SelectedEquipment.IsChargeEquipment && sourceActor.SelectedEquipment.Ability == ability)
		{
			if (survivorAnimationController.CurrentWeaponPose != WeaponPose.Raised && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingRaised)
			{
				VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(base.Model, WeaponPose.Raised));
			}
			string text = sourceActor.SelectedEquipment.Definition?.AnimatorOverride;
			if (!string.IsNullOrEmpty(text) && survivorAnimationController.ControllerId != text)
			{
				survivorAnimationController.SetController(text);
			}
		}
		else
		{
			if (survivorAnimationController.CurrentWeaponPose != WeaponPose.Lowered && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingLowered)
			{
				VisualizationQueue.Instance.Add(new ChangeWeaponPoseVisualizationTask(base.Model, WeaponPose.Lowered));
			}
			if (survivorAnimationController.ControllerId == sourceActor.GetChargeEquipment()?.Definition?.AnimatorOverride)
			{
				RequestedWeapon = base.Model.SelectedEquipment;
			}
		}
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		CanUpdateVisibility = true;
		if (GameManager.Instance.State == GameState.Combat)
		{
			CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
			if (combatHUD != null)
			{
				combatHUD.OnAbilitySelected -= OnAbilitySelected;
				combatHUD.OnAbilitySelected += OnAbilitySelected;
			}
		}
		base.Model.Changed += OnActorModelChanged;
		CharacterAnimationController = GetComponent<CharacterAnimationController>();
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		spineAttachTarget = UnityUtils.FindChild(base.transform, "Bind_Head");
		notificationManager = new ActorNotificationManager(base.transform);
		if (UseModelForInitialPosition && GridView.Instance != null)
		{
			FixedVec3 position = GridView.Instance.GetPosition(base.Model.GridCoordinate);
			base.transform.position = position.ToVector3();
		}
		if (survivorAnimationController != null)
		{
			if (IsWeaponCarrying)
			{
				survivorAnimationController.WeaponSwitched += OnWeaponSwitched;
				survivorAnimationController.WeaponRaised += OnWeaponRaised;
			}
			if (!DisableAnimationLoading && base.Model is SurvivorModel survivorModel)
			{
				if (survivorModel.IsHero)
				{
					UnityEngine.Object[] array = Resources.LoadAll("AnimationControllers/" + survivorModel.ActorDefinitionID);
					for (int i = 0; i < array.Length; i++)
					{
						RuntimeAnimatorController runtimeAnimatorController = array[i] as RuntimeAnimatorController;
						if (runtimeAnimatorController != null)
						{
							survivorAnimationController.AddController(runtimeAnimatorController.name, runtimeAnimatorController);
						}
					}
				}
				AddOverrideWeaponAnimator(survivorModel.GetChargeEquipment()?.Definition?.AnimatorOverride);
			}
		}
		if (!(CombatView.Instance != null) || CombatView.Instance.Model.MissionCompleted)
		{
			return;
		}
		IsInvisible = base.Model.IsInvisible;
		if (base.Model.Faction == Faction.Survivor)
		{
			CreateChargeSelectionIndicator();
		}
		else if (base.Model.Faction == Faction.Dormant)
		{
			WalkerModel walkerModel = base.Model as WalkerModel;
			(CharacterAnimationController as WalkerAnimationController).SetDormant(walkerModel.DormantType);
		}
		CreateAbilityRangeIndicator();
		CreateActivationRangeIndicator();
		if (base.Model.Faction == Faction.Survivor)
		{
			if (GridView.Instance != null)
			{
				GridModel model2 = GridView.Instance.Model;
				FixedVec3 position2 = GridView.Instance.GetPosition(base.Model.GridCoordinate);
				Vector3 normalized = (GridView.Instance.GetPosition(new GridCoordinate(model2.Width / 2, model2.Height / 2)) - position2).ToVector3().normalized;
				base.transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
			}
		}
		else
		{
			base.transform.eulerAngles = new Vector3(0f, UnityEngine.Random.Range(0, 360), 0f);
		}
		CreateSelectionIndicator();
		CreateHealthIndicator();
		CreateExplosionEffect();
		if (base.Model.StrugglesLeft <= 0)
		{
			if (base.Model.IsFriendlyHuman)
			{
				UpdateHealthBarColor();
			}
			else
			{
				healthIndicator.HealthBar.foregroundWidget.color = Color.red;
			}
		}
		else if (base.Model.IsFriendlyHuman)
		{
			UpdateHealthBarColor();
		}
		float healthIndicatorValue = (float)base.Model.Hitpoints / (float)base.Model.MaxHitPoints;
		SetHealthIndicatorValue(healthIndicatorValue);
		if (base.Model.IsWalker && base.Model is WalkerModel { WalkerType: WalkerType.WalkerCommonWealth } walkerModel2 && Helpers.GameObjectSetActive(healthIndicator.MultiIconIndicator.gameObject, value: true))
		{
			TraitEntry traitWithTraitIdentifier = walkerModel2.GetTraitWithTraitIdentifier("HealthThresholdedStatusResistance");
			int parameterCount = GameManager.Instance.gameEconomyData.GetTraitDefinition(traitWithTraitIdentifier.TraitIdentifier).GetParameterCount();
			InitializeMultiIconBar(base.Model.Hitpoints, base.Model.MaxHitPoints, parameterCount);
		}
		CreateStunIndicator();
		CreateStaggerIndicator();
		CreateRemoteIndicator();
		CreateABTestA2Indicator();
		renderers = GetComponentsInChildren<Renderer>();
		shadowBlob = GetComponent<ShadowBlobOrient>();
		SetBurningState(IsBurning);
		SetPoisonState(base.Model.IsBePoisoned());
		SetDebuffDamageState(base.Model.IsDebuffDamagePerRound());
		SetCitadelState_LeaderBuffCitadel(base.Model.IsCitadelLeaderBuff);
		SetSurvivalGameManEffect(base.Model.IsSurvivalGameEnemy() || base.Model.IsSurvivalGameLeader());
		SetQuantunState(base.Model.IsQuantuned);
		if (healthIndicator != null)
		{
			healthIndicator.UpdateAsthenia(isActive: true);
			healthIndicator.UpdateGrenade(isActive: true);
		}
		SetReloadingState(base.Model.IsReloading);
		CreateBossEffect();
		isCombatActor = true;
		SetupAnimationState();
	}

	public static ModularCharacter GetPrefabOverrideForActor(ActorModel actorModel)
	{
		if (actorModel.OutfitDefinitionID != null && actorModel.OutfitDefinitionID.Length > 0)
		{
			OutfitResourceEntry outfitResourceEntry = GameManager.Instance.GetOutfitResourceEntry(actorModel.OutfitDefinitionID);
			if (outfitResourceEntry != null)
			{
				return UnityUtils.LoadFromAssetBundle<ModularCharacter>("Characters/Outfits/" + ((actorModel.Gender == ActorGender.Male) ? outfitResourceEntry.MaleAssetName : outfitResourceEntry.FemaleAssetName), "scriptableobjects");
			}
		}
		return null;
	}

	public static ModularCharacter GetPrefabOverrideForActorDefinition(string outfitDefinitionId, ActorGender gender)
	{
		if (outfitDefinitionId != null)
		{
			OutfitResourceEntry outfitResourceEntry = GameManager.Instance.GetOutfitResourceEntry(outfitDefinitionId);
			if (outfitResourceEntry != null)
			{
				return UnityUtils.LoadFromAssetBundle<ModularCharacter>("Characters/Outfits/" + ((gender == ActorGender.Male) ? outfitResourceEntry.MaleAssetName : outfitResourceEntry.FemaleAssetName), "scriptableobjects");
			}
		}
		return null;
	}

	public static ModularCharacter GetPrefabOverrideWithDefinition(ActorModel actorModel, OutfitDefinition outfitDefinitionID)
	{
		if (actorModel != null && outfitDefinitionID != null)
		{
			OutfitResourceEntry outfitResourceEntry = GameManager.Instance.GetOutfitResourceEntry(outfitDefinitionID.ID);
			if (outfitResourceEntry != null)
			{
				return UnityUtils.LoadFromAssetBundle<ModularCharacter>("Characters/Outfits/" + ((actorModel.Gender == ActorGender.Male) ? outfitResourceEntry.MaleAssetName : outfitResourceEntry.FemaleAssetName), "scriptableobjects");
			}
		}
		return null;
	}

	public static ModularCharacter GetPrefabForActor(ActorModel actorModel)
	{
		return GetPrefabForActor(actorModel.Definition.ID, actorModel.CharacterPrefab);
	}

	public static ModularCharacter GetPrefabForActor(string actorDefinitionId, string characterPrefab)
	{
		CharacterResourceEntry resources = GameManager.Instance.GetResources<CharacterResourceEntry>(actorDefinitionId);
		if (resources == null)
		{
			Debug.LogError("Could not find resources for actor prefab list " + actorDefinitionId + "!");
			return null;
		}
		if (resources != null && characterPrefab != null && characterPrefab.Length > 0)
		{
			string text = resources.Characters.Find((string x) => x == characterPrefab);
			if (text != null)
			{
				ModularCharacter modularCharacter = LoadAsset(text);
				if (modularCharacter == null)
				{
					Debug.LogWarning("Character asset '" + text + "' could not be loaded.");
				}
				return modularCharacter;
			}
			Debug.LogWarning("Character asset not found for" + characterPrefab);
		}
		return null;
	}

	public static ModularCharacter LoadAsset(string name)
	{
		CharacterWeight characterWeight = CharacterWeight.Light;
		switch (name[2])
		{
		case 'H':
			characterWeight = CharacterWeight.Heavy;
			break;
		case 'M':
			characterWeight = CharacterWeight.Medium;
			break;
		case 'L':
			characterWeight = CharacterWeight.Light;
			break;
		}
		return UnityUtils.LoadFromAssetBundle<ModularCharacter>("Characters/" + characterWeight.ToString() + "/" + (name[0].Equals('F') ? "Female" : "Male") + "/" + name, "scriptableobjects");
	}

	private static ActorGender GetAssetGender(string name)
	{
		return name[0] switch
		{
			'M' => ActorGender.Male,
			'F' => ActorGender.Female,
			_ => ActorGender.NotSpecified,
		};
	}

	public static ModularCharacter SelectRandomPrefabForActorDefinition(string definitionId, ActorGender gender)
	{
		bool flag = SurvivorModel.IsHeroFormActorDefinition(definitionId);
		CharacterResourceEntry resources = GameManager.Instance.GetResources<CharacterResourceEntry>(definitionId);
		if (resources == null)
		{
			Debug.LogError("Couldn't load resources for " + definitionId);
			return null;
		}
		List<string> list = new List<string>();
		if (flag)
		{
			list.Add(resources.Characters[0]);
		}
		else
		{
			list.AddRange(resources.Characters.Where((string x) => gender == ActorGender.NotSpecified || GetAssetGender(x) == gender));
		}
		if (list.Count == 0)
		{
			Debug.LogError("No character found for actor " + definitionId + " " + gender);
			return null;
		}
		string text = null;
		ModelList<SurvivorModel> survivors = GameManager.Instance.playerModel.SurvivorContainer.Survivors;
		int num = 0;
		bool flag2;
		do
		{
			text = list[UnityEngine.Random.Range(0, list.Count)];
			num++;
			flag2 = false;
			if (list.Count <= 1 || num >= 10)
			{
				continue;
			}
			for (int num2 = 0; num2 < survivors.Count; num2++)
			{
				if (survivors[num2].CharacterPrefab == text)
				{
					list.Remove(text);
					flag2 = true;
				}
			}
		}
		while (flag2);
		return LoadAsset(text);
	}

	public static ModularCharacter SelectRandomPrefabForActor(ActorModel actorModel)
	{
		return SelectRandomPrefabForActorDefinition(actorModel.Definition.ID, actorModel.Gender);
	}

	public static void PrepareActor(ActorModel actorModel, bool isTransient = false, bool isInPreview = false)
	{
		PortraitManager instance = PortraitManager.Instance;
		if (instance == null && GameManager.DevFastTrackLoad == DevFastTrackType.None)
		{
			Debug.LogWarning("PortraitManager not found in scene");
			return;
		}
		bool flag = GameManager.Instance.GetHeroSkinResourceEntry(actorModel.ActorDefinitionID) != null;
		if (!string.IsNullOrEmpty(actorModel.CharacterPrefab) && !isInPreview && actorModel is SurvivorModel survivorModel && survivorModel.IsHero && flag && !GameManager.Instance.playerModel.SurvivorContainer.HeroSkinsOwned.Contains(actorModel.CharacterPrefab))
		{
			HeroSkinInfo heroSkinInfo = null;
			HeroSkinDefinition[] heroSkinDefinitions = GameManager.Instance.gameEconomyData.HeroSkinDefinitions;
			foreach (HeroSkinDefinition heroSkinDefinition in heroSkinDefinitions)
			{
				if (heroSkinDefinition.HeroID == actorModel.ActorDefinitionID && GameManager.Instance.playerModel.SurvivorContainer.HeroSkinsOwned.Contains(heroSkinDefinition.ID))
				{
					heroSkinInfo = GameManager.Instance.GetHeroSkinInfoEntry(heroSkinDefinition.ID);
					break;
				}
			}
			if (heroSkinInfo != null && heroSkinInfo.PrefabId != null)
			{
				Helpers.ExecuteCommand(new AssignCharacterPrefabCommand(actorModel, heroSkinInfo.PrefabId, null));
			}
		}
		if (!string.IsNullOrEmpty(actorModel.CharacterPrefab) && instance.GetPortrait(PortraitRenderSource.fromActorModel(actorModel)) != null)
		{
			return;
		}
		ModularCharacter modularCharacter = GetPrefabForActor(actorModel);
		bool flag2 = modularCharacter != null;
		if (modularCharacter == null)
		{
			modularCharacter = SelectRandomPrefabForActor(actorModel);
		}
		ModularCharacter prefabOverrideForActor = GetPrefabOverrideForActor(actorModel);
		string outfitDefinitionID = "";
		if (prefabOverrideForActor != null)
		{
			outfitDefinitionID = prefabOverrideForActor.name;
		}
		if (modularCharacter != null && (string.IsNullOrEmpty(actorModel.CharacterPrefab) || !flag2) && !isTransient)
		{
			Helpers.ExecuteCommand(new AssignCharacterPrefabCommand(actorModel, modularCharacter.name, outfitDefinitionID));
		}
		else if (modularCharacter != null && (string.IsNullOrEmpty(actorModel.CharacterPrefab) || !flag2) && isTransient)
		{
			if (modularCharacter.name != null)
			{
				actorModel.CharacterPrefab = modularCharacter.name;
			}
		}
		else if (instance.GetPortrait(PortraitRenderSource.fromActorModel(actorModel)) == null)
		{
			instance.CreatePortrait(PortraitRenderSource.fromActorModel(actorModel), modularCharacter, delegate
			{
			});
		}
	}

	public void VisualizationTaskCompleted(VisualizationTask task)
	{
		if ((!isCombatActor || GameManager.Instance.playerModel.Combat == null) && CanShowProductionIndicator && GetComponent<CampActorController>().currentBuildingModel.TypeName == "Tents")
		{
			CreateProductionIndicator();
		}
	}

	public void SetActivationRangeVisualization(bool enabled)
	{
		if (ActivationRangeVisualizer == null || !(healthIndicator != null))
		{
			return;
		}
		if (enabled)
		{
			if (healthIndicator.AlertedIcon != null)
			{
				healthIndicator.AlertedIcon.gameObject.SetActive(value: true);
			}
		}
		else if (healthIndicator != null && healthIndicator.AlertedIcon != null)
		{
			healthIndicator.AlertedIcon.gameObject.SetActive(value: false);
		}
	}

	public void SetFreeAttackWarningVisualization(bool enabled)
	{
		if (!(healthIndicator != null))
		{
			return;
		}
		if (enabled)
		{
			if (healthIndicator.FreeAttackWarning != null)
			{
				healthIndicator.FreeAttackWarning.gameObject.SetActive(value: true);
			}
		}
		else if (healthIndicator != null && healthIndicator.FreeAttackWarning != null)
		{
			healthIndicator.FreeAttackWarning.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (this == null || !base.IsInitialized || !isCombatActor || GameManager.Instance.playerModel.Combat == null)
		{
			return;
		}
		notificationManager.Update(Time.deltaTime);
		UpdateFadeout();
		if (base.Model.IsDead)
		{
			return;
		}
		if (newTurn && GameManager.Instance.playerModel.Combat.TurnManager.ActiveFaction == base.Model.Faction && VisualizationQueue.Instance.IsQueueEmpty)
		{
			OnNewTurn();
		}
		if (IsWeaponCarrying && CurrentWeapon == null && RequestedWeapon == null)
		{
			EquipmentItemModel equipmentItemModel = (base.Model.SelectedEquipment.IsConsumable ? base.Model.GetConsumableEquipment() : base.Model.GetWeaponEquipment());
			if (equipmentItemModel != null)
			{
				RequestedWeapon = equipmentItemModel;
				OnWeaponSwitched();
			}
			else
			{
				Debug.LogError("Actor " + base.name + " does not have weapon equipment!");
			}
		}
		if (base.Model.IsWalker && !base.Model.Definition.IsEnvironmental)
		{
			WalkerModel walkerModel = base.Model as WalkerModel;
			WalkerAnimationController walkerAnimationController = CharacterAnimationController as WalkerAnimationController;
			if (!walkerModel.IsStunned && !walkerModel.IsEatingLure)
			{
				switch (walkerModel.AIController.AIDataModel.Alertness)
				{
				case AIAlertness.Idle:
				case AIAlertness.Wandering:
					walkerAnimationController.Alertness = Alertness.Idle;
					break;
				case AIAlertness.Alerted:
				case AIAlertness.Homing:
					walkerAnimationController.Alertness = Alertness.Alert;
					break;
				case AIAlertness.Aggressive:
					walkerAnimationController.Alertness = Alertness.Aggressive;
					break;
				}
			}
			else
			{
				if (walkerModel.IsStaggered && spineAttachTarget != null)
				{
					Vector3 position = base.transform.position;
					Vector3 vector = spineAttachTarget.position - position;
					staggerIndicator.transform.position = position + new Vector3(vector.x, 0f, vector.z);
				}
				if (walkerModel.IsRemoteWeakened && spineAttachTarget != null)
				{
					Vector3 position2 = base.transform.position;
					Vector3 vector2 = spineAttachTarget.position - position2;
					remoteIndicator.transform.position = position2 + new Vector3(vector2.x, 0f, vector2.z);
				}
				if (spineAttachTarget != null && stunIndicator != null)
				{
					Vector3 position3 = base.transform.position;
					Vector3 vector3 = spineAttachTarget.position - position3;
					stunIndicator.transform.position = position3 + new Vector3(vector3.x, 0f, vector3.z);
				}
			}
		}
		UpdateHealthBar();
		UpdateCharacterSelectionIndicator();
		UpdateChargeSelectionIndicator();
		UpdateMeleeWeaponImpactDirection();
		UpdateExplosion();
		SetForwardDirection();
	}

	private void SetForwardDirection()
	{
		if (EnableCarolRealTimeUpdateDirection)
		{
			FixedVec3 forward = new FixedVec3(base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
			if (!forward.Equals(base.Model.ForwardDirection))
			{
				Helpers.ExecuteCommand(new SetActorDirectionCommand(base.Model, forward));
			}
		}
	}

	private void ShowTimedEffectIndicator(List<ActorStatusInfoHealthBar> statusInfos)
	{
		if (healthIndicator != null && HealthIndicator.gameObject.activeInHierarchy)
		{
			healthIndicator.SetTimedEffectIndicator(statusInfos);
		}
	}

	private void ShowSecondaryTimedEffectIndicator(ActorStatusInfoHealthBar statusInfo)
	{
		if (healthIndicator != null && HealthIndicator.gameObject.activeInHierarchy)
		{
			healthIndicator.SetSecondaryTimedEffectIndicator(statusInfo);
		}
	}

	private void ClearSecondaryStatusEffect()
	{
		if (healthIndicator != null && HealthIndicator.gameObject.activeInHierarchy)
		{
			healthIndicator.ClearSecondaryStatusEffect();
		}
	}

	private void UpdateTimedEffectIndicator()
	{
		List<ActorStatusInfoHealthBar> list = new List<ActorStatusInfoHealthBar>();
		int num = 0;
		if (base.Model.ScorchTimedEffect != null)
		{
			int num2 = base.Model.ScorchTimedEffect.Duration - base.Model.ScorchTimedEffect.Counter;
			if (num2 > 0 && base.Model.ScorchTimedEffect.Type == TimedEffectType.Scorch && healthIndicator != null)
			{
				healthIndicator.SetScorchTimedEffectIndicatorVisibility(visible: true, num2);
				float scorchTimedEffectIndicatorLayer = (float)base.Model.ScorchTimedEffect.Layers * 1f / (float)base.Model.ScorchTimedEffect.MaxLayers;
				healthIndicator.SetScorchTimedEffectIndicatorLayer(scorchTimedEffectIndicatorLayer);
			}
		}
		if (!base.Model.IsScorching && healthIndicator != null)
		{
			healthIndicator.SetScorchTimedEffectIndicatorVisibility(visible: false, 0);
		}
		if (base.Model.IsTaunted)
		{
			int count = base.Model.TauntTimedEffect.Duration - base.Model.TauntTimedEffect.Counter;
			if (healthIndicator != null)
			{
				healthIndicator.SetTauntEffectIndicatorVisibility(visible: true, count);
			}
		}
		else if (healthIndicator != null)
		{
			healthIndicator.SetTauntEffectIndicatorVisibility(visible: false, 0);
		}
		if (base.Model.IsSneak)
		{
			if (healthIndicator != null)
			{
				healthIndicator.SetSneakContainer(visible: true);
			}
		}
		else if (healthIndicator != null)
		{
			healthIndicator.SetSneakContainer(visible: false);
		}
		if (base.Model.ExclusiveTimedEffect != null)
		{
			int num3 = base.Model.ExclusiveTimedEffect.Duration - base.Model.ExclusiveTimedEffect.Counter;
			if (num3 > 0)
			{
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Struggle || base.Model.ExclusiveTimedEffect.Type == TimedEffectType.BleedOut)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Struggle, num3, base.Model.ExclusiveTimedEffect.Duration));
					EnableLocationIndicator(IndicatorType.Struggle);
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.InteractingWithObject)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.InteractingWithObject, num3, base.Model.ExclusiveTimedEffect.Duration));
					EnableLocationIndicator(IndicatorType.Loot);
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Stun)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Stun, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.IsFlare && base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Lure)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Lure, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.EatingLure)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.EatingLure, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Root)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Root, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Pitfall)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Root, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Crippled)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Crippled, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Herd)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Herd, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type != TimedEffectType.Root)
				{
					SetOverwatchIndicator(enabled: false);
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Disorient)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Disorient, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.DisorientLock)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.DisorientLock, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.ABTesterA)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.ABTesterA, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
				if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.ElectricShock)
				{
					list.Add(new ActorStatusInfoHealthBar(TimedEffectType.ElectricShock, num3, base.Model.ExclusiveTimedEffect.Duration));
				}
			}
		}
		if (base.Model.IsInvisible || base.Model.IsCamouflaged)
		{
			TraitEntry trait = base.Model.TraitContainer.GetTrait("WalkerMikeActive");
			TraitEntry traitEntry = base.Model.TraitContainer.GetTrait("Gore") ?? trait;
			list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Invisible, (int)traitEntry.TraitDuration, 0));
		}
		if (base.Model.HasAnyLevelTrait("DebuffMarkEnemy"))
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				ShowSecondaryTimedEffectIndicator(new ActorStatusInfoHealthBar(TimedEffectType.Marked));
			}));
		}
		else
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, ClearSecondaryStatusEffect));
		}
		if (base.Model.IsReloading)
		{
			int remainingTurnsToReload = base.Model.SelectedEquipment.RemainingTurnsToReload;
			list.Add(new ActorStatusInfoHealthBar(TimedEffectType.Reloading, remainingTurnsToReload));
		}
		if (list.Count > 0)
		{
			ShowTimedEffectIndicator(list);
		}
		else if (num <= 0)
		{
			DisableLocationIndicator();
			if (healthIndicator != null)
			{
				healthIndicator.DisableMultipleTurnIndicator();
			}
		}
	}

	private void UpdateHealthBar()
	{
		if (healthIndicator != null && healthIndicator.HealthBar.value != healthValue)
		{
			if (healthValue > healthIndicator.HealthBar.value)
			{
				healthIndicator.HealthBar.value = Mathf.Min(healthValue, healthIndicator.HealthBar.value + Time.deltaTime * 0.5f);
			}
			else
			{
				healthIndicator.HealthBar.value = Mathf.Max(healthValue, healthIndicator.HealthBar.value - Time.deltaTime * 0.5f);
			}
			if (base.Model.Faction == Faction.Survivor)
			{
				UpdateHealthBarColor();
			}
			UpdateMultiIconIndicator();
		}
	}

	private void UpdateHealthBarColor()
	{
		InjuryType injuryTypeFromRatio = GetInjuryTypeFromRatio(GameManager.Instance.gameEconomyData, base.Model, healthIndicator.HealthBar.value);
		healthIndicator.HealthBar.foregroundWidget.color = healthBarInjuryTypeColorsConfig.GetColorForInjuryType(injuryTypeFromRatio);
	}

	private void UpdateCharacterSelectionIndicator()
	{
		if (!CanUpdateVisibility || GameManager.Instance.playerModel.Combat == null || !(characterSelectionIndicator != null))
		{
			return;
		}
		if (base.Model.Faction == Faction.Survivor)
		{
			if (base.Model.UserCanControl)
			{
				characterSelectionIndicator.SetActive(value: true);
				characterSelectionIndicator.transform.localScale = new Vector3(1f, 1f, 1f);
				characterSelectionIndicator.transform.rotation = Quaternion.identity;
				bool isSelected = GameManager.Instance.playerModel.Combat.ActiveActor == base.Model && CombatView.Instance.CurrentViewFaction != Faction.Walker;
				EffectRumble effectRumble = characterSelectionRumble;
				if (effectRumble != null)
				{
					effectRumble.enabled = isSelected;
				}
				SelectionMesh selectionMesh = characterSelectionAP1Mesh;
				if (selectionMesh != null)
				{
					selectionMesh.IsSelected = isSelected;
				}
				selectionMesh = characterSelectionAP2Mesh;
				if (selectionMesh != null)
				{
					selectionMesh.IsSelected = isSelected;
				}
			}
			else
			{
				characterSelectionIndicator.SetActive(value: false);
			}
			return;
		}
		AIController aIController = null;
		if (base.Model.Faction == Faction.Walker)
		{
			aIController = base.Model.AIController as WalkerController;
		}
		else if (base.Model.Faction == Faction.Raider)
		{
			aIController = base.Model.AIController as RaiderController;
		}
		bool flag = false;
		if (aIController != null)
		{
			flag = aIController.AIDataModel.Alertness != AIAlertness.Idle;
			if (aiOverwatchIndicator)
			{
				characterSelectionMesh.SetOverwatchStateTexture();
			}
			else
			{
				characterSelectionMesh.SetAlertnessState(aIController.AIDataModel.Alertness);
			}
		}
		characterSelectionIndicator.transform.localScale = new Vector3(1f, 1f, 1f);
		characterSelectionIndicator.SetActive(IsVisibleToSurvivors && (flag || aiOverwatchIndicator));
		EffectRumble effectRumble2 = characterSelectionRumble;
		if (effectRumble2 != null && aIController != null && aIController.AIDataModel != null && aIController.AIDataModel.Alertness >= AIAlertness.Homing)
		{
			effectRumble2.enabled = flag;
		}
	}

	private void UpdateChargeSelectionIndicator()
	{
		if (!(chargeSelectionIndicator != null))
		{
			return;
		}
		if (GameManager.Instance.playerModel.Combat.ActiveActor == base.Model)
		{
			if (base.Model.ChargeMeter.ChargeAvailable && base.Model.SelectedEquipment.IsChargeEquipment)
			{
				chargeSelectionIndicator.SetActive(value: true);
			}
			else
			{
				chargeSelectionIndicator.SetActive(value: false);
			}
		}
		else
		{
			chargeSelectionIndicator.SetActive(value: false);
		}
	}

	private void UpdateExplosion()
	{
		if (activeExplosion != null)
		{
			ParticleSystem component = activeExplosion.GetComponent<ParticleSystem>();
			if (component != null && !component.IsAlive())
			{
				UnityEngine.Object.Destroy(activeExplosion);
				activeExplosion = null;
			}
		}
	}

	private void OnEnable()
	{
		if (base.Model != null && CombatView.Instance != null && !CombatView.Instance.Model.MissionCompleted)
		{
			CreateHealthIndicator();
			CreateSelectionIndicator();
			CreateStunIndicator();
			CreateStaggerIndicator();
			CreateRemoteIndicator();
			CreateAbilityRangeIndicator();
			CreateActivationRangeIndicator();
			CreateChargeSelectionIndicator();
			CreateExplosionEffect();
			SetupAnimationState();
			CreateBossEffect();
			CreateABTestA2Indicator();
		}
		EnableCarolRealTimeUpdateDirection = GameManager.Instance.gameEconomyData.GetFeature("EnableCarolRealTimeUpdateDirection").Enabled;
	}

	private void OnDisable()
	{
		DestroyHealthIndicator();
		DestroySelectionIndicator();
		DestroyStunIndicator();
		DestroyStaggerIndicator();
		DestroyRemoteIndicator();
		DestroyAbilityRangeIndicator();
		DestroyActivationRangeIndicator();
		DestroyChargeSelectionIndicator();
		DestroySpeechBubble();
		DestroyHerdLineIndicator();
		DestroyABTestA2Indicator();
	}

	private void OnDestroy()
	{
		if (VisualizationQueue.Instance != null)
		{
			VisualizationQueue.Instance.StopDependentTasks(base.Model);
		}
		DisableLocationIndicator();
		if (base.Model != null)
		{
			base.Model.Changed -= OnActorModelChanged;
		}
		if (productionIndicator != null)
		{
			UnityEngine.Object.Destroy(productionIndicator.gameObject);
		}
		if (GameManager.Instance.State == GameState.Combat && SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD, null, createIfNotExist: false) as CombatHUD;
			if (combatHUD != null)
			{
				combatHUD.OnAbilitySelected -= OnAbilitySelected;
			}
		}
		if (CombatView.Instance != null)
		{
			CombatView.Instance.RemoveActorView(this);
		}
	}

	private void CheckForStruggleSeriousness()
	{
		if (base.Model.ExclusiveTimedEffect == null || base.Model.ExclusiveTimedEffect.Duration - base.Model.ExclusiveTimedEffect.Counter != 1 || !CharacterAnimationController.IsStruggling)
		{
			return;
		}
		CharacterAnimationController.SetSeriousStruggle(serious: true);
		if (base.Model.ExclusiveTimedEffect.Target != null)
		{
			ActorModel model = base.Model.ExclusiveTimedEffect.Target as ActorModel;
			ActorView actorView = GameManager.Instance.GetViewForModel(model) as ActorView;
			if (actorView != null)
			{
				actorView.CharacterAnimationController.SetSeriousStruggle(serious: true);
			}
		}
	}

	private void OnActorModelChanged(ModelObject m, string changed, object args)
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null)
		{
			return;
		}
		if (changed == "actorWokeUp")
		{
			if (base.Model.Faction == Faction.Dormant)
			{
				(CharacterAnimationController as WalkerAnimationController).StandUp();
			}
			return;
		}
		if (changed == "actorKilledEvent")
		{
			SetActivationRangeVisualization(enabled: false);
			hasDeathVisualizationStarted = true;
			if (CharacterAnimationController != null && !CharacterAnimationController.IsInDeath)
			{
				ActorModel actorModel = args as ActorModel;
				DeathVisualizationTask task = new DeathVisualizationTask(base.Model, actorModel);
				DelayedActionGrenadeThrowVisualizationTask.AddDependenciesForPendingThrowAttacker(task, actorModel);
				VisualizationQueue.Instance.Add(task);
				if (actorModel != null && base.Model.Faction == Faction.Walker && PersonalityManager.Instance != null)
				{
					PersonalityManager.Instance.ReactToWalkerKill(actorModel);
				}
			}
			else if (base.Model.Faction == Faction.Environmental)
			{
				ActorModel attacker = args as ActorModel;
				EnvironmentalActorDestructionVisualizationTask task2 = new EnvironmentalActorDestructionVisualizationTask(base.Model, attacker);
				VisualizationQueue.Instance.Add(task2);
			}
			else if (base.Model.Definition.ShouldDestroyViewOnDeath)
			{
				CombatView.Instance.RemoveActorViewWithDelay(this, 2f);
			}
			RemoveNotificationsForTrait("LeaderBuffExplosiveBullets");
			return;
		}
		if (changed == "actorTimedEffectEnd")
		{
			notificationManager.WipeNotificationList();
			object[] obj = (object[])args;
			TimedEffect timedEffect = (TimedEffect)obj[0];
			bool flag = (bool)obj[1];
			string message = "";
			if (base.Model.IsDead)
			{
				return;
			}
			switch (timedEffect.Type)
			{
			case TimedEffectType.Stun:
				VisualizationQueue.Instance.Add(new EndStunVisualizationTask(base.Model));
				break;
			case TimedEffectType.Struggle:
				VisualizationQueue.Instance.Add(new EndStruggleVisualizationTask(base.Model));
				break;
			case TimedEffectType.EatingLure:
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, EndEatLure));
				break;
			case TimedEffectType.BleedOut:
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, EndBleedingOut));
				break;
			case TimedEffectType.Herd:
				if (herdVisualizationLine != null)
				{
					herdVisualizationLine.Clear();
				}
				break;
			case TimedEffectType.ElectricShock:
				SetElectricState(enabled: false);
				break;
			}
			if (!flag || timedEffect.Type != TimedEffectType.Herd)
			{
				return;
			}
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				if (!base.Model.IsHerded)
				{
					message = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.HerdBroken");
					AddNotification(new ActorNotificationMessage(message, "Ui_Icon_StatusEffect_Herd", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
				}
			}));
			return;
		}
		if (changed == "actorInteractionCompleting")
		{
			InteractiveObjectModel target = args as InteractiveObjectModel;
			VisualizationQueue.Instance.Add(new EndInteractiveObjectVisualizationTask(base.Model, target, completed: true));
			return;
		}
		if (changed == "actorInteractionInterrupting")
		{
			InteractiveObjectModel target2 = args as InteractiveObjectModel;
			VisualizationQueue.Instance.Add(new EndInteractiveObjectVisualizationTask(base.Model, target2, completed: false));
			return;
		}
		if (changed == "actorStruggleSaved")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.StruggleSaved");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_StatusEffect_Struggling", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}));
			return;
		}
		if (changed == "actorStruggleFinished")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/saved");
				}
				if (base.Model.StrugglesLeft <= 0)
				{
					if (healthIndicator != null && healthIndicator.HealthBar != null && healthIndicator.HealthBar.foregroundWidget != null)
					{
						healthIndicator.HealthBar.foregroundWidget.color = Color.red;
						ShowHealthIndicator(visible: true);
						float healthIndicatorValue = (float)base.Model.Hitpoints / (float)base.Model.MaxHitPoints;
						SetHealthIndicatorValue(healthIndicatorValue);
					}
					SetOverwatchIndicator(enabled: false);
				}
				if (healthIndicator != null && healthIndicator.ActionPoint1 != null && healthIndicator.ActionPoint2 != null)
				{
					healthIndicator.ActionPoint1.color = (base.Model.AbilityCompleted ? Color.gray : Color.green);
					healthIndicator.ActionPoint2.color = (base.Model.MoveCompleted ? Color.gray : Color.green);
				}
				if (characterSelectionIndicator != null)
				{
					ActionPointIndicator actionPointIndicator4 = characterSelectionActionPointIndicator;
					if (actionPointIndicator4 != null && actionPointIndicator4.ActionPoint1 != null && actionPointIndicator4.ActionPoint2 != null)
					{
						characterSelectionAP1Mesh.IsInactive = base.Model.AbilityCompleted;
						characterSelectionAP2Mesh.IsInactive = base.Model.MoveCompleted || (base.Model.AbilityCompleted && !base.Model.AllowSecondMoveAfterAbility);
					}
				}
				UpdateSurvivorPortraitStatusEffect();
				CombatView.Instance.CombatHUD.SetActorPortraitTurnCompleted(base.Model, completed: false);
			}));
			return;
		}
		if (changed == "actorBleedingOutSaved")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.BleedingOutSaved");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_StatusEffect_Bleeding", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/saved");
				}
				if (base.Model.StrugglesLeft <= 0)
				{
					if (healthIndicator != null && healthIndicator.HealthBar != null && healthIndicator.HealthBar.foregroundWidget != null)
					{
						if (base.Model.IsFriendlyHuman)
						{
							UpdateHealthBarColor();
						}
						else
						{
							healthIndicator.HealthBar.foregroundWidget.color = Color.red;
						}
					}
					ShowHealthIndicator(visible: true);
					float healthIndicatorValue = (float)base.Model.Hitpoints / (float)base.Model.MaxHitPoints;
					SetHealthIndicatorValue(healthIndicatorValue);
					SetOverwatchIndicator(enabled: false);
				}
				if (healthIndicator != null && healthIndicator.ActionPoint1 != null && healthIndicator.ActionPoint2 != null)
				{
					healthIndicator.ActionPoint1.color = (base.Model.AbilityCompleted ? Color.gray : Color.green);
					healthIndicator.ActionPoint2.color = (base.Model.MoveCompleted ? Color.gray : Color.green);
				}
				if (characterSelectionIndicator != null)
				{
					ActionPointIndicator actionPointIndicator4 = characterSelectionActionPointIndicator;
					if (actionPointIndicator4 != null && actionPointIndicator4.ActionPoint1 != null && actionPointIndicator4.ActionPoint2 != null)
					{
						characterSelectionAP1Mesh.IsInactive = base.Model.AbilityCompleted;
						characterSelectionAP2Mesh.IsInactive = base.Model.MoveCompleted || (base.Model.AbilityCompleted && !base.Model.AllowSecondMoveAfterAbility);
					}
				}
				CombatView.Instance.CombatHUD.SetActorPortraitTurnCompleted(base.Model, completed: false);
			}));
			return;
		}
		if (changed == "actorTimedEffectUpdated")
		{
			notificationManager.WipeNotificationList();
			if (base.Model.ExclusiveTimedEffect.Type == TimedEffectType.Struggle)
			{
				CheckForStruggleSeriousness();
			}
			return;
		}
		if (changed == "actorTimedEffectStart")
		{
			notificationManager.WipeNotificationList();
			TimedEffect obj2 = (TimedEffect)args;
			string text = "";
			if (obj2.Type != TimedEffectType.Herd && herdLineIndicator != null)
			{
				herdVisualizationLine.Clear();
			}
			switch (obj2.Type)
			{
			case TimedEffectType.EatingLure:
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, StartEatLure));
				break;
			case TimedEffectType.Struggle:
				text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorStatusInfo.Struggling");
				AddNotification(new ActorNotificationMessage(text, "Ui_Icon_StatusEffect_Struggling", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
				break;
			}
			return;
		}
		if (changed == "actorMoveCompleted")
		{
			if (healthIndicator != null && healthIndicator.ActionPoint2 != null)
			{
				healthIndicator.ActionPoint2.color = Color.gray;
			}
			if (characterSelectionIndicator != null)
			{
				ActionPointIndicator actionPointIndicator = characterSelectionActionPointIndicator;
				if (actionPointIndicator != null && actionPointIndicator.ActionPoint2 != null)
				{
					characterSelectionAP2Mesh.IsInactive = true;
				}
			}
			if (base.Model.CommandSkillModelManager.CommandSkills.Count > 0)
			{
				CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatHUD) as CombatHUD;
				if (combatHUD != null)
				{
					combatHUD.UpdateTheActiveSKill(base.Model);
				}
			}
			return;
		}
		if (changed == "actorAbilityCompleted" || changed == "actorSecondMoveCompleted" || changed == "ActorExtraMoveAction")
		{
			if (healthIndicator != null && healthIndicator.ActionPoint1 != null && healthIndicator.ActionPoint2 != null)
			{
				healthIndicator.ActionPoint1.color = Color.gray;
				healthIndicator.ActionPoint2.color = Color.gray;
			}
			if (characterSelectionIndicator != null)
			{
				ActionPointIndicator actionPointIndicator2 = characterSelectionActionPointIndicator;
				if (actionPointIndicator2 != null && actionPointIndicator2.ActionPoint1 != null && actionPointIndicator2.ActionPoint2 != null)
				{
					characterSelectionAP1Mesh.IsInactive = true;
					characterSelectionAP2Mesh.IsInactive = (base.Model.MoveCompleted && (base.Model.AdditionalAttackCount <= 0 || !base.Model.CanMoveWithoutAttacking)) || ((base.Model.SecondMoveCompleted || base.Model.AbilityCompleted) && !base.Model.AllowSecondMoveAfterAbility);
				}
			}
			if (changed == "ActorExtraMoveAction")
			{
				object[] array = (object[])args;
				bool dueLuck = (bool)array[1];
				string textId = "Traits." + (string)array[0];
				string icon = "Ui_Icon_Trait_" + (string)array[0];
				AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textId), icon), dueLuck);
			}
			return;
		}
		if (changed == "actorExtraAbilityAction")
		{
			if (healthIndicator != null && healthIndicator.ActionPoint1 != null && healthIndicator.ActionPoint2 != null)
			{
				healthIndicator.ActionPoint1.color = Color.green;
				healthIndicator.ActionPoint2.color = (base.Model.MoveCompleted ? Color.gray : Color.green);
			}
			if (characterSelectionIndicator != null)
			{
				ActionPointIndicator actionPointIndicator3 = characterSelectionActionPointIndicator;
				if (actionPointIndicator3 != null && actionPointIndicator3.ActionPoint1 != null && actionPointIndicator3.ActionPoint2 != null)
				{
					characterSelectionAP1Mesh.IsInactive = false;
					characterSelectionAP2Mesh.IsInactive = base.Model.MoveCompleted || (base.Model.AbilityCompleted && !base.Model.AllowSecondMoveAfterAbility);
				}
			}
			return;
		}
		if (changed == "actorCleaved")
		{
			string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Cleaved");
			VisualizationQueue.Instance.Add(new NotificationVisualizationTask(base.Model, new ActorNotificationMessage(localizedText, "Ui_Icon_Trait_FollowThrough")));
			return;
		}
		if (changed == "actorNewTurn")
		{
			newTurn = true;
			return;
		}
		if (changed == "actorTurnToTarget")
		{
			GridCoordinate coordinate = (GridCoordinate)args;
			FixedVec3 position = GridView.Instance.GetPosition(base.Model.GridCoordinate);
			FixedVec3 position2 = GridView.Instance.GetPosition(coordinate);
			VisualizationQueue.Instance.Add(new TurnToTargetVisualizationTask(base.Model, position.ToVector3(), position2.ToVector3()));
			return;
		}
		if (changed == "actorCreateThreat")
		{
			int num = (int)args;
			if (num > 0)
			{
				float num2 = 0f;
				if (combat.ThreatIncreasePerTurn > 0)
				{
					num2 = (float)num / (float)combat.ThreatIncreasePerTurn;
				}
				string localizedText2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.ThreatCreated", num2);
				VisualizationQueue.Instance.Add(new NotificationVisualizationTask(base.Model, new ActorNotificationMessage(localizedText2)));
			}
			return;
		}
		if (changed == "actorThreatReduction")
		{
			bool dueLuck2 = (bool)((object[])args)[1];
			string localizedText3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.ThreatReductionTraitUsed");
			AddNotification(new ActorNotificationMessage(localizedText3, "Ui_Icon_Trait_ThreatReduction"), dueLuck2);
			return;
		}
		if (changed == "actorCriticalAim")
		{
			bool flag2 = (bool)((object[])args)[0];
			string text2 = "";
			if (flag2)
			{
				text2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.CriticalAim");
				AddNotification(new ActorNotificationMessage(text2, "Ui_Icon_Trait_CriticalAim"), flag2);
			}
			else
			{
				text2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.StunAvoided");
				AddNotification(new ActorNotificationMessage(text2));
			}
			return;
		}
		if (changed == "ActorRedact")
		{
			bool flag3 = (bool)((object[])args)[0];
			string text3 = "";
			if (flag3)
			{
				text3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Redact");
				AddNotification(new ActorNotificationMessage(text3, "Ui_Icon_Trait_Redact"), flag3);
			}
			else
			{
				text3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.StunAvoided");
				AddNotification(new ActorNotificationMessage(text3));
			}
			return;
		}
		if (changed == "actorAIAlertnessStateChanged")
		{
			AIAlertness aIAlertness = (AIAlertness)args;
			string text4 = Enum.GetName(typeof(Faction), base.Model.Faction);
			string text5 = "";
			string text6 = "";
			if (base.Model.Faction != Faction.Walker)
			{
				text6 = "_" + Enum.GetName(typeof(ActorGender), base.Model.Gender);
			}
			switch (aIAlertness)
			{
			case AIAlertness.Alerted:
				text5 = "combat_" + text4 + "/" + text4 + text6 + "_alert";
				break;
			case AIAlertness.Homing:
			case AIAlertness.Aggressive:
				text5 = "combat_" + text4 + "/" + text4 + text6 + "_aggressive";
				break;
			}
			if (SingularityMonoBehaviour<AudioManager>.Instance != null && !string.IsNullOrEmpty(text5))
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(text5.ToLower(), base.gameObject);
			}
			return;
		}
		if (changed == "actorReceivedSP")
		{
			int num3 = (int)args;
			VisualizationQueue.Instance.Add(new NotificationVisualizationTask(base.Model, new ActorNotificationMessage(num3.ToString(), ActorNotificationType.CurrencySP)));
			return;
		}
		if (changed == "ActorBePoisoned")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Poison");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_StatusEffect_Poison", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}));
			return;
		}
		if (changed == "ActorBeRemoteWeakened")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.RemoteWeakened");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_Trait_DebuffRemoteRepulse", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}));
			return;
		}
		if (changed == "UpdateEffectDurationEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateRemote(isActive: true);
					if (base.Model.IsRemoteWeakened)
					{
						SetRemote(enabled: true);
					}
					else
					{
						SetRemote(enabled: false);
					}
				}
			}));
			return;
		}
		if (changed == "ActorPoisonUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdatePoison(isActive: true);
					SetPoisonState(base.Model.IsBePoisoned());
				}
			}));
			return;
		}
		if (changed == "ActorElectricShockedEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					SetElectricState(base.Model.IsElectricShocked);
				}
			}));
			return;
		}
		if (changed == "ActorElectronChargeUpdateEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateElectronChargeState();
				}
			}));
			return;
		}
		if (changed == "ActorElectricSurgedEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				SetElectricSurgedState(enabled: true);
			}));
			return;
		}
		if (changed == "ActorBeAsthenia")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Asthenia");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_Trait_DebuffAsthenia", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}));
			return;
		}
		if (changed == "ActorAstheniaUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateAsthenia(isActive: true);
				}
			}));
			return;
		}
		if (changed == "ActorBeGrenadeFragmentDamaged")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.GrenadeFragmentDamage");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_Trait_DebuffGrenadeFragmentDamage", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}));
			return;
		}
		if (changed == "ActorGrenadeFragmentDamageUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateGrenade(isActive: true);
				}
			}));
			return;
		}
		if (changed == "ActorHeirloomsHershelFetterUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateIgniteBoost();
				}
			}));
			return;
		}
		if (changed == "ActorHeirloomsHershelFetterMessage")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.IgniteBoost");
				AddNotification(new ActorNotificationMessage(localizedText5, ActorNotificationType.IgniteBoost));
			}));
			return;
		}
		if (changed == "ActorPassiveFlameMessage")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string text9 = (string)args;
				AddNotification(new ActorNotificationMessage(text9, ActorNotificationType.DamageFlame));
			}));
			return;
		}
		if (changed == "actorTraitGained")
		{
			TraitDefinition traitDefinition = (TraitDefinition)args;
			if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "Burning".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Burning");
					AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_StatusEffect_Burning", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
					SetBurningState(enabled: true);
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "Bleeding".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Bleeding");
					AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_StatusEffect_Bleeding", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "InspirePerKillIncreaseDamageModifierTrait".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.LeaderBuffInspiration");
					AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_Trait_LeaderBuffInspire"));
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "Gore".ToLower())
			{
				IsInvisible = true;
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					SwitchToZombieMode();
					CombatHUD combatHUD2 = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatHUD) as CombatHUD;
					if (combatHUD2 != null)
					{
						combatHUD2.SetSurvivorTurnHUD(base.Model);
					}
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower().Contains("DebuffMarkEnemy".ToLower()))
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.LeaderBuffMarkEnemy");
					AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_Trait_LeaderBuffMarkEnemy"));
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "StaggerActive".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Stagger");
					AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_StatusEffect_Staggered", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
					SetStagger(enabled: true);
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "ABTesterA2Active".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					SetABTestA2(enabled: true);
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "Skinned".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					if (healthIndicator != null)
					{
						healthIndicator.UpdateSkinned(isActive: true);
					}
				}));
			}
			else if (traitDefinition != null && traitDefinition.Identifier.ToLower() == "RemoteWeakenActiveFlag".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					if (healthIndicator != null)
					{
						healthIndicator.UpdateRemote(isActive: true);
						SetRemote(enabled: true);
					}
				}));
			}
			else
			{
				if (traitDefinition == null || !(traitDefinition.Identifier.ToLower() == "DebuffEquipmentKaboom".ToLower()))
				{
					return;
				}
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					if (healthIndicator != null)
					{
						healthIndicator.UpdateKaboomStateContainerEffected();
					}
				}));
			}
			return;
		}
		if (changed == "actorLostTrait")
		{
			TraitDefinition traitDefinition2 = (TraitDefinition)args;
			if (traitDefinition2 != null && traitDefinition2.Identifier.ToLower() == "Bleeding".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.BleedingOutSaved");
					AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizedText5), "Ui_Icon_StatusEffect_Bleeding", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
					ActorHitEffects component = GetComponent<ActorHitEffects>();
					if (component != null)
					{
						component.SpawnHealEffects(base.Model);
					}
					UpdateSurvivorPortraitStatusEffect();
					if (SingularityMonoBehaviour<AudioManager>.Instance != null)
					{
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/saved");
					}
				}));
			}
			else if (traitDefinition2 != null && traitDefinition2.Identifier.ToLower() == "Burning".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.BurningOutSaved");
					AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizedText5), "Ui_Icon_StatusEffect_Burning", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
					SetBurningState(enabled: false);
					ActorHitEffects component = GetComponent<ActorHitEffects>();
					if (component != null)
					{
						component.SpawnHealEffects(base.Model);
					}
					UpdateSurvivorPortraitStatusEffect();
					if (SingularityMonoBehaviour<AudioManager>.Instance != null)
					{
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/saved");
					}
				}));
			}
			else if (traitDefinition2?.Identifier.ToLower() == "Gore".ToLower())
			{
				IsInvisible = false;
				RequestedWeapon = base.Model.SelectedEquipment;
				OnInternalWeaponSwitched();
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					UpdateSurvivorPortraitStatusEffect();
				}));
			}
			else if (traitDefinition2 != null && traitDefinition2.Identifier.ToLower() == "StaggerActive".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					SetStagger(enabled: false);
					UpdateSurvivorPortraitStatusEffect();
				}));
			}
			else if (traitDefinition2 != null && traitDefinition2.Identifier.ToLower() == "ABTesterA2Active".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					SetABTestA2(enabled: false);
					UpdateSurvivorPortraitStatusEffect();
				}));
			}
			else if (traitDefinition2 != null && traitDefinition2.Identifier.ToLower() == "Skinned".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					if (healthIndicator != null)
					{
						healthIndicator.UpdateSkinned(isActive: false);
					}
					SetRemote(enabled: false);
				}));
			}
			else if (traitDefinition2 != null && traitDefinition2.Identifier.ToLower() == "RemoteWeakenActiveFlag".ToLower())
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					if (healthIndicator != null)
					{
						healthIndicator.UpdateRemote(isActive: false);
					}
				}));
			}
			else
			{
				if (traitDefinition2 == null || !(traitDefinition2.Identifier.ToLower() == "DebuffEquipmentKaboom".ToLower()))
				{
					return;
				}
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					if (healthIndicator != null)
					{
						healthIndicator.UpdateKaboomStateContainerEffected();
					}
				}));
			}
			return;
		}
		if (changed == "actorExploded")
		{
			if (explosionPrefab != null)
			{
				if (base.Model.Definition.IsEnvironmental)
				{
					VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
					{
						activeExplosion = UnityEngine.Object.Instantiate(explosionPrefab);
						activeExplosion.transform.position = base.transform.position;
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_level/barrel_explosion_1", base.gameObject);
					}, !hasDeathVisualizationStarted));
					return;
				}
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					activeExplosion = UnityEngine.Object.Instantiate(explosionPrefab);
					activeExplosion.transform.position = base.transform.position;
					SpawnGorePieces(4);
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_level/barrel_explosion_1", base.gameObject);
				}));
				return;
			}
			string text7 = (string)args;
			if (string.IsNullOrEmpty(text7))
			{
				Debug.LogError("No explosionType available");
				return;
			}
			ExplosionResourceEntry resources = UnityUtils.LoadFromAssetBundle<ExplosionResourcesMap>("Combat/ExplosionResourcesMap", "scriptableobjects").GetResources(text7);
			if (resources != null)
			{
				explosionPrefab = resources.ExplosionAsset.GetPrefab();
			}
			else
			{
				Debug.LogError("Could not find explosion resource " + text7);
			}
			if (!(explosionPrefab != null))
			{
				return;
			}
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				activeExplosion = SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(explosionPrefab);
				if (activeExplosion != null)
				{
					activeExplosion.transform.parent = base.transform.parent;
					activeExplosion.transform.position = base.transform.position;
					activeExplosion.transform.rotation = explosionPrefab.transform.rotation;
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_level/barrel_explosion_1", base.gameObject);
				}
			}));
			return;
		}
		if (changed == "freeAttackFailed")
		{
			string localizedText4 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.FreeAttackFailed");
			VisualizationQueue.Instance.Add(new NotificationVisualizationTask(base.Model, new ActorNotificationMessage(localizedText4)));
			return;
		}
		if (changed == "ActorHealthChanged")
		{
			string commandSkillChangeRedHealth = (string)args;
			float healthRatio = (float)base.Model.Hitpoints / (float)base.Model.MaxHitPoints;
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (base.Model.StrugglesLeft <= 0 && healthIndicator != null && healthIndicator.HealthBar != null && healthIndicator.HealthBar.foregroundWidget != null)
				{
					healthIndicator.HealthBar.foregroundWidget.color = Color.red;
				}
				if (commandSkillChangeRedHealth == "HealRedHealth")
				{
					if (base.Model.OnRedHealthBar)
					{
						healthIndicator.HealthBar.foregroundWidget.color = Color.red;
					}
					else
					{
						healthIndicator.HealthBar.foregroundWidget.color = Color.green;
					}
				}
				SetHealthIndicatorValue(healthRatio);
			}));
			return;
		}
		if (changed == "ActorReloadingStarted")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Reloading");
				AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizedText5), "Ui_Icon_StatusEffect_Reloading", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
				SetReloadingState(isReloading: true);
			}));
			return;
		}
		if (changed == "ActorReloadingFinished")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.ReloadingFinished");
				AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizedText5), "Ui_Icon_StatusEffect_Reloading", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
				SetReloadingState(isReloading: false);
			}));
			return;
		}
		if (changed == "ToggleEquippedEquipments")
		{
			if (CurrentWeapon != base.Model.SelectedEquipment)
			{
				string localizationString = "ActorNotification.Consumable.";
				if (base.Model.SelectedEquipment.IsConsumable)
				{
					string equipmentName = HelpersLocalization.GetEquipmentName(base.Model.SelectedEquipment);
					if (base.Model.SelectedAbility.Definition.TriggerType == AbilityTriggerType.Instant)
					{
						localizationString += "Used{Consumable}";
					}
					else
					{
						localizationString += "Equipped{Consumable}";
					}
					AddDelayedNotification(null, delegate
					{
						AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizationString, equipmentName)));
					});
				}
				else if (CurrentWeapon.IsConsumable)
				{
					bool flag4 = args != null && (bool)args;
					string equipmentName2 = HelpersLocalization.GetEquipmentName(CurrentWeapon);
					localizationString += (flag4 ? "Used{Consumable}" : "Unequipped{Consumable}");
					AddDelayedNotification(null, delegate
					{
						AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(localizationString, equipmentName2)));
					});
					if (!flag4)
					{
						SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
						if (survivorAnimationController != null && survivorAnimationController.CurrentWeaponPose != WeaponPose.Lowered && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingLowered)
						{
							survivorAnimationController.DesiredWeaponPose = WeaponPose.Lowered;
						}
					}
				}
			}
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(null, delegate
			{
				ForceWeaponSwitch(base.Model.SelectedEquipment);
			}));
			return;
		}
		if (changed == "UnEquipConsumable")
		{
			CombatView.Instance.CombatHUD.ConsumableUnselected();
			return;
		}
		if (changed == "AbilityVisited")
		{
			object[] array2 = (object[])args;
			bool dueLuck3 = (bool)array2[1];
			TraitDefinition activeWeaponTraitByIdentifier = base.Model.GetActiveWeaponTraitByIdentifier((string)array2[0]);
			string textId2 = "Traits." + (string)array2[0];
			string text8 = "";
			string sourceTraitIdentifier = (string)array2[0];
			bool wipeAllPreviousOfSameType = false;
			if (array2.Length >= 3 && array2[2] is bool flag5)
			{
				wipeAllPreviousOfSameType = flag5;
			}
			AddNotification(new ActorNotificationMessage(icon: (activeWeaponTraitByIdentifier == null) ? ("Ui_Icon_Trait_" + (string)array2[0]) : HelpersGfx.GetEquipmentTraitIconNameUsingTraitDefinition(activeWeaponTraitByIdentifier), text: SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textId2), sound: NotificationSound.None, type: ActorNotificationType.ActionNotification, timedEffectType: TimedEffectType.None, sourceTraitIdentifier: sourceTraitIdentifier), dueLuck3, null, null, TimedEffectType.None, stackMultiple: false, wipeAllPreviousOfSameType);
			return;
		}
		if (changed == "WeaponAbilityVisited")
		{
			if (args is string equipmentId)
			{
				AddNotification(new ActorNotificationMessage(HelpersLocalization.GetEquipmentName(equipmentId)));
			}
			return;
		}
		if (changed == "CriticalHitAvoided")
		{
			AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.CriticalHitAvoided")));
			return;
		}
		if (changed == "KnockKnockMarkUpdateEvent")
		{
			UpdateIndicatorKnockKnockMark();
			return;
		}
		if (changed == "PhonePortraitUpdateEvent")
		{
			bool isActive = args != null && (bool)args;
			UpdateIndicatorPhonePortrait(isActive);
			return;
		}
		if (changed == "ABtestBUpdateEvent")
		{
			bool isActive2 = args != null && (bool)args;
			UpdateIndicatorUpdateABtestB(isActive2);
			return;
		}
		if (changed == "TurnCountChangedEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.OnTurnCountChangedEvent();
				}
			}));
			return;
		}
		if (changed == "EquipmentActiveChargeLoadEvent")
		{
			healthIndicator.UpdateEquipmentChargeLoaded();
			return;
		}
		if (changed == "ShieldChanged")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.ShieldChanged();
				}
			}));
			return;
		}
		if (changed == "HelpHandDamageChanged")
		{
			int damage = ((args != null) ? ((int)args) : 0);
			if (Mathf.Abs(damage) > 0)
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					AddNotification(new ActorNotificationMessage(Mathf.Abs(damage).ToString(), ActorNotificationType.Damage));
				}));
			}
			return;
		}
		if (changed == "CommnDamageChanged")
		{
			int damage2 = ((args != null) ? ((int)args) : 0);
			if (Mathf.Abs(damage2) > 0)
			{
				VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
				{
					AddNotification(new ActorNotificationMessage(Mathf.Abs(damage2).ToString(), ActorNotificationType.Damage));
				}));
			}
			return;
		}
		if (changed == "RefreshFistSpikeTurns")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateFistSpike();
				}
			}));
			return;
		}
		if (changed == "PerlieFlameTrigger")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateFlameTrigger();
				}
			}));
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.BurningAgain");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_StatusEffect_Burning", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}));
			return;
		}
		if (changed == "RefreshDodgeShot")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateDodgeShot();
				}
			}));
			return;
		}
		if (changed == "FlameTrigger")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Battle.DynamicTips.FlameTrigger")));
			}));
			return;
		}
		if (changed == "DelayedActionGrenadeThrow")
		{
			GridCoordinate targetCell = (GridCoordinate)args;
			{
				foreach (VisualizationTask item in new DelayedActionGrenadeThrowVisualizationTask(base.Model, targetCell).TasksToQueue())
				{
					VisualizationQueue.Instance.Add(item);
				}
				return;
			}
		}
		if (changed == "HealTargetRemoveRemoteWeakened")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateRemote(isActive: false);
				}
				SetRemote(enabled: false);
			}));
		}
		else if (changed == "RefreshCommandSkill")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				CombatHUD combatHUD2 = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatHUD) as CombatHUD;
				if (combatHUD2 != null)
				{
					combatHUD2.SetSurvivorTurnHUD(base.Model);
				}
			}));
		}
		else if (changed == "ActorQuantunUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				SetQuantunState(base.Model.IsQuantuned);
				healthIndicator?.UpdateQuantun();
			}));
		}
		else if (changed == "ActorMomentumUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				healthIndicator?.UpdateMomentum();
			}));
		}
		else if (changed == "UpParryRiposteFloor")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				healthIndicator?.UpdateRiposte();
			}));
		}
		else if (changed == "SurvivalDashFlagUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateSurvivalDashFlag();
				}
			}));
		}
		else if (changed == "RaiderDashFlagUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateRaiderDashFlag();
				}
			}));
		}
		else if (changed == "UpdateSurvivalGameEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateSurvivalGame();
					SetSurvivalGameManEffect(base.Model.IsSurvivalGameEnemy() || base.Model.IsSurvivalGameLeader());
				}
			}));
		}
		else if (changed == "ActorUnluckyUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateUnlucky();
				}
			}));
		}
		else if (changed == "SupportTalent_Lowerlucky")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateUnlucky2();
				}
			}));
		}
		else if (changed == "UpdateDeadlyFocus")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateDeadlyFocus();
				}
			}));
		}
		else if (changed == "TornApartUpdateEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateTornApartFlag();
				}
			}));
		}
		else if (changed == "bloodFrenzyFlagUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateBloodFrenzyFlag();
				}
			}));
		}
		else if (changed == "ActorAttackChainUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateAttackChainFlag();
				}
			}));
		}
		else if (changed == "ActorShieldBreakerUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateShieldBreaker();
				}
			}));
		}
		else if (changed == "Blind")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateBlind();
				}
			}));
		}
		else if (changed == "SurvivalManualStorySkill_D")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateSurvivalManualStorySkill_D();
				}
			}));
		}
		else if (changed == "SurvivalManualStorySkill_F")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateSurvivalManualStorySkill_F();
				}
			}));
		}
		else if (changed == "ActorDebuffReduceRecoveryUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateReduceRecovery();
				}
			}));
		}
		else if (changed == "ActorDebuffDamagePerRoundUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					SetDebuffDamageState(base.Model.IsDebuffDamagePerRound());
				}
			}));
		}
		else if (changed == "UpdateShadowedGuardEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateShadowedGuard();
				}
			}));
		}
		else if (changed == "ActorRageUpdateEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateRage();
				}
			}));
		}
		else if (changed == "ActorVengefulChargeUpdateEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateVengefulCharge();
				}
			}));
		}
		else if (changed == "ActorCitadelLeaderBuffUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				SetCitadelState_LeaderBuffCitadel(base.Model.IsCitadelLeaderBuff && !base.Model.IsDead);
			}));
		}
		else if (changed == "ActorCitadelBeEffectedUpdate")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateCitadelBeEffected();
				}
			}));
		}
		else if (changed == "actorUndyingUpdateEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateUndyingStateContainerEffected();
				}
			}));
		}
		else if (changed == "UpdateGuardianVowEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateGuardianStateContainerEffected();
					healthIndicator.UpdateSovereignStateContainerEffected();
				}
			}));
		}
		else if (changed == "UpdateDeathsDoor")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateDeathsDoor();
				}
			}));
		}
		else if (changed == "DeathsDoorBlockSecondChance")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("HeroBattleTipsScoutMaggieKilled");
				AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_Trait_LeaderBuffDeathsDoor"));
			}));
		}
		else if (changed == "AbilityRangeTridentStateChanged" || changed == "AbilityRangeTridentChargeChanged")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				healthIndicator?.UpdateTrident();
				if (changed == "AbilityRangeTridentStateChanged")
				{
					AbilityRangeTridentSkill abilityRangeTridentSkill = base.Model.CommandSkillModelManager?.GetActorCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
					if (abilityRangeTridentSkill == null && base.Model.CommandSkillModelManager != null)
					{
						abilityRangeTridentSkill = base.Model.CommandSkillModelManager.GetCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
					}
					if (abilityRangeTridentSkill != null && abilityRangeTridentSkill.IsActive)
					{
						string text9 = "AbilityRangeTridentMark";
						string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Traits." + text9);
						AddNotification(new ActorNotificationMessage(localizedText5, "Ui_Icon_Trait_Trident", NotificationSound.None, ActorNotificationType.ActionNotification, TimedEffectType.None, text9));
					}
				}
			}));
		}
		else if (changed == "UpdateBloodMarkEvent")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				if (healthIndicator != null)
				{
					healthIndicator.UpdateBloodMark();
				}
			}));
		}
		else if (changed == "EquipmentPassiveRemoveNegativeVisited")
		{
			VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(base.Model, delegate
			{
				string localizedText5 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Traits.Equipment.Passive.RemoveNegative");
				AddNotification(new ActorNotificationMessage(localizedText5));
				PlayRemoveNegativeEffect();
			}));
		}
	}

	private void AddDelayedNotification(ActorModel actor, Action delayedNotification)
	{
		VisualizationQueue.Instance.Add(new DelayedNotificationVisualizationTask(actor, delayedNotification));
	}

	private void UpdateSurvivorPortraitStatusEffect()
	{
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CombatHUD) as CombatHUD;
		if (combatHUD != null)
		{
			combatHUD.SetSurvivorTurnHUD(base.Model);
		}
	}

	public void SetActiveActor(bool active)
	{
		if (active || !IsFireWeaponTaskInQueue)
		{
			PlayEquipmentSound(active);
		}
	}

	public void PlayEquipmentSound(bool active)
	{
		GameObject gameObject = GetCurrentWeaponPrefab();
		if (gameObject != null)
		{
			EquipmentActiveSound component = gameObject.GetComponent<EquipmentActiveSound>();
			if (component != null)
			{
				component.SetEquipmentActive(active);
			}
		}
	}

	public void SetWeaponActive(bool active)
	{
		Helpers.GameObjectSetActive(currentWeaponPrefab, active);
		if (currentOtherWeaponPrefab != null)
		{
			Helpers.GameObjectSetActive(currentOtherWeaponPrefab, active);
		}
	}

	public void SetWeaponToReloadMode(bool reloadMode)
	{
		GameObject gameObject = GetCurrentWeaponPrefab();
		if (gameObject != null)
		{
			WeaponEffectsSpawner component = gameObject.GetComponent<WeaponEffectsSpawner>();
			if (component != null)
			{
				component.SetReloadingMode(reloadMode);
			}
		}
	}

	public void ForceWeaponSwitch(EquipmentItemModel newEquipment)
	{
		if (newEquipment != null && CurrentWeapon != newEquipment)
		{
			RequestedWeapon = newEquipment;
			OnInternalWeaponSwitched();
		}
	}

	private void OnWeaponSwitched()
	{
		OnInternalWeaponSwitched();
	}

	private void OnWeaponRaised(bool raised)
	{
		if (currentWeaponPrefab != null)
		{
			WeaponEffectsRaised component = currentWeaponPrefab.GetComponent<WeaponEffectsRaised>();
			if (component != null)
			{
				component.ActivateProjectile(raised);
			}
		}
		if (currentOtherWeaponPrefab != null)
		{
			WeaponEffectsRaised component2 = currentOtherWeaponPrefab.GetComponent<WeaponEffectsRaised>();
			if (component2 != null)
			{
				component2.ActivateProjectile(raised);
			}
		}
		if (!raised)
		{
			OnInternalWeaponSwitched();
		}
	}

	private void OnInternalWeaponSwitched(bool skipAnimationManagement = false)
	{
		Transform transform = UnityUtils.FindChild(base.transform, "Bind_RightGunParent");
		if (transform != null && RequestedWeapon != null)
		{
			DestroyWeapon();
			EquipmentItemModel currentWeapon = CurrentWeapon;
			CurrentWeapon = RequestedWeapon;
			RequestedWeapon = null;
			EquipmentResourceEntry equipmentResourceEntry = null;
			if (CurrentWeapon != null)
			{
				equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(CurrentWeapon);
			}
			if ((equipmentResourceEntry == null || (string.IsNullOrEmpty(equipmentResourceEntry.PrefabName) && string.IsNullOrEmpty(equipmentResourceEntry.OtherHandPrefabName))) && currentWeapon != null)
			{
				equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(currentWeapon);
			}
			if (equipmentResourceEntry == null)
			{
				Debug.LogWarning("Could not find equipment resource for actor " + base.name + " CurrentWeapon.Definition.ID = " + CurrentWeapon.Definition.ID);
				return;
			}
			PlayEquipmentSound(active: false);
			if (equipmentResourceEntry.PrefabName != null && (!string.IsNullOrEmpty(equipmentResourceEntry.PrefabName) || string.IsNullOrEmpty(equipmentResourceEntry.OtherHandPrefabName)))
			{
				GameObject gameObject = LoadWeapon(equipmentResourceEntry.PrefabName, transform, ref weaponRenderers);
				if (gameObject != null)
				{
					currentWeaponPrefab = gameObject;
				}
			}
			if (!string.IsNullOrEmpty(equipmentResourceEntry.OtherHandPrefabName))
			{
				Transform transform2 = UnityUtils.FindChild(base.transform, "Bind_LeftGunParent");
				if (transform2 != null)
				{
					GameObject gameObject2 = LoadWeapon(equipmentResourceEntry.OtherHandPrefabName, transform2, ref otherWeaponRenderers);
					if (gameObject2 != null)
					{
						currentOtherWeaponPrefab = gameObject2;
					}
				}
			}
			if (GameManager.Instance.playerModel.Combat != null)
			{
				PlayEquipmentSound(GameManager.Instance.playerModel.Combat.ActiveActor == base.Model);
			}
			CharacterAnimationController.UpdateBlendShapeRenderers();
			if (base.Model.IsReloading)
			{
				SetWeaponToReloadMode(reloadMode: true);
			}
			string text = "Survivor_";
			string animationId = equipmentResourceEntry.AnimationId;
			string animationId2 = equipmentResourceEntry.AnimationId;
			if (base.Model is SurvivorModel { IsHero: not false })
			{
				text = base.Model.ActorDefinitionID + "_";
			}
			string text2 = text + animationId;
			if (!skipAnimationManagement && !string.IsNullOrEmpty(text2))
			{
				SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
				if (survivorAnimationController.ControllerId != text2)
				{
					survivorAnimationController.ForceIdle();
					survivorAnimationController.SetController(text2, animationId2);
					SetupAnimationState();
					survivorAnimationController.ResumeAfterForce();
				}
			}
			EquipmentTypeSoundOverride = equipmentResourceEntry.TypeSoundOverride;
			if (CurrentWeapon.ChargeEquipment != null)
			{
				equipmentResourceEntry = HelpersGfx.GetEquipmentResourceEntry(CurrentWeapon.ChargeEquipment);
				if (equipmentResourceEntry != null)
				{
					ChargedEquipmentTypeSoundOverride = equipmentResourceEntry.TypeSoundOverride;
				}
			}
		}
		else
		{
			_ = RequestedWeapon;
		}
	}

	private void SwitchToZombieMode()
	{
		EquipmentItemModel currentWeapon = CurrentWeapon;
		if (currentWeapon != null && !currentWeapon.IsConsumable)
		{
			DestroyWeapon();
			currentWeaponPrefab = null;
			currentOtherWeaponPrefab = null;
		}
		string controllerId = "Survivor_Walker";
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		if (!(survivorAnimationController.ControllerId != controllerId))
		{
			return;
		}
		if (!survivorAnimationController.animators.Exists((AnimatorSetup c) => c.Id == controllerId))
		{
			RuntimeAnimatorController runtimeAnimatorController = UnityUtils.LoadFromAssetBundle<RuntimeAnimatorController>(controllerId, "prefabresources");
			if (runtimeAnimatorController != null)
			{
				survivorAnimationController.AddController(runtimeAnimatorController.name, runtimeAnimatorController);
			}
			else
			{
				Debug.LogError("Cannot load Survivor_Walker Controller");
			}
		}
		survivorAnimationController.ForceIdle();
		survivorAnimationController.SetController(controllerId);
		survivorAnimationController.ResumeAfterForce();
	}

	private GameObject LoadWeapon(string prefabName, Transform weaponAttachTarget, ref Renderer[] refWeaponRenderers)
	{
		UnityEngine.Object obj = AssetBundleManager.Instance.LoadAsset<UnityEngine.Object>(prefabName, "weapons");
		if (obj != null)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(obj);
			gameObject.transform.parent = weaponAttachTarget;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject.SetLayerRecursively(base.gameObject.layer);
			refWeaponRenderers = gameObject.GetComponentsInChildren<Renderer>();
			SetRenderersEnabled(refWeaponRenderers, IsVisibleToSurvivors);
			return gameObject;
		}
		Debug.LogWarning("Could not find equipment prefab for actor " + base.name + " CurrentWeapon.Definition.ID = " + CurrentWeapon.Definition.ID + " prefab name: " + prefabName);
		return null;
	}

	private void OnDoNothing()
	{
	}

	private void CreateHealthIndicator()
	{
		if (!(healthIndicator == null))
		{
			return;
		}
		healthBarPosition = new GameObject("Health bar position");
		healthBarPosition.transform.parent = base.transform;
		healthBarPosition.transform.localPosition = new Vector3(0f, base.Model.IsAIControlled ? 1.8f : 2.1f, 0f);
		healthIndicator = CombatView.Instance.CombatHUD.CreateHealthIndicator(base.Model.Faction);
		if (healthIndicator != null)
		{
			healthIndicator.SetBindActor(base.Model);
			FactionColorEntry factionColorData = GameManager.Instance.GetFactionColorData(base.Model.Faction);
			if (healthIndicator.ActionPoint1 != null && healthIndicator.ActionPoint2 != null)
			{
				healthIndicator.ActionPoint1.color = (base.Model.AbilityCompleted ? Color.gray : Color.green);
				healthIndicator.ActionPoint2.color = (base.Model.MoveCompleted ? Color.gray : Color.green);
			}
			if (healthIndicator.LevelLabel != null)
			{
				healthIndicator.LevelLabel.text = (base.Model.IsEnvironmental ? "" : base.Model.Level.ToString());
			}
			if (base.Model.IsEnvironmental)
			{
				Vector3 localPosition = healthIndicator.transform.localPosition;
				healthIndicator.ActorClass.transform.localPosition = new Vector3(0f, localPosition.y, localPosition.z);
				healthIndicator.LevelBackgroundSprite.enabled = false;
			}
			if (healthIndicator.ChargePointBgIcons != null)
			{
				int maxLevel = base.Model.ChargeMeter.MaxLevel;
				for (int i = 0; i < healthIndicator.ChargePointBgIcons.Length; i++)
				{
					healthIndicator.ChargePointBgIcons[i].SetActive(i < maxLevel);
				}
				healthIndicator.UpdateChargeMeterIcons(base.Model.ChargeMeter);
				if (healthIndicator.ChargePointContainer != null)
				{
					healthIndicator.ChargePointContainer.SetActive(!base.Model.IsAIControlled);
				}
			}
			healthIndicator.ActorClass.spriteName = HelpersGfx.GetHealthbarClassIconName(base.Model);
			if (factionColorData != null)
			{
				if (healthIndicator.ActorClass != null)
				{
					healthIndicator.ActorClass.color = factionColorData.UIColor;
				}
				if (healthIndicator.NameLabel != null)
				{
					healthIndicator.NameLabel.color = factionColorData.UIColor;
				}
				if (healthIndicator.LevelLabel != null)
				{
					healthIndicator.LevelLabel.color = factionColorData.UIColor;
				}
			}
			healthIndicator.FollowTarget(healthBarPosition);
			ShowHealthIndicator(visible: true);
			CombatModel combat = GameManager.Instance.playerModel.Combat;
			if (combat != null && combat.HasCover(base.Model.GridCoordinate))
			{
				if (combat.IsCoverFlanked(base.Model.GridCoordinate, base.Model))
				{
					SetCoverIconState(CoverIconState.Flanked);
				}
				else
				{
					SetCoverIconState(CoverIconState.HalfCover);
				}
			}
			else
			{
				SetCoverIconState(CoverIconState.None);
			}
		}
		InvokeRepeating("UpdateTimedEffectIndicator", 0f, 1f);
	}

	private void DestroyHealthIndicator()
	{
		if (healthIndicator != null)
		{
			UnityEngine.Object.Destroy(healthIndicator.gameObject);
			healthIndicator = null;
		}
	}

	private void CreateSelectionIndicator()
	{
		if (base.Model.Faction == Faction.Civilian)
		{
			return;
		}
		string text = "Combat/CharacterSelectionIndicator";
		text = ((!base.Model.IsEnemyNPC) ? "Combat/CharacterSelectionIndicator" : "Combat/WalkerActiveIndicator");
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>(text, "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: " + text);
			return;
		}
		characterSelectionIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		characterSelectionMesh = characterSelectionIndicator.GetComponent<SelectionMesh>();
		characterSelectionRumble = characterSelectionIndicator.GetComponent<EffectRumble>();
		characterSelectionActionPointIndicator = characterSelectionIndicator.GetComponent<ActionPointIndicator>();
		if (characterSelectionActionPointIndicator != null)
		{
			characterSelectionAP1Mesh = characterSelectionActionPointIndicator.ActionPoint1.GetComponent<SelectionMesh>();
			characterSelectionAP2Mesh = characterSelectionActionPointIndicator.ActionPoint2.GetComponent<SelectionMesh>();
		}
		else
		{
			characterSelectionAP1Mesh = null;
			characterSelectionAP2Mesh = null;
		}
		characterSelectionIndicator.SetActive(base.Model.Faction == Faction.Survivor);
		characterSelectionIndicator.transform.parent = base.transform;
		characterSelectionIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
		FactionColorEntry factionColorData = GameManager.Instance.GetFactionColorData(base.Model.Faction);
		if (base.Model.Faction == Faction.Survivor)
		{
			ActionPointIndicator actionPointIndicator = characterSelectionActionPointIndicator;
			if (actionPointIndicator != null && actionPointIndicator.ActionPoint1 != null && actionPointIndicator.ActionPoint2 != null)
			{
				SelectionMesh selectionMesh = characterSelectionAP1Mesh;
				selectionMesh.IsInactive = base.Model.AbilityCompleted || base.Model.SecondMoveCompleted;
				SelectionMesh selectionMesh2 = characterSelectionAP2Mesh;
				selectionMesh2.IsInactive = base.Model.MoveCompleted || (base.Model.AbilityCompleted && !base.Model.AllowSecondMoveAfterAbility);
				if (factionColorData != null)
				{
					selectionMesh.SetNormalColor(factionColorData.ShaderNormalColor);
					selectionMesh.SetSelectedColor(factionColorData.ShaderSelectedColor);
					selectionMesh.SetInactiveColor(factionColorData.ShaderInactiveColor);
					selectionMesh2.SetNormalColor(factionColorData.ShaderNormalColor);
					selectionMesh2.SetSelectedColor(factionColorData.ShaderSelectedColor);
					selectionMesh2.SetInactiveColor(factionColorData.ShaderInactiveColor);
				}
				SetOverwatchIndicator(base.Model.TurnComplete && base.Model.HadActionPointsAtEndOfTurn && !base.Model.IsInvisible);
			}
		}
		else
		{
			SelectionMesh component = characterSelectionIndicator.GetComponent<SelectionMesh>();
			if (factionColorData != null && component != null)
			{
				component.SetNormalColor(factionColorData.ShaderNormalColor);
				component.SetSelectedColor(factionColorData.ShaderSelectedColor);
				component.SetInactiveColor(factionColorData.ShaderInactiveColor);
			}
		}
	}

	private void DestroySelectionIndicator()
	{
		if (characterSelectionIndicator != null)
		{
			UnityEngine.Object.Destroy(characterSelectionIndicator.gameObject);
			characterSelectionIndicator = null;
			characterSelectionActionPointIndicator = null;
			characterSelectionAP1Mesh = null;
			characterSelectionAP2Mesh = null;
			characterSelectionMesh = null;
			characterSelectionRumble = null;
		}
	}

	public void CreateSelectionChangedIndicator()
	{
		if (base.Model.Faction != Faction.Survivor)
		{
			return;
		}
		if (characterSelectionChangeIndicatorPrefab == null)
		{
			string text = "Combat/ActorSelectionChangeIndicator";
			PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>(text, "scriptableobjects");
			if (prefabResource == null)
			{
				Debug.LogError("Could not find resource: " + text);
			}
			else
			{
				characterSelectionChangeIndicatorPrefab = prefabResource.GetPrefab();
			}
		}
		if (characterSelectionChangeIndicatorPrefab != null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(characterSelectionChangeIndicatorPrefab);
			obj.SetActive(value: true);
			obj.transform.parent = base.transform;
			obj.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	private void CreateStunIndicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/StunIndicator", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/StunIndicator");
			return;
		}
		stunIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		stunIndicator.SetActive(value: false);
		stunIndicator.transform.parent = base.transform;
		stunIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void CreateStaggerIndicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/StaggerIndicator", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/StaggerIndicator");
			return;
		}
		staggerIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		staggerIndicator.SetActive(value: false);
		staggerIndicator.transform.parent = base.transform;
		staggerIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void CreateRemoteIndicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/StaggerIndicator_1", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/StaggerIndicator_1");
			return;
		}
		remoteIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		remoteIndicator.SetActive(value: false);
		remoteIndicator.transform.parent = base.transform;
		remoteIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void DestroyStunIndicator()
	{
		if (stunIndicator != null)
		{
			UnityEngine.Object.Destroy(stunIndicator.gameObject);
			stunIndicator = null;
		}
	}

	private void DestroyStaggerIndicator()
	{
		if (staggerIndicator != null)
		{
			UnityEngine.Object.Destroy(staggerIndicator.gameObject);
			staggerIndicator = null;
		}
	}

	private void DestroyRemoteIndicator()
	{
		if (remoteIndicator != null)
		{
			UnityEngine.Object.Destroy(remoteIndicator.gameObject);
			remoteIndicator = null;
		}
	}

	private void CreateChargeSelectionIndicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/ChargeSelectionIndicator", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/ChargeSelectionIndicator");
			return;
		}
		chargeSelectionIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		chargeSelectionIndicator.SetActive(value: false);
		chargeSelectionIndicator.transform.parent = base.transform;
		chargeSelectionIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void DestroyChargeSelectionIndicator()
	{
		if (chargeSelectionIndicator != null)
		{
			UnityEngine.Object.Destroy(chargeSelectionIndicator.gameObject);
			chargeSelectionIndicator = null;
		}
	}

	private void DestroyHerdLineIndicator()
	{
		if (herdLineIndicator != null)
		{
			UnityEngine.Object.Destroy(herdLineIndicator);
			herdVisualizationLine = null;
		}
	}

	private void DestroyBossAuraEffect()
	{
		if (bossAuraFX != null)
		{
			bossAuraFX.GetComponent<CacheableObject>().Destroy();
		}
	}

	private void CreateTurnCountPosition(Transform parent)
	{
		if (turnCountPosition == null)
		{
			turnCountPosition = new GameObject("Turn Count indicator position");
			turnCountPosition.transform.parent = parent;
			turnCountPosition.transform.localPosition = new Vector3(0f, 1.5f, 0f);
		}
	}

	private void DestroyTurnCountPosition()
	{
		if (turnCountPosition != null)
		{
			UnityEngine.Object.Destroy(turnCountPosition.gameObject);
			turnCountPosition = null;
		}
	}

	private void CreateAbilityRangeIndicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/WeaponRangeIndicator", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/WeaponRangeIndicator");
			return;
		}
		abilityRangeIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		abilityRangeIndicator.transform.parent = base.transform;
		abilityRangeIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void DestroyAbilityRangeIndicator()
	{
		if (abilityRangeIndicator != null)
		{
			UnityEngine.Object.Destroy(abilityRangeIndicator.gameObject);
			abilityRangeIndicator = null;
		}
	}

	private void CreateActivationRangeIndicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/ActivationRangeIndicator", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/ActivationRangeIndicator");
			return;
		}
		activationRangeIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		activationRangeIndicator.transform.parent = base.transform;
		activationRangeIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void DestroyActivationRangeIndicator()
	{
		if (activationRangeIndicator != null)
		{
			UnityEngine.Object.Destroy(activationRangeIndicator.gameObject);
			activationRangeIndicator = null;
		}
	}

	private void CreateHerdLineIndicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/HerdLineIndicator", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/HerdLineIndicator");
			return;
		}
		herdLineIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		herdLineIndicator.transform.parent = base.transform;
		herdLineIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
		herdVisualizationLine = herdLineIndicator.GetComponent<HerdVisualizationLine>();
	}

	private void CreateBossEffect()
	{
		if (base.Model is WalkerModel walkerModel && walkerModel.ActorDefinitionID.Contains("_Boss"))
		{
			PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/BossActorEffect", "scriptableobjects");
			bossAuraFX = SingularityMonoBehaviour<ObjectPoolManager>.Instance.FetchObject(prefabResource.GetPrefab());
			bossAuraFX.transform.parent = base.transform;
			bossAuraFX.transform.Reset();
		}
	}

	private void CreateSpeechBubble()
	{
		if (speechBubble == null)
		{
			speechBubble = CombatView.Instance.CombatHUD.CreateSpeechBubble();
			speechBubble.FollowTarget(base.gameObject);
			speechBubble.SetActive(active: false);
		}
	}

	private void DestroySpeechBubble()
	{
		if (speechBubble != null)
		{
			UnityEngine.Object.Destroy(speechBubble.gameObject);
			speechBubble = null;
		}
	}

	private void CreateExplosionEffect()
	{
		List<TraitDefinition> traitsWithTag = base.Model.GetTraitsWithTag("Explosive");
		if (traitsWithTag != null && traitsWithTag.Count > 0)
		{
			ExplosionResourcesMap explosionResourcesMap = UnityUtils.LoadFromAssetBundle<ExplosionResourcesMap>("Combat/ExplosionResourcesMap", "scriptableobjects");
			TraitDefinition traitDefinition = traitsWithTag[0];
			ExplosionResourceEntry resources = explosionResourcesMap.GetResources(traitDefinition.Identifier);
			if (resources != null)
			{
				explosionPrefab = resources.ExplosionAsset.GetPrefab();
			}
			else
			{
				Debug.LogError("Could not find explosion resource from ExplosionResourceMap with name: " + traitDefinition.Identifier);
			}
			CreateGoreSpawnerEffect();
		}
	}

	private void CreateGoreSpawnerEffect()
	{
		if (goreSpawnerPrefab == null)
		{
			string text = "Combat/GoreSpawner";
			PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>(text, "scriptableobjects");
			if (prefabResource == null)
			{
				Debug.LogError("Could not find resource: " + text);
			}
			else if (!string.IsNullOrEmpty(prefabResource.PrefabName))
			{
				goreSpawnerPrefab = prefabResource.GetPrefab();
			}
		}
	}

	private void EnableLocationIndicator(IndicatorType type)
	{
		if (CombatView.Instance != null && CombatView.Instance.CombatHUD != null)
		{
			CombatView.Instance.CombatHUD.ShowLocationIndicator(base.gameObject, type);
		}
	}

	private void DisableLocationIndicator()
	{
		if (CombatView.Instance != null && CombatView.Instance.CombatHUD != null)
		{
			CombatView.Instance.CombatHUD.HideLocationIndicator(base.gameObject);
		}
	}

	public void EndStun()
	{
		if (stunIndicator != null)
		{
			stunIndicator.SetActive(value: false);
		}
		if (healthIndicator != null)
		{
			healthIndicator.DisableMultipleTurnIndicator();
		}
		if (base.Model.IsWalker && !base.Model.IsDead)
		{
			WalkerAnimationController walkerAnimationController = CharacterAnimationController as WalkerAnimationController;
			if (walkerAnimationController != null)
			{
				walkerAnimationController.SetStunned(astunned: false);
			}
		}
	}

	private void StartEatLure()
	{
		if (base.Model != null && base.Model.IsWalker && !base.Model.IsDead)
		{
			if (VisualizationQueue.Instance != null && base.Model.ExclusiveTimedEffect != null && base.Model.ExclusiveTimedEffect.TargetCoordinate.IsValid)
			{
				Vector3 targetPosition = GridView.Instance.GetPosition(base.Model.ExclusiveTimedEffect.TargetCoordinate).ToVector3();
				VisualizationQueue.Instance.Add(new TurnToTargetVisualizationTask(base.Model, base.transform.position, targetPosition, ignoreTimedEffect: true));
				string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.EatingLure");
				AddNotification(new ActorNotificationMessage(localizedText, "Ui_Icon_StatusEffect_Lured", NotificationSound.None, ActorNotificationType.TimedEffectNotification));
			}
			WalkerAnimationController walkerAnimationController = CharacterAnimationController as WalkerAnimationController;
			if (walkerAnimationController != null)
			{
				walkerAnimationController.SetEatingLure(eating: true);
			}
		}
	}

	private void EndEatLure()
	{
		if (base.Model.IsWalker && !base.Model.IsDead)
		{
			(CharacterAnimationController as WalkerAnimationController).SetEatingLure(eating: false);
		}
	}

	private void DestroyWeapon()
	{
		if (currentWeaponPrefab != null)
		{
			UnityEngine.Object.Destroy(currentWeaponPrefab);
		}
		if (currentOtherWeaponPrefab != null)
		{
			UnityEngine.Object.Destroy(currentOtherWeaponPrefab);
		}
		UnityUtils.FindChild(base.transform, "Bind_RightGunParent").DetachChildren();
		UnityUtils.FindChild(base.transform, "Bind_LeftGunParent").DetachChildren();
	}

	private void CreateProductionIndicator()
	{
		if (!(CampView.Instance.ActorHUD != null) || !(productionIndicator == null))
		{
			return;
		}
		productionIndicator = CampView.Instance.ActorHUD.CreateActorProductionIndicator(this);
		if (productionIndicator != null)
		{
			productionIndicator.BuildingProducer = BuildingToCollect;
			if (BuildingToCollect != null)
			{
				BuildingToCollect.Model.Producer.Changed += OnProducerChanged;
			}
			else if (base.Model.Producer != null)
			{
				base.Model.Producer.Changed += OnProducerChanged;
			}
		}
	}

	private void DestroyProductionIndicator()
	{
		if (productionIndicator != null)
		{
			if (BuildingToCollect != null)
			{
				BuildingToCollect.Model.Producer.Changed -= OnProducerChanged;
			}
			else if (base.Model.Producer != null)
			{
				base.Model.Producer.Changed -= OnProducerChanged;
			}
			UnityEngine.Object.Destroy(productionIndicator.gameObject);
			productionIndicator = null;
			BuildingToCollect = null;
		}
	}

	private void OnProducerChanged(ModelObject model, string changed, object args)
	{
		if (changed == "collect")
		{
			DestroyProductionIndicator();
		}
	}

	private void OnNewTurn()
	{
		newTurn = false;
		FlankedNotificationShown = false;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat != null)
		{
			if (!base.Model.TurnComplete)
			{
				if (healthIndicator != null && healthIndicator.ActionPoint1 != null && healthIndicator.ActionPoint2 != null)
				{
					healthIndicator.ActionPoint1.color = Color.green;
					healthIndicator.ActionPoint2.color = Color.green;
				}
				if (characterSelectionIndicator != null)
				{
					ActionPointIndicator actionPointIndicator = characterSelectionActionPointIndicator;
					if (actionPointIndicator != null && actionPointIndicator.ActionPoint1 != null && actionPointIndicator.ActionPoint2 != null)
					{
						characterSelectionAP1Mesh.IsInactive = false;
						characterSelectionAP2Mesh.IsInactive = false;
					}
				}
				SetOverwatchIndicator(enabled: false);
				if (base.Model.Faction == Faction.Survivor && !FlankedNotificationShown && combat.IsCoverFlanked(base.Model.GridCoordinate, base.Model))
				{
					AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Flanked")));
					FlankedNotificationShown = true;
				}
			}
			PlayEquipmentSound(combat.TurnManager.ActiveActor == base.Model);
		}
		healthIndicator?.OnNewTurn();
	}

	private void UpdateFadeout()
	{
		if (!FadeOutRequested)
		{
			return;
		}
		if (CharacterAnimationController.ControlState != ControlState.Ragdoll)
		{
			if (FadeOutTimer != 0f)
			{
				FadeOutTimer -= Time.deltaTime;
				base.transform.position -= new Vector3(0f, 0.75f / FadeOutTime * Time.deltaTime, 0f);
				if (FadeOutTimer <= 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			else
			{
				FadeOutTimer = FadeOutTime;
			}
		}
		else
		{
			RagdollWaitTimer += Time.deltaTime;
			if (RagdollWaitTimer > 2f)
			{
				CharacterAnimationController.DisableRagdoll(enableAnimator: false, disableCollisions: true);
			}
		}
	}

	public void FadeAndDestroy()
	{
		RagdollWaitTimer = 0f;
		FadeOutRequested = true;
	}

	private void SetupAnimationState()
	{
		if (!(CharacterAnimationController != null))
		{
			return;
		}
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (!base.Model.IsWalker && combat != null && combat.HasCover(base.Model.GridCoordinate))
		{
			CharacterAnimationController.SetIdleStance(IdleStance.HalfCover);
		}
		if (IsInvisible)
		{
			SwitchToZombieMode();
		}
		TimedEffect exclusiveTimedEffect = base.Model.ExclusiveTimedEffect;
		if (exclusiveTimedEffect != null)
		{
			switch (exclusiveTimedEffect.Type)
			{
			case TimedEffectType.Struggle:
				if (VisualizationQueue.Instance != null && base.Model.IsHuman)
				{
					VisualizationQueue.Instance.Add(new StruggleVisualizationTask(base.Model.ExclusiveTimedEffect.Instigator, base.Model));
				}
				break;
			case TimedEffectType.BleedOut:
				if (VisualizationQueue.Instance != null && base.Model.IsHuman)
				{
					VisualizationQueue.Instance.Add(new BleedingOutVisualizationTask(base.Model.ExclusiveTimedEffect.Instigator, base.Model));
				}
				break;
			case TimedEffectType.Stun:
			{
				int counter = exclusiveTimedEffect.Duration - base.Model.ExclusiveTimedEffect.Counter;
				Stun(counter, exclusiveTimedEffect.Duration);
				break;
			}
			case TimedEffectType.Lure:
				CharacterAnimationController.Die(struggleDeath: false);
				break;
			case TimedEffectType.InteractingWithObject:
				if (VisualizationQueue.Instance != null)
				{
					InteractiveObjectModel target = base.Model.ExclusiveTimedEffect.Target as InteractiveObjectModel;
					VisualizationQueue.Instance.Add(new StartInteractiveObjectVisualizationTask(base.Model, target));
				}
				break;
			case TimedEffectType.EatingLure:
				StartEatLure();
				break;
			case TimedEffectType.Herd:
			{
				int counter = exclusiveTimedEffect.Duration - base.Model.ExclusiveTimedEffect.Counter;
				Herd(counter, exclusiveTimedEffect.Duration);
				break;
			}
			}
		}
		if (base.Model.IsReloading)
		{
			CharacterAnimationController.SetReloading(isReloading: true);
		}
	}

	private void SetBurningState(bool enabled)
	{
		BurningMan component = GetComponent<BurningMan>();
		if (component != null)
		{
			if (base.Model.IsDead && enabled)
			{
				component.SetDelayedDestroyDelay(5f);
			}
			component.enabled = enabled;
		}
	}

	private void SetPoisonState(bool enabled)
	{
		PoisonMan component = GetComponent<PoisonMan>();
		if (component != null)
		{
			if (base.Model.IsDead && enabled)
			{
				component.SetDelayedDestroyDelay(5f);
			}
			component.enabled = enabled;
		}
	}

	private void SetDebuffDamageState(bool enabled)
	{
		DebuffDamageMan component = GetComponent<DebuffDamageMan>();
		if (component != null)
		{
			if (base.Model.IsDead && enabled)
			{
				component.SetDelayedDestroyDelay(5f);
			}
			component.enabled = enabled;
		}
	}

	public void SetElectricState(bool enabled)
	{
		ElectricMan component = GetComponent<ElectricMan>();
		if (component != null)
		{
			component.enabled = enabled;
		}
	}

	public void SetCitadelState_LeaderBuffCitadel(bool enabled)
	{
		CitadelLeaderMan component = GetComponent<CitadelLeaderMan>();
		if (!(component != null))
		{
			return;
		}
		if (enabled)
		{
			if (component.enabled)
			{
				component.PlayEffect();
			}
			else
			{
				component.enabled = true;
			}
		}
		else
		{
			component.enabled = false;
		}
	}

	public void SetSurvivalGameManEffect(bool enabled)
	{
		SurvivalGameMan component = GetComponent<SurvivalGameMan>();
		if (component != null)
		{
			component.BindData(base.Model);
			component.enabled = enabled;
		}
	}

	public void SetQuantunState(bool enabled)
	{
		QuantunMan component = GetComponent<QuantunMan>();
		if (component != null)
		{
			component.enabled = enabled;
		}
	}

	private void SetElectricSurgedState(bool enabled)
	{
		ElectricSurgedMan component = GetComponent<ElectricSurgedMan>();
		if (component != null)
		{
			if (base.Model.IsDead && enabled)
			{
				component.SetDelayedDestroyDelay(3f);
			}
			component.enabled = enabled;
		}
	}

	private void SetReloadingState(bool isReloading)
	{
		if (base.Model.IsHuman)
		{
			CharacterAnimationController.SetReloading(isReloading);
			SetWeaponToReloadMode(isReloading);
		}
	}

	private void SpawnGorePieces(int count)
	{
		if (!(goreSpawnerPrefab == null))
		{
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.Instantiate(goreSpawnerPrefab).transform.position = base.transform.position;
			}
		}
	}

	public void Say(string localizationKey)
	{
		List<string> list = new List<string>();
		List<ActorModel> factionActors = GameManager.Instance.playerModel.Combat.GetFactionActors(Faction.Survivor);
		List<string> list2 = new List<string> { "Survivor_A", "Survivor_B", "Survivor_C" };
		string text = list2[0];
		for (int i = 0; i < factionActors.Count; i++)
		{
			if (factionActors[i] == base.Model && i < list2.Count)
			{
				text = list2[i];
				break;
			}
		}
		if (text != null)
		{
			string item = "Dialog," + text + "," + localizationKey;
			list.Add(item);
		}
		if (list != null && list.Count > 0)
		{
			DialogVisualizationTask task = new DialogVisualizationTask(list);
			VisualizationQueue.Instance.Add(task);
		}
	}

	public void ResetTargetActorProperties()
	{
		IsTargetHuman = false;
		IsTargetInCover = false;
		IsAttackDodged = false;
		IsTargetEnvironmentalActor = false;
	}

	public static InjuryType GetInjuryTypeFromRatio(GameEconomyData gameEconomyData, ActorModel model, float ratio)
	{
		if (model.StrugglesLeft == 0)
		{
			return InjuryType.Critical;
		}
		int num = (int)((float)model.MaxHitPoints * ratio) + model.MaxHitPoints;
		int num2 = model.MaxHitPoints * 2;
		FixedPoint fixedPoint = new FixedPoint((float)num / (float)num2 * 100f);
		if (fixedPoint < gameEconomyData.ConfigData.InjuryMajorBelowHealthPercentage)
		{
			return InjuryType.Major;
		}
		if (fixedPoint < gameEconomyData.ConfigData.InjuryMinorBelowHealthPercentage)
		{
			return InjuryType.Minor;
		}
		return InjuryType.None;
	}

	private void AddOverrideWeaponAnimator(string overrideAnimationName)
	{
		RuntimeAnimatorController controller;
		if (!string.IsNullOrEmpty(overrideAnimationName) && CharacterAnimationController is SurvivorAnimationController survivorAnimationController && (bool)(controller = Resources.Load<RuntimeAnimatorController>("AnimationControllers/WeaponOverrides/" + overrideAnimationName)))
		{
			survivorAnimationController.AddController(overrideAnimationName, controller);
		}
	}

	private void InitializeMultiIconBar(float currentHealth, float maxHealth, float segments)
	{
		healthIndicator.MultiIconIndicator.SetMaxValue(maxHealth);
		healthIndicator.MultiIconIndicator.SetValuePerSegment(maxHealth / segments);
		healthIndicator.MultiIconIndicator.SetCurrentValue(currentHealth);
		if (base.Model.Faction == Faction.Walker && base.Model is WalkerModel { WalkerType: WalkerType.WalkerCommonWealth } walkerModel)
		{
			healthIndicator.MultiIconIndicator.SetCurrentValue(walkerModel.Hitpoints);
		}
	}

	private void UpdateMultiIconIndicator()
	{
		if (base.Model.Faction == Faction.Walker && base.Model is WalkerModel { WalkerType: WalkerType.WalkerCommonWealth } walkerModel)
		{
			healthIndicator.MultiIconIndicator.SetCurrentValue(walkerModel.Hitpoints);
		}
	}

	private void UpdateIndicatorKnockKnockMark()
	{
		healthIndicator?.UpdateKnockKnockMarkIcons(base.Model);
	}

	public void ClearIndicatorKnockKnockMark()
	{
		healthIndicator?.ClearKnockKnockMarkIcons();
	}

	public void UpdateIndicatorPhonePortrait(bool isActive)
	{
		if (healthIndicator != null)
		{
			healthIndicator.UpdatePhonePortrait(isActive);
		}
	}

	public void UpdateIndicatorUpdateABtestB(bool isActive)
	{
		if (healthIndicator != null)
		{
			healthIndicator.UpdateABtestB(isActive);
		}
	}

	private void CreateABTestA2Indicator()
	{
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Combat/ABTestA2Indicator", "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/ABTestA2Indicator");
			return;
		}
		ABTestA2Indicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
		ABTestA2Indicator.SetActive(value: false);
		ABTestA2Indicator.transform.parent = base.transform;
		ABTestA2Indicator.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void DestroyABTestA2Indicator()
	{
		if (ABTestA2Indicator != null)
		{
			UnityEngine.Object.Destroy(ABTestA2Indicator.gameObject);
			ABTestA2Indicator = null;
		}
	}

	public void SetABTestA2(bool enabled)
	{
		if (ABTestA2Indicator != null && ABTestA2Indicator.activeInHierarchy != enabled)
		{
			ABTestA2Indicator.SetActive(enabled);
		}
		if (base.Model.IsWalker && !base.Model.IsDead)
		{
			WalkerAnimationController walkerAnimationController = CharacterAnimationController as WalkerAnimationController;
			if (walkerAnimationController != null)
			{
				walkerAnimationController.SetABTestA2ed(enabled);
			}
		}
	}

	private void PlayRemoveNegativeEffect()
	{
		PrefabResource prefabResource = (PrefabResource)UnityUtils.LoadAsset("Combat/RemoveNegativeEffect");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/RemoveNegativeEffect");
			return;
		}
		GameObject prefab = prefabResource.GetPrefab();
		if (prefab != null)
		{
			Helpers.InstantiateToParent(prefab, base.gameObject);
		}
	}

	public void PlayBloodMarkSettleEffect()
	{
		PrefabResource prefabResource = (PrefabResource)UnityUtils.LoadAsset("Combat/BloodMarkSettleEffect");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find resource: Combat/BloodMarkSettleEffect");
			return;
		}
		GameObject prefab = prefabResource.GetPrefab();
		if (!(prefab == null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			if (gameObject != null)
			{
				gameObject.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f);
			}
		}
	}

	public void CreateCommandSkillSelectableIndicator()
	{
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (!(combatHUD == null))
		{
			string text = "";
			text = ((!combatHUD.GetCurBaseCommandSkill().Definition.TargetType.Contains(CommandSkillTargetType.Enemy)) ? "Combat/CommandSkillSelectableIndicator" : "Combat/CommandSkillSelectableIndicatorEnemy");
			PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>(text, "scriptableobjects");
			if (prefabResource == null)
			{
				Debug.LogError("Could not find resource: " + text);
				return;
			}
			commandSkillSelectableIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
			commandSkillSelectableIndicator.SetActive(value: true);
			commandSkillSelectableIndicator.transform.parent = base.transform;
			commandSkillSelectableIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	public void DestroyCommandSkillSelectableIndicator()
	{
		if (commandSkillSelectableIndicator != null)
		{
			UnityEngine.Object.Destroy(commandSkillSelectableIndicator.gameObject);
			commandSkillSelectableIndicator = null;
		}
	}

	public void CreateCommandSkillSelectedIndicator()
	{
		CombatHUD combatHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CombatHUD) as CombatHUD;
		if (!(combatHUD == null))
		{
			string text = "";
			text = ((!combatHUD.GetCurBaseCommandSkill().Definition.TargetType.Contains(CommandSkillTargetType.Enemy)) ? "Combat/CommandSkillSelectedIndicator" : "Combat/CommandSkillSelectedIndicatorEnemy");
			PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>(text, "scriptableobjects");
			if (prefabResource == null)
			{
				Debug.LogError("Could not find resource: " + text);
				return;
			}
			commandSkillSelectedIndicator = UnityEngine.Object.Instantiate(prefabResource.GetPrefab());
			commandSkillSelectedIndicator.SetActive(value: true);
			commandSkillSelectedIndicator.transform.parent = base.transform;
			commandSkillSelectedIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	public void DestroyCommandSkillSelectedIndicator()
	{
		if (commandSkillSelectedIndicator != null)
		{
			UnityEngine.Object.Destroy(commandSkillSelectedIndicator.gameObject);
			commandSkillSelectedIndicator = null;
		}
	}
}
