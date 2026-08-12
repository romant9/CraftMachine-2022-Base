using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class RecycleWeaponPopupBlueprints : MonoBehaviour
{
	[SerializeField]
	private UITable table1;

	[SerializeField]
	private UITable table2;

	[SerializeField]
	private UISprite ClassIcon;

	[SerializeField]
	private UILabel ClassName;

	[Header("Display Images (Pic Field: List<string>)")]
	[SerializeField]
	private EquipmentTokenButton Type1Image1;

	[SerializeField]
	private EquipmentTokenButton Type1Image2;

	[SerializeField]
	private EquipmentTokenButton Type1Image3;

	[Header("Selected Display")]
	[SerializeField]
	private GameObject haveSelect;

	[SerializeField]
	private EquipmentTokenButton equipmentTokenButton;

	[SerializeField]
	private UILabel LabelNum;

	[SerializeField]
	private GameObject ImageNum;

	[SerializeField]
	[Header("Select Panel")]
	private RecycleWeaponPopupBlueprintsSelect selectPanel;

	private RecycleWeaponDefinition _definition;

	private RecycleWeaponRewardDefinition _rewardDef;

	private EquipmentTokenButton[] _displayImages;

	private readonly List<string> _selectedBlueprintIds = new List<string>();

	private PlayerModel playerModel => GameManager.Instance.playerModel;

	public bool HasSelection => _selectedBlueprintIds.Count > 0;

	public List<string> SelectedIds => new List<string>(_selectedBlueprintIds);

	public Action OnSelectionChanged { get; set; }

	private void OnEnable()
	{
		table1.Reposition();
		table2.Reposition();
	}

	public void SetInfo(RecycleWeaponDefinition definition)
	{
		_definition = definition;
		_rewardDef = playerModel?.gameEconomyData?.GetRecycleWeaponRewardDefinition(_definition.Reward, _definition.Type);
		_displayImages = new EquipmentTokenButton[3] { Type1Image1, Type1Image2, Type1Image3 };
		CloseSelectPanel();
		ClearSelection();
		ClassIcon.spriteName = HelpersGfx.GetSurvivorEventIconName(definition.Object);
		ClassName.text = HelpersLocalization.GetSurvivorClassName(definition.Object);
		table1.Reposition();
		table2.Reposition();
		BuildDisplayImages();
	}

	public void RefreshSelection(List<string> selectedIds)
	{
		_selectedBlueprintIds.Clear();
		if (selectedIds != null)
		{
			_selectedBlueprintIds.AddRange(selectedIds);
		}
		int count = _selectedBlueprintIds.Count;
		Helpers.GameObjectSetActive(haveSelect, count > 0);
		if (count == 0)
		{
			return;
		}
		if (equipmentTokenButton != null && _selectedBlueprintIds.Count > 0)
		{
			string firstId = _selectedBlueprintIds[0];
			EquipTokenItemModel equipTokenItemModel = playerModel?.EquipTokenContainer?.EquipTokenItems?.Find((EquipTokenItemModel t) => t.EquipTokenId == firstId);
			if (equipTokenItemModel != null)
			{
				equipmentTokenButton.SetUpForReward(equipTokenItemModel, "RecycleBlueprintClick");
			}
		}
		HelpersUI.SetContentToLabel(LabelNum, $"{count}");
		Helpers.GameObjectSetActive(ImageNum, count >= 3);
	}

	public void ClearSelection()
	{
		_selectedBlueprintIds.Clear();
		RefreshSelection(new List<string>());
	}

	public void OpenSelectPanel(int remainLimit)
	{
		List<EquipTokenItemModel> recyclableBlueprints = GetRecyclableBlueprints();
		selectPanel?.OpenBlueprints(_definition, recyclableBlueprints, _selectedBlueprintIds, remainLimit, OnBlueprintsSelected);
	}

	public void CloseSelectPanel()
	{
		selectPanel?.Close();
	}

	public List<EquipTokenItemModel> GetRecyclableBlueprints()
	{
		ModelList<EquipTokenItemModel> modelList = playerModel?.EquipTokenContainer?.EquipTokenItems;
		if (modelList == null || _definition == null)
		{
			return new List<EquipTokenItemModel>();
		}
		string targetClass = _definition.Object;
		return modelList.Models?.FindAll((EquipTokenItemModel t) => t != null && t.OwnedTokensAmount > 0 && t.Definition != null && t.Definition.SurvivorClass.ToString() == targetClass) ?? new List<EquipTokenItemModel>();
	}

	public Rewards GetSelectedRewards(out int blueprintCount)
	{
		blueprintCount = _selectedBlueprintIds.Count;
		if (blueprintCount == 0)
		{
			return null;
		}
		if (string.IsNullOrEmpty(_rewardDef?.RewardShow))
		{
			return null;
		}
		return new Rewards(_rewardDef.RewardShow);
	}

	public List<RewardShowPicEntry> GetSelectedRewardsPic(out int blueprintCount)
	{
		blueprintCount = _selectedBlueprintIds.Count;
		if (blueprintCount == 0)
		{
			return null;
		}
		return _rewardDef.RewardShowPicEntries;
	}

	public TWDModelResult ExecuteRecycle(int activityModelId)
	{
		if (_selectedBlueprintIds.Count == 0)
		{
			return TWDModelResult.Error;
		}
		return Helpers.ExecuteCommand(new RecyleBlueprintsCommand(_selectedBlueprintIds.ToList(), activityModelId));
	}

	private void OnBlueprintsSelected(List<string> selectedIds)
	{
		RefreshSelection(selectedIds);
		OnSelectionChanged?.Invoke();
	}

	private void BuildDisplayImages()
	{
		if (_definition?.Pic == null)
		{
			return;
		}
		for (int i = 0; i < _displayImages?.Length; i++)
		{
			if (_displayImages[i] == null)
			{
				continue;
			}
			if (i < _definition.Pic.Count && !string.IsNullOrEmpty(_definition.Pic[i]))
			{
				if (playerModel.gameEconomyData.GetEquipTokenDefinition(_definition.Pic[i]) == null)
				{
					Helpers.GameObjectSetActive(_displayImages[i].gameObject, value: false);
					continue;
				}
				Helpers.GameObjectSetActive(_displayImages[i].gameObject, value: true);
				EquipTokenItemModel equipTokenItemModel = new EquipTokenItemModel(_definition.Pic[i], 1);
				equipTokenItemModel.SetManager(GameManager.Instance.modelManager);
				equipTokenItemModel.Initialize();
				_displayImages[i].SetUpForReward(equipTokenItemModel);
			}
			else
			{
				Helpers.GameObjectSetActive(_displayImages[i].gameObject, value: false);
			}
		}
	}
}
