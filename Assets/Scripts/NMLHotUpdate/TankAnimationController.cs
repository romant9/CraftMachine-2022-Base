using System;
using System.Collections;
using Client.Utils;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class TankAnimationController : CharacterAnimationController
{
	private static readonly int UseWeaponHash = Animator.StringToHash("UseWeapon");

	private static readonly int UseWeaponVariationHash = Animator.StringToHash("UseWeaponVariation");

	private static readonly int TakeDamageHash = Animator.StringToHash("TakeDamage");

	private const string IdleStateName = "Guild_Tank_Idle";

	private Transform muzzleFireFxRoot;

	private Transform dustFxRoot;

	private Coroutine stopFireEffectsCoroutine;

	private Coroutine startEngineIdleCoroutine;

	private bool engineIdlePlaying;

	private const string AttackPointExplosionPrefabName = "Explosion Version 3";

	private const float AttackPointExplosionLifeSeconds = 3f;

	private GameObject cachedAttackPointExplosionPrefab;

	public override bool IsIdle
	{
		get
		{
			if (base.Animator == null)
			{
				return false;
			}
			if (base.Animator.GetCurrentAnimatorStateInfo(0).IsName("Guild_Tank_Idle"))
			{
				return !base.Animator.IsInTransition(0);
			}
			return false;
		}
	}

	private void Awake()
	{
		CacheEffectRoots();
		StopFireEffects();
		startEngineIdleCoroutine = StartCoroutine(StartEngineIdleWhenReady());
	}

	private void OnDestroy()
	{
		if (startEngineIdleCoroutine != null)
		{
			StopCoroutine(startEngineIdleCoroutine);
			startEngineIdleCoroutine = null;
		}
		StopEngineIdle();
	}

	private void CacheEffectRoots()
	{
		Transform transform = base.transform.Find("Root");
		if (!(transform == null))
		{
			dustFxRoot = transform.Find("fx_tank_dust");
			Transform transform2 = transform.Find("GuildBoss_Tank_Turret");
			if (transform2 != null)
			{
				muzzleFireFxRoot = transform2.Find("fx_tank_muzzlefire");
			}
		}
	}

	public override void UseWeapon(bool criticalDamage, bool useFenceAttack, bool useChargeAttack)
	{
		base.UseWeapon(criticalDamage, useFenceAttack, useChargeAttack);
		float value = 0f;
		if (base.ActorView != null && base.ActorView.Model != null && base.ActorView.Model.SelectedEquipment?.Definition != null)
		{
			string iD = base.ActorView.Model.SelectedEquipment.Definition.ID;
			if (iD != null && iD.IndexOf("MachineGun", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				value = 1f;
			}
		}
		base.Animator.SetFloat(UseWeaponVariationHash, value);
		base.Animator.SetTrigger(UseWeaponHash);
	}

	public void OnFireAnimationComplete()
	{
	}

	public void PlayFireEffects()
	{
		if (stopFireEffectsCoroutine != null)
		{
			StopCoroutine(stopFireEffectsCoroutine);
			stopFireEffectsCoroutine = null;
		}
		PlaySound("combat_level/tank_shoot");
		PlayParticlesOn(muzzleFireFxRoot);
		PlayParticlesOn(dustFxRoot);
		SpawnAttackPointExplosion();
		float maxEffectDuration = GetMaxEffectDuration();
		stopFireEffectsCoroutine = StartCoroutine(StopFireEffectsAfterDelay(maxEffectDuration));
	}

	private void SpawnAttackPointExplosion()
	{
		GameObject attackPointExplosionPrefab = GetAttackPointExplosionPrefab();
		if (!(attackPointExplosionPrefab == null) && TryGetAttackTargetWorldPosition(out var position))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(attackPointExplosionPrefab, position, Quaternion.identity);
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject, 3f);
			}
		}
	}

	private bool TryGetAttackTargetWorldPosition(out Vector3 position)
	{
		position = Vector3.zero;
		ActorModel actorModel = ((base.ActorView != null) ? base.ActorView.Model : null);
		if (actorModel == null)
		{
			return false;
		}
		ActorModel actorModel2 = null;
		FireWeaponVisualizationTask fireWeaponVisualizationTask = ((VisualizationQueue.Instance != null) ? VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<FireWeaponVisualizationTask>(actorModel) : null);
		if (fireWeaponVisualizationTask != null)
		{
			actorModel2 = fireWeaponVisualizationTask.TargetActor;
		}
		if (actorModel2 == null && actorModel.AIController != null && actorModel.AIController.AIDataModel != null)
		{
			actorModel2 = actorModel.AIController.AIDataModel.GetCurrentTarget();
		}
		if (actorModel2 == null)
		{
			return false;
		}
		ActorView actorView = GameManager.Instance.GetViewForModel(actorModel2) as ActorView;
		if (actorView != null)
		{
			position = actorView.transform.position;
			return true;
		}
		if (GridView.Instance != null)
		{
			position = GridView.Instance.GetPosition(actorModel2.GridCoordinate).ToVector3();
			return true;
		}
		return false;
	}

	private GameObject GetAttackPointExplosionPrefab()
	{
		if (cachedAttackPointExplosionPrefab != null)
		{
			return cachedAttackPointExplosionPrefab;
		}
		cachedAttackPointExplosionPrefab = AssetBundleManager.Instance.LoadAsset<GameObject>("Explosion Version 3", "prefabresources");
		if (cachedAttackPointExplosionPrefab == null)
		{
			Debug.LogError("TankAnimationController: failed to load \"Explosion Version 3\" from bundle prefabresources");
		}
		return cachedAttackPointExplosionPrefab;
	}

	public void StopFireEffects()
	{
		if (stopFireEffectsCoroutine != null)
		{
			StopCoroutine(stopFireEffectsCoroutine);
			stopFireEffectsCoroutine = null;
		}
		StopParticlesOn(muzzleFireFxRoot, clearAndHide: true);
		StopParticlesOn(dustFxRoot, clearAndHide: true);
	}

	private IEnumerator StopFireEffectsAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		StopParticlesOn(muzzleFireFxRoot, clearAndHide: false);
		StopParticlesOn(dustFxRoot, clearAndHide: false);
		stopFireEffectsCoroutine = null;
	}

	private float GetMaxEffectDuration()
	{
		float a = 0f;
		a = Mathf.Max(a, GetMaxParticleDuration(muzzleFireFxRoot));
		a = Mathf.Max(a, GetMaxParticleDuration(dustFxRoot));
		if (!(a > 0f))
		{
			return 5f;
		}
		return a;
	}

	private static float GetMaxParticleDuration(Transform fxRoot)
	{
		if (fxRoot == null)
		{
			return 0f;
		}
		float num = 0f;
		ParticleSystem[] componentsInChildren = fxRoot.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem.MainModule main = componentsInChildren[i].main;
			float num2 = ((main.startLifetime.mode == ParticleSystemCurveMode.Constant) ? main.startLifetime.constant : main.startLifetime.constantMax);
			num = Mathf.Max(num, main.duration + num2);
		}
		return num;
	}

	private static void PlayParticlesOn(Transform fxRoot)
	{
		if (!(fxRoot == null))
		{
			fxRoot.gameObject.SetActive(value: true);
			ParticleSystem[] componentsInChildren = fxRoot.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				componentsInChildren[i].Play(withChildren: true);
			}
		}
	}

	private static void StopParticlesOn(Transform fxRoot, bool clearAndHide)
	{
		if (fxRoot == null)
		{
			return;
		}
		fxRoot.gameObject.SetActive(value: true);
		ParticleSystem[] componentsInChildren = fxRoot.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (clearAndHide)
			{
				componentsInChildren[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			else
			{
				componentsInChildren[i].Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
			}
		}
		if (clearAndHide)
		{
			fxRoot.gameObject.SetActive(value: false);
		}
	}

	public override void MeleeDamage(bool criticalDamage, Vector3 direction)
	{
		if (!(base.Animator == null))
		{
			base.Animator.ResetTrigger(TakeDamageHash);
			base.Animator.SetTrigger(TakeDamageHash);
			PlaySound("combat_level/tank_damaged");
		}
	}

	public override void QuickHit(string direction, EquipmentType equipmentType, Vector3 attackerPosition, DamageAction action, string subCategory = null)
	{
	}

	public void PlaySound(string eventName)
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName, base.gameObject);
		}
	}

	private IEnumerator StartEngineIdleWhenReady()
	{
		WaitForSeconds wait = new WaitForSeconds(0.5f);
		while (SingularityMonoBehaviour<AudioManager>.Instance == null || !SingularityMonoBehaviour<AudioManager>.Instance.combatSfxLoaded)
		{
			yield return wait;
		}
		PlayEngineIdle();
		startEngineIdleCoroutine = null;
	}

	private void PlayEngineIdle()
	{
		if (!engineIdlePlaying && !(SingularityMonoBehaviour<AudioManager>.Instance == null))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_level/tank_engine", base.gameObject);
			engineIdlePlaying = true;
		}
	}

	private void StopEngineIdle()
	{
		if (engineIdlePlaying && !(SingularityMonoBehaviour<AudioManager>.Instance == null))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("combat_level/tank_engine", base.gameObject);
			engineIdlePlaying = false;
		}
	}
}
