using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillMainItem : MonoBehaviour
{
	[SerializeField]
	private GameObject normalContent;

	[SerializeField]
	private GameObject noneContent;

	[SerializeField]
	private UISprite skillBg;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private GameObject selectGo;

	private ModSkillSlot modSkillSlot;

	private int highlightOperateSlotIndex = -1;

	private readonly List<GameObject> Entries = new List<GameObject>();

	public bool IsEmpty
	{
		get
		{
			if (modSkillSlot != null && modSkillSlot.ModSkillMode != null)
			{
				return slotDefinition == null;
			}
			return true;
		}
	}

	private SPTraitsRemoldDefinitions slotDefinition
	{
		get
		{
			if (modSkillSlot == null || modSkillSlot.ModSkillMode == null)
			{
				return null;
			}
			return modSkillSlot.ModSkillMode.GetSpTraitsDefaultTrait();
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SPRemoldOperateItemClick":
			if (parameter is int num)
			{
				highlightOperateSlotIndex = num;
				ApplyOperateSlotSelectionHighlight();
			}
			break;
		case "SPRemoldEquipModSkill":
		case "SPRemoldUnEquipModSkill":
			UpdateUI();
			break;
		}
	}

	public void Setup(ModSkillSlot modSkillSlot, int highlightOperateSlotIndex = -1)
	{
		this.modSkillSlot = modSkillSlot;
		this.highlightOperateSlotIndex = highlightOperateSlotIndex;
		UpdateUI();
	}

	public void UpdateUI()
	{
		Helpers.GameObjectSetActive(selectGo, value: false);
		Helpers.GameObjectSetActive(normalContent, value: false);
		Helpers.GameObjectSetActive(noneContent, value: false);
		if (IsEmpty)
		{
			Helpers.GameObjectSetActive(noneContent, value: true);
			ApplyOperateSlotSelectionHighlight();
			return;
		}
		Helpers.GameObjectSetActive(normalContent, value: true);
		skillBg.color = Helpers.HexToColor(slotDefinition.Color);
		HelpersUI.SetTraitsIconOnSprite(traitIcon, slotDefinition.SPTraitsIcon, slotDefinition.SPTraitsIconOnCloud);
		starList.Setup(slotDefinition.Star);
		level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", slotDefinition.Level);
		ApplyOperateSlotSelectionHighlight();
	}

	private void ApplyOperateSlotSelectionHighlight()
	{
		bool value = highlightOperateSlotIndex >= 0 && modSkillSlot != null && highlightOperateSlotIndex == modSkillSlot.Index;
		Helpers.GameObjectSetActive(selectGo, value);
	}

	public void OnclickOperate()
	{
		if (!Helpers.IsSystemOpenById("SystemBase.SPEquipRemold"))
		{
			HUDNotification.Info(LocalizationManager.GetText("System.EquipSPRemold.FuncInfo13"));
		}
		else
		{
			UIEvent.Send("SPRemoldOperateItemClick", modSkillSlot.Index);
		}
	}
}
