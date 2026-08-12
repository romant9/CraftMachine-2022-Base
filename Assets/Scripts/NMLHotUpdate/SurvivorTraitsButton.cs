using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorTraitsButton : MonoBehaviour
{
	public enum AnimState
	{
		Locked = 0,
		LockedToUnlocked = 1,
		Unlocked = 2,
		None = 3
	}

	public static string UnknownTraitSprite = "Ui_Icon_Trait_Unknown_";

	public AnimState currectState = AnimState.None;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UILabel unlockedLevelLabel;

	[Header("Locked icon")]
	[SerializeField]
	private UISprite lockedIcon;

	private string tooltipText = "";

	private Animator animator;

	private EquipmentItemModel equipmentItemModel;

	private UpgradeTraitsData traitsData;

	public void initWithTrait(EquipmentItemModel equipmentModel = null, UpgradeTraitsData upgradeTraitsData = null, int level = -1, bool showThisLevelUnlocks = false)
	{
		equipmentItemModel = equipmentModel;
		traitsData = upgradeTraitsData;
		if (upgradeTraitsData != null && level > -1)
		{
			base.gameObject.SetActive(value: true);
			tooltipText = HelpersLocalization.GetInstantiatedTraitDescription(upgradeTraitsData);
			if (unlockedLevelLabel != null)
			{
				unlockedLevelLabel.gameObject.SetActive(value: true);
				unlockedLevelLabel.text = LocalizationManager.GetText("Popup.SurvivorInfoPopup.LevelTrait{Level}", upgradeTraitsData.UnlockingLevel);
			}
			if (upgradeTraitsData.UnlockingLevel > level)
			{
				stateLocked();
				showLockedIcon(value: true);
			}
			else if (showThisLevelUnlocks && upgradeTraitsData.UnlockingLevel == level)
			{
				stateLockedToUnlocked();
			}
			else
			{
				stateUnlocked();
				showLockedIcon(value: false);
			}
			if (traitIcon != null)
			{
				traitIcon.spriteName = HelpersGfx.GetEquipmentTraitIconName(upgradeTraitsData);
			}
			if (traitsData.RemodelIng && CanOpenRemodel())
			{
				DebugTWD.Log("Click TraitButton ");
				OpenRemodelPopup();
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void setUnkownIcon(int rarityLevel)
	{
		showLockedIcon(value: false);
		setParam(AnimState.Locked);
		if (traitIcon != null)
		{
			string text = "";
			switch (rarityLevel)
			{
			case 0:
				text = "Low";
				break;
			case 1:
				text = "Mid";
				break;
			case 2:
				text = "High";
				break;
			}
			traitIcon.spriteName = UnknownTraitSprite + text;
		}
		if (unlockedLevelLabel != null)
		{
			unlockedLevelLabel.text = "";
		}
	}

	private void showLockedIcon(bool value)
	{
		if (lockedIcon != null)
		{
			lockedIcon.gameObject.SetActive(value);
		}
	}

	public void OnClick()
	{
		DebugTWD.Log("Click TraitButton ");

		if (CanOpenRemodel())
		{
			OpenRemodelPopup();
		}
		else if (tooltipText != "")
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, tooltipText);
		}
	}

	[ContextMenu("Locked")]
	private void stateLocked()
	{
		IsLocked = true;
		setParam(AnimState.Locked);
	}

	[ContextMenu("LockedToUnlocked")]
	private void stateLockedToUnlocked()
	{
		setParam(AnimState.LockedToUnlocked);
	}

	[ContextMenu("Unlocked")]
	private void stateUnlocked()
	{
		IsLocked = false;
		setParam(AnimState.Unlocked);
	}

	private void setParam(AnimState state)
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
		if (animator != null)
		{
			currectState = state;
			animator.SetInteger("State", (int)state);
		}
	}

	private void OpenRemodelPopup()
	{
		RemodelPopup remodelPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RemodelPopup, HUDElement.GetParent()) as RemodelPopup;
		if (remodelPopup != null && !remodelPopup.IsOpen)
		{
			remodelPopup.OnClose += OnCloseRemodel;
			remodelPopup.InitData(equipmentItemModel, traitsData);
			remodelPopup.Open();
			if (OfflineManager.IsLoadDataManager)
			{
				EquipmentUpgradePopup equipmentUpgradePopup;
				if (equipmentItemModel.Definition != null && equipmentItemModel.Definition.SwitchRemoldMode)
				{
					equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopupNew) as EquipmentUpgradePopup;
				}
				else
				{
					equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
				}

                if (equipmentUpgradePopup != null)
				{
					if (equipmentUpgradePopupLayer != remodelPopup.gameObject.layer)
					{
						equipmentUpgradePopupLayer = equipmentUpgradePopup.gameObject.layer;
						NGUITools.SetLayer(equipmentUpgradePopup.gameObject, remodelPopup.gameObject.layer);
					}
				}
			}
		}
	}

	private void OnCloseRemodel(HUDElement element, HUDElementConfig hudElementConfig)
	{
		RemodelPopup remodelPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.RemodelPopup, HUDElement.GetParent()) as RemodelPopup;
		if (remodelPopup != null)
		{
			remodelPopup.OnClose -= OnCloseRemodel;

			if (OfflineManager.IsLoadDataManager)
			{
				EquipmentUpgradePopup equipmentUpgradePopup;
				if (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.CampEquipmentLevelUpPopupNew))
				{
					equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopupNew) as EquipmentUpgradePopup;
				}
				else
				{
					equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
				}
                if (equipmentUpgradePopup != null)
				{
					if (equipmentUpgradePopupLayer > 0 && equipmentUpgradePopupLayer != equipmentUpgradePopup.gameObject.layer)
					{
						NGUITools.SetLayer(equipmentUpgradePopup.gameObject, equipmentUpgradePopupLayer);
					}
				}
			}
		}
	}

	private bool CanOpenRemodel()
	{
		if (equipmentItemModel == null || traitsData == null)
		{
			return false;
		}
		int equipmentRemodelRarity = GameManager.Instance.gameEconomyData.ConfigData.EquipmentRemodelRarity;
		if (equipmentItemModel.RarityLevel < equipmentRemodelRarity || traitsData.UnlockingLevel > equipmentItemModel.Level)
		{
			if (!OfflineManager.IsLoadDataManager || IsLocked) return false;
		}
		else
		{
			if (OfflineManager.IsLoadDataManager && IsLocked) return false;
		}

		EquipmentUpgradePopup equipmentUpgradePopup;
		if (equipmentItemModel.Definition != null && equipmentItemModel.Definition.SwitchRemoldMode)
		{
			equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopupNew) as EquipmentUpgradePopup;
		}
		else
		{
			equipmentUpgradePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		}
		
        if (equipmentUpgradePopup != null)
		{
			DefaultPopup defaultPopup = equipmentUpgradePopup.GetDefaultPopup();
			if (defaultPopup != null && defaultPopup.isActiveAndEnabled)
			{
				return true;
			}
		}		
		return false;
	}



	#region myparams
	private int equipmentUpgradePopupLayer;
	public bool IsLocked { get; private set; }
	#endregion

	#region mycode
	public void SetState(bool isLocked)
	{
		if (isLocked)
		{
			stateLocked();
			showLockedIcon(value: true);
		}
		else
		{
			stateUnlocked();
			showLockedIcon(value: false);
		}
	}
	#endregion
}
