using System.Collections;
using System.Collections.Generic;
using BaseModel;
using BaseModel.ContentTypes;
using Client.Connectivity;
using TWD.Externals;
using TWDModel;
using UnityEngine;

public class CombatEndFlowThreeByThree : CombatEndFlowStep
{
	private enum State
	{
		NotStarted = 0,
		KeysShown = 1,
		GetMoreKeysShown = 2
	}

	private const float secondsToWaitToContineAfterBoxOpened = 1.8f;

	private const float secondsToWaitForAfterFailure = 5f;

	public GameObject cratesFoundContainer;

	public GameObject BuyObject;

	public GameObject BuyObjectOld;

	public GameObject ReturnToCampObject;

	public GameObject UnlockButtonsContainer;

	public GameObject UnlockButtonsContainerOld;

	public GameObject KeysContainer;

	public GameObject TimeLeftKeyRefreshContainer;

	public GameObject TimeLeftKeyRefreshContainerOld1;

	public GameObject TimeLeftKeyRefreshContainerOld2;

	public UIButton WatchAdButton;

	public UIButton WatchAnotherButton;

	public UIButton WatchAnotherOldButton;

	public UIButton SpendLootKeysButton;

	public UIButton SpendLootKeysOldButton;

	public GameObject Key1Container;

	public GameObject Key2Container;

	public GameObject Key3Container;

	public UISprite BuyMoreBackground;

	public UISprite BuyMoreBackgroundOld;

	public int BuyMoreBackgroundHeight;

	public int BuyMoreBackgroundHeightWithoutAds;

	public int BuyMoreBackgroundHeightWithAds = 480;

	public UILabel BuyButtonCostLabel;

	public UILabel BuyButtonFreeCostLabel;

	public UILabel TimeLeftKeyRefreshLabel;

	public UILabel TimeLeftKeyRefreshLabelOld1;

	public UILabel TimeLeftKeyRefreshLabelOld2;

	public UILabel BuyButtonCostLabelOld;

	public UILabel BuyButtonFreeCostLabelOld;

	[Header("Share")]
	[SerializeField]
	private UIButton shareButton;

	[SerializeField]
	private GameObject sharePanel;

	[SerializeField]
	private UITexture shareBadge;

	[SerializeField]
	private UIButton infoButton;

	[SerializeField]
	private UILabel lootKeysRemainingLabel;

	[SerializeField]
	private UILabel lootKeysRemainingLabelOld;

	public List<SpecialRewardIcon> SpecialLootIcons;

	private GameObject equipmentCard;

	private bool UseLootKeysFeature;

	private bool isAdPlaying;

	private float adPlayStartTime;

	private Coroutine waitCommandCoroutine;

	private State state;

	private bool SpendAllLootKeys => GameManager.Instance.playerModel.GetCurrency(CurrencyType.LootKeys).Value == 0;

	public CombatEndFlowThreeByThree()
	{
		DestroyAfterCompletion = false;
	}

	public override void Update()
	{
		base.Update();
		if (!(TimeLeftKeyRefreshContainer != null) || !(TimeLeftKeyRefreshLabel != null) || (!TimeLeftKeyRefreshContainer.activeSelf && !TimeLeftKeyRefreshContainerOld1.activeSelf && !TimeLeftKeyRefreshContainerOld2.activeSelf))
		{
			return;
		}
		long timeLeft = GameManager.Instance.playerModel.LootKeysFirstSpentTime + GameManager.Instance.playerModel.gameEconomyData.ConfigData.LootKeyRefreshRate - GameManager.Instance.playerModel.UtcTimeStamp;
		if (FormatTimeLeftUntilKeyRefresh(timeLeft) == "")
		{
			if (TimeLeftKeyRefreshContainer.activeSelf)
			{
				BuyMoreBackground.height = BuyMoreBackgroundHeightWithoutAds;
				TimeLeftKeyRefreshContainer.SetActive(value: false);
			}
			if (TimeLeftKeyRefreshContainerOld1.activeSelf)
			{
				BuyMoreBackgroundOld.height = BuyMoreBackgroundHeight;
				TimeLeftKeyRefreshContainerOld1.SetActive(value: false);
			}
			if (TimeLeftKeyRefreshContainerOld2.activeSelf)
			{
				BuyMoreBackgroundOld.height = BuyMoreBackgroundHeightWithoutAds;
				TimeLeftKeyRefreshContainerOld2.SetActive(value: false);
			}
		}
		else
		{
			TimeLeftKeyRefreshLabel.text = FormatTimeLeftUntilKeyRefresh(timeLeft);
			TimeLeftKeyRefreshLabelOld1.text = FormatTimeLeftUntilKeyRefresh(timeLeft);
			TimeLeftKeyRefreshLabelOld2.text = FormatTimeLeftUntilKeyRefresh(timeLeft);
		}
	}

