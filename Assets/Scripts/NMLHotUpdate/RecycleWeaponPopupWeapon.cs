using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RecycleWeaponPopupWeapon : MonoBehaviour
{
	[SerializeField]
	private UILabel ClassName;

	[SerializeField]
	[Header("Display Image (Pic Field: List<string>[0])")]
	private UITexture Type2Image;

	[SerializeField]
	[Header("Selected Display")]
	private GameObject haveSelect;

	[SerializeField]
	private EquipmentButton equipmentButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon btnTips;

	[SerializeField]
	[Header("Select Panel")]
	private RecycleWeaponPopupWeaponSelect selectPanel;

	private RecycleWeaponDefinition _definition;

	private EquipmentItemModel _selectedWeapon;

	private int _selectedWeaponModelId => _selectedWeapon?.ModelId ?? 0;

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public bool HasSelection => _selectedWeaponModelId > 0;

	public Action OnSelectionChanged { get; set; }

	private void Awake()
	{
		btnTips?.onClick.Add(new EventDelegate(OnClickTips));
	}

	public void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "EquipmentScrapped":
			OnWeaponSelected(0);
			break;
		case "BreakThroughed":
		case "SPRemoldLockChanged":
		case "SPRemoldRandomChanged":
		case "SPRemoldUpgradeChanged":
		case "EquipmentInstantUpgraded":
			OnWeaponSelected(_selectedWeaponModelId);
			break;
		}
	}

	public void SetInfo(RecycleWeaponDefinition definition)
	{
		_definition = definition;
		CloseSelectPanel();
		ClearSelection();
		EquipmentDefinition equipmentDefinition = playerModel?.gameEconomyData?.GetEquipmentDefinition(definition.Object);
		if (equipmentDefinition != null)
		{
			ClassName.text = HelpersLocalization.GetEquipmentName(equipmentDefinition.ID);
		}
		BuildDisplayImage();
	}

	public void RefreshSelection(int weaponModelId)
	{
		bool flag = weaponModelId > 0;
		Helpers.GameObjectSetActive(haveSelect, flag);
		if (!flag)
		{
			_selectedWeapon = null;
			return;
		}
		_selectedWeapon = playerModel?.Equipment?.GetAllEquipments()?.Find((EquipmentItemModel e) => e.ModelId == weaponModelId);
		if (_selectedWeapon != null && equipmentButton != null)
		{
			equipmentButton.Setup(_selectedWeapon, null, null, string.Empty, showOwnerAndUpgradeIndicator: false);
		}
	}

	public void ClearSelection()
	{
		_selectedWeapon = null;
		RefreshSelection(0);
	}

	public void OpenSelectPanel()
	{
		List<EquipmentItemModel> recyclableWeapons = GetRecyclableWeapons();
		selectPanel?.OpenWeapons(_definition, recyclableWeapons, _selectedWeaponModelId, OnWeaponSelected);
	}

	public void CloseSelectPanel()
	{
		selectPanel?.Close();
	}

	public List<EquipmentItemModel> GetRecyclableWeapons()
	{
		List<EquipmentItemModel> list = playerModel?.Equipment?.GetAllEquipments();
		if (list == null || _definition == null)
		{
			return new List<EquipmentItemModel>();
		}
		string text = _definition.Object;
		List<string> list2 = new List<string>();
		if (playerModel != null && playerModel.gameEconomyData != null && playerModel.gameEconomyData.EquipTokenDefinitions != null)
		{
			EquipTokenDefinition[] equipTokenDefinitions = playerModel.gameEconomyData.EquipTokenDefinitions;
			foreach (EquipTokenDefinition equipTokenDefinition in equipTokenDefinitions)
			{
				if (equipTokenDefinition.EquipmentBreakthroughsType == text)
				{
					list2.Add(equipTokenDefinition.RelateEquipId);
				}
			}
		}
		List<EquipmentItemModel> list3 = new List<EquipmentItemModel>();
		foreach (string se in list2)
		{
			list3.AddRange(list.FindAll((EquipmentItemModel e) => e != null && e.EquipmentDefinitionIdentifier == se));
		}
		list3.Sort((EquipmentItemModel x, EquipmentItemModel y) => CompareEquipmentEquippedAndHigherLevelFirst(x, y));
		return list3;
	}

	private int CompareEquipmentEquippedAndHigherLevelFirst(EquipmentItemModel equipmentA, EquipmentItemModel equipmentB)
	{
		if (equipmentA.Owner != null && equipmentB.Owner == null)
		{
			return 1;
		}
		if (equipmentA.Owner == null && equipmentB.Owner != null)
		{
			return -1;
		}
		return 0;
	}

	public Rewards GetSelectedRewards(out int blueprintCount)
	{
		blueprintCount = 0;
		if (_selectedWeaponModelId <= 0)
		{
			return null;
		}
		RecycleWeaponRewardDefinition weaponRewardDefinition = GetWeaponRewardDefinition();
		if (string.IsNullOrEmpty(weaponRewardDefinition?.RewardShow))
		{
			return null;
		}
		blueprintCount = 1;
		return new Rewards(weaponRewardDefinition.RewardShow);
	}

	public List<RewardShowPicEntry> GetSelectedRewardsPic(out int blueprintCount)
	{
		blueprintCount = 0;
		if (_selectedWeaponModelId <= 0)
		{
			return null;
		}
		RecycleWeaponRewardDefinition weaponRewardDefinition = GetWeaponRewardDefinition();
		if (weaponRewardDefinition == null)
		{
			return null;
		}
		blueprintCount = 1;
		return weaponRewardDefinition?.RewardShowPicEntries;
	}

	public int GetSurvivalPoints()
	{
		return (_selectedWeapon?.GetScrapCashier)?.GetTotalCost(CurrencyType.SurvivalPoints) ?? 0;
	}

	public TWDModelResult ExecuteRecycle(int activityModelId)
	{
		if (_selectedWeaponModelId <= 0)
		{
			return TWDModelResult.Error;
		}
		return Helpers.ExecuteCommand(new RecyleWeaponCommand(new List<int> { _selectedWeaponModelId }, activityModelId));
	}

	private RecycleWeaponRewardDefinition GetWeaponRewardDefinition()
	{
		if (_selectedWeapon == null || _definition == null)
		{
			return null;
		}
		return playerModel?.gameEconomyData?.GetRecycleWeaponRewardDefinitionByLevel(_definition.Identifier, _selectedWeapon.BreakthroughLevel);
	}

	private void OnWeaponSelected(int id)
	{
		RefreshSelection(id);
		OnSelectionChanged?.Invoke();
	}

	private void BuildDisplayImage()
	{
		if (Type2Image == null || _definition?.Pic == null || _definition.Pic.Count == 0)
		{
			return;
		}
		if (!string.IsNullOrEmpty(_definition.Pic[0]))
		{
			UnityEngine.Object obj = UnityUtils.LoadFromAssetBundle(_definition.Pic[0], "itemgraphics");
			if (obj != null)
			{
				Type2Image.mainTexture = (Texture)obj;
			}
			Helpers.GameObjectSetActive(Type2Image.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(Type2Image.gameObject, value: false);
		}
	}

	private void OnClickTips()
	{
		if (_selectedWeapon != null)
		{
			Helpers.OpenEquipmentUpgradePopup(equipmentButton.GetEquipment());
		}
	}
}
