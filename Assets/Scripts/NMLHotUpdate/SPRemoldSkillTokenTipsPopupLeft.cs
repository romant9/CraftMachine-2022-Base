using TWDModel;
using UnityEngine;

public class SPRemoldSkillTokenTipsPopupLeft : MonoBehaviour
{
	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UITableList starList;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UILabel traitDesc;

	private SPTraitsRemoldDefinitions definition;

	public void Setup(SPTraitsRemoldDefinitions definition)
	{
		this.definition = definition;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (definition != null)
		{
			traitName.text = LocalizationManager.GetText(definition.SPTraitsName);
			HelpersUI.SetTraitsIconOnSprite(traitIcon, definition.SPTraitsIcon, definition.SPTraitsIconOnCloud);
			classIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(definition.AvailableClass);
			starList.Setup(definition.Star);
			level.text = LocalizationManager.GetText("System.EquipSPRemold.TraitLv", definition.Level);
			if (traitDesc != null)
			{
				UILabel uILabel = traitDesc;
				string sPTraitsDesc = definition.SPTraitsDesc;
				object[] arguments = definition.SPTraitsLcValue.ToArray();
				uILabel.text = LocalizationManager.GetText(sPTraitsDesc, arguments);
			}
		}
	}
}
