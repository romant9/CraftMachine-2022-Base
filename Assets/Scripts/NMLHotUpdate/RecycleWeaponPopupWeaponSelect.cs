using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RecycleWeaponPopupWeaponSelect : MonoBehaviour
{
	[SerializeField]
	[Header("Weapon List")]
	private GameObject EntryContainerWeapon;

	private GameObject EntryContainerWeaponItem;

	private readonly List<GameObject> EntryContainerWeapons = new List<GameObject>();

	[SerializeField]
	private UIScrollView scrollViewWeapon;

	[SerializeField]
	[Header("Close Button")]
	private UIButton BtnClose;

	[SerializeField]
	[Header("Title")]
	private GameObject titleLabel;

	[SerializeField]
	private GameObject titleLabelNo;

	private List<EquipmentItemModel> _weaponList = new List<EquipmentItemModel>();

	private Action<int> _onWeaponSelected;

	private void Awake()
	{
		BtnClose.onClick.Add(new EventDelegate(Close));
		EntryContainerWeaponItem = Helpers.GameObjectChildItem(EntryContainerWeapon);
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
		if (type == "RecycleWeaponEquipmentSelected" && parameter is EquipmentButton equipmentButton)
		{
			OnWeaponItemClick(equipmentButton);
		}
	}

	public void OpenWeapons(RecycleWeaponDefinition definition, List<EquipmentItemModel> weapons, int preSelectedModelId, Action<int> onSelected)
	{
		_weaponList = weapons ?? new List<EquipmentItemModel>();
		_onWeaponSelected = onSelected;
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		Helpers.GameObjectSetActive(EntryContainerWeapon, value: true);
		BuildWeaponList(preSelectedModelId);
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void BuildWeaponList(int preSelectedModelId)
	{
		ClearList(EntryContainerWeapons);
		if (_weaponList.Count == 0)
		{
			Helpers.GameObjectSetActive(titleLabelNo, value: true);
			return;
		}
		Helpers.GameObjectSetActive(titleLabelNo, value: false);
		for (int i = 0; i < _weaponList.Count; i++)
		{
			EquipmentItemModel equipmentItemModel = _weaponList[i];
			if (equipmentItemModel == null)
			{
				continue;
			}
			GameObject gameObject = EntryContainerWeapon.AddChild(EntryContainerWeaponItem);
			if (gameObject.TryGetComponent<EquipmentButton>(out var component))
			{
				if (!gameObject.TryGetComponent<TooltipTarget>(out var component2))
				{
					component2 = gameObject.AddComponent<TooltipTarget>();
				}
				component2.OrientationOverride = TooltipTarget.Orientation.CENTER;
				component.isDisableScrap = false;
				component.Setup(equipmentItemModel, null, null, "RecycleWeaponEquipmentSelected", showOwnerAndUpgradeIndicator: false);
				component.OnSelectionHighlight(equipmentItemModel.ModelId == preSelectedModelId);
				EntryContainerWeapons.Add(gameObject);
			}
		}
		EntryContainerWeapon.GetComponent<UIGrid>()?.Reposition();
		scrollViewWeapon?.ResetPosition();
	}

	private void OnWeaponItemClick(EquipmentButton equipmentButton)
	{
		EquipmentItemModel equipmentItemModel = equipmentButton?.GetEquipment();
		if (equipmentItemModel != null)
		{
			if (equipmentItemModel.Owner != null)
			{
				string text = LocalizationManager.GetText("Popup.ScrapConfirmation.InvalidEquipmentScrapMessage{survivorName}", equipmentItemModel.Owner.Name);
				TooltipManager.OpenTextBoxWithText(equipmentButton.gameObject, text, TooltipManager.Prefabs.TooltipTextboxGold);
			}
			else
			{
				_onWeaponSelected?.Invoke(equipmentItemModel.ModelId);
				Close();
			}
		}
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
