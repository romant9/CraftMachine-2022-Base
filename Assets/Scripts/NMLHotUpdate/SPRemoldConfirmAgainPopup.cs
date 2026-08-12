using System.Linq;
using TWDModel;
using UnityEngine;

public class SPRemoldConfirmAgainPopup : HUDElement
{
	public enum InitType
	{
		Upgrade = 0,
		Remold = 1
	}

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private UISprite tokenIcon;

	[SerializeField]
	private UILabel tokenNum;

	[SerializeField]
	private GameObject selectedGO;

	[SerializeField]
	private GameObject unSelectGO;

	private InitType initType;

	private Callback callBack;

	private EquipmentItemModel equipmentItemModel;

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public void SetInitData(EquipmentItemModel equipmentItemModel, InitType initType, Callback callBack)
	{
		this.initType = initType;
		this.callBack = callBack;
		this.equipmentItemModel = equipmentItemModel;
	}

	public void OnClickConfirmButton()
	{
		if (callBack != null)
		{
			callBack();
		}
		switch (initType)
		{
		case InitType.Upgrade:
			if (Helpers.IsSPRemold24UpgradeComfirm())
			{
				Helpers.SetSPRemold24UpgradeComfirmTimePlayerPrefs();
			}
			break;
		case InitType.Remold:
			if (Helpers.IsSPRemold24Comfirm())
			{
				Helpers.SetSPRemold24ComfirmTimePlayerPrefs();
			}
			break;
		}
		Close();
	}

	public void OnClickSwitchTips()
	{
		switch (initType)
		{
		case InitType.Upgrade:
			Helpers.SetSPRemold24UpgradeComfirm(!Helpers.IsSPRemold24UpgradeComfirm());
			break;
		case InitType.Remold:
			Helpers.SetSPRemold24Comfirm(!Helpers.IsSPRemold24Comfirm());
			break;
		}
		UpdateUI();
	}

	public override void UpdateUI()
	{
		switch (initType)
		{
		case InitType.Upgrade:
		{
			titleLabel.text = LocalizationManager.GetText("System.EquipSPRemold.Button.Upgrade");
			contentLabel.text = LocalizationManager.GetText("System.EquipSPRemold.FuncInfo4");
			tokenIcon.spriteName = "UI_Icon_SPTraitsUpgradeToken";
			int value3 = GameManager.Instance.playerModel.gameEconomyData.SPTraitsRemoldConfigs.GetUpgradeCost().FirstOrDefault().Value;
			tokenNum.text = value3.ToString();
			Helpers.GameObjectSetActive(selectedGO, Helpers.IsSPRemold24UpgradeComfirm());
			Helpers.GameObjectSetActive(unSelectGO, !Helpers.IsSPRemold24UpgradeComfirm());
			break;
		}
		case InitType.Remold:
		{
			titleLabel.text = LocalizationManager.GetText("System.EquipSPRemold.Button.Reroll");
			contentLabel.text = LocalizationManager.GetText("System.EquipSPRemold.FuncInfo2");
			tokenIcon.spriteName = "UI_Icon_SPTraitsRemoldToken";
			int lockedNoForceTraitCount = equipmentItemModel.SpEquipmentRemoldModel.GetLockedNoForceTraitCount();
			int value = equipmentItemModel.SpEquipmentRemoldModel.GetRemoldBaseCost().FirstOrDefault().Value;
			int value2 = equipmentItemModel.SpEquipmentRemoldModel.CalculateRemoldLockedCost(lockedNoForceTraitCount).FirstOrDefault().Value;
			string text = "";
			if (value2 > 0)
			{
				text = "+" + value2;
			}
			tokenNum.text = value + text;
			Helpers.GameObjectSetActive(selectedGO, Helpers.IsSPRemold24Comfirm());
			Helpers.GameObjectSetActive(unSelectGO, !Helpers.IsSPRemold24Comfirm());
			break;
		}
		}
	}
}
