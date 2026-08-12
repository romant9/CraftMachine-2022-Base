using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ComponentShopListItem : ShopCardBase<ComponentCrateItem>
{
	[SerializeField]
	private UILabel itemNameLabel;

	[SerializeField]
	private UISprite itemSprite;

	[SerializeField]
	private UISprite rarityBackgroundSprite;

	[SerializeField]
	private GameObject randomIcon;

	[SerializeField]
	private UIButton infoButton;

	public override void SetData(ComponentCrateItem data)
	{
		base.SetData(data);
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GetData() == null)
		{
			return;
		}
		bool flag = GetData().IsFixedRarity();
		bool flag2 = GetData().IsFixedType();
		bool flag3 = false;
		bool value = false;
		if (flag2 && flag)
		{
			CurrencyType componentCurrencyType = GameManager.Instance.playerModel.GetComponentCurrencyType(GetData().Type, GetData().Rarity);
			itemSprite.spriteName = HelpersGfx.GetCurrencyIconName(componentCurrencyType);
		}
		else if (!flag2 && flag)
		{
			flag3 = true;
			value = true;
			if (rarityBackgroundSprite != null)
			{
				rarityBackgroundSprite.spriteName = HelpersGfx.GetEquipmentRaritySprite(GetData().Rarity);
			}
		}
		else if (flag2)
		{
			itemSprite.spriteName = HelpersGfx.GetRandomComponentIconName(GetData().Type);
		}
		else if (!flag2 && !flag)
		{
			flag3 = true;
		}
		Helpers.GameObjectSetActive(randomIcon, flag3);
		Helpers.GameObjectSetActive(rarityBackgroundSprite, value);
		Helpers.GameObjectSetActive(itemSprite, !flag3);
		HelpersUI.SetContentToLabel(itemNameLabel, GetDescription());
	}

	private string GetDescription()
	{
		bool num = GetData().IsFixedRarity();
		bool flag = GetData().IsFixedType();
		string textId = "ComponentShop.Item{Amount}{Parameter1}{Parameter2}";
		if (!flag)
		{
			textId = "ComponentShop.RandomItem{Amount}{Parameter1}{Parameter2}";
		}
		string text = "";
		string text2 = "";
		if (num)
		{
			text = HelpersLocalization.GetRarityLevel(GetData().Rarity);
		}
		text2 = ((!flag) ? LocalizationManager.GetText("Component.Name" + ((GetData().Count == 1) ? "" : ".Plural")) : (LocalizationManager.GetText("Component." + GetData().Type + ".Name" + ((GetData().Count == 1) ? "" : ".Plural")) + " "));
		if (!num && !flag)
		{
			return GetData().Count + " " + LocalizationManager.GetText((GetData().Count == 1) ? "ComponentShop.RandomComponent.Title" : "ComponentShop.RandomComponent.Title.Plural");
		}
		return LocalizationManager.GetText(textId, GetData().Count, text, text2);
	}

	public void OnInfoClicked()
	{
		ComponentCrateItem data = GetData();
		if (data != null && GameManager.Instance.gameEconomyData != null)
		{
			int scavengerLevel = GetScavengerLevel();
			List<ItemAmountProbabilityData> probabilities = GameManager.Instance.gameEconomyData.GetComponentProbabilities(scavengerLevel, DropEventDefinition.DropEventTag.ComponentCrate, GameManager.Instance.playerModel.ActivityManager, data.Type, data.Rarity);
			DropRatesNamesHelper.GetNameForComponents(ref probabilities);
			DropRatesInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup;
			string description = GetDescription();
			DropTableItem dropTableItem = new DropTableItem
			{
				DropName = description,
				Description = LocalizationManager.GetText("Popup.DropRateInformation.ComponentCrate.Description"),
				Probabilities = probabilities
			};
			obj.TryOpenWithNormalData(dropTableItem);
		}
	}

	private int GetScavengerLevel()
	{
		int result = 0;
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Camp != null)
		{
			result = GameManager.Instance.playerModel.Camp.GetBuildingLevel("Scavenger");
		}
		return result;
	}

	public override void Clear()
	{
		base.Clear();
	}

	protected override void OnClickedTooltipButton(UIButtonExtended button)
	{
		base.OnClickedTooltipButton(button);
		TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForComponentCrateItem(GetData()));
	}
}
