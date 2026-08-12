using System.Collections;
using System.Collections.Generic;
using System.Text;
using TWDModel;
using UnityEngine;

public class IAPConfirmPopupNew : ConfirmationPopup
{
	[Tooltip("Panel used for showing the currency rewards plus survivor slots")]
	[SerializeField]
	private AnimateIapRewardsNew CurrencyRewardParent;

	[Tooltip("Panel used for showing the equipment rewards")]
	[SerializeField]
	private AnimateIapRewardsNew EquipmentRewardParent;

	[Tooltip("Panel used for showing the equipment token rewards")]
	[SerializeField]
	private AnimateIapRewardsNew EquipmentTokenRewardParent;

	[Tooltip("Panel used for showing the outfit rewards")]
	[SerializeField]
	private AnimateIapRewardsNew OutfitRewardParent;

	[Tooltip("Panel used for showing the hero skin rewards")]
	[SerializeField]
	private AnimateIapRewardsNew HeroSkinRewardParent;

	[Tooltip("Button that exits the popup")]
	[SerializeField]
	private UIButton ConfirmButton;

	[Header("Summary List")]
	[SerializeField]
	private UIButton ButtonClose;

	[SerializeField]
	private GameObject ListParent;

	[SerializeField]
	private UILabel ListLabel;

	[SerializeField]
	private UILabel ListTitle;

	private bool SkipBlocked;

	private bool bundleGivenBySupport;

	private List<AnimateIapRewardsNew> AllPanelList = new List<AnimateIapRewardsNew>();

	private int CurrentPanelIndex;

	private BundleContentDefinition ContentDefinition;

	private IList<IReward> cachedRewards;

	private bool enableContinueButton;

	private Callback closeAnimOverCallback;

	public bool ShowShopWhenClosed { get; set; }

	public static IAPConfirmPopupNew OpenWithSubscriptionContent(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition, bool givenBySupport, string overrideTitle = null)
	{
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		if (!iAPConfirmPopupNew.IsOpen)
		{
			iAPConfirmPopupNew.OpenForBundleContentDefinition(bundleStoreDefinition, bundleContentDefinition, givenBySupport, overrideTitle);
		}
		return iAPConfirmPopupNew;
	}

	public static IAPConfirmPopupNew OpenWithBundleContent(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition, bool givenBySupport, string overrideTitle = null)
	{
		if (bundleContentDefinition.Category == BundleContentDefinition.CategoryHidden)
		{
			return null;
		}
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		if (!iAPConfirmPopupNew.IsOpen)
		{
			iAPConfirmPopupNew.OpenForBundleContentDefinition(bundleStoreDefinition, bundleContentDefinition, givenBySupport, overrideTitle);
		}
		return iAPConfirmPopupNew;
	}

