using TWDModel;

public class GvgStartBattleStateRegistrationBase : GvgStartBattleStateBase
{
	public override void UpdateUI()
	{
		base.UpdateUI();
		GameEconomyData gameEconomyData = GameManager.Instance.gameEconomyData;
		int maxPlayerCountInBattle = gameEconomyData.GuildWarConfig.MaxPlayerCountInBattle;
		int registeredPlayersCountForBattleTimeSlot = GuildWarHelper.GetRegisteredPlayersCountForBattleTimeSlot();
		bool flag = GuildWarHelper.GetRegisteredPlayersForBattleTimeSlot().Contains(GameManager.Instance.playerModel.HashedId);
		bool flag2 = registeredPlayersCountForBattleTimeSlot >= maxPlayerCountInBattle;
		bool flag3 = registeredPlayersCountForBattleTimeSlot >= gameEconomyData.GuildWarConfig.MinPlayersToStartBattle && (flag || !flag2);
		Helpers.GameObjectSetActive(MatchTimeLabel, value: true);
		Helpers.GameObjectSetActive(GuildEmblem, flag3);
		Helpers.GameObjectSetActive(GuildEmblemEmpty, !flag3);
		Helpers.GameObjectSetActive(EnemyGuildEmblemEmpty, value: true);
		Helpers.GameObjectSetActive(EnemyGuildEmblem, value: false);
		Helpers.GameObjectSetActive(FakeEnemyGuildEmblem, value: false);
		Helpers.GameObjectSetActive(StartBattleContainer, value: false);
		Helpers.GameObjectSetActive(FakeBattleStartContainer, value: false);
	}
}
