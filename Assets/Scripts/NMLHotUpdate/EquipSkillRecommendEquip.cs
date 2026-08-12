using System;
using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class EquipSkillRecommendEquip : MonoBehaviour
{
	[SerializeField]
	private Transform classFilterPosition;

	[SerializeField]
	private GameObject classFilterPrefab;

	private GameObject classFilterInstance;

	[SerializeField]
	private EquipmentSelectionBox equipmentSelectionBox;

	[SerializeField]
	private GameObject EntryContainer;

	private GameObject EntryPrefab;

	private readonly List<GameObject> Entries = new List<GameObject>();

	[SerializeField]
	private UIButton SpriteCheck;

	[SerializeField]
	private GameObject EntryContainerCard;

	private GameObject EntryPrefabCard;

	private readonly List<GameObject> EntriesCard = new List<GameObject>();

	[SerializeField]
	private GameObject EntryContainerSelectParent;

	[SerializeField]
	private GameObject EntryContainerSelect;

	private GameObject EntryPrefabSelect;

	private readonly List<EquipSkillRecommendEquipSelectItem> EntriesSelect = new List<EquipSkillRecommendEquipSelectItem>();

	[SerializeField]
	private UIButton BtnSelect;

	[SerializeField]
	private UILabel LabelSelect;

	[SerializeField]
	private UIButton BtnCloseSelect;

	private EquipmentItemModel equipmentItemModel;

	private Dictionary<string, EquipSkillRecommendEquipModel> dictModelId = new Dictionary<string, EquipSkillRecommendEquipModel>();

	private Dictionary<string, List<EquipSkillRecommendEquipModel>> dictModel = new Dictionary<string, List<EquipSkillRecommendEquipModel>>();

	private List<EquipSkillRecommendEquipModel> curList;

	private int selectedTagIndex { get; set; }

	private string selectedTag => GetTagByIndex(selectedTagIndex);

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	private void Awake()
	{
		EntryPrefab = Helpers.GameObjectChildItem(EntryContainer);
		EntryPrefabCard = Helpers.GameObjectChildItem(EntryContainerCard);
		EntryPrefabSelect = Helpers.GameObjectChildItem(EntryContainerSelect);
		SpriteCheck.onClick.Add(new EventDelegate(OnClickDetailInfo));
		BtnCloseSelect.onClick.Add(new EventDelegate(OnClickCloseSelect));
		BtnSelect.onClick.Add(new EventDelegate(OnClickSelect));
		FreshListSelect();
	}

	private void OnDestroy()
	{
		ClearEvent();
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		foreach (KeyValuePair<string, EquipSkillRecommendEquipModel> item in dictModelId)
		{
			if (item.Value.GetIsChange())
			{
				dictionary[item.Key] = item.Value.CurrentLikeStatus.HasLiked;
			}
		}
		EquipSkillRecommendEquipModel.RequestLike(dictionary, null);
	}

	private void ClearEvent()
	{
		foreach (List<EquipSkillRecommendEquipModel> value in dictModel.Values)
		{
			foreach (EquipSkillRecommendEquipModel item in value)
			{
				item.OnRefreshCall = (Action<EquipSkillRecommendEquipModel>)Delegate.Remove(item.OnRefreshCall, new Action<EquipSkillRecommendEquipModel>(OnRefreshCall));
			}
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		if (classFilterInstance == null)
		{
			classFilterInstance = Helpers.InstantiateToParent(classFilterPrefab, classFilterPosition.gameObject);
			classFilterInstance.layer = base.gameObject.layer;
			classFilterInstance.transform.SetChildLayer(base.gameObject.layer);
			classFilterInstance.GetComponent<UIPanel>().depth = classFilterPosition.parent.GetComponentInParent<UIPanel>().depth + 1;
		}
		if (classFilterInstance != null)
		{
			SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
			if (component != null)
			{
				component.OnClassFilterSelected += OnClassFilterButtonClicked;
				component.SetGenericFilterButtonsEnabled(active: false);
				component.UpdatePositionAndState();
			}
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		if (classFilterInstance != null)
		{
			SurvivorClassFilter component = classFilterInstance.GetComponent<SurvivorClassFilter>();
			if (component != null)
			{
				component.OnClassFilterSelected -= OnClassFilterButtonClicked;
			}
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "NewRecommendEquipmentSelected":
		{
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton != null)
			{
				equipmentItemModel = equipmentButton.GetEquipment();
				UpdateUI();
			}
			break;
		}
		case "SPRemoldEquipModSkill":
			HUDNotification.Info(LocalizationManager.GetText("System.EquipSkillSuggestion.Info4"));
			UpdateUI();
			break;
		case "SPRemoldUnEquipModSkill":
			UpdateUI();
			break;
		}
	}

	private string GetTagByIndex(int index)
	{
		List<string> equipmentSkillSuggestionTags = GameManager.Instance.gameEconomyData.ConfigData.EquipmentSkillSuggestionTags;
		if (equipmentSkillSuggestionTags != null && index >= 0 && index < equipmentSkillSuggestionTags.Count)
		{
			return equipmentSkillSuggestionTags[index];
		}
		return string.Empty;
	}

	private void OnClassFilterButtonClicked(SurvivorClass selectedClass)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_tab_change");
		equipmentItemModel = equipmentSelectionBox.SetItems(equipmentItemModel, playerModel, selectedClass);
		UpdateUI();
	}

	public void OpenForModel(ModelObject model)
	{
		equipmentItemModel = model as EquipmentItemModel;
		SurvivorClassFilter survivorClassFilter = classFilterInstance?.GetComponent<SurvivorClassFilter>();
		if (survivorClassFilter != null)
		{
			survivorClassFilter.SetSelectedClass(equipmentItemModel.EquipmentSurvivorClass);
		}
		else
		{
			equipmentSelectionBox.SetItems(equipmentItemModel, playerModel, equipmentItemModel.EquipmentSurvivorClass);
		}
	}

	private void UpdateUI()
	{
		FreshListData();
		FreshRecommendListData();
	}

	private void FreshListSelect()
	{
		List<string> equipmentSkillSuggestionTags = GameManager.Instance.gameEconomyData.ConfigData.EquipmentSkillSuggestionTags;
		if (equipmentSkillSuggestionTags != null && equipmentSkillSuggestionTags.Count > 0)
		{
			selectedTagIndex = 0;
			UITable component = EntryContainerSelect.GetComponent<UITable>();
			for (int i = 0; i < equipmentSkillSuggestionTags.Count; i++)
			{
				if (EntryContainerSelect.AddChild(EntryPrefabSelect).TryGetComponent<EquipSkillRecommendEquipSelectItem>(out var component2))
				{
					component2.Initialize();
					component2.ActCall = OnClassFilterButtonClicked;
					component2.SetInfo(i, equipmentSkillSuggestionTags[i]);
					EntriesSelect.Add(component2);
				}
			}
			component.Reposition();
		}
		FreshListSelectState(enable: false);
	}

	private void FreshListSelectState(bool enable)
	{
		for (int i = 0; i < EntriesSelect?.Count; i++)
		{
			EntriesSelect[i].SetSelect(selectedTagIndex);
		}
		LabelSelect.text = LocalizationManager.GetText(EquipSkillRecommendEquipModel.GetTagText(selectedTag));
		Helpers.GameObjectSetActive(EntryContainerSelectParent, enable);
	}

	private void OnClassFilterButtonClicked(int index)
	{
		if (selectedTagIndex != index)
		{
			selectedTagIndex = index;
			FreshListSelectState(enable: false);
			FreshRecommendListData();
		}
	}

	private void FreshListData()
	{
		ClearEntries(Entries);
		Helpers.GameObjectSetActive(SpriteCheck.gameObject, equipmentItemModel != null);
		if (equipmentItemModel == null)
		{
			return;
		}
		UITable component = EntryContainer.GetComponent<UITable>();
		ModSkillSlot[] modSkillSlots = equipmentItemModel.ModSkillSlots;
		int num = ((modSkillSlots != null) ? modSkillSlots.Length : 0);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			if (gameObject.TryGetComponent<EquipSkillRecommendEquipItemItem>(out var component2))
			{
				component2.Setup(i, equipmentItemModel.ModSkillSlots[i].ModSkillMode);
				Entries.Add(gameObject);
			}
		}
		component.Reposition();
	}

	private void FreshRecommendListData()
	{
		ClearEntries(EntriesCard);
		ClearEvent();
		if (equipmentItemModel != null)
		{
			List<EquipmentSkillSuggestion> equipmentSkillSuggestionByEquipID = playerModel.gameEconomyData.GetEquipmentSkillSuggestionByEquipID(equipmentItemModel.Definition.ID);
			curList = GetListModel(equipmentItemModel.Definition.ID, equipmentSkillSuggestionByEquipID);
			for (int i = 0; i < curList?.Count; i++)
			{
				EquipSkillRecommendEquipModel equipSkillRecommendEquipModel = curList[i];
				equipSkillRecommendEquipModel.OnRefreshCall = (Action<EquipSkillRecommendEquipModel>)Delegate.Combine(equipSkillRecommendEquipModel.OnRefreshCall, new Action<EquipSkillRecommendEquipModel>(OnRefreshCall));
			}
			SortRecommendations(curList);
			OnSortRecommendList(curList);
		}
	}

	private void OnSortRecommendList(List<EquipSkillRecommendEquipModel> list)
	{
		ClearEntries(EntriesCard);
		for (int i = 0; i < list?.Count; i++)
		{
			GameObject gameObject = EntryContainerCard.AddChild(EntryPrefabCard);
			if (gameObject.TryGetComponent<EquipSkillRecommendEquipItem>(out var component))
			{
				component.Initialize();
				component.SetInfo(equipmentItemModel, list[i], selectedTag);
				EntriesCard.Add(gameObject);
			}
		}
		EntryContainerCard.GetComponent<UITable>().Reposition();
		EntryContainerCard.GetComponentInParent<UIScrollView>()?.ResetPosition();
	}

	private List<EquipSkillRecommendEquipModel> GetListModel(string id, List<EquipmentSkillSuggestion> listForm)
	{
		if (!dictModel.TryGetValue(id, out var value))
		{
			value = new List<EquipSkillRecommendEquipModel>();
			for (int i = 0; i < listForm?.Count; i++)
			{
				EquipmentSkillSuggestion equipmentSkillSuggestion = listForm[i];
				if (equipmentSkillSuggestion == null)
				{
					continue;
				}
				EquipmentDefinition equipmentDefinition = playerModel?.gameEconomyData?.GetEquipmentDefinition(id);
				if (equipmentDefinition != null && !(equipmentDefinition.SurvivorClass.ToString() != equipmentSkillSuggestion.Class))
				{
					if (!dictModelId.TryGetValue(equipmentSkillSuggestion.ID, out var value2))
					{
						value2 = new EquipSkillRecommendEquipModel(equipmentSkillSuggestion);
						dictModelId.Add(equipmentSkillSuggestion.ID, value2);
					}
					value.Add(value2);
				}
			}
			dictModel[id] = value;
		}
		return value;
	}

	private void OnRefreshCall(EquipSkillRecommendEquipModel conf)
	{
		if (curList == null || curList.Count != EntriesCard.Count)
		{
			return;
		}
		foreach (EquipSkillRecommendEquipModel cur in curList)
		{
			if (!cur.Default && cur.LikeStatusNet == null)
			{
				return;
			}
		}
		ClearEvent();
		SortRecommendations(curList);
		for (int i = 0; i < EntriesCard?.Count; i++)
		{
			EntriesCard[i].TryGetComponent<EquipSkillRecommendEquipItem>(out var component);
			component?.SetInfo(curList[i]);
		}
	}

	private void SortRecommendations(List<EquipSkillRecommendEquipModel> suggestions)
	{
		if (suggestions == null || suggestions.Count <= 1)
		{
			return;
		}
		if (selectedTagIndex == 0)
		{
			suggestions.Sort(delegate(EquipSkillRecommendEquipModel a, EquipSkillRecommendEquipModel b)
			{
				if (a.Default != b.Default)
				{
					if (!a.Default)
					{
						return 1;
					}
					return -1;
				}
				int likeCount = GetLikeCount(a);
				return GetLikeCount(b).CompareTo(likeCount);
			});
			return;
		}
		suggestions.Sort(delegate(EquipSkillRecommendEquipModel a, EquipSkillRecommendEquipModel b)
		{
			if (a.Default != b.Default)
			{
				if (!a.Default)
				{
					return 1;
				}
				return -1;
			}
			bool flag = HasTag(a, selectedTag);
			bool flag2 = HasTag(b, selectedTag);
			if (flag && !flag2)
			{
				return -1;
			}
			if (!flag && flag2)
			{
				return 1;
			}
			int likeCount = GetLikeCount(a);
			return GetLikeCount(b).CompareTo(likeCount);
		});
	}

	private bool HasTag(EquipSkillRecommendEquipModel suggestion, string tag)
	{
		if (suggestion.equipmentSkillSuggestion.Tags == null || string.IsNullOrEmpty(tag))
		{
			return false;
		}
		return suggestion.equipmentSkillSuggestion.Tags.Contains(tag);
	}

	private int GetLikeCount(EquipSkillRecommendEquipModel suggestion)
	{
		return suggestion.CurrentLikeStatus?.Count ?? 0;
	}

	private void ClearEntries(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			NGUITools.Destroy(list[i]);
		}
		list.Clear();
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

	private void OnClickSelect()
	{
		FreshListSelectState(!EntryContainerSelectParent.activeSelf);
	}

	private void OnClickCloseSelect()
	{
		Helpers.GameObjectSetActive(EntryContainerSelectParent, value: false);
	}
}
