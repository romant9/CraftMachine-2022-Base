using TWDModel;
using UnityEngine;

public class GuildInvitePopup : HUDElement
{
	[SerializeField]
	private GameObject duplicatedButtonGo;

	[SerializeField]
	private UILabel duplicatedLabel;

	public override void Open()
	{
		base.Open();
		duplicatedButtonGo.SetActive(value: false);
	}

	public void OnGenerateInviteLink()
	{
		duplicatedButtonGo.SetActive(value: true);
		duplicatedLabel.text = "[u]" + duplicatedLabel.text;
	}

	public void OnDuplicate()
	{
		string text = "Please join my group in The Walking Dead: No Man's Land! Click here to join: ";
		string bundleURLScheme = GameConfiguration.Instance.Config.BundleURLScheme;
		string id = GameManager.Instance.guildModel.Id;
		string hashedId = GameManager.Instance.playerModel.HashedId;
		Helpers.ExecuteCommandDelayed(new SendGuildInviteMetricsCommand(SendGuildInviteMetricsCommand.EventType.InviteSent));
		string text2 = ((!(bundleURLScheme != "twdnomansland") || !(bundleURLScheme != "twdnomanslandlv")) ? $"http://www.thewalkingdeadnomansland.com/guildinvite/?g={id}&p={hashedId}&l={SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage}" : $"http://www.thewalkingdeadnomansland.com/guildinvite/?g={id}&p={hashedId}&l={SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage}&s={bundleURLScheme}");
		GUIUtility.systemCopyBuffer = text + text2;
	}
}
