using TWDModel;

public class GuildBattleWelcomePopup : HUDElement
{
	public static void TryOpenWelcomePopup()
	{
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenGuildBattleWelcome"))
		{
			HUDManager.TryOpenPopup(UIType.GuildBattleWelcomePopup);
		}
	}

	public override void Open()
	{
		base.Open();
		if (!GameManager.Instance.playerModel.Blackboard.IsToggleOn("HasSeenGuildBattleWelcome"))
		{
			Helpers.ExecuteCommand(new SetBlackboardToggleCommand("HasSeenGuildBattleWelcome"));
		}
	}
}
