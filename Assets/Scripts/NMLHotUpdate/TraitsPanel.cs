using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class TraitsPanel : MonoBehaviour
{
	[SerializeField]
	private SurvivorTraitsButton[] traitsButtons;

	[SerializeField]
	private SurvivorTraitsButton OverwatchTraitButton;

	[SerializeField]
	private UILabel traitsLabel;

	[SerializeField]
	private SurvivorTraitsButton BreakThroughTraitsButton;

	[SerializeField]
	private UILabel BTipsLabel;

	public void setInfo(EquipmentItemModel equipmentModel, List<UpgradeTraitsData> traitsDataList, int currentPlayerLevel, int skipFirstAmount = 1, bool showThisLevelUnlocks = false)
	{
		if (traitsLabel != null)
		{
			traitsLabel.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.Traits");
		}
		base.gameObject.SetActive(value: true);
		for (int i = 0; i < traitsButtons.Length; i++)
		{
			if (traitsButtons[i] != null)
			{
				if (traitsDataList.Count - skipFirstAmount > i)
				{
					traitsButtons[i].initWithTrait(equipmentModel, traitsDataList[i + skipFirstAmount], currentPlayerLevel, showThisLevelUnlocks);
				}
				else
				{
					traitsButtons[i].initWithTrait();
				}
			}
		}
		if (OverwatchTraitButton != null && traitsDataList[0] != null)
		{
			OverwatchTraitButton.initWithTrait(equipmentModel, traitsDataList[0], currentPlayerLevel);
		}
		UpdateBT(equipmentModel, currentPlayerLevel, showThisLevelUnlocks);
	}

	public void fillTraitsAsUnknown(int rarityLevel)
	{
		base.gameObject.SetActive(value: true);
		for (int i = 0; i < traitsButtons.Length; i++)
		{
			if (traitsButtons[i] != null)
			{
				traitsButtons[i].setUnkownIcon(rarityLevel);
			}
		}
		UpdateBT(null);
	}

	private void UpdateBT(EquipmentItemModel equipmentModel, int currentPlayerLevel = -1, bool showThisLevelUnlocks = false)
	{
		Helpers.GameObjectSetActive(BreakThroughTraitsButton, value: false);
		Helpers.GameObjectSetActive(BTipsLabel, value: false);
		if (equipmentModel != null && equipmentModel.RarityLevel >= GameManager.Instance.gameEconomyData.ConfigData.EquipmentBreakthroughsRarity)
		{
			UpgradeTraitsData breakThroughUpgradeTraitsData = equipmentModel.GetBreakThroughUpgradeTraitsData();
			if (breakThroughUpgradeTraitsData != null)
			{
				Helpers.GameObjectSetActive(BreakThroughTraitsButton, value: true);
				BreakThroughTraitsButton.initWithTrait(equipmentModel, breakThroughUpgradeTraitsData, currentPlayerLevel, showThisLevelUnlocks);
			}
			else if (BTipsLabel != null)
			{
				int breakThroughUpgradeNeedBTlevel = equipmentModel.GetBreakThroughUpgradeNeedBTlevel();
				BTipsLabel.text = LocalizationManager.GetText("Popup.EquipmentLevelUp.Traits{Parameter}", breakThroughUpgradeNeedBTlevel);
				Helpers.GameObjectSetActive(BTipsLabel, value: true);
			}
		}
	}

	#region mycode
	public void SetStateImmediate(bool isLocked)
	{
		for (int i = 0; i < traitsButtons.Length; i++)
		{
			if (traitsButtons[i] != null && traitsButtons[i].gameObject.activeSelf)
			{
				traitsButtons[i].SetState(isLocked);
			}
		}
	}
	#endregion
}
