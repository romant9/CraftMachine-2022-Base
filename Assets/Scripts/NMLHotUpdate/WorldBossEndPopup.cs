using BaseModel;
using TWDModel;
using UnityEngine;

public class WorldBossEndPopup : HUDElement
{
	[SerializeField]
	private UILabel resultLabel;

	[SerializeField]
	private UILabel rewardsNumberLabel;

	[SerializeField]
	private UILabel totalScoreLabel;

	[SerializeField]
	private UILabel totalDamageLabel;

	[SerializeField]
	private UILabel battleTimeLabel;

	[SerializeField]
	private UILabel scoreRankLabel;

	[SerializeField]
	private GameObject blueFlag;

	[SerializeField]
	private UILabel blueScoreLabel;

	[SerializeField]
	private UILabel blueNameLabel;

	[SerializeField]
	private GameObject redFlag;

	[SerializeField]
	private UILabel redScoreLabel;

	[SerializeField]
	private UILabel redNameLabel;

	[SerializeField]
	private UILabel seasonTitleLabel;

	private static readonly Color VictoryColor = new Color(1f, 64f / 85f, 8f / 51f, 1f);

	private static readonly Color DefeatColor = new Color(0.5294118f, 29f / 51f, 0.6039216f, 1f);

	private WorldBossCycleSettlementSnapshot settlementSnapshot;

	private bool isProcessing;

	private readonly WorldBossBaseSnapshotHelper worldBossBaseSnapshotHelper = new WorldBossBaseSnapshotHelper();

	public void OpenForSettlement(WorldBossCycleSettlementSnapshot snapshot)
	{
		if (snapshot != null)
		{
			settlementSnapshot = snapshot;
			Open();
		}
	}

	public override void OpenWithStateData(object data)
	{
		OpenForSettlement(data as WorldBossCycleSettlementSnapshot);
	}

	public override void Open()
	{
		if (settlementSnapshot != null)
		{
			base.Open();
			UpdateUI();
		}
	}

	public void ClickGo()
	{
		CompleteSettlement();
	}

	public override void OnClickClose()
	{
		if (TutorialView.Allowed("Close"))
		{
			CompleteSettlement();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager != null && settlementSnapshot != null)
		{
			bool isVictory = settlementSnapshot.IsVictory;
			if (resultLabel != null)
			{
				resultLabel.text = LocalizationManager.GetText(isVictory ? "Combat.EndFade.Victory" : "Combat.EndFade.Defeat");
				resultLabel.color = (isVictory ? VictoryColor : DefeatColor);
			}
			GameManager.Instance?.gameEconomyData?.FindWorldBossSeasonDefinition(settlementSnapshot.SeasonId);
			HelpersUI.SetContentToLabel(totalScoreLabel, Helpers.FormatNumber(settlementSnapshot.PlayerScore));
			HelpersUI.SetContentToLabel(totalDamageLabel, Helpers.FormatNumber(settlementSnapshot.PlayerMaxDamage));
			HelpersUI.SetContentToLabel(battleTimeLabel, Helpers.FormatNumber(settlementSnapshot.PlayerBattleCount));
			HelpersUI.SetContentToLabel(scoreRankLabel, settlementSnapshot.CrossGuildScoreRank.ToString());
			Rewards rewards = worldBossBaseSnapshotHelper.BuildSettlementRewards(worldBossModelManager, settlementSnapshot.SeasonId, settlementSnapshot.RewardDifficulty, settlementSnapshot.MyGuildScore, settlementSnapshot.OpponentGuildScore, settlementSnapshot.PassScore);
			HelpersUI.SetContentToLabel(rewardsNumberLabel, Helpers.FormatNumber(GetFirstRewardAmount(rewards)));
			UpdateBlueRedTeams();
		}
	}

	private void UpdateBlueRedTeams()
	{
		bool flag = !string.IsNullOrEmpty(settlementSnapshot.MyGroupId) && settlementSnapshot.MyGroupId == settlementSnapshot.GroupIdA;
		bool flag2 = !string.IsNullOrEmpty(settlementSnapshot.MyGroupId) && settlementSnapshot.MyGroupId == settlementSnapshot.GroupIdB;
		Helpers.GameObjectSetActive(blueFlag, flag);
		Helpers.GameObjectSetActive(redFlag, flag2);
		long value = (flag ? settlementSnapshot.MyGuildScore : settlementSnapshot.OpponentGuildScore);
		long value2 = (flag2 ? settlementSnapshot.MyGuildScore : settlementSnapshot.OpponentGuildScore);
		HelpersUI.SetContentToLabel(blueNameLabel, settlementSnapshot.GroupNameA);
		HelpersUI.SetContentToLabel(redNameLabel, settlementSnapshot.GroupNameB);
		HelpersUI.SetContentToLabel(blueScoreLabel, Helpers.FormatNumber(value));
		HelpersUI.SetContentToLabel(redScoreLabel, Helpers.FormatNumber(value2));
	}

	private void CompleteSettlement()
	{
		if (!isProcessing && settlementSnapshot != null)
		{
			isProcessing = true;
			if ((settlementSnapshot.HasClaimedSettlement ? Helpers.ExecuteCommand(new MarkWorldBossSettlementShownCommand(settlementSnapshot.SeasonId, settlementSnapshot.CycleId)) : Helpers.ExecuteCommand(new ClaimWorldBossSettlementRewardCommand(settlementSnapshot.SeasonId, settlementSnapshot.CycleId, settlementSnapshot.RewardDifficulty, settlementSnapshot.MyGuildScore, settlementSnapshot.OpponentGuildScore, settlementSnapshot.PassScore))) != TWDModelResult.OK)
			{
				isProcessing = false;
				return;
			}
			settlementSnapshot = null;
			isProcessing = false;
			base.Close();
		}
	}

	private static long GetFirstRewardAmount(Rewards rewards)
	{
		if (rewards?.RewardsList == null || rewards.RewardsList.Count == 0)
		{
			return 0L;
		}
		return (rewards.RewardsList[0] is RewardCurrency rewardCurrency) ? rewardCurrency.Amount : 0;
	}
}
