using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;
using TwdCustomMod;
using System.Collections;
using static TWDModel.EquipPrizeWheelModel;

public partial class PhoneWeaponContainer : MonoBehaviourExtended
{
	[SerializeField]
	private PhoneWeaponListPanel phoneWeaponListPanel;

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
	private GameObject DropInfoButton;

	[SerializeField]
	private GameObject goldRadioBG;

	[SerializeField]
	private GameObject radioBG;

	[SerializeField]
	private GameObject goldRadioBG1;

	[SerializeField]
	private GameObject radioBG1;

	[SerializeField]
	private GameObject goldRadioOneBtIcon;

	[SerializeField]
	private GameObject goldRadioTenBtIcon;

	[SerializeField]
	private GameObject radioOneBtIcon;

	[SerializeField]
	private GameObject radioTenBtIcon;

	[SerializeField]
	private GameObject starsContainer;

	[SerializeField]
	private UISprite[] starsSpriteArray;

	[SerializeField]
	private GameObject bgEffect;

	private EquipPrizeWheelDefinition _currentEquipPrizeWheelDefinition;

	private int equipmentRarityLevel;

	private ColorEntry rarityColorEntry;

	public void OnEnable()
	{
		if (IsLoadDataManager)
		{
			if (!IsInitialized)
			{
				IsInitialized = true;

				LuckyTimeValue = (Player.EquipPrizeWheelModel?.LuckyTime).GetValueOrDefault();
				LuckyTimeValueDefault = (Player.gameEconomyData.ConfigData?.EquipPrizeWheelLuckPoint).GetValueOrDefault();

				radioCount = Player.GetCurrency(CurrencyType.Phone).Value;

				var equipment = Player.Equipment;
				equipCount = new List<int>() { equipment.Armors.Count, equipment.MeleeWeapons.Count, equipment.RangeWeapons.Count };
				callTypeList = new List<int>();

				playerPrizeCounterLabel.text = playerPrizeCounter.ToString();

				ResetEquipCounters();

				if (Prize_Token) Prize_Token.enabled = true;
			}

			PlayerRandomValues.Instance.On_Call_Reset += OnClickReset;
			PlayerRandomValues.Instance.On_Call_Change += OnCounterChange;
			CallCraft.Instance.AddRadiosAction += ChangeRadios;
			SelectWeaponsPopupCurrent = null;
		}
		UIEvent.OnUIEvent += OnUIEvent;
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
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
		if (IsLoadDataManager)
		{
			if (type == "PhoneWeaponSelected" && parameter is EquipPrizeWheelDefinition definition)
			{
				OnClickTab(definition);
			}
		}
		else
		{
			if (type == "PhoneWeaponSelected")
			{
				if (parameter is EquipPrizeWheelDefinition definition)
				{
					OnClickTab(definition);
					NewPhonePopup newPhonePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup;
					if (newPhonePopup != null)
					{
						newPhonePopup.ShowPhone(flag: true);
					}
				}
			}
			else if (type == "PhoneWeaponChanged")
			{
				NewPhonePopup newPhonePopup2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup) as NewPhonePopup;
				if (newPhonePopup2 != null)
				{
					newPhonePopup2.ShowPhone(flag: true);
				}
			}
		}
	}

	public void Open()
	{
		List<EquipPrizeWheelDefinition> definitions = (from d in GameManager.Instance.gameEconomyData.GetOpenEquipPrizeWheelDefinition(GameManager.Instance.playerModel.UtcTimeStamp)
			where d.RadioType == RadioType.Phone
			orderby d.Order descending, d.Identifier descending
			select d).ToList();
		if (phoneWeaponListPanel != null)
		{
			phoneWeaponListPanel.Init(definitions);
			UIListCard<EquipPrizeWheelDefinition> cardAt = phoneWeaponListPanel.getCardAt(0);
			if (cardAt != null && cardAt.Item != null)
			{
				UIEvent.Send("PhoneWeaponSelected", cardAt.Item);
			}
			if (RewardTypeList) lastRewardValue = RewardTypeList.value;
		}
	}

	private void OnClickTab(EquipPrizeWheelDefinition definition)
	{
		if (definition != null)
		{
			_currentEquipPrizeWheelDefinition = definition;
			HelpersUI.SetContentToLabel(weaponLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(definition.NameLocKey));
			DateTime dateTime = GameEconomyData.ParseDateTime(definition.StartTimeUtc);
			DateTime dateTime2 = GameEconomyData.ParseDateTime(definition.EndTimeUtc);
			HelpersUI.SetContentToLabel(timeLabel, $"{dateTime:yyyy-MM-dd}~{dateTime2:yyyy-MM-dd}");
			Texture texture = UnityUtils.LoadFromAssetBundle<Texture>(definition.HighlightedReward, "itemgraphics");
			//Texture value = UnityUtils.LoadFromAssetBundle<Texture>(definition.HighlightedReward + "_alpha", "itemgraphics");
			if (!string.IsNullOrEmpty(definition.cdnIcon))
			{
				Helpers.GameObjectSetActive(cdnTexture, value: true);
				Helpers.GameObjectSetActive(rewardTexture, value: false);
				LoadImageFromCdn.LoadImageToTarget(cdnTexture, definition.cdnIcon);
			}
			else
			{
				Helpers.GameObjectSetActive(cdnTexture, value: false);
				Helpers.GameObjectSetActive(rewardTexture, value: true);
				rewardTexture.mainTexture = texture;
				//rewardTexture.material.SetTexture("_MainTex", texture);
				//rewardTexture.material.SetTexture("_AlphaTex", value);
			}
			int value2 = (GameManager.Instance.playerModel?.EquipPrizeWheelModel?.LuckyTime).GetValueOrDefault();
			int valueOrDefault = (GameManager.Instance.gameEconomyData?.ConfigData?.EquipPrizeWheelLuckPoint).GetValueOrDefault();

			if (IsLoadDataManager)
			{
				string _luckyLabel;
				if (LocalizationManager.Instance.CurrentLanguage == "ru")
				{
					_luckyLabel = "Очки удачи: " + value2 + '/' + valueOrDefault;
				}
				else
				{
					_luckyLabel = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.Lucky", value2, valueOrDefault);
				}
				HelpersUI.SetContentToLabel(luckyLabel, _luckyLabel);
			}
			else
			{
				HelpersUI.SetContentToLabel(luckyLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.Lucky", value2, valueOrDefault));
			}
			HelpersUI.SetContentToLabel(descriptionLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(definition.DescLocKey));
			HelpersUI.SetContentToLabel(oneButtonLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.OneButton"));
			HelpersUI.SetContentToLabel(oneTimeLabel, $"x{definition.OncePrice}");
			HelpersUI.SetContentToLabel(tenButtonLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.TenButton"));
			HelpersUI.SetContentToLabel(oldTenTimeLabel, $"x{definition.TenTimesOriginalPrice}");
			HelpersUI.SetContentToLabel(tenTimeLabel, $"x{definition.TenTimesPrice}");
			Helpers.GameObjectSetActive(DropInfoButton, _currentEquipPrizeWheelDefinition.HighlightedRewardTexture != null);
			//if (definition.RadioType == RadioType.GoldRadio)
			//{
			//	Helpers.GameObjectSetActive(goldRadioBG, value: true);
			//	Helpers.GameObjectSetActive(goldRadioBG1, value: true);
			//	Helpers.GameObjectSetActive(goldRadioOneBtIcon, value: true);
			//	Helpers.GameObjectSetActive(goldRadioTenBtIcon, value: true);
			//	Helpers.GameObjectSetActive(radioBG, value: false);
			//	Helpers.GameObjectSetActive(radioBG1, value: false);
			//	Helpers.GameObjectSetActive(radioOneBtIcon, value: false);
			//	Helpers.GameObjectSetActive(radioTenBtIcon, value: false);
			//}
			//else
			//{
			//	Helpers.GameObjectSetActive(radioBG, value: true);
			//	Helpers.GameObjectSetActive(radioBG1, value: true);
			//	Helpers.GameObjectSetActive(radioOneBtIcon, value: true);
			//	Helpers.GameObjectSetActive(radioTenBtIcon, value: true);
			//	Helpers.GameObjectSetActive(goldRadioBG, value: false);
			//	Helpers.GameObjectSetActive(goldRadioBG1, value: false);
			//	Helpers.GameObjectSetActive(goldRadioOneBtIcon, value: false);
			//	Helpers.GameObjectSetActive(goldRadioTenBtIcon, value: false);
			//}
			SetBgEffectColor();
			updateRarityRating(starsSpriteArray);
		}
	}

	private void updateRarityRating(UISprite[] starsArray)
	{
		starsContainer.SetActive(value: false);
	}

	private void SetBgEffectColor()
	{
		if (!(bgEffect != null))
		{
			return;
		}
		UIWidget component = bgEffect.GetComponent<UIWidget>();
		if (component != null)
		{
			if (_currentEquipPrizeWheelDefinition != null && _currentEquipPrizeWheelDefinition.RadioType == RadioType.GoldRadio)
			{
				component.color = new Color(1f, 1f, 1f, 0.716f);
			}
			else
			{
				component.color = new Color(1f, 1f, 1f, 0.43f);
			}
		}
	}

	public void SetBgEffectColor(Color color)
	{
		if (bgEffect != null)
		{
			UIWidget component = bgEffect.GetComponent<UIWidget>();
			if (component != null)
			{
				component.color = color;
			}
		}
	}

	public void OnClickOneButton()
	{
		EquipPrizeType = EquipPrizeType.One;

		if (IsLoadDataManager && IsAuto && !CoroutineStarted)
		{
			StartCoroutine(ExecuteCallBatch(EquipPrizeType.One));
		}
		else
		{
			ExecuteCallCommand(EquipPrizeType.One);
		}
	}

	public void OnClickTenButton()
	{
		EquipPrizeType = EquipPrizeType.Ten;

		if (IsLoadDataManager && IsAuto && !CoroutineStarted)
		{
			StartCoroutine(ExecuteCallBatch(EquipPrizeType.Ten));
		}
		else
		{
			ExecuteCallCommand(EquipPrizeType.Ten);
		}
	}

	public void ExecuteCallCommand(EquipPrizeType prizeType)
	{
		CurrentPrizeType = prizeType;
		if (_currentEquipPrizeWheelDefinition == null)
		{
			UnityEngine.Debug.LogError("WeaponCardDraw err：_currentEquipPrizeWheelDefinition NULL");
			return;
		}
		if (IsLoadDataManager)
		{
			IsNoCountOffset = true;
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
				cashier = Cashier.CreateOneItemCashier(manager, PurchaseType.EquipPrize, CurrencyType.Phone, (prizeType == EquipPrizeType.Ten) ? equipPrizeWheelDefinition.TenTimesPrice : equipPrizeWheelDefinition.OncePrice);
				cashier.UseDiamondsAmount = -2;
				tWDModelResult = cashier.Pay();
				if (tWDModelResult != 0)
				{
					return;
				}
			}

			callCounter += 1;
			CallCounterLabel.text = callCounter.ToString();

			playerPrizeCounter += prizeType == EquipPrizeType.One ? 1 : 10;
			tenTimesPrice = equipPrizeWheelDefinition.TenTimesPrice;
			playerRadioCounter += prizeType == EquipPrizeType.One ? 10 : tenTimesPrice;
			callTypeList.Add(prizeType == EquipPrizeType.One ? 1 : 10);
			playerPrizeCounterLabel.text = playerPrizeCounter.ToString() + " / " + playerRadioCounter.ToString();

			equipPrizeWheelModel.CurrentEquipPrizeType = prizeType;
			equipPrizeWheelModel.CurrentEquipPrizeWheelDefinition = equipPrizeWheelDefinition;
			equipPrizeWheelModel.AddReward(prizeType, equipPrizeWheelDefinition.SlotNumber);

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
					SelectWeaponsPopupCurrent = selectWeaponsPopup;

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

	public override void Clear()
	{
		base.Clear();
		if (phoneWeaponListPanel != null)
		{
			phoneWeaponListPanel.ClearCards();
		}
	}

	public void OnShowweaponButton()
	{
		EquipmentUpgradePopup equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		equipmentUpgradePopup.ShowNextLevel = false;
		equipmentUpgradePopup.OpenForPreview(_currentEquipPrizeWheelDefinition.HighlightedRewardTexture, 5);
	}



	#region myparams
	[SerializeField]
	private UILabel playerPrizeCounterLabel;
	//затраченные рации
	public int playerRadioCounter { get; private set; }
	public EquipPrizeType CurrentPrizeType = EquipPrizeType.One;
	//призы (карточки)
	public int playerPrizeCounter { get; private set; }
	public UILabel CallCounterLabel;
	private int callCounter;

	bool IsInitialized = false;
	private int radioCount;

	private int LuckyTimeValue;
	private int LuckyTimeValueDefault;

	public List<int> equipCount { get; private set; }
	//Вызов за 10 - 1, за 90 - 10
	public List<int> callTypeList { get; private set; }

	public List<PrizeCounter> AllCounters;

	//Части чертежа
	//public UILabel tokenPartsCounterLabel;
	//public int equiPlusCounter { get; set; }
	//public int equiPlusCounterNearest { get; set; }
	//public List<int> equiPlusCounterNearestList { get; set; }

	//Чертежи
	//public UILabel tokenCounterLabel;
	//public int tokenCounter { get; set; }
	//public int tokenCounterNearest { get; set; }
	//public List<int> tokenCounterNearestList { get; set; }

	//ремодел детали
	//public UILabel partCounterLabel;
	//public int partCounter { get; set; }
	//public int partCounterNearest { get; set; }
	//public List<int> partCounterNearestList { get; set; }

	//апо детали
	//public UILabel apoCounterLabel;
	//public int apoCounter { get; set; }
	//public int apoCounterNearest { get; set; }
	//public List<int> apoCounterNearestList { get; set; }

	//Оружие
	//public UILabel weaponCounterLabel;
	//public int weaponCounter { get; set; }
	//public int weaponCounterNearest { get; set; }
	//public List<int> weaponCounterNearestList { get; set; }

	//Броня
	//public UILabel armorCounterLabel;
	//public int armorCounter { get; set; }
	//public int armorCounterNearest { get; set; }
	//public List<int> armorCounterNearestList { get; set; }

	//апо
	//public UILabel remoldCounterLabel;
	//public int remoldCounter { get; set; }
	//public int remoldCounterNearest { get; set; }
	//public List<int> remoldCounterNearestList { get; set; }

	public SelectWeaponsPopup SelectWeaponsPopupCurrent { get; private set; }
	public GameObject SelectWeaponsPopupPrefab;

	public EquipPrizeType EquipPrizeType { get; private set; }
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private PlayerModel Player => GameManager.Instance.playerModel;

	public UIInput MaxCallsInput;
	private int MaxCalls;
	private int tenTimesPrice = 80;
	public UIPopupList RewardTypeList;
	public UILabel RewardTypeListLabel;
	public PrizeCounterType CurrentRewardType { get; private set; } = PrizeCounterType.Any;

	public bool IsQuick { get; set; }
	public bool isFree { get; set; }
	public bool IsAuto { get; set; }
	private bool CoroutineStarted;
	public bool IsFineConstructFinded { get; set; }
	public int batchCount { get; set; }
	public bool IsCallDone { get; set; }
	public bool IsNoCountOffset { get; set; }

	[SerializeField]
	private UITexture Prize_Token;

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
				MaxCalls = Mathf.FloorToInt(result / (prizeType == EquipPrizeType.One ? 10 : tenTimesPrice));
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

	private void ResetEquipCounters()
	{
		IsFineConstructFinded = false;
		NewPhonePopup.Instance.GoldRadioWeaponContainer.IsCallDone = false;
		NewPhonePopup.Instance.GoldRadioWeaponContainer.IsFineConstructFinded = false;

		IsCallDone = false;

		foreach (var counter in AllCounters)
		{
			counter.CounterValue = 0;
			counter.CounterNearestValue = 0;
			counter.CounterLabel.text = "0";
			counter.CounterNearestList = new List<int>();
		}
	}

	private void Start()
	{
		if (!IsLoadDataManager) return;

		foreach (var counter in AllCounters)
		{
			counter.GetComponent<UIButtonExtended>().SetClickCallback(delegate
			{
				TooltipManager.OpenTextBoxWithText(counter.CounterLabel.gameObject, string.Join(", ", counter.CounterNearestList));
			});
		}
	}

	public void OnCounterChange(int value)
	{
		RerollPrizeCalls();
	}

	private void RerollPrizeCalls()
	{
		if (callTypeList.Count > 0 && !NewPhonePopup.Instance.IsWeaponSkillMode)
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

	private void ChangeRadios()
	{
		radioCount = DataManager.Instance.Player.GetCurrency(CurrencyType.Phone).Value;
	}

	public void SetFree(UIToggle tg)
	{
		isFree = tg.value;
		NewPhonePopup.Instance.GoldRadioWeaponContainer.isFree = isFree;
	}

	private void SetWeaponPopupNull(HUDElement element, HUDElementConfig hudElementConfig)
	{
		SelectWeaponsPopupCurrent = null;
	}

	public void Reset()
	{
		PlayerRandomValues.Instance.ResetAll(true);
	}

	public void OnClickReset(bool isZeroCounter)
	{
		if (SelectWeaponsPopupCurrent != null)
		{
			SelectWeaponsPopupCurrent.OnClickClose();
		}

		Player.EquipPrizeWheelModel.LuckyTime = LuckyTimeValue;
		Player.gameEconomyData.ConfigData.EquipPrizeWheelLuckPoint = LuckyTimeValueDefault;
		string locLucky;
		if (LocalizationManager.Instance.CurrentLanguage == "ru")
		{
			locLucky = "Очки удачи: " + LuckyTimeValue + '/' + LuckyTimeValueDefault;
		}
		else
		{
			locLucky = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.Lucky", LuckyTimeValue, LuckyTimeValueDefault);
		}
		HelpersUI.SetContentToLabel(luckyLabel, locLucky);

		var currencyAmount = Player.GetCurrencyAmount(CurrencyType.Phone);
		var delta = radioCount - currencyAmount;
		Player.SetCurrency(CurrencyType.Phone, delta);
		var currencyAmountNew = Player.GetCurrencyAmount(CurrencyType.Phone);
		NewPhonePopup.Instance.RadiophonesAmountLabel.text = currencyAmountNew.ToString();

		callCounter = 0;
		CallCounterLabel.text = "0";
		playerPrizeCounter = 0;
		playerRadioCounter = 0;
		playerPrizeCounterLabel.text = playerPrizeCounter.ToString() + " / " + playerRadioCounter.ToString();

		ResetEquipCounters();

		if (isZeroCounter)
		{
			callTypeList.Clear();
		}
		DebugTWD.Log("Current radio amount is " + currencyAmountNew);
	}

	public void ShowHelp()
	{
		MyTools.OpenAlert("Для изменения рандома жмите на счетчик рандома сверху. Ниже в списке наград в скобках " +
			"- ближайшее положение в очереди вызовов");
	}

	public void SetAutoCall(UIToggle tg)
	{
		if (NewPhonePopup.Instance.IsWeaponSkillMode) return;
		IsAuto = tg.value;
		NewPhonePopup.Instance.GoldRadioWeaponContainer.IsAuto = IsAuto;
		SetRewardTypeListValue();
		if (Player.EquipPrizeWheelModel != null && !IsAuto) Player.EquipPrizeWheelModel.SetIsAuto(false);

		Helpers.GameObjectSetActive(RewardTypeList.gameObject, IsAuto);
		Helpers.GameObjectSetActive(MaxCallsInput.transform.parent.gameObject, IsAuto);
	}

	public void SetRewardType(UIPopupList uIPopupList)
	{
		int index = uIPopupList.items.IndexOf(uIPopupList.value);
		CurrentRewardType = (PrizeCounterType)index;
		lastRewardValue = uIPopupList.value;
	}

	private string lastRewardValue;

	public void SetIsFineConstructFinded(PrizeCounterType type)
	{
		if (IsAuto && !IsFineConstructFinded)
		{
			IsFineConstructFinded = CurrentRewardType == type || CurrentRewardType == PrizeCounterType.Any;
		}
	}

	public void SetRewardTypeListValue()
	{
		RewardTypeList.gameObject.SetActive(IsAuto);
		MaxCallsInput.transform.parent.gameObject.SetActive(IsAuto);

		if (IsAuto && !string.IsNullOrEmpty(lastRewardValue))
		{
			RewardTypeList.value = lastRewardValue;
			UIPopupList.current = RewardTypeList;
			RewardTypeListLabel.SetCurrentSelection();
		}
	}

	public void CallUndo()
	{
		if (callCounter > 1 && !NewPhonePopup.Instance.IsWeaponSkillMode)
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
