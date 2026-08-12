using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using TWDModel.SqEquipmentRemold;
using UnityEngine;

public class SPRemoldMainPopup : HUDElement
{
	[SerializeField]
	private HUDMeter SPTraitsUpgradeMeter;

	[SerializeField]
	private HUDMeter SPTraitsRemoldMeter;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private UILabel weaponName;

	[SerializeField]
	private EquipmentButton equipmentButton;

	[SerializeField]
	private UILabel weaponRate;

	[SerializeField]
	private UILabel range;

	[SerializeField]
	private UILabel chargeRange;

	[SerializeField]
	private UILabel area;

	[SerializeField]
	private UILabel chargeArea;

	[SerializeField]
	private UILabel targetNum;

	[SerializeField]
	private UILabel chargeTargetNum;

	[SerializeField]
	private UILabel upgradeTokenNumLabel;

	[SerializeField]
	private UILabel remoldTokenNumLabel;

	[SerializeField]
	private GameObject cancelButton;

	[SerializeField]
	private GameObject rollButton;

	[SerializeField]
	private GameObject confirmButton;

	[SerializeField]
	private UIButton updateButton;

	[SerializeField]
	private GameObject LvMaxButton;

	[SerializeField]
	private GameObject updateInfobtn;

	private bool curIsMax;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private EquipmentItemModel equipmentItemModel;

	private float lastClickTime;

