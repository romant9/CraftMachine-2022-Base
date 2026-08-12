using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class ItemListPopup : HUDElement
{
	[SerializeField]
	private ItemListTypeList ItemListTypeList;

	[SerializeField]
	private ItemListItemList ItemListItemList;

	[SerializeField]
	private UISprite detailIcon;

	[SerializeField]
	private UISprite detailSkillTokenIcon;

	[SerializeField]
	private UISprite detailSkillTokenIconBg;

	[SerializeField]
	private UILabel detailName;

	[SerializeField]
	private UILabel detailDec;

	[SerializeField]
	private UILabel detailAcquisition;

	[SerializeField]
	private UILabel detailAmount;

	[SerializeField]
	private GameObject detailContentGO;

	[SerializeField]
	private GameObject anchor;

	private List<string> foldedSubTypes = new List<string>();

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		ReloadAnchorPosition();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (!(type == "ItemListPopupTabClickEvent"))
		{
			if (type == "ItemListPopupItemClickEvent" && parameter is ItemDefinition itemDefinition)
			{
				OnClickItem(itemDefinition);
			}
		}
		else if (parameter is TypeDefinition typeDefinition)
		{
			OnClickTab(typeDefinition);
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Open()
	{
		base.Open();
		Helpers.SetItemListOpened(on: true);
		foldedSubTypes.Clear();
		TypeDefinition typeDefinition = Helpers.GetTypeDefinition(Helpers.GetDefaultItemDefinition());
		ItemListTypeList.InitData(Helpers.GetItemTypes());
		UIEvent.Send("ItemListPopupTabClickEvent", typeDefinition);
	}

	public override void Close()
	{
		base.Close();
	}

	private void UpdateDetailUI(ItemDefinition itemDefinition)
	{
		Helpers.GameObjectSetActive(detailSkillTokenIcon, value: false);
		Helpers.GameObjectSetActive(detailSkillTokenIconBg, value: false);
		Helpers.GameObjectSetActive(detailIcon, value: false);
		_ = GameManager.Instance.playerModel;
		SPTraitsSkillKitTokenSet sPTraitsSkillKitTokenSetByID = GameManager.Instance.playerModel.gameEconomyData.GetSPTraitsSkillKitTokenSetByID(itemDefinition.ItemName);
		if (sPTraitsSkillKitTokenSetByID != null)
		{
			HelpersUI.SetTraitsIconOnSprite(detailSkillTokenIcon, sPTraitsSkillKitTokenSetByID.TopIcon, sPTraitsSkillKitTokenSetByID.TopIconOnCloud);
			detailSkillTokenIconBg.spriteName = sPTraitsSkillKitTokenSetByID.BGIcon;
			Helpers.GameObjectSetActive(detailSkillTokenIcon, value: true);
			Helpers.GameObjectSetActive(detailSkillTokenIconBg, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(detailIcon, value: true);
			HelpersUI.SetTraitsIconOnSprite(detailIcon, itemDefinition.ImageIcon, itemDefinition.ImageIconOnCloud);
		}
		detailName.text = LocalizationManager.GetText(itemDefinition.NameLocaliztion);
		detailDec.text = LocalizationManager.GetText(itemDefinition.DetailDescription);
		detailAcquisition.text = "";
		foreach (string item in itemDefinition.AcquisitionLocalization)
		{
			UILabel uILabel = detailAcquisition;
			uILabel.text = uILabel.text + LocalizationManager.GetText(item) + "\n";
		}
		string text = Helpers.FormatNumber(GameManager.Instance.modelManager.Player.GetItemNumByName(itemDefinition.ItemName));
		detailAmount.text = LocalizationManager.GetText("Currency.OwnedAmount{OwnedAmount}", text);
	}

	private void OnClickTab(TypeDefinition typeDefinition)
	{
		ItemListTypeList.FreshSelectData(typeDefinition);
		ItemListItemList.InitData(typeDefinition.ItemDefinitions);
		UIEvent.Send("ItemListPopupItemClickEvent", typeDefinition.ItemDefinitions.First((ItemDefinition t) => !t.IsSubType));
	}

	private void OnClickItem(ItemDefinition itemDefinition)
	{
		if (itemDefinition == null)
		{
			return;
		}
		if (itemDefinition.IsSubType)
		{
			if (foldedSubTypes.Contains(itemDefinition.Type))
			{
				foldedSubTypes.Remove(itemDefinition.Type);
			}
			else
			{
				foldedSubTypes.Add(itemDefinition.Type);
			}
			TypeDefinition typeDefinition = Helpers.GetTypeDefinition(itemDefinition);
			List<ItemDefinition> filterItemsList = GetFilterItemsList(typeDefinition);
			ItemListItemList.InitData(filterItemsList);
		}
		ItemListItemList.FreshSelectData(itemDefinition, foldedSubTypes);
		UpdateDetailUI(itemDefinition);
		if (itemDefinition.IsSubType)
		{
			Helpers.GameObjectSetActive(detailContentGO, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(detailContentGO, value: true);
		}
	}

	private List<ItemDefinition> GetFilterItemsList(TypeDefinition typeDefinition)
	{
		List<ItemDefinition> list = new List<ItemDefinition>(typeDefinition.ItemDefinitions);
		list.RemoveAll((ItemDefinition t) => foldedSubTypes.Contains(t.Type) && !t.IsSubType);
		return list;
	}

	private void ReloadAnchorPosition()
	{
		Vector3 position = Vector3.zero;
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			position = campHUD.GetDiamondsMeterV();
		}
		anchor.transform.position = position;
	}
}
