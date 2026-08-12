using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldTraiListPopup : HUDElement
{
	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private UILabel AttackRangeVal;

	[SerializeField]
	private UILabel DamageRangeVal;

	[SerializeField]
	private UILabel MaxTargetsVal;

	[SerializeField]
	private GameObject selectedIcon;

	[SerializeField]
	private GameObject unSelectedIcon;

	[SerializeField]
	private UIButton gotoBtn;

	[SerializeField]
	private GameObject anchor;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private bool isPreview;

	private string equipmentDefID;

	private EquipmentItemModel equipmentItemModel;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private EquipmentDefinition equipmentDefinition
	{
		get
		{
			if (isPreview)
			{
				return GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipmentDefID);
			}
			if (equipmentItemModel != null)
			{
				return equipmentItemModel.Definition;
			}
			return null;
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		ReloadAnchorPosition();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public void OpenForModel(EquipmentItemModel model)
	{
		base.OpenForModel(model);
		equipmentItemModel = model;
		UpdateUI();
	}

	public void OpenForPreview(string equipmentDefID)
	{
		base.Open();
		isPreview = true;
		this.equipmentDefID = equipmentDefID;
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
	}

	private void OnUiEvent(string type, object parameter)
	{
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (!Helpers.IsSystemOpenById("SystemBase.EquipRemold") || isPreview)
		{
			gotoBtn.defaultColor = Color.gray;
			gotoBtn.normalSprite = "UI_Button_WhitePlusGrey_Middle";
		}
		UpdateList();
		AbilityDefinition abilityDefinition = GameManager.Instance.gameEconomyData.GetAbilityDefinition(this.equipmentDefinition.AbilityIdentifier);
		EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(this.equipmentDefinition.ChargeEquipmentIdentifier);
		AbilityDefinition abilityDefinition2 = null;
		if (equipmentDefinition != null)
		{
			abilityDefinition2 = GameManager.Instance.gameEconomyData.GetAbilityDefinition(equipmentDefinition.AbilityIdentifier);
		}
		string text = LocalizationManager.GetText("BasicInfo.Ability.Base.Desc", abilityDefinition.AbilityRange);
		string text2 = "";
		if (abilityDefinition2 != null)
		{
			text2 = "/" + LocalizationManager.GetText("BasicInfo.Ability.Base.Desc", abilityDefinition2.AbilityRange);
		}
		AttackRangeVal.text = LocalizationManager.GetText("BasicInfo.Ability.AttackRange.Name") + text + text2;
		text = HelpersLocalization.GetWeaponAreaDescNoArea(abilityDefinition);
		text2 = "";
		if (abilityDefinition2 != null)
		{
			text2 = "/" + HelpersLocalization.GetWeaponAreaDescNoArea(abilityDefinition2);
		}
		DamageRangeVal.text = LocalizationManager.GetText("BasicInfo.Ability.DamageRange.Name") + text + text2;
		text = abilityDefinition.MaxAffectedTargetsCount.ToString() ?? "";
		text2 = "";
		if (abilityDefinition2 != null)
		{
			text2 = "/" + abilityDefinition2.MaxAffectedTargetsCount;
		}
		MaxTargetsVal.text = LocalizationManager.GetText("BasicInfo.Ability.MaxTarget.Name") + text + text2;
		Helpers.GameObjectSetActive(selectedIcon, Helpers.IsSPRemoldEasy());
		Helpers.GameObjectSetActive(unSelectedIcon, !Helpers.IsSPRemoldEasy());
	}

	private void UpdateList()
	{
		ClearBTLevelEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		UIScrollView componentInParent = EntryContainer.GetComponentInParent<UIScrollView>();
		FreshListData();
		component.Reposition();
		componentInParent.ResetPosition();
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
		List<SPTraitSlot> list = new List<SPTraitSlot>();
		if (isPreview)
		{
			list = Helpers.GetSPTraitSlotsForPreview(equipmentDefID);
		}
		if (equipmentItemModel != null && equipmentItemModel.SpEquipmentRemoldModel != null)
		{
			list = equipmentItemModel.SpEquipmentRemoldModel.SPTraitSlots;
		}
		if (list == null || list.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && !string.IsNullOrEmpty(list[i].ID))
			{
				GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
				NGUITools.SetActive(gameObject, state: true);
				if (gameObject.TryGetComponent<SPRemoldTraitListItem>(out var component))
				{
					component.Setup(list[i]);
					Entries.Add(gameObject);
				}
			}
		}
	}

	public void OnClickGoSPRemold()
	{
		if (equipmentItemModel == null || equipmentItemModel.SpEquipmentRemoldModel == null)
		{
			return;
		}
		if (!Helpers.IsSystemOpenById("SystemBase.EquipRemold"))
		{
			SPRemoldCommonNoticePopup sPRemoldCommonNoticePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldCommonNoticePopup) as SPRemoldCommonNoticePopup;
			if (sPRemoldCommonNoticePopup != null)
			{
				SystemOpen systemOpenById = GameManager.Instance.gameEconomyData.GetSystemOpenById("SystemBase.SPEquipRemold");
				if (systemOpenById != null)
				{
					sPRemoldCommonNoticePopup.SetContent(LocalizationManager.GetText("System.EquipSPRemold.FuncInfo18"), LocalizationManager.GetText(systemOpenById.UnOpenedTips));
					sPRemoldCommonNoticePopup.Open();
				}
			}
		}
		else
		{
			SPRemoldMainPopup sPRemoldMainPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldMainPopup) as SPRemoldMainPopup;
			if (sPRemoldMainPopup != null)
			{
				sPRemoldMainPopup.BindData(equipmentItemModel);
				sPRemoldMainPopup.Open();
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
			}
			Close();
		}
	}

	public void OnClickSwitchEasy()
	{
		Helpers.SetSPRemoldEasy(!Helpers.IsSPRemoldEasy());
		UpdateUI();
	}

	private void ReloadAnchorPosition()
	{
		Vector3 position = Vector3.zero;
		EquipmentUpgradePopup equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		if (equipmentUpgradePopup != null)
		{
			position = equipmentUpgradePopup.GetSPRemoldDescriptionButtonV();
		}
		anchor.transform.position = position;
	}
}
