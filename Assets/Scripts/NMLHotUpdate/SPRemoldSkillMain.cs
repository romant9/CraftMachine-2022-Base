using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SPRemoldSkillMain : MonoBehaviour
{
	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject bgRecommend;

	[SerializeField]
	private GameObject btnRecommend;

	private EquipmentItemModel equipmentItemModel;

	private EquipmentDefinition equipmentDefinition;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private int highlightOperateSlotIndex = -1;

	private string operateSelectedModSkillId = "";

	private bool IsOpenRecommend => Helpers.IsSystemOpenById("SystemBase.SkillBag");

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
		if (!(type != "SPRemoldOperatePreviewItemClick") && parameter is string text)
		{
			operateSelectedModSkillId = text;
		}
	}

	public void Setup(EquipmentDefinition equipmentDefinition, int rarityLevel, int level, int highlightOperateSlotIndex = -1)
	{
		this.equipmentDefinition = equipmentDefinition;
		this.highlightOperateSlotIndex = highlightOperateSlotIndex;
		operateSelectedModSkillId = "";
		if (highlightOperateSlotIndex >= 0 && equipmentItemModel != null && highlightOperateSlotIndex < equipmentItemModel.ModSkillSlots.Length)
		{
			operateSelectedModSkillId = equipmentItemModel.ModSkillSlots[highlightOperateSlotIndex].ModSkillMode?.ID ?? "";
		}
		UpdateUI();
	}

	public void Setup(EquipmentItemModel equipmentItemModel, int highlightOperateSlotIndex = -1)
	{
		this.equipmentItemModel = equipmentItemModel;
		Setup(equipmentItemModel.Definition, equipmentItemModel.RarityLevel, equipmentItemModel.Level, highlightOperateSlotIndex);
	}

	public void UpdateUI()
	{
		HelpersUI.SetSprite(classIcon, HelpersGfx.GetSurvivorClassSmallIconName(equipmentDefinition.SurvivorClass));
		FreshListData();
	}

	private void FreshListData()
	{
		ClearEntries();
		if (equipmentItemModel == null)
		{
			Helpers.GameObjectSetActive(bgRecommend, value: false);
			Helpers.GameObjectSetActive(btnRecommend, value: false);
			return;
		}
		UITable component = EntryContainer.GetComponent<UITable>();
		ModSkillSlot[] modSkillSlots = equipmentItemModel.ModSkillSlots;
		int num = ((modSkillSlots != null) ? modSkillSlots.Length : 0);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<SPRemoldSkillMainItem>(out var component2))
			{
				component2.Setup(equipmentItemModel.ModSkillSlots[i], highlightOperateSlotIndex);
				if (!component2.IsEmpty)
				{
					num2++;
				}
				Entries.Add(gameObject);
			}
		}
		component.Reposition();
		Helpers.GameObjectSetActive(bgRecommend, num2 <= 0 && IsOpenRecommend);
		Helpers.GameObjectSetActive(btnRecommend, IsOpenRecommend);
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public void OnClickDetailInfo()
	{
		if (equipmentItemModel != null)
		{
			SPRemoldTraitsSkillDetailInfoPopup sPRemoldTraitsSkillDetailInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsSkillDetailInfoPopup) as SPRemoldTraitsSkillDetailInfoPopup;
			if (!(sPRemoldTraitsSkillDetailInfoPopup == null))
			{
				sPRemoldTraitsSkillDetailInfoPopup.Setup(equipmentItemModel);
				sPRemoldTraitsSkillDetailInfoPopup.Open();
			}
		}
	}

	public void OnClickRecommendInfo()
	{
		if (equipmentItemModel != null)
		{
			EquipSkillRecommend equipSkillRecommend = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.EquipSkillRecommend) as EquipSkillRecommend;
			if (!(equipSkillRecommend == null))
			{
				equipSkillRecommend.OpenForModel(equipmentItemModel);
			}
		}
	}
}
