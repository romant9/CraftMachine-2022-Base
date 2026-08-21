using System;
using System.Collections;
using System.Collections.Generic;
using BaseModel;
using Client.Tweener;
using TWDModel;
using UnityEngine;
using System.Linq;

public class SelectSurvivorsPopup : HUDElement
{
	public enum SelectedRewardType
	{
		None = 0,
		Survivor = 1,
		ClassToken = 2,
		HeroToken = 3
	}

	private class CollectAllTarget
	{
		public RadioCallCardBase Card;

		public SelectedRewardType RewardType;

		public CurrencyType TokenCurrency;

		public int TokenAmount;
	}

	private bool OpenedAfterReroll;

	[Header("Dynamically Loaded Content")]
	[SerializeField]
	public Transform ButtonParentTarget;

	[SerializeField]
	private GameObject SurvivorCardPrefab;

	[SerializeField]
	private GameObject TokenCardPrefab;

	[SerializeField]
	private GameObject CardEffectPrefab;

	[SerializeField]
	private GameObject collectAnimationPrefab;

	[SerializeField]
	private UILabel Title;

	[SerializeField]
	private GameObject RevealedCardEffect;

	[Header("Delays and Times")]
	[Tooltip("How long we wait for user input before the cards stopped/clicked automatically")]
	[SerializeField]
	private float InputWaitTime = 6f;

	[Tooltip("The close popup wait time for survivor.")]
	[SerializeField]
	private float ClosePopupWaitTimeSurvivor;

	[Tooltip("The close popup wait time for class token.")]
	[SerializeField]
	private float ClosePopupWaitTimeClassToken;

	[Tooltip("The close popup wait time for hero token.")]
	[SerializeField]
	private float ClosePopupWaitTimeHeroToken;

	[Tooltip("Delay between the start if the card animations.")]
	[SerializeField]
	private float AnimationsDelay;

	[Header("Positions and Offsets")]
	[Tooltip("How much should the cards offse when the animation is done.")]
	[SerializeField]
	private Vector3 SidewaysOffset = new Vector3(-130f, 0f, 0f);

	[Header("Internal Panels")]
	[SerializeField]
	public PhoneManagePanel ManagePanel;

	[SerializeField]
	private FakeTrainingGroundsHudButton fakeHudButton;

	[SerializeField]
	private GameObject skipButton;

	private SelectedRewardType selectedRewardType;

	private int LatestSelectedRewardIndex = -1;

	private LootEntry SelectedLootEntry;

	public List<RadioCallCardBase> CardsList = new List<RadioCallCardBase>();

	private int CardIntroCompletedCount;

	private bool AutoClickDone;

	private float ClickWaitTime;

	private float CloseWaitTime;

	private bool closeOnCurrencyAnimationComplete = true;

	private bool returnToSelectSurvivor = true;

	private bool collectAllInProgress;

	private int collectAllFlyingCards;

	public bool SkipClickIntro { get; set; }

	public int RerollsLeft { get; set; }

	public void UpdateTitleText()
	{
		if (Title != null)
		{
			if (RerollsLeft > 0)
			{
				Title.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SelectSurvivor.SelectCardsToKeep");
			}
			else if (GameManager.Instance.playerModel.PhoneCall.NumLootChoosable > 1)
			{
				Title.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SelectSurvivor.ChooseMultipleTitle");
			}
			else
			{
				Title.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.SelectSurvivor.Title");
			}
		}
	}

