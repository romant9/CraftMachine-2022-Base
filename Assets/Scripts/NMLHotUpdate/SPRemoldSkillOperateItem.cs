using TWDModel;
using UnityEngine;

public class SPRemoldSkillOperateItem : MonoBehaviour
{
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
	private GameObject selectedGo;

	[SerializeField]
	private GameObject excludeGo;

	[SerializeField]
	private GameObject usedGo;

	private ModSkillMode modSkillMode;

	private EquipmentItemModel equipmentItemModel;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
		UpdateUI();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SPRemoldEquipModSkill":
		case "SPRemoldUnEquipModSkill":
			UpdateUI();
			break;
		case "SPRemoldOperatePreviewItemClick":
			if ((string)parameter == modSkillMode?.ID)
			{
				Helpers.GameObjectSetActive(selectedGo, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(selectedGo, value: false);
			}
			break;
		}
	}

	public void Setup(ModSkillMode modSkillMode, EquipmentItemModel equipmentItemModel)
	{
		this.modSkillMode = modSkillMode;
		this.equipmentItemModel = equipmentItemModel;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (modSkillMode != null)
		{
			SPTraitsRemoldDefinitions spTraitsDefaultTrait = modSkillMode.GetSpTraitsDefaultTrait();
			skillBg.color = Helpers.HexToColor(spTraitsDefaultTrait.Color);
			traitName.text = LocalizationManager.GetText(spTraitsDefaultTrait.SPTraitsName);
			HelpersUI.SetTraitsIconOnSprite(traitIcon, spTraitsDefaultTrait.SPTraitsIcon, spTraitsDefaultTrait.SPTraitsIconOnCloud);
			starList.Setup(spTraitsDefaultTrait.Star);
			level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", spTraitsDefaultTrait.Level);
			ModSkillManager modSkillManager = GameManager.Instance.playerModel.ModSkillManager;
			Helpers.GameObjectSetActive(excludeGo, modSkillManager.IsModSkillExcludedByEquipped(modSkillMode, equipmentItemModel));
			Helpers.GameObjectSetActive(selectedGo, value: false);
			UpdateWeaponUI();
		}
	}

	private void UpdateWeaponUI()
	{
		Helpers.GameObjectSetActive(weaponIcon, value: false);
		Helpers.GameObjectSetActive(armorIcon, value: false);
		Helpers.GameObjectSetActive(usedGo, value: false);
		if (modSkillMode == null || modSkillMode.EquipmentItemModel == null)
		{
			return;
		}
		Helpers.GameObjectSetActive(usedGo, modSkillMode.ModSkillState == ModSkillState.Equipped);
		EquipmentDefinition definition = modSkillMode.EquipmentItemModel.Definition;
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

	public void OnclickOperatePreview()
	{
		if (modSkillMode != null)
		{
			UIEvent.Send("SPRemoldOperatePreviewItemClick", modSkillMode.ID);
		}
	}
}