	public void OnInfoClicked()
	{
		if (GameManager.Instance.playerModel == null)
		{
			return;
		}
		ModelList<LootEntry> loots = GameManager.Instance.playerModel.LootManager.Loots;
		List<DropType> lootEntryTypesAvailable = GetLootEntryTypesAvailable();
		LootEntry lootEntry = ((loots != null && loots.Count > 0) ? loots[0] : null);
		DropEventDefinition.DropEventContext dropContext = GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionModel.DropContext;
		if (lootEntryTypesAvailable.Count > 0 && lootEntry != null)
		{
			DropEventDefinition dropEventDefinition = lootEntry.DropEventDefinition;
			DropType usedDropType = DropType.Regular;
			DropTableItem[] array = new DropTableItem[lootEntryTypesAvailable.Count];
			for (int i = 0; i < lootEntryTypesAvailable.Count; i++)
			{
				List<ItemAmountProbabilityData> probabilities = GameManager.Instance.gameEconomyData.GetCurrencyProbabilities(dropEventDefinition.EventType, lootEntryTypesAvailable[i], dropContext, dropEventDefinition.Tag, lootEntry.TargetLevel, out usedDropType, GameManager.Instance.playerModel.ActivityManager);
				DropRatesNamesHelper.GetNamesForDropCurrencies(ref probabilities);
				DropTableItem dropTableItem = new DropTableItem
				{
					DropName = LocalizationManager.GetText("Droprate.Table.Name.Lootbox" + usedDropType),
					Description = LocalizationManager.GetText("Droprate.Table.Description.Lootbox" + usedDropType),
					Probabilities = probabilities
				};
				array[i] = dropTableItem;
			}
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup).TryOpenWithNormalData(array);
		}
	}

	private List<DropType> GetLootEntryTypesAvailable()
	{
		List<DropType> list = new List<DropType>();
		if (GameManager.Instance.playerModel != null)
		{
			ModelList<LootEntry> loots = GameManager.Instance.playerModel.LootManager.Loots;
			for (int i = 0; i < (loots?.Count ?? 0); i++)
			{
				if (!list.Contains(loots[i].DropType))
				{
					list.Add(loots[i].DropType);
				}
			}
			list.Sort((DropType a, DropType b) => a.CompareTo(b) * -1);
		}
		return list;
	}

	private void OnEnable()
	{
		UseLootKeysFeature = true;
		UnlockButtonsContainer.SetActive(value: false);
		KeysContainer.SetActive(value: false);
		state = State.NotStarted;
		PlayerModel player = GameManager.Instance.modelManager.Player;
		if (player != null)
		{
			player.Changed += OnPlayerModelChanged;
		}
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.OnRewardBoxClicked += OnRewardBoxClicked;
			RewardScreenHandler.Instance.OnRewardBoxOpened += OnRewardBoxOpened;
			RewardScreenHandler.Instance.OnRewardBoxOpenedAnimationOver += OnRewardBoxOpenedOver;
		}
		EventManager.OnEvent += OnEvent;
		int lootKeySoftCap = player.ActivityManager.GetLootKeySoftCap(player.gameEconomyData.ConfigData);
		if (UseLootKeysFeature)
		{
			if (player.UtcTimeStamp - player.LootKeysFirstSpentTime >= player.gameEconomyData.ConfigData.LootKeyRefreshRate && player.GetCurrency(CurrencyType.LootKeys).Value < lootKeySoftCap)
			{
				Helpers.ExecuteCommand(new RefreshLootKeysCommand());
			}
		}
		else if (player.UtcTimeStamp - player.LootKeysFirstSpentTime >= player.gameEconomyData.ConfigData.LootKeyRefreshRate && player.GetCurrency(CurrencyType.LootKeys).Value < lootKeySoftCap)
		{
			Helpers.ExecuteCommand(new RefreshLootKeysCommand());
		}
		UpdateUI();
		if (waitCommandCoroutine != null)
		{
			StartWaitCommandQueueCoroutine();
		}
	}

	private void HandleAdAvailable()
	{
		if (SingularityMonoBehaviour<VideoAdController>.Instance.IsVideoAdReady(AdUsage.CombatRewardKey))
		{
			UpdateUI();
		}
	}

	private void OnDisable()
	{
		CampHUD.Get().PauseCurrencyMeters = false;
		PlayerModel player = GameManager.Instance.modelManager.Player;
		EventManager.OnEvent -= OnEvent;
		if (player != null)
		{
			player.Changed -= OnPlayerModelChanged;
		}
		if (RewardScreenHandler.Instance != null)
		{
			RewardScreenHandler.Instance.OnRewardBoxClicked -= OnRewardBoxClicked;
			RewardScreenHandler.Instance.OnRewardBoxOpened -= OnRewardBoxOpened;
			RewardScreenHandler.Instance.OnRewardBoxOpenedAnimationOver -= OnRewardBoxOpenedOver;
		}
		_ = UseLootKeysFeature;
		isAdPlaying = false;
		Helpers.ClearUnusedMemory(gcCollect: true);
	}

	public void OnPlayerModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "currencyChangedEvent" && args is CurrencyModel { Type: CurrencyType.Diamonds })
		{
			UpdateBuyButton();
		}
	}

	private bool CanOpenCards()
	{
		if (RewardScreenHandler.Instance != null)
		{
			return RewardScreenHandler.Instance.CanClickRewardBox();
		}
		return false;
	}

	private int GetKeysLeft()
	{
		if (RewardScreenHandler.Instance != null)
		{
			return RewardScreenHandler.Instance.GetKeysLeft();
		}
		return 0;
	}

	private int GetUnopenedRewardsLeft()
	{
		if (RewardScreenHandler.Instance != null)
		{
			return RewardScreenHandler.Instance.GetUnopenedRewardsLeft();
		}
		return 0;
	}

	public override void StartFlow()
	{
		base.StartFlow();
		shareButton.gameObject.SetActive(value: false);
		sharePanel.SetActive(value: false);
		UpdateUI();
		InitializeSpecialLootIcons();
		RewardScreenHandler.Instance.ShowScene(LootScreenType.Combat);
		NotifySetBackground(enabled: false);
		CampHUD.OpenHudPostCombat();
		if (GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionGroupModel != null)
		{
			int numberCompletedStoryMissions = GameManager.Instance.playerModel.MapContainerModel.AttackTargetMissionGroupModel.GetNumberCompletedStoryMissions();
			string text = "RewardsScreen" + numberCompletedStoryMissions;
			if (numberCompletedStoryMissions == 3 && GameManager.Instance.gameEconomyData.ConfigData.HideMissionGoldSilverChest)
			{
				text = null;
			}
			if (text != null)
			{
				if (text == GameManager.Instance.playerModel.Tutorial.CurrentPartId)
				{
					TutorialView.Instance.ResumeCurrentPart();
				}
				else
				{
					TutorialView.Instance.StartPart(text);
				}
			}
		}
		cratesFoundContainer.SetActive(NeedShowBuyMorePanel() && !GameManager.Instance.gameEconomyData.ConfigData.HideMissionGoldSilverChest);
		if (infoButton != null && TutorialView.Instance != null && TutorialView.Instance.Running)
		{
			Helpers.GameObjectSetActive(infoButton, value: false);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateRewardsCount();
		UpdateBuyButton();
		SetReturnToCamp();
	}

	private void InitializeSpecialLootIcons()
	{
		LootManagerModel lootManager = GameManager.Instance.playerModel.LootManager;
		int goldenBoxCount = lootManager.GetGoldenBoxCount();
		int silverBoxCount = lootManager.GetSilverBoxCount();
		int num = goldenBoxCount + silverBoxCount;
		if (num > 6)
		{
			Animator component = cratesFoundContainer.GetComponent<Animator>();
			if (component != null)
			{
				component.enabled = false;
			}
			cratesFoundContainer.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		}
		else
		{
			cratesFoundContainer.transform.localScale = Vector3.one;
		}
		for (int i = 0; i < SpecialLootIcons.Count; i++)
		{
			if (i < num)
			{
				SpecialLootIcons[i].gameObject.SetActive(value: true);
				SpecialLootIcons[i].SetIcon((i >= goldenBoxCount) ? SpecialRewardIconState.Silver : SpecialRewardIconState.Gold);
			}
			else
			{
				SpecialLootIcons[i].gameObject.SetActive(value: false);
			}
		}
	}

	private void UpdateBuyButton()
	{
		if (WatchAdButton != null)
		{
			WatchAdButton.gameObject.SetActive(value: false);
		}
		if (WatchAnotherButton != null)
		{
			WatchAnotherButton.gameObject.SetActive(value: false);
		}
		if (WatchAnotherOldButton != null)
		{
			WatchAnotherOldButton.gameObject.SetActive(value: false);
		}
		if (GetKeysLeft() <= 0 && GetUnopenedRewardsLeft() > 0 && NeedShowBuyMorePanel())
		{
			if (UseLootKeysFeature)
			{
				if (BuyObject != null)
				{
					BuyObject.SetActive(value: true);
					if (BuyButtonCostLabel != null)
					{
						BuyButtonCostLabel.color = (CanBuyMore() ? Color.white : Color.red);
						int buyMorePrice = GetBuyMorePrice();
						BuyButtonCostLabel.gameObject.SetActive(buyMorePrice > 0);
						BuyButtonFreeCostLabel.gameObject.SetActive(buyMorePrice == 0);
						BuyButtonCostLabel.text = buyMorePrice.ToString();
					}
				}
			}
			else if (BuyObjectOld != null)
			{
				BuyObjectOld.SetActive(value: true);
				if (BuyButtonCostLabelOld != null)
				{
					BuyButtonCostLabelOld.color = (CanBuyMore() ? Color.white : Color.red);
					int buyMorePrice2 = GetBuyMorePrice();
					BuyButtonCostLabelOld.gameObject.SetActive(buyMorePrice2 > 0);
					BuyButtonFreeCostLabelOld.gameObject.SetActive(buyMorePrice2 == 0);
					BuyButtonCostLabelOld.text = buyMorePrice2.ToString();
				}
			}
			bool flag = false;
			flag = CanBuyMoreWithKeys() && TutorialView.Instance.Model.CurrentPartId == null && GameManager.Instance.playerModel.Combat != null && !GameManager.Instance.playerModel.Combat.HasSpentLootKeyCurrency;
			int lootKeySoftCap = GameManager.Instance.playerModel.ActivityManager.GetLootKeySoftCap(GameManager.Instance.playerModel.gameEconomyData.ConfigData);
			if (UseLootKeysFeature)
			{
				Helpers.GameObjectSetActive(UnlockButtonsContainer, value: true);
				Helpers.GameObjectSetActive(UnlockButtonsContainerOld, value: false);
				if (SpendLootKeysButton != null)
				{
					SpendLootKeysButton.gameObject.SetActive(flag);
					TimeLeftKeyRefreshContainer.SetActive(SpendAllLootKeys);
					if (flag)
					{
						HelpersUI.SetContentToLabel(lootKeysRemainingLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.CombatEndScreen.UnlocksRemaining", GameManager.Instance.playerModel.GetCurrency(CurrencyType.LootKeys).Value + "/" + lootKeySoftCap));
					}
				}
				if (WatchAdButton != null)
				{
					WatchAdButton.gameObject.SetActive(value: false);
				}
				BuyMoreBackground.height = ((flag || SpendAllLootKeys) ? BuyMoreBackgroundHeight : BuyMoreBackgroundHeightWithoutAds);
			}
			else
			{
				Helpers.GameObjectSetActive(UnlockButtonsContainer, value: false);
				Helpers.GameObjectSetActive(UnlockButtonsContainerOld, value: true);
				bool flag2 = GameManager.Instance.playerModel.Combat == null || GameManager.Instance.playerModel.Combat.VideoAdsServedInRewardScreen >= GameManager.Instance.gameEconomyData.ConfigData.VideoAdRewardScreenLimit;
				if ((GameManager.Instance.ShouldAskForAdConsent() || SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(AdUsage.CombatRewardKey)) && TutorialView.Instance.Model.CurrentPartId == null)
				{
					_ = !flag2;
				}
				else
					_ = 0;
				if (SpendLootKeysOldButton != null)
				{
					SpendLootKeysOldButton.gameObject.SetActive(flag);
					if (flag)
					{
						HelpersUI.SetContentToLabel(lootKeysRemainingLabelOld, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.CombatEndScreen.UnlocksRemaining", GameManager.Instance.playerModel.GetCurrency(CurrencyType.LootKeys).Value + "/" + lootKeySoftCap));
					}
				}
				TimeLeftKeyRefreshContainerOld2.SetActive(SpendAllLootKeys && !WatchAdButton.gameObject.activeSelf);
				if (TimeLeftKeyRefreshContainerOld2.gameObject.activeSelf || flag)
				{
					BuyMoreBackgroundOld.height = BuyMoreBackgroundHeight;
				}
				else if (WatchAdButton.gameObject.activeSelf)
				{
					BuyMoreBackgroundOld.height = BuyMoreBackgroundHeightWithAds;
				}
				else
				{
					BuyMoreBackgroundOld.height = BuyMoreBackgroundHeightWithoutAds;
				}
			}
			if (WatchAnotherButton != null)
			{
				WatchAnotherButton.gameObject.SetActive(value: false);
			}
			if (WatchAnotherOldButton != null)
			{
				WatchAnotherOldButton.gameObject.SetActive(value: false);
			}
			if (state != State.GetMoreKeysShown && TutorialView.Instance.Model.CurrentPartId == null)
			{
				HideKeysShowBuyMore();
			}
		}
		else
		{
			if (state == State.KeysShown)
			{
				return;
			}
			state = State.KeysShown;
			if (UseLootKeysFeature)
			{
				if (UnlockButtonsContainer.activeInHierarchy)
				{
					TweenManager.PlayTweenGroup(UnlockButtonsContainer, 2, forward: true, ShowKeys);
				}
				else
				{
					ShowKeys();
				}
			}
			else if (UnlockButtonsContainerOld.activeInHierarchy)
			{
				TweenManager.PlayTweenGroup(UnlockButtonsContainerOld, 2, forward: true, ShowKeys);
			}
			else
			{
				ShowKeys();
			}
		}
	}

	private IEnumerator DelayShowWatchAnotherButton()
	{
		yield return new WaitForSeconds(5f);
		if (isAdPlaying)
		{
			WatchAdButton.gameObject.SetActive(value: false);
		}
	}

	private void ShowKeys()
	{
		UnlockButtonsContainer.SetActive(value: false);
		KeysContainer.SetActive(value: true);
		TweenManager.PlayTweenGroup(KeysContainer, 10);
	}

	private void HideKeysShowBuyMore()
	{
		state = State.GetMoreKeysShown;
		if (KeysContainer.activeSelf)
		{
			TweenManager.PlayTweenGroup(KeysContainer, 11, forward: true, ShowBuyMorePanel);
		}
		else
		{
			ShowBuyMorePanel();
		}
	}

	private void ShowBuyMorePanel()
	{
		KeysContainer.SetActive(value: false);
		if (NeedShowBuyMorePanel())
		{
			if (UseLootKeysFeature)
			{
				UnlockButtonsContainer.SetActive(value: true);
				TweenManager.PlayTweenGroup(UnlockButtonsContainer, 1);
			}
			else
			{
				UnlockButtonsContainerOld.SetActive(value: true);
				TweenManager.PlayTweenGroup(UnlockButtonsContainerOld, 1);
			}
		}
	}

	private bool CanBuyMore()
	{
		if (GetBuyMorePrice() <= GameManager.Instance.playerModel.GetCurrency(CurrencyType.Diamonds).Value)
		{
			return GetKeysLeft() < GetUnopenedRewardsLeft();
		}
		return false;
	}

	public void OnBuyMore()
	{
		if (state != State.GetMoreKeysShown || GetKeysLeft() >= GetUnopenedRewardsLeft())
		{
			return;
		}
		CampHUD.Get().PauseCurrencyMeters = false;
		if (CanBuyMore())
		{
			EventManager.NotifyClick("BuyMore");
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("reward_screen/buy_more_rewards");
			if (Helpers.ExecuteCommand(new BuyMoreRewardsCommand
			{
				BuyWithDiamonds = true
			}) == TWDModelResult.OK)
			{
				UpdateUI();
			}
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(GetBuyMorePrice());
		}
	}

	private bool NeedShowBuyMorePanel()
	{
		if (TutorialView.Instance.Model.MissionHasFakeMissionRewards())
		{
			return TutorialView.Instance.Model.CurrentPartId == "RewardsScreen3";
		}
		return true;
	}

	private int GetBuyMorePrice()
	{
		if (TutorialView.Instance.Model.CurrentPartId == "RewardsScreen3")
		{
			return 0;
		}
		if (GameManager.Instance.playerModel.ActivityManager.TryGetActivityParam(ActivityType.Jackpot, out var activityParams))
		{
			return int.Parse(activityParams[2]);
		}
		return GameManager.Instance.gameEconomyData.ConfigData.ThreeRewardsCost;
	}

	private int GetBuyMorePriceLootKeys()
	{
		return GameManager.Instance.gameEconomyData.ConfigData.ThreeRewardsLootKeyCost;
	}

	private void UpdateRewardsCount()
	{
		int keysLeft = GetKeysLeft();
		Key1Container.SetActive(keysLeft > 0);
		Key2Container.SetActive(keysLeft > 1);
		Key3Container.SetActive(keysLeft > 2);
	}

	private void OnRewardBoxClicked(GameObject box, LootEntry reward, LootEntry reward2)
	{
		if (reward2 != null)
		{
			Debug.LogError("CombatEndFlowThreeByThree supports single reward boxes only (but non-null reward2 parameter was provided).");
		}
		UpdateRewardsCount();
	}

	private void OnRewardBoxOpened(GameObject box, LootEntry reward, LootEntry reward2)
	{
		if (reward2 != null)
		{
			Debug.LogError("CombatEndFlowThreeByThree supports single reward boxes only (but non-null reward2 parameter was provided).");
		}
		RewardScreenHandler.Instance.CreateLootCard(box, reward, base.transform, RewardScreenHandler.LootCardPlacement.ThreeByThree);
		if (reward.DropType != DropType.Regular)
		{
			OpenSpecialLoot(reward.DropType);
		}
	}

	private void OnRewardBoxOpenedOver()
	{
		StartCoroutine(DelayShowButtonAfterRewards());
	}

	private IEnumerator DelayShowButtonAfterRewards()
	{
		yield return new WaitForSeconds(1.8f);
		SetReturnToCamp();
		UpdateBuyButton();
	}

	public void OnReturnToCamp()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("reward_screen/return_to_map");
		GameManager.Instance.ReturnFromVisit();
		base.gameObject.SetActive(value: false);
	}

	private void SetReturnToCamp()
	{
		bool flag = GetKeysLeft() == 0 || GetUnopenedRewardsLeft() == 0;
		if (!(ReturnToCampObject != null))
		{
			return;
		}
		if (TutorialView.Instance.Model.CurrentPartId != null)
		{
			ReturnToCampObject.SetActive(value: false);
			if (flag)
			{
				EventManager.NotifyClick("CanReturnToCamp");
			}
		}
		else
		{
			shareButton.gameObject.SetActive(flag);
			ReturnToCampObject.SetActive(flag);
		}
	}

	private void OpenSpecialLoot(DropType type)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("reward_screen/found_on_mission");
		foreach (SpecialRewardIcon specialLootIcon in SpecialLootIcons)
		{
			switch (type)
			{
			case DropType.Gold:
				if (specialLootIcon.State == SpecialRewardIconState.Gold)
				{
					specialLootIcon.Open();
					return;
				}
				break;
			case DropType.Silver:
				if (specialLootIcon.State == SpecialRewardIconState.Silver)
				{
					specialLootIcon.Open();
					return;
				}
				break;
			}
		}
	}

	private bool CanBuyMoreWithKeys()
	{
		if (GetBuyMorePriceLootKeys() <= GameManager.Instance.playerModel.GetCurrency(CurrencyType.LootKeys).Value)
		{
			return GetKeysLeft() < GetUnopenedRewardsLeft();
		}
		return false;
	}

	public void OnBuyMoreWithKeys()
	{
		if (state != State.GetMoreKeysShown || GetKeysLeft() >= GetUnopenedRewardsLeft())
		{
			return;
		}
		CampHUD.Get().PauseCurrencyMeters = false;
		if (CanBuyMoreWithKeys())
		{
			EventManager.NotifyClick("BuyMore");
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("reward_screen/buy_more_rewards");
			if (Helpers.ExecuteCommand(new BuyMoreRewardsCommand
			{
				BuyWithDiamonds = false,
				BuyWithKeys = true
			}) == TWDModelResult.OK)
			{
				UpdateUI();
			}
		}
	}

	public void OnWatchAdd()
	{
		if (isAdPlaying && Time.time > adPlayStartTime + 60f)
		{
			isAdPlaying = false;
		}
		if (!isAdPlaying && GetKeysLeft() <= 0)
		{
			if (SingularityMonoBehaviour<VideoAdManager>.Instance.IsPlaying)
			{
				HUDNotification.Error(LocalizationManager.GetText("Error.AdShowFailed"));
				SingularityMonoBehaviour<VideoAdManager>.Instance.CancelAd(AdUsage.CombatRewardKey);
				UpdateUI();
			}
			else if (GameManager.Instance.HasAnsweredTargetedAdsConsentQuestion())
			{
				StartPlayingAd();
			}
			else
			{
				GameManager.Instance.AskForAdConsent(AdUsage.CombatRewardKey, StartPlayingAd, UpdateUI);
			}
		}
	}

	public void StartPlayingAd()
	{
		isAdPlaying = true;
		adPlayStartTime = Time.time;
		SingularityMonoBehaviour<VideoAdManager>.Instance.FadeOutAudio();
		SingularityMonoBehaviour<VideoAdManager>.Instance.PlayAd(AdUsage.CombatRewardKey);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		StartCoroutine(DelayShowWatchAnotherButton());
	}

	public void OnWatchAnotherAd()
	{
		isAdPlaying = false;
		SingularityMonoBehaviour<VideoAdManager>.Instance.CancelAd(AdUsage.CombatRewardKey);
		UpdateUI();
	}

	private void OnEvent(EventManager.EventType eventtype, object parameter)
	{
		switch (eventtype)
		{
		case EventManager.EventType.VideoWatched:
			OnVideoWatched((bool)parameter);
			return;
		case EventManager.EventType.TutorialEvent:
			if (parameter.ToString() == "ShowBuyMore")
			{
				HideKeysShowBuyMore();
				return;
			}
			break;
		}
		if (eventtype == EventManager.EventType.TutorialPartOver)
		{
			UpdateUI();
		}
	}

	public void OnVideoWatched(bool completely)
	{
		if (!isAdPlaying)
		{
			Debug.LogError("Received OnVideoWatched even though not playing.");
			return;
		}
		isAdPlaying = false;
		if (!completely && !SingularityMonoBehaviour<VideoAdManager>.Instance.IsAdAvailable(AdUsage.CombatRewardKey))
		{
			UpdateUI();
		}
		else
		{
			AdRewardPlayer();
		}
	}

	private void AdRewardPlayer()
	{
		isAdPlaying = false;
		if (GameManager.Instance.playerModel.Combat != null)
		{
			GameManager.Instance.playerModel.Combat.VideoAdsServedInRewardScreen++;
		}
		Helpers.ExecuteCommand(new BuyMoreRewardsCommand
		{
			BuyWithDiamonds = false,
			BuyWithKeys = false
		});
		StartWaitCommandQueueCoroutine();
	}

	private void StartWaitCommandQueueCoroutine()
	{
		if (waitCommandCoroutine != null)
		{
			StopCoroutine(waitCommandCoroutine);
		}
		waitCommandCoroutine = StartCoroutine(WaitCommandQueueToContinue());
	}

	private IEnumerator WaitCommandQueueToContinue()
	{
		IngameLoading ingameLoading = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading) as IngameLoading;
		if (ingameLoading != null)
		{
			ingameLoading.isShowLootCard = true;
			ingameLoading.Open();
		}
		while (SignalRClient.Instance.IsWaitingForResponse)
		{
			yield return null;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		UpdateUI();
		waitCommandCoroutine = null;
	}

	private void OnWaitCommandQueueDone()
	{
	}

	private void ShowUiForScreenshot(bool show)
	{
		sharePanel.SetActive(show);
		UnlockButtonsContainer.transform.parent.gameObject.SetActive(!show);
	}

	public void OnClickShare()
	{
		StartCoroutine(GetComponent<ScreenshotShare>().TakeScreenshot("Survivor", shareButton, shareBadge, ShowUiForScreenshot));
	}

	private string FormatTimeLeftUntilKeyRefresh(long timeLeft)
	{
		string text = Helpers.FormatTime(timeLeft);
		if (timeLeft <= 0)
		{
			return "";
		}
		return LocalizationManager.GetText("Popup.CombatEndScreen.KeyRefreshTime{0}", text);
	}
}
