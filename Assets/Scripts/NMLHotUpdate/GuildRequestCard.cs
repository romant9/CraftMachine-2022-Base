using TWDModel;
using UnityEngine;

public class GuildRequestCard : UIListCard<GuildMemberInfo>
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel leveLabel;

	private RefuseRequestMembershipAction refuseRequestMembershipAction;

	private AcceptRequestMembershipAction acceptRequestMembershipAction;

	public override void UpdateUI()
	{
		base.UpdateUI();
		nameLabel.text = GameManager.Instance.GetFilteredText(base.Item.Name);
		leveLabel.text = LocalizationManager.GetText("Generic.Level{Level}", base.Item.PlayerLevel);
	}

	public void OnDisable()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
	}

	public void OnInfo()
	{
		GuildPlayerInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildPlayerInfoPopup) as GuildPlayerInfoPopup;
		obj.GuildMemberInfo = base.Item;
		obj.Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnRefuseRequestMembership()
	{
		refuseRequestMembershipAction = new RefuseRequestMembershipAction(base.Item.MemberId);
	}

	public void OnAcceptRequestMembership()
	{
		acceptRequestMembershipAction = new AcceptRequestMembershipAction(base.Item.MemberId);
	}
}
