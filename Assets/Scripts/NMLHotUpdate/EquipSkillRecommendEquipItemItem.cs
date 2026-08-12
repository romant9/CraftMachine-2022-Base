using TWDModel;
using UnityEngine;

public class EquipSkillRecommendEquipItemItem : MonoBehaviour
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

	[SerializeField]
	private GameObject noGet;

	private ModSkillMode modSkillDefault;

	private ModSkillMode modSkillEquip;

	private int index;

	private int highlightOperateSlotIndex = -1;

	private ModSkillMode modSkillSlotShow
	{
		get
		{
			if (modSkillEquip == null)
			{
				return modSkillDefault;
			}
			return modSkillEquip;
		}
	}

	private SPTraitsRemoldDefinitions slotDefinition
	{
		get
		{
			if (modSkillSlotShow == null)
			{
				return null;
			}
			return modSkillSlotShow.GetSpTraitsDefaultTrait();
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
		if (type == "SPRemoldOperateItemClick" && parameter is int num)
		{
			highlightOperateSlotIndex = num;
			ApplyOperateSlotSelectionHighlight();
		}
	}

	public void Setup(int i, ModSkillMode modSkillSlot, int highlightOperateSlotIndex = -1)
	{
		index = i;
		modSkillDefault = modSkillSlot;
		this.highlightOperateSlotIndex = highlightOperateSlotIndex;
		UpdateUI();
	}

	public void UpdateUI()
	{
		modSkillEquip = GameManager.Instance.playerModel.ModSkillManager.GetModSkillModeByGroupID(modSkillDefault?.Type);
		Helpers.GameObjectSetActive(selectGo, value: false);
		Helpers.GameObjectSetActive(normalContent, value: false);
		Helpers.GameObjectSetActive(noneContent, value: false);
		if (modSkillSlotShow == null || slotDefinition == null)
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
		Helpers.GameObjectSetActive(noGet, modSkillSlotShow.ModSkillState == ModSkillState.Count);
		ApplyOperateSlotSelectionHighlight();
	}

	private void ApplyOperateSlotSelectionHighlight()
	{
		bool value = highlightOperateSlotIndex >= 0 && modSkillSlotShow != null && highlightOperateSlotIndex == index;
		Helpers.GameObjectSetActive(selectGo, value);
	}
}
