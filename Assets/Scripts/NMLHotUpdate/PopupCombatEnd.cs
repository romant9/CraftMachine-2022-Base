using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class PopupCombatEnd : HUDElement
{
	[Header("Main List")]
	[SerializeField]
	private CombatEndWidgetList widgetList;

	[Header("Confirm Button")]
	[SerializeField]
	private UIButton ConfirmButton;

	[Header("Delay between widgets")]
	[SerializeField]
	private float WidgetsDelayTime = 0.3f;

	[Header("Delay List Start")]
	[SerializeField]
	private float TweenStartDelay;

	[Header("Delay widget activation")]
	[SerializeField]
	private float StartActivatingWidgetsDelay;

	private float TweenDuration = 3f;

	[Header("List tween easing")]
	[SerializeField]
	private Easing.All TweeEasing = Easing.All.BackEaseOut;

	[Header("Widgets. The order in the list dictates the order in the list")]
	[SerializeField]
	private WidgetsMapping[] widgetsPrefabs;

	[Header("Guild War Mission Retry")]
	[SerializeField]
	private GameObject defaultButtonContainer;

	[SerializeField]
	private GameObject gvgButtonContainer;

	[SerializeField]
	private UIButton retryMissionButton;

	[SerializeField]
	private UILabel retryLabel;

	[SerializeField]
	private UILabel gvgRetryCostLabel;

	[SerializeField]
	private UILabel retryAmountLabel;

	[SerializeField]
	private GameObject retryUnavailableGameObject;

	[SerializeField]
	private HUDMeter gvgGasMeter;

	[SerializeField]
	private HUDMeter gvgAttacksMeter;

	[SerializeField]
	private Color retryUnavailableColor;

	private CombatModel combatModel;

	private Dictionary<CombatEndWidget.Types, int> OrderDict = new Dictionary<CombatEndWidget.Types, int>();

	private Tweener ListTweener;

	private List<UIListCard<string>> TempSortedWidgets;

	private const float ScrollTop = 0f;

	private const float ScrollCenter = 0.5f;

	private const float ScrollBottom = 1f;

	private bool SpeedUpDone;

	public Callback ConfirmedCallback { get; set; }

	public Callback RetryMissionCallback { get; set; }

	private void Awake()
	{
		DebugClassString = "PopupCombatEnd";
		if (ConfirmButton != null)
		{
			ConfirmButton.gameObject.SetActive(value: true);
		}
		SaveTheArrayPriority();
	}

	private void OnEnable()
	{
	}

	public override void Update()
	{
		base.Update();
		if (!SpeedUpDone && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
		{
			SpeedUpDone = true;
			if (ListTweener != null)
			{
				ListTweener.easeFromTo(ListTweener.progression, ListTweener.to, TweenDuration);
			}
		}
		if (ListTweener != null)
		{
			if (ListTweener.animating)
			{
				ListTweener.update();
				ScrollToPosition(ListTweener.progression.y);
			}
			else
			{
				ListTweener = null;
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (combatModel == null)
		{
			return;
		}
		MapMissionModel mapMissionModel = ((GameManager.Instance.playerModel.MapContainerModel != null) ? GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionModel : null);
		bool flag = mapMissionModel?.IsGrindMission ?? false;
		bool flag2 = mapMissionModel != null && (mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge);
		bool flag3 = mapMissionModel?.IsInApocalyptiWeeklyChallenge ?? false;
		bool isEndlessBattleMission = EndlessModeHelpers.IsEndlessBattleMission;
		bool flag4 = GuildWarHelper.IsGuildBattleMapMission();
		bool IsOngoing = IsLoadDataManager || GameManager.Instance.playerModel.GuildWarModel.CurrentBattle.IsOngoing(GameManager.Instance.playerModel.UtcTimeStamp + 5000);
		bool flag5 = flag4 && combatModel.MissionResult != ECombatResult.Successful && GameManager.Instance.playerModel.GuildBattlePlayer.IsCurrentGuildBattle() && IsOngoing;
		bool hasPvPRules = combatModel.HasPvPRules;
		bool flag6 = false;
		bool flag7 = false;
		int value = 0;
		if (GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity != null && GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.LastBattleRewards != null && GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition != null)
		{
			CurrencyType starCurrencyType = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.StarCurrencyType;
			flag6 = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.LastBattleRewards.TryGetValue(starCurrencyType, out value);
			flag7 = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.IsActive;
		}
		Helpers.GameObjectSetActive(defaultButtonContainer, !flag5);
		Helpers.GameObjectSetActive(gvgButtonContainer, flag5);
		bool flag8 = combatModel.StaticRewardSuppliesGranted > 0;
		bool flag9 = combatModel.StaticRewardSurvivalPointsGranted > 0;
		int	num = (GameManager.Instance.playerModel.AchievementManager != null || (IsLoadDataManager && !OfflineManager.IsUseServices)) ? GameManager.Instance.playerModel.AchievementManager.GetQuestChallengeBonusStars() : 0;
		int num2 = ((mapMissionModel != null && mapMissionModel.Stars != null && mapMissionModel.Stars.FeaturedHeroExtraChallengeStar) ? 1 : 0);
		int collectedSupplies = combatModel.MissionStatistics.CollectedSupplies;
		int bonusSp = combatModel.MissionStatistics.BonusSp;
		CombatEndFlowStatsWidget combatEndFlowStatsWidget = null;
		CombatEndFlowStatsWidget combatEndFlowStatsWidget2 = null;
		CombatEndFlowStatsWidget combatEndFlowStatsWidget3 = null;
		CombatEndFlowStatsWidget combatEndFlowStatsWidget4 = null;
		CombatEndFlowStatsWidget combatEndFlowStatsWidget5 = null;
		CombatEndFlowSurvivorWidget combatEndFlowSurvivorWidget = null;
		CombatEndFlowBonusStarWidget combatEndFlowBonusStarWidget = null;
		CombatEndOutpostWidget combatEndOutpostWidget = null;
		CombatEndFlowStatsWidget combatEndFlowStatsWidget6 = null;
		CreateTopBannerWidget();
		if (combatModel.MissionResult == ECombatResult.Successful)
		{
			if (flag2)
			{
				CreateWidget(CombatEndWidget.Types.Stars);
				if (num > 0 || num2 > 0)
				{
					combatEndFlowBonusStarWidget = CreateWidget(CombatEndWidget.Types.BonusStar) as CombatEndFlowBonusStarWidget;
				}
				if (flag3 && flag7 && flag6 && value > 0)
				{
					CreateWidget(CombatEndWidget.Types.WeeklyChallengeActivity);
				}
			}
			else if (flag)
			{
				if (flag9)
				{
					combatEndFlowStatsWidget4 = CreateWidget(CombatEndWidget.Types.MissionReward) as CombatEndFlowStatsWidget;
					combatEndFlowStatsWidget4.SetInfo(LocalizationManager.GetText("Popup.Victory.StaticRewards"));
				}
				if (flag8)
				{
					combatEndFlowStatsWidget5 = CreateWidget(CombatEndWidget.Types.MissionReward) as CombatEndFlowStatsWidget;
					combatEndFlowStatsWidget5.SetInfo(LocalizationManager.GetText("Popup.Victory.StaticRewards"));
				}
			}
			else if (flag4)
			{
				GuildBattleModel currentBattle = GuildWarHelper.GetCurrentBattle();
				bool isPvPCombat = GuildWarHelper.GetGuildWarPlayer().GuildBattleModel.AttackTargetMission.IsPvPCombat;
				GuildBattleMapMissionModel guildBattleMapMissionModel = GameManager.Instance.playerModel.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
				_ = ((SurvivorModel)combatModel.Survivors[0]).FeaturedDefinition;
				int personalGuildBattleMissionRewardPoints = currentBattle.GetPersonalGuildBattleMissionRewardPoints(guildBattleMapMissionModel.SectorIdOwner, isPvPCombat, guildBattleMapMissionModel.AreaIndex);
				CombatEndFlowStatsWidget obj = CreateWidget(CombatEndWidget.Types.GuildBattleRpReward) as CombatEndFlowStatsWidget;
				obj.SetInfo(HelpersLocalization.GetCurrencyName(CurrencyType.GuildBattleRP));
				obj.SetCurrencyData(CurrencyType.GuildBattleRP, personalGuildBattleMissionRewardPoints.ToString());
				int num3 = 0;
				string text = LocalizationManager.GetText("Popup.Victory.GuildBattle.VpReward");
				if (GuildWarHelper.GetGuildWarPlayer().GuildBattleModel.IsCurrentGuildBattle() && !GuildWarHelper.GetCurrentBattle().HasEnded() && !GuildWarHelper.IsBattleReadyToEnd())
				{
					num3 = currentBattle.GetGuildBattleMissionVictoryPoints(guildBattleMapMissionModel.SectorIdOwner, isPvPCombat, guildBattleMapMissionModel.AreaIndex);
					if (combatModel.RetryMission && !HelpersModel.IsUnlockAllSectors)
					{
						DebugTWD.LogMycode("if (combatModel.RetryMission && !IsLoadDataManager)");
						DebugTWD.Log("Ignore RetryMissionPenalty!", DebugType.Wars);
						int num4 = (int)FixedPoint.Round(num3 * (GameManager.Instance.gameEconomyData.GuildWarConfig.RetryMissionPenalty + 0.0001));
						num3 -= num4;
					}
					if (guildBattleMapMissionModel.Type == GuildBattleMapMissionModel.MissionType.PVP && !isPvPCombat)
					{
						(CreateWidget(CombatEndWidget.Types.GuildBattlePvPEnemyFound) as CombatEndFlowStatsWidget).SetInfo(LocalizationManager.GetText("Popup.GuildBattle.PvPEnemyFound"));
					}
				}
				else
				{
					text = LocalizationManager.GetText("Popup.GuildBattle.BattleEnded");
				}
				CombatEndFlowStatsWidget obj2 = CreateWidget(CombatEndWidget.Types.GuildBattleVpReward) as CombatEndFlowStatsWidget;
				obj2.SetInfo(text);
				obj2.SetSecondAmount(num3.ToString());
				obj2.SetCurrencyIcon("Ui_Icon_Resource_Vp");
			}
			else
			{
				if (combatModel.SeasonRewardMissionAmount > 0)
				{
					CombatEndFlowStatsWidget obj3 = CreateWidget(CombatEndWidget.Types.MissionReward) as CombatEndFlowStatsWidget;
					obj3.SetInfo(LocalizationManager.GetText("Popup.Victory.SeasonRewards"));
					obj3.SetCurrencyData(combatModel.SeasonRewardMissionCurrency, combatModel.SeasonRewardMissionAmount.ToString());
				}
				if (combatModel.StaticRewardStoryMissionCurrencyList != null && combatModel.StaticRewardStoryMissionCurrencyList.Count > 0)
				{
					for (int i = 0; i < combatModel.StaticRewardStoryMissionCurrencyList.Count; i++)
					{
						RewardCurrency rewardCurrency = combatModel.StaticRewardStoryMissionCurrencyList[i];
						CombatEndFlowStatsWidget obj4 = CreateWidget(CombatEndWidget.Types.MissionReward) as CombatEndFlowStatsWidget;
						obj4.SetInfo(LocalizationManager.GetText("Popup.Victory.StaticRewards"));
						obj4.SetCurrencyData(rewardCurrency.CurrencyType, rewardCurrency.Amount.ToString());
					}
				}
				if (combatModel.StaticRewardStoryMissionEquipment != null && !IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (combatModel.StaticRewardStoryMissionEquipment != null && !IsLoadDataManager)");
					DebugTWD.Log("Ignore StaticRewardStoryMissionEquipment!", DebugType.Wars);
					if (combatModel.StaticRewardStoryMissionEquipment.IsConsumable)
					{
						if (mapMissionModel?.GetStoryMissionRewards() != null)
						{
							foreach (RewardEquipment item in (from t in mapMissionModel.GetStoryMissionRewards().RewardsList.OfType<RewardEquipment>()
								where t.IsConsumableReward(GameManager.Instance.modelManager)
								select t).ToList())
							{
								(CreateWidget(CombatEndWidget.Types.ConsumableReward) as CombatEndFlowConsumableWidget).SetupConsumableReward(item);
							}
						}
					}
					else
					{
						(CreateWidget(CombatEndWidget.Types.Equipment) as CombatEndFlowEquipmentWidget).SetEquipment(combatModel.StaticRewardStoryMissionEquipment);
					}
				}
			}
			for (int num5 = 0; num5 < combatModel.ExtraSurvivors.Count; num5++)
			{
				if (combatModel.ExtraSurvivors[num5] is SurvivorModel { IsDead: false, IsNotGivenToPlayer: false })
				{
					CombatEndFlowStatsWidget combatEndFlowStatsWidget7 = CreateWidget(CombatEndWidget.Types.TextMessage) as CombatEndFlowStatsWidget;
					if (combatEndFlowStatsWidget7 != null)
					{
						combatEndFlowStatsWidget7.SetInfo(LocalizationManager.GetText("Popup.CombatEndScreen.RescuedSurvivor{Param}", combatModel.ExtraSurvivors[num5].Name));
					}
				}
			}
		}
		else if (combatModel.MissionResult == ECombatResult.Failed)
		{
			if (flag2)
			{
				CreateWidget(CombatEndWidget.Types.Stars);
			}
			else if (!flag && isEndlessBattleMission)
			{
				CreateEndlessModeMissionScoreWidget();
			}
		}
		else if (combatModel.MissionResult == ECombatResult.Flee && isEndlessBattleMission)
		{
			CreateEndlessModeMissionScoreWidget();
		}
		if (flag5)
		{
			GuildWarHelper.GetCurrentBattle();
			GuildBattleModelPlayer guildBattleModel = GuildWarHelper.GetGuildWarPlayer().GuildBattleModel;
			_ = guildBattleModel.AttackTargetMission.IsPvPCombat;
			GuildBattleMapMissionModel obj5 = GameManager.Instance.playerModel.GetAttackTargetMissionModel() as GuildBattleMapMissionModel;
			gvgGasMeter.SetCurrencyType(CurrencyType.GvGGas);
			gvgGasMeter.SetValue(GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.GvGGas));
			gvgAttacksMeter.SetCurrencyType(CurrencyType.GvGMissionKey);
			gvgAttacksMeter.SetValue(GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.GvGMissionKey));
			Cashier retryGvGMissionCashier = obj5.GetRetryGvGMissionCashier(GameManager.Instance.modelManager);
            bool flag10 = retryGvGMissionCashier.CanAfford() && guildBattleModel.CanRetryMission();
            int num6 = GameManager.Instance.gameEconomyData.GuildWarConfig.MaxAmountOfRetries - guildBattleModel.CurrentMissionRetriedAttempts;
            if (!IsLoadDataManager)
			{
				flag10 = retryGvGMissionCashier.CanAfford() && guildBattleModel.CanRetryMission();
				if (!flag10)
				{
					HelpersUI.SetColor(retryLabel, retryUnavailableColor);
					if (!retryGvGMissionCashier.CanAfford())
					{
						HelpersUI.SetColor(gvgRetryCostLabel, retryUnavailableColor);
					}
					if (num6 == 0)
					{
						HelpersUI.SetColor(retryAmountLabel, retryUnavailableColor);
					}
				}
			}
			else
			{
                DebugTWD.LogMycode("if (!IsLoadDataManager)");
            }

            Helpers.GameObjectSetActive(retryUnavailableGameObject, !flag10 && num6 > 0);
			HelpersUI.SetButtonState(retryMissionButton, (!flag10) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal);
			HelpersUI.SetContentToLabel(gvgRetryCostLabel, HelpersModel.IsUnlockAllSectors ? "0" :  retryGvGMissionCashier.GetTotalCost(CurrencyType.GvGGas).ToString());
			HelpersUI.SetContentToLabel(retryAmountLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.Defeat.RetriesLeft{Amount}", num6));
		}
		if (flag4 && combatModel.MissionResult != ECombatResult.Successful)
		{
			Helpers.ExecuteCommand(new SendMetricCommand(SendMetricCommand.MetricType.GvGRetryScreenViewed));
		}
		if (hasPvPRules)
		{
			combatEndOutpostWidget = CreateWidget(CombatEndWidget.Types.OutpostObjectives) as CombatEndOutpostWidget;
			if (combatEndOutpostWidget != null)
			{
				combatEndOutpostWidget.SetData(combatModel);
			}
		}
		if (collectedSupplies > 0)
		{
			combatEndFlowStatsWidget2 = CreateWidget(CombatEndWidget.Types.LeaderTraitSupplyBonus) as CombatEndFlowStatsWidget;
			if (combatEndFlowStatsWidget2 != null)
			{
				combatEndFlowStatsWidget2.SetInfo(LocalizationManager.GetText("Popup.PopupCombatEnd.LeaderTraitBonus"));
				combatEndFlowStatsWidget2.SetCurrencyData(CurrencyType.Supplies, collectedSupplies.ToString());
			}
		}
		if (bonusSp > 0)
		{
			combatEndFlowStatsWidget3 = CreateWidget(CombatEndWidget.Types.LeaderTraitSupplyBonus) as CombatEndFlowStatsWidget;
			if (combatEndFlowStatsWidget3 != null)
			{
				combatEndFlowStatsWidget3.SetInfo(LocalizationManager.GetText("Popup.PopupCombatEnd.LeaderTraitBonus"));
				combatEndFlowStatsWidget3.SetCurrencyData(CurrencyType.SurvivalPoints, bonusSp.ToString());
			}
		}
		combatEndFlowStatsWidget = CreateWidget(CombatEndWidget.Types.WalkersDispatched) as CombatEndFlowStatsWidget;
		combatEndFlowSurvivorWidget = CreateWidget(CombatEndWidget.Types.TeamStatus) as CombatEndFlowSurvivorWidget;
		if (combatEndFlowStatsWidget != null && combatModel.MissionStatistics != null)
		{
			int num7 = combatModel.MissionStatistics.WalkersKilled + combatModel.MissionStatistics.RaidersKilled;
			combatEndFlowStatsWidget.SetInfo(LocalizationManager.GetText("Popup.Defeat.NumberEnemiesKilled"));
			combatEndFlowStatsWidget.SetFirstAmount(num7.ToString());
			combatEndFlowStatsWidget.SetCurrencyData(CurrencyType.SurvivalPoints, combatModel.MissionStatistics.CollectedSp.ToString());
			combatEndFlowStatsWidget.CreateCurrencyAnimation(CurrencyType.SurvivalPoints, combatModel.MissionStatistics.CollectedSp);
		}
		if (combatEndFlowStatsWidget4 != null)
		{
			combatEndFlowStatsWidget4.SetCurrencyData(CurrencyType.SurvivalPoints, combatModel.StaticRewardSurvivalPointsGranted.ToString());
		}
		if (combatEndFlowStatsWidget5 != null)
		{
			combatEndFlowStatsWidget5.SetCurrencyData(CurrencyType.Supplies, combatModel.StaticRewardSuppliesGranted.ToString());
		}
		if (combatEndFlowBonusStarWidget != null)
		{
			combatEndFlowBonusStarWidget.SetInfo(LocalizationManager.GetText("Popup.Victory.BonusStar"));
			combatEndFlowBonusStarWidget.SetCurrencyData((num + num2).ToString());
			combatEndFlowBonusStarWidget.SetStar(mapMissionModel);
		}
		if (combatEndFlowSurvivorWidget != null)
		{
			combatEndFlowSurvivorWidget.SetSurvivors(combatModel);
		}
		BattlePassModel battlePass = combatModel.manager.Player.BattlePass;
		if (battlePass != null && battlePass.IsSeasonActive)
		{
			combatEndFlowStatsWidget6 = CreateWidget(CombatEndWidget.Types.BCGained) as CombatEndFlowStatsWidget;
			if (combatEndFlowStatsWidget6 != null && combatModel.MissionStatistics != null)
			{
				int battlePassCurrencyEarned = combatModel.MissionStatistics.BattlePassCurrencyEarned;
				bool flag11 = battlePass.EarnedFromKillsThisCycle >= battlePass.MaxDailyBCFromKills && battlePassCurrencyEarned <= 0;
				combatEndFlowStatsWidget6.SetInfo(LocalizationManager.GetText("Popup.Combatend.BattleCurrency"));
				combatEndFlowStatsWidget6.SetCurrencyData(CurrencyType.BattlePassPoints, flag11 ? LocalizationManager.GetText("Popup.Combatend.BattleCurrency.DailyMax") : battlePassCurrencyEarned.ToString());
				combatEndFlowStatsWidget6.CreateCurrencyAnimation(CurrencyType.BattlePassPoints, battlePassCurrencyEarned);
			}
		}
		if (combatModel.MissionStatistics.HaveConsumablesBeenUsed())
		{
			TWDPlayerPrefs.SetInt("NewConsumablesAcquiredAmount", 0);
		}
		widgetList.Position();
		Invoke("StartActivatingWidgets", StartActivatingWidgetsDelay);
	}

	public override void OnBackButtonClicked()
	{
		OnConfirmClicked();
	}

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		if (model is CombatModel)
		{
			combatModel = model as CombatModel;
			UpdateUI();
		}
		else
		{
			DebugLogError("was expecting Model of type: CombatModel!");
		}
	}

	public void OnConfirmClicked()
	{
		if (ConfirmedCallback != null)
		{
			DebugLog("OnConfirmClicked");
			ConfirmedCallback();
			ConfirmedCallback = null;
			RetryMissionCallback = null;
			Close();
		}
		else
		{
			DebugLogError("OnConfirmClicked is NULL Can't continue with End Flow");
		}
	}

	public void OnRetryClicked()
	{
		if (RetryMissionCallback != null)
		{
			RetryMissionCallback();
			RetryMissionCallback = null;
			ConfirmedCallback = null;
		}
	}

	private void CreateTopBannerWidget()
	{
		if (combatModel == null)
		{
			return;
		}
		if (combatModel.OutOfTurns)
		{
			(CreateWidget(CombatEndWidget.Types.OutpostSpecialBanner) as CombatEndFlowStatsWidget).SetInfo(LocalizationManager.GetText("Popup.Defeat.OutOfTurns"));
		}
		else if (CombatView.Instance.ranOutOfTime)
		{
			(CreateWidget(CombatEndWidget.Types.OutpostSpecialBanner) as CombatEndFlowStatsWidget).SetInfo(LocalizationManager.GetText("Popup.Defeat.OutOfTime"));
		}
		else if (combatModel.IsEndlessBattleMission)
		{
			CombatEndWidget combatEndWidget = CreateWidget(CombatEndWidget.Types.EndlessModeBanner);
			if (combatEndWidget != null)
			{
				string text = LocalizationManager.GetText("Combat.EndFade.EndlessMode{0}", EndlessModeHelpers.OverAllWaveCount);
				combatEndWidget.SetContent(text);
			}
		}
		else if (combatModel.MissionResult == ECombatResult.Successful)
		{
			CreateWidget(CombatEndWidget.Types.VictoryBanner);
		}
		else if (combatModel.MissionResult == ECombatResult.Failed)
		{
			CreateWidget(CombatEndWidget.Types.DefeatBanner);
		}
		else if (combatModel.MissionResult == ECombatResult.Flee)
		{
			CreateWidget(CombatEndWidget.Types.FleeBanner);
		}
		else if (combatModel.MissionResult == ECombatResult.Draw)
		{
			CreateWidget(CombatEndWidget.Types.DrawBanner);
		}
	}

	private void StartTween()
	{
		if (ListTweener == null)
		{
			ListTweener = new Tweener();
			Vector4 zero = Vector4.zero;
			Vector4 one = Vector4.one;
			ListTweener.easeFromTo(zero, one, TweenDuration, TweenerHelpers.getGetByEnum(TweeEasing));
		}
	}

	private void onDragStarted()
	{
	}

	private void CreateEndlessModeMissionScoreWidget()
	{
		CombatEndFlowStatsWidget combatEndFlowStatsWidget = null;
		combatEndFlowStatsWidget = ((!EndlessModeHelpers.IsEndlessExpertMode()) ? (CreateWidget(CombatEndWidget.Types.EndlessModeMissionScore) as CombatEndFlowStatsWidget) : (CreateWidget(CombatEndWidget.Types.EndlessModeExpertModeMissionScore) as CombatEndFlowStatsWidget));
		if (combatEndFlowStatsWidget != null)
		{
			long currentAttemptScore = EndlessModeHelpers.GetCurrentAttemptScore();
			int currentAttemptRanking = EndlessModeHelpers.GetCurrentAttemptRanking();
			string formattedScoreText = EndlessModeHelpers.GetFormattedScoreText(currentAttemptScore);
			combatEndFlowStatsWidget.SetFirstAmount(currentAttemptRanking.ToString());
			combatEndFlowStatsWidget.SetSecondAmount(formattedScoreText);
			combatEndFlowStatsWidget.SetBestScoreContainer(currentAttemptRanking == 1);
		}
	}

	private void StartActivatingWidgets()
	{
		if (widgetList != null && widgetList.GetCards() != null)
		{
			TweenDuration = (float)widgetList.GetCards().Count * WidgetsDelayTime;
		}
		ScrollToPosition(0f);
		float y = widgetList.CalculateContentSize().y;
		float w = widgetList.ScrollView.panel.finalClipRegion.w;
		if (y > w)
		{
			Invoke("StartTween", TweenStartDelay);
		}
		StartCoroutine(ActivateWidgets());
	}

	private IEnumerator ActivateWidgets()
	{
		if (TempSortedWidgets == null && widgetList != null)
		{
			TempSortedWidgets = widgetList.GetCards();
		}
		if (TempSortedWidgets != null)
		{
			for (int i = TempSortedWidgets.Count - 1; i >= 0; i--)
			{
				if (TempSortedWidgets[i] != null && TempSortedWidgets[i] is CombatEndWidget)
				{
					(TempSortedWidgets[i] as CombatEndWidget).Activate();
					yield return new WaitForSeconds(WidgetsDelayTime);
				}
			}
		}
		if (!OfflineManager.IsTutorialDisable && TutorialView.Instance != null)
		{
			TutorialView.Instance.StartPart("VictoryScreen");
		}
		TempSortedWidgets = null;
	}

	private CombatEndWidget CreateWidget(CombatEndWidget.Types type, int sortOrder = 0)
	{
		GameObject gameObject = TryGetWidgetPrefabOfType(type);
		CombatEndWidget combatEndWidget = null;
		if (gameObject != null && widgetList != null)
		{
			combatEndWidget = widgetList.InstantiateItemToList(gameObject) as CombatEndWidget;
			if (combatEndWidget != null)
			{
				combatEndWidget.Deactivate();
				combatEndWidget.CurrentType = type;
				combatEndWidget.SetSortValue(TryGetSortValue(type) + sortOrder);
			}
		}
		return combatEndWidget;
	}

	private void SaveTheArrayPriority()
	{
		int num = 10;
		if (!(widgetList != null))
		{
			return;
		}
		for (int i = 0; i < widgetsPrefabs.Length; i++)
		{
			if (widgetsPrefabs[i] != null)
			{
				OrderDict[widgetsPrefabs[i].type] = i * num;
			}
		}
	}

	private int TryGetSortValue(CombatEndWidget.Types type)
	{
		int value = -1;
		if (OrderDict != null && OrderDict.TryGetValue(type, out value))
		{
			DebugLog("Sort depth for type: " + type.ToString() + " order: " + value);
			return value;
		}
		DebugLogWarning("Could not find order for type: " + type);
		return value;
	}

	private GameObject TryGetWidgetPrefabOfType(CombatEndWidget.Types type)
	{
		for (int i = 0; i < widgetsPrefabs.Length; i++)
		{
			if (widgetsPrefabs[i] != null && widgetsPrefabs[i].type == type && widgetsPrefabs[i].widget != null)
			{
				return widgetsPrefabs[i].widget.gameObject;
			}
		}
		DebugLogWarning("Could not find widget prefab of type: " + type);
		return null;
	}

	private void ScrollToPosition(float value)
	{
		if (widgetList != null)
		{
			widgetList.SetDragAmount(0f, value, updateScrollbars: false);
		}
	}

	public override void Close()
	{
		Helpers.GameObjectSetActive(defaultButtonContainer, value: false);
		Helpers.GameObjectSetActive(gvgButtonContainer, value: false);
		base.Close();
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion
}