	public static IAPConfirmPopupNew OpenCustomBundleContent(CustomBundleDefinition customBundleDefinition)
	{
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		if (iAPConfirmPopupNew != null && !iAPConfirmPopupNew.IsOpen)
		{
			iAPConfirmPopupNew.ShowShopWhenClosed = true;
			iAPConfirmPopupNew.OpenForRewards(GameManager.Instance.playerModel.CustomizedBundleManager.LastCustomReward);
			iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
			UIEvent.Send("CustomRewardBundleBoughtEvent", customBundleDefinition);
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
			}
		}
		return iAPConfirmPopupNew;
	}

	public void OpenForBundleContentDefinition(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition, bool givenBySupport, string overrideTitle = null)
	{
		bundleGivenBySupport = givenBySupport;
		ContentDefinition = bundleContentDefinition;
		Reset(overrideTitle);
		if (AddRewardsToPanel(CurrencyRewardParent))
		{
			AllPanelList.Add(CurrencyRewardParent);
		}
		if (AddRewardsToPanel(OutfitRewardParent))
		{
			AllPanelList.Add(OutfitRewardParent);
		}
		if (AddRewardsToPanel(EquipmentRewardParent))
		{
			AllPanelList.Add(EquipmentRewardParent);
		}
		if (AddRewardsToPanel(EquipmentTokenRewardParent))
		{
			AllPanelList.Add(EquipmentTokenRewardParent);
		}
		if (AddRewardsToPanel(HeroSkinRewardParent))
		{
			AllPanelList.Add(HeroSkinRewardParent);
		}
		Open();
		PlayCurrentPanel();
	}

	public void OpenForEquipment(EquipmentItemModel equipment, string overrideTitle = null)
	{
		if (equipment != null)
		{
			Reset(overrideTitle);
			AllPanelList.Add(EquipmentRewardParent);
			RewardEquipment reward = new RewardEquipment();
			EquipmentRewardParent.AddReward(reward, equipment);
			Open();
			PlayCurrentPanel();
			enableContinueButton = true;
		}
		else
		{
			DebugLogError("Cant Open. Given equipment definition is NULL!");
		}
	}

	public void OpenForEquipmentToken(EquipTokenItemModel equipment, string overrideTitle = null)
	{
		if (equipment != null)
		{
			Reset(overrideTitle);
			AllPanelList.Add(EquipmentTokenRewardParent);
			RewardEquipment reward = new RewardEquipment();
			EquipmentTokenRewardParent.AddReward(reward, null, null, null, equipment);
			Open();
			PlayCurrentPanel();
			enableContinueButton = true;
		}
		else
		{
			DebugLogError("Cant Open. Given equipment token definition is NULL!");
		}
	}

	public void AddEquipment(EquipmentItemModel equipment)
	{
		if (equipment != null)
		{
			AllPanelList.Add(EquipmentRewardParent);
			RewardEquipment reward = new RewardEquipment();
			EquipmentRewardParent.AddReward(reward, equipment);
		}
	}

	public void OpenForConsumable(RewardEquipment equipment, string overrideTitle = null)
	{
		if (equipment != null)
		{
			Reset(overrideTitle);
			AllPanelList.Add(CurrencyRewardParent);
			CurrencyRewardParent.AddReward(equipment);
			Open();
			PlayCurrentPanel();
			enableContinueButton = true;
		}
		else
		{
			DebugLogError("Cant Open. Given equipment definition is NULL!");
		}
	}

	public void OpenForHeroSKin(RewardHeroSkin rewardHeroSkin, string overrideTitle = null)
	{
		if (rewardHeroSkin != null)
		{
			Reset(overrideTitle);
			AllPanelList.Add(HeroSkinRewardParent);
			HeroSkinRewardParent.AddReward(rewardHeroSkin, null, null, GameManager.Instance.gameEconomyData.GetSkinDefinition(rewardHeroSkin.PreferredOrder[0]));
			Open();
			PlayCurrentPanel();
			enableContinueButton = true;
		}
		else
		{
			DebugLogError("Cant Open. Given equipment definition is NULL!");
		}
	}

	public void OpenForCurrency(RewardCurrency currency, bool isGift = true)
	{
		if (currency == null)
		{
			DebugLogError("Cant Open. Given currency definition is NULL!");
			return;
		}
		PlayRevealCurrencyAnimationIfMetersArePaused(currency);
		Reset();
		if (titleLabel != null && isGift)
		{
			titleLabel.text = LocalizationManager.GetText("Popup.IAPConfirm.ReceivedGift");
		}
		AllPanelList.Add(CurrencyRewardParent);
		CurrencyRewardParent.AddReward(currency);
		Open();
		PlayCurrentPanel();
		enableContinueButton = true;
	}

	public void OpenForTimedReward(RewardTimedBonus timedReward, string overrideTitle = null)
	{
		if (timedReward != null)
		{
			Reset(overrideTitle);
			AllPanelList.Add(CurrencyRewardParent);
			CurrencyRewardParent.AddReward(timedReward);
			Open();
			PlayCurrentPanel();
			enableContinueButton = true;
		}
		else
		{
			DebugLogError("Cant Open. Given equipment definition is NULL!");
		}
	}

	public void OpenForCurrencyList(List<RewardCurrency> currencyList, bool isGift = true)
	{
		if (currencyList != null && currencyList.Count > 0)
		{
			Reset();
			if (titleLabel != null && isGift)
			{
				titleLabel.text = LocalizationManager.GetText("Popup.IAPConfirm.ReceivedGift");
			}
			AllPanelList.Add(CurrencyRewardParent);
			foreach (RewardCurrency currency in currencyList)
			{
				PlayRevealCurrencyAnimationIfMetersArePaused(currency);
				CurrencyRewardParent.AddReward(currency);
			}
			Open();
			PlayCurrentPanel();
			enableContinueButton = true;
		}
		else
		{
			DebugLogError("Cant Open. Given currency definition is NULL!");
		}
	}

	public void OpenForOutfit(RewardOutfit rewardOutfit)
	{
		OutfitDefinition outfitDefinition = GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]);
		if (outfitDefinition != null)
		{
			Reset();
			AllPanelList.Add(OutfitRewardParent);
			OutfitRewardParent.AddReward(rewardOutfit, null, outfitDefinition);
			Open();
			PlayCurrentPanel();
			enableContinueButton = true;
		}
		else
		{
			DebugLogError("Cant Open. Given outfit definition is NULL!");
		}
	}

	public override void Update()
	{
		if (ConfirmButton != null && CurrencyRewardParent != null)
		{
			ConfirmButton.isEnabled = enableContinueButton;
		}
	}

	public override void Open()
	{
		DebugClassString = "IAPConfirmPopupNew";
		enableContinueButton = false;
		Helpers.GameObjectSetActive(ButtonClose, GameManager.Instance.gameEconomyData.ConfigData.EnableIapConfirmList);
		Helpers.GameObjectSetActive(ListParent, value: false);
		base.Open();
	}

	public override void Close()
	{
		MarkBundleAsViewed();
		base.Close();
		CheckForBonusGift();
	}

	private void CheckForBonusGift()
	{
		if ((GameManager.Instance.playerModel.BundleManager.IAPBonusGiftLootEntry != null || GameManager.Instance.playerModel.BundleManager.WebShopLootEntrys.Count > 0) && !RewardScreenHandler.Instance.gameObject.activeInHierarchy)
		{
			OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				openLootInUi.OpenForModel(GameManager.Instance.playerModel.BundleManager);
				SingularityMonoBehaviour<HUDManager>.Instance.CloseAllElementsOfType(UIType.BuyResourcesPopup);
			}
		}
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public override void OkPressed()
	{
		base.OkPressed();
		MarkBundleAsViewed();
		CheckForBonusGift();
	}

	public void OnOpenListClicked()
	{
		if (!GameManager.Instance.gameEconomyData.ConfigData.EnableIapConfirmList || (ContentDefinition == null && cachedRewards == null))
		{
			Helpers.GameObjectSetActive(ButtonClose, value: false);
			Helpers.GameObjectSetActive(ListParent, value: false);
			OnClickClose();
			return;
		}
		if (CurrencyRewardParent != null)
		{
			CurrencyRewardParent.Hide();
		}
		if (EquipmentRewardParent != null)
		{
			EquipmentRewardParent.Hide();
		}
		if (EquipmentTokenRewardParent != null)
		{
			EquipmentTokenRewardParent.Hide();
		}
		if (OutfitRewardParent != null)
		{
			OutfitRewardParent.Hide();
		}
		if (HeroSkinRewardParent != null)
		{
			HeroSkinRewardParent.Hide();
		}
		CurrentPanelIndex = 0;
		Helpers.GameObjectSetActive(ButtonClose, value: false);
		Helpers.GameObjectSetActive(ListParent, value: true);
		IList<IReward> list = null;
		if (ContentDefinition != null && ContentDefinition.RewardEntries != null && ContentDefinition.RewardEntries.RewardsList != null)
		{
			list = ContentDefinition.RewardEntries.RewardsList;
		}
		else if (cachedRewards != null)
		{
			list = cachedRewards;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (list != null)
		{
			List<IReward> list2 = CombineRewards(list);
			list2.StableSort(delegate(IReward a, IReward b)
			{
				int num2 = 0;
				int num3 = 0;
				if (a is RewardCurrency && b is RewardCurrency)
				{
					num2 = (int)((RewardCurrency)a).CurrencyType;
					num3 = (int)((RewardCurrency)b).CurrencyType;
				}
				if (num2 == num3)
				{
					return 0;
				}
				return (num2 <= num3) ? 1 : (-1);
			});
			for (int num = 0; num < list2.Count; num++)
			{
				if (list2[num] != null)
				{
					if (list2[num] is RewardCurrency && ComponentHelper.IsComponentCurrency((list2[num] as RewardCurrency).CurrencyType))
					{
						RewardCurrency rewardCurrency = list2[num] as RewardCurrency;
						stringBuilder.AppendLine(HelpersLocalization.GetComponentRewardName(rewardCurrency.CurrencyType, rewardCurrency.Amount, ComponentHelper.GetComponentRarityLevel(rewardCurrency.CurrencyType), colorRarity: true));
					}
					else
					{
						stringBuilder.AppendLine(HelpersLocalization.GetBundleTitleForIReward(list2[num]));
					}
				}
			}
		}
		HelpersUI.SetContentToLabel(ListLabel, stringBuilder.ToString());
		HelpersUI.SetContentToLabel(ListTitle, LocalizationManager.GetText("Popup.IAPConfirm.SummaryList.Title"));
	}

	private List<IReward> CombineRewards(IList<IReward> currentRewards)
	{
		Dictionary<CurrencyType, RewardCurrency> dictionary = new Dictionary<CurrencyType, RewardCurrency>();
		Dictionary<string, RewardEquipment> dictionary2 = new Dictionary<string, RewardEquipment>();
		List<IReward> list = new List<IReward>();
		RewardCurrency rewardCurrency = null;
		RewardCurrency value = null;
		RewardEquipment rewardEquipment = null;
		RewardEquipment value2 = null;
		for (int i = 0; i < currentRewards.Count; i++)
		{
			rewardCurrency = currentRewards[i] as RewardCurrency;
			rewardEquipment = currentRewards[i] as RewardEquipment;
			if (rewardCurrency != null)
			{
				if (dictionary.TryGetValue(rewardCurrency.CurrencyType, out value))
				{
					value.Amount += rewardCurrency.Amount;
				}
				else
				{
					dictionary.Add(rewardCurrency.CurrencyType, rewardCurrency.GetClone());
				}
			}
			else if (rewardEquipment != null && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
			{
				if (dictionary2.TryGetValue(rewardEquipment.EquipmentId, out value2))
				{
					value2.Amount += rewardEquipment.Amount;
				}
				else
				{
					dictionary2.Add(rewardEquipment.EquipmentId, rewardEquipment);
				}
			}
			else if (currentRewards[i] != null)
			{
				list.Add(currentRewards[i]);
			}
		}
		foreach (KeyValuePair<CurrencyType, RewardCurrency> item in dictionary)
		{
			list.Add(item.Value);
		}
		foreach (KeyValuePair<string, RewardEquipment> item2 in dictionary2)
		{
			list.Add(item2.Value);
		}
		dictionary2.Clear();
		dictionary.Clear();
		return list;
	}

	public void SkipAnimClicked()
	{
		if (!SkipBlocked && GetCurrentPanel() != null)
		{
			GetCurrentPanel().SkipCurrentBeingShown();
			StartCoroutine(BlockSkipForSeconds(0.5f));
		}
	}

	private void Reset(string overrideTitle = null)
	{
		cachedRewards = null;
		if (CurrencyRewardParent != null && EquipmentRewardParent != null && OutfitRewardParent != null && HeroSkinRewardParent != null && EquipmentTokenRewardParent != null)
		{
			CurrencyRewardParent.Hide();
			EquipmentRewardParent.Hide();
			EquipmentTokenRewardParent.Hide();
			OutfitRewardParent.Hide();
			HeroSkinRewardParent.Hide();
			CurrentPanelIndex = 0;
			AllPanelList = new List<AnimateIapRewardsNew>();
			enableContinueButton = false;
		}
		if (titleLabel != null)
		{
			if (!string.IsNullOrEmpty(overrideTitle))
			{
				titleLabel.text = LocalizationManager.GetText(overrideTitle);
			}
			else if (ContentDefinition != null && string.IsNullOrEmpty(ContentDefinition.IAPProduct))
			{
				titleLabel.text = LocalizationManager.GetText("Popup.IAPConfirm.ReceivedGift");
			}
			else
			{
				titleLabel.text = (bundleGivenBySupport ? LocalizationManager.GetText("Popup.IAPConfirm.ReceivedFromSupport") : LocalizationManager.GetText("Popup.IAPConfirm.Title"));
			}
		}
	}

	private void PlayCurrentPanel()
	{
		bool lastPanel = CurrentPanelIndex + 1 >= AllPanelList.Count;
		bool firstPanel = CurrentPanelIndex == 0;
		if (CurrentPanelIndex < AllPanelList.Count && AllPanelList[CurrentPanelIndex] != null)
		{
			AllPanelList[CurrentPanelIndex].StartPlaying(PanelComplete, lastPanel, firstPanel);
		}
	}

	private void PanelComplete()
	{
		if (CurrentPanelIndex + 1 < AllPanelList.Count)
		{
			AllPanelList[CurrentPanelIndex].Hide();
			CurrentPanelIndex++;
			PlayCurrentPanel();
		}
		else
		{
			DebugLog("All Panels Complete");
			enableContinueButton = true;
		}
	}

	private AnimateIapRewardsNew GetCurrentPanel()
	{
		if (AllPanelList != null && AllPanelList.Count > CurrentPanelIndex && AllPanelList[CurrentPanelIndex] != null)
		{
			return AllPanelList[CurrentPanelIndex];
		}
		return null;
	}

	private IEnumerator BlockSkipForSeconds(float delay)
	{
		SkipBlocked = true;
		yield return new WaitForSeconds(delay);
		SkipBlocked = false;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnNewEquipmentCardSelected")
		{
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton != null && equipmentButton.GetEquipment() != null && GetCurrentPanel() != null && GetCurrentPanel().IsLastToBeLeftVisible())
			{
				EquipmentUpgradePopup equipmentUpgradePopup = Helpers.OpenEquipmentUpgradePopup(equipmentButton.GetEquipment());
				equipmentUpgradePopup.ShowNextLevel = false;
				equipmentUpgradePopup.ShowEquipmentReceivedVersion();
				CampHUD.Get().PauseCurrencyMeters = false;
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
				Close();
			}
		}
	}

	private bool AddRewardsToPanel(AnimateIapRewardsNew panel)
	{
		BundleManagerModel bundleManager = GameManager.Instance.playerModel.BundleManager;
		if (bundleManager != null && panel != null && ContentDefinition != null && ContentDefinition.RewardEntries != null && ContentDefinition.RewardEntries.RewardsList != null)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < ContentDefinition.RewardEntries.RewardsList.Count; i++)
			{
				if (ContentDefinition.RewardEntries.RewardsList[i] == null)
				{
					continue;
				}
				bool flag = bundleManager.PendingViewBundleContentDefinition == ContentDefinition.Identifier;
				bool flag2 = panel.GetPanelType() == AnimateIapRewardsNew.PanelType.Equipment;
				bool flag3 = panel.GetPanelType() == AnimateIapRewardsNew.PanelType.EquipmentToken;
				bool flag4 = panel.GetPanelType() == AnimateIapRewardsNew.PanelType.Outfit;
				bool flag5 = panel.GetPanelType() == AnimateIapRewardsNew.PanelType.HeroSkin;
				bool flag6 = ContentDefinition.RewardEntries.RewardsList[i].Type == RewardType.Equipment || ContentDefinition.RewardEntries.RewardsList[i].Type == RewardType.RandomEquipment;
				bool flag7 = flag6 && ContentDefinition.RewardEntries.RewardsList[i] is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager);
				bool flag8 = ContentDefinition.RewardEntries.RewardsList[i].Type == RewardType.Outfit;
				bool flag9 = ContentDefinition.RewardEntries.RewardsList[i].Type == RewardType.HeroSkin;
				bool num5 = ContentDefinition.RewardEntries.RewardsList[i].Type == RewardType.EquipToken;
				EquipmentItemModel equipment = null;
				EquipTokenItemModel equipTokenItemModel = null;
				OutfitDefinition outfit = null;
				HeroSkinDefinition heroSkinDefinition = null;
				if (num5)
				{
					if (flag3 && bundleManager.PendingViewEquipTokens != null && bundleManager.PendingViewEquipTokens.Count > num2)
					{
						equipTokenItemModel = bundleManager.PendingViewEquipTokens[num2];
						num2++;
						if (equipTokenItemModel.manager != null)
						{
							panel.AddReward(ContentDefinition.RewardEntries.RewardsList[i], null, null, null, equipTokenItemModel);
						}
					}
				}
				else if (flag6 && !flag7)
				{
					if (flag2 && flag && bundleManager.PendingViewEquipments != null && bundleManager.PendingViewEquipments.Count > num)
					{
						equipment = bundleManager.PendingViewEquipments[num];
						num++;
						if (equipment.manager != null)
						{
							panel.AddReward(ContentDefinition.RewardEntries.RewardsList[i], equipment, outfit);
						}
					}
				}
				else if (flag8)
				{
					if (flag4 && flag && bundleManager.PendingViewOutfits != null && bundleManager.PendingViewOutfits.Count > num3)
					{
						outfit = GameManager.Instance.gameEconomyData.GetOutfitDefinition(bundleManager.PendingViewOutfits[num3]);
						num3++;
						panel.AddReward(ContentDefinition.RewardEntries.RewardsList[i], equipment, outfit);
					}
				}
				else if (flag9)
				{
					if (flag5 && flag && bundleManager.PendingViewHeroSkins != null && bundleManager.PendingViewHeroSkins.Count > num4)
					{
						heroSkinDefinition = GameManager.Instance.gameEconomyData.GetSkinDefinition(bundleManager.PendingViewHeroSkins[num4]);
						num4++;
						panel.AddReward(ContentDefinition.RewardEntries.RewardsList[i], equipment, outfit, heroSkinDefinition);
					}
				}
				else if (!flag4 && !flag2 && !flag5 && !flag3)
				{
					panel.AddReward(ContentDefinition.RewardEntries.RewardsList[i]);
				}
			}
			return panel.GetRewardsListCount() > 0;
		}
		return false;
	}

	private void MarkBundleAsViewed()
	{
		BundleManagerModel bundleManager = GameManager.Instance.playerModel.BundleManager;
		if (bundleManager != null && bundleManager.PendingViewBundleContentDefinition != null && ContentDefinition != null && bundleManager.PendingViewBundleContentDefinition == ContentDefinition.Identifier)
		{
			Helpers.ExecuteCommand(new BuyBundleViewedCommand(bundleManager));
		}
	}

	private void PlayRevealCurrencyAnimationIfMetersArePaused(RewardCurrency currency)
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager) return");
			return;
		}
		CampHUD campHUD = CampHUD.Get();
		if (campHUD != null && campHUD.PauseCurrencyMeters)
		{
			campHUD.AddToMeter(currency.CurrencyType, currency.Amount);
			campHUD.GetComponent<BuildingsHUD>().CreateCollectAnim(currency.CurrencyType, base.gameObject, currency.Amount, null, BuildingsHUD.CollectSoundTrigger.OnStart, base.gameObject);
		}
	}

	public void OpenForRewards(IList<IReward> rewards)
	{
		Reset();
		rewards = CombineRewards(rewards);
		cachedRewards = rewards;
		for (int i = 0; i < rewards.Count; i++)
		{
			IReward reward = rewards[i];
			EquipmentItemModel equipment = null;
			EquipTokenItemModel equipTokenItemModel = null;
			OutfitDefinition outfit = null;
			HeroSkinDefinition heroSkin = null;
			IReward reward2 = reward;
			AnimateIapRewardsNew animateIapRewardsNew;
			IReward reward3;
			if (!(reward2 is RewardAvatars))
			{
				if (!(reward2 is RewardSevenDayPremium))
				{
					if (!(reward2 is RewardCurrency))
					{
						if (!(reward2 is RewardEquipment rewardEquipment))
						{
							if (!(reward2 is RewardEquipToken rewardEquipToken))
							{
								if (!(reward2 is RewardRandomEquipment rewardRandomEquipment))
								{
									if (!(reward2 is RewardOutfit rewardOutfit))
									{
										if (!(reward2 is RewardHeroSkin rewardHeroSkin))
										{
											if (!(reward2 is RewardMissingTokens rewardMissingTokens))
											{
												if (!(reward2 is RewardTimedBonus))
												{
													if (!(reward2 is RewardRemoldSkill))
													{
														continue;
													}
													animateIapRewardsNew = CurrencyRewardParent;
													reward3 = reward;
												}
												else
												{
													animateIapRewardsNew = CurrencyRewardParent;
													reward3 = reward;
												}
											}
											else
											{
												animateIapRewardsNew = CurrencyRewardParent;
												reward3 = new RewardCurrency
												{
													CurrencyType = rewardMissingTokens.RewardCurrencyType,
													Amount = GameManager.Instance.playerModel.BlackMarket.LastAmountMissingTokensGiven,
													IsDiamondExchange = false
												};
											}
										}
										else
										{
											reward3 = reward;
											heroSkin = GameManager.Instance.gameEconomyData.GetSkinDefinition(rewardHeroSkin.PreferredOrder[0]);
											animateIapRewardsNew = HeroSkinRewardParent;
										}
									}
									else
									{
										reward3 = reward;
										outfit = GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]);
										animateIapRewardsNew = OutfitRewardParent;
									}
								}
								else
								{
									animateIapRewardsNew = EquipmentRewardParent;
									reward3 = new RewardEquipment();
									equipment = rewardRandomEquipment.GivenEquipment;
								}
							}
							else
							{
								equipTokenItemModel = rewardEquipToken.GivenEquipmentToken;
								animateIapRewardsNew = EquipmentTokenRewardParent;
								reward3 = reward;
							}
						}
						else if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
						{
							animateIapRewardsNew = CurrencyRewardParent;
							reward3 = reward;
						}
						else
						{
							RewardEquipment rewardEquipment2 = rewardEquipment;
							if (rewardEquipment2.IsConsumableReward(GameManager.Instance.modelManager))
							{
								continue;
							}
							equipment = rewardEquipment2.GivenEquipment;
							animateIapRewardsNew = EquipmentRewardParent;
							reward3 = reward;
						}
					}
					else
					{
						animateIapRewardsNew = CurrencyRewardParent;
						reward3 = reward;
					}
				}
				else
				{
					animateIapRewardsNew = CurrencyRewardParent;
					reward3 = reward;
				}
			}
			else
			{
				animateIapRewardsNew = CurrencyRewardParent;
				reward3 = reward;
			}
			if (!AllPanelList.Contains(animateIapRewardsNew))
			{
				AllPanelList.Add(animateIapRewardsNew);
			}
			animateIapRewardsNew.AddReward(reward3, equipment, outfit, heroSkin, equipTokenItemModel);
		}
		Open();
		PlayCurrentPanel();
		enableContinueButton = false;
	}

	public void DisableSkipButton()
	{
		Helpers.GameObjectSetActive(ButtonClose, value: false);
	}

	public override void OnClickClose()
	{
		Close();
	}

	public void SetCloseAnimOverCallback(Callback callback)
	{
		closeAnimOverCallback = callback;
	}

	protected override void OnCloseAnimOver()
	{
		base.OnCloseAnimOver();
		if (closeAnimOverCallback != null)
		{
			closeAnimOverCallback();
		}
	}
}
