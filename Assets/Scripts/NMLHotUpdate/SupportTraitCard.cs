using TWDModel;
using UnityEngine;

public class SupportTraitCard : UIListCard<SupportTalentDefinition>
{
	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel positionLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UIButton traitButton;

	[SerializeField]
	private UILabel traitOccupationLabel;

	private int _slotIndex;

	private SupportModel _supportModel;

	public void SetContent(SupportModel supportModel, int slotIndex)
	{
		_supportModel = supportModel;
		_slotIndex = slotIndex;
		foreach (int value in supportModel.SlotAssembledTalentIds.Values)
		{
			if (value == base.Item.Id)
			{
				SetUI(isSelected: true);
				return;
			}
		}
		SetUI(isSelected: false);
	}

	public void OnSlotClick()
	{
		if (Helpers.ExecuteCommand(new AssembleTalentTraitToSupportCommand(_supportModel.ModelId, base.Item.Id, _slotIndex)) == TWDModelResult.OK)
		{
			UIEvent.Send("SupportDetailSelectedEvent");
		}
	}

	private void SetUI(bool isSelected)
	{
		if (isSelected)
		{
			Helpers.GameObjectSetActive(traitButton.gameObject, value: false);
			Helpers.GameObjectSetActive(traitOccupationLabel.gameObject, value: true);
			background.color = new Color(0.3058824f, 0.1490196f, 0.1411765f);
		}
		else
		{
			Helpers.GameObjectSetActive(traitButton.gameObject, value: true);
			Helpers.GameObjectSetActive(traitOccupationLabel.gameObject, value: false);
			background.color = new Color(0.1490196f, 0.145098f, 0.1411765f);
		}
		SupportTalentTreeBranchDefinition supportTalentTreeBranchDefinitionByBranchId = GameManager.Instance.gameEconomyData.GetSupportTalentTreeBranchDefinitionByBranchId(base.Item.SupportTalentId);
		HelpersUI.SetContentToLabel(nameLabel, LocalizationManager.GetText(supportTalentTreeBranchDefinitionByBranchId.TalentName));
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(base.Item.TalentTrait);
		UILabel label = descriptionLabel;
		string talentTraitDesc = base.Item.TalentTraitDesc;
		object[] arguments = traitDefinition.ConstructionParameters.ToArray();
		HelpersUI.SetContentToLabel(label, LocalizationManager.GetText(talentTraitDesc, arguments));
		traitIcon.spriteName = supportTalentTreeBranchDefinitionByBranchId.Icon;
	}
}
