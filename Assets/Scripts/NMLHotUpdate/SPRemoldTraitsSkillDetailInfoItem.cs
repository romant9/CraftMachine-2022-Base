using TWDModel;
using UnityEngine;

public class SPRemoldTraitsSkillDetailInfoItem : MonoBehaviour
{
	[SerializeField]
	private GameObject NormalContent;

	[SerializeField]
	private GameObject NoneContent;

	[SerializeField]
	private UISprite skillBg;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UITexture weaponIcon;

	[SerializeField]
	private UITexture armorIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UILabel traitDesc;

	private ModSkillSlot modSkillSlot;

	public void Setup(ModSkillSlot modSkillSlot)
	{
		this.modSkillSlot = modSkillSlot;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (modSkillSlot != null)
		{
			UpdateUINormalContent();
			UpdateUINoneContent();
		}
	}

	private void UpdateUINormalContent()
	{
		Helpers.GameObjectSetActive(NormalContent, value: false);
		if (modSkillSlot != null && modSkillSlot.ModSkillMode != null)
		{
			Helpers.GameObjectSetActive(NormalContent, value: true);
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillSlot.ModSkillMode.GetSpTraitsDefaultTrait();
			skillBg.color = Helpers.HexToColor(spTraitsDefaultTrait.Color);
			traitName.text = LocalizationManager.GetText(spTraitsDefaultTrait.SPTraitsName);
			HelpersUI.SetTraitsIconOnSprite(traitIcon, spTraitsDefaultTrait.SPTraitsIcon, spTraitsDefaultTrait.SPTraitsIconOnCloud);
			starList.Setup(spTraitsDefaultTrait.Star);
			level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", spTraitsDefaultTrait.Level);
			if (traitDesc != null)
			{
				UILabel uILabel = traitDesc;
				string sPTraitsDesc = spTraitsDefaultTrait.SPTraitsDesc;
				object[] arguments = spTraitsDefaultTrait.SPTraitsLcValue.ToArray();
				uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
			}
			UpdateWeaponUI();
		}
	}

	private void UpdateUINoneContent()
	{
		Helpers.GameObjectSetActive(NoneContent, value: false);
		if (modSkillSlot != null && modSkillSlot.ModSkillMode == null)
		{
			Helpers.GameObjectSetActive(NoneContent, value: true);
		}
	}

	private void UpdateWeaponUI()
	{
		if (modSkillSlot == null || modSkillSlot.ModSkillMode == null || modSkillSlot.ModSkillMode.EquipmentItemModel == null)
		{
			return;
		}
		EquipmentDefinition definition = modSkillSlot.ModSkillMode.EquipmentItemModel.Definition;
		Helpers.GameObjectSetActive(weaponIcon, value: false);
		Helpers.GameObjectSetActive(armorIcon, value: false);
		if (definition.Category == EquipmentCategory.Armor)
		{
			Helpers.GameObjectSetActive(armorIcon, value: true);
			armorIcon.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(definition.ID);
			if (definition.UseSpecialMaterial)
			{
				Material specialMaterial = HelpersGfx.GetEquipmentResourceEntry(definition).specialMaterial;
				armorIcon.material = specialMaterial ?? armorIcon.material;
			}
		}
		else
		{
			Helpers.GameObjectSetActive(weaponIcon, value: true);
			weaponIcon.mainTexture = HelpersGfx.GetEquipmentIconTextureFromID(definition.ID);
			if (definition.UseSpecialMaterial)
			{
				Material specialMaterial2 = HelpersGfx.GetEquipmentResourceEntry(definition).specialMaterial;
				weaponIcon.material = specialMaterial2 ?? weaponIcon.material;
			}
		}
	}
}
