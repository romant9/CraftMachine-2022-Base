using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using static PhoneWeaponContainer;
using static TWDModel.EquipPrizeWheelModel;

public class GoldRadioWeaponContainer : MonoBehaviourExtended
{
	[SerializeField]
	private GoldRadioWeaponListPanel goldRadioListPanel;

	[SerializeField]
	private UILabel weaponLabel;

	[SerializeField]
	private UILabel timeLabel;

	[SerializeField]
	private UITexture rewardTexture;

	[SerializeField]
	private UITexture cdnTexture;

	[SerializeField]
	private UILabel luckyLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel oneButtonLabel;

	[SerializeField]
	private UILabel oneTimeLabel;

	[SerializeField]
	private UILabel tenButtonLabel;

	[SerializeField]
	private UILabel oldTenTimeLabel;

	[SerializeField]
	private UILabel tenTimeLabel;

	[SerializeField]
	private GameObject goldRadioBG;

	[SerializeField]
	private GameObject goldRadioBG1;

	[SerializeField]
	private GameObject goldRadioOneBtIcon;

	[SerializeField]
	private GameObject goldRadioTenBtIcon;

	[SerializeField]
	private GameObject bgEffect;

	[SerializeField]
	private GoldRadioWeaponRightListPanel goldRadioWeaponRightListPanel;

	[SerializeField]
	private UIButton oneButton;

	[SerializeField]
	private UIButton tenButton;

	[SerializeField]
	private UIButton detailButton;

	private EquipPrizeWheelDefinition _currentEquipPrizeWheelDefinition;

	private GoldRadioCallDenifition _currentGoldRadioCallDefinition;

	private void Awake()
	{
		oneButton.onClick.Add(new EventDelegate(OnClickOneButton));
		tenButton.onClick.Add(new EventDelegate(OnClickTenButton));
		detailButton.onClick.Add(new EventDelegate(OnClickDetailButton));
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;

		if (IsLoadDataManager)
		{
			PlayerRandomValues.Instance.On_Call_Reset += OnClickReset;
			PlayerRandomValues.Instance.On_Call_Change += OnCounterChange;
			CallCraft.Instance.AddRadiosAction += ChangeRadios;
			selectWeaponsPopupCurrent = null;
		}
	}

