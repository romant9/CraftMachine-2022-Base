using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DamageVisualizationTask : ActorVisualizationTask
{
	private bool started;

	private int targetStrugglesLeft;

	private bool targetInCover;

	private bool targetFlanked;

	private bool damagerMoveCompleted;

	private CombatModel combat;

	private bool DeathVisualizationTriggered
	{
		get
		{
			Faction originalTargetFaction = (base.Action as DamageAction).OriginalTargetFaction;
			Faction faction = base.Actor.Faction;
			if (originalTargetFaction != Faction.Survivor || faction != Faction.Lure)
			{
				return base.Actor.Hitpoints <= 0;
			}
			return true;
		}
	}

	public float Delay { get; set; }

	public ActorModel DamagerActor { get; set; }

	private ActorView DamagerView { get; set; }

	private IEnumerator ShowDamageHandle { get; set; }

	public bool ForceChargePoint { get; set; }

	public bool IsCritical
	{
		get
		{
			if (base.Action is DamageAction damageAction)
			{
				return damageAction.Critical;
			}
			return false;
		}
	}

	public bool IsFollowThrough
	{
		get
		{
			if (base.Action is DamageAction damageAction)
			{
				return damageAction.IsFollowThrough;
			}
			return false;
		}
	}

	public bool IsPushDamage
	{
		get
		{
			if (base.Action is DamageAction damageAction)
			{
				return damageAction.IsPushDamage;
			}
			return false;
		}
	}

	private bool Done { get; set; }

	public DamageVisualizationTask(DamageAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		combat = GameManager.Instance.playerModel.Combat;
		if (!DeathVisualizationTriggered && !action.IsPushDamage)
		{
			AddFactionDependency(base.Actor.Faction);
			AddActorDependency(base.Actor);
		}
		else if (action.IsPushDamage && DeathVisualizationTriggered)
		{
			DeathVisualizationTask mostRecentlyAddedActorTask = VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<DeathVisualizationTask>(base.Actor);
			mostRecentlyAddedActorTask.AddDependency(mostRecentlyAddedActorTask.Attacker);
		}
		DamagerActor = action.DamagerActor;
		if (DamagerActor != null)
		{
			AddDependency(DamagerActor, reserve: false);
			DelayedActionGrenadeThrowVisualizationTask.AddDependenciesForPendingThrowAttacker(this, DamagerActor);
			DamagerView = GameManager.Instance.GetViewForModel(DamagerActor) as ActorView;
		}
		if (combat != null)
		{
			if (DamagerActor != null)
			{
				targetInCover = combat.IsInCover(base.Actor.GridCoordinate, DamagerActor.GridCoordinate);
			}
			targetFlanked = combat.IsCoverFlanked(base.Actor.GridCoordinate, base.Actor);
			damagerMoveCompleted = base.Actor.MoveCompleted;
		}
		targetStrugglesLeft = base.Actor.StrugglesLeft;
		TurnToTargetVisualizationTask mostRecentlyAddedActorTask2 = VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<TurnToTargetVisualizationTask>(DamagerActor);
		if (action.IsFollowThrough)
		{
			mostRecentlyAddedActorTask2?.SetGlobalBlocker(blocking: true);
		}
	}

	public void StartDamage()
	{
		Start();
		ReleaseDependency(DamagerActor);
		if ((base.Action as DamageAction).OriginalTargetFaction != Faction.Lure)
		{
			Done = false;
			if (DamagerView != null && !DeathVisualizationTriggered && !base.Actor.IsStruggling && base.ActorView.CharacterAnimationController != null)
			{
				Vector3 direction = Vector3.Normalize(base.ActorView.transform.position - DamagerView.transform.position);
				base.ActorView.CharacterAnimationController.MeleeDamage(IsCritical, direction);
			}
			ShowDamageHandle = ShowDamage();
			GameManager.Instance.StartCoroutine(ShowDamageHandle);
		}
		else
		{
			Done = true;
		}
	}

	private IEnumerator ShowDamage()
	{
		ActorView actorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		GameObject gameObject = ((DamagerView != null) ? DamagerView.GetCurrentWeaponPrefab() : null);
		DamageAction action = base.Action as DamageAction;
		WeaponEffectsSpawner weaponEffectsSpawner = ((gameObject == null) ? null : gameObject.GetComponent<WeaponEffectsSpawner>());
		if (weaponEffectsSpawner != null)
		{
			float bulletFlightTime = weaponEffectsSpawner.BulletFlightTime;
			yield return new WaitForSeconds(bulletFlightTime);
		}
		else if (!DeathVisualizationTriggered)
		{
			yield return new WaitForSeconds(0.5f);
		}
		if (DamagerView != null && DamagerActor != null && DamagerActor.LastOOT != OOTType.None && DamagerActor.LastOOT == OOTType.FreeAttack)
		{
			DamagerView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.FreeAttack")));
		}
		if (base.Actor != null && base.ActorView != null && !base.Actor.IsDead && action.BodyShot)
		{
			base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.BodyShot")));
		}
		if (action.DamageRelatedVisualisations != null)
		{
			foreach (KeyValuePair<ActorModel, List<DamageNotificationData>> damageRelatedVisualisation in action.DamageRelatedVisualisations)
			{
				ActorModel key = damageRelatedVisualisation.Key;
				bool isDead = key.IsDead;
				if ((isDead && !damageRelatedVisualisation.Value.Contains(new DamageNotificationData("FollowThrough", dueLuck: false))) || !(GameManager.Instance.GetViewForModel(key) as ActorView != null))
				{
					continue;
				}
				foreach (DamageNotificationData item in damageRelatedVisualisation.Value)
				{
					if (!isDead || item.TraitIdentifier == "FollowThrough")
					{
						string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Traits." + item.TraitIdentifier);
						string icon = "Ui_Icon_Trait_" + item.TraitIdentifier;
						DamagerView.AddNotification(new ActorNotificationMessage(localizedText, icon), item.DueLuck);
					}
				}
			}
		}
		if (action.Dodged && !action.Critical)
		{
			if (DamagerView != null)
			{
				DamagerView.IsAttackDodged = true;
			}
			if (base.ActorView != null)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Dodge"), "Ui_Icon_Trait_Dodge"), action.ProbabilityOutcome == PlayerRandomChanceResult.SuccessDueToExtension);
			}
		}
		else if (base.ActorView != null && action.FinalDamage != 0)
		{
			if (action.SavedFromDeath)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.SecondChance"), "Ui_Icon_Trait_LeaderBuffSecondChance"), action.ProbabilityOutcome == PlayerRandomChanceResult.SuccessDueToExtension);
			}
			else if (action.Critical && !action.Dodged)
			{
				int size = 32;
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Critical"), ActorNotificationType.AttackEvent, size, NotificationSound.CriticalHit), action.ProbabilityOutcome == PlayerRandomChanceResult.SuccessDueToExtension);
				base.ActorView.AddNotification(new ActorNotificationMessage(Mathf.Abs(action.FinalDamage).ToString(), ActorNotificationType.Damage, size));
				if (base.Actor.HasTraitsThatContains("SureShot") && !damagerMoveCompleted)
				{
					base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.SureShot", "Ui_Icon_Trait_SureShot")));
				}
			}
			else if (action.DamageType == DamageType.Fire)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(Mathf.Abs(action.FinalDamage).ToString(), ActorNotificationType.DamageFire));
			}
			else if (action.DamageType == DamageType.Poison)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(Mathf.Abs(action.FinalDamage).ToString(), ActorNotificationType.DamagePoison));
			}
			else if (action.DamageType == DamageType.GrenadeFragmentDamage)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(Mathf.Abs(action.FinalDamage).ToString(), ActorNotificationType.DamageGrenade));
			}
			else if (action.DamageType == DamageType.Bleeding)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(Mathf.Abs(action.FinalDamage).ToString(), ActorNotificationType.DamageBleeding));
			}
			else if (!action.DamageIgnored)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(Mathf.Abs(action.FinalDamage).ToString(), ActorNotificationType.Damage));
			}
			if (targetInCover && !targetFlanked && !base.Actor.IsWalker && !base.Actor.IsEnvironmental && action.DamageType == DamageType.Ranged && base.ActorView.HealthIndicator != null)
			{
				base.ActorView.HealthIndicator.PlayCoverIconEffect();
			}
		}
		if (DamagerView != null && action.ProbabilityOutcome == PlayerRandomChanceResult.SuccessDueToExtension && !action.Dodged)
		{
			string localizedText2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorNotification.Lucky");
			DamagerView.AddNotification(new ActorNotificationMessage(localizedText2, "Ui_Icon_Trait_Lucky"));
		}
		if (base.ActorView != null && base.Actor != null && base.Actor.Faction == Faction.Survivor)
		{
			GameEconomyData gameEconomyData = base.Actor.manager.GameEconomyData;
			float num = action.LowestHpBeforeDmg + base.Actor.MaxHitPoints * targetStrugglesLeft;
			float num2 = action.LowestHpAfterDmg + base.Actor.MaxHitPoints * targetStrugglesLeft;
			float num3 = (float)base.Actor.MaxHitPoints * 2f;
			float num4 = 100f * num / num3;
			float num5 = 100f * num2 / num3;
			if (num5 < (float)gameEconomyData.ConfigData.InjuryCriticalBelowHealthPercentage && num4 >= (float)gameEconomyData.ConfigData.InjuryCriticalBelowHealthPercentage)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("SurvivorStatus.Injury.Critical"), ActorNotificationType.Damage, -1, NotificationSound.CriticalInjury));
			}
			else if (num5 < (float)gameEconomyData.ConfigData.InjuryMajorBelowHealthPercentage && num4 >= (float)gameEconomyData.ConfigData.InjuryMajorBelowHealthPercentage)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("SurvivorStatus.Injury.Major"), ActorNotificationType.Damage, -1, NotificationSound.MajorInjury));
			}
			else if (num5 < (float)gameEconomyData.ConfigData.InjuryMinorBelowHealthPercentage && num4 >= (float)gameEconomyData.ConfigData.InjuryMinorBelowHealthPercentage)
			{
				base.ActorView.AddNotification(new ActorNotificationMessage(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("SurvivorStatus.Injury.Minor"), ActorNotificationType.Damage, -1, NotificationSound.MinorInjury));
			}
		}
		else if (base.Actor != null && base.ActorView != null)
		{
			if (DamagerActor != null && DamagerActor.Faction == Faction.Survivor && base.Actor.IsDead && !base.ActorView.IsDeathInfoVisualized)
			{
				base.ActorView.IsDeathInfoVisualized = true;
				GameEconomyData gameEconomyData2 = GameManager.Instance.gameEconomyData;
				int num6 = ((gameEconomyData2 != null && gameEconomyData2.ConfigData != null) ? gameEconomyData2.ConfigData.MissionMaxEnemiesKillGivingXP : 0);
				bool shouldCap = ((combat != null && combat.MissionStatistics != null) ? (combat.MissionStatistics.WalkersKilled + combat.MissionStatistics.RaidersKilled) : 0) > num6 && num6 > 0;
				int[] sPGain = base.Actor.GetSPGain(DamagerActor, shouldCap);
				if (sPGain[0] != 0)
				{
					base.ActorView.AddNotification(new ActorNotificationMessage((sPGain[0] + sPGain[1]).ToString(), ActorNotificationType.CurrencySP, -1, NotificationSound.CurrencySP));
				}
				int suppliesGain = base.Actor.GetSuppliesGain(DamagerActor, sPGain[0] + sPGain[1]);
				if (suppliesGain != 0)
				{
					base.ActorView.AddNotification(new ActorNotificationMessage(suppliesGain.ToString(), ActorNotificationType.CurrencySupplies, -1, NotificationSound.CurrencySP));
				}
			}
			string text = "combat_walker/walker_hit";
			if (action.Critical)
			{
				text += "_critical";
			}
			if (base.Actor.Faction == Faction.Walker)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(text, actorView.gameObject);
			}
		}
		if (base.ActorView != null)
		{
			if (!action.Dodged || (action.Dodged && action.Critical))
			{
				ActorHitEffects component = base.ActorView.GetComponent<ActorHitEffects>();
				if (component != null && base.ActorView.CurrentWeapon != null && base.ActorView.CurrentWeapon.Definition.Type != EquipmentType.Grenade)
				{
					if (action.FinalDamage > 0 && !action.Dodged)
					{
						component.SpawnHitEffects(action.DamagerActor);
					}
					else if (action.FinalDamage < 0)
					{
						component.SpawnHealEffects(action.DamagerActor);
					}
				}
			}
			if (!action.SavedFromDeath && !action.IgnoreIndicatorUpdate && !action.DealDamagePostAbility && !action.IsPushDamage)
			{
				float healthIndicatorValue = (float)action.HealthAfterDamage / (float)base.Actor.MaxHitPoints;
				base.ActorView.SetHealthIndicatorValue(healthIndicatorValue);
			}
		}
		if (action.DamageType == DamageType.BloodMarkSettlement && base.ActorView != null)
		{
			base.ActorView.PlayBloodMarkSettleEffect();
			if (base.ActorView.HealthIndicator != null)
			{
				base.ActorView.HealthIndicator.UpdateBloodMark();
			}
		}
		Done = true;
	}

	public override void Stop()
	{
		if (ShowDamageHandle != null && GameManager.Instance != null)
		{
			GameManager.Instance.StopCoroutine(ShowDamageHandle);
		}
	}

	public override bool Update(float deltaTime)
	{
		Delay -= deltaTime;
		if (Delay > 0f)
		{
			return true;
		}
		if (!started)
		{
			StartDamage();
			started = true;
		}
		return !Done;
	}
}
