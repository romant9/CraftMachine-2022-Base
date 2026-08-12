using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class MissionObjectivesPopup : HUDElement
{
	public GameObject ChallengeContainer;

	public GameObject StoryContainer;

	public GameObject PvPContainer;

	public UIButton EndMissionButton;

	public UILabel EndMissionButtonLabel;

	public UIButton ContinueButton;

	public GameObject StarPrefab;

	public UILabel ChallengeObjectiveLabel;

	public UILabel StoryObjectiveLabel;

	public UILabel NoStarsLabel;

	public List<GameObject> StarAnchors;

	public UISprite[] MissionObjectiveIcons1;

	public UISprite MissionObjectiveIcon2;

	public UISprite[] MissionObjectiveIcons3;

	public UILabel MissionObjectiveLabel1;

	public UILabel MissionObjectiveLabel2;

	public UILabel MissionObjectiveLabel3;

	public UILabel MissionObjective1TradeGoodAmountLabel;

	public UILabel MissionObjective1InfluenceAmountLabel;

	public UILabel MissionObjective2TradeGoodAmountLabel;

	public UILabel MissionObjective2InfluenceAmountLabel;

	public UILabel MissionObjective3TradeGoodAmountLabel;

	public UILabel MissionObjective3InfluenceAmountLabel;

	public GameObject TutorialRewardObject;

	public UILabel TutorialRewardTradeGoodAmountLabel;

	public UILabel TutorialRewardInfluenceAmountLabel;

	public Color CompletedObjectiveColor;

	public Color UncompletedObjectiveColor;

	private List<GameObject> Stars;

	public void InstantiateStars(int numOfStars)
	{
		int num = Mathf.Min(StarAnchors.Count, numOfStars);
		if (StarAnchors == null || (Stars != null && Stars.Count == num))
		{
			return;
		}
		if (Stars == null)
		{
			Stars = new List<GameObject>();
		}
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = StarAnchors[i];
			if (StarPrefab != null && gameObject != null && gameObject.transform.childCount == 0)
			{
				GameObject gameObject2 = Object.Instantiate(StarPrefab);
				gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
				Stars.Add(gameObject2);
			}
		}
	}

	public override void Open()
	{
		base.Open();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		CombatModel combat = playerModel.Combat;
		ConfigData configData = GameManager.Instance.gameEconomyData.ConfigData;
		bool flag = false;
		bool flag2 = combat?.HasPvPRules ?? false;
		MapContainerModel mapContainerModel = GameManager.Instance.playerModel.MapContainerModel;
		MapMissionStars mapMissionStars = null;
		if (mapContainerModel != null && mapContainerModel.AttackTargetMissionModel != null)
		{
			mapMissionStars = mapContainerModel.AttackTargetMissionModel.Stars;
			if (mapMissionStars != null && StarPrefab != null && (mapContainerModel.AttackTargetMissionModel.IsInWeeklyChallenge || mapContainerModel.AttackTargetMissionModel.IsInApocalyptiWeeklyChallenge))
			{
				flag = true;
				InstantiateStars(3);
				MissionStarCondition[] conditions = mapContainerModel.AttackTargetMissionModel.MissionData.MissionStarConditions.Conditions;
				for (int i = 0; i < 3 && i < Stars.Count; i++)
				{
					MissionStarCondition starCondition = conditions[i];
					Stars[i].GetComponent<MissionObjectiveStar>().SetStar(starCondition, flag || mapMissionStars.Stars[i]);
				}
			}
		}
		if (ChallengeContainer != null)
		{
			ChallengeContainer.SetActive(flag);
		}
		if (StoryContainer != null)
		{
			StoryContainer.SetActive(!flag && !flag2);
		}
		if (PvPContainer != null)
		{
			PvPContainer.SetActive(flag2);
		}
		if (TutorialRewardObject != null)
		{
			TutorialRewardObject.SetActive(value: false);
		}
		if (EndMissionButton != null && flag2)
		{
			EndMissionButton.gameObject.SetActive(combat.HasPvPRules && (combat.IsPvPFlagCollected || combat.IsPvPLootCollected || combat.IsPvpDefendersKilled));
			int survivorsIncapacitated = 0;
			int survivorsInExits = 0;
			combat.GetSurvivorStatus(out survivorsIncapacitated, out survivorsInExits);
			switch (combat.GetPvpResult(survivorsIncapacitated, combat.Survivors.Count))
			{
			case ECombatResult.Draw:
				EndMissionButtonLabel.text = LocalizationManager.GetText("Combat.Button.DrawOutpost");
				break;
			case ECombatResult.Successful:
				EndMissionButtonLabel.text = LocalizationManager.GetText("Combat.Button.CompleteOutpost");
				break;
			}
			ContinueButton.gameObject.SetActive(combat.HasPvPRules && (combat.IsPvPFlagCollected || combat.IsPvPLootCollected || combat.IsPvpDefendersKilled));
			int num = 0;
			int num2 = 0;
			int firstValue = 0;
			int firstValue2 = 0;
			int secondValue = 0;
			int secondValue2 = 0;
			int thirdValue = 0;
			int thirdValue2 = 0;
			if (combat.IsPVPMission)
			{
				OutpostCombat outpostCombat = ((playerModel.Combat == null) ? null : playerModel.Combat.OutpostCombat);
				num = outpostCombat?.AttackerInfluenceGain ?? 0;
				num2 = outpostCombat?.TradeGoodsGain ?? 0;
				UtilsMath.SplitValue(num, configData.OutpostCratesCompletedInfluencePercentage, configData.OutpostFlagCompletedInfluencePercentage, configData.OutpostDefendersCompletedInfluencePercentage, out firstValue, out secondValue, out thirdValue);
				UtilsMath.SplitValue(num2, configData.OutpostCratesCompletedResourcePercentage, configData.OutpostFlagCompletedResourcePercentage, configData.OutpostDefendersCompletedResourcePercentage, out firstValue2, out secondValue2, out thirdValue2);
			}
			else if (combat.HasPvPRules)
			{
				num = playerModel.GetOutpostTutorialInfluenceReward();
				num2 = playerModel.GetOutpostTutorialTradeGoodsReward();
				if (num > 0)
				{
					UtilsMath.SplitValue(num, 33, 34, 33, out firstValue, out secondValue, out thirdValue);
				}
				if (num2 > 0)
				{
					UtilsMath.SplitValue(num2, 33, 34, 33, out firstValue2, out secondValue2, out thirdValue2);
				}
				if (TutorialRewardObject != null)
				{
					TutorialRewardTradeGoodAmountLabel.text = num2.ToString();
					TutorialRewardInfluenceAmountLabel.text = num.ToString();
					TutorialRewardObject.SetActive(value: true);
				}
			}
			if (MissionObjectiveIcons1 != null)
			{
				if (combat.PvPMissionType == PvPMissionType.PVPMultiLoot || combat.PvPMissionType == PvPMissionType.FakePVPMultiLoot)
				{
					for (int j = 0; j < MissionObjectiveIcons1.Length; j++)
					{
						UISprite obj = MissionObjectiveIcons1[j];
						obj.color = (combat.IsPvPLootCollected ? CompletedObjectiveColor : UncompletedObjectiveColor);
						obj.spriteName = "Ui_Icon_Crate";
					}
					MissionObjectiveLabel1.text = LocalizationManager.GetText("Popup.Objectives.LootBoxes");
					MissionObjective1TradeGoodAmountLabel.text = firstValue2.ToString();
					MissionObjective1InfluenceAmountLabel.text = firstValue.ToString();
				}
				else if (combat.PvPMissionType == PvPMissionType.PVPMultiFlag || combat.PvPMissionType == PvPMissionType.FakePVPMultiFlag)
				{
					for (int k = 0; k < MissionObjectiveIcons1.Length; k++)
					{
						UISprite obj2 = MissionObjectiveIcons1[k];
						obj2.color = (combat.IsPvPFlagCollected ? CompletedObjectiveColor : UncompletedObjectiveColor);
						obj2.spriteName = "Ui_Icon_Flag";
					}
					MissionObjectiveLabel1.text = LocalizationManager.GetText("Popup.Objectives.ClaimFlags");
					MissionObjective1TradeGoodAmountLabel.text = secondValue2.ToString();
					MissionObjective1InfluenceAmountLabel.text = secondValue.ToString();
				}
			}
			if (MissionObjectiveIcon2 != null)
			{
				if (combat.PvPMissionType == PvPMissionType.PVPMultiLoot || combat.PvPMissionType == PvPMissionType.FakePVPMultiLoot)
				{
					MissionObjectiveIcon2.color = (combat.IsPvPFlagCollected ? CompletedObjectiveColor : UncompletedObjectiveColor);
					MissionObjectiveIcon2.spriteName = "Ui_Icon_Flag";
					MissionObjectiveLabel2.text = LocalizationManager.GetText("Popup.Objectives.ClaimFlag");
					MissionObjective2TradeGoodAmountLabel.text = secondValue2.ToString();
					MissionObjective2InfluenceAmountLabel.text = secondValue.ToString();
				}
				else if (combat.PvPMissionType == PvPMissionType.PVPMultiFlag || combat.PvPMissionType == PvPMissionType.FakePVPMultiFlag)
				{
					MissionObjectiveIcon2.color = (combat.IsPvPLootCollected ? CompletedObjectiveColor : UncompletedObjectiveColor);
					MissionObjectiveIcon2.spriteName = "Ui_Icon_Crate";
					MissionObjectiveLabel2.text = LocalizationManager.GetText("Popup.Objectives.LootBox");
					MissionObjective2TradeGoodAmountLabel.text = firstValue2.ToString();
					MissionObjective2InfluenceAmountLabel.text = firstValue.ToString();
				}
			}
			if (MissionObjectiveIcons3 != null)
			{
				for (int l = 0; l < MissionObjectiveIcons3.Length; l++)
				{
					MissionObjectiveIcons3[l].color = (combat.IsPvpDefendersKilled ? CompletedObjectiveColor : UncompletedObjectiveColor);
				}
				MissionObjectiveLabel3.text = LocalizationManager.GetText("Popup.Objectives.KillDefenders");
				MissionObjective3TradeGoodAmountLabel.text = thirdValue2.ToString();
				MissionObjective3InfluenceAmountLabel.text = thirdValue.ToString();
			}
		}
		if (NoStarsLabel != null)
		{
			NoStarsLabel.gameObject.SetActive(value: false);
		}
		if (ChallengeObjectiveLabel != null)
		{
			MissionObjective currentMissionObjective = GameManager.Instance.playerModel.Combat.CurrentMissionObjective;
			ChallengeObjectiveLabel.text = LocalizationManager.GetText("MissionObjective.Details." + currentMissionObjective.Description, currentMissionObjective.CustomText1, currentMissionObjective.CustomText2);
		}
		if (StoryObjectiveLabel != null)
		{
			MissionObjective currentMissionObjective2 = GameManager.Instance.playerModel.Combat.CurrentMissionObjective;
			StoryObjectiveLabel.text = LocalizationManager.GetText("MissionObjective.Details." + currentMissionObjective2.Description, currentMissionObjective2.CustomText1, currentMissionObjective2.CustomText2);
		}
	}

	public void OnEndMissionClicked()
	{
		if (GameManager.Instance.playerModel.Combat.IsPVPMission)
		{
			ConfirmCompleteMission();
			return;
		}
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.Outpost.Tutorial.CompleteMission.Title"), LocalizationManager.GetText("Popup.Outpost.Tutorial.CompleteMission.Body"), LocalizationManager.GetText("Button.Ok"), delegate
		{
			ConfirmCompleteMission();
		}, LocalizationManager.GetText("Button.Cancel"), delegate
		{
		});
	}

	private void ConfirmCompleteMission()
	{
		Helpers.ExecuteCommand(new EndCombatCommand());
	}

	private void OnClick()
	{
		Close();
		CheckForCoverPart1SmartTutorial();
	}

	private void CheckForCoverPart1SmartTutorial()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat != null && !GameManager.Instance.SmartTutorialData.HasShown(SmartTutorialType.CoverPart1))
		{
			List<TWDModelObject> models = combat.GetModels<CoverModel>();
			if (models != null && models.Count > 0)
			{
				GameManager.Instance.SmartTutorialData.StartSmartTutorial(SmartTutorialType.CoverPart1);
			}
		}
	}
}
