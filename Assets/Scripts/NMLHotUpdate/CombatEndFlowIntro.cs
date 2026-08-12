using System.Collections;
using TWDModel;
using UnityEngine;

public class CombatEndFlowIntro : CombatEndFlowStep
{
	[SerializeField]
	private GameObject[] starsContainer;

	[SerializeField]
	private GameObject starPrefab;

	[SerializeField]
	private GameObject staticRewardsContainer;

	[SerializeField]
	private UILabel staticRewardsLabel;

	[SerializeField]
	private UISprite staticRewardsIcon;

	[SerializeField]
	private GameObject questBonusContainer;

	[SerializeField]
	private UILabel questBonusAmountLabel;

	[SerializeField]
	private EffectSparkle questBonusSparkleEffect;

	[SerializeField]
	private float timeBetweenRewardLineApparition = 1f;

	[SerializeField]
	private GameObject[] outpostDefenderCards;

	[SerializeField]
	private UISprite outpostDefendersKilledRewardIcon;

	[SerializeField]
	private UILabel outpostDefendersKilledRewardLabel;

	[SerializeField]
	private UILabel outpostDefendersKilledCountLabel;

	[SerializeField]
	private GameObject outpostFirstObjectiveCompletedObject;

	[SerializeField]
	private UISprite[] outpostFirstObjectiveCompletedSprites;

	[SerializeField]
	private GameObject outpostFirstObjectiveFailedObject;

	[SerializeField]
	private UISprite[] outpostFirstObjectiveFailedSprites;

	[SerializeField]
	private GameObject outpostSecondObjectiveCompletedObject;

	[SerializeField]
	private UISprite[] outpostSecondObjectiveCompletedSprites;

	[SerializeField]
	private GameObject outpostSecondObjectiveFailedObject;

	[SerializeField]
	private UISprite[] outpostSecondObjectiveFailedSprites;

	[SerializeField]
	private GameObject outpostDefendersKilledCompletedObject;

	[SerializeField]
	private GameObject outpostDefendersKilledFailedObject;

	public string GoreAnimationStateName = "Victory_Animation";

	public string NoGoreAnimationStateName = "Victory_Animation_No_Gore";

	private CombatEndFlowStar[] stars;

	private int currentStarAnimated;

	private int[] starIds;

	private MapMissionStars mapMissionStars;

	private CampHUD hud;

	private int finalSp;

	private int finalSupplies;

	private bool isOutpostCombat;

	public CombatEndFlowIntro()
	{
		DestroyAfterCompletion = false;
	}