	public void OnDisable()
	{
		if (IsLoadDataManager)
		{
			PlayerRandomValues.Instance.On_Call_Reset -= OnClickReset;
			PlayerRandomValues.Instance.On_Call_Change -= OnCounterChange;
			CallCraft.Instance.AddRadiosAction -= ChangeRadios;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		if (_currentEquipPrizeWheelDefinition != null)
		{
			OnClickTab(_currentEquipPrizeWheelDefinition);
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "PhoneGoldRadioSelected":
			if (parameter is EquipPrizeWheelDefinition definition)
			{
				OnClickTab(definition);
				if (!OfflineManager.IsLoadDataManager)
				{
					NewPhonePopup newPhonePopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup;
					if (newPhonePopup2 != null)
					{
						newPhonePopup2.ShowPhone(flag: false);
					}
				}
			}
			goldRadioListPanel.Reposition();
			break;
		case "PhoneGoldRadioSelectedReposition":
			goldRadioListPanel.Reposition();
			break;
		case "PhoneGoldRadioChanged":
			if (!OfflineManager.IsLoadDataManager)
			{
				if (_currentEquipPrizeWheelDefinition != null)
				{
					NewPhonePopup newPhonePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup;
					if (newPhonePopup != null)
					{
						newPhonePopup.ShowPhone(flag: false);
					}
				}
			}
			goldRadioListPanel.Reposition(resetScrollView: true);
			break;
		}
	}

	public void Open()
	{
		List<EquipPrizeWheelDefinition> definitions = (from d in GameManager.Instance.gameEconomyData.GetOpenEquipPrizeWheelDefinition(GameManager.Instance.playerModel.UtcTimeStamp)
			where d.RadioType == RadioType.GoldRadio
			orderby d.Order descending, d.Identifier descending
			select d).ToList();
		if (goldRadioListPanel != null)
		{
			goldRadioListPanel.Init(definitions);
			UIListCard<EquipPrizeWheelDefinition> cardAt = goldRadioListPanel.getCardAt(0);
			if (cardAt != null && cardAt.Item != null)
			{
				UIEvent.Send("PhoneGoldRadioSelected", cardAt.Item);
			}
			if (RewardTypeSkillList) lastRewardValue = RewardTypeSkillList.value;
		}
	}

	private void OnClickTab(EquipPrizeWheelDefinition definition)
	{
		if (definition != null)
		{
			_currentEquipPrizeWheelDefinition = definition;
			_currentGoldRadioCallDefinition = GameManager.Instance.gameEconomyData.GetGoldRadioCallDenifitionByID(definition.Identifier);
			goldRadioWeaponRightListPanel.Init(_currentGoldRadioCallDefinition?.Class);
			HelpersUI.SetContentToLabel(weaponLabel, LocalizationManager.GetText(definition.NameLocKey));
			DateTime dateTime = GameEconomyData.ParseDateTime(definition.StartTimeUtc);
			DateTime dateTime2 = GameEconomyData.ParseDateTime(definition.EndTimeUtc);
			HelpersUI.SetContentToLabel(timeLabel, $"{dateTime:yyyy-MM-dd}~{dateTime2:yyyy-MM-dd}");
			RefreshRewardTexture(definition);
			RefreshLuckyValue(definition);
			HelpersUI.SetContentToLabel(descriptionLabel, LocalizationManager.GetText(definition.DescLocKey));
			HelpersUI.SetContentToLabel(oneButtonLabel, LocalizationManager.GetText("NewPhonePopup.PhoneWeapon.OneButton"));
			HelpersUI.SetContentToLabel(oneTimeLabel, $"x{definition.OncePrice}");
			HelpersUI.SetContentToLabel(tenButtonLabel, LocalizationManager.GetText("NewPhonePopup.PhoneWeapon.TenButton"));
			HelpersUI.SetContentToLabel(oldTenTimeLabel, $"x{definition.TenTimesOriginalPrice}");
			HelpersUI.SetContentToLabel(tenTimeLabel, $"x{definition.TenTimesPrice}");
			SetBgEffectColor();
		}
	}

	private void RefreshRewardTexture(EquipPrizeWheelDefinition definition)
	{
		if (!string.IsNullOrEmpty(definition.cdnIcon))
		{
			Helpers.GameObjectSetActive(cdnTexture, value: true);
			Helpers.GameObjectSetActive(rewardTexture, value: false);
			LoadImageFromCdn.LoadImageToTarget(cdnTexture, definition.cdnIcon);
			return;
		}
		Helpers.GameObjectSetActive(cdnTexture, value: false);
		Helpers.GameObjectSetActive(rewardTexture, value: true);
		Texture texture = UnityUtils.LoadFromAssetBundle<Texture>(definition.HighlightedReward, "itemgraphics");
		Texture value = UnityUtils.LoadFromAssetBundle<Texture>(definition.HighlightedReward + "_alpha", "itemgraphics");
		rewardTexture.mainTexture = texture;
		rewardTexture.material.SetTexture("_MainTex", texture);
		rewardTexture.material.SetTexture("_AlphaTex", value);
	}

	private void RefreshLuckyValue(EquipPrizeWheelDefinition definition)
	{
		var equipPrizeWheelModelOrigin = GameManager.Instance.playerModel.EquipPrizeWheelModel;

		equipPrizeWheelModelOrigin.CurrentEquipPrizeWheelDefinition = definition;
		int currentLuckyTime = equipPrizeWheelModelOrigin.GetCurrentLuckyTime();

		//(GameManager.Instance.playerModel?.EquipPrizeWheelModel?.GoldRadioLuckyTimeDict)?.TryGetValue(definition.Identifier, out value);
		int valueOrDefault = (GameManager.Instance.gameEconomyData.ConfigData?.EquipPrizeWheelLuckPoint_GoldRadio).GetValueOrDefault();
		LuckyTimeGoldDefault = valueOrDefault;

		if (IsLoadDataManager)
		{
			if (!IsInitialized)
			{
				callTypeList = new List<int>();
				IsInitialized = true;
				goldRadioCount = Player.GetCurrency(CurrencyType.GoldRadio).Value;
				playerPrizeSkillCounterLabel.text = playerPrizeSkillCounter.ToString();
				foreach (var lucky in equipPrizeWheelModelOrigin.GoldRadioLuckyTimeDict)
				{
					goldRadioLuckyTimeDict.Add(lucky.Key, lucky.Value);
				}
			}

			string _luckyLabel;
			if (LocalizationManager.Instance.CurrentLanguage == "ru")
			{
				_luckyLabel = "Очки удачи: " + currentLuckyTime + '/' + valueOrDefault;
			}
			else
			{
				_luckyLabel = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.Lucky", currentLuckyTime, valueOrDefault);
			}
			HelpersUI.SetContentToLabel(luckyLabel, _luckyLabel);
		}
		else
		{
			HelpersUI.SetContentToLabel(luckyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.Lucky", currentLuckyTime, valueOrDefault));
		}
	}

	public void OnClickDetailButton()
	{
		if (_currentGoldRadioCallDefinition != null && _currentEquipPrizeWheelDefinition != null)
		{
			TweenFavoriteSign(false);
			isFavoriteSeen = true;
			GoldRadioCallDetailPopup goldRadioCallDetailPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GoldRadioCallDetailPopup, HUDManager.Instance.UIContainerTopCameras) as GoldRadioCallDetailPopup;
			if (goldRadioCallDetailPopup != null)
			{
				goldRadioCallDetailPopup.Open(_currentEquipPrizeWheelDefinition, _currentGoldRadioCallDefinition);
			}
		}
	}

	public void OnClickOneButton()
	{
		if (_currentEquipPrizeWheelDefinition != null)
		{
			equipPrizeType = EquipPrizeType.One;

			if (IsLoadDataManager && IsAuto && !CoroutineStarted)
			{
				StartCoroutine(ExecuteCallBatch(EquipPrizeType.One));
			}
			else
			{
				int oncePrice = _currentEquipPrizeWheelDefinition.OncePrice;
				int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GoldRadio).Value;
				if (oncePrice > value && !OfflineManager.IsFreeAll)
				{
					ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(oncePrice, CurrencyType.GoldRadio);
				}
				else
				{
					ExecuteCallCommand(EquipPrizeType.One);
				}
			}
		}
	}

