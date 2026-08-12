using System.Collections;
using System.Collections.Generic;
using TWDModel;
using TWDModel.SqEquipmentRemold;
using UnityEngine;

public class SPRemoldMainListItem : MonoBehaviour
{
	[SerializeField]
	private UISprite bg;

	[SerializeField]
	private UISprite lockState;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private GameObject EntryContainer;

	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private GameObject EmptyPrefab;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UILabel traitDesc;

	[SerializeField]
	private GameObject upgradeEffect;

	[SerializeField]
	private GameObject remoldEffect;

	[SerializeField]
	private GameObject newLabel;

	private bool isMaxLevelPreview;

	private SPTraitSlot spTraitSlot;

	private EquipmentItemModel equipmentItemModel;

	private readonly List<GameObject> Entries = new List<GameObject>();

	private SPTraitsRemoldDefinitions definition
	{
		get
		{
			if (spTraitSlot == null)
			{
				return null;
			}
			if (isMaxLevelPreview)
			{
				return Helpers.GetMaxLevelSPTraitsRemodeDefinition(spTraitSlot.ID);
			}
			return GameManager.Instance.gameEconomyData.GetSPTraitsRemodeDefinition(spTraitSlot.ID);
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		StopAllCoroutines();
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (isMaxLevelPreview)
		{
			return;
		}
		if (!(type == "SPRemoldRandomChangedEffect"))
		{
			if (type == "SPRemoldUpgradeChangedEffect")
			{
				object[] obj = (object[])parameter;
				string text = (string)obj[0];
				string text2 = (string)obj[1];
				if (spTraitSlot != null && equipmentItemModel != null && (spTraitSlot.ID == text || spTraitSlot.ID == text2))
				{
					ShowUpgradeEffect(show: true);
				}
				else
				{
					ShowUpgradeEffect(show: false);
				}
			}
		}
		else if (spTraitSlot != null && equipmentItemModel != null && spTraitSlot.LockState == SPTraitsLockState.Unlocked)
		{
			ShowRemoldEffect(show: true);
		}
		else
		{
			ShowRemoldEffect(show: false);
		}
	}

	public void Setup(EquipmentItemModel equipmentItemModel, SPTraitSlot dataEntry)
	{
		this.equipmentItemModel = equipmentItemModel;
		spTraitSlot = dataEntry;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (spTraitSlot != null && definition != null)
		{
			bg.color = Helpers.HexToColor(definition.Color);
			Helpers.GameObjectSetActive(lockState, value: true);
			lockState.spriteName = spTraitSlot.GetLockIcon();
			if (isMaxLevelPreview)
			{
				Helpers.GameObjectSetActive(lockState, value: false);
			}
			traitName.text = LocalizationManager.GetText(definition.SPTraitsName);
			HelpersUI.SetTraitsIconOnSprite(traitIcon, definition.SPTraitsIcon, definition.SPTraitsIconOnCloud);
			FreshListData();
			if (spTraitSlot.IsMaxLevel() || isMaxLevelPreview)
			{
				level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLvMax");
			}
			else
			{
				level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", spTraitSlot.Level);
			}
			UILabel uILabel = traitDesc;
			string sPTraitsDesc = definition.SPTraitsDesc;
			object[] arguments = definition.SPTraitsLcValue.ToArray();
			uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
			Helpers.GameObjectSetActive(newLabel, value: false);
		}
	}

	private void FreshListData()
	{
		ClearEntries();
		UITable component = EntryContainer.GetComponent<UITable>();
		int star = definition.Star;
		for (int i = 0; i < star; i++)
		{
			GameObject gameObject = EntryContainer.AddChild(EntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			Entries.Add(gameObject);
		}
		component.Reposition();
	}

	private void ClearEntries()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			NGUITools.Destroy(Entries[i]);
		}
		Entries.Clear();
	}

	public void OnclickLock()
	{
		if (isMaxLevelPreview)
		{
			return;
		}
		switch (spTraitSlot.LockState)
		{
		case SPTraitsLockState.Unlocked:
			if (Helpers.ExecuteCommand(new SpEquipmentRemoldToggleTraitsLockCommand(spTraitSlot.ID, equipmentItemModel.ModelId)) == TWDModelResult.OK)
			{
				UIEvent.Send("SPRemoldLockChanged");
			}
			break;
		case SPTraitsLockState.Locked:
			if (Helpers.ExecuteCommand(new SpEquipmentRemoldToggleTraitsUnLockCommand(spTraitSlot.ID, equipmentItemModel.ModelId)) == TWDModelResult.OK)
			{
				UIEvent.Send("SPRemoldLockChanged");
			}
			break;
		case SPTraitsLockState.ForceLocked:
		{
			SPRemoldCommonNoticePopup sPRemoldCommonNoticePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldCommonNoticePopup) as SPRemoldCommonNoticePopup;
			if (sPRemoldCommonNoticePopup != null)
			{
				sPRemoldCommonNoticePopup.SetContent(LocalizationManager.GetText("System.EquipSPRemold.FuncInfo18"), LocalizationManager.GetText("System.EquipSPRemold.FuncInfo26"));
				sPRemoldCommonNoticePopup.Open();
			}
			break;
		}
		}
	}

	public void OnclickDetail()
	{
		if (!isMaxLevelPreview)
		{
			SPRemoldTraitsInfoPopup sPRemoldTraitsInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SPRemoldTraitsInfoPopup) as SPRemoldTraitsInfoPopup;
			if (sPRemoldTraitsInfoPopup != null)
			{
				sPRemoldTraitsInfoPopup.Initialized(equipmentItemModel, spTraitSlot);
				sPRemoldTraitsInfoPopup.Open();
			}
		}
	}

	public void ShowUpgradeEffect(bool show)
	{
		Helpers.GameObjectSetActive(upgradeEffect, value: false);
		if (show)
		{
			StartCoroutine(EffectUpgradeShow());
		}
	}

	private IEnumerator EffectUpgradeShow()
	{
		yield return new WaitForSeconds(0.1f);
		Helpers.GameObjectSetActive(remoldEffect, value: true);
	}

	public void ShowRemoldEffect(bool show)
	{
		Helpers.GameObjectSetActive(newLabel, show);
		Helpers.GameObjectSetActive(remoldEffect, value: false);
		if (show)
		{
			StartCoroutine(EffectRemoldShow());
		}
	}

	private IEnumerator EffectRemoldShow()
	{
		yield return new WaitForSeconds(0.1f);
		Helpers.GameObjectSetActive(remoldEffect, value: true);
	}

	public void SetupMaxLevelPreview(bool isMaxLevel)
	{
		isMaxLevelPreview = isMaxLevel;
		UpdateUI();
	}
}
