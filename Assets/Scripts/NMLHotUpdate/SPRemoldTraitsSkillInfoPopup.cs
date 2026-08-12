using TWDModel;
using UnityEngine;

public class SPRemoldTraitsSkillInfoPopup : HUDElement
{
	[SerializeField]
	private UITexture apocalypticIcon;

	[SerializeField]
	private UILabel apocalypticDesc;

	[SerializeField]
	private UILabel apocalypticName;

	[SerializeField]
	private UISprite chargeIcon;

	[SerializeField]
	private UILabel chargeName;

	[SerializeField]
	private UILabel chargeDesc;

	public void InitData(string equipmentDefinitionIdentifier)
	{
		TraitDefinition apocalypticTraitDefinitionByEquipmentDefinitionId = Helpers.GetApocalypticTraitDefinitionByEquipmentDefinitionId(equipmentDefinitionIdentifier);
		if (apocalypticTraitDefinitionByEquipmentDefinitionId == null)
		{
			return;
		}
		Object obj = UnityUtils.LoadFromAssetBundle(Helpers.GetApocalypticIconNameByTraitIdentifier(apocalypticTraitDefinitionByEquipmentDefinitionId.Identifier), "itemgraphics");
		if (obj != null)
		{
			apocalypticIcon.mainTexture = (Texture)obj;
		}
		apocalypticName.text = HelpersLocalization.GetTraitName(apocalypticTraitDefinitionByEquipmentDefinitionId);
		apocalypticDesc.text = HelpersLocalization.GetTraitDescription(apocalypticTraitDefinitionByEquipmentDefinitionId);
		EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipmentDefinitionIdentifier);
		if (equipmentDefinition != null)
		{
			EquipmentDefinition equipmentDefinition2 = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipmentDefinition.ChargeEquipmentIdentifier);
			if (equipmentDefinition2 != null)
			{
				chargeIcon.spriteName = HelpersGfx.GetEquipmentResourceEntry(equipmentDefinition2).IconSprite;
				chargeName.text = LocalizationManager.GetText("Equipment.ChargeLabel." + equipmentDefinition2.ID);
				string text = LocalizationManager.GetText("Traits." + equipmentDefinition2.ID + ".Description");
				text = text.Substring(text.IndexOf(":") + 1);
				chargeDesc.text = text;
			}
		}
	}
}
