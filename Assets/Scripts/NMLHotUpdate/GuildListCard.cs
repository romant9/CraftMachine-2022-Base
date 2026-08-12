using TWDModel;
using UnityEngine;

public class GuildListCard : UIListCard<GuildModel>
{
	[SerializeField]
	protected UILabel nameLabel;

	[SerializeField]
	protected UILabel numberMembersLabel;

	[SerializeField]
	protected UILabel scoreLabel;

	[SerializeField]
	protected UILabel currentVpLabel;

	[SerializeField]
	private UISprite backgroundSprite;

	[SerializeField]
	private Color defaultBgColor;

	[SerializeField]
	public Color highlightBgColor;

	public override void UpdateUI()
	{
		base.UpdateUI();
		HelpersUI.SetContentToLabel(nameLabel, GameManager.Instance.GetFilteredText(base.Item.Name));
		HelpersUI.SetContentToLabel(numberMembersLabel, base.Item.NumberMembers + "/" + 20);
		HelpersUI.SetContentToLabel(scoreLabel, base.Item.TotalChallengeStars.ToString());
		HelpersUI.SetContentToLabel(currentVpLabel, base.Item.GuildInfoCurrentVP.ToString());
		bool flag = GameManager.Instance.playerModel.IsGuildMember && GameManager.Instance.playerModel.GuildId == base.Item.Id;
		HelpersUI.SetColor(backgroundSprite, flag ? highlightBgColor : defaultBgColor);
	}

	public virtual void OnClick()
	{
		GuildModelWrapper model = new GuildModelWrapper(base.Item);

		GuildInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildInfoPopup, HUDElement.GetParent()) as GuildInfoPopup;
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.Log("OnClick GuildListCard of: " + base.Item.Name, DebugType.OnClick);
			obj.transform.localScale = Vector3.one * 1.25f;
		}
		obj.GuildInfoPopupType = GuildInfoPopup.GuildPopupType.GuildSearch;
		obj.OpenForModel(model);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}
}
