using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class HealthIndicator : HUDElementFollowTarget
{
	[Serializable]
	private class IgniteBoostCard
	{
		public GameObject container;

		public UILabel turnLabel;

		public UILabel floorLabel;
	}

	public Action EffectGridUpdated;

	[Tooltip("Current actor health progress bar.")]
	public UIProgressBar HealthBar;

	[Tooltip("Current actor ShieldHP progress bar.")]
	public UIProgressBar ShieldHPBar;

	[Tooltip("GameObject that contains the Charge Point background icons.")]
	public GameObject ChargePointContainer;

	[Tooltip("Sprites that indicate the maximum amount of charge points the actor can have.")]
	public GameObject[] ChargePointBgIcons;

	[Tooltip("Sprites that indicate the amount of charge points the actor has.")]
	public UISprite[] ChargePointFgIcons;

	[Tooltip("Sprite of actor class.")]
	public UISprite ActorClass;

	[Tooltip("Sprite of first action point.")]
	public UISprite ActionPoint1;

	[Tooltip("Sprite of second action point.")]
	public UISprite ActionPoint2;

	[Tooltip("Label for actor name.")]
	public UILabel NameLabel;

	[Tooltip("Label for actor level.")]
	public UILabel LevelLabel;

	[Tooltip("Background for actor level.")]
	public UISprite LevelBackgroundSprite;

	[Tooltip("Sprite of the alerted icon.")]
	public UISprite AlertedIcon;

	[Tooltip("Sprite of the free attack warning icon.")]
	public UISprite FreeAttackWarning;

	[Tooltip("Sprite of the tough walker icon.")]
	public UISprite ToughWalkerIcon;

	[Tooltip("Sprite of the boss walker icon.")]
	public UISprite BossWalkerIcon;

	[Tooltip("Sprite of the boss walker icon.")]
	public MultiIconIndicator MultiIconIndicator;

	[Tooltip("List of multiple turn action indicators.")]
	public List<TimedEffectEntry> TimedEffectIndicators = new List<TimedEffectEntry>();

	[Tooltip("Label for multiple turn action turn counter.")]
	public UILabel TurnCountLabel;

	[Tooltip("Sprite of the cover icon.")]
	public UISprite CoverIcon;

	[Tooltip("Sprite of the Status Indicator.")]
	public GameObject StatusIndicator;

	[Tooltip("Status Effect Forward Delay")]
	public float AnimationForwardDelay;

	[Tooltip("Status Effect Reverse Delay")]
	public float AnimationReverseDelay;

	[Tooltip("Status Effect Sprite")]
	public UISprite StatusIndicatorSprite;

	[Tooltip("Status Effect Reverse Animation")]
	public TweenAlpha StatusIndicatorTweenAlpha;

	[Tooltip("Status Effect Reverse Animation")]
	public TweenAlpha TurnIndicatorTweenAlpha;

	[Tooltip("Faction of Actor")]
	public Faction ActorFaction;

	[Tooltip("Secondary Status Effect Sprite")]
	public UISprite SecondaryStatusIndicatorSprite;

	[Tooltip("Sprites that indicate the amount of KnockKnockMarks the actor has.")]
	public UISprite[] KnockKnockMarkIcons;

	[SerializeField]
	private GameObject KnockKnockContainer;

	public GameObject PhonePortrait;

	public UISprite ABtestB;

	public GameObject ScorchIndicator;

	public UILabel ScorchTurnCountLabel;

	public UISprite ScorchLayerSprite;

	[SerializeField]
	private GameObject TauntEffects;

	[SerializeField]
	private GameObject SneakContainer;

	[SerializeField]
	private GameObject FlameTriggerContainer;

	[SerializeField]
	private UILabel FlameTriggerLayerLabel;

	[SerializeField]
	private UILabel FlameTriggerTurnsLabel;

	[SerializeField]
	private List<IgniteBoostCard> IgniteBoostList;

	[SerializeField]
	private UILabel TauntTurnCountLabel;

	[SerializeField]
	private UILabel FistSpikeTurns;

	[SerializeField]
	private GameObject FistSpikeContainer;

	[SerializeField]
	private UILabel DodgeShoteTurns;

	[SerializeField]
	private GameObject DodgeShoteContainer;

	[SerializeField]
	private UIGrid EffectGrid;

	[SerializeField]
	private UILabel SkinnedTurns;

	[SerializeField]
	private GameObject SkinnedContainer;

	[SerializeField]
	private UILabel RemoteTurns;

	[SerializeField]
	private GameObject RemoteContainer;

	[SerializeField]
	private UILabel GrenadeTurns;

	[SerializeField]
	private GameObject GrenadeContainer;

	[SerializeField]
	private UILabel AstheniaTurns;

	[SerializeField]
	private GameObject AstheniaContainer;

	[SerializeField]
	private UILabel PoisonTurns1;

	[SerializeField]
	private GameObject PoisonContainer1;

	[SerializeField]
	private UISprite PoisonSprite1;

	[SerializeField]
	private UILabel PoisonTurns3;

	[SerializeField]
	private GameObject PoisonContainer3;

	[SerializeField]
	private UISprite PoisonSprite3;

	[SerializeField]
	private UILabel PoisonTurns2;

	[SerializeField]
	private GameObject PoisonContainer2;

	[SerializeField]
	private UISprite PoisonSprite2;

	[SerializeField]
	private UILabel EquipmentChargeLoadedNums;

	[SerializeField]
	private GameObject EquipmentActiveChargeLoadContainer;

	[SerializeField]
	private GameObject ElectronChargeContainer;

	[SerializeField]
	private GameObject ElectronChargeEnemyContainer;

	[SerializeField]
	private UILabel QuantunTurns;

	[SerializeField]
	private GameObject QuantunContainer;

	[SerializeField]
	private GameObject RiposteContainer;

	[SerializeField]
	private GameObject MomentumContainer;

	[SerializeField]
	private GameObject SuvivalDashContainer;

	[SerializeField]
	private GameObject RaiderDashContainer;

	[SerializeField]
	private GameObject AttackChainContainer1;

	[SerializeField]
	private UILabel AttackChainTurns1;

	[SerializeField]
	private GameObject AttackChainContainer2;

	[SerializeField]
	private UILabel AttackChainTurns2;

	[SerializeField]
	private GameObject AttackChainContainer3;

	[SerializeField]
	private UILabel AttackChainTurns3;

	[SerializeField]
	private GameObject BloodFrenzyContainer;

	[SerializeField]
	private GameObject TornApartContainer;

	[SerializeField]
	private UILabel TornApartTurns;

	[SerializeField]
	private GameObject SurvivalGameContainer;

	[SerializeField]
	private GameObject UnluckyContainer;

	[SerializeField]
	private GameObject UnluckyContainer2;

	[SerializeField]
	private GameObject DeadlyFocusRaiderContainer;

	[SerializeField]
	private GameObject DeadlyFocusSurvivorContainer;

	[SerializeField]
	private GameObject DeadlyFocus_SkillIncreaseAttackContainer;

	[SerializeField]
	private GameObject ShieldBreakerContainer;

	[SerializeField]
	private GameObject BlindContainer;

	[SerializeField]
	private GameObject SurvivalManualStorySkill_D_Container;

	[SerializeField]
	private GameObject SurvivalManualStorySkill_F_Container;

	[SerializeField]
	private GameObject ReduceRecoveryContainer;

	[SerializeField]
	private GameObject ShadowedGuardContainer;

	[SerializeField]
	private GameObject RageContainer;

	[SerializeField]
	private GameObject VengefulChargeContainer;

	private CoverIconState coverIconState;

	private float coverSparkleTimer;

	private List<ActorStatusInfoHealthBar> activeTimedEffects = new List<ActorStatusInfoHealthBar>();

	private int statusIndex;

	private bool updatingStatusEffects;

	protected ActorModel BindActor;

	private int PoisonMaxShow = 3;

	private int ChainAttackShow = 3;

	[Tooltip("被光塔 影响 特效")]
	[SerializeField]
	private GameObject CitadelBeEffectedContainer_icon;

	[SerializeField]
	private GameObject UndyingStateContainer;

	[SerializeField]
	private GameObject UndyingStateOpenObj;

	[SerializeField]
	private GameObject UndyingStateCloseObj;

	[SerializeField]
	private UILabel UndyingStateTurnsLabel;

	[SerializeField]
	private UILabel UndyingStateCountLabel;

	[SerializeField]
	private GameObject KaboomStateContainer;

	[SerializeField]
	private UILabel KaboomStateLabel;

	[SerializeField]
	private GameObject GuardianStateContainer;

	[SerializeField]
	private UILabel GuardianStateLabel;

	[SerializeField]
	private GameObject SovereignStateContainer;

	[SerializeField]
	private UILabel SovereignStateLabel;

	[SerializeField]
	private GameObject DeathsDoorContainer;

	[SerializeField]
	private GameObject BloodMarkContainer;

	[SerializeField]
	private GameObject TridentContainer;

	[SerializeField]
	private GameObject FortificationsContainer;

	public virtual bool IsScreenTopBossBar => false;

	private void OnDisable()
	{
		if (updatingStatusEffects)
		{
			StopCoroutine("SetStatusEffect");
			updatingStatusEffects = false;
		}
	}

	private void Start()
	{
		OnTurnCountChangedEvent();
	}

	private void Update()
	{
		if (coverSparkleTimer > 0f)
		{
			coverSparkleTimer -= Time.deltaTime;
		}
		else
		{
			DisableCoverIconEffect();
		}
	}

	public void SetBindActor(ActorModel actor)
	{
		BindActor = actor;
	}

	private void RepositionEffectGrid()
	{
		if (EffectGrid != null)
		{
			EffectGrid.repositionNow = true;
		}
		EffectGridUpdated?.Invoke();
	}

	private IEnumerator SetStatusEffect()
	{
		if (StatusIndicator == null)
		{
			yield break;
		}
		updatingStatusEffects = true;
		SetTimedEffectIndicatorVisibility(visible: true);
		int cachedEffectCount = activeTimedEffects.Count;
		while (activeTimedEffects.Count > 1 && StatusIndicator.activeInHierarchy && activeTimedEffects.Count == cachedEffectCount)
		{
			if (statusIndex > activeTimedEffects.Count - 1)
			{
				statusIndex = 0;
			}
			ActorStatusInfoHealthBar activeStatusInfo = activeTimedEffects[statusIndex];
			SetupStatusIndicator(activeStatusInfo);
			yield return new WaitForSecondsRealtime(AnimationForwardDelay);
			if (StatusIndicatorTweenAlpha != null)
			{
				StatusIndicatorTweenAlpha.PlayReverse();
			}
			if (TurnIndicatorTweenAlpha != null)
			{
				TurnIndicatorTweenAlpha.PlayReverse();
			}
			yield return new WaitForSecondsRealtime(AnimationReverseDelay);
			statusIndex++;
			yield return null;
		}
		DisableMultipleTurnIndicator();
		yield return null;
	}

	public void UpdateChargeMeterIcons(ChargeMeterModel model)
	{
		if (model == null || ChargePointFgIcons == null || model.Actor.Faction != Faction.Survivor)
		{
			return;
		}
		int maxLevel = model.MaxLevel;
		for (int i = 0; i < maxLevel && i < ChargePointFgIcons.Length; i++)
		{
			if (ChargePointFgIcons[i] != null)
			{
				ChargePointFgIcons[i].spriteName = model.GetLevelSpriteName(i);
			}
		}
	}

	public void SetTimedEffectIndicator(List<ActorStatusInfoHealthBar> types)
	{
		activeTimedEffects = types;
		if (activeTimedEffects == null || activeTimedEffects.Count == 0)
		{
			return;
		}
		if (activeTimedEffects.Count > 1)
		{
			if (!updatingStatusEffects)
			{
				StartCoroutine(SetStatusEffect());
			}
		}
		else
		{
			ActorStatusInfoHealthBar activeStatusInfo = activeTimedEffects[0];
			SetTimedEffectIndicatorVisibility(visible: true);
			SetupStatusIndicator(activeStatusInfo);
		}
	}

	public void SetSecondaryTimedEffectIndicator(ActorStatusInfoHealthBar status)
	{
		SetupSecondaryStatusIndicator(status);
	}

	public void DisableMultipleTurnIndicator()
	{
		if (StatusIndicator != null)
		{
			StatusIndicator.gameObject.SetActive(value: false);
		}
		updatingStatusEffects = false;
		UpdateTurnCount("");
		activeTimedEffects.Clear();
		if (ActorClass != null)
		{
			ActorClass.gameObject.SetActive(value: true);
		}
	}

	public void SetSneakContainer(bool visible)
	{
		Helpers.GameObjectSetActive(SneakContainer, visible);
	}

	public void SetTauntEffectIndicatorVisibility(bool visible, int count)
	{
		string content = ((count > 0) ? count.ToString() : "");
		Helpers.GameObjectSetActive(TauntEffects, visible);
		HelpersUI.SetContentToLabel(TauntTurnCountLabel, content);
		RepositionEffectGrid();
	}

	public void SetScorchTimedEffectIndicatorVisibility(bool visible, int count)
	{
		string content = ((count > 0) ? count.ToString() : "");
		Helpers.GameObjectSetActive(ScorchIndicator, visible);
		HelpersUI.SetContentToLabel(ScorchTurnCountLabel, content);
	}

	public void SetScorchTimedEffectIndicatorLayer(float amount)
	{
		if (ScorchLayerSprite != null)
		{
			ScorchLayerSprite.fillAmount = amount;
		}
	}

	public void SetTimedEffectIndicatorVisibility(bool visible)
	{
		if (StatusIndicator != null)
		{
			Helpers.GameObjectSetActive(StatusIndicator, visible);
		}
	}

	public void UpdateTurnCount(string text)
	{
		HelpersUI.SetContentToLabel(TurnCountLabel, text);
	}

	public void SetCoverIconEnabled(CoverIconState state)
	{
		if (!(CoverIcon != null))
		{
			return;
		}
		CoverIcon.gameObject.SetActive(state != CoverIconState.None);
		if (state != CoverIconState.None)
		{
			CoverIcon.spriteName = HelpersGfx.GetCoverIconName(state);
			if (coverIconState != state)
			{
				coverIconState = state;
				TweenManager.PlayTweenGroup(CoverIcon.gameObject, 20, forward: true, OnCoverIconStateChangeTweenPlayed);
			}
		}
	}

	public void PlayCoverIconEffect()
	{
		if (!(CoverIcon != null))
		{
			return;
		}
		TweenManager.PlayTweenGroup(CoverIcon.gameObject, 10);
		EffectSparkle component = CoverIcon.GetComponent<EffectSparkle>();
		if (!(component != null) || component.enabled)
		{
			return;
		}
		for (int i = 0; i < component.currentSparkle.Sparkles.Count; i++)
		{
			if (component.currentSparkle.Sparkles[i].Duration > coverSparkleTimer)
			{
				coverSparkleTimer = component.currentSparkle.Sparkles[i].Duration;
			}
		}
		component.enabled = true;
	}

	private void DisableCoverIconEffect()
	{
		if (CoverIcon != null)
		{
			EffectSparkle component = CoverIcon.GetComponent<EffectSparkle>();
			if (component != null && component.enabled)
			{
				component.enabled = false;
				coverSparkleTimer = 0f;
			}
		}
	}

	private void OnCoverIconStateChangeTweenPlayed()
	{
		if (CoverIcon != null && coverIconState == CoverIconState.Flanked)
		{
			TweenManager.PlayTweenGroup(CoverIcon.gameObject, 0);
		}
	}

	private void SetupStatusIndicator(ActorStatusInfoHealthBar activeStatusInfo)
	{
		if (StatusIndicator == null || StatusIndicatorSprite == null)
		{
			return;
		}
		if (ActorFaction == Faction.Raider || ActorFaction == Faction.Survivor)
		{
			if (ActorClass != null)
			{
				Helpers.GameObjectSetActive(ActorClass, value: false);
			}
		}
		else if (ActorClass != null)
		{
			Helpers.GameObjectSetActive(ActorClass, value: true);
		}
		int num = TimedEffectIndicators.FindIndex((TimedEffectEntry x) => x.TimedEffectType == activeStatusInfo.StatusType);
		if (num >= 0)
		{
			TimedEffectEntry timedEffectEntry = TimedEffectIndicators[num];
			StatusIndicatorSprite.spriteName = timedEffectEntry.Sprite;
			StatusIndicatorSprite.gradientTop = timedEffectEntry.GradientTop;
			StatusIndicatorSprite.gradientBottom = timedEffectEntry.GradientBottom;
			if (StatusIndicatorTweenAlpha != null)
			{
				StatusIndicatorTweenAlpha.PlayForward();
			}
			if (TurnIndicatorTweenAlpha != null)
			{
				TurnIndicatorTweenAlpha.PlayForward();
			}
			TweenManager.PlayTweenGroup(StatusIndicator, timedEffectEntry.TweenGroupId);
			UpdateTurnCount((activeStatusInfo.TurnCount > 0) ? activeStatusInfo.TurnCount.ToString() : "");
		}
	}

	private void SetupSecondaryStatusIndicator(ActorStatusInfoHealthBar activeStatusInfo)
	{
		if ((bool)SecondaryStatusIndicatorSprite)
		{
			int num = TimedEffectIndicators.FindIndex((TimedEffectEntry x) => x.TimedEffectType == activeStatusInfo.StatusType);
			if (num >= 0)
			{
				TimedEffectEntry timedEffectEntry = TimedEffectIndicators[num];
				Helpers.GameObjectSetActive(SecondaryStatusIndicatorSprite, value: true);
				SecondaryStatusIndicatorSprite.spriteName = timedEffectEntry.Sprite;
				SecondaryStatusIndicatorSprite.gradientTop = timedEffectEntry.GradientTop;
				SecondaryStatusIndicatorSprite.gradientBottom = timedEffectEntry.GradientBottom;
			}
		}
	}

	public void ClearSecondaryStatusEffect()
	{
		Helpers.GameObjectSetActive(SecondaryStatusIndicatorSprite, value: false);
	}

	public void ClearKnockKnockMarkIcons()
	{
		Helpers.GameObjectSetActive(KnockKnockContainer, value: false);
		if (KnockKnockMarkIcons != null && KnockKnockMarkIcons.Length != 0)
		{
			for (int i = 0; i < KnockKnockMarkIcons.Length; i++)
			{
				Helpers.GameObjectSetActive(KnockKnockMarkIcons[i], value: false);
			}
		}
	}

	public void UpdateKnockKnockMarkIcons(ActorModel actor)
	{
		if (actor == null)
		{
			ClearKnockKnockMarkIcons();
			return;
		}
		CombatModel combatModel = actor.manager.CombatModel;
		if (combatModel == null || combatModel.MissionCompleted)
		{
			ClearKnockKnockMarkIcons();
			return;
		}
		FixedPoint debuffKnockKnockMarkMaxConfig = actor.DebuffKnockKnockMarkMaxConfig;
		if (debuffKnockKnockMarkMaxConfig <= 0L)
		{
			ClearKnockKnockMarkIcons();
			return;
		}
		if (KnockKnockMarkIcons == null || KnockKnockMarkIcons.Length < debuffKnockKnockMarkMaxConfig)
		{
			ClearKnockKnockMarkIcons();
			return;
		}
		Helpers.GameObjectSetActive(KnockKnockContainer, value: true);
		for (int i = 0; i < KnockKnockMarkIcons.Length; i++)
		{
			if (debuffKnockKnockMarkMaxConfig == i + 1)
			{
				Helpers.GameObjectSetActive(KnockKnockMarkIcons[i], value: true);
				UISprite component = KnockKnockMarkIcons[i].transform.Find("Foreground").GetComponent<UISprite>();
				FixedPoint fixedPoint = actor.DebuffKnockKnockMarkCount / debuffKnockKnockMarkMaxConfig;
				component.fillAmount = (float)fixedPoint;
			}
			else
			{
				Helpers.GameObjectSetActive(KnockKnockMarkIcons[i], value: false);
			}
		}
		RepositionEffectGrid();
	}

	public void UpdatePhonePortrait(bool isActive)
	{
		if (PhonePortrait != null)
		{
			PhonePortrait.SetActive(isActive);
		}
		RepositionEffectGrid();
	}

	public void UpdateABtestB(bool isActive)
	{
		Helpers.GameObjectSetActive(ABtestB, isActive);
		RepositionEffectGrid();
	}

	public void UpdateSkinned(bool isActive)
	{
		if (BindActor != null)
		{
			TraitEntry trait = BindActor.TraitContainer.GetTrait("Skinned");
			long num = trait?.TraitDuration ?? 0;
			if (trait != null && num > 0)
			{
				HelpersUI.SetContentToLabel(SkinnedTurns, num.ToString());
				Helpers.GameObjectSetActive(SkinnedContainer, isActive);
			}
			else
			{
				Helpers.GameObjectSetActive(SkinnedContainer, value: false);
			}
			if (EffectGrid != null)
			{
				RepositionEffectGrid();
			}
		}
	}

	public void UpdateGrenade(bool isActive)
	{
		if (BindActor != null)
		{
			if (BindActor.GetBeGrenadeFragmentDamagedList().Count > 0)
			{
				HelpersUI.SetContentToLabel(GrenadeTurns, "");
				Helpers.GameObjectSetActive(GrenadeContainer, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(GrenadeContainer, value: false);
			}
			if (EffectGrid != null)
			{
				RepositionEffectGrid();
			}
		}
	}

	public void UpdateAsthenia(bool isActive)
	{
		if (BindActor != null)
		{
			int astheniaLeftTurns = BindActor.GetAstheniaLeftTurns();
			if (astheniaLeftTurns > 0)
			{
				HelpersUI.SetContentToLabel(AstheniaTurns, astheniaLeftTurns.ToString());
				Helpers.GameObjectSetActive(AstheniaContainer, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(AstheniaContainer, value: false);
			}
			if (EffectGrid != null)
			{
				RepositionEffectGrid();
			}
		}
	}

	public void UpdateRemote(bool isActive)
	{
		if (BindActor == null)
		{
			return;
		}
		if (BindActor.IsRemoteWeakened)
		{
			Helpers.GameObjectSetActive(RemoteContainer, value: false);
			long remoteWeakenLeftTurns = BindActor.GetRemoteWeakenLeftTurns();
			if (remoteWeakenLeftTurns > 0)
			{
				HelpersUI.SetContentToLabel(RemoteTurns, remoteWeakenLeftTurns.ToString());
				Helpers.GameObjectSetActive(RemoteContainer, value: true);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(RemoteContainer, value: false);
		}
		if (EffectGrid != null)
		{
			RepositionEffectGrid();
		}
	}

	public void UpdateIgniteBoost()
	{
		for (int i = 0; i < IgniteBoostList.Count; i++)
		{
			Helpers.GameObjectSetActive(IgniteBoostList[i].container, value: false);
		}
		if (BindActor == null)
		{
			return;
		}
		Dictionary<Faction, HeirloomsHershelFetter> heirloomsHershelFetterFloor = BindActor.HeirloomsHershelFetterFloor;
		if (heirloomsHershelFetterFloor == null || heirloomsHershelFetterFloor.Count <= 0)
		{
			return;
		}
		List<HeirloomsHershelFetter> list = heirloomsHershelFetterFloor.Values.ToList();
		int num = UtilsMath.Clamp(list.Count, 0, IgniteBoostList.Count);
		for (int j = 0; j < num; j++)
		{
			HeirloomsHershelFetter heirloomsHershelFetter = list[j];
			IgniteBoostCard igniteBoostCard = IgniteBoostList[j];
			if (igniteBoostCard != null && heirloomsHershelFetter != null)
			{
				igniteBoostCard.floorLabel.text = heirloomsHershelFetter.Floor.ToString() ?? "";
				igniteBoostCard.turnLabel.text = heirloomsHershelFetter.Roundm.ToString() ?? "";
				Helpers.GameObjectSetActive(igniteBoostCard.container, value: true);
			}
		}
		RepositionEffectGrid();
	}

	public void UpdatePoison(bool isActive)
	{
		if (BindActor == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject> { PoisonContainer1, PoisonContainer2, PoisonContainer3 };
		if (BindActor.IsBePoisoned())
		{
			List<PerPoisonStatus> bePoisonedLayerList = BindActor.GetBePoisonedLayerList();
			List<UILabel> list2 = new List<UILabel> { PoisonTurns1, PoisonTurns2, PoisonTurns3 };
			List<UISprite> list3 = new List<UISprite> { PoisonSprite1, PoisonSprite2, PoisonSprite3 };
			for (int i = 0; i < PoisonMaxShow; i++)
			{
				if (i < bePoisonedLayerList.Count)
				{
					string text = "Ui_Icon_Trait_Poison_";
					text += bePoisonedLayerList[i].LayerCount;
					HelpersUI.SetContentToLabel(list2[i], bePoisonedLayerList[i].LeftTurns.ToString());
					list3[i].spriteName = text;
					Helpers.GameObjectSetActive(list[i], value: true);
				}
				else
				{
					Helpers.GameObjectSetActive(list[i], value: false);
				}
			}
		}
		else
		{
			for (int j = 0; j < PoisonMaxShow; j++)
			{
				Helpers.GameObjectSetActive(list[j], value: false);
			}
		}
		RepositionEffectGrid();
	}

	public void OnTurnCountChangedEvent()
	{
		UpdateIgniteBoost();
		UpdatePoison(isActive: true);
		UpdateSkinned(isActive: true);
		UpdateEquipmentChargeLoaded();
		ShieldChanged();
		UpdateFistSpike();
		UpdateDodgeShot();
		UpdateElectronChargeState();
		UpdateFlameTrigger();
		UpdateQuantun();
		UpdateMomentum();
		UpdateRiposte();
		UpdateSurvivalDashFlag();
		UpdateRaiderDashFlag();
		UpdateAttackChainFlag();
		UpdateTornApartFlag();
		UpdateBloodFrenzyFlag();
		UpdateSurvivalGame();
		UpdateUnlucky();
		UpdateUnlucky2();
		UpdateDeadlyFocus();
		UpdateShieldBreaker();
		UpdateBlind();
		UpdateSurvivalManualStorySkill_D();
		UpdateSurvivalManualStorySkill_F();
		UpdateReduceRecovery();
		UpdateShadowedGuard();
		UpdateRage();
		UpdateVengefulCharge();
		UpdateCitadelBeEffected();
		UpdateUndyingStateContainerEffected();
		UpdateKaboomStateContainerEffected();
		UpdateGuardianStateContainerEffected();
		UpdateSovereignStateContainerEffected();
		UpdateBloodMark();
		UpdateTrident();
		UpdateFortifications();
	}

	public void OnNewTurn()
	{
		UpdateDeadlyFocus();
	}

	public void UpdateEquipmentChargeLoaded()
	{
		if (BindActor != null)
		{
			int num = (int)BindActor.ChargeLoadFloor;
			if (num > 0)
			{
				HelpersUI.SetContentToLabel(EquipmentChargeLoadedNums, num.ToString());
				Helpers.GameObjectSetActive(EquipmentActiveChargeLoadContainer, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(EquipmentActiveChargeLoadContainer, value: false);
			}
			RepositionEffectGrid();
		}
	}

	public void ShieldChanged()
	{
		if (BindActor != null && !(ShieldHPBar == null))
		{
			if (BindActor.MaxShieldHitPoints <= 0)
			{
				Helpers.GameObjectSetActive(ShieldHPBar, value: false);
				return;
			}
			Helpers.GameObjectSetActive(ShieldHPBar, value: true);
			float value = (float)BindActor.ShieldHitPoints / (float)BindActor.MaxShieldHitPoints;
			ShieldHPBar.value = value;
		}
	}

	public void UpdateFlameTrigger()
	{
		if (BindActor != null)
		{
			if (BindActor.ExtraBurnTurn > 0)
			{
				HelpersUI.SetContentToLabel(FlameTriggerTurnsLabel, BindActor.ExtraBurnTurn.ToString());
				HelpersUI.SetContentToLabel(FlameTriggerLayerLabel, BindActor.ExtraBurnLayer.ToString());
				Helpers.GameObjectSetActive(FlameTriggerContainer, value: true);
				Helpers.GameObjectSetActive(FlameTriggerLayerLabel, BindActor.ExtraBurnLayer > 1);
			}
			else
			{
				Helpers.GameObjectSetActive(FlameTriggerContainer, value: false);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateFistSpike()
	{
		if (BindActor != null)
		{
			if (BindActor.IsFistSpike)
			{
				HelpersUI.SetContentToLabel(FistSpikeTurns, BindActor.FistSpikeTurns.ToString());
				Helpers.GameObjectSetActive(FistSpikeContainer, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(FistSpikeContainer, value: false);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateDodgeShot()
	{
		if (BindActor != null)
		{
			if (BindActor.IsDodgeShot)
			{
				HelpersUI.SetContentToLabel(DodgeShoteTurns, BindActor.DodgeShotTurns.ToString());
				Helpers.GameObjectSetActive(DodgeShoteContainer, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(DodgeShoteContainer, value: false);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateElectronChargeState()
	{
		Helpers.GameObjectSetActive(ElectronChargeContainer, value: false);
		Helpers.GameObjectSetActive(ElectronChargeEnemyContainer, value: false);
		if (BindActor == null)
		{
			return;
		}
		List<PerElectronChargeStatus> beElectronChargeList = BindActor.GetBeElectronChargeList();
		if (beElectronChargeList != null && beElectronChargeList.Count <= 0)
		{
			return;
		}
		PerElectronChargeStatus perElectronChargeStatus = beElectronChargeList.Find((PerElectronChargeStatus t) => t.Faction == Faction.Survivor);
		PerElectronChargeStatus perElectronChargeStatus2 = beElectronChargeList.Find((PerElectronChargeStatus t) => t.Faction == Faction.Raider);
		if (perElectronChargeStatus != null && perElectronChargeStatus.LeftTurns > 0 && perElectronChargeStatus.LayerCount > 0)
		{
			int num = perElectronChargeStatus.LayerCount;
			if (num >= 3)
			{
				num = 3;
			}
			Helpers.GameObjectSetActive(ElectronChargeContainer, value: true);
			HelpersUI.SetContentToLabel(ElectronChargeContainer.GetComponentInChildren<UILabel>(), perElectronChargeStatus.LeftTurns.ToString());
			ElectronChargeContainer.GetComponentInChildren<UISprite>().spriteName = "Ui_Icon_Buff_ElectronCharge_" + num;
		}
		if (perElectronChargeStatus2 != null && perElectronChargeStatus2.LeftTurns > 0 && perElectronChargeStatus2.LayerCount > 0)
		{
			int num2 = perElectronChargeStatus2.LayerCount;
			if (num2 >= 3)
			{
				num2 = 3;
			}
			Helpers.GameObjectSetActive(ElectronChargeEnemyContainer, value: true);
			HelpersUI.SetContentToLabel(ElectronChargeEnemyContainer.GetComponentInChildren<UILabel>(), perElectronChargeStatus2.LeftTurns.ToString());
			ElectronChargeEnemyContainer.GetComponentInChildren<UISprite>().spriteName = "Ui_Icon_Buff_ElectronCharge_Enemy_" + num2;
		}
		RepositionEffectGrid();
	}

	public void UpdateQuantun()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(QuantunContainer, value: false);
			if (BindActor.IsQuantuned)
			{
				HelpersUI.SetContentToLabel(QuantunTurns, BindActor.QuantunTurns.ToString());
				HelpersUI.SetContentToLabel(QuantunContainer.FindInChildren("level").GetComponent<UILabel>(), BindActor.QuantunLevel.ToString());
				Helpers.GameObjectSetActive(QuantunContainer, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateRiposte()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(RiposteContainer, value: false);
			if (BindActor.IsRiposte())
			{
				HelpersUI.SetContentToLabel(RiposteContainer.FindInChildren("level").GetComponent<UILabel>(), BindActor.ParryRiposteIncreaseStorey.ToString());
				Helpers.GameObjectSetActive(RiposteContainer, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateMomentum()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(MomentumContainer, value: false);
			if (BindActor.IsMomentum())
			{
				HelpersUI.SetContentToLabel(MomentumContainer.FindInChildren("level").GetComponent<UILabel>(), BindActor.MomentumTimedEffect?.CurrentLayer.ToString());
				Helpers.GameObjectSetActive(MomentumContainer, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateSurvivalDashFlag()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(SuvivalDashContainer, BindActor.IsSurvivalDashFlag);
			RepositionEffectGrid();
		}
	}

	public void UpdateRaiderDashFlag()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(RaiderDashContainer, BindActor.IsRaiderDashFlag);
			RepositionEffectGrid();
		}
	}

	public void UpdateAttackChainFlag()
	{
		if (BindActor == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject> { AttackChainContainer1, AttackChainContainer2, AttackChainContainer3 };
		if (BindActor.AsTargetAttackChainSlots != null)
		{
			List<int> asTargetAttackChainSlots = BindActor.AsTargetAttackChainSlots;
			List<UILabel> list2 = new List<UILabel> { AttackChainTurns1, AttackChainTurns2, AttackChainTurns3 };
			for (int i = 0; i < ChainAttackShow; i++)
			{
				if (i < asTargetAttackChainSlots.Count)
				{
					HelpersUI.SetContentToLabel(list2[i], asTargetAttackChainSlots[i].ToString());
					Helpers.GameObjectSetActive(list[i], value: true);
				}
				else
				{
					Helpers.GameObjectSetActive(list[i], value: false);
				}
			}
		}
		else
		{
			for (int j = 0; j < ChainAttackShow; j++)
			{
				Helpers.GameObjectSetActive(list[j], value: false);
			}
		}
		RepositionEffectGrid();
	}

	public void UpdateBloodFrenzyFlag()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(BloodFrenzyContainer, BindActor.bloodFrenzyFlag);
			RepositionEffectGrid();
		}
	}

	public void UpdateTornApartFlag()
	{
		if (BindActor != null)
		{
			bool flag = BindActor.TornApartMarkCount > 0L;
			Helpers.GameObjectSetActive(TornApartContainer, flag);
			if (flag)
			{
				TornApartTurns.text = BindActor.TornApartMarkCount.ToString();
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateSurvivalGame()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(SurvivalGameContainer, value: false);
			if (BindActor.IsSurvivalGameEnemy())
			{
				HelpersUI.SetContentToLabel(SurvivalGameContainer.FindInChildren("turns").GetComponent<UILabel>(), BindActor.GetEnemy_SurvivalGameModel()?.LeftCount.ToString());
				Helpers.GameObjectSetActive(SurvivalGameContainer, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateUnlucky()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(UnluckyContainer, value: false);
		UnluckyTimedEffect unluckyTimedEffect = BindActor.UnluckyTimedEffect;
		if (unluckyTimedEffect != null)
		{
			int num = unluckyTimedEffect.Duration - unluckyTimedEffect.Counter;
			if (num > 0)
			{
				HelpersUI.SetContentToLabel(UnluckyContainer.FindInChildren("turns").GetComponent<UILabel>(), num.ToString());
				Helpers.GameObjectSetActive(UnluckyContainer, value: true);
			}
		}
		RepositionEffectGrid();
	}

	public void UpdateUnlucky2()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(UnluckyContainer2, value: false);
			int unluckyFlagTurns = BindActor.UnluckyFlagTurns;
			if (unluckyFlagTurns > 0)
			{
				HelpersUI.SetContentToLabel(UnluckyContainer2.FindInChildren("turns").GetComponent<UILabel>(), unluckyFlagTurns.ToString());
				Helpers.GameObjectSetActive(UnluckyContainer2, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateShieldBreaker()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(ShieldBreakerContainer, value: false);
		ShieldBreakerTimedEffect shieldBreakerTimedEffect = BindActor.ShieldBreakerTimedEffect;
		if (shieldBreakerTimedEffect != null)
		{
			int num = shieldBreakerTimedEffect.Duration - shieldBreakerTimedEffect.Counter;
			if (num > 0)
			{
				HelpersUI.SetContentToLabel(ShieldBreakerContainer.FindInChildren("turns").GetComponent<UILabel>(), num.ToString());
				Helpers.GameObjectSetActive(ShieldBreakerContainer, value: true);
			}
		}
		RepositionEffectGrid();
	}

	public void UpdateDeadlyFocus()
	{
		UpdateDeadlyFocusRaider();
		UpdateDeadlyFocusSurvivor();
		UpdateDeadlyFocus_SkillIncreaseAttack();
		if (BindActor != null && EffectGrid != null)
		{
			RepositionEffectGrid();
		}
	}

	public void UpdateDeadlyFocusRaider()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(DeadlyFocusRaiderContainer, value: false);
			int deadlyFocusLeftCount_SourceRaider = BindActor.DeadlyFocusLeftCount_SourceRaider;
			if (deadlyFocusLeftCount_SourceRaider > 0)
			{
				HelpersUI.SetContentToLabel(DeadlyFocusRaiderContainer.FindInChildren("turns").GetComponent<UILabel>(), deadlyFocusLeftCount_SourceRaider.ToString());
				Helpers.GameObjectSetActive(DeadlyFocusRaiderContainer, value: true);
			}
		}
	}

	public void UpdateDeadlyFocusSurvivor()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(DeadlyFocusSurvivorContainer, value: false);
			int deadlyFocusLeftCount_SourceSurvivor = BindActor.DeadlyFocusLeftCount_SourceSurvivor;
			if (deadlyFocusLeftCount_SourceSurvivor > 0)
			{
				HelpersUI.SetContentToLabel(DeadlyFocusSurvivorContainer.FindInChildren("turns").GetComponent<UILabel>(), deadlyFocusLeftCount_SourceSurvivor.ToString());
				Helpers.GameObjectSetActive(DeadlyFocusSurvivorContainer, value: true);
			}
		}
	}

	public void UpdateDeadlyFocus_SkillIncreaseAttack()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(DeadlyFocus_SkillIncreaseAttackContainer, value: false);
		int deadlyFocus_EXDamageLayerCount = BindActor.DeadlyFocus_EXDamageLayerCount;
		if (deadlyFocus_EXDamageLayerCount > 0)
		{
			CombatModel combatModel = BindActor.manager.CombatModel;
			if (combatModel != null && !combatModel.MissionCompleted && CombatHelpers.GetLeaderBuffDeadlyFocusMan(combatModel, BindActor.Faction) != null)
			{
				HelpersUI.SetContentToLabel(DeadlyFocus_SkillIncreaseAttackContainer.FindInChildren("level").GetComponent<UILabel>(), deadlyFocus_EXDamageLayerCount.ToString());
				Helpers.GameObjectSetActive(DeadlyFocus_SkillIncreaseAttackContainer, value: true);
			}
		}
	}

	public void UpdateBlind()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(BlindContainer, value: false);
		int blindLeftTurns = BindActor.BlindLeftTurns;
		if (blindLeftTurns > 0)
		{
			CombatModel combatModel = BindActor.manager.CombatModel;
			if (combatModel != null && !combatModel.MissionCompleted)
			{
				HelpersUI.SetContentToLabel(BlindContainer.FindInChildren("turns").GetComponent<UILabel>(), blindLeftTurns.ToString());
				Helpers.GameObjectSetActive(BlindContainer, value: true);
			}
		}
	}

	public void UpdateSurvivalManualStorySkill_D()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(SurvivalManualStorySkill_D_Container, value: false);
		int survivalManualStorySkill_DLayerCount = BindActor.SurvivalManualStorySkill_DLayerCount;
		if (survivalManualStorySkill_DLayerCount > 0)
		{
			CombatModel combatModel = BindActor.manager.CombatModel;
			if (combatModel != null && !combatModel.MissionCompleted)
			{
				HelpersUI.SetContentToLabel(SurvivalManualStorySkill_D_Container.FindInChildren("level").GetComponent<UILabel>(), survivalManualStorySkill_DLayerCount.ToString());
				Helpers.GameObjectSetActive(SurvivalManualStorySkill_D_Container, value: true);
			}
		}
	}

	public void UpdateSurvivalManualStorySkill_F()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(SurvivalManualStorySkill_F_Container, value: false);
		int sharpBladeLayers = BindActor.SharpBladeLayers;
		if (sharpBladeLayers > 0)
		{
			CombatModel combatModel = BindActor.manager.CombatModel;
			if (combatModel != null && !combatModel.MissionCompleted)
			{
				HelpersUI.SetContentToLabel(SurvivalManualStorySkill_F_Container.FindInChildren("level").GetComponent<UILabel>(), sharpBladeLayers.ToString());
				Helpers.GameObjectSetActive(SurvivalManualStorySkill_F_Container, value: true);
			}
		}
	}

	public void UpdateReduceRecovery()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(ReduceRecoveryContainer, value: false);
		if (BindActor.IsDebuffReduceRecovery())
		{
			CombatModel combatModel = BindActor.manager.CombatModel;
			if (combatModel != null && !combatModel.MissionCompleted)
			{
				Helpers.GameObjectSetActive(ReduceRecoveryContainer, value: true);
			}
		}
	}

	public void UpdateShadowedGuard()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(ShadowedGuardContainer, value: false);
			if (BindActor.ShadowedGuard_LeftCount > 0)
			{
				HelpersUI.SetContentToLabel(ShadowedGuardContainer.FindInChildren("turns").GetComponent<UILabel>(), BindActor.ShadowedGuard_LeftCount.ToString());
				Helpers.GameObjectSetActive(ShadowedGuardContainer, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateRage()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(RageContainer, value: false);
		if (BindActor.TotalRage > 0 && RageContainer != null)
		{
			CombatModel combatModel = BindActor.manager.CombatModel;
			if (combatModel != null && !combatModel.MissionCompleted)
			{
				HelpersUI.SetContentToLabel(RageContainer.FindInChildren("level").GetComponent<UILabel>(), BindActor.TotalRage.ToString());
				Helpers.GameObjectSetActive(RageContainer, value: true);
			}
		}
	}

	public void UpdateVengefulCharge()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(VengefulChargeContainer, value: false);
		if (BindActor.TotalVengefulChargeNums > 0)
		{
			CombatModel combatModel = BindActor.manager.CombatModel;
			if (combatModel != null && !combatModel.MissionCompleted)
			{
				HelpersUI.SetContentToLabel(VengefulChargeContainer.FindInChildren("level").GetComponent<UILabel>(), BindActor.TotalVengefulChargeNums.ToString());
				Helpers.GameObjectSetActive(VengefulChargeContainer, value: true);
			}
		}
	}

	public void UpdateCitadelBeEffected()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(CitadelBeEffectedContainer_icon, value: false);
			if (BindActor.IsCitadelBeEffected && !BindActor.IsDead)
			{
				Helpers.GameObjectSetActive(CitadelBeEffectedContainer_icon, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateUndyingStateContainerEffected()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(UndyingStateContainer, value: false);
		if (!BindActor.OnRedHealthBar && BindActor.HasTraitsThatContains("Undying") && !BindActor.IsDead)
		{
			if (!BindActor.UndyingState.IsUndying && BindActor.RemainingNumOfUndyingTimes() <= 0)
			{
				return;
			}
			Helpers.GameObjectSetActive(UndyingStateContainer, value: true);
			bool flag = !BindActor.UndyingState.IsUndying && BindActor.TurnsUntilNextUndying() > 0;
			Helpers.GameObjectSetActive(UndyingStateCloseObj, flag);
			Helpers.GameObjectSetActive(UndyingStateTurnsLabel, flag);
			if (flag)
			{
				HelpersUI.SetContentToLabel(UndyingStateTurnsLabel, BindActor.TurnsUntilNextUndying().ToString());
			}
			if (BindActor.RemainingNumOfUndyingTimes() <= 0)
			{
				Helpers.GameObjectSetActive(UndyingStateCountLabel, value: false);
			}
			else
			{
				Helpers.GameObjectSetActive(UndyingStateCountLabel, value: true);
				HelpersUI.SetContentToLabel(UndyingStateCountLabel, BindActor.RemainingNumOfUndyingTimes().ToString());
			}
		}
		RepositionEffectGrid();
	}

	public void UpdateKaboomStateContainerEffected()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(KaboomStateContainer, value: false);
			TraitEntry traitEntry = BindActor.TraitContainer?.GetTrait("DebuffEquipmentKaboom");
			if (traitEntry != null && traitEntry.TraitDuration > 0 && !BindActor.IsDead)
			{
				Helpers.GameObjectSetActive(KaboomStateContainer, value: true);
				HelpersUI.SetContentToLabel(KaboomStateLabel, traitEntry.TraitDuration.ToString());
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateGuardianStateContainerEffected()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(GuardianStateContainer, value: false);
			if (BindActor.GuardianVowBindingAsGuardian != null && BindActor.GuardianVow_LeftTurns > 0 && !BindActor.IsDead)
			{
				Helpers.GameObjectSetActive(GuardianStateContainer, value: true);
				HelpersUI.SetContentToLabel(GuardianStateLabel, BindActor.GuardianVow_LeftTurns.ToString());
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateSovereignStateContainerEffected()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(SovereignStateContainer, value: false);
			if (BindActor.GuardianVowBindingAsSovereign != null && BindActor.GuardianVowBindingAsSovereign.LeftTurns > 0 && !BindActor.IsDead)
			{
				Helpers.GameObjectSetActive(SovereignStateContainer, value: true);
				HelpersUI.SetContentToLabel(SovereignStateLabel, BindActor.GuardianVowBindingAsSovereign.LeftTurns.ToString());
			}
			RepositionEffectGrid();
		}
	}

	public virtual void SyncBossBarFromModel(ActorModel actor, HealthBarUpdateMode mode, float visualRatio = -1f)
	{
	}

	public virtual void ShowBossDefeated()
	{
	}

	public void UpdateDeathsDoor()
	{
		if (BindActor != null)
		{
			Helpers.GameObjectSetActive(DeathsDoorContainer, value: false);
			if (BindActor.DeathsDoor_DmgUpLayer > 0 && DeathsDoorContainer != null)
			{
				HelpersUI.SetContentToLabel(DeathsDoorContainer.FindInChildren("level").GetComponent<UILabel>(), BindActor.DeathsDoor_DmgUpLayer.ToString());
				HelpersUI.SetContentToLabel(DeathsDoorContainer.FindInChildren("turns").GetComponent<UILabel>(), BindActor.DeathsDoor_DmgUpLeftTurns.ToString());
				Helpers.GameObjectSetActive(DeathsDoorContainer, value: true);
			}
			RepositionEffectGrid();
		}
	}

	public void UpdateBloodMark()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(BloodMarkContainer, value: false);
		BloodMarkTimedEffect bloodMarkTimedEffect = BindActor.BloodMarkTimedEffect;
		if (bloodMarkTimedEffect != null && BloodMarkContainer != null)
		{
			int num = bloodMarkTimedEffect.Duration - bloodMarkTimedEffect.Counter;
			if (num > 0)
			{
				GameObject gameObject = BloodMarkContainer.FindInChildren("turns");
				if (gameObject != null)
				{
					HelpersUI.SetContentToLabel(gameObject.GetComponent<UILabel>(), num.ToString());
				}
				GameObject gameObject2 = BloodMarkContainer.FindInChildren("Icon");
				if (gameObject2 != null)
				{
					HelpersUI.SetSprite(gameObject2.GetComponent<UISprite>(), (bloodMarkTimedEffect.MarkFaction == Faction.Survivor) ? "Ui_Icon_Trait_BloodMark_Top" : "Ui_Icon_Trait_BloodMark_R");
				}
				Helpers.GameObjectSetActive(BloodMarkContainer, value: true);
			}
		}
		RepositionEffectGrid();
	}

	public void UpdateTrident()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(TridentContainer, value: false);
		AbilityRangeTridentSkill abilityRangeTridentSkill = null;
		if (BindActor.CommandSkillModelManager != null)
		{
			abilityRangeTridentSkill = BindActor.CommandSkillModelManager.GetActorCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
			if (abilityRangeTridentSkill == null)
			{
				abilityRangeTridentSkill = BindActor.CommandSkillModelManager.GetCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
			}
		}
		if (abilityRangeTridentSkill != null && TridentContainer != null && (abilityRangeTridentSkill.IsActive || abilityRangeTridentSkill.CurrentCharge > 0))
		{
			GameObject gameObject = TridentContainer.FindInChildren("turns");
			if (gameObject != null)
			{
				HelpersUI.SetContentToLabel(gameObject.GetComponent<UILabel>(), abilityRangeTridentSkill.CurrentCharge.ToString());
			}
			Helpers.GameObjectSetActive(TridentContainer, value: true);
		}
		RepositionEffectGrid();
	}

	public void UpdateFortifications()
	{
		if (BindActor == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(FortificationsContainer, value: false);
		FortificationsTimedEffect fortificationsTimedEffect = BindActor.FortificationsTimedEffect;
		if (fortificationsTimedEffect != null && FortificationsContainer != null && fortificationsTimedEffect.LeftTurns > 0)
		{
			GameObject gameObject = FortificationsContainer.FindInChildren("turns");
			if (gameObject != null)
			{
				HelpersUI.SetContentToLabel(gameObject.GetComponent<UILabel>(), fortificationsTimedEffect.LeftTurns.ToString());
			}
			Helpers.GameObjectSetActive(FortificationsContainer, value: true);
		}
		RepositionEffectGrid();
	}

	public List<ActorEffectInfoSnapshot> CaptureVisibleEffects()
	{
		List<ActorEffectInfoSnapshot> list = new List<ActorEffectInfoSnapshot>();
		CaptureCover(list);
		CaptureScorch(list);
		CaptureTaunt(list);
		CaptureSneak(list);
		CaptureKnockKnock(list);
		CapturePhonePortrait(list);
		CaptureABtestB(list);
		CaptureContainer(SkinnedContainer, "Skinned", SkinnedTurns, list);
		CaptureContainer(RemoteContainer, "RemoteWeakened", RemoteTurns, list);
		CaptureContainer(GrenadeContainer, "GrenadeFragment", GrenadeTurns, list);
		CaptureContainer(AstheniaContainer, "Asthenia", AstheniaTurns, list);
		CapturePoison(list);
		CaptureContainer(EquipmentActiveChargeLoadContainer, "EquipmentChargeLoad", null, list);
		CaptureElectronCharge(list);
		CaptureContainer(QuantunContainer, "Quantun", QuantunTurns, list);
		CaptureContainer(RiposteContainer, "Riposte", null, list);
		CaptureContainer(MomentumContainer, "Momentum", null, list);
		CaptureContainer(SuvivalDashContainer, "SurvivalDash", null, list);
		CaptureContainer(RaiderDashContainer, "RaiderDash", null, list);
		CaptureAttackChain(list);
		CaptureContainer(BloodFrenzyContainer, "BloodFrenzy", null, list);
		CaptureContainer(TornApartContainer, "TornApart", TornApartTurns, list);
		CaptureContainer(SurvivalGameContainer, "SurvivalGame", null, list);
		CaptureContainer(UnluckyContainer, "Unlucky", null, list);
		CaptureContainer(UnluckyContainer2, "Unlucky2", null, list);
		CaptureDeadlyFocus(list);
		CaptureContainer(ShieldBreakerContainer, "ShieldBreaker", null, list);
		CaptureContainer(BlindContainer, "Blind", null, list);
		CaptureContainer(SurvivalManualStorySkill_D_Container, "SurvivalManualStorySkill_D", null, list);
		CaptureContainer(SurvivalManualStorySkill_F_Container, "SurvivalManualStorySkill_F", null, list);
		CaptureContainer(ReduceRecoveryContainer, "ReduceRecovery", null, list);
		CaptureContainer(ShadowedGuardContainer, "ShadowedGuard", null, list);
		CaptureContainer(RageContainer, "Rage", null, list);
		CaptureContainer(VengefulChargeContainer, "VengefulCharge", null, list);
		CaptureFlameTrigger(list);
		CaptureContainer(FistSpikeContainer, "FistSpike", FistSpikeTurns, list);
		CaptureContainer(DodgeShoteContainer, "DodgeShot", DodgeShoteTurns, list);
		CaptureIgniteBoost(list);
		CaptureContainer(CitadelBeEffectedContainer_icon, "CitadelBeEffected", null, list);
		CaptureUndying(list);
		CaptureContainer(KaboomStateContainer, "Kaboom", KaboomStateLabel, list);
		CaptureContainer(GuardianStateContainer, "Guardian", GuardianStateLabel, list);
		CaptureContainer(SovereignStateContainer, "Sovereign", SovereignStateLabel, list);
		CaptureContainer(BloodMarkContainer, "BloodMark", null, list);
		CaptureContainer(TridentContainer, "Trident", null, list);
		CaptureContainer(FortificationsContainer, "Fortifications", null, list);
		CaptureSecondaryMarked(list);
		return list;
	}

	private void CaptureCover(List<ActorEffectInfoSnapshot> results)
	{
		if (!(CoverIcon == null) && CoverIcon.gameObject.activeSelf)
		{
			string id = ((coverIconState == CoverIconState.Flanked) ? "Cover_Flanked" : "Cover");
			TryAdd(results, id, CoverIcon, 0);
		}
	}

	private void CaptureScorch(List<ActorEffectInfoSnapshot> results)
	{
		if (!(ScorchIndicator == null) && ScorchIndicator.activeSelf)
		{
			ActorEffectCapture capture = ((ScorchLayerSprite != null) ? HealthBarEffectIconCapture.CaptureFromSprite(ScorchLayerSprite, ScorchIndicator) : HealthBarEffectIconCapture.CaptureContainer(ScorchIndicator));
			TryAdd(results, "Scorch", capture, ParseLabelInt(ScorchTurnCountLabel));
		}
	}

	private void CaptureTaunt(List<ActorEffectInfoSnapshot> results)
	{
		TryAddContainer(results, "Taunt", TauntEffects, ParseLabelInt(TauntTurnCountLabel));
	}

	private void CaptureSneak(List<ActorEffectInfoSnapshot> results)
	{
		TryAddContainer(results, "Sneak", SneakContainer, 0);
	}

	private void CaptureKnockKnock(List<ActorEffectInfoSnapshot> results)
	{
		if (KnockKnockContainer == null || !KnockKnockContainer.activeSelf)
		{
			return;
		}
		UISprite sprite = null;
		if (KnockKnockMarkIcons != null)
		{
			for (int i = 0; i < KnockKnockMarkIcons.Length; i++)
			{
				if (KnockKnockMarkIcons[i] != null && KnockKnockMarkIcons[i].gameObject.activeSelf)
				{
					Transform transform = KnockKnockMarkIcons[i].transform.Find("Foreground");
					UISprite uISprite = ((transform != null) ? transform.GetComponent<UISprite>() : null);
					sprite = ((uISprite != null) ? uISprite : KnockKnockMarkIcons[i]);
					break;
				}
			}
		}
		TryAdd(results, "KnockKnock", sprite, KnockKnockContainer, 0);
	}

	private void CapturePhonePortrait(List<ActorEffectInfoSnapshot> results)
	{
		TryAddContainer(results, "PhonePortrait", PhonePortrait, 0);
	}

	private void CaptureABtestB(List<ActorEffectInfoSnapshot> results)
	{
		if (!(ABtestB == null) && ABtestB.gameObject.activeSelf)
		{
			TryAdd(results, "ABtestB", ABtestB, 0);
		}
	}

	private void CapturePoison(List<ActorEffectInfoSnapshot> results)
	{
		CapturePoisonSlot(PoisonContainer1, PoisonSprite1, PoisonTurns1, results);
		CapturePoisonSlot(PoisonContainer2, PoisonSprite2, PoisonTurns2, results);
		CapturePoisonSlot(PoisonContainer3, PoisonSprite3, PoisonTurns3, results);
	}

	private void CapturePoisonSlot(GameObject container, UISprite sprite, UILabel turnsLabel, List<ActorEffectInfoSnapshot> results)
	{
		if (!(container == null) && container.activeSelf)
		{
			ActorEffectCapture capture = ((sprite != null) ? HealthBarEffectIconCapture.CaptureFromSprite(sprite, container) : HealthBarEffectIconCapture.CaptureContainer(container));
			TryAdd(results, "Poison", capture, ParseLabelInt(turnsLabel));
		}
	}

	private void CaptureElectronCharge(List<ActorEffectInfoSnapshot> results)
	{
		CaptureElectronChargeContainer(ElectronChargeContainer, "ElectronCharge", results);
		CaptureElectronChargeContainer(ElectronChargeEnemyContainer, "ElectronChargeEnemy", results);
	}

	private void CaptureElectronChargeContainer(GameObject container, string id, List<ActorEffectInfoSnapshot> results)
	{
		if (!(container == null) && container.activeSelf)
		{
			UILabel componentInChildren = container.GetComponentInChildren<UILabel>();
			TryAddContainer(results, id, container, ParseLabelInt(componentInChildren));
		}
	}

	private void CaptureAttackChain(List<ActorEffectInfoSnapshot> results)
	{
		CaptureContainer(AttackChainContainer1, "AttackChain", AttackChainTurns1, results);
		CaptureContainer(AttackChainContainer2, "AttackChain", AttackChainTurns2, results);
		CaptureContainer(AttackChainContainer3, "AttackChain", AttackChainTurns3, results);
	}

	private void CaptureDeadlyFocus(List<ActorEffectInfoSnapshot> results)
	{
		CaptureContainer(DeadlyFocusRaiderContainer, "DeadlyFocus_Raider", null, results);
		CaptureContainer(DeadlyFocusSurvivorContainer, "DeadlyFocus_Survivor", null, results);
		CaptureContainer(DeadlyFocus_SkillIncreaseAttackContainer, "DeadlyFocus_EXDamage", null, results);
	}

	private void CaptureFlameTrigger(List<ActorEffectInfoSnapshot> results)
	{
		TryAddContainer(results, "FlameTrigger", FlameTriggerContainer, ParseLabelInt(FlameTriggerTurnsLabel));
	}

	private void CaptureIgniteBoost(List<ActorEffectInfoSnapshot> results)
	{
		if (IgniteBoostList != null)
		{
			for (int i = 0; i < IgniteBoostList.Count; i++)
			{
				IgniteBoostCard igniteBoostCard = IgniteBoostList[i];
				TryAddContainer(results, "IgniteBoost", igniteBoostCard?.container, ParseLabelInt(igniteBoostCard?.turnLabel));
			}
		}
	}

	private void CaptureUndying(List<ActorEffectInfoSnapshot> results)
	{
		TryAddContainer(results, "Undying", UndyingStateContainer, ParseLabelInt(UndyingStateTurnsLabel));
	}

	private void CaptureSecondaryMarked(List<ActorEffectInfoSnapshot> results)
	{
		if (!(SecondaryStatusIndicatorSprite == null) && SecondaryStatusIndicatorSprite.gameObject.activeSelf)
		{
			TryAdd(results, "Marked", SecondaryStatusIndicatorSprite, StatusIndicator, 0);
		}
	}

	private void CaptureContainer(GameObject container, string id, UILabel turnsLabel, List<ActorEffectInfoSnapshot> results)
	{
		if (container == null || !container.activeSelf)
		{
			return;
		}
		int num = ParseLabelInt(turnsLabel);
		if (num == 0)
		{
			Transform transform = container.transform.FindInChildren("turns");
			if (transform != null)
			{
				num = ParseLabelInt(transform.GetComponent<UILabel>());
			}
		}
		TryAddContainer(results, id, container, num);
	}

	private static void TryAddContainer(List<ActorEffectInfoSnapshot> results, string id, GameObject container, int turnCount)
	{
		if (!(container == null) && container.activeSelf)
		{
			TryAdd(results, id, HealthBarEffectIconCapture.CaptureContainer(container), turnCount);
		}
	}

	private static void TryAdd(List<ActorEffectInfoSnapshot> results, string id, UISprite sprite, GameObject searchRoot, int turnCount)
	{
		TryAdd(results, id, HealthBarEffectIconCapture.CaptureFromSprite(sprite, searchRoot), turnCount);
	}

	private static void TryAdd(List<ActorEffectInfoSnapshot> results, string id, UISprite sprite, int turnCount)
	{
		GameObject searchRoot = ((sprite != null && sprite.transform.parent != null) ? sprite.transform.parent.gameObject : null);
		TryAdd(results, id, HealthBarEffectIconCapture.CaptureFromSprite(sprite, searchRoot), turnCount);
	}

	private static void TryAdd(List<ActorEffectInfoSnapshot> results, string id, ActorEffectCapture capture, int turnCount)
	{
		if (capture.Icon.IsValid)
		{
			results.Add(new ActorEffectInfoSnapshot
			{
				Id = id,
				Icon = capture.Icon,
				Bg = capture.Bg,
				TurnCount = turnCount
			});
		}
	}

	private static int ParseLabelInt(UILabel label)
	{
		if (label == null || string.IsNullOrEmpty(label.text))
		{
			return 0;
		}
		int.TryParse(label.text, out var result);
		return result;
	}
}
