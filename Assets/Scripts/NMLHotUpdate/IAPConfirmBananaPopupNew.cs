using System.Collections;
using System.Collections.Generic;
using System.Text;
using TWDModel;
using UnityEngine;

public class IAPConfirmBananaPopupNew : ConfirmationPopup
{
	[Tooltip("Panel used for showing the currency rewards plus survivor slots")]
	[SerializeField]
	private AnimateIapRewardsNew CurrencyRewardParent;

	[Tooltip("Panel used for showing the equipment rewards")]
	[SerializeField]
	private AnimateIapRewardsNew EquipmentRewardParent;

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

	private TradefairBundleContentDefinition TradefairContentDefinition;

	private IList<IReward> cachedRewards;

	private bool enableContinueButton;

	private Callback closeAnimOverCallback;

	private string contentStr = "";

	private bool needMarkFlag;

	public bool ShowShopWhenClosed { get; set; }

	public static IAPConfirmBananaPopupNew OpenWithBundleContent(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition, bool givenBySupport)
	{
		if (bundleContentDefinition.Category == BundleContentDefinition.CategoryHidden)
		{
			return null;
		}
		IAPConfirmBananaPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmBananaPopupNew) as IAPConfirmBananaPopupNew;
		obj.needMarkFlag = false;
		obj.OpenForBundleContentDefinition(bundleStoreDefinition, bundleContentDefinition, givenBySupport);
		obj.MarkBundleAsViewed();
		return obj;
	}

	public static IAPConfirmBananaPopupNew OpenWithTradeFairBundleContent(TradefairBundleStoreDefinition bundleStoreDefinition, TradefairBundleContentDefinition bundleContentDefinition, bool givenBySupport)
	{
		IAPConfirmBananaPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmBananaPopupNew) as IAPConfirmBananaPopupNew;
		obj.needMarkFlag = false;
		obj.OpenForTradeFairBundleContentDefinition(bundleStoreDefinition, bundleContentDefinition, givenBySupport);
		obj.MarkBundleAsViewed();
		return obj;
	}

	public static IAPConfirmBananaPopupNew OpenWithBundleContentLogin(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition, bool givenBySupport, bool isLast)
	{
		IAPConfirmBananaPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmBananaPopupNew) as IAPConfirmBananaPopupNew;
		obj.needMarkFlag = false;
		obj.MarkBundleAsViewed();
		obj.OpenForBundleContentDefinition(bundleStoreDefinition, bundleContentDefinition, givenBySupport);
		return obj;
	}

	public static IAPConfirmBananaPopupNew OpenWithTradeFairBundleContentLogin(TradefairBundleStoreDefinition bundleStoreDefinition, TradefairBundleContentDefinition bundleContentDefinition, bool givenBySupport, bool isLast)
	{
		IAPConfirmBananaPopupNew obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmBananaPopupNew) as IAPConfirmBananaPopupNew;
		obj.needMarkFlag = false;
		obj.MarkBundleAsViewed();
		obj.OpenForTradeFairBundleContentDefinition(bundleStoreDefinition, bundleContentDefinition, givenBySupport);
		return obj;
	}

	public void OpenForTradeFairBundleContentDefinition(TradefairBundleStoreDefinition bundleStoreDefinition, TradefairBundleContentDefinition bundleContentDefinition, bool givenBySupport)
	{
		bundleGivenBySupport = givenBySupport;
		TradefairContentDefinition = bundleContentDefinition;
		if (contentStr == "")
		{
			Open();
		}
		OnOpenTradeFairListClicked();
	}

	public void OpenForBundleContentDefinition(BundleStoreDefinition bundleStoreDefinition, BundleContentDefinition bundleContentDefinition, bool givenBySupport)
	{
		bundleGivenBySupport = givenBySupport;
		ContentDefinition = bundleContentDefinition;
		if (contentStr == "")
		{
			Open();
		}
		OnOpenListClicked();
	}

	public override void Update()
	{
	}

	public override void Open()
	{
		DebugClassString = "IAPConfirmBananaPopupNew";
		base.Open();
	}

	public override void Close()
	{
		base.Close();
		CheckForBonusGift();
	}

	private void CheckForBonusGift()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.BundleManager != null && (playerModel.BundleManager.IAPBonusGiftLootEntry != null || playerModel.BundleManager.WebShopLootEntrys.Count > 0) && SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.OpenLootInUi) == null)
		{
			OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				openLootInUi.OpenForModel(GameManager.Instance.playerModel.BundleManager);
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
		if (needMarkFlag)
		{
			MarkBundleAsViewed();
		}
		CheckForBonusGift();
	}

	public void OnOpenTradeFairListClicked()
	{
		IList<IReward> list = new List<IReward>();
		if (TradefairContentDefinition != null && TradefairContentDefinition.RewardEntries != null && TradefairContentDefinition.RewardEntries.RewardsList != null)
		{
			for (int i = 0; i < TradefairContentDefinition.RewardEntries.RewardsList.Count; i++)
			{
				list.Add(TradefairContentDefinition.RewardEntries.RewardsList[i]);
			}
			if (TradefairContentDefinition.ExtraRewardEntries != null && TradefairContentDefinition.ExtraRewardEntries.RewardsList != null)
			{
				for (int j = 0; j < TradefairContentDefinition.ExtraRewardEntries.RewardsList.Count; j++)
				{
					list.Add(TradefairContentDefinition.ExtraRewardEntries.RewardsList[j]);
				}
			}
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
		contentStr = ((contentStr == "") ? stringBuilder.ToString() : (contentStr + stringBuilder.ToString()));
		HelpersUI.SetContentToLabel(ListLabel, contentStr);
	}

	public void OnOpenListClicked()
	{
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
		contentStr = ((contentStr == "") ? stringBuilder.ToString() : (contentStr + stringBuilder.ToString()));
		HelpersUI.SetContentToLabel(ListLabel, contentStr);
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
				EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
				obj.ShowNextLevel = false;
				obj.OpenForModel(equipmentButton.GetEquipment());
				obj.ShowEquipmentReceivedVersion();
				CampHUD.Get().PauseCurrencyMeters = false;
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
				Close();
			}
		}
	}

	private void MarkBundleAsViewed()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.WebshopBuyedBundleSingularSyncDatas != null)
		{
			SingularityMonoBehaviour<SDKManager>.Instance.SentWebShopData(GameManager.Instance.playerModel.WebshopBuyedBundleSingularSyncDatas);
		}
	}

	public override void OnClickClose()
	{
		if (needMarkFlag)
		{
			MarkBundleAsViewed();
		}
		Close();
	}

	protected override void OnCloseAnimOver()
	{
		base.OnCloseAnimOver();
		if (!needMarkFlag)
		{
			return;
		}
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null && playerModel.WebShopBuyedTradeFairBundleIds != null && playerModel.WebShopBuyedTradeFairBundleIds.Count > 0)
		{
			List<string> webShopBuyedTradeFairBundleIds = GameManager.Instance.playerModel.WebShopBuyedTradeFairBundleIds;
			if (webShopBuyedTradeFairBundleIds.Count > 0)
			{
				for (int i = 0; i < webShopBuyedTradeFairBundleIds.Count; i++)
				{
					TradefairBundleStoreDefinition bundleTradefairDefinition = GameManager.Instance.gameEconomyData.GetBundleTradefairDefinition(webShopBuyedTradeFairBundleIds[i]);
					TradefairBundleContentDefinition tradefairBundleContentDefinition = GameManager.Instance.gameEconomyData.GetTradefairBundleContentDefinition(bundleTradefairDefinition.BundleIdentifier);
					UIEvent.Send("OnBundleBought", bundleTradefairDefinition);
					bool isLast = false;
					_ = webShopBuyedTradeFairBundleIds.Count;
					if (i == webShopBuyedTradeFairBundleIds.Count - 1)
					{
						isLast = true;
					}
					OpenWithTradeFairBundleContentLogin(bundleTradefairDefinition, tradefairBundleContentDefinition, givenBySupport: false, isLast);
				}
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
				}
				return;
			}
			Debug.LogError("MEIYOUDE");
		}
		if (playerModel == null || playerModel.WebShopBuyedBundleIds == null || playerModel.WebShopBuyedBundleIds.Count <= 0)
		{
			return;
		}
		List<string> webShopBuyedBundleIds = GameManager.Instance.playerModel.WebShopBuyedBundleIds;
		if (webShopBuyedBundleIds.Count > 0)
		{
			for (int j = 0; j < webShopBuyedBundleIds.Count; j++)
			{
				BundleStoreDefinition bundleStoreDefinition = GameManager.Instance.gameEconomyData.GetBundleStoreDefinition(webShopBuyedBundleIds[j]);
				BundleContentDefinition bundleContentDefinition = GameManager.Instance.gameEconomyData.GetBundleContentDefinition(bundleStoreDefinition.BundleIdentifier);
				UIEvent.Send("OnBundleBought", bundleStoreDefinition);
				bool isLast2 = false;
				_ = webShopBuyedBundleIds.Count;
				if (j == webShopBuyedBundleIds.Count - 1)
				{
					isLast2 = true;
				}
				OpenWithBundleContentLogin(bundleStoreDefinition, bundleContentDefinition, givenBySupport: false, isLast2);
			}
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
			}
		}
		else
		{
			Debug.LogError("MEIYOUDE");
		}
	}
}
