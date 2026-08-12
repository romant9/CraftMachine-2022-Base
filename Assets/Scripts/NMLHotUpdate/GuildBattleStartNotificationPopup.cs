using TWDModel;
using UnityEngine;

public class GuildBattleStartNotificationPopup : HUDElement
{
	[SerializeField]
	private UIButtonExtended enterBattleButton;

	[SerializeField]
	private GameObject enemyGuildEmblem;

	[SerializeField]
	private GvGFakeBattleContainer fakeEnemyGuildEmblem;

	public static bool CanShow()
	{
		return (byte)((0u | (ShowStartParticipant() ? 1u : 0u)) & ((!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.GuildBattleMapPopup)) ? 1u : 0u) & ((!GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.HasSeenBattleStart()) ? 1u : 0u)) != 0;
	}

	private static bool ShowStartParticipant()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool flag = GuildWarHelper.IsPlayerRegisteredForBattle();
		bool num = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentBattleActiveForPlayer();
		bool flag2 = GuildWarHelper.IsBattleOnGoing();
		return num && flag && flag2;
	}

	public override void Open()
	{
		base.Open();
		if (enterBattleButton != null)
		{
			enterBattleButton.SetClickCallback(OnEnterBattleButtonClicked);
		}
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GuildWarHelper.GetCurrentBattle().IsFakeBattle)
		{
			Helpers.GameObjectSetActive(enemyGuildEmblem, value: false);
			Helpers.GameObjectSetActive(fakeEnemyGuildEmblem, value: true);
			fakeEnemyGuildEmblem.Setup();
		}
		else
		{
			Helpers.GameObjectSetActive(enemyGuildEmblem, value: true);
			Helpers.GameObjectSetActive(fakeEnemyGuildEmblem, value: false);
		}
	}

	private void OnEnterBattleButtonClicked(UIButtonExtended button)
	{
		Close();
		MissionHubNavigation.OpenGuildBattleMap();
	}

	public override void Close()
	{
		base.Close();
		GuildWarHelper.SendHasSeenGuildBattleStartFlagCommand();
	}
}
