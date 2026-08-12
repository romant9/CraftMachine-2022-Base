using System.Collections;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SelectWeaponsPopup : HUDElement
{
	[SerializeField]
	public Transform ButtonParentTarget;

	[SerializeField]
	private GameObject WeaponCardPrefab;

	[SerializeField]
	private GameObject CardEffectPrefab;

	[SerializeField]
	public GameObject oneButton;

	[SerializeField]
	private GameObject tenButton;

	[SerializeField]
	private GameObject skipButton;

	[SerializeField]
	private GameObject closeButton;

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
	private float AnimationsDelay;

	[SerializeField]
	private float InputWaitTime = 6f;

	[SerializeField]
	private float SizeOffset = 25f;

	private float ClickWaitTime;

	private bool AutoClickDone;

	private int CardIntroCompletedCount;

	[SerializeField]
	private GameObject radioOneIcon;

	[SerializeField]
	private GameObject radioTenIcon;

	[SerializeField]
	private GameObject goldRadioOneIcon;

	[SerializeField]
	private GameObject goleRadioTenIcon;

	private List<RadioCallCardBase> CardsList = new List<RadioCallCardBase>();

	public override void Open()
	{
		base.Open();
		StartDrawCard();
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

	private void StartDrawCard()
	{
		CardIntroCompletedCount = 0;

		if (IsLoadDataManager)
		{
			NewPhonePopup.Instance.SetWeaponPopup(this);
		}
		else
		{
			AutoClickDone = false;
			Helpers.GameObjectSetActive(skipButton, value: true);
		}
		Helpers.GameObjectSetActive(oneButton, value: false);
		Helpers.GameObjectSetActive(tenButton, value: false);
		Helpers.GameObjectSetActive(closeButton, value: IsLoadDataManager);
		EquipPrizeWheelDefinition equipPrizeWheelDefinition = GameManager.Instance.playerModel?.EquipPrizeWheelModel?.CurrentEquipPrizeWheelDefinition;
		if (equipPrizeWheelDefinition != null)
		{
			HelpersUI.SetContentToLabel(oneButtonLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.OneButton"));
			HelpersUI.SetContentToLabel(oneTimeLabel, $"x{equipPrizeWheelDefinition.OncePrice}");
			HelpersUI.SetContentToLabel(tenButtonLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("NewPhonePopup.PhoneWeapon.TenButton"));
			HelpersUI.SetContentToLabel(oldTenTimeLabel, $"x{equipPrizeWheelDefinition.TenTimesOriginalPrice}");
			HelpersUI.SetContentToLabel(tenTimeLabel, $"x{equipPrizeWheelDefinition.TenTimesPrice}");
			UIEvent.Send("PhoneWeaponDrawCardDone");
			if (equipPrizeWheelDefinition.RadioType == RadioType.Phone)
			{
				UIEvent.Send("PhoneWeaponSelected", equipPrizeWheelDefinition);
			}
			else
			{
				UIEvent.Send("PhoneGoldRadioSelected", equipPrizeWheelDefinition);
			}
			if (equipPrizeWheelDefinition.RadioType == RadioType.GoldRadio)
			{
				Helpers.GameObjectSetActive(radioOneIcon, value: false);
				Helpers.GameObjectSetActive(radioTenIcon, value: false);
				Helpers.GameObjectSetActive(goldRadioOneIcon, value: true);
				Helpers.GameObjectSetActive(goleRadioTenIcon, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(radioOneIcon, value: true);
				Helpers.GameObjectSetActive(radioTenIcon, value: true);
				Helpers.GameObjectSetActive(goldRadioOneIcon, value: false);
				Helpers.GameObjectSetActive(goleRadioTenIcon, value: false);
			}
		}
		Clear();
		List<EquipPrizeWheelReward> list = GameManager.Instance.playerModel?.EquipPrizeWheelModel?.Rewards;
		if (list == null)
		{
			DebugLogError("SelectWeaponsPopup wheelRewards NULL");
			return;
		}
		InstantiateButtons(list);
		PositionCards();

		if (CardsList != null && CardsList.Count > 0)
		{
			for (int i = 0; i < CardsList.Count; i++)
			{
				if (CardsList[i] != null)
				{
					if (IsLoadDataManager)
					{
						CardsList[i].ShowRewardCard();
					}
					else
					{
						CardsList[i].AnimateIntro();
					}
				}
			}
		}
		ClickWaitTime = Time.time + InputWaitTime;
	}

	private void InstantiateButtons(List<EquipPrizeWheelReward> wheelRewards)
	{
		RadioCallCardBase radioCallCardBase = null;
		for (int i = 0; i < wheelRewards.Count; i++)
		{
			EquipPrizeWheelReward equipPrizeWheelReward = wheelRewards[i];
			if (equipPrizeWheelReward == null)
			{
				continue;
			}
			int.TryParse(equipPrizeWheelReward.Rarity, out var result);
			if (!(ButtonParentTarget == null) && !(WeaponCardPrefab == null))
			{
				radioCallCardBase = Helpers.AddComponent<RadioCallWeaponCard>(Helpers.InstantiateToParentAndLayer(WeaponCardPrefab, ButtonParentTarget.gameObject));
				if (radioCallCardBase != null)
				{
					if (!OfflineManager.IsNoEffects) radioCallCardBase.InitEffects(CardEffectPrefab, result);
					radioCallCardBase.InitWeaponCard(equipPrizeWheelReward.RewardEntries?.RewardsList[0]);
					if (!IsLoadDataManager) radioCallCardBase.SetIntroCompleteCallaback(IntroCompleteCallaback);
					radioCallCardBase.UpdateUI();
					CardsList.Add(radioCallCardBase);
				}
			}
		}
		radioCallCardBase = null;
	}

	private void PositionCards()
	{
		if (CardsList != null && CardsList.Count > 0)
		{
			int count = CardsList.Count;
			if (count > 5)
			{
				int num = count / 2;
				for (int i = 0; i < num; i++)
				{
					RadioCallCardBase radioCallCardBase = CardsList[i];
					if (radioCallCardBase != null)
					{
						radioCallCardBase.SetInitPosition(HelpersUI.GetRowPositionX(i, num, new Vector2(radioCallCardBase.localSize.x + SizeOffset, radioCallCardBase.localSize.y), new Vector3(0f, 120f)));
						GameObject obj = radioCallCardBase.gameObject;
						obj.name = obj.name + "_Count_" + (i + 1) + "_Up";
					}
					radioCallCardBase = null;
				}
				for (int j = num; j < count; j++)
				{
					RadioCallCardBase radioCallCardBase2 = CardsList[j];
					if (radioCallCardBase2 != null)
					{
						radioCallCardBase2.SetInitPosition(HelpersUI.GetRowPositionX(j - num, num, new Vector2(radioCallCardBase2.localSize.x + SizeOffset, radioCallCardBase2.localSize.y), new Vector3(0f, -50f)));
						GameObject obj2 = radioCallCardBase2.gameObject;
						obj2.name = obj2.name + "_Count_" + (j + 1) + "_Down";
					}
					radioCallCardBase2 = null;
				}
				return;
			}
			for (int k = 0; k < CardsList.Count; k++)
			{
				RadioCallCardBase radioCallCardBase3 = CardsList[k];
				if (radioCallCardBase3 != null)
				{
					radioCallCardBase3.SetInitPosition(HelpersUI.GetRowPositionX(k, CardsList.Count, new Vector2(radioCallCardBase3.localSize.x + SizeOffset, radioCallCardBase3.localSize.y)));
					GameObject obj3 = radioCallCardBase3.gameObject;
					obj3.name = obj3.name + "_Count_" + (k + 1);
				}
				radioCallCardBase3 = null;
			}
		}
		else
		{
			DebugLogError("CardsList is NULL or Empty!");
		}
	}

	private void IntroCompleteCallaback(RadioCallCardBase card)
	{
		CardIntroCompletedCount++;
		if (CardsList != null && CardsList.Count > 0 && CardIntroCompletedCount >= CardsList.Count)
		{
			for (int i = 0; i < CardsList.Count; i++)
			{
				CardsList[i].UpdateUI();
			}
			EventManager.NotifyClick("SearchOver");
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("camp/phonecall");
			}
			if (!IsLoadDataManager)
			{
				switch (GameManager.Instance.playerModel.EquipPrizeWheelModel.CurrentEquipPrizeType)
				{
					case EquipPrizeType.One:
						Helpers.GameObjectSetActive(oneButton, value: true);
						break;
					case EquipPrizeType.Ten:
						Helpers.GameObjectSetActive(tenButton, value: true);
						break;
				}
				Helpers.GameObjectSetActive(skipButton, value: false);
			}
			Helpers.GameObjectSetActive(closeButton, value: true);
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

	public override void Close()
	{
		if (!IsLoadDataManager) RestoreCampBeforeCloseAnim();
		Clear();
		base.Close();
	}

	public override void OnClickClose()
	{
		if (IsLoadDataManager)
		{
			Clear();
			NewPhonePopup.Instance.SetWeaponPopup(null);
		}
		base.OnClickClose();
		if (SingularityMonoBehaviour<AudioManager>.Instance != null)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.OnRadioCallDone();
		}
	}

	private void RestoreCampBeforeCloseAnim()
	{
		if (CampManager.Instance != null)
		{
			CampManager.Instance.FullscreenPopupShowCamp(SingularityMonoBehaviour<HUDManager>.Instance.CanEnableCamp(UIType.RadioSelectSurvivorPopup));
		}
	}

	public void Clear()
	{
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
		CardsList.Clear();
		if (IsLoadDataManager)
		{
			DestroyChildren();
		}
	}

	public void OnClickOneButton()
	{
		ExecuteCallCommand();
	}

	public void OnClickTenButton()
	{
		ExecuteCallCommand();
	}

	private void ExecuteCallCommand()
	{
		EquipPrizeWheelModel equipPrizeWheelModel = GameManager.Instance.playerModel?.EquipPrizeWheelModel;
		if (equipPrizeWheelModel == null)
		{
			DebugLogError("WeaponCardDraw err：wheelModel NULL");
			return;
		}
		EquipPrizeWheelDefinition currentEquipPrizeWheelDefinition = equipPrizeWheelModel.CurrentEquipPrizeWheelDefinition;
		EquipPrizeType currentEquipPrizeType = equipPrizeWheelModel.CurrentEquipPrizeType;
		if (currentEquipPrizeWheelDefinition == null)
		{
			UnityEngine.Debug.LogError("WeaponCardDraw err：definition NULL");
			return;
		}
		if (currentEquipPrizeWheelDefinition.RadioType == RadioType.GoldRadio)
		{
			int num = currentEquipPrizeWheelDefinition.OncePrice;
			if (currentEquipPrizeType == EquipPrizeType.Ten)
			{
				num = currentEquipPrizeWheelDefinition.TenTimesPrice;
			}
			int value = GameManager.Instance.playerModel.GetCurrency(CurrencyType.GoldRadio).Value;
			if (num > value)
			{
				ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(num, CurrencyType.GoldRadio);
				return;
			}
		}
		if (IsLoadDataManager)
		{
			EquipPrizeWheelCommandExecute(equipPrizeWheelModel);
		}
		else
		{
			ConsumeCurrencyCommandUtils.Execute(new EquipPrizeWheelCommand(currentEquipPrizeType, currentEquipPrizeWheelDefinition.Identifier)
			{
				Cashier = EquipPrizeWheelCommand.GetCashier(GameManager.Instance.modelManager, currentEquipPrizeWheelDefinition.Identifier, currentEquipPrizeType)
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
			DestroyChildren();
			StartDrawCard();
		}
	}

	private void DestroyChildren()
	{
		if (ButtonParentTarget != null && ButtonParentTarget.gameObject != null)
		{
			Helpers.DestroyAllChildren(ButtonParentTarget.gameObject);
		}
	}


	public void OnClickSkipButton()
	{
		Helpers.GameObjectSetActive(skipButton, value: false);
		StartCoroutine(AutoClickEffect(AnimationsDelay));
	}



	#region myparams
	public int CardsListCount => CardsList.Count;
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private TWDModelManager modelManager => DataManager.Instance.ModelManager;
	#endregion

	#region mycode
	public void OnClickHide(UIButtonToggle bt)
	{
		bool isHide = ButtonParentTarget.GetComponent<UIWidget>().alpha == 0;
		ButtonParentTarget.GetComponent<UIWidget>().alpha = isHide ? 1 : 0;
		bt.transform.GetChild(0).GetComponent<UISprite>().color = isHide ? Color.gray : Color.white;
	}

	private void EquipPrizeWheelCommandExecute(EquipPrizeWheelModel equipPrizeWheelModel)
	{
		EquipPrizeType currentEquipPrizeType = equipPrizeWheelModel.CurrentEquipPrizeType;
		EquipPrizeWheelDefinition equipPrizeWheelDefinition = modelManager.GameEconomyData.GetEquipPrizeWheelDefinition(equipPrizeWheelModel.CurrentEquipPrizeWheelDefinition.Identifier);
		if (equipPrizeWheelDefinition == null || !equipPrizeWheelDefinition.IsOpen(modelManager.Player.UtcTimeStamp))
		{
			return;
		}
		Cashier cashier = Cashier.CreateOneItemCashier(modelManager, PurchaseType.EquipPrize, CurrencyType.Phone, (currentEquipPrizeType == EquipPrizeType.Ten) ? equipPrizeWheelDefinition.TenTimesPrice : equipPrizeWheelDefinition.OncePrice);
		cashier.UseDiamondsAmount = -2;
		TWDModelResult tWDModelResult;

		if (OfflineManager.IsFreeAll)
		{
			tWDModelResult = TWDModelResult.OK;
		}
		else
		{
			tWDModelResult = cashier.Pay();
		}
		if (tWDModelResult != 0)
		{
			return;
		}
		equipPrizeWheelModel.AddReward(currentEquipPrizeType, equipPrizeWheelDefinition.SlotNumber);
		equipPrizeWheelModel.CurrentEquipPrizeType = currentEquipPrizeType;
		equipPrizeWheelModel.CurrentEquipPrizeWheelDefinition = equipPrizeWheelDefinition;

		Metrics metrics = modelManager.Metrics;
		metrics.ResourceChangeUsedReason = "EquipmentCall";
		metrics.AddItemChange().AddResources(cashier).Send();
		modelManager.TdMetrics.SetEventType("equipment_call").AddProperty("equip_prize_phone_used", (currentEquipPrizeType == EquipPrizeType.Ten) ? equipPrizeWheelDefinition.TenTimesPrice : equipPrizeWheelDefinition.OncePrice).AddProperty("equip_prize_choose_type", currentEquipPrizeType)
			.AddProperty("equip_prize_call_slot", equipPrizeWheelDefinition.SlotNumber)
			.AddProperty("equip_prize_acceptance", equipPrizeWheelDefinition.SlotNumber)
			.Send();

		OnWeaponCall(tWDModelResult);
	}
	#endregion
}
