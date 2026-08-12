using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostDetailsPanelMatchMaking : OutpostDetailsPanelEdit
{
	[SerializeField]
	private UILabel DefendersName;

	[SerializeField]
	private UILabel RewardsCurrencyLabel;

	[SerializeField]
	private UISprite RewardsCurrencyIcon;

	[SerializeField]
	private UILabel RewardsTrophyLabel;

	[SerializeField]
	private UISprite RewardsTrophyIcon;

	[Header("Only one will be shown")]
	[SerializeField]
	private UIButton StartMissionButton;

	[SerializeField]
	private PayButton StartMissionPayButton;

	public MatchInfo CurrentMatchInfo { get; set; }

	public string CurrentMatchSurviviorName { get; set; }

	public string CurrentMatchPlayerHashedId { get; set; }

	public override void UpdateUI()
	{
		if (CurrentMatchInfo == null)
		{
			return;
		}
		if (RewardsCurrencyLabel != null)
		{
			RewardsCurrencyLabel.text = OutpostCombat.GetTradeGoodsReward(GameManager.Instance.gameEconomyData, CurrentMatchInfo.DefendingPlayerLevel, CurrentMatchInfo.DefendingOutpostPower, GameManager.Instance.playerModel.HashedId, CurrentMatchPlayerHashedId, CurrentMatchInfo.IsFake).ToString();
		}
		if (RewardsCurrencyIcon != null)
		{
			RewardsCurrencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.Outpost);
		}
		if (RewardsTrophyLabel != null)
		{
			RewardsTrophyLabel.text = CurrentMatchInfo.GetRankingScoreGain(GameManager.Instance.playerModel).ToString();
		}
		if (RewardsTrophyIcon != null)
		{
			RewardsTrophyIcon.gameObject.SetActive(value: true);
		}
		if (DefendersName != null)
		{
			DefendersName.text = GameManager.Instance.GetFilteredText(CurrentMatchSurviviorName);
		}
		if (StartMissionPayButton != null && StartMissionButton != null && GameManager.Instance.playerModel.OutpostModel != null)
		{
			bool hasInjuredSurvivorInCombatTeam = GameManager.Instance.playerModel.SurvivorContainer.HasInjuredSurvivorInCombatTeam;
			bool hasUpgradingSurvivorInCombatTeam = GameManager.Instance.playerModel.SurvivorContainer.HasUpgradingSurvivorInCombatTeam;
			bool isEnabled = !hasInjuredSurvivorInCombatTeam && !hasUpgradingSurvivorInCombatTeam;
			StartMissionPayButton.UpdateUI(GameManager.Instance.playerModel.OutpostModel.GetRaidCashier());
			bool flag = GameManager.Instance.playerModel.OutpostModel.GetRaidCashier().GetTotalCost(CurrencyType.ReplayToken) <= 0;
			if (GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.UnlimitedGas))
			{
				flag = true;
			}
			StartMissionPayButton.gameObject.SetActive(!flag);
			StartMissionButton.gameObject.SetActive(flag);
			StartMissionButton.isEnabled = isEnabled;
			if (StartMissionPayButton.GetComponent<UIButton>() != null)
			{
				StartMissionPayButton.GetComponent<UIButton>().isEnabled = isEnabled;
			}
		}
		UpdateLevel(CurrentMatchInfo.DefendingOutpostWalkerPower);
		List<SurvivorModel> list = new List<SurvivorModel>();
		for (int i = 0; i < CurrentMatchInfo.DefendingSurvivorClasses.Count; i++)
		{
			SurvivorModel survivorModel = new SurvivorModel();
			survivorModel.SurvivorClass = CurrentMatchInfo.DefendingSurvivorClasses[i];
			survivorModel.SurvivorRarityLevel = CurrentMatchInfo.DefendingSurvivorRarityLevels[i];
			survivorModel.SurvivorName = CurrentMatchInfo.DefendingSurvivorNames[i];
			survivorModel.Level = CurrentMatchInfo.DefendingSurvivorLevels[i];
			list.Add(survivorModel);
			UpdateDefendersIconsAndLevel(list);
		}
	}

	public override void Update()
	{
	}
}