	private const float clickInterval = 1.75f;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
		if (!Helpers.IsSPRemoldNotFirstOpen())
		{
			OnclickTips2();
			Helpers.SetSPRemoldNotFirstOpen(on: true);
		}
	}

	public override void Close()
	{
		base.Close();
		if (equipmentItemModel != null && equipmentItemModel.SpEquipmentRemoldModel != null)
		{
			equipmentItemModel.SpEquipmentRemoldModel.Changed -= OnEquipmentItemModelChanged;
		}
		curIsMax = false;
		StopAllCoroutines();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SPRemoldLockChanged" || type == "miniShopCLoseEvent")
		{
			UpdateUI();
		}
	}

	public void BindData(EquipmentItemModel equipmentItemModel)
	{
		this.equipmentItemModel = equipmentItemModel;
		equipmentItemModel.SpEquipmentRemoldModel.Changed += OnEquipmentItemModelChanged;
	}

	private void OnEquipmentItemModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "SpEquipmentRemoldTraitsUpgrade")
		{
			object[] obj = (object[])args;
			string text = (string)obj[0];
			string text2 = (string)obj[1];
			UIEvent.Send("SPRemoldUpgradeChanged", new object[2] { text, text2 });
			StartCoroutine(UpgradedUIShow(text, text2));
		}
	}

	private IEnumerator UpgradedUIShow(string oldTraitID, string newTraitID)
	{
		UpdateUI();
		yield return new WaitForSeconds(0.1f);
		UIEvent.Send("SPRemoldUpgradeChangedEffect", new object[2] { oldTraitID, newTraitID });
		yield return new WaitForSeconds(1.3f);
		SPRemoldTraitsUpdatedPopup sPRemoldTraitsUpdatedPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsUpdatedPopup) as SPRemoldTraitsUpdatedPopup;
		if (sPRemoldTraitsUpdatedPopup != null)
		{
			sPRemoldTraitsUpdatedPopup.InitData(oldTraitID, newTraitID);
			sPRemoldTraitsUpdatedPopup.Open();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		SPTraitsUpgradeMeter.SetCurrencyType(CurrencyType.SPTraitsUpgradeToken);
		SPTraitsUpgradeMeter.SetValue(playerModel.GetCurrencyAmount(CurrencyType.SPTraitsUpgradeToken));
		SPTraitsRemoldMeter.SetCurrencyType(CurrencyType.SPTraitsRemoldToken);
		SPTraitsRemoldMeter.SetValue(playerModel.GetCurrencyAmount(CurrencyType.SPTraitsRemoldToken));
		int value = GameManager.Instance.playerModel.gameEconomyData.SPTraitsRemoldConfigs.GetUpgradeCost().FirstOrDefault().Value;
		upgradeTokenNumLabel.text = value.ToString();
		upgradeTokenNumLabel.color = Color.white;
		if (value > playerModel.GetCurrencyAmount(CurrencyType.SPTraitsUpgradeToken))
		{
			upgradeTokenNumLabel.color = Color.red;
		}
		int lockedNoForceTraitCount = equipmentItemModel.SpEquipmentRemoldModel.GetLockedNoForceTraitCount();
		int value2 = equipmentItemModel.SpEquipmentRemoldModel.GetRemoldBaseCost().FirstOrDefault().Value;
		int value3 = equipmentItemModel.SpEquipmentRemoldModel.CalculateRemoldLockedCost(lockedNoForceTraitCount).FirstOrDefault().Value;
		if (value3 > 0)
		{
			remoldTokenNumLabel.text = value2 + "+" + value3;
		}
		else
		{
			remoldTokenNumLabel.text = value2.ToString();
		}
		remoldTokenNumLabel.color = Color.white;
		if (value2 + value3 > playerModel.GetCurrencyAmount(CurrencyType.SPTraitsRemoldToken))
		{
			remoldTokenNumLabel.color = Color.red;
		}
		weaponName.text = HelpersLocalization.GetEquipmentName(equipmentItemModel);
		equipmentButton.Setup(equipmentItemModel, null, null, "OnNewEquipmentCardSelected", showOwnerAndUpgradeIndicator: false);
		weaponRate.text = equipmentItemModel.SpEquipmentRemoldModel.GetRateStr();
		if (curIsMax)
		{
			weaponRate.text = Helpers.GetRateStrForPreviewMax(equipmentItemModel.Definition.ID);
		}
		AbilityDefinition abilityDefinition = GameManager.Instance.gameEconomyData.GetAbilityDefinition(equipmentItemModel.Definition.AbilityIdentifier);
		AbilityDefinition abilityDefinition2 = GameManager.Instance.gameEconomyData.GetAbilityDefinition(equipmentItemModel.ChargeEquipment.Definition.AbilityIdentifier);
		range.text = LocalizationManager.GetText("BasicInfo.Ability.Base.Desc", abilityDefinition.AbilityRange);
		area.text = HelpersLocalization.GetWeaponAreaDescNoArea(abilityDefinition);
		targetNum.text = abilityDefinition.MaxAffectedTargetsCount.ToString();
		Helpers.GameObjectSetActive(chargeRange, value: false);
		Helpers.GameObjectSetActive(chargeArea, value: false);
		Helpers.GameObjectSetActive(chargeTargetNum, value: false);
		if (abilityDefinition2 != null)
		{
			Helpers.GameObjectSetActive(chargeRange, value: true);
			Helpers.GameObjectSetActive(chargeArea, value: true);
			Helpers.GameObjectSetActive(chargeTargetNum, value: true);
			chargeRange.text = LocalizationManager.GetText("BasicInfo.Ability.Base.Desc", abilityDefinition2.AbilityRange);
			chargeArea.text = HelpersLocalization.GetWeaponAreaDescNoArea(abilityDefinition2);
			chargeTargetNum.text = abilityDefinition2.MaxAffectedTargetsCount.ToString();
		}
		ClearBTLevelEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		FreshListData();
		component.Reposition();
		Helpers.GameObjectSetActive(cancelButton, value: false);
		Helpers.GameObjectSetActive(confirmButton, value: false);
		if (equipmentItemModel.SpEquipmentRemoldModel.HasPendingRemold)
		{
			Helpers.GameObjectSetActive(cancelButton, value: true);
			Helpers.GameObjectSetActive(confirmButton, value: true);
		}
		Helpers.GameObjectSetActive(updateButton, value: false);
		Helpers.GameObjectSetActive(LvMaxButton, value: false);
		Helpers.GameObjectSetActive(updateInfobtn, value: false);
		if (equipmentItemModel.SpEquipmentRemoldModel.HasAnyUpgradeableTrait())
		{
			Helpers.GameObjectSetActive(updateButton, value: true);
			updateButton.normalSprite = "UI_Button_WhitePlusRed_Middle";
			if (equipmentItemModel.SpEquipmentRemoldModel.HasPendingRemold)
			{
				updateButton.normalSprite = "UI_Button_WhitePlusGrey_Middle";
			}
			if (upgradeTokenNumLabel.color != Color.red)
			{
				Helpers.GameObjectSetActive(updateInfobtn, value: true);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(LvMaxButton, value: true);
		}
	}

	private void ClearBTLevelEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	private void FreshListData()
	{
		if (equipmentItemModel == null || equipmentItemModel.SpEquipmentRemoldModel == null)
		{
			return;
		}
		List<SPTraitSlot> list = equipmentItemModel.SpEquipmentRemoldModel.SPTraitSlots;
		if (equipmentItemModel.SpEquipmentRemoldModel.HasPendingRemold)
		{
			list = equipmentItemModel.SpEquipmentRemoldModel.PendingSPTraitSlots;
		}
		if (list == null || list.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == null || string.IsNullOrEmpty(list[i].ID))
			{
				continue;
			}
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			if (gameObject.TryGetComponent<SPRemoldMainListItem>(out var component))
			{
				component.Setup(equipmentItemModel, list[i]);
				if (curIsMax)
				{
					component.SetupMaxLevelPreview(curIsMax);
				}
				Entries.Add(gameObject);
			}
		}
	}

	public void OnclickTips1()
	{
		SPRemoldTips1Popup sPRemoldTips1Popup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTips1Popup) as SPRemoldTips1Popup;
		if (sPRemoldTips1Popup != null)
		{
			sPRemoldTips1Popup.Open();
		}
	}

	public void OnclickTips2()
	{
		SPRemoldTips2Popup sPRemoldTips2Popup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTips2Popup) as SPRemoldTips2Popup;
		if (sPRemoldTips2Popup != null)
		{
			sPRemoldTips2Popup.Open();
		}
	}

	public void OnClickNotice()
	{
		SPRemoldNotice1Popup sPRemoldNotice1Popup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldNotice1Popup) as SPRemoldNotice1Popup;
		if (sPRemoldNotice1Popup != null)
		{
			sPRemoldNotice1Popup.Open();
		}
	}

	public void OnClickConfirm()
	{
		if (Helpers.ExecuteCommand(new SpEquipmentRemoldTraitsConfirm(equipmentItemModel.ModelId)) == TWDModelResult.OK)
		{
			UpdateUI();
		}
	}

	public void OnClickCancel()
	{
		SPRemoldCommonConfirmPopup sPRemoldCommonConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldCommonConfirmPopup) as SPRemoldCommonConfirmPopup;
		if (sPRemoldCommonConfirmPopup == null)
		{
			return;
		}
		string text = LocalizationManager.GetText("System.EquipSPRemold.FuncInfo31");
		string text2 = LocalizationManager.GetText("System.EquipSPRemold.FuncInfo32");
		string text3 = LocalizationManager.GetText("System.EquipSPRemold.Button.RerollCancel.Yes");
		string text4 = LocalizationManager.GetText("System.EquipSPRemold.Button.RerollCancel.Donot");
		sPRemoldCommonConfirmPopup.SetContent(text, text2, text3, text4);
		sPRemoldCommonConfirmPopup.SetOKcallBack(delegate
		{
			if (Helpers.ExecuteCommand(new SqEquipmentRemoldTratsCancelCommand(equipmentItemModel.ModelId)) == TWDModelResult.OK)
			{
				UpdateUI();
			}
		});
		sPRemoldCommonConfirmPopup.Open();
	}

	public void OnClickRoll()
	{
		if (Time.time - lastClickTime < 1.75f)
		{
			return;
		}
		lastClickTime = Time.time;
		if (curIsMax)
		{
			curIsMax = !curIsMax;
			UpdateUI();
			return;
		}
		bool flag = false;
		int lockedNoForceTraitCount = equipmentItemModel.SpEquipmentRemoldModel.GetLockedNoForceTraitCount();
		int value = equipmentItemModel.SpEquipmentRemoldModel.GetRemoldBaseCost().FirstOrDefault().Value;
		int value2 = equipmentItemModel.SpEquipmentRemoldModel.CalculateRemoldLockedCost(lockedNoForceTraitCount).FirstOrDefault().Value;
		if (value + value2 > playerModel.GetCurrencyAmount(CurrencyType.SPTraitsRemoldToken))
		{
			flag = true;
		}
		if (flag)
		{
			ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(value + value2, CurrencyType.SPTraitsRemoldToken);
		}
		else if (Helpers.IsSpEquipmentRemoldAllLocked(equipmentItemModel))
		{
			SPRemoldNotice2Popup sPRemoldNotice2Popup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldNotice2Popup) as SPRemoldNotice2Popup;
			if (sPRemoldNotice2Popup != null)
			{
				sPRemoldNotice2Popup.Open();
			}
		}
		else if (!Helpers.IsSPRemold24Comfirm() || Helpers.IsSPRemold24ComfirmTimeOver())
		{
			SPRemoldConfirmAgainPopup sPRemoldConfirmAgainPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldConfirmAgainPopup) as SPRemoldConfirmAgainPopup;
			if (sPRemoldConfirmAgainPopup != null)
			{
				sPRemoldConfirmAgainPopup.SetInitData(equipmentItemModel, SPRemoldConfirmAgainPopup.InitType.Remold, GoRollCommand);
				sPRemoldConfirmAgainPopup.Open();
			}
		}
		else
		{
			GoRollCommand();
		}
	}

	public void GoRollCommand()
	{
		if (Helpers.ExecuteCommand(new SpEquipmentRemoldTraitsCommand(equipmentItemModel.ModelId)) == TWDModelResult.OK)
		{
			GameManager.Instance.CheckConnectionReachability(showPopup: true, "EquipRemodelCommand");
			UpdateUI();
			StartCoroutine(DelayRollUIEffect(0.1f));
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_workshop");
		}
	}

	private IEnumerator DelayRollUIEffect(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		UIEvent.Send("SPRemoldRandomChangedEffect");
	}

	public void OnClickUpgrade()
	{
		if (Time.time - lastClickTime < 1.75f)
		{
			return;
		}
		lastClickTime = Time.time;
		if (curIsMax)
		{
			curIsMax = !curIsMax;
			UpdateUI();
			return;
		}
		bool flag = false;
		if (GameManager.Instance.playerModel.gameEconomyData.SPTraitsRemoldConfigs.GetUpgradeCost().FirstOrDefault().Value > playerModel.GetCurrencyAmount(CurrencyType.SPTraitsUpgradeToken))
		{
			flag = true;
		}
		if (flag)
		{
			SPRemoldCommonNoticePopup sPRemoldCommonNoticePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldCommonNoticePopup) as SPRemoldCommonNoticePopup;
			if (sPRemoldCommonNoticePopup != null)
			{
				sPRemoldCommonNoticePopup.SetContent(LocalizationManager.GetText("System.EquipSPRemold.FuncInfo18"), LocalizationManager.GetText("System.EquipSPRemold.FuncInfo27"));
				sPRemoldCommonNoticePopup.Open();
			}
		}
		else if (equipmentItemModel.SpEquipmentRemoldModel.HasPendingRemold)
		{
			SPRemoldNotice1Popup sPRemoldNotice1Popup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldNotice1Popup) as SPRemoldNotice1Popup;
			if (sPRemoldNotice1Popup != null)
			{
				sPRemoldNotice1Popup.Open();
			}
		}
		else if (!Helpers.IsSPRemold24UpgradeComfirm() || Helpers.IsSPRemold24UpgradeComfirmTimeOver())
		{
			SPRemoldConfirmAgainPopup sPRemoldConfirmAgainPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldConfirmAgainPopup) as SPRemoldConfirmAgainPopup;
			if (sPRemoldConfirmAgainPopup != null)
			{
				sPRemoldConfirmAgainPopup.SetInitData(equipmentItemModel, SPRemoldConfirmAgainPopup.InitType.Upgrade, GoUpgradeCommand);
				sPRemoldConfirmAgainPopup.Open();
			}
		}
		else
		{
			GoUpgradeCommand();
		}
	}

	public void GoUpgradeCommand()
	{
		if (Helpers.ExecuteCommand(new SpEquipmentRemoldTraitsUpgradeCommand(equipmentItemModel.ModelId)) == TWDModelResult.OK)
		{
			GameManager.Instance.CheckConnectionReachability(showPopup: true, "EquipRemodelCommand");
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_upgrade_trait");
		}
	}

	public void OnclickResourceShop()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		ShopPopupHelper.OpenWithIndex(2);
	}

	public void OnclickPreMax()
	{
		curIsMax = !curIsMax;
		UpdateUI();
	}
}
