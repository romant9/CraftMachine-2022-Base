using System;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class RecycleWeaponPopupBlueprintsSelect : MonoBehaviour
{
	[Header("Blueprint List")]
	[SerializeField]
	private GameObject EntryContainerBlueprints;

	private GameObject EntryContainerBlueprintsItem;

	private readonly List<GameObject> EntryContainerBlueprintss = new List<GameObject>();

	[SerializeField]
	private UIScrollView scrollViewBlueprints;

	[Header("Close Button")]
	[SerializeField]
	private UIButton BtnClose;

	[SerializeField]
	[Header("Blueprint Mode Controls")]
	private UILabel selectedNumLabel;

	[SerializeField]
	private UIButton selectAllButton;

	[SerializeField]
	private UIButton diselectAllButton;

	[SerializeField]
	private UIButton confirmButton;

	[SerializeField]
	[Header("Title")]
	private GameObject titleLabel;

	[SerializeField]
	private GameObject titleLabelNo;

	private List<EquipTokenItemModel> _blueprintList = new List<EquipTokenItemModel>();

	private Dictionary<string, HashSet<int>> _selectedBlueprintSlots = new Dictionary<string, HashSet<int>>();

	private int _totalSelectedCount;

	private readonly List<(string id, int index)> _blueprintSlotInfo = new List<(string, int)>();

	private int _blueprintMaxSelect;

	private Action<List<string>> _onBlueprintsConfirm;

	private void Awake()
	{
		BtnClose.onClick.Add(new EventDelegate(Close));
		selectAllButton?.onClick.Add(new EventDelegate(OnClickSelectAll));
		diselectAllButton?.onClick.Add(new EventDelegate(OnClickDisSelectAll));
		confirmButton?.onClick.Add(new EventDelegate(OnClickConfirm));
		EntryContainerBlueprintsItem = Helpers.GameObjectChildItem(EntryContainerBlueprints);
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
		_ = type == "RecycleBlueprintClick";
	}

	public void OpenBlueprints(RecycleWeaponDefinition definition, List<EquipTokenItemModel> blueprints, List<string> preSelectedIds, int maxSelect, Action<List<string>> onConfirm)
	{
		_blueprintList = blueprints ?? new List<EquipTokenItemModel>();
		_onBlueprintsConfirm = onConfirm;
		_blueprintMaxSelect = maxSelect;
		_selectedBlueprintSlots.Clear();
		_totalSelectedCount = 0;
		if (preSelectedIds != null)
		{
			foreach (IGrouping<string, string> item in from result in preSelectedIds
				group result by result)
			{
				string id = item.Key;
				int a = item.Count();
				int b = _blueprintList.FirstOrDefault((EquipTokenItemModel x) => x != null && x.EquipTokenId == id)?.OwnedTokensAmount ?? 0;
				int num = Mathf.Min(a, b);
				if (num > 0)
				{
					HashSet<int> hashSet = new HashSet<int>();
					for (int num2 = 0; num2 < num; num2++)
					{
						hashSet.Add(num2);
					}
					_selectedBlueprintSlots[id] = hashSet;
					_totalSelectedCount += num;
				}
			}
		}
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		Helpers.GameObjectSetActive(EntryContainerBlueprints, value: true);
		Helpers.GameObjectSetActive(selectAllButton?.gameObject, value: true);
		Helpers.GameObjectSetActive(confirmButton?.gameObject, value: true);
		Helpers.GameObjectSetActive(selectedNumLabel?.gameObject, value: true);
		BuildBlueprintList();
		UpdateBottomUI();
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void BuildBlueprintList()
	{
		ClearList(EntryContainerBlueprintss);
		_blueprintSlotInfo.Clear();
		if (_blueprintList.Count == 0)
		{
			Helpers.GameObjectSetActive(titleLabelNo, value: true);
			return;
		}
		Helpers.GameObjectSetActive(titleLabelNo, value: false);
		for (int i = 0; i < _blueprintList.Count; i++)
		{
			EquipTokenItemModel equipTokenItemModel = _blueprintList[i];
			if (equipTokenItemModel == null)
			{
				continue;
			}
			int ownedTokensAmount = equipTokenItemModel.OwnedTokensAmount;
			for (int j = 0; j < ownedTokensAmount; j++)
			{
				GameObject gameObject = EntryContainerBlueprints.AddChild(EntryContainerBlueprintsItem);
				if (!gameObject.TryGetComponent<EquipmentTokenButton>(out var component))
				{
					continue;
				}
				component.SetUpForReward(equipTokenItemModel, "RecycleBlueprintClick");
				UIButton componentInChildren = gameObject.GetComponentInChildren<UIButton>(includeInactive: true);
				if (componentInChildren != null)
				{
					string capturedId = equipTokenItemModel.EquipTokenId;
					int capturedIndex = j;
					componentInChildren.onClick.Add(new EventDelegate(delegate
					{
						OnBlueprintSlotClicked(capturedId, capturedIndex);
					}));
				}
				bool selectStateUI = _selectedBlueprintSlots.ContainsKey(equipTokenItemModel.EquipTokenId) && _selectedBlueprintSlots[equipTokenItemModel.EquipTokenId].Contains(j);
				component.SetSelectStateUI(selectStateUI);
				_blueprintSlotInfo.Add((equipTokenItemModel.EquipTokenId, j));
				EntryContainerBlueprintss.Add(gameObject);
			}
		}
		EntryContainerBlueprints.GetComponent<UITable>()?.Reposition();
		scrollViewBlueprints?.ResetPosition();
	}

	private void OnBlueprintSlotClicked(string id, int slotIndex)
	{
		if (string.IsNullOrEmpty(id))
		{
			return;
		}
		if (!_selectedBlueprintSlots.ContainsKey(id))
		{
			_selectedBlueprintSlots[id] = new HashSet<int>();
		}
		HashSet<int> hashSet = _selectedBlueprintSlots[id];
		bool flag = hashSet.Contains(slotIndex);
		if (flag)
		{
			hashSet.Remove(slotIndex);
			_totalSelectedCount--;
			if (hashSet.Count == 0)
			{
				_selectedBlueprintSlots.Remove(id);
			}
		}
		else
		{
			if (_totalSelectedCount >= _blueprintMaxSelect)
			{
				return;
			}
			hashSet.Add(slotIndex);
			_totalSelectedCount++;
		}
		for (int i = 0; i < _blueprintSlotInfo.Count; i++)
		{
			if (_blueprintSlotInfo[i].id == id && _blueprintSlotInfo[i].index == slotIndex)
			{
				RefreshBlueprintItemSelect(i, !flag);
				break;
			}
		}
		UpdateBottomUI();
	}

	private void RefreshBlueprintItemSelect(int index, bool isSelected)
	{
		if (index >= 0 && index < EntryContainerBlueprintss.Count && EntryContainerBlueprintss[index].TryGetComponent<EquipmentTokenButton>(out var component))
		{
			component.SetSelectStateUI(isSelected);
		}
	}

	private void UpdateBottomUI()
	{
		int totalSelectedCount = _totalSelectedCount;
		HelpersUI.SetContentToLabel(selectedNumLabel, LocalizationManager.GetText("RecycleBlueprints.SelectedNum", totalSelectedCount, _blueprintMaxSelect));
		int num = Mathf.Min(_blueprintSlotInfo.Count, _blueprintMaxSelect);
		bool flag = totalSelectedCount > 0 && totalSelectedCount >= num;
		Helpers.GameObjectSetActive(selectAllButton, !flag);
		Helpers.GameObjectSetActive(diselectAllButton, flag);
		confirmButton.isEnabled = totalSelectedCount > 0;
	}

	private void OnClickSelectAll()
	{
		_selectedBlueprintSlots.Clear();
		_totalSelectedCount = 0;
		for (int i = 0; i < _blueprintList.Count; i++)
		{
			if (_totalSelectedCount >= _blueprintMaxSelect)
			{
				break;
			}
			EquipTokenItemModel equipTokenItemModel = _blueprintList[i];
			if (equipTokenItemModel == null)
			{
				continue;
			}
			int ownedTokensAmount = equipTokenItemModel.OwnedTokensAmount;
			HashSet<int> hashSet = new HashSet<int>();
			for (int j = 0; j < ownedTokensAmount; j++)
			{
				if (_totalSelectedCount >= _blueprintMaxSelect)
				{
					break;
				}
				hashSet.Add(j);
				_totalSelectedCount++;
			}
			if (hashSet.Count > 0)
			{
				_selectedBlueprintSlots[equipTokenItemModel.EquipTokenId] = hashSet;
			}
		}
		for (int k = 0; k < EntryContainerBlueprintss.Count; k++)
		{
			bool isSelected = k < _blueprintSlotInfo.Count && _selectedBlueprintSlots.ContainsKey(_blueprintSlotInfo[k].id) && _selectedBlueprintSlots[_blueprintSlotInfo[k].id].Contains(_blueprintSlotInfo[k].index);
			RefreshBlueprintItemSelect(k, isSelected);
		}
		UpdateBottomUI();
	}

	private void OnClickDisSelectAll()
	{
		_selectedBlueprintSlots.Clear();
		_totalSelectedCount = 0;
		for (int i = 0; i < EntryContainerBlueprintss.Count; i++)
		{
			RefreshBlueprintItemSelect(i, isSelected: false);
		}
		UpdateBottomUI();
	}

	private void OnClickConfirm()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, HashSet<int>> selectedBlueprintSlot in _selectedBlueprintSlots)
		{
			string key = selectedBlueprintSlot.Key;
			int count = selectedBlueprintSlot.Value.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(key);
			}
		}
		_onBlueprintsConfirm?.Invoke(list);
		Close();
	}

	private void ClearList(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			NGUITools.Destroy(list[i]);
		}
		list.Clear();
	}
}
