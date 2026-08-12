using TWDModel;
using UnityEngine;

public class SupportTalentTreeCard : UIListCard<SupportTalentTreeMainDefinition>
{
	[SerializeField]
	private UISprite talentSelectBg;

	[SerializeField]
	private UISprite talentBg;

	[SerializeField]
	private UILabel talentTreeLabel;

	[SerializeField]
	private UIButton talentTreeButton;

	public override void UpdateUI()
	{
		HelpersUI.SetContentToLabel(talentTreeLabel, LocalizationManager.GetText(base.Item.SupportTalentTreeName));
	}

	public void OnClick()
	{
		UIEvent.Send("SupportTalentSelectedEvent", base.Item);
	}

	public void SetSelected(bool selected)
	{
		Helpers.GameObjectSetActive(talentBg, !selected);
		Helpers.GameObjectSetActive(talentSelectBg, selected);
	}
}