	public void OnClickTenButton()
	{
		if (_currentEquipPrizeWheelDefinition != null)
		{
			equipPrizeType = EquipPrizeType.Ten;

			if (IsLoadDataManager && IsAuto && !CoroutineStarted)
			{
				StartCoroutine(ExecuteCallBatch(EquipPrizeType.Ten));
			}
			else
			{
				int tenTimesPrice = _currentEquipPrizeWheelDefinition.TenTimesPrice;
				int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GoldRadio).Value;
				if (tenTimesPrice > value && !OfflineManager.IsFreeAll)
				{
					ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(tenTimesPrice, CurrencyType.GoldRadio);
				}
				else
				{
					ExecuteCallCommand(EquipPrizeType.Ten);
				}
			}
		}
	}

	private void ExecuteCallCommand(EquipPrizeType prizeType)
	{
		CurrentPrizeType = prizeType;
		if (_currentEquipPrizeWheelDefinition == null)
		{
			Debug.LogError("[GoldRadioWeaponContainer] ExecuteCallCommand: _currentEquipPrizeWheelDefinition is NULL");
			return;
		}
		if (IsLoadDataManager)
		{
			var manager = Player.manager;
			EquipPrizeWheelModel equipPrizeWheelModel = Player.EquipPrizeWheelModel;
			if (equipPrizeWheelModel == null)
			{
				return;
			}
			EquipPrizeWheelDefinition equipPrizeWheelDefinition = Player.gameEconomyData.GetEquipPrizeWheelDefinition(_currentEquipPrizeWheelDefinition.Identifier);
			if (equipPrizeWheelDefinition == null || !equipPrizeWheelDefinition.IsOpen(Player.UtcTimeStamp))
			{
				return;
			}
			Cashier cashier = null;
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (!IsAuto && !isFree && !OfflineManager.IsFreeAll)
			{
				cashier = Cashier.CreateOneItemCashier(manager, PurchaseType.EquipPrize, CurrencyType.GoldRadio, (prizeType == EquipPrizeType.Ten) ? equipPrizeWheelDefinition.TenTimesPrice : equipPrizeWheelDefinition.OncePrice);
				cashier.UseDiamondsAmount = -2;
				tWDModelResult = cashier.Pay();
				if (tWDModelResult != 0)
				{
					return;
				}
			}

			callCounter += 1;
			CallCounterLabel.text = callCounter.ToString();

			playerPrizeSkillCounter += prizeType == EquipPrizeType.One ? 1 : 10;
			tenTimesPrice = equipPrizeWheelDefinition.TenTimesPrice;
			playerGoldRadioCounter += prizeType == EquipPrizeType.One ? 1 : tenTimesPrice;
			callTypeList.Add(prizeType == EquipPrizeType.One ? 1 : 10);
			playerPrizeSkillCounterLabel.text = playerPrizeSkillCounter.ToString() + " / " + playerGoldRadioCounter.ToString();

			equipPrizeWheelModel.CurrentEquipPrizeType = prizeType;
			equipPrizeWheelModel.CurrentEquipPrizeWheelDefinition = equipPrizeWheelDefinition;
			equipPrizeWheelModel.AddReward(prizeType, equipPrizeWheelDefinition.SlotNumber); //RemoldSkill Hunter_4002

			if (!IsAuto && OfflineManager.IsUseSendMetrics && cashier != null)
			{
				Metrics metrics = manager.Metrics;
				metrics.ResourceChangeUsedReason = "EquipmentCall";
				metrics.AddItemChange().AddResources(cashier).Send();
				manager.TdMetrics.SetEventType("equipment_call").AddProperty("equip_prize_phone_used", (prizeType == EquipPrizeType.Ten) ? equipPrizeWheelDefinition.TenTimesPrice : equipPrizeWheelDefinition.OncePrice).AddProperty("equip_prize_choose_type", prizeType)
					.AddProperty("equip_prize_call_slot", equipPrizeWheelDefinition.SlotNumber)
					.AddProperty("equip_prize_acceptance", equipPrizeWheelDefinition.SlotNumber)
					.Send();
			}

			if (IsAuto && IsQuick)
			{
				IsCallDone = true;
			}
			else
			{
				OnWeaponCall(tWDModelResult);
			}
		}
		else
		{
			ConsumeCurrencyCommandUtils.Execute(new EquipPrizeWheelCommand(prizeType, _currentEquipPrizeWheelDefinition.Identifier)
			{
				Cashier = EquipPrizeWheelCommand.GetCashier(GameManager.Instance.modelManager, _currentEquipPrizeWheelDefinition.Identifier, prizeType)
			}, OnWeaponCall);
		}
	}

	private void OnWeaponCall(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			if (!IsLoadDataManager && !GameManager.Instance.CheckConnectionReachability(showPopup: true, "EquipPrizeWheelCommand"))
			{
				if (!OfflineManager.IsIgnoreReconnect) VisualizationQueue.Instance.GameDisconnected();
			}
			SelectWeaponsPopup selectWeaponsPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RadioSelectWeaponPopup, prefabVariant: IsLoadDataManager ? SelectWeaponsPopupPrefab : null) as SelectWeaponsPopup;
			if (selectWeaponsPopup != null)
			{
				if (IsLoadDataManager)
				{
					selectWeaponsPopupCurrent = selectWeaponsPopup;

					selectWeaponsPopup.Clear();
					selectWeaponsPopup.Open();
					selectWeaponsPopup.OnClose -= SetWeaponPopupNull;
					selectWeaponsPopup.OnClose += SetWeaponPopupNull;
				}
				else
				{
					selectWeaponsPopup.Open();
				}
			}
			UIEvent.Send("PhoneWeaponDrawCardDone");
			if (_currentEquipPrizeWheelDefinition != null)
			{
				OnClickTab(_currentEquipPrizeWheelDefinition);
			}
		}
		IsCallDone = true;
	}

	public void OnShowWeaponButton()
	{
		EquipmentUpgradePopup equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		if (!(equipmentUpgradePopup == null))
		{
			equipmentUpgradePopup.ShowNextLevel = false;
			equipmentUpgradePopup.OpenForPreview(_currentEquipPrizeWheelDefinition.HighlightedRewardTexture, 6);
		}
	}

	private void SetBgEffectColor()
	{
		if (!(bgEffect == null))
		{
			UIWidget component = bgEffect.GetComponent<UIWidget>();
			if (component != null)
			{
				component.color = new Color(1f, 1f, 1f, 0.716f);
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (goldRadioListPanel != null)
		{
			goldRadioListPanel.ClearCards();
		}
		_currentEquipPrizeWheelDefinition = null;
		_currentGoldRadioCallDefinition = null;
	}


	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private PlayerModel Player => GameManager.Instance.playerModel;

	public EquipPrizeType equipPrizeType { get; set; }
	public bool IsQuick { get; set; }
	public bool IsAuto { get; set; }
	public bool isFree { get; set; }

	private bool CoroutineStarted;
	public bool IsFineConstructFinded { get; set; }
	public int batchCount { get; set; }
	public bool IsCallDone { get; set; }

	public SelectWeaponsPopup selectWeaponsPopupCurrent { get; private set; }
	public List<RewardRemoldSkill> addedRewardSkillList { get; private set; } = new List<RewardRemoldSkill>();
	public bool IsInitialized { get; private set; }

	//private int LuckyTimeGoldValue;
	private int LuckyTimeGoldDefault;

	public UIInput MaxCallsInput;
	private int MaxCalls;
	private int goldRadioCount;

	public List<int> callTypeList { get; private set; }

	[SerializeField]
	private UILabel playerPrizeSkillCounterLabel;
	//затраченные рации
	public int playerGoldRadioCounter { get; private set; }
	//призы (карточки)
	public int playerPrizeSkillCounter { get; private set; }
	public UILabel CallCounterLabel;
	private int callCounter;
	public EquipPrizeType CurrentPrizeType = EquipPrizeType.One;
	private int tenTimesPrice = 8;

	private Dictionary<string, int> goldRadioLuckyTimeDict { get; set; } = new Dictionary<string, int>();
	public GameObject SelectWeaponsPopupPrefab;

	public UIPopupList RewardTypeSkillList;
	public UIPopupList RewardStarsList;
	public int RewardStarValue { get; set; } = 1;
	public UILabel RewardTypeSkillListLabel;
	public PrizeCounterType CurrentRewardTypeSkill { get; private set; } = PrizeCounterType.Skill;
	public GameObject FavoriteSign;
	private bool isFavoriteSeen;

	public PrizeCounter FavoriteCounterSkill;
	public PrizeCounter FavoriteCounterSkillPart;
	#endregion

	#region mycode
	public IEnumerator ExecuteCallBatch(EquipPrizeType prizeType)
	{
		RandomValuesPopup randomValuesPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RandomValuesPopup) as RandomValuesPopup;
		if (randomValuesPopup != null)
		{
			if (!randomValuesPopup.IsOpen)
			{
				randomValuesPopup.Open();
			}

			IsFineConstructFinded = false;
			IsQuick = true;
			if (int.TryParse(MaxCallsInput.value, out int result))
			{
				MaxCalls = Mathf.FloorToInt(result / (prizeType == EquipPrizeType.One ? 1 : tenTimesPrice));
			}
			else
			{
				yield break;
			}

			for (int i = 0; i < MaxCalls; i++)
			{
				ExecuteCallCommand(prizeType);
				if (IsFineConstructFinded)
				{
					yield break;
				}
			}

			CoroutineStarted = true;
			Player.EquipPrizeWheelModel.SetIsAuto(true);
			batchCount = 0;

			while (true)
			{
				//int count = batchCount;
				//IsCallDone = false;
				//randomValuesPopup.Change_HubUp();

				//yield return new WaitUntil(() => IsCallDone || IsFineConstructFinded);
				//if (IsFineConstructFinded || count > 300 || !IsAuto)
				//{
				//	CoroutineStarted = false;
				//	IsFineConstructFinded = false;
				//	DebugTWD.Log("FineConstruct Finded for " + (count + 1).ToString() + "repeates");
				//	yield break;
				//}

				IsCallDone = false;
				randomValuesPopup.Change_HubUp();
				yield return null;

				if (IsFineConstructFinded || !IsAuto)
				{
					CoroutineStarted = false;
					IsFineConstructFinded = false;
					Player.EquipPrizeWheelModel.SetIsAuto(false);

					DebugTWD.Log("FineConstruct Finded for " + (batchCount + 1).ToString() + "repeates");
					yield break;
				}
			}
		}
		else
		{
			CoroutineStarted = false;
			IsFineConstructFinded = false;
			Player.EquipPrizeWheelModel.SetIsAuto(false);
			yield break;
		}
	}

	private void ChangeRadios()
	{
		goldRadioCount = DataManager.Instance.Player.GetCurrency(CurrencyType.GoldRadio).Value;
	}

	public void SetFree(UIToggle tg)
	{
		isFree = tg.value;
		NewPhonePopup.Instance.PhoneWeaponContainer.isFree = isFree;
	}

	public void OnCounterChange(int value)
	{
		RerollPrizeCalls();
	}

	private void RerollPrizeCalls()
	{
		if (callTypeList.Count > 0 && NewPhonePopup.Instance.IsWeaponSkillMode)
		{
			var copyOfcallTypeList = new List<int>();
			copyOfcallTypeList.AddRange(callTypeList);
			callTypeList.Clear();

			for (int i = 0; i < copyOfcallTypeList.Count; i++)
			{
				if (IsAuto && IsFineConstructFinded)
				{
					break;
				}
				if (copyOfcallTypeList[i] == 1)
				{
					ExecuteCallCommand(EquipPrizeType.One);
				}
				else
				{
					ExecuteCallCommand(EquipPrizeType.Ten);
				}
			}
			batchCount++;
		}
	}

	private void SetWeaponPopupNull(HUDElement element, HUDElementConfig hudElementConfig)
	{
		selectWeaponsPopupCurrent = null;
	}

	public void OnClickReset(bool isZeroCounter)
	{
		if (selectWeaponsPopupCurrent != null)
		{
			selectWeaponsPopupCurrent.OnClickClose();
		}

		string locLucky;
		int luckyTimeGoldValue = this.goldRadioLuckyTimeDict.TryGetValue(_currentEquipPrizeWheelDefinition.Identifier, out int value) ? value : 0;

		if (LocalizationManager.Instance.CurrentLanguage == "ru")
		{
			locLucky = "Очки удачи: " + luckyTimeGoldValue + '/' + LuckyTimeGoldDefault;
		}
		else
		{
			locLucky = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.Lucky", luckyTimeGoldValue, LuckyTimeGoldDefault);
		}
		HelpersUI.SetContentToLabel(luckyLabel, locLucky);

		var goldRadioLuckyTimeDict = GameManager.Instance.playerModel?.EquipPrizeWheelModel.GoldRadioLuckyTimeDict;
		if (goldRadioLuckyTimeDict != null)
		{
			goldRadioLuckyTimeDict.Clear();
			foreach (var lucky in this.goldRadioLuckyTimeDict)
			{
				goldRadioLuckyTimeDict.Add(lucky.Key, lucky.Value);
			}
			GameManager.Instance.playerModel.EquipPrizeWheelModel.GoldRadioLuckyTimeDict = goldRadioLuckyTimeDict;
		}
		//GoldRadioLuckyTimeDict.Clear();

		var ConfigData = GameManager.Instance.gameEconomyData?.ConfigData;
		if (ConfigData != null) ConfigData.EquipPrizeWheelLuckPoint_GoldRadio = LuckyTimeGoldDefault;

		var goldRadioAmount = Player.GetCurrencyAmount(CurrencyType.GoldRadio);
		var deltaGoldRadio = goldRadioCount - goldRadioAmount;
		Player.SetCurrency(CurrencyType.GoldRadio, deltaGoldRadio);
		var goldRadioAmountNew = Player.GetCurrencyAmount(CurrencyType.GoldRadio);
		NewPhonePopup.Instance.GoldRadiophonesAmountLabel.text = goldRadioAmountNew.ToString();

		CallCounterLabel.text = "0";
		callCounter = 0;
		playerPrizeSkillCounter = 0;
		playerGoldRadioCounter = 0;
		playerPrizeSkillCounterLabel.text = playerPrizeSkillCounter.ToString() + " / " + playerGoldRadioCounter.ToString();

		if (addedRewardSkillList.Count > 0)
		{
			var modSkillManager = GameManager.Instance.playerModel.ModSkillManager;
			for (int i=0; i < addedRewardSkillList.Count; i++)
			{
				var rewardRemoldSkill = addedRewardSkillList[i];
				rewardRemoldSkill?.Remove(modSkillManager);
			}
			addedRewardSkillList.Clear();
		}

		if (isZeroCounter)
		{
			callTypeList.Clear();
		}
		DebugTWD.Log("Current gold radio amount is " + goldRadioAmountNew);
	}

	public void SetAutoCall(UIToggle tg)
	{
		if (!NewPhonePopup.Instance.IsWeaponSkillMode) return;
		IsAuto = tg.value;
		NewPhonePopup.Instance.PhoneWeaponContainer.IsAuto = IsAuto;
		SetRewardTypeListValue();
		if (Player.EquipPrizeWheelModel != null && !IsAuto) Player.EquipPrizeWheelModel.SetIsAuto(false);

		Helpers.GameObjectSetActive(RewardTypeSkillList.gameObject, IsAuto);
		Helpers.GameObjectSetActive(MaxCallsInput.transform.parent.gameObject, IsAuto);
	}

	public void SetRewardStartsValue(UIPopupList uIPopupList)
	{
		RewardStarValue = uIPopupList.items.IndexOf(uIPopupList.value) + 1;
	}

	public void SetRewardTypeSkill(UIPopupList uIPopupList)
	{
		int index = uIPopupList.items.IndexOf(uIPopupList.value) + 5;
		CurrentRewardTypeSkill = (PrizeCounterType)index;
		lastRewardValue = uIPopupList.value;
	}
	private string lastRewardValue;

	public void SetIsFineConstructFinded(PrizeCounterType type, string skillType, bool isSkillPart, bool bigStar)
	{
		if (IsAuto && !IsFineConstructFinded)
		{
			if (!isSkillPart && CurrentRewardTypeSkill == PrizeCounterType.Favorites || isSkillPart && CurrentRewardTypeSkill == PrizeCounterType.FavoritesPart)
			{
				IsFineConstructFinded = NewPhonePopup.Instance.FavoriteModSkillList.Contains(skillType);
			}
			else
			{
				IsFineConstructFinded = bigStar &&
					(CurrentRewardTypeSkill == type ||
					(!isSkillPart && CurrentRewardTypeSkill == PrizeCounterType.Skill) ||
					(isSkillPart && CurrentRewardTypeSkill == PrizeCounterType.SkillPart));
			}
		}
	}

	public void SetRewardTypeListValue()
	{
		RewardTypeSkillList.gameObject.SetActive(IsAuto);
		MaxCallsInput.transform.parent.gameObject.SetActive(IsAuto);

		if (IsAuto && !string.IsNullOrEmpty(lastRewardValue))
		{
			RewardTypeSkillList.value = lastRewardValue;
			UIPopupList.current = RewardTypeSkillList;
			RewardTypeSkillListLabel.SetCurrentSelection();
		}
	}

	public void TweenFavoriteSign(bool isTween)
	{
		if (FavoriteSign && !isFavoriteSeen)
		{
			var tween = FavoriteSign.GetComponent<TweenPosition>();
			if (isTween) tween.PlayForward();
			else
			{
				tween.ResetToBeginning();
				FavoriteSign.SetActive(false);
			}
		}
	}

	public void CallUndo()
	{
		if (callCounter > 1 && NewPhonePopup.Instance.IsWeaponSkillMode)
		{
			callCounter = 0;
			CallCounterLabel.text = "0";
			callTypeList.RemoveAt(callTypeList.Count - 1);
			PlayerRandomValues.Instance.ReseedRandom();

			//StartCoroutine(ExecuteCallUndo(tempCounter));
		}
	}

	private IEnumerator ExecuteCallUndo(int counter)
	{
		for (int i = 0; i < counter; i++)
		{
			ExecuteCallCommand(CurrentPrizeType);
			yield return null;
		}
	}
	#endregion
}
