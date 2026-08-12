using TWDModel;
using UnityEngine;

public class SupportTalentUpdatePopup : HUDElement
{
	[SerializeField]
	private UILabel talentNameLabel;

	[SerializeField]
	private UILabel talentCurLevelLabel;

	[SerializeField]
	private UILabel talentNextLevelLabel;

	[SerializeField]
	private UILabel talentDescLabel;

	private bool _isAnimationFinish;

	public override void Close()
	{
		if (_isAnimationFinish)
		{
			base.Close();
		}
	}

	public void SetContent(SupportTalentNodeAbstract nodeModel)
	{
		SupportTalentDefinition currentTalentNodeDefinition = nodeModel.GetCurrentTalentNodeDefinition();
		HelpersUI.SetContentToLabel(talentNameLabel, LocalizationManager.GetText(nodeModel.GetTalentName()));
		HelpersUI.SetContentToLabel(talentCurLevelLabel, (currentTalentNodeDefinition.Level - 1).ToString());
		HelpersUI.SetContentToLabel(talentNextLevelLabel, currentTalentNodeDefinition.Level.ToString());
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(currentTalentNodeDefinition.TalentTrait);
		if (currentTalentNodeDefinition.Type == SupportTalentType.Attribute)
		{
			HelpersUI.SetContentToLabel(talentDescLabel, LocalizationManager.GetText(currentTalentNodeDefinition.TalentTraitDesc, currentTalentNodeDefinition.TalentAttributeValue));
		}
		else if (currentTalentNodeDefinition.Type == SupportTalentType.Trait && traitDefinition != null)
		{
			UILabel label = talentDescLabel;
			string talentTraitDesc = currentTalentNodeDefinition.TalentTraitDesc;
			object[] arguments = traitDefinition.ConstructionParameters.ToArray();
			HelpersUI.SetContentToLabel(label, LocalizationManager.GetText(talentTraitDesc, arguments));
		}
	}

	public void IsAnimationFinish()
	{
		_isAnimationFinish = true;
	}
}