	public override void StartFlow()
	{
		base.StartFlow();
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			GameObject gameObject = base.gameObject.FindInChildren("Animation");
			if (gameObject != null)
			{
				Animator component = gameObject.GetComponent<Animator>();
				if (component != null)
				{
					component.Play(GameManager.Instance.IsGoreDisabled ? NoGoreAnimationStateName : GoreAnimationStateName);
				}
			}
		}
		EventManager.OnEvent += OnEvent;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		finalSp = GameManager.Instance.playerModel.GetCurrency(CurrencyType.SurvivalPoints).Value;
		GameManager.Instance.playerModel.GetCurrency(CurrencyType.SurvivalPoints).SetValue(combat.SurvivalPointsAtStart);
		if (combat.StaticRewardSuppliesGranted > 0)
		{
			CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(CurrencyType.Supplies);
			finalSupplies = currency.Value;
			currency.SetValue(currency.Value - combat.StaticRewardSuppliesGranted);
		}
		hud = CampHUD.OpenHudPostCombat();
		isOutpostCombat = combat.HasPvPRules;
		if (isOutpostCombat)
		{
			statisticLines = new EndScreenStatisticLine[3];
			StartCoroutine(ShowOutpostStaticLine());
		}
		else
		{
			statisticLines = new EndScreenStatisticLine[1];
			CreateEndScreenLine(0, OnSinglePlayerCombatAnimationFinished, LocalizationManager.GetText("Popup.Defeat.NumberWalkersKilled"), combat.MissionStatistics.WalkersKilled.ToString(), CurrencyType.SurvivalPoints, combat.MissionStatistics.CollectedSp);
		}
		if (staticRewardsLabel != null)
		{
			int num = combat.StaticRewardSuppliesGranted + combat.StaticRewardSurvivalPointsGranted;
			staticRewardsLabel.text = num.ToString();
		}
		if (staticRewardsIcon != null)
		{
			if (combat.StaticRewardSuppliesGranted > 0)
			{
				staticRewardsIcon.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Supplies);
			}
			else if (combat.StaticRewardSurvivalPointsGranted > 0)
			{
				staticRewardsIcon.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.SurvivalPoints);
			}
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("combat_ui/timer_warning");
			SingularityMonoBehaviour<AudioManager>.Instance.RequestMusicStateChange(MusicState.Victory);
		}
	}

	private IEnumerator ShowOutpostStaticLine()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		CombatModel combatModel = playerModel.Combat;
		ConfigData configData = GameManager.Instance.gameEconomyData.ConfigData;
		int lineIndex = 0;
		int rankingScoreChange = 0;
		int tradeGoodsStolen = 0;
		int resourceScore = 0;
		int influenceScore = 0;
		int secondObjectiveResourceScore = 0;
		int secondObjectiveInfluenceeScore = 0;
		if (playerModel.Combat != null && playerModel.Combat.OutpostCombat != null)
		{
			tradeGoodsStolen = playerModel.GetFinalResourcesStolen(playerModel.Combat.OutpostCombat.TradeGoodsGain);
			rankingScoreChange = playerModel.GetFinalRankingScoreChange(playerModel.Combat.OutpostCombat.AttackerInfluenceGain);
		}
		if (combatModel.PvPMissionType == PvPMissionType.PVPMultiLoot || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiLoot)
		{
			influenceScore = (combatModel.IsPvPLootCollected ? (rankingScoreChange / 2) : 0);
			resourceScore = (combatModel.IsPvPLootCollected ? (tradeGoodsStolen / 2) : 0);
			secondObjectiveInfluenceeScore = (combatModel.IsPvPFlagCollected ? (rankingScoreChange / 2) : 0);
			secondObjectiveResourceScore = (combatModel.IsPvPFlagCollected ? (tradeGoodsStolen / 2) : 0);
		}
		else if (combatModel.PvPMissionType == PvPMissionType.PVPMultiFlag || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiFlag)
		{
			influenceScore = (combatModel.IsPvPFlagCollected ? (rankingScoreChange / 2) : 0);
			resourceScore = (combatModel.IsPvPFlagCollected ? (tradeGoodsStolen / 2) : 0);
			secondObjectiveInfluenceeScore = (combatModel.IsPvPLootCollected ? (rankingScoreChange / 2) : 0);
			secondObjectiveResourceScore = (combatModel.IsPvPLootCollected ? (tradeGoodsStolen / 2) : 0);
		}
		ShowFirstOutpostObjective(combatModel, lineIndex, resourceScore, influenceScore);
		lineIndex++;
		yield return new WaitForSeconds(timeBetweenRewardLineApparition);
		ShowSecondOutpostObjective(combatModel, lineIndex, secondObjectiveResourceScore, secondObjectiveInfluenceeScore);
		lineIndex++;
		yield return new WaitForSeconds(timeBetweenRewardLineApparition);
		outpostDefendersKilledCompletedObject.SetActive(combatModel.IsPvpDefendersKilled);
		outpostDefendersKilledFailedObject.SetActive(!combatModel.IsPvpDefendersKilled);
		int medalAmount = (combatModel.IsPvpDefendersKilled ? (rankingScoreChange * configData.OutpostDefendersCompletedInfluencePercentage / 100) : 0);
		int mainCurrencyAmount = (combatModel.IsPvpDefendersKilled ? (tradeGoodsStolen * configData.OutpostDefendersCompletedResourcePercentage / 100) : 0);
		CreateOutpostEndScreenLine(lineIndex, OnOutpostCombatAnimationFinished, LocalizationManager.GetText("Popup.Victory.DefendersKilled"), null, CurrencyType.Outpost, mainCurrencyAmount, medalAmount);
	}

	private void CreateEndScreenLine(int positionIndex, Callback callback, string statText, string moreInfoText, CurrencyType mainCurrency, int mainCurrencyAmount, int medalAmount = int.MaxValue)
	{
		CreateEndScreenLine(positionIndex, callback);
		statisticLines[positionIndex].Setup(statText, moreInfoText, mainCurrency, mainCurrencyAmount, medalAmount);
	}

	private void CreateOutpostEndScreenLine(int positionIndex, Callback callback, string statText, string moreInfoText, CurrencyType mainCurrency, int mainCurrencyAmount, int medalAmount = int.MaxValue)
	{
		CreateOutpostEndScreenLine(positionIndex, callback);
		statisticLines[positionIndex].Setup(statText, moreInfoText, mainCurrency, mainCurrencyAmount, medalAmount);
	}

	public override void ForceFlowEnd()
	{
		base.ForceFlowEnd();
		EventManager.OnEvent -= OnEvent;
		Close();
	}

	public override void OnOpenAnimationOver()
	{
		base.OnOpenAnimationOver();
		InstantiateStars();
		if (stars != null)
		{
			currentStarAnimated = 0;
			ShowStar();
		}
	}

	private void ShowFirstOutpostObjective(CombatModel combatModel, int lineIndex, int resourceScore, int influenceScore)
	{
		string textId = "";
		string spriteName = "";
		bool flag = false;
		if (combatModel.PvPMissionType == PvPMissionType.PVPMultiFlag || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiFlag)
		{
			flag = combatModel.IsPvPFlagCollected;
			textId = "Popup.Victory.TakenFlags";
			spriteName = "Ui_Icon_Flag";
		}
		else if (combatModel.PvPMissionType == PvPMissionType.PVPMultiLoot || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiLoot)
		{
			flag = combatModel.IsPvPLootCollected;
			textId = "Popup.Victory.TakenTreasures";
			spriteName = "Ui_Icon_Crate";
		}
		if (flag)
		{
			for (int i = 0; i < outpostFirstObjectiveCompletedSprites.Length; i++)
			{
				outpostFirstObjectiveCompletedSprites[i].spriteName = spriteName;
			}
		}
		else
		{
			for (int j = 0; j < outpostFirstObjectiveFailedSprites.Length; j++)
			{
				outpostFirstObjectiveFailedSprites[j].spriteName = spriteName;
			}
		}
		CreateOutpostEndScreenLine(lineIndex, null, LocalizationManager.GetText(textId), null, CurrencyType.Outpost, resourceScore, influenceScore);
		outpostFirstObjectiveCompletedObject.SetActive(flag);
		outpostFirstObjectiveFailedObject.SetActive(!flag);
	}

	private void ShowSecondOutpostObjective(CombatModel combatModel, int lineIndex, int resourceScore, int influenceScore)
	{
		string textId = "";
		string spriteName = "";
		bool flag = false;
		if (combatModel.PvPMissionType == PvPMissionType.PVPMultiFlag || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiFlag)
		{
			flag = combatModel.IsPvPLootCollected;
			textId = "Popup.Victory.TakenTreasure";
			spriteName = "Ui_Icon_Crate";
		}
		if (combatModel.PvPMissionType == PvPMissionType.PVPMultiLoot || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiLoot)
		{
			flag = combatModel.IsPvPFlagCollected;
			textId = "Popup.Victory.TakenFlag";
			spriteName = "Ui_Icon_Flag";
		}
		if (flag)
		{
			for (int i = 0; i < outpostSecondObjectiveCompletedSprites.Length; i++)
			{
				outpostSecondObjectiveCompletedSprites[i].spriteName = spriteName;
			}
		}
		else
		{
			for (int j = 0; j < outpostSecondObjectiveFailedSprites.Length; j++)
			{
				outpostSecondObjectiveFailedSprites[j].spriteName = spriteName;
			}
		}
		CreateOutpostEndScreenLine(lineIndex, null, LocalizationManager.GetText(textId), null, CurrencyType.Outpost, resourceScore, influenceScore);
		outpostSecondObjectiveCompletedObject.SetActive(flag);
		outpostSecondObjectiveFailedObject.SetActive(!flag);
	}

	private void InstantiateStars()
	{
		MapMissionModel attackTargetMissionModel = GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionModel;
		mapMissionStars = attackTargetMissionModel?.Stars;
		if (mapMissionStars == null || (!attackTargetMissionModel.IsInWeeklyChallenge && !attackTargetMissionModel.IsInApocalyptiWeeklyChallenge))
		{
			return;
		}
		stars = new CombatEndFlowStar[3];
		for (int i = 0; i < 3; i++)
		{
			stars[i] = Helpers.InstantiateToParent(starPrefab, starsContainer[i]).GetComponent<CombatEndFlowStar>();
		}
		int num = 0;
		starIds = new int[3];
		for (int j = 0; j < 3; j++)
		{
			if (mapMissionStars.Stars[j])
			{
				starIds[num] = j;
				num++;
			}
		}
		for (int k = 0; k < 3; k++)
		{
			if (!mapMissionStars.Stars[k])
			{
				starIds[num] = k;
				num++;
			}
		}
	}

	private void ShowStar()
	{
		int num = starIds[currentStarAnimated];
		bool flag = mapMissionStars.Stars[num];
		stars[currentStarAnimated].SetStar(num, flag);
		TweenManager.PlayTweenGroup(stars[currentStarAnimated].gameObject, flag ? 20 : 21);
		Invoke("OnStarShown", 0.5f);
		int num2 = ((GameManager.Instance.playerModel.AchievementManager != null) ? GameManager.Instance.playerModel.AchievementManager.GetQuestChallengeBonusStars() : 0);
		if (num2 > 0)
		{
			if (!questBonusContainer.activeSelf)
			{
				questBonusContainer.SetActive(value: true);
				questBonusAmountLabel.text = num2.ToString();
				questBonusSparkleEffect.enabled = true;
			}
		}
		else
		{
			questBonusContainer.SetActive(value: false);
		}
		if (flag)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("combat_ui/star_show");
		}
	}

	private void OnStarShown()
	{
		currentStarAnimated++;
		if (currentStarAnimated < 3)
		{
			ShowStar();
		}
		else
		{
			AnimationEnded();
		}
	}

	private void OnSinglePlayerCombatAnimationFinished()
	{
		if (hud != null)
		{
			GameManager.Instance.playerModel.GetCurrency(CurrencyType.SurvivalPoints).SetValue(finalSp);
		}
		if (staticRewardsContainer != null)
		{
			CombatModel combat = GameManager.Instance.playerModel.Combat;
			int num = combat.StaticRewardSuppliesGranted + combat.StaticRewardSurvivalPointsGranted;
			staticRewardsContainer.SetActive(num > 0);
			if (hud != null && num > 0)
			{
				if (combat.StaticRewardSuppliesGranted > 0)
				{
					hud.GetComponent<BuildingsHUD>().CreateCollectAnim(CurrencyType.Supplies, staticRewardsIcon.gameObject, combat.StaticRewardSuppliesGranted);
					GameManager.Instance.playerModel.GetCurrency(CurrencyType.Supplies).SetValue(finalSupplies);
				}
				else if (combat.StaticRewardSurvivalPointsGranted > 0)
				{
					hud.GetComponent<BuildingsHUD>().CreateCollectAnim(CurrencyType.SurvivalPoints, staticRewardsIcon.gameObject, combat.StaticRewardSurvivalPointsGranted);
				}
			}
		}
		if (!TutorialView.Instance.StartPart("VictoryScreen"))
		{
			AnimationEnded();
		}
	}

	private void OnOutpostCombatAnimationFinished()
	{
		AnimationEnded();
	}

	public void OnStatisticLineAnimationFinished()
	{
		if (false)
		{
			AnimationEnded();
			return;
		}
		if (hud != null)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			CombatModel combat = playerModel.Combat;
			if (combat.MissionStatistics.CollectedSp > 0)
			{
				hud.GetComponent<BuildingsHUD>().CreateCollectAnim(CurrencyType.SurvivalPoints, statisticLines[0].MainCurrencyIcon.gameObject, combat.MissionStatistics.CollectedSp);
			}
			playerModel.GetCurrency(CurrencyType.SurvivalPoints).SetValue(finalSp);
		}
		if (staticRewardsContainer != null)
		{
			CombatModel combat2 = GameManager.Instance.playerModel.Combat;
			int num = combat2.StaticRewardSuppliesGranted + combat2.StaticRewardSurvivalPointsGranted;
			staticRewardsContainer.SetActive(num > 0);
		}
		if (!TutorialView.Instance.StartPart("VictoryScreen"))
		{
			AnimationEnded();
		}
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		if (eventtype == EventManager.EventType.TutorialPartOver)
		{
			AnimationEnded();
		}
	}

	public override void Update()
	{
		base.Update();
		if (animationEnded && Input.GetMouseButtonUp(0) && !TutorialView.Instance.Running)
		{
			ForceFlowEnd();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("combat_ui/combat_victory");
		}
	}
}
