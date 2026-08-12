using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ItemListItemButton : MonoBehaviour
{
	[SerializeField]
	private GameObject normalBG;

	[SerializeField]
	private UISprite itemSprite;

	[SerializeField]
	private UILabel Label;

	[SerializeField]
	private UILabel Num;

	[SerializeField]
	private GameObject foldGO;

	[SerializeField]
	private GameObject selectGO;

	private ItemDefinition itemDefinition;

	public void Setup(ItemDefinition definition)
	{
		itemDefinition = definition;
		UpdateUI();
	}

	private void UpdateUI()
	{
		Helpers.GameObjectSetActive(normalBG, value: false);
		Helpers.GameObjectSetActive(Num, value: false);
		Helpers.GameObjectSetActive(itemSprite, value: false);
		Helpers.GameObjectSetActive(foldGO, value: false);
		Helpers.GameObjectSetActive(selectGO, value: false);
		if (itemDefinition != null)
		{
			if (itemDefinition.IsSubType)
			{
				Helpers.GameObjectSetActive(foldGO, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(normalBG, value: true);
				Helpers.GameObjectSetActive(itemSprite, value: true);
				Helpers.GameObjectSetActive(Num, value: true);
				HelpersUI.SetTraitsIconOnSprite(itemSprite, itemDefinition.ImageIcon, itemDefinition.ImageIconOnCloud);
				long itemNumByName = GameManager.Instance.modelManager.Player.GetItemNumByName(itemDefinition.ItemName);
				Num.text = Helpers.FormatNumber(itemNumByName);
			}
			Label.text = LocalizationManager.GetText(itemDefinition.NameLocaliztion);
		}
	}

	public void OnButtonClick()
	{
		UIEvent.Send("ItemListPopupItemClickEvent", itemDefinition);
	}

	public void SetSelectState(bool select)
	{
		if (select && itemDefinition != null && !itemDefinition.IsSubType)
		{
			Helpers.GameObjectSetActive(selectGO, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(selectGO, value: false);
		}
	}

	public void FreshSelectData(ItemDefinition selectData, List<string> foldedSubTypes)
	{
		if (selectData != null)
		{
			SetSelectState(itemDefinition == selectData);
			if (itemDefinition.IsSubType && foldedSubTypes.Contains(itemDefinition.Type))
			{
				foldGO.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			else
			{
				foldGO.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			}
		}
	}
}
