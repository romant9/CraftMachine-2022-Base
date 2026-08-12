using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BattleLogListCard : UIListCard<OutpostVisitEntry>
{
	[SerializeField]
	private UILabel opponentNameLabel;

	[SerializeField]
	private UILabel combatResultLabel;

	[SerializeField]
	private UILabel timeStampLabel;

	[SerializeField]
	private UILabel influenceAmountLabel;

	[SerializeField]
	private UILabel tradeGoodAmountLabel;

	[SerializeField]
	private UISprite victoryBg;

	[SerializeField]
	private UISprite logItemTypeBg;

	[SerializeField]
	private UISprite flagObjectiveIcon;

	[SerializeField]
	private UISprite flagObjectiveCompletedIcon;

	[SerializeField]
	private UISprite defendersObjectiveIcon;

	[SerializeField]
	private UISprite defendersObjectiveCompletedIcon;

	[SerializeField]
	private UISprite tradeGoodObjectiveIcon;

	[SerializeField]
	private UISprite tradeGoodObjectiveCompletedIcon;

	[SerializeField]
	private List<UISprite> defendingTeamIcons;

	[SerializeField]
	private List<UILabel> defendingTeamLevelLabels;

	[SerializeField]
	private List<UISprite> defendingTeamDefeatedIcons;

	[SerializeField]
	private UILabel defendingTeamLabel;

	[SerializeField]
	private List<UISprite> attackingTeamIcons;

	[SerializeField]
	private List<UILabel> attackingTeamLevelLabels;

	[SerializeField]
	private List<UISprite> attackingTeamDefeatedIcons;

	[SerializeField]
	private UILabel attackingTeamLabel;

	[SerializeField]
	private Color objectiveCompletedColor;

	[SerializeField]
	private Color objectiveFailedColor;

	[SerializeField]
	private Color victoryColor;

	[SerializeField]
	private Color defeatColor;

	[SerializeField]
	private Color drawColor;

	[SerializeField]
	private Color bgVictoryColor;

	[SerializeField]
	private Color bgDefeatColor;

	[SerializeField]
	private Color bgDrawColor;

	[SerializeField]
	private Color bgAttackColor;

	[SerializeField]
	private Color bgDefenseColor;

	[SerializeField]
	private EffectSparkle effectSparkle;

	public UISprite TradeGoodsIcon;

	public UISprite InfluenceIcon;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item == null)
		{
			return;
		}
		ECombatResult eCombatResult = base.Item.CombatResult;
		bool flag = base.Item.EntryType == OutpostVisitEntryType.Attacked;
		if (!flag)
		{
			eCombatResult = eCombatResult switch
			{
				ECombatResult.Failed => ECombatResult.Successful, 
				ECombatResult.Successful => ECombatResult.Failed, 
				_ => eCombatResult, 
			};
		}
		if (opponentNameLabel != null)
		{
			opponentNameLabel.text = GameManager.Instance.GetFilteredText(base.Item.OtherPlayerName);
		}
		if (combatResultLabel != null)
		{
			string text = "";
			text = ((!flag) ? (eCombatResult switch
			{
				ECombatResult.Draw => "BattleLog.Title.Draw", 
				ECombatResult.Successful => "BattleLog.Title.DefenderVictory", 
				_ => "BattleLog.Title.DefenderDefeat", 
			}) : (eCombatResult switch
			{
				ECombatResult.Draw => "BattleLog.Title.Draw", 
				ECombatResult.Successful => "BattleLog.Title.AttackerVictory", 
				_ => "BattleLog.Title.AttackerDefeat", 
			}));
			combatResultLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(text);
			combatResultLabel.color = eCombatResult switch
			{
				ECombatResult.Draw => drawColor, 
				ECombatResult.Successful => victoryColor, 
				_ => defeatColor, 
			};
		}
		if (timeStampLabel != null)
		{
			timeStampLabel.text = Helpers.FormatTimeAgo(GameManager.Instance.playerModel.UtcTimeStamp, base.Item.UtcTime);
		}
		if (influenceAmountLabel != null)
		{
			int rankingScoreChange = base.Item.RankingScoreChange;
			influenceAmountLabel.text = rankingScoreChange.ToString();
		}
		if (tradeGoodAmountLabel != null)
		{
			int resourcesStolen = base.Item.ResourcesStolen;
			tradeGoodAmountLabel.text = resourcesStolen.ToString();
		}
		if (defendingTeamIcons != null)
		{
			SurvivorClass[] array = (flag ? base.Item.OtherSurvivorClasses : base.Item.SurvivorClasses);
			int[] array2 = (flag ? base.Item.OtherSurvivorRarityLevels : base.Item.SurvivorRarityLevels);
			for (int i = 0; i < array.Length; i++)
			{
				if (i < defendingTeamIcons.Count)
				{
					UISprite uISprite = defendingTeamIcons[i];
					int rarityLevel = 0;
					if (array2 != null)
					{
						rarityLevel = array2[i];
					}
					uISprite.spriteName = HelpersGfx.GetSurvivorClassIconName(array[i].ToString(), rarityLevel);
				}
			}
		}
		if (defendingTeamLevelLabels != null)
		{
			int[] array3 = (flag ? base.Item.OtherSurvivorLevels : base.Item.SurvivorLevels);
			for (int j = 0; j < array3.Length; j++)
			{
				if (j < defendingTeamLevelLabels.Count)
				{
					defendingTeamLevelLabels[j].text = array3[j].ToString();
				}
			}
		}
		if (defendingTeamDefeatedIcons != null)
		{
			bool[] array4 = (flag ? base.Item.OtherSurvivorDefeated : base.Item.SurvivorDefeated);
			for (int k = 0; k < array4.Length; k++)
			{
				if (k < defendingTeamDefeatedIcons.Count)
				{
					defendingTeamDefeatedIcons[k].gameObject.SetActive(array4[k]);
				}
			}
		}
		if (attackingTeamIcons != null)
		{
			SurvivorClass[] array5 = (flag ? base.Item.SurvivorClasses : base.Item.OtherSurvivorClasses);
			int[] array6 = (flag ? base.Item.SurvivorRarityLevels : base.Item.OtherSurvivorRarityLevels);
			for (int l = 0; l < array5.Length; l++)
			{
				if (l < attackingTeamIcons.Count)
				{
					UISprite uISprite2 = attackingTeamIcons[l];
					int rarityLevel2 = 0;
					if (array6 != null)
					{
						rarityLevel2 = array6[l];
					}
					uISprite2.spriteName = HelpersGfx.GetSurvivorClassIconName(array5[l].ToString(), rarityLevel2);
				}
			}
		}
		if (attackingTeamLevelLabels != null)
		{
			int[] array7 = (flag ? base.Item.SurvivorLevels : base.Item.OtherSurvivorLevels);
			for (int m = 0; m < array7.Length; m++)
			{
				if (m < attackingTeamLevelLabels.Count)
				{
					attackingTeamLevelLabels[m].text = array7[m].ToString();
				}
			}
		}
		if (attackingTeamDefeatedIcons != null)
		{
			bool[] array8 = (flag ? base.Item.SurvivorDefeated : base.Item.OtherSurvivorDefeated);
			for (int n = 0; n < array8.Length; n++)
			{
				if (n < attackingTeamDefeatedIcons.Count)
				{
					attackingTeamDefeatedIcons[n].gameObject.SetActive(array8[n]);
				}
			}
		}
		if (flag)
		{
			attackingTeamLabel.text = LocalizationManager.GetText("BattleLog.Friendly");
			defendingTeamLabel.text = LocalizationManager.GetText("BattleLog.Enemy");
		}
		else
		{
			attackingTeamLabel.text = LocalizationManager.GetText("BattleLog.Enemy");
			defendingTeamLabel.text = LocalizationManager.GetText("BattleLog.Friendly");
		}
		if (victoryBg != null)
		{
			victoryBg.color = eCombatResult switch
			{
				ECombatResult.Draw => bgDrawColor, 
				ECombatResult.Successful => bgVictoryColor, 
				_ => bgDefeatColor, 
			};
		}
		if (logItemTypeBg != null)
		{
			logItemTypeBg.color = (flag ? bgAttackColor : bgDefenseColor);
		}
		flagObjectiveCompletedIcon.gameObject.SetActive(base.Item.FirstObjectiveCompleted);
		defendersObjectiveCompletedIcon.gameObject.SetActive(base.Item.DefendersObjectiveCompleted);
		tradeGoodObjectiveCompletedIcon.gameObject.SetActive(base.Item.SecondObjectiveCompleted);
		flagObjectiveIcon.color = (base.Item.FirstObjectiveCompleted ? objectiveCompletedColor : objectiveFailedColor);
		defendersObjectiveIcon.color = (base.Item.DefendersObjectiveCompleted ? objectiveCompletedColor : objectiveFailedColor);
		tradeGoodObjectiveIcon.color = (base.Item.SecondObjectiveCompleted ? objectiveCompletedColor : objectiveFailedColor);
	}

	public override int GetSortValue()
	{
		return 0;
	}

	public void SetSparkeEnabled(bool enabled)
	{
		effectSparkle.enabled = enabled;
	}
}
