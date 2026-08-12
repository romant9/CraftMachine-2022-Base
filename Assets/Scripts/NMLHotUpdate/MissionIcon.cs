using System.Collections;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class MissionIcon : MonoBehaviour
{
	public enum GrindAnimStates
	{
		Available = 0,
		Completed = 1,
		New = 2,
		Replaced = 3
	}

	[SerializeField]
	[Tooltip("Background sprite")]
	private UISprite backgroundSprite;

	[SerializeField]
	[Tooltip("Background sprite")]
	private UISprite backgroundSprite2;

	[SerializeField]
	[Tooltip("Deadly mission indicator")]
	private UISprite deadlyMissionIndicator;

	[SerializeField]
	[Tooltip("Hard difficulty indicator (used in survival mode).")]
	private GameObject hardDifficultyIndicator;

	[Tooltip("Nightmare difficulty indicator (used in survival mode).")]
	[SerializeField]
	private GameObject nightmareDifficultyIndicator;

	[Tooltip("Glow")]
	[SerializeField]
	private GameObject ActiveGlow;

	[SerializeField]
	[Tooltip("Intro Glow")]
	private GameObject introGlow;

	[SerializeField]
	private TutorialArrowParent tutorialArrow;

	[SerializeField]
	[Tooltip("The sprite that is shown on top of the background.")]
	private UISprite iconSprite;

	[Tooltip("The UITweener component which is used to animate the icon on interaction.")]
	[SerializeField]
	private UITweener missionTweener;

	[Tooltip("The UITweener component which is used to destroy the icon.")]
	[SerializeField]
	private UITweener missionDestroyTweener;

	[SerializeField]
	private UITexture finalMissionTexture;

	[SerializeField]
	private UILabel completionTimes;

	[SerializeField]
	private UISprite iconCurrency;

	[SerializeField]
	private UISprite iconCurrency1_2;

	[SerializeField]
	private UISprite iconCurrency2_2;

	[Header("Grind")]
	[SerializeField]
	private GameObject grindContainer;

	[Header("Challenge")]
	[SerializeField]
	private GameObject challengeContainer;

	[SerializeField]
	private UILabel challengeStarAmountLabel;

	[SerializeField]
	private TweenAlpha skipTokenBonusStars;

	[SerializeField]
	private UILabel masterMissionDifficultyLabel;

	[Header("Timer")]
	[SerializeField]
	private GameObject timerContainer;

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	[Header("Cost")]
	private GameObject CostParent;

	[SerializeField]
	private UISprite CostIcon;

	[SerializeField]
	private UISprite CostBg;

	[SerializeField]
	private UILabel CostLabel;

	[SerializeField]
	[Header("Path")]
	private UISprite pathNormal;

	[SerializeField]
	private UISprite pathLocked;

	[SerializeField]
	private GameObject pathContainer;

	[SerializeField]
	[Header("Survival")]
	private UIGrid survivalRarityStarGrid;

	[SerializeField]
	private GameObject[] survivalRarityStars;

	[SerializeField]
	private GameObject survivalDoubleRewardsIndicator;

	[SerializeField]
	[Header("Season")]
	private Color seasonLockedColor;

	[SerializeField]
	private Color seasonAvailableColor;

	[SerializeField]
	private Color seasonCompletedColor;

	[SerializeField]
	private GameObject seasonLock;

	[SerializeField]
	private GameObject collectAnimationPrefab;

	[SerializeField]
	private GameObject rewardOrigin;

	[SerializeField]
	private GameObject bloodSplatter;

	[SerializeField]
	private string trialCompletedSpriteName;

	[SerializeField]
	private UILabel trialResetLabel;

	[SerializeField]
	private GameObject[] challengeStars;

	[SerializeField]
	private GameObject challengeCheckMark;

	[SerializeField]
	private UIAtlas mapIconAtlas;

	[SerializeField]
	private UIAtlas currenciesAtlas;

	private bool fadeAndDestroyOnEnable;

	public static int ChallengeStarsTweenGroup = 20;

	public static int ChallengeDifficultyChangedTweenGroup = 5;

	private int nextStarToAnimateIndex = -1;

	private int lastStarToAnimateIndex = -1;

	private UIWidget iconWidget;

	private Animator animator;

	private GrindAnimStates animState;

	private MapMissionModel missionPlayedModel;

	private MapVisualData mapVisualData;

	private MapMissionModel targetMissionModel;

	private MissionHighlight cachedNextHighlight;

	private string trialsLocalisationString = "";

	private long trialsResetTimeLeft;

	private bool isWaitingPopupsToAnimate;

	private bool needToAnimateCheckMark;

	public static bool needsToShowCycleEndPopup = false;

	private static readonly HashSet<CurrencyType> hasTokens = new HashSet<CurrencyType>
	{
		CurrencyType.CarolToken,
		CurrencyType.RickToken,
		CurrencyType.AbrahamToken,
		CurrencyType.NeganToken,
		CurrencyType.MichonneToken,
		CurrencyType.MorganToken,
		CurrencyType.MaggieToken,
		CurrencyType.JesusToken,
		CurrencyType.GlennToken,
		CurrencyType.DarylToken,
		CurrencyType.AssaultToken,
		CurrencyType.ScoutToken,
		CurrencyType.BruiserToken,
		CurrencyType.WarriorToken,
		CurrencyType.ShooterToken,
		CurrencyType.HunterToken,
		CurrencyType.CarlToken,
		CurrencyType.TaraToken,
		CurrencyType.RositaToken,
		CurrencyType.TalkingDeadToken,
		CurrencyType.EugeneToken,
		CurrencyType.AaronToken,
		CurrencyType.GabrielToken,
		CurrencyType.EzekielToken,
		CurrencyType.DwightToken,
		CurrencyType.SashaToken,
		CurrencyType.MerleToken,
		CurrencyType.GovernorToken,
		CurrencyType.JerryToken,
		CurrencyType.ScoutRickToken,
		CurrencyType.BruiserGlennToken,
		CurrencyType.HunterMorganToken,
		CurrencyType.ScoutDarylToken,
		CurrencyType.ShooterMaggieToken,
		CurrencyType.AlphaToken,
		CurrencyType.BetaToken,
		CurrencyType.TDogToken,
		CurrencyType.ShaneToken,
		CurrencyType.PrincessToken,
		CurrencyType.YumikoToken,
		CurrencyType.BethToken,
		CurrencyType.ShivaToken,
		CurrencyType.DogToken,
		CurrencyType.WhisperersMaskToken,
		CurrencyType.MercerToken,
		CurrencyType.CommonwealthArmorToken,
		CurrencyType.RainbowCatToken,
		CurrencyType.AssassinCarolToken,
		CurrencyType.HwachaToken,
		CurrencyType.CarolsCookiesToken,
		CurrencyType.WalkerMikeToken,
		CurrencyType.HunterHershelToken,
		CurrencyType.TyreeseToken,
		CurrencyType.BruiserRositaToken,
		CurrencyType.ConnieToken,
		CurrencyType.CowboyNeganToken,
		CurrencyType.QuinnToken,
		CurrencyType.JadisToken,
		CurrencyType.SimonToken,
		CurrencyType.ProtectorDarylToken,
		CurrencyType.GauntletAaronToken,
		CurrencyType.PerlieToken,
		CurrencyType.MagnaToken,
		CurrencyType.CroatToken,
		CurrencyType.QuickdrawCarolToken,
		CurrencyType.LydiaToken,
		CurrencyType.StrandToken,
		CurrencyType.ScoutMaggieToken
	};

	public MapMissionModel MapMissionModel { get; set; }

	public static bool HasTokens(CurrencyType currencyType)
	{
		return hasTokens.Contains(currencyType);
	}

	public static bool HasTokens(List<RewardCurrency> rewards)
	{
		for (int i = 0; i < rewards.Count; i++)
		{
			if (HasTokens(rewards[i].CurrencyType))
			{
				return true;
			}
		}
		return false;
	}

	private bool GetIconSpriteNameForSurvival(MapMissionModel missionModel, out string icon1, out string icon2, out int rarityStars)
	{
		int num = missionModel.SolveOrderNumberInGroup();
		if (missionModel.State == MapMissionState.Completed)
		{
			icon1 = null;
			icon2 = null;
			rarityStars = 0;
			return false;
		}
		PlayerModel playerModel = (PlayerModel)missionModel.Manager.GetPlayer();
		WeeklySurvivalModel weeklySurvival = playerModel.WeeklySurvival;
		if (weeklySurvival == null)
		{
			Debug.LogError("No weekly survival model, cannot solve icon sprite.");
			icon1 = null;
			icon2 = null;
			rarityStars = 0;
			return false;
		}
		List<WeeklySurvivalReward> personalRewardsBetween = weeklySurvival.GetPersonalRewardsBetween(num, num + 1);
		if (personalRewardsBetween.Count == 0)
		{
			Debug.LogError("Survival mission completion gives no reward, cannot solve icon sprite.");
			icon1 = null;
			icon2 = null;
			rarityStars = 0;
			return false;
		}
		WeeklySurvivalReward weeklySurvivalReward = personalRewardsBetween[0];
		int currentDifficulty = (int)weeklySurvival.CurrentDifficulty;
		if (weeklySurvivalReward.RewardEntries == null || currentDifficulty >= weeklySurvivalReward.RewardEntries.Length || weeklySurvivalReward.RewardEntries[currentDifficulty] == null || weeklySurvivalReward.RewardEntries[currentDifficulty].Count < 1)
		{
			Debug.LogError("Survival mission completion reward list is empty, cannot solve icon sprite.");
			icon1 = null;
			icon2 = null;
			rarityStars = 0;
			return false;
		}
		icon1 = null;
		icon2 = null;
		int num2 = weeklySurvivalReward.RewardEntries[currentDifficulty].Count;
		if (num2 > 2)
		{
			num2 = 2;
		}
		for (int i = 0; i < num2; i++)
		{
			IReward rewardAt = weeklySurvivalReward.RewardEntries[currentDifficulty].GetRewardAt(i);
			if (rewardAt.Type == RewardType.Equipment || rewardAt.Type == RewardType.RandomEquipment)
			{
				icon1 = "Ui_Icon_Equipment";
				icon2 = null;
				rarityStars = 0;
				if (rewardAt is RewardEquipment)
				{
					rarityStars = ((RewardEquipment)rewardAt).RarityLevel + 1;
				}
				else if (rewardAt is RewardRandomEquipment)
				{
					rarityStars = ((RewardRandomEquipment)rewardAt).RarityLevel + 1;
				}
				return true;
			}
			HelpersGfx.GetIconNameForIReward(rewardAt, out var spriteName, null, null, null, playerModel);
			if (!string.IsNullOrEmpty(spriteName))
			{
				if (icon1 == null)
				{
					icon1 = spriteName;
				}
				else
				{
					icon2 = spriteName;
				}
			}
		}
		rarityStars = 0;
		return icon1 != null;
	}

	private string GetIconSpriteName(MapMissionModel missionModel, out UIAtlas atlas)
	{
		atlas = mapIconAtlas;
		bool flag = missionModel.GetStoryMissionRewards() != null && missionModel.GetStoryMissionRewards().GetTotalCurrencyRewardAmount(CurrencyType.DarylToken) > 0;
		bool flag2 = missionModel.GetStoryMissionRewards() != null && missionModel.GetStoryMissionRewards().GetTotalCurrencyRewardAmount(CurrencyType.NeganToken) > 0;
		bool flag3 = missionModel.GetStoryMissionRewards() != null && missionModel.GetStoryMissionRewards().GetTotalCurrencyRewardAmount(CurrencyType.EzekielToken) > 0;
		RewardCurrency rewardCurrency = missionModel.GetStoryMissionRewards()?.GetStoryFirstCurrency();
		bool flag4 = missionModel.GetStoryMissionRewards() != null && HasTokens(missionModel.GetStoryMissionRewards().GetAllRewardCurrencies());
		bool flag5 = missionModel.MissionSpawnPointGroup.Category == MapCategory.Season && missionModel.IsLastInGroup;
		if (missionModel.MissionSpawnPointGroup.Category == MapCategory.Season && flag2)
		{
			return "Ui_Mission_Icon_Negan";
		}
		if (missionModel.MissionSpawnPointGroup.Category == MapCategory.Season && flag3)
		{
			return "Ui_Mission_Icon_Ezekiel";
		}
		if (missionModel.MissionSpawnPointGroup.Category == MapCategory.Season && (missionModel.HasStoryMissionRewardOfType(RewardType.SurvivorToken) || flag4))
		{
			return "Ui_Mission_Icon_Token";
		}
		if (missionModel.MissionSpawnPointGroup.Category == MapCategory.Season && missionModel.HasStoryMissionRewardOfType(RewardType.Currency))
		{
			if (iconCurrency == null || iconCurrency1_2 == null || iconCurrency2_2 == null)
			{
				return "Ui_Mission_Icon_Supplies";
			}
			if (missionModel.IsLastInGroup && missionModel.State == MapMissionState.Locked)
			{
				return "";
			}
			List<RewardCurrency> allRewardCurrencies = missionModel.GetStoryMissionRewards().GetAllRewardCurrencies();
			bool flag6 = true;
			for (int i = 0; i < (allRewardCurrencies?.Count ?? 0); i++)
			{
				RewardCurrency rewardCurrency2 = allRewardCurrencies[i];
				if (rewardCurrency2 != null && allRewardCurrencies.Count == 1)
				{
					iconCurrency.enabled = true;
					iconCurrency.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency2.CurrencyType);
				}
				else if (rewardCurrency2 != null)
				{
					if (flag6)
					{
						flag6 = false;
						iconCurrency1_2.enabled = true;
						iconCurrency1_2.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency2.CurrencyType);
					}
					else
					{
						iconCurrency2_2.enabled = true;
						iconCurrency2_2.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency2.CurrencyType);
					}
				}
			}
			return "Ui_Mission_Icon_Supplies";
		}
		if (missionModel.MissionSpawnPointGroup.Category == MapCategory.Season)
		{
			if (!(missionModel.State == MapMissionState.Completed && flag5))
			{
				return "Ui_Mission_Icon_Supplies";
			}
			if (!string.IsNullOrEmpty(trialCompletedSpriteName))
			{
				return trialCompletedSpriteName;
			}
		}
		if (flag)
		{
			return "Ui_Mission_Icon_Daryl";
		}
		if (rewardCurrency != null)
		{
			string currencyIconName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			if (!string.IsNullOrEmpty(currencyIconName))
			{
				atlas = currenciesAtlas;
				return currencyIconName;
			}
		}
		if (missionModel.MissionData.MissionType == MissionType.Rescue)
		{
			return "Ui_Mission_Icon_Rescue";
		}
		if (missionModel.HasStoryMissionRewardOfType(RewardType.Equipment))
		{
			return "Ui_Mission_Icon_Equipment";
		}
		return "";
	}

	private string GetBackgroundSpriteForSurvival(MapMissionModel missionModel, MapMissionState missionState)
	{
		return missionState switch
		{
			MapMissionState.Completed => "Ui_Mission_Marker_Distance_Complete",
			MapMissionState.Locked => "Ui_Mission_Marker_Distance_Locked",
			_ => "Ui_Mission_Marker_Distance_Active",
		};
	}

	private string GetBackgroundSprite(MapMissionModel missionModel, MapMissionState missionState)
	{
		bool flag = missionModel.GetStoryMissionRewards() != null && missionModel.GetStoryMissionRewards().GetTotalCurrencyRewardAmount(CurrencyType.DarylToken) > 0;
		string text = "";
		if (finalMissionTexture != null)
		{
			text = "Final_";
		}
		else if (missionModel.MissionData.MissionType == MissionType.Rescue || missionModel.HasStoryMissionRewardOfType(RewardType.Equipment) || missionModel.HasStoryMissionRewardOfSpeedUpToken() || flag || missionModel.MissionSpawnPointGroup.Category == MapCategory.Season)
		{
			text = "Special_";
		}
		switch (missionState)
		{
		case MapMissionState.Locked:
			return "Ui_Mission_Marker_" + text + "Locked";
		case MapMissionState.Completed:
			if (missionModel.MissionSpawnPointGroup.Category != MapCategory.Season)
			{
				return "Ui_Mission_Marker_" + text + "Complete";
			}
			break;
		}
		return "Ui_Mission_Marker_" + text + "Active";
	}

	public void SetPath(GameObject previous)
	{
		if (!(previous == null) && !(pathNormal == null) && !(pathLocked == null))
		{
			UISprite uISprite;
			if (MapMissionModel.IsLocked)
			{
				uISprite = pathLocked;
				pathLocked.enabled = true;
			}
			else
			{
				uISprite = pathNormal;
				pathNormal.enabled = true;
			}
			float num = Vector3.Distance(previous.transform.localPosition, base.gameObject.transform.localPosition);
			uISprite.height = (int)num;
			pathContainer.transform.LookAt(previous.transform.position, new Vector3(0f, 0f, -1f));
		}
	}

	public void SetDifficulty(SurvivalDifficulty survivalDifficulty)
	{
		Helpers.GameObjectSetActive(hardDifficultyIndicator, survivalDifficulty == SurvivalDifficulty.Hard);
		Helpers.GameObjectSetActive(nightmareDifficultyIndicator, survivalDifficulty == SurvivalDifficulty.Nightmare);
	}

	public void EnableDoubleRewardsIcon(bool doubleRewardsEnabled)
	{
		Helpers.GameObjectSetActive(survivalDoubleRewardsIndicator, doubleRewardsEnabled && !MapMissionModel.IsCompleted);
	}

	private void OnModelChange(ModelObject model, string changed, object args)
	{
		SetMission(MapMissionModel, mapVisualData);
	}

	public void OnDestroy()
	{
		if (MapMissionModel != null)
		{
			MapMissionModel.Changed -= OnModelChange;
		}
	}

	private IEnumerator CreateCollectAnimation(RewardCurrency reward, GameObject from)
	{
		if (collectAnimationPrefab == null)
		{
			yield break;
		}
		yield return new WaitForSeconds(0.1f);
		_ = base.gameObject.transform;
		DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DetailMapPopUp) as DetailMapPopUp;
		SeasonDefinition seasonDefinition = GameManager.Instance.gameEconomyData.GetSeasonDefinition(detailMapPopUp.CurrentSeason);
		Transform collectAnimationDestination;
		if (seasonDefinition != null && seasonDefinition.RewardCurrency != CurrencyType.None && reward.CurrencyType == seasonDefinition.RewardCurrency)
		{
			collectAnimationDestination = detailMapPopUp.seasonRewardPosition.transform;
		}
		else if (HasTokens(reward.CurrencyType))
		{
			collectAnimationDestination = detailMapPopUp.fakeSurvivorButton.GetIconTarget().transform;
			detailMapPopUp.ToggleFakeSurvivor(enable: true, reward);
		}
		else if (reward.CurrencyType == CurrencyType.Phone)
		{
			collectAnimationDestination = detailMapPopUp.fakeRadioButton.GetIconTarget().transform;
			detailMapPopUp.ToggleFakePhone(enable: true, reward);
		}
		else
		{
			collectAnimationDestination = CampView.Instance.Hud.GetCollectAnimationDestination(reward.CurrencyType);
		}
		int amount = reward.Amount;
		int b = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 10 : 20);
		int num = Mathf.Min(amount, b);
		if (!(from != null) || !(collectAnimationDestination != null))
		{
			yield break;
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/collect_token");
		}
		for (int i = 0; i < num; i++)
		{
			CollectAnimation component = Helpers.InstantiateToParentAndLayer(collectAnimationPrefab, base.gameObject).GetComponent<CollectAnimation>();
			if (component != null)
			{
				component.FollowTarget(from);
			}
			bool isFirst = i == 0;
			component.StartAnimation(amount, reward.CurrencyType, collectAnimationDestination, null, isFirst);
		}
	}

	private IEnumerator WaitForPendingPopups(GameObject targetObject, bool needToAnimateCheckMark)
	{
		yield return new WaitForSeconds(1f);
		int count = SingularityMonoBehaviour<HUDManager>.Instance.GetOpenPopupsList().Count;
		int num = ((GameManager.Instance.playerModel.WeeklyChallenge.Rewards != null) ? GameManager.Instance.playerModel.WeeklyChallenge.Rewards.Count : 0);
		isWaitingPopupsToAnimate = num > 0 || count > 1;
		if (!isWaitingPopupsToAnimate)
		{
			TriggerChallengeStarsAnimation(targetObject, needToAnimateCheckMark);
		}
	}

	private void TriggerChallengeStarsAnimation(GameObject targetObject, bool needToAnimateCheckMark)
	{
		if (needToAnimateCheckMark)
		{
			TweenManager.PlayTweenGroup(base.gameObject, ChallengeStarsTweenGroup + 3, forward: true, OnCompletedMissionAnimationPart2Done);
		}
		if (challengeStars[nextStarToAnimateIndex] != null)
		{
			challengeStars[nextStarToAnimateIndex].SetActive(value: true);
		}
		TweenManager.PlayTweenGroup(targetObject, ChallengeStarsTweenGroup + nextStarToAnimateIndex, forward: true, OnChallengeAnimationDone);
	}

	private IEnumerator TriggerNewChallengeDifficultyAnimation(GameObject targetObject)
	{
		yield return new WaitForSeconds(2f);
		if (targetMissionModel != null)
		{
			targetMissionModel.MissionStarsDisplayedInUI = 0;
		}
		TweenManager.PlayTweenGroup(base.gameObject, ChallengeDifficultyChangedTweenGroup, forward: true, OnNewDifficultyAnimationDone);
	}

	private void ResetTweens(GameObject icon, bool begin)
	{
		if (!(icon != null))
		{
			return;
		}
		TweenScale component = icon.GetComponent<TweenScale>();
		if (component != null)
		{
			if (begin)
			{
				component.ResetToBeginning();
			}
			else
			{
				component.ResetToEnd();
			}
		}
		TweenAlpha component2 = icon.GetComponent<TweenAlpha>();
		if (component2 != null)
		{
			if (begin)
			{
				component2.ResetToBeginning();
			}
			else
			{
				component2.ResetToEnd();
			}
		}
	}

	public void SetMission(MapMissionModel mapMissionModel, MapVisualData mapVisualData)
	{
		isWaitingPopupsToAnimate = false;
		targetMissionModel = mapMissionModel;
		this.mapVisualData = mapVisualData;
		MapMissionState mapMissionState = mapMissionModel.State;
		_ = mapMissionModel.LootTag;
		bool flag = mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge;
		bool isInWeeklySurvival = mapMissionModel.IsInWeeklySurvival;
		bool flag2 = mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && mapMissionModel.IsLastInGroup;
		MapMissionModel lastPlayedMissionModel = GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel;
		if (lastPlayedMissionModel != null && lastPlayedMissionModel.IsCompleted && !flag && (lastPlayedMissionModel.CompletionTimes == 0 || lastPlayedMissionModel.CompletionTimes == 1))
		{
			missionPlayedModel = lastPlayedMissionModel;
			switch (mapMissionState)
			{
			case MapMissionState.Unlocked:
				mapMissionState = MapMissionState.Locked;
				TweenManager.PlayTweenGroup(base.gameObject, 10, forward: true, OnUnlockedAnimationPart1Done);
				break;
			case MapMissionState.Completed:
				if (lastPlayedMissionModel == mapMissionModel)
				{
					mapMissionState = MapMissionState.Unlocked;
					TweenManager.PlayTweenGroup(base.gameObject, 20, forward: true, OnCompletedMissionAnimationPart1Done);
				}
				break;
			}
		}
		if (lastPlayedMissionModel != null && lastPlayedMissionModel == mapMissionModel && lastPlayedMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && (lastPlayedMissionModel.IsCompleted || lastPlayedMissionModel.State == MapMissionState.Respawning))
		{
			Rewards storyMissionRewards = lastPlayedMissionModel.GetStoryMissionRewards(1);
			if (storyMissionRewards != null)
			{
				List<RewardCurrency> allRewardCurrencies = storyMissionRewards.GetAllRewardCurrencies();
				if (allRewardCurrencies != null && lastPlayedMissionModel.LatestRunResult == ECombatResult.Successful)
				{
					for (int i = 0; i < allRewardCurrencies.Count; i++)
					{
						StartCoroutine(CreateCollectAnimation(allRewardCurrencies[i], (rewardOrigin != null) ? rewardOrigin : base.gameObject));
					}
				}
			}
		}
		if (lastPlayedMissionModel != null && lastPlayedMissionModel == mapMissionModel && lastPlayedMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && lastPlayedMissionModel.IsCompleted)
		{
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(lastPlayedMissionModel.MissionSpawnPointGroupId);
			if (missionGroupModelForSpawnPointGroup != null)
			{
				MissionHighlight isFeaturedData = missionGroupModelForSpawnPointGroup.IsFeaturedData;
				if (isFeaturedData != null && mapMissionModel.IsLastInGroup)
				{
					List<RewardCurrency> allRewardCurrencies2 = isFeaturedData.CompletionRewards.GetAllRewardCurrencies();
					if (allRewardCurrencies2 != null && lastPlayedMissionModel.LatestRunResult == ECombatResult.Successful)
					{
						for (int j = 0; j < allRewardCurrencies2.Count; j++)
						{
							StartCoroutine(CreateCollectAnimation(allRewardCurrencies2[j], (rewardOrigin != null) ? rewardOrigin : base.gameObject));
						}
					}
				}
			}
		}
		if (iconCurrency != null && iconCurrency1_2 != null && iconCurrency2_2 != null)
		{
			iconCurrency.enabled = false;
			iconCurrency1_2.enabled = false;
			iconCurrency2_2.enabled = false;
		}
		if (pathNormal != null && pathLocked != null)
		{
			pathNormal.enabled = false;
			pathLocked.enabled = false;
			if (!flag)
			{
				if (mapMissionModel.IsLocked)
				{
					pathLocked.enabled = true;
				}
				else
				{
					pathNormal.enabled = true;
				}
			}
		}
		if (iconSprite != null)
		{
			if (isInWeeklySurvival)
			{
				int rarityStars = 0;
				GetIconSpriteNameForSurvival(mapMissionModel, out var icon, out var icon2, out rarityStars);
				if (icon2 != null)
				{
					iconCurrency1_2.enabled = true;
					iconCurrency1_2.gameObject.SetActive(value: true);
					iconCurrency1_2.spriteName = icon;
					iconCurrency2_2.enabled = true;
					iconCurrency2_2.gameObject.SetActive(value: true);
					iconCurrency2_2.spriteName = icon2;
					iconSprite.enabled = false;
				}
				else
				{
					iconSprite.spriteName = icon ?? "";
					if (rarityStars > 0 && survivalRarityStars != null)
					{
						for (int k = 0; k < survivalRarityStars.Length; k++)
						{
							survivalRarityStars[k].SetActive(k < rarityStars);
						}
						if (survivalRarityStarGrid != null)
						{
							survivalRarityStarGrid.hideInactive = true;
						}
					}
				}
			}
			else
			{
				UIAtlas atlas;
				string iconSpriteName = GetIconSpriteName(mapMissionModel, out atlas);
				HelpersUI.SetSpriteAndAtlas(iconSprite, iconSpriteName, atlas);
			}
		}
		if (deadlyMissionIndicator != null)
		{
			deadlyMissionIndicator.gameObject.SetActive(mapMissionModel.IsDeadly);
		}
		if (grindContainer != null)
		{
			grindContainer.SetActive(value: false);
		}
		if (ActiveGlow != null)
		{
			bool active = mapMissionState == MapMissionState.Unlocked;
			ActiveGlow.SetActive(active);
		}
		if (completionTimes != null)
		{
			if (mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && mapMissionModel.IsCompleted && flag2)
			{
				completionTimes.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Season.Completion.Progression.Completed");
			}
			else
			{
				completionTimes.text = LocalizationManager.GetText("Season.Completion.Progression{amount}{total}", Mathf.Min(mapMissionModel.CompletionTimes, GameManager.Instance.gameEconomyData.ConfigData.SeasonTrialDifficultyLevels.Count), GameManager.Instance.gameEconomyData.ConfigData.SeasonTrialDifficultyLevels.Count);
			}
		}
		if (finalMissionTexture != null && mapVisualData != null)
		{
			MapMissionGroupModel originalDifficultyMapMissionGroupModel = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(mapMissionModel.MissionSpawnPointGroupId).GetOriginalDifficultyMapMissionGroupModel();
			GameObject detailMapItemPrefab = mapVisualData.GetDetailMapItemPrefab(originalDifficultyMapMissionGroupModel.MissionSpawnPointGroup.MapId);
			if (detailMapItemPrefab != null)
			{
				Renderer componentInChildren = detailMapItemPrefab.GetComponentInChildren<Renderer>();
				if (componentInChildren != null)
				{
					finalMissionTexture.material = componentInChildren.sharedMaterial;
					finalMissionTexture.transform.localScale = componentInChildren.transform.localScale;
					finalMissionTexture.transform.localPosition = componentInChildren.transform.localPosition;
				}
			}
		}
		if (bloodSplatter != null)
		{
			bloodSplatter.SetActive(mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && GameManager.Instance.playerModel.MapContainerModel.GetMissionIndex(mapMissionModel) == 1);
		}
		if (tutorialArrow != null)
		{
			tutorialArrow.Id = mapMissionModel.MissionData.DisplayTextID;
		}
		if (backgroundSprite != null && !flag && !isInWeeklySurvival && !flag2)
		{
			backgroundSprite.spriteName = GetBackgroundSprite(mapMissionModel, mapMissionState);
		}
		if (backgroundSprite != null && isInWeeklySurvival)
		{
			backgroundSprite.spriteName = GetBackgroundSpriteForSurvival(mapMissionModel, mapMissionState);
		}
		if (isInWeeklySurvival && mapMissionModel.State == MapMissionState.Completed)
		{
			Helpers.GameObjectSetActive(iconSprite, value: false);
		}
		if (flag2 && seasonLock != null)
		{
			seasonLock.SetActive(mapMissionState == MapMissionState.Locked);
			if (backgroundSprite != null)
			{
				switch (mapMissionState)
				{
				case MapMissionState.Completed:
					backgroundSprite.color = seasonCompletedColor;
					break;
				case MapMissionState.Locked:
					backgroundSprite.color = seasonLockedColor;
					break;
				case MapMissionState.Respawning:
					backgroundSprite.color = seasonAvailableColor;
					break;
				case MapMissionState.Unlocked:
					backgroundSprite.color = seasonAvailableColor;
					break;
				}
			}
		}
		if (challengeContainer != null)
		{
			challengeContainer.SetActive(flag);
		}
		if (challengeContainer != null && flag && challengeStarAmountLabel != null)
		{
			challengeStarAmountLabel.text = mapMissionModel.Stars.NumberStars.ToString();
		}
		if (flag)
		{
			if (mapMissionModel.IsMasterMission)
			{
				HelpersUI.SetContentToLabel(masterMissionDifficultyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TeamSelection.MissionLocked[MinSurvivorLevel]", mapMissionModel.RequiredSurvivorLevel));
			}
			bool flag3 = lastPlayedMissionModel != null && lastPlayedMissionModel == mapMissionModel;
			int missionStarsDisplayedInUI = targetMissionModel.MissionStarsDisplayedInUI;
			int num = Mathf.Min(mapMissionModel.Stars.TotalStars, 4) - missionStarsDisplayedInUI;
			for (int l = 0; l < challengeStars.Length; l++)
			{
				if (challengeStars[l] != null)
				{
					bool active2 = (!flag3 && l < Mathf.Min(mapMissionModel.Stars.TotalStars, 4)) || (flag3 && l < missionStarsDisplayedInUI);
					challengeStars[l].SetActive(active2);
					if (challengeStars[l].activeSelf)
					{
						ResetTweens(challengeStars[l], begin: false);
					}
				}
			}
			targetMissionModel.MissionStarsDisplayedInUI = Mathf.Max(targetMissionModel.MissionStarsDisplayedInUI, Mathf.Min(mapMissionModel.Stars.TotalStars, 4));
			needToAnimateCheckMark = missionStarsDisplayedInUI == 0 && flag3;
			if (challengeCheckMark != null)
			{
				challengeCheckMark.SetActive(Mathf.Min(mapMissionModel.Stars.TotalStars, 4) > 0);
				if (challengeCheckMark.activeSelf && !needToAnimateCheckMark)
				{
					ResetTweens(challengeCheckMark, begin: false);
				}
			}
			if (flag3 && num > 0)
			{
				nextStarToAnimateIndex = missionStarsDisplayedInUI;
				lastStarToAnimateIndex = Mathf.Min(mapMissionModel.Stars.TotalStars, 4) - 1;
				StartCoroutine(WaitForPendingPopups(base.gameObject, needToAnimateCheckMark));
			}
			else
			{
				StartCoroutine(TryOpenCycleEndPopup(waitForOtherPopups: true));
			}
		}
		bool active3 = false;
		if (mapMissionState == MapMissionState.Respawning)
		{
			active3 = true;
			timerLabel.text = Helpers.FormatTimeNoZero(mapMissionModel.RespawnTimer);
		}
		if (timerContainer != null)
		{
			timerContainer.SetActive(active3);
		}
		if (CostParent != null && mapMissionModel != null && mapMissionModel.GetStartMissionCashier() != null)
		{
			CostParent.SetActive(!mapMissionModel.IsCompleted || mapMissionModel.IsMasterMission);
			int num2 = mapMissionModel.GetStartMissionCashier().GetTotalCost(CurrencyType.ReplayToken);
			if (GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.UnlimitedGas))
			{
				num2 = 0;
			}
			HelpersUI.SetContentToLabel(CostLabel, num2.ToString());
		}
		MapMissionGroupModel missionGroupModelForSpawnPointGroup2 = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(mapMissionModel.MissionSpawnPointGroupId);
		if (missionGroupModelForSpawnPointGroup2 != null)
		{
			cachedNextHighlight = missionGroupModelForSpawnPointGroup2.NextFeaturedData;
		}
		else
		{
			cachedNextHighlight = null;
		}
		if (MapMissionModel != null)
		{
			MapMissionModel.Changed -= OnModelChange;
		}
		MapMissionModel = mapMissionModel;
		if (MapMissionModel != null)
		{
			MapMissionModel.Changed += OnModelChange;
		}
		if (!flag)
		{
			return;
		}
		if ((mapMissionModel.IsInWeeklyChallenge ? GameManager.Instance.playerModel.WeeklyChallenge.ActiveSkipTokens : GameManager.Instance.playerModel.ApocalypseWeeklyChallenge.ActiveSkipTokens) > 0 && skipTokenBonusStars != null)
		{
			skipTokenBonusStars.gameObject.SetActive(value: true);
			TweenManager.PlayTweenGroup(skipTokenBonusStars.gameObject, 5);
		}
		if (mapMissionModel.IsInWeeklyChallenge)
		{
			backgroundSprite.gameObject.SetActive(value: true);
			backgroundSprite2.gameObject.SetActive(value: false);
			return;
		}
		backgroundSprite.gameObject.SetActive(value: true);
		backgroundSprite2.gameObject.SetActive(value: false);
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		if (weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.CurrentCycle > 50)
		{
			backgroundSprite.gameObject.SetActive(value: false);
			backgroundSprite2.gameObject.SetActive(value: true);
		}
	}

	public void PlayNewDifficultyAnimation()
	{
		int missionStarsDisplayedInUI = targetMissionModel.MissionStarsDisplayedInUI;
		for (int i = 0; i < challengeStars.Length; i++)
		{
			if (challengeStars[i] != null)
			{
				challengeStars[i].SetActive(i < missionStarsDisplayedInUI);
				if (challengeStars[i].activeSelf)
				{
					TweenManager.PlayTweenGroup(base.gameObject, ChallengeStarsTweenGroup + i, forward: true, null, resetToEnd: true);
				}
			}
		}
		if (challengeCheckMark != null)
		{
			challengeCheckMark.SetActive(value: true);
			if (challengeCheckMark.activeSelf)
			{
				ResetTweens(challengeCheckMark, begin: false);
			}
		}
		StartCoroutine(TriggerNewChallengeDifficultyAnimation(base.gameObject));
	}

	public void OnUnlockedAnimationPart1Done()
	{
		SetMission(MapMissionModel, mapVisualData);
		TweenManager.PlayTweenGroup(base.gameObject, 11, forward: true, OnUnlockedAnimationPart2Done);
	}

	public void OnUnlockedAnimationPart2Done()
	{
	}

	public void OnCompletedMissionAnimationPart1Done()
	{
		SetMission(MapMissionModel, mapVisualData);
		TweenManager.PlayTweenGroup(base.gameObject, 21, forward: true, OnCompletedMissionAnimationPart2Done);
	}

	public void OnCompletedMissionAnimationPart2Done()
	{
		ShowStuffAfterMissionCompletion();
	}

	private IEnumerator TryOpenCycleEndPopup(bool waitForOtherPopups)
	{
		if (!needsToShowCycleEndPopup)
		{
			if (waitForOtherPopups)
			{
				yield return new WaitForSeconds(1f);
			}
			WeeklyChallengeModel weeklyChallenge = GameManager.Instance.playerModel.WeeklyChallenge;
			if (weeklyChallenge != null && weeklyChallenge.CanStartNextCycle() && (!weeklyChallenge.HasShownCycleEndedOnClient || WeeklyChallengeHelper.WasLastCompletedMissionTheMasterMission()))
			{
				needsToShowCycleEndPopup = true;
			}
			ApocalypseWeeklyChallengeModel apocalypseWeeklyChallenge = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge;
			if (apocalypseWeeklyChallenge != null && apocalypseWeeklyChallenge.CanStartNextCycle() && (!apocalypseWeeklyChallenge.HasShownCycleEndedOnClient || WeeklyChallengeHelper.WasLastCompletedMissionTheMasterMission()))
			{
				needsToShowCycleEndPopup = true;
			}
		}
	}

	public void OnChallengeAnimationDone()
	{
		nextStarToAnimateIndex++;
		if (nextStarToAnimateIndex <= lastStarToAnimateIndex)
		{
			challengeStars[nextStarToAnimateIndex].SetActive(value: true);
			TweenManager.PlayTweenGroup(base.gameObject, ChallengeStarsTweenGroup + nextStarToAnimateIndex, forward: true, OnChallengeAnimationDone);
		}
		else
		{
			StartCoroutine(TryOpenCycleEndPopup(waitForOtherPopups: false));
		}
	}

	public void OnNewDifficultyAnimationDone()
	{
		for (int i = 0; i < challengeStars.Length; i++)
		{
			challengeStars[i].gameObject.SetActive(value: false);
		}
		if (challengeCheckMark != null)
		{
			challengeCheckMark.gameObject.SetActive(value: false);
		}
	}

	public void ShowStuffAfterMissionCompletion()
	{
		if (missionPlayedModel == null)
		{
			return;
		}
		MissionQuest currentQuestAssociatedToEpisode = GetCurrentQuestAssociatedToEpisode(missionPlayedModel);
		if (currentQuestAssociatedToEpisode != null && currentQuestAssociatedToEpisode.HasCompleted && missionPlayedModel.Stars != null && missionPlayedModel.PreviousNumberStars == 0 && missionPlayedModel.Stars.NumberStars > 0)
		{
			if (GameManager.Instance.gameEconomyData.GetFeature("NML_4962_ContinueQuestInMap").Enabled)
			{
				StoryTellerFlow.StartFlow(GameManager.Instance.playerModel.SurvivorContainer.StoryTeller);
			}
			else
			{
				StoryTellerView.Say(currentQuestAssociatedToEpisode.QuestDefinition.DebriefingKey);
			}
		}
	}

	public MissionQuest GetCurrentQuestAssociatedToEpisode(MapMissionModel mapMissionModel)
	{
		MapMissionGroupModel missionGroupModelForSpawnPointGroup = GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(mapMissionModel.MissionSpawnPointGroupId);
		foreach (StoryTellerView storyTellerView in CampView.Instance.CampViewActors.StoryTellerViews)
		{
			if (storyTellerView.StoryTeller.CurrentQuest is MissionQuest missionQuest && missionQuest.GetUnlockedEpisode() == missionGroupModelForSpawnPointGroup)
			{
				return missionQuest;
			}
		}
		return null;
	}

	public void UpdateAnchorTarget(GameObject obj)
	{
		GetComponent<UIWidget>().SetAnchor(obj);
	}

	public void SetAnimationState(GrindAnimStates state)
	{
		animState = state;
	}

	public void OnClick()
	{
		if (MapMissionModel == null)
		{
			return;
		}
		if (MapMissionModel.State == MapMissionState.Respawning)
		{
			if (MapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season)
			{
				HUDNotification.Info(LocalizationManager.GetText("Notification.Mission.Respawn"));
			}
		}
		else if (MapMissionModel.IsLocked && !MapMissionModel.IsInWeeklySurvival)
		{
			HUDNotification.Info(LocalizationManager.GetText("Notification.Mission.Locked"));
		}
		else if ((!MapMissionModel.IsCompleted || MapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season) && (!MapMissionModel.IsCompleted || MapMissionModel.MissionSpawnPointGroup.Category != MapCategory.Season || !MapMissionModel.IsLastInGroup) && TutorialView.Allowed(MapMissionModel.MissionData.DisplayTextID))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("map/mission_click");
			OpenMissionBriefing(base.gameObject.transform.parent.gameObject, MapMissionModel);
			PlayTween();
			EventManager.NotifyClick(MapMissionModel.MissionData.DisplayTextID);
		}
	}

	public void OpenMissionBriefing(GameObject anchor, MapMissionModel mapMissionModel)
	{
		MissionStartPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MissionBriefing) as MissionStartPopup;
		obj.Open();
		obj.SetMission(mapMissionModel);
	}

	public void PlayTween()
	{
		if (missionTweener != null)
		{
			missionTweener.ResetToBeginning();
			missionTweener.PlayForward();
		}
	}

	public void FadeAndDestroy()
	{
		if (!base.gameObject.activeSelf)
		{
			fadeAndDestroyOnEnable = true;
			return;
		}
		missionDestroyTweener.ResetToBeginning();
		missionDestroyTweener.PlayForward();
	}

	public void RefreshToCurrentAnimState()
	{
		if (animator != null && animator.GetInteger("State") != (int)animState)
		{
			animator.SetInteger("State", (int)animState);
		}
	}

	private void OnEnable()
	{
		if (fadeAndDestroyOnEnable)
		{
			missionDestroyTweener.ResetToBeginning();
			missionDestroyTweener.PlayForward();
		}
		if (MapMissionModel != null)
		{
			SetMission(MapMissionModel, mapVisualData);
		}
		if (introGlow != null)
		{
			introGlow.SetActive(value: false);
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void OnDisable()
	{
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		if (targetMissionModel != null)
		{
			HelpersUI.SetContentToLabel(masterMissionDifficultyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TeamSelection.MissionLocked[MinSurvivorLevel]", targetMissionModel.RequiredSurvivorLevel));
		}
	}

	private void Start()
	{
		iconWidget = GetComponent<UIWidget>();
		animator = GetComponent<Animator>();
		RefreshToCurrentAnimState();
	}

	public void EnableIntroGlow()
	{
		introGlow.SetActive(value: true);
	}

	public void HideIntroGlow(float secondsDelay)
	{
		if (introGlow != null)
		{
			StartCoroutine(CoRoutineHideIntroGlow(secondsDelay));
		}
	}

	private IEnumerator CoRoutineHideIntroGlow(float secondsDelay)
	{
		yield return new WaitForSeconds(secondsDelay);
		if (introGlow != null)
		{
			introGlow.SetActive(value: false);
		}
	}

	private void Update()
	{
		iconWidget.SetAnchor(iconWidget.topAnchor.target);
		if (MapMissionModel != null && MapMissionModel.State == MapMissionState.Respawning)
		{
			timerLabel.text = Helpers.FormatTimeNoZero(MapMissionModel.RespawnTimer);
		}
		else if (timerContainer != null && timerContainer.activeSelf)
		{
			timerContainer.SetActive(value: false);
		}
		if (trialResetLabel != null && trialResetLabel.gameObject != null && GameManager.Instance != null)
		{
			Helpers.GameObjectSetActive(trialResetLabel, cachedNextHighlight != null);
			if (cachedNextHighlight != null)
			{
				trialsResetTimeLeft = cachedNextHighlight.GetTimeUntilStart(GameManager.Instance.playerModel.UtcTimeStamp);
				trialsLocalisationString = LocalizationManager.GetText("SeasonSeven.MissionIconTrial.Timer{Parameter}", Helpers.FormatTime(trialsResetTimeLeft));
				HelpersUI.SetContentToLabel(trialResetLabel, trialsLocalisationString);
				if (trialsResetTimeLeft <= 0)
				{
					cachedNextHighlight = null;
					Helpers.GameObjectSetActive(trialResetLabel, value: false);
				}
			}
		}
		WeeklyChallengeModel weeklyChallenge = GameManager.Instance.playerModel.WeeklyChallenge;
		ApocalypseWeeklyChallengeModel apocalypseWeeklyChallenge = GameManager.Instance.playerModel.ApocalypseWeeklyChallenge;
		int num = ((weeklyChallenge.Rewards != null) ? GameManager.Instance.playerModel.WeeklyChallenge.Rewards.Count : 0);
		int count = SingularityMonoBehaviour<HUDManager>.Instance.GetOpenPopupsList().Count;
		if (isWaitingPopupsToAnimate && count <= 1 && num == 0)
		{
			isWaitingPopupsToAnimate = false;
			TriggerChallengeStarsAnimation(base.gameObject, needToAnimateCheckMark);
		}
		if (needsToShowCycleEndPopup && count <= 1)
		{
			needsToShowCycleEndPopup = false;
			weeklyChallenge.HasShownCycleEndedOnClient = true;
			apocalypseWeeklyChallenge.HasShownCycleEndedOnClient = true;
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeNextCycle).Open();
		}
	}

	public void DestroyTweenerFinished()
	{
		Object.Destroy(base.gameObject);
	}

	public void AnimationCompleteCompleted()
	{
		if (animState == GrindAnimStates.Completed)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
