using TWDModel;
using UnityEngine;

public class SPRemoldSkillLeft : MonoBehaviour
{
	[SerializeField]
	private UILabel rarityLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel statLabel;

	[SerializeField]
	private UISprite normalBg;

	[SerializeField]
	private UISprite chargeBg;

	[SerializeField]
	private UILabel atkRange;

	[SerializeField]
	private UISprite dmgRangeIcon;

	[SerializeField]
	private UITexture apocalypticIcon;

	[SerializeField]
	private UISprite chargeIcon;

	private EquipmentDefinition equipmentDefinition;

	private int rarityLevel;

	private int level;

	private bool isNormal = true;

	private void Awake()
	{
		UIButton component = normalBg.GetComponent<UIButton>();
		if (component != null)
		{
			component.normalSprite = "";
		}
		UIButton component2 = chargeBg.GetComponent<UIButton>();
		if (component2 != null)
		{
			component2.normalSprite = "";
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "BreakThroughed":
		case "EquipmentRemodelSelectioned":
		case "SPRemoldLockChanged":
		case "SPRemoldRandomChanged":
		case "SPRemoldUpgradeChanged":
		case "EquipmentInstantUpgraded":
			if (parameter is EquipmentItemModel equipmentItemModel)
			{
				Setup(equipmentItemModel);
				UpdateUI();
			}
			break;
		}
	}

	public void Setup(EquipmentDefinition equipmentDefinition, int rarityLevel, int level)
	{
		this.equipmentDefinition = equipmentDefinition;
		this.rarityLevel = rarityLevel;
		this.level = level;
		UpdateUI();
	}

	public void Setup(EquipmentItemModel equipmentItemModel)
	{
		Setup(equipmentItemModel.Definition, equipmentItemModel.RarityLevel, equipmentItemModel.Level);
		string text = ((equipmentItemModel.EquipmentUpgradeTokenLevelUpgrades > 0) ? (" + " + equipmentItemModel.EquipmentUpgradeTokenLevelUpgrades) : "");
		levelLabel.text = LocalizationManager.GetText("Generic.Level{Level}", equipmentItemModel.Level - equipmentItemModel.EquipmentUpgradeTokenLevelUpgrades + text);
		int mainStat = equipmentItemModel.MainStat;
		statLabel.text = LocalizationManager.GetText("System.EquipInfo.Value") + mainStat;
	}

	public void UpdateUI()
	{
		rarityLabel.text = LocalizationManager.GetText("System.EquipInfo.Rarity") + HelpersLocalization.GetRarityLevel(rarityLevel);
		levelLabel.text = LocalizationManager.GetText("Generic.Level{Level}", level);
		AbilityDefinition abilityDefinition = GameManager.Instance.gameEconomyData.GetAbilityDefinition(this.equipmentDefinition.AbilityIdentifier);
		EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(this.equipmentDefinition.ChargeEquipmentIdentifier);
		if (equipmentDefinition == null)
		{
			return;
		}
		AbilityDefinition abilityDefinition2 = GameManager.Instance.gameEconomyData.GetAbilityDefinition(equipmentDefinition.AbilityIdentifier);
		if (isNormal)
		{
			normalBg.spriteName = "Ui_Common_Tab_Yellow";
			chargeBg.spriteName = "Ui_Common_Tab_Grey";
			UILabel uILabel = atkRange;
			string text = LocalizationManager.GetText("BasicInfo.Ability.AttackRange.Name");
			FixedPoint abilityRange = abilityDefinition.AbilityRange;
			uILabel.text = text + abilityRange.ToString();
			dmgRangeIcon.spriteName = abilityDefinition.DMGRangeDisplayImage;
		}
		else
		{
			normalBg.spriteName = "Ui_Common_Tab_Grey";
			chargeBg.spriteName = "Ui_Common_Tab_Yellow";
			UILabel uILabel2 = atkRange;
			string text2 = LocalizationManager.GetText("BasicInfo.Ability.AttackRange.Name");
			FixedPoint abilityRange = abilityDefinition2.AbilityRange;
			uILabel2.text = text2 + abilityRange.ToString();
			dmgRangeIcon.spriteName = abilityDefinition2.DMGRangeDisplayImage;
		}
		if (dmgRangeIcon.spriteName == "")
		{
			Helpers.GameObjectSetActive(dmgRangeIcon.gameObject, false);
		}
		TraitDefinition apocalypticTraitDefinitionByEquipmentDefinitionId = Helpers.GetApocalypticTraitDefinitionByEquipmentDefinitionId(this.equipmentDefinition.ID);
		if (apocalypticTraitDefinitionByEquipmentDefinitionId != null)
		{
			Object obj = UnityUtils.LoadFromAssetBundle(Helpers.GetApocalypticIconNameByTraitIdentifier(apocalypticTraitDefinitionByEquipmentDefinitionId.Identifier), "itemgraphics");
			if (obj != null)
			{
				apocalypticIcon.mainTexture = (Texture)obj;
			}
			chargeIcon.spriteName = HelpersGfx.GetEquipmentResourceEntry(equipmentDefinition).IconSprite;
		}
	}

	public void OnclickNormal()
	{
		isNormal = true;
		UpdateUI();
	}

	public void OnclickCharge()
	{
		isNormal = false;
		UpdateUI();
	}
}