	public override void Open()
	{
		RerollIndex = 0;

		OpenedAfterReroll = false;
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			PhoneCallModel phoneCall = GameManager.Instance.playerModel.PhoneCall;
			if (phoneCall == null || phoneCall.LootsList == null)
			{
				DebugTWD.LogError("Could not open Data was NULL!");
				RerollsLeft = 0;
				return;
			}
			RerollsLeft = phoneCall.NumRerolls;
		}
		else
		{
			base.Open();
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup);
			if (GameManager.Instance == null || GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.PhoneCall == null || GameManager.Instance.playerModel.PhoneCall.LootsList == null)
			{
				DebugLogError("Could not open Data was NULL!");
				return;
			}
			RerollsLeft = GameManager.Instance.playerModel.PhoneCall.NumRerolls;
		}
		UpdateTitleText();
		if (ManagePanel != null)
		{
			if (IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				ManagePanel.Init();
			}
			else
			{
				ManagePanel.SetClickManageCallback(OnClickManagement);
				ManagePanel.SetClickBuySlotsCallback(OnClickBuySlots);
			}
			ManagePanel.SetClickRerollCallback(OnClickReroll);
			ManagePanel.SetClickCollectAllCallback(OnClickCollectAll);
			UpdateManagePanel();
		}
		if (GameManager.Instance.PhoneCallResponseReceived || IsLoadDataManager)
		{
			DebugTWD.LogMycode("...|| IsLoadDataManager)");
			InstantiateButtons(GameManager.Instance.playerModel.PhoneCall);
			PositionCards();
			if (!OfflineManager.IsNoEffects)
			{
				for (int i = 0; i < CardsList.Count; i++)
				{
					if (CardsList[i] != null)
					{
						CardsList[i].AnimateIntro();
					}
				}
				ClickWaitTime = Time.time + InputWaitTime;
			}
		}
	}

	public void ReOpenAfterReroll()
	{
		collectAllInProgress = false;
		collectAllFlyingCards = 0;
		if (!IsLoadDataManager) HideSidePanels();
		OpenedAfterReroll = true;
		if (CardsList != null)
		{
			for (int i = 0; i < CardsList.Count; i++)
			{
				if (CardsList[i] != null)
				{
					CardsList[i].Clear();
				}
			}
			CardsList = new List<RadioCallCardBase>();
		}

		if (ButtonParentTarget != null && ButtonParentTarget.gameObject != null)
		{
			Helpers.DestroyAllChildren(ButtonParentTarget.gameObject);
		}
		UpdateTitleText();
		if (IsSelectedMode && OfflineManager.IsLoadDataManager)
		{
			IsSelectedMode = false;
			InstantiateButtonsForSelected();
		}
		else
		{
			InstantiateButtons(null);
		}
		PositionCards();
		CardIntroCompletedCount = 0;
	}

	public override void Update()
	{
		base.Update();
		if (IsLoadDataManager) return;

		if (!AutoClickDone && ClickWaitTime != 0f && ClickWaitTime < Time.time)
		{
			AutoClickDone = true;
			StartCoroutine(AutoClickEffect(AnimationsDelay));
		}
	}

	public void ExitBackToRadioPhone()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup).Open();
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallDone();
		}
		Close();
	}

	public void ExitToCamp()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.NewRadioPopup);
		if (noCreation != null)
		{
			noCreation.Close();
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallDone();
		}
		Close();
	}

	private void RestoreCampBeforeCloseAnim()
	{
		if (CampManager.Instance != null)
		{
			CampManager.Instance.FullscreenPopupShowCamp(SingularityMonoBehaviour<HUDManager>.Instance.CanEnableCamp(UIType.RadioSelectSurvivorPopup));
		}
	}

	public override void Close()
	{
		if (!IsLoadDataManager) RestoreCampBeforeCloseAnim();
		Clear();
		base.Close();
	}

	public void ShowSidePanels(bool skipTween = false)
	{
		if (ManagePanel != null)
		{
			ManagePanel.Show(skipTween);
		}
	}

	public void HideSidePanels()
	{
		if (ManagePanel != null)
		{
			ManagePanel.Hide();
		}
	}

	public void Clear()
	{
		DebugTWD.Log("Clear data for SelectSurvivorPopup", DebugType.Call);

		if (ManagePanel != null)
		{
			ManagePanel.Clear();
		}
		if (CardsList == null)
		{
			return;
		}
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i] != null)
			{
				CardsList[i].Clear();
			}
		}
		CardsList = new List<RadioCallCardBase>();
	}

	public override void OnClickClose()
	{
		base.OnClickClose();
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallDone();
		}
	}

	public void SelectLootEntry(LootEntry selectedLootEntry)
	{
		if (SelectedLootEntry != selectedLootEntry)
		{
			SelectedLootEntry = selectedLootEntry;
			int num = UpdateCardUIStateWithModel(SelectedLootEntry);
			if (num > -1)
			{
				LatestSelectedRewardIndex = num;
			}
		}
	}

	private void OnClickAcceptSelectedLoot(string uiEvent, object parameter)
	{
		bool flag = IsAllLootClaimable();
		LootEntry lootEntry = SelectedLootEntry;
		if (flag)
		{
			if (!(parameter is int))
			{
				Debug.LogError("OnClickAcceptSelectedLoot called without loot index parameter (paramete required when IsAllLootSelectable is true).");
				return;
			}
			lootEntry = null;
			int num = (int)parameter;
			if (num >= 0 && num < GameManager.Instance.playerModel.PhoneCall.LootsList.Count)
			{
				lootEntry = (SelectedLootEntry = GameManager.Instance.playerModel.PhoneCall.LootsList[num]);
				LatestSelectedRewardIndex = num;
			}
			else
			{
				Debug.LogError("OnClickAcceptSelectedLoot called with loot index parameter that is out of bounds.");
			}
		}
		if (lootEntry == null)
		{
			return;
		}
		if (uiEvent == "OnAcceptSelectedLootEntrySurvivor")
		{
			if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
			{
				AcceptSurvivor(lootEntry.GeneratedSurvivor);
			}
		}
		else if (uiEvent == "OnAcceptSelectedLootEntryTokens")
		{
			if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
			{
				AcceptClassTokens(lootEntry.GeneratedSurvivor);
			}
			else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
			{
				AcceptHeroTokens(lootEntry);
			}
		}
		if (flag)
		{
			UpdateManagePanel();
			StartSingleCardCollectAnimation();
		}
		else
		{
			StartOutroAnimation();
		}
	}

	private void UpdateLockingState(int lootIndex)
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			CardsList[i].UpdateUI();
		}
		if (ManagePanel != null)
		{
			ManagePanel.UpdateRerollButtonLabel();
		}
	}

	public void OnClickLockLoot(int lootIndex)
	{
		if ((IsLoadDataManager ? TWDModelResultReroll(lootIndex, locked: true) : Helpers.ExecuteCommand(new LockPhoneCallCardForRerollCommand
		{
			PhoneCallLootIndex = lootIndex,
			Locked = true
		})) == TWDModelResult.OK)
		{
			if (IsLoadDataManager)
			{
				CallCraft.Instance.CurrentCall.AcceptIndexes[RerollIndex][lootIndex].Set(true);
				DebugTWD.LogMycode("if ((IsLoadDataManager ?");
			}
			UpdateLockingState(lootIndex);
		}
	}

	private void OnClickUnlockLoot(int lootIndex)
	{
		if ((IsLoadDataManager ? TWDModelResultReroll(lootIndex, locked: false) : Helpers.ExecuteCommand(new LockPhoneCallCardForRerollCommand
		{
			PhoneCallLootIndex = lootIndex,
			Locked = false
		})) == TWDModelResult.OK)
		{
			DebugTWD.LogMycode("if ((IsLoadDataManager ?");
			UpdateLockingState(lootIndex);
		}
	}

	private void AcceptSurvivor(SurvivorModel survivorModel)
	{
		selectedRewardType = SelectedRewardType.Survivor;
		CloseWaitTime = ClosePopupWaitTimeSurvivor;
		if (survivorModel != null)
		{
			Helpers.ExecuteCommand(new AcceptSurvivorCommand(survivorModel, NewSurvivorSource.Phone));
			EventManager.NotifyClick("AcceptSurvivor");
			EventManager.NotifyEvent(EventManager.EventType.AcceptSurvivor);
		}
		else
		{
			Debug.LogError("Cannot accept NULL survivorModel");
		}
	}

	private TWDModelResult AcceptHeroTokens(LootEntry entry)
	{
		selectedRewardType = SelectedRewardType.HeroToken;
		CloseWaitTime = ClosePopupWaitTimeHeroToken;
		TWDModelResult tWDModelResult = TWDModelResult.Error;
		if (entry != null)
		{
			tWDModelResult = Helpers.ExecuteCommand(new AcceptSurvivorTokenCommand(entry));
			if (tWDModelResult == TWDModelResult.OK)
			{
				string heroId = SurvivorToken.GetHeroId(entry.RewardedCurrency);
				ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(heroId);
				bool flag = GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId);
				if (actorDefinition != null && GameManager.Instance.playerModel.GetCurrency(entry.RewardedCurrency).Value >= actorDefinition.TokensToUnlock && !flag)
				{
					closeOnCurrencyAnimationComplete = false;
				}
			}
			EventManager.NotifyEvent(EventManager.EventType.AcceptHeroTokens);
		}
		else
		{
			Debug.LogError("Cannot accept NULL entry");
		}
		return tWDModelResult;
	}

	private TWDModelResult AcceptClassTokens(SurvivorModel survivorModel)
	{
		selectedRewardType = SelectedRewardType.ClassToken;
		CloseWaitTime = ClosePopupWaitTimeClassToken;
		TWDModelResult result = TWDModelResult.Error;
		if (survivorModel != null)
		{
			result = Helpers.ExecuteCommand(new RejectSurvivorCommand(survivorModel, NewSurvivorSource.Phone));
			EventManager.NotifyEvent(EventManager.EventType.RejectSurvivor);
		}
		else
		{
			Debug.LogError("Cannot accept NULL survivorModel");
		}
		return result;
	}

	private void UpdateSidePanelInfo()
	{
		if (ManagePanel != null)
		{
			ManagePanel.UpdateUI();
		}
	}

	private void DisableSidePanelsButtons()
	{
		if (ManagePanel != null)
		{
			ManagePanel.DisableButtons();
		}
	}

	private void EnableSidePanelsButtons()
	{
		if (ManagePanel != null)
		{
			ManagePanel.EnableButtons();
		}
	}

	private void StartSingleCardCollectAnimation()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DisableSidePanelsButtons();
			LockAllCards(value: true);
			SelectLootEntry(null);
		}
		else
		{
			HideAllAnimations();
			DisableSidePanelsButtons();
			LockAllCards(value: true);
			if (fakeHudButton != null && selectedRewardType != SelectedRewardType.Survivor)
			{
				fakeHudButton.Init(SelectedLootEntry, selectedRewardType);
			}
			TweenChildScaleOnly(LatestSelectedRewardIndex, -1f, 1f, OutroTweenInPlaceComplete);
			if (RevealedCardEffect != null)
			{
				Helpers.InstantiateToParentAndLayer(RevealedCardEffect, CardsList[LatestSelectedRewardIndex].gameObject);
			}
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_move");
		}
	}

	private void StartOutroAnimation()
	{
		HideAllRewardCards(SelectedLootEntry);
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DisableSidePanelsButtons();
			LockAllCards(value: true);
			SelectLootEntry(null);
			if (!CallCraft.Instance.IsMultiCallMode)
			{
				TweenChildTo(LatestSelectedRewardIndex, Vector3.one, -1f, .5f, null);
			}
		}
		else
		{
			HideAllAnimations();
			HideSidePanels();
			LockAllCards(value: true);
			if (fakeHudButton != null && selectedRewardType != SelectedRewardType.Survivor)
			{
				fakeHudButton.Init(SelectedLootEntry, selectedRewardType);
			}
			TweenChildTo(LatestSelectedRewardIndex, Vector3.one, -1f, 1f, OutroTweenCenterComplete);
			if (RevealedCardEffect != null)
			{
				Helpers.InstantiateToParentAndLayer(RevealedCardEffect, CardsList[LatestSelectedRewardIndex].gameObject);
			}
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_move");
		}
	}

	private void OutroTweenInPlaceComplete()
	{
		if (fakeHudButton != null && selectedRewardType != SelectedRewardType.Survivor)
		{
			fakeHudButton.ShowCollect();
		}
		SelectionDone(SelectedLootEntry, CollectSingleCardInPlaceAnimationComplete, animate: !OfflineManager.IsNoEffects, doInPlaceAnimation: true);
		SelectLootEntry(null);
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_zoom");
		}
	}

	private void OutroTweenCenterComplete()
	{
		if (fakeHudButton != null)
		{
			fakeHudButton.ShowCollect();
		}
		if (selectedRewardType == SelectedRewardType.Survivor || selectedRewardType == SelectedRewardType.ClassToken)
		{
			SelectionDone(SelectedLootEntry, CollectAnimationComplete, animate: !OfflineManager.IsNoEffects, doInPlaceAnimation: false);
			SelectLootEntry(null);
		}
		else
		{
			if (IsLoadDataManager)
			{
				DebugTWD.LogMycode("if (IsLoadDataManager)");
				SelectionDone(SelectedLootEntry, CollectAnimationComplete, animate: false, doInPlaceAnimation: false);
				SelectLootEntry(null);
			}
			else
			{
				SelectionDone(SelectedLootEntry, null, animate: false, doInPlaceAnimation: false);
				SelectLootEntry(null);
				TweenChildTo(LatestSelectedRewardIndex, Vector3.one, 1.5f, 0.5f, OutroAnimationComplete);
			}
		}
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_zoom");
		}
	}

	private void CollectSingleCardInPlaceAnimationComplete(RadioCallCardBase cardBase)
	{
		OutroAnimationCompleteImpl(pickedSingleLootOfMany: true);
	}

	private void CollectAnimationComplete(RadioCallCardBase cardBase)
	{
		OutroAnimationComplete();
	}

	private void OutroAnimationComplete()
	{
		OutroAnimationCompleteImpl(pickedSingleLootOfMany: false);
	}

	private void OutroAnimationCompleteImpl(bool pickedSingleLootOfMany)
	{
		RadioCallCardBase cardByIndex = GetCardByIndex(LatestSelectedRewardIndex);
		if (cardByIndex != null)
		{
			CreateCollectAnimation(cardByIndex, fakeHudButton.GetIconTarget(), pickedSingleLootOfMany);
		}
		bool flag = !pickedSingleLootOfMany || GameManager.Instance.playerModel.PhoneCall.LootsList.Count == 0;
		if (flag && SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallDone();
		}
	}

	private void CreateCollectAnimation(RadioCallCardBase from, GameObject to, bool pickedSingleLootOfMany)
	{
		if (selectedRewardType == SelectedRewardType.ClassToken || selectedRewardType == SelectedRewardType.HeroToken)
		{
			int rewardAmount = fakeHudButton.GetRewardAmount();
			int b = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 10 : 20);
			int num = Mathf.Min(rewardAmount, b);
			if (!(from != null) || !(to != null) || !(fakeHudButton != null))
			{
				return;
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
					component.FollowTarget(from.gameObject);
				}
				bool isFirst = i == 0;
				if (pickedSingleLootOfMany)
				{
					component.StartAnimation(rewardAmount, fakeHudButton.GetCurrencyType(), to.transform, CurrencyAnimCompleteSingleOfMany, isFirst);
				}
				else
				{
					component.StartAnimation(rewardAmount, fakeHudButton.GetCurrencyType(), to.transform, CurrencyAnimCompleteOne, isFirst);
				}
			}
		}
		else
		{
			CurrencyAnimCompleteImpl(isComplete: true, CurrencyType.None, pickedSingleLootOfMany);
		}
	}

	private void CurrencyAnimCompleteSingleOfMany(bool isComplete, CurrencyType currencyType)
	{
		if (fakeHudButton != null)
		{
			fakeHudButton.HideCollect();
		}
		CurrencyAnimCompleteImpl(isComplete, currencyType, pickedSingleLootOfMany: true);
	}

	private void CurrencyAnimCompleteOne(bool isComplete, CurrencyType currencyType)
	{
		CurrencyAnimCompleteImpl(isComplete, currencyType, pickedSingleLootOfMany: false);
	}

	private void CurrencyAnimCompleteImpl(bool isComplete, CurrencyType currencyType, bool pickedSingleLootOfMany)
	{
		bool flag;
		bool flag2;
		if (pickedSingleLootOfMany)
		{
			flag = GameManager.Instance.playerModel.PhoneCall.LootsList.Count == 0;
			flag2 = GameManager.Instance.gameEconomyData.ConfigData.EnableHeroUnlockInMultiCardCall;
			if (!flag)
			{
				UpdateSidePanelInfo();
				EnableSidePanelsButtons();
				LockAllCards(value: false);
			}
		}
		else
		{
			flag = true;
			flag2 = true;
		}
		if (flag)
		{
			UpdateManagePanel();
			if (closeOnCurrencyAnimationComplete || !flag2)
			{
				StartCoroutine(DelayedClose(CloseWaitTime));
			}
		}
	}

	private IEnumerator DelayedClose(float delay)
	{
		yield return new WaitForSeconds(delay);
		ExitBackToRadioPhone();
	}

	private void OnClickManagement(UIButtonExtended button)
	{
		DebugLog("OnClickManagement");
		SelectLootEntry(null);
		Close();
		SurvivorManagementPopUp obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds) as SurvivorManagementPopUp;
		obj.OnCloseCallback = (Callback)Delegate.Remove(obj.OnCloseCallback, new Callback(OnSurvivorManagementClose));
		obj.OnCloseCallback = (Callback)Delegate.Combine(obj.OnCloseCallback, new Callback(OnSurvivorManagementClose));
		obj.IsAcceptingSurvivor = true;
		obj.Open();
	}

	private void OnClickBuySlots(UIButtonExtended button)
	{
		DebugLog("OnClickBuySlots");
		if (ManagePanel != null && ManagePanel.SlotsCashier != null)
		{
			ConsumeCurrencyCommandUtils.Execute(new BuyMoreSurvivorSlotsCommand
			{
				Cashier = ManagePanel.SlotsCashier
			}, BuyMoreSlotsCallback);
		}
	}

	private void BuyMoreSlotsCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			UIEvent.Send("SurvivorExtraSlotBought");
			if (ManagePanel != null)
			{
				ManagePanel.UpdateUI();
				ManagePanel.UpdateBuySlotsUIState();
			}
		}
	}

	private void OnRerollMade()
	{
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallStarted();
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel.PhoneCall.LootsList == null)
		{
			return;
		}
		if (!IsLoadDataManager)
		{
			for (int i = 0; i < playerModel.PhoneCall.LootsList.Count; i++)
			{
				if (playerModel.PhoneCall.LootsList[i] != null && playerModel.PhoneCall.LootsList[i].GeneratedSurvivor != null)
				{
					ActorView.PrepareActor(playerModel.PhoneCall.LootsList[i].GeneratedSurvivor);
				}
			}
		}
	}

	private void OnClickReroll(UIButtonExtended button)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.Log("OnClickReroll", DebugType.OnClick);
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			if (RerollIndex < CallCraft.Instance.CurrentCall.AcceptIndexesGroups.Count) CallCraft.Instance.CurrentCall.AcceptIndexesGroups[RerollIndex].gameObject.SetActive(true);
		}
		else
		{
			DebugLog("OnClickReroll");
		}
		SelectLootEntry(null);
		GameManager.Instance.PhoneCallResponseReceived = false;
		if ((IsLoadDataManager ? OnClickRerollResult() : Helpers.ExecuteCommand(new RerollPhoneCallCommand())) == TWDModelResult.OK)
		{
			CallCraft.Instance.CalculateHeroTokenQueue();
			OnRerollMade();
			RerollsLeft = GameManager.Instance.playerModel.PhoneCall.NumRerolls;
			UpdateManagePanel();
			ReOpenAfterReroll();
			if (IsLoadDataManager) RerollIndex++;
		}
	}

	private void OnClickCollectAll(UIButtonExtended button)
	{
		if (collectAllInProgress)
		{
			return;
		}
		PhoneCallModel phoneCallModel = GameManager.Instance.playerModel?.PhoneCall;
		ModelList<LootEntry> modelList = phoneCallModel?.LootsList;
		if (phoneCallModel == null || modelList == null)
		{
			return;
		}
		if (!phoneCallModel.CanClaimEntireMultiLootsList())
		{
			Debug.LogError("OnClickCollectAll called in non multi-loot claim mode.");
			return;
		}
		List<CollectAllTarget> list = new List<CollectAllTarget>();
		for (int i = 0; i < modelList.Count; i++)
		{
			LootEntry lootEntry = modelList[i];
			if (lootEntry == null || phoneCallModel.IsLootClaimed(i))
			{
				continue;
			}
			RadioCallCardBase cardByIndex = GetCardByIndex(i);
			if (cardByIndex == null)
			{
				continue;
			}
			SelectedRewardType selectedRewardType;
			if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
			{
				if (lootEntry.GeneratedSurvivor == null)
				{
					continue;
				}
				selectedRewardType = SelectedRewardType.ClassToken;
			}
			else
			{
				if (lootEntry.DropCurrencyType != DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
				{
					continue;
				}
				selectedRewardType = SelectedRewardType.HeroToken;
			}
			if (((selectedRewardType == SelectedRewardType.ClassToken) ? AcceptClassTokens(lootEntry.GeneratedSurvivor) : AcceptHeroTokens(lootEntry)) == TWDModelResult.OK)
			{
				GetLootTokenReward(lootEntry, selectedRewardType, out var currencyType, out var amount);
				list.Add(new CollectAllTarget
				{
					Card = cardByIndex,
					RewardType = selectedRewardType,
					TokenCurrency = currencyType,
					TokenAmount = amount
				});
				if (list.Count == 1 && fakeHudButton != null)
				{
					fakeHudButton.Init(lootEntry, selectedRewardType);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		collectAllInProgress = true;
		collectAllFlyingCards = list.Count;
		UpdateManagePanel();
		DisableSidePanelsButtons();
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_move");
		}
		for (int j = 0; j < list.Count; j++)
		{
			CollectAllTarget target = list[j];
			target.Card.HideAnimation();
			target.Card.SetCardLocked(value: true, introAnimationLock: false);
			if (RevealedCardEffect != null)
			{
				Helpers.InstantiateToParentAndLayer(RevealedCardEffect, target.Card.gameObject);
			}
			bool leading = j == 0;
			target.Card.TweenToPosition(target.Card.transform.localPosition, Vector3.one * -1f, 1f, delegate
			{
				CollectAllCardTweenComplete(target, leading);
			});
		}
	}

	private void CollectAllCardTweenComplete(CollectAllTarget target, bool leading)
	{
		if (leading)
		{
			if (fakeHudButton != null)
			{
				fakeHudButton.ShowCollect();
			}
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/card_zoom");
			}
		}
		target.Card.CollectCard(delegate
		{
			CollectAllCardCollected(target);
		}, target.RewardType, animate: true, doInPlaceAnimation: true);
	}

	private void CollectAllCardCollected(CollectAllTarget target)
	{
		GameObject gameObject = ((fakeHudButton != null) ? fakeHudButton.GetIconTarget() : null);
		if (gameObject == null || target.TokenAmount <= 0 || target.TokenCurrency == CurrencyType.None)
		{
			CollectAllCardFlyComplete();
			return;
		}
		int b = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 10 : 20);
		int num = Mathf.Min(target.TokenAmount, b);
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/collect_token");
		}
		bool reported = false;
		AnimComplete animComplete = delegate
		{
			if (!reported)
			{
				reported = true;
				CollectAllCardFlyComplete();
			}
		};
		for (int num2 = 0; num2 < num; num2++)
		{
			CollectAnimation component = Helpers.InstantiateToParentAndLayer(collectAnimationPrefab, base.gameObject).GetComponent<CollectAnimation>();
			if (!(component == null))
			{
				component.FollowTarget(target.Card.gameObject);
				component.StartAnimation(target.TokenAmount, target.TokenCurrency, gameObject.transform, animComplete, num2 == 0);
			}
		}
	}

	private void CollectAllCardFlyComplete()
	{
		collectAllFlyingCards--;
		if (collectAllFlyingCards <= 0)
		{
			collectAllInProgress = false;
			if (fakeHudButton != null)
			{
				fakeHudButton.HideCollect();
			}
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallDone();
			}
			bool enableHeroUnlockInMultiCardCall = GameManager.Instance.gameEconomyData.ConfigData.EnableHeroUnlockInMultiCardCall;
			if (closeOnCurrencyAnimationComplete || !enableHeroUnlockInMultiCardCall)
			{
				StartCoroutine(DelayedClose(CloseWaitTime));
			}
		}
	}

	private void GetLootTokenReward(LootEntry entry, SelectedRewardType rewardType, out CurrencyType currencyType, out int amount)
	{
		currencyType = CurrencyType.None;
		amount = 0;
		if (entry == null)
		{
			return;
		}
		switch (rewardType)
		{
		case SelectedRewardType.HeroToken:
			currencyType = entry.RewardedCurrency;
			amount = entry.RewardedAmount;
			break;
		case SelectedRewardType.ClassToken:
			if (entry.GeneratedSurvivor != null)
			{
				currencyType = SurvivorToken.GetClassAsCurrency(entry.GeneratedSurvivor.SurvivorClass);
				amount = entry.GeneratedSurvivor.GetDemoteCashier().GetTotalCost(currencyType);
			}
			break;
		}
	}

	public void OnClickSkipButton()
	{
		Helpers.GameObjectSetActive(skipButton, value: false);
		AutoClickDone = true;
		StartCoroutine(AutoClickEffect(AnimationsDelay));
	}

	private void OnSurvivorInfoOpen()
	{
		DebugLog("OnClickManagement");
		Close();
		SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
		if (survivorInfoPopup != null && returnToSelectSurvivor)
		{
			survivorInfoPopup.OnCloseCallback = (Callback)Delegate.Remove(survivorInfoPopup.OnCloseCallback, new Callback(OnSurvivorInfoClose));
			survivorInfoPopup.OnCloseCallback = (Callback)Delegate.Combine(survivorInfoPopup.OnCloseCallback, new Callback(OnSurvivorInfoClose));
		}
	}

	private static void OnSurvivorManagementClose()
	{
		SelectSurvivorsPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RadioSelectSurvivorPopup) as SelectSurvivorsPopup;
		obj.SkipClickIntro = true;
		obj.Open();
	}

	private void OnSurvivorInfoClose()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			CallCraft.Instance._SelectSurvivorsPopup.gameObject.SetActive(true);
			return;
		}
		LootEntry selectedLootEntry = null;
		SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
		if (survivorInfoPopup != null)
		{
			selectedLootEntry = GetLootEntryFromCards(survivorInfoPopup.survivorModel);
		}
		SelectSurvivorsPopup selectSurvivorsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RadioSelectSurvivorPopup) as SelectSurvivorsPopup;
		selectSurvivorsPopup.SkipClickIntro = true;
		selectSurvivorsPopup.Open();
		if (RerollsLeft == 0 && !IsAllLootClaimable())
		{
			selectSurvivorsPopup.SelectLootEntry(selectedLootEntry);
		}
		else
		{
			selectSurvivorsPopup.SelectLootEntry(null);
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewSurvivorSelected" && parameter != null && parameter is SurvivorModel && !IsSurvivorManagementPopUpOpen())
		{
			SurvivorModel survivorModel = parameter as SurvivorModel;
			if (SelectedLootEntry == null || survivorModel != SelectedLootEntry.GeneratedSurvivor)
			{
				DebugLog("Survivor Selected: " + survivorModel.Name);
			}
			else
			{
				DebugLog("Survivor Deselected: " + survivorModel.Name);
				survivorModel = null;
			}
			if (RerollsLeft == 0 && !IsAllLootClaimable())
			{
				SelectLootEntry(GetLootEntryFromCards(survivorModel));
			}
			return;
		}
		switch (type)
		{
		case "OnSurvivorInfoClosed":
		case "OnBundleBought":
			UpdateManagePanel();
			break;
		case "OnSurvivorInfoOpen":
			OnSurvivorInfoOpen();
			break;
		case "OnAcceptSelectedLootEntryTokens":
		case "OnAcceptSelectedLootEntrySurvivor":
			OnClickAcceptSelectedLoot(type, parameter);
			break;
		case "OnLockLootEntry":
			if (parameter != null && parameter is int)
			{
				OnClickLockLoot((int)parameter);
			}
			break;
		case "OnUnlockLootEntry":
			if (parameter != null && parameter is int)
			{
				OnClickUnlockLoot((int)parameter);
			}
			break;
		case "OnTriggerHeroUnlock":
			returnToSelectSurvivor = false;
			if (parameter != null && parameter is ActorDefinition)
			{
				HeroUnlockHelper.UnlockHero((ActorDefinition)parameter, OnReturnToRadioOrSelect);
			}
			break;
		case "StartPhoneCallCommandResponseReceived":
		{
			UnityEngine.Debug.LogError("========StartPhoneCallCommandResponseReceived");
			InstantiateButtons(GameManager.Instance.playerModel.PhoneCall);
			PositionCards();
			for (int j = 0; j < CardsList.Count; j++)
			{
				if (CardsList[j] != null)
				{
					CardsList[j].AnimateIntro();
				}
			}
			ClickWaitTime = Time.time + InputWaitTime;
			break;
		}
		case "RerollPhoneCallCommandResponseReceived":
		{
			UnityEngine.Debug.LogError("========RerollPhoneCallCommandResponseReceived");
			InstantiateButtons(GameManager.Instance.playerModel.PhoneCall);
			PositionCards();
			CardIntroCompletedCount = 0;
			for (int i = 0; i < CardsList.Count; i++)
			{
				if (CardsList[i] != null)
				{
					CardsList[i].AnimateIntro();
				}
			}
			TweenOffsetAllChildrenTo(SidewaysOffset, -1f, null, skipTween: true);
			ClickWaitTime = Time.time + InputWaitTime;
			break;
		}
		}
	}

	private void OnReturnToRadioOrSelect()
	{
		if (IsAllLootClaimable())
		{
			SelectSurvivorsPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RadioSelectSurvivorPopup) as SelectSurvivorsPopup;
			obj.SkipClickIntro = true;
			obj.Open();
			return;
		}
		NewPhonePopup newPhonePopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.NewRadioPopup) as NewPhonePopup;
		if (newPhonePopup != null)
		{
			newPhonePopup.Open();
		}
	}

	private IEnumerator AutoClickEffect(float delayBetween)
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i] != null)
			{
				CardsList[i].FakeEffectClicked();
				yield return new WaitForSeconds(delayBetween);
			}
		}
	}

	private IEnumerator CheckAutoSelectFirstDelayed(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
	}

	private bool IsSurvivorManagementPopUpOpen()
	{
		SurvivorManagementPopUp survivorManagementPopUp = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds, null, createIfNotExist: false) as SurvivorManagementPopUp;
		if (survivorManagementPopUp != null)
		{
			return survivorManagementPopUp.IsOpen;
		}
		return false;
	}

	private bool IsAllLootClaimable()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.PhoneCall == null)
		{
			return false;
		}
		DebugTWD.LogMycode("|| OfflineManager.IsFreeAll");
		return GameManager.Instance.playerModel.PhoneCall.CanClaimEntireMultiLootsList() || OfflineManager.IsFreeAll;
	}

	private void InstantiateButtons(PhoneCallModel phoneCallModel)
	{
		RadioCallCardBase radioCallCardBase = null;
		int count = GameManager.Instance.playerModel.PhoneCall.LootsList.Count;
		bool flag = IsAllLootClaimable();
		if (!flag && GameManager.Instance.playerModel.PhoneCall.NumLootChoosable != 1 && GameManager.Instance.playerModel.PhoneCall.NumLootChoosable != 0)
		{
			Debug.LogError("Unsupported amount of NumLootChoosable, should be either 1 or match loot amount.");
		}
		if (CardsList != null)
		{
			if (CardsList.Count > 0)
			{
				return;
			}
			if (ButtonParentTarget != null && ButtonParentTarget.gameObject != null)
			{
				DebugTWD.Log("CardsList setup", DebugType.Call);
				for (int i = 0; i < count; i++)
				{
					LootEntry lootEntry = GameManager.Instance.playerModel.PhoneCall.LootsList[i];
					if (lootEntry == null || !(SurvivorCardPrefab != null) || !(TokenCardPrefab != null))
					{
						continue;
					}
					SurvivorModel generatedSurvivor = lootEntry.GeneratedSurvivor;
					DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = GameManager.Instance.playerModel.PhoneCall.LootsList[i].DropCurrencyType;
					DropType dropType = GameManager.Instance.playerModel.PhoneCall.LootsList[i].DropType;
					if (generatedSurvivor != null && dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
					{
						radioCallCardBase = Helpers.AddComponent<RadioCallSurvivorCard>(Helpers.InstantiateToParentAndLayer(SurvivorCardPrefab, ButtonParentTarget.gameObject));
						if (radioCallCardBase != null)
						{
							radioCallCardBase.ForRerolling = RerollsLeft > 0;
							radioCallCardBase.DisableSelectVisualization = flag;
							radioCallCardBase.SetLootEntry(lootEntry, i);
							if (!OfflineManager.IsNoEffects) radioCallCardBase.InitEffects(CardEffectPrefab, generatedSurvivor.SurvivorClass, generatedSurvivor.SurvivorRarityLevel, dropType, isToken: false);
							radioCallCardBase.InitSurvivorCard(generatedSurvivor, i, OnClickSurvivorCard);
							radioCallCardBase.InitRerollButtons();
							if (flag && GameManager.Instance.playerModel.PhoneCall.IsLootClaimed(i))
							{
								Helpers.GameObjectSetActive(radioCallCardBase, value: false);
							}
							DebugTWD.Log("Survivor Token Add: " + generatedSurvivor.ActorDefinitionID, DebugType.Call);
						}
					}
					else if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
					{
						radioCallCardBase = Helpers.AddComponent<RadioCallTokenCard>(Helpers.InstantiateToParentAndLayer(TokenCardPrefab, ButtonParentTarget.gameObject));
						if (radioCallCardBase != null)
						{
							radioCallCardBase.ForRerolling = RerollsLeft > 0;
							radioCallCardBase.DisableSelectVisualization = flag;
							radioCallCardBase.SetLootEntry(lootEntry, i);
							if (!OfflineManager.IsNoEffects) radioCallCardBase.InitEffects(CardEffectPrefab, SurvivorClass.None, 4, dropType, isToken: true);
							radioCallCardBase.InitTokenCard(lootEntry, i, lootEntry.ModelId.ToString(), OnClickTokenCard);
							radioCallCardBase.InitRerollButtons();
							if (flag && GameManager.Instance.playerModel.PhoneCall.IsLootClaimed(i))
							{
								Helpers.GameObjectSetActive(radioCallCardBase, value: IsLoadDataManager);
								if (IsLoadDataManager)
								{
									string actorId = SurvivorToken.GetHeroId(lootEntry.RewardedCurrency);
									DebugTWD.Log("HeroToken Add: " + actorId, DebugType.Call);
								}
							}
						}
					}
					if (IsLoadDataManager)
					{
						DebugTWD.LogMycode("if (IsLoadDataManager)");
						radioCallCardBase.ShowRewardCard();
					}
					radioCallCardBase.SetIntroCompleteCallaback(IntroCompleteCallaback);
					radioCallCardBase.UpdateUI();
					CardsList.Add(radioCallCardBase);
				}
			}
			else
			{
				DebugLogError("Could not instantiate target is NULL!");
			}
		}
		else
		{
			DebugLogError("CallButtonsList is NULL!");
		}
		radioCallCardBase = null;
	}

	private void OnClickTokenCard(UIButtonExtended button)
	{
		if (IsLoadDataManager && IsDisableCardClick)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager && IsDisableCardClick) return");
			return;
		}

		if (button != null)
		{
			LootEntry lootEntry = GetCardWith(button.id);
			if (SelectedLootEntry == null || lootEntry != SelectedLootEntry)
			{
				DebugLog("Token Selected: " + lootEntry.RewardedCurrency);
			}
			else
			{
				DebugLog("Token Deselected: " + lootEntry.RewardedCurrency);
				lootEntry = null;
			}
			if (RerollsLeft == 0 && !IsAllLootClaimable())
			{
				SelectLootEntry(lootEntry);
			}
		}
	}

	private void PositionCards()
	{
		Helpers.GameObjectSetActive(skipButton, value: true);
		if (CardsList != null && CardsList.Count > 0)
		{
			for (int i = 0; i < CardsList.Count; i++)
			{
				RadioCallCardBase radioCallCardBase = CardsList[i];
				if (radioCallCardBase != null)
				{
					radioCallCardBase.SetInitPosition(HelpersUI.GetRowPositionX(i, CardsList.Count, radioCallCardBase.localSize));
					GameObject obj = radioCallCardBase.gameObject;
					obj.name = obj.name + "_Count_" + (i + 1);
				}
				radioCallCardBase = null;
			}
		}
		else
		{
			DebugLogError("CardsList is NULL or Empty!");
		}
	}

	private void HideAllAnimations()
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i] != null)
			{
				CardsList[i].HideAnimation();
			}
		}
	}

	private void HideAllRewardCards(LootEntry DontHideReward = null)
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i] != null && (CardsList[i].GetLootEntry() == null || DontHideReward == null || DontHideReward != CardsList[i].GetLootEntry()))
			{
				CardsList[i].HideRewardCard();
			}
		}
	}

	private void LockAllCards(bool value)
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i] != null)
			{
				CardsList[i].SetCardLocked(value, introAnimationLock: false);
			}
		}
	}

	private void SelectionDone(LootEntry entry, RadioCallCardBase.Callback collectAnimationComplete, bool animate, bool doInPlaceAnimation)
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i].GetLootEntry() != null && entry != null && CardsList[i].GetLootEntry() == entry)
			{
				CardsList[i].CollectCard(collectAnimationComplete, selectedRewardType, animate, doInPlaceAnimation);
				break;
			}
		}
	}

	public void UpdateManagePanel()
	{
		if (ManagePanel != null)
		{
			ManagePanel.RerollsLeft = RerollsLeft;
			if (IsLoadDataManager)
			{
				ManagePanel.LootIndexes = CardsList.Select(x => x.GetLootEntryIndex()).ToList();
				DebugTWD.Log("Update Manage Panel", DebugType.Call);
				DebugTWD.LogMycode("if (IsLoadDataManager)");
			}
			ManagePanel.UpdateUI();
		}
	}

	private bool CurrentIsAcceptableSurvior()
	{
		if (SelectedLootEntry != null && SelectedLootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
		{
			return GameManager.Instance.playerModel.SurvivorContainer.CanAddSurvivor();
		}
		return false;
	}

	private bool CurrentIsAcceptableHeroTokens()
	{
		if (SelectedLootEntry != null)
		{
			return SelectedLootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken;
		}
		return false;
	}

	private int UpdateCardUIStateWithModel(LootEntry selectedLootEntry)
	{
		int result = -1;
		bool flag = false;
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList == null || !(CardsList[i] != null))
			{
				continue;
			}
			if (IsAllLootClaimable())
			{
				flag = true;
			}
			else
			{
				flag = selectedLootEntry != null && CardsList[i].GetLootEntry() == selectedLootEntry;
				if (flag)
				{
					result = i;
				}
			}
			CardsList[i].Select(flag);
			CardsList[i].UpdateUI();
		}
		return result;
	}

	private string GetRejectPrice()
	{
		SurvivorModel mostValueableSurvivor = GetMostValueableSurvivor();
		if (mostValueableSurvivor == null)
		{
			return "";
		}
		return mostValueableSurvivor.GetDemoteCashier().GetTotalCost(CurrencyType.SurvivalPoints).ToString();
	}

	private SurvivorModel GetMostValueableSurvivor()
	{
		SurvivorModel survivorModel = null;
		int num = 0;
		int num2 = 0;
		LootEntry lootEntry = null;
		for (int i = 0; i < GameManager.Instance.playerModel.PhoneCall.LootsList.Count; i++)
		{
			lootEntry = GameManager.Instance.playerModel.PhoneCall.LootsList[i];
			if (lootEntry != null && lootEntry.GeneratedSurvivor != null)
			{
				num2 = lootEntry.GeneratedSurvivor.GetDemoteCashier().GetTotalCost(CurrencyType.SurvivalPoints);
				if (survivorModel == null || num < num2)
				{
					survivorModel = lootEntry.GeneratedSurvivor;
					num = survivorModel.GetDemoteCashier().GetTotalCost(CurrencyType.SurvivalPoints);
				}
			}
		}
		return survivorModel;
	}

	private void ShowSelectionForAllCards()
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			CardsList[i].Select(selected: true);
			CardsList[i].UpdateUI();
		}
	}

	private void AllCardsClicked(bool skipTween)
	{
		LockAllCards(value: false);
		if (!IsLoadDataManager)
		{
			if (CardsList.Count > 1)
			{
				TweenOffsetAllChildrenTo(SidewaysOffset, -1f, null, skipTween);
			}
			ShowSidePanels();
		}
		StartCoroutine(CheckAutoSelectFirstDelayed(0.5f));
		if (IsAllLootClaimable() && RerollsLeft == 0)
		{
			EnableSidePanelsButtons();
			ShowSelectionForAllCards();
		}
		DebugTWD.Log("All Cards Clicked", DebugType.Call);
		Helpers.GameObjectSetActive(skipButton, value: false);
		EventManager.NotifyClick("SearchOver");
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("camp/phonecall");
		}
	}

	private void TweenOffsetAllChildrenTo(Vector3 localPositionOffset, float newLocalScale = -1f, Tweener.CallBackDelegate callback = null, bool skipTween = false)
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i] != null)
			{
				float duration = (skipTween ? 0f : 1f);
				CardsList[i].TweenOffsetToPosition(localPositionOffset, Vector3.one * newLocalScale, duration, callback);
			}
		}
	}

	private void TweenChildTo(int index, Vector3 localPosition, float newLocalScale = -1f, float duration = 1f, Tweener.CallBackDelegate callback = null)
	{
		if (CardsList != null && index >= 0 && CardsList.Count > index && CardsList[index] != null)
		{
			CardsList[index].TweenToPosition(localPosition, Vector3.one * newLocalScale, duration, callback);
		}
	}

	private void TweenChildScaleOnly(int index, float newLocalScale = -1f, float duration = 1f, Tweener.CallBackDelegate callback = null)
	{
		if (CardsList != null && index >= 0 && CardsList.Count > index && CardsList[index] != null)
		{
			CardsList[index].TweenToPosition(CardsList[index].transform.localPosition, Vector3.one * newLocalScale, duration, callback);
		}
	}

	private RadioCallCardBase GetCardByIndex(int index)
	{
		if (CardsList != null && index >= 0 && CardsList.Count > index && CardsList[index] != null)
		{
			return CardsList[index];
		}
		return null;
	}

	private LootEntry GetLootEntryFromCards(SurvivorModel survivorModel)
	{
		for (int i = 0; i < CardsList.Count; i++)
		{
			if (CardsList[i] != null && CardsList[i].GetLootEntry() != null && CardsList[i].GetLootEntry().GeneratedSurvivor != null && CardsList[i].GetLootEntry().GeneratedSurvivor == survivorModel)
			{
				return CardsList[i].GetLootEntry();
			}
		}
		return null;
	}

	private LootEntry GetCardWith(string lootEntryId)
	{
		int result = -1;
		if (int.TryParse(lootEntryId, out result))
		{
			for (int i = 0; i < CardsList.Count; i++)
			{
				bool condition = IsLoadDataManager ? i == result : CardsList[i].GetLootEntry().ModelId == result;

				if (CardsList[i] != null && CardsList[i].GetLootEntry() != null && condition)
				{
					return CardsList[i].GetLootEntry();
				}
			}
		}
		return null;
	}

	private void IntroCompleteCallaback(RadioCallCardBase card)
	{
		if (card != null && card.GetLootEntry() != null && !card.GetLootEntry().Opened)
		{
			Helpers.ExecuteCommand(new LootRewardOpenedCommand
			{
				ModelId = card.GetLootEntry().ModelId
			});
		}
		CardIntroCompletedCount++;
		if (CardsList != null && CardsList.Count > 0 && CardIntroCompletedCount >= CardsList.Count)
		{
			AllCardsClicked(OpenedAfterReroll);
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;

	public int RerollIndex { get; set; }

	public bool IsDisableCardClick;

	public bool IsSelectedMode;
	#endregion

	#region mycode
	private void InstantiateButtonsForSelected()
	{
		var SelectedCall = CallCraft.Instance.SelectedCall;
		if (SelectedCall == null) return;

		RadioCallCardBase radioCallCardBase = null;
		int count = SelectedCall.LootEntryList.Count;

		if (CardsList == null || CardsList.Count > 0 || ButtonParentTarget == null || ButtonParentTarget.gameObject == null)
		{
			return;
		}
		DebugTWD.Log("CardsList setup");
		for (int i = 0; i < count; i++)
		{
			LootEntry lootEntry = SelectedCall.LootEntryList[i];
			if (lootEntry == null || SurvivorCardPrefab == null || TokenCardPrefab == null)
			{
				continue;
			}
			SurvivorModel generatedSurvivor = lootEntry.GeneratedSurvivor;
			DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = SelectedCall.LootEntryList[i].DropCurrencyType;
			DropType dropType = SelectedCall.LootEntryList[i].DropType;
			if (generatedSurvivor != null && dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
			{
				radioCallCardBase = Helpers.AddComponent<RadioCallSurvivorCard>(Helpers.InstantiateToParentAndLayer(SurvivorCardPrefab, ButtonParentTarget.gameObject));
				if (radioCallCardBase != null)
				{
					radioCallCardBase.ForRerolling = false;
					radioCallCardBase.DisableSelectVisualization = true;
					radioCallCardBase.SetLootEntry(lootEntry, i);
					radioCallCardBase.InitSurvivorCard(generatedSurvivor, i, OnClickSurvivorCard);
					radioCallCardBase.InitRerollButtons();

					Helpers.GameObjectSetActive(radioCallCardBase, value: true);

					DebugTWD.Log("Survivor Token Add : " + generatedSurvivor.ActorDefinitionID);
				}
			}
			else if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
			{
				radioCallCardBase = Helpers.AddComponent<RadioCallTokenCard>(Helpers.InstantiateToParentAndLayer(TokenCardPrefab, ButtonParentTarget.gameObject));

				if (radioCallCardBase != null)
				{
					radioCallCardBase.ForRerolling = false;
					radioCallCardBase.DisableSelectVisualization = true;
					radioCallCardBase.SetLootEntry(lootEntry, i);
					radioCallCardBase.InitTokenCard(lootEntry, i, i.ToString(), OnClickTokenCard);
					radioCallCardBase.InitRerollButtons();

					Helpers.GameObjectSetActive(radioCallCardBase, value: true);

					string actorId = SurvivorToken.GetHeroId(lootEntry.RewardedCurrency);
					DebugTWD.Log("HeroToken Add : " + actorId);
				}
			}
			radioCallCardBase.ShowRewardCard();
			radioCallCardBase.SetIntroCompleteCallaback(IntroCompleteCallaback);
			radioCallCardBase.UpdateUI();
			CardsList.Add(radioCallCardBase);
		}
	}

	public void OnClickAcceptSelectedLoot(int num)
	{
		bool flag = IsAllLootClaimable();
		LootEntry lootEntry = null;
		if (num >= 0 && num < GameManager.Instance.playerModel.PhoneCall.LootsList.Count)
		{
			lootEntry = SelectedLootEntry = GameManager.Instance.playerModel.PhoneCall.LootsList[num];
			LatestSelectedRewardIndex = num;
		}
		if (lootEntry == null)
		{
			return;
		}
		if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
		{
			AcceptClassTokens(lootEntry.GeneratedSurvivor);
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
		{
			AcceptHeroTokens(lootEntry);
		}
		if (flag)
		{
			StartSingleCardCollectAnimation();
		}
		else
		{
			StartOutroAnimation();
		}
	}

	private void OnClickSurvivorCard(UIButtonExtended button)
	{
		if (IsDisableCardClick && IsLoadDataManager) return;

		if (button != null && !string.IsNullOrEmpty(button.id))
		{
			LootEntry lootEntry = GetCardWith(button.id);
			if (SelectedLootEntry == null || lootEntry != SelectedLootEntry)
			{
				DebugLog("Survivor Selected: " + lootEntry.RewardedCurrency);
			}
			else
			{
				DebugLog("Survivor Deselected: " + lootEntry.RewardedCurrency);
				lootEntry = null;
			}
			DebugTWD.Log("Rerolls Left: " + RerollsLeft);
			if (RerollsLeft == 0 && (IsLoadDataManager || !IsAllLootClaimable()))
			{
				SelectLootEntry(lootEntry);
			}
		}
	}

	public void FinishCall(bool pickedSingleLootOfMany)
	{
		GameManager.Instance.playerModel.PhoneCall.LootsList.Clear();

		if (pickedSingleLootOfMany)
		{
			EnableSidePanelsButtons();
		}
	}

	public TWDModelResult TWDModelResultReroll(int lootIndex, bool locked)
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		TWDModelResult result = TWDModelResult.Error;
		if (playerModel != null)
		{
			if (playerModel.PhoneCall.NumRerolls > 0)
			{
				if (playerModel.PhoneCall.SetLootLockedForReroll(lootIndex, locked))
				{
					result = TWDModelResult.OK;
				}
			}
			else
			{
				result = TWDModelResult.Error;
			}
		}
		return result;
	}

	private TWDModelResult OnClickRerollResult()
	{
		return GameManager.Instance.playerModel.PhoneCall.NumRerolls > 0 ? GameManager.Instance.playerModel.PhoneCall.RerollCall() : TWDModelResult.Error;
	}
	#endregion
}
