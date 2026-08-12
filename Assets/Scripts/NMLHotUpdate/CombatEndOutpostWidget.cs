using TWDModel;
using UnityEngine;

public class CombatEndOutpostWidget : CombatEndWidget
{
	[Header("0 Flag 1 Treasure 2 Defenders")]
	[SerializeField]
	private OutpostObjectiveCard[] ObjectivesArray;

	[SerializeField]
	private GameObject[] FailedObjectivesArray;

	[Header("Rewards")]
	[SerializeField]
	private UILabel RatingLabel;

	[SerializeField]
	private UISprite RatingSprite;

	[SerializeField]
	private UILabel TradegoodsLabel;

	[SerializeField]
	private UISprite TradegoodsSprite;

	public override void Awake()
	{
		base.Awake();
		DebugClassString = "CombatEndOutpostWidget";
	}

	public void SetData(CombatModel combatModel)
	{
		if (combatModel != null && GameManager.Instance.playerModel != null)
		{
			if (ObjectivesArray != null)
			{
				for (int i = 0; i < ObjectivesArray.Length; i++)
				{
					if (!(ObjectivesArray[i] != null))
					{
						continue;
					}
					switch (i)
					{
					case 0:
					{
						string text3 = "";
						string text4 = "";
						bool flag2 = false;
						if (combatModel.PvPMissionType == PvPMissionType.PVPMultiLoot || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiLoot)
						{
							text3 = "Popup.Victory.TakenTreasures";
							text4 = "Ui_Icon_Crate";
							flag2 = combatModel.IsPvPLootCollected;
						}
						else
						{
							text3 = "Popup.Victory.TakenFlags";
							text4 = "Ui_Icon_Flag";
							flag2 = combatModel.IsPvPFlagCollected;
						}
						FailedObjectivesArray[i].SetActive(!flag2);
						ObjectivesArray[i].SetObjectiveStatus(LocalizationManager.GetText(text3), flag2, text4);
						break;
					}
					case 1:
					{
						string text = "";
						string text2 = "";
						bool flag = false;
						if (combatModel.PvPMissionType == PvPMissionType.PVPMultiLoot || combatModel.PvPMissionType == PvPMissionType.FakePVPMultiLoot)
						{
							text = "Popup.Victory.TakenFlag";
							text2 = "Ui_Icon_Flag";
							flag = combatModel.IsPvPFlagCollected;
						}
						else
						{
							text = "Popup.Victory.TakenTreasure";
							text2 = "Ui_Icon_Crate";
							flag = combatModel.IsPvPLootCollected;
						}
						FailedObjectivesArray[i].SetActive(!flag);
						ObjectivesArray[i].SetObjectiveStatus(LocalizationManager.GetText(text), flag, text2);
						break;
					}
					case 2:
						FailedObjectivesArray[i].SetActive(!combatModel.IsPvpDefendersKilled);
						ObjectivesArray[i].SetObjectiveStatus(LocalizationManager.GetText("Popup.Victory.DefendersKilled"), combatModel.IsPvpDefendersKilled);
						break;
					}
				}
			}
			CalculateAndShowRewards(combatModel, GameManager.Instance.playerModel);
		}
		else
		{
			DebugLogError("CombatModel is NULL");
		}
	}

	private void CalculateAndShowRewards(CombatModel combatModel, PlayerModel playerModel)
	{
		int num = 0;
		int num2 = 0;
		if (combatModel != null && combatModel.OutpostCombat != null)
		{
			num = playerModel.GetFinalResourcesStolen(combatModel.OutpostCombat.TradeGoodsGain);
			if (combatModel.MissionResult == ECombatResult.Successful)
			{
				num2 = playerModel.GetFinalRankingScoreChange(combatModel.OutpostCombat.AttackerInfluenceGain);
			}
			else if (combatModel.MissionResult == ECombatResult.Failed)
			{
				num2 = -combatModel.OutpostCombat.AttackerInfluenceLoss;
			}
		}
		else
		{
			num = playerModel.GetOutpostTutorialTradeGoodsReward();
			num2 = playerModel.GetOutpostTutorialInfluenceReward();
		}
		HelpersUI.SetContentToLabel(RatingLabel, num2.ToString());
		HelpersUI.SetContentToLabel(TradegoodsLabel, num.ToString());
	}
}
