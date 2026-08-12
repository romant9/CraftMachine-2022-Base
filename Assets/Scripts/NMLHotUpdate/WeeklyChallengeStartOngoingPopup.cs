using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeStartOngoingPopup : HUDElement
{
	[Header("Top Level")]
	[SerializeField]
	private UILabel titleLabelSmall;

	[SerializeField]
	private UILabel titleLabelBig;

	[SerializeField]
	private UIButtonWithLabelAndIcon mainButton;

	[SerializeField]
	private UILabel starConditionLabel1;

	[SerializeField]
	private UILabel starConditionLabel2;

	[SerializeField]
	private UILabel starConditionLabel3;

	[SerializeField]
	private UIChallengeDifficultyProgressBar challengeDifficultyProgressBar;

	[Header("Progress Bar")]
	[SerializeField]
	private UIRewardsProgressBar progressBar;

	[Header("Info Panel")]
	[SerializeField]
	private UIButtonWithLabelAndIcon leftDataButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon rightDataButton;

	[SerializeField]
	private UILabel statsTitle;

	[SerializeField]
	private UIDataRow[] statsRows;

	[SerializeField]
	private UIDataRows dataRows;

	private bool personal = true;

	public static bool TryOpenOnChallengeEnter()
	{
		if (WeeklyChallengeHelper.GetWeeklyChallengeModel() != null && WeeklyChallengeHelper.IsChallengeOngoing() && !WeeklyChallengeHelper.GetWeeklyChallengeModel().ChallengeStartedSeen)
		{
			return TryOpenWithWeeklyModel(WeeklyChallengeHelper.GetWeeklyChallengeModel());
		}
		return false;
	}

	public static bool TryOpenFromClick()
	{
		return TryOpenWithWeeklyModel(WeeklyChallengeHelper.GetWeeklyChallengeModel());
	}

	public static bool TryOpenWithWeeklyModel(WeeklyChallengeModel model)
	{
		if (model != null)
		{
			WeeklyChallengeStartOngoingPopup weeklyChallengeStartOngoingPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeStartOngoing) as WeeklyChallengeStartOngoingPopup;
			if (weeklyChallengeStartOngoingPopup != null)
			{
				weeklyChallengeStartOngoingPopup.OpenForModel(model);
				return true;
			}
		}
		else
		{
			Debug.LogError("Could not open WeeklyChallengeStartOngoingPopup because model was NULL");
		}
		return false;
	}

	public override void Open()
	{
		base.Open();
		if (WeeklyChallengeHelper.GetWeeklyChallengeModel() != null)
		{
			WeeklyChallengeHelper.MarkCurrentChallengeAsSeen();
		}
		if (mainButton != null)
		{
			mainButton.SetClickCallback(OnMainButtonClicked);
		}
		if (leftDataButton != null)
		{
			Helpers.GameObjectSetActive(leftDataButton, GameManager.Instance.playerModel.IsGuildMember);
			leftDataButton.SetClickCallback(OnDataButtonClickedLeft);
		}
		if (rightDataButton != null)
		{
			Helpers.GameObjectSetActive(rightDataButton, GameManager.Instance.playerModel.IsGuildMember);
			rightDataButton.SetClickCallback(OnDataButtonClickedRight);
		}
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WeeklyChallengeModel weeklyChallengeModel = GetModel<WeeklyChallengeModel>();
		if (weeklyChallengeModel != null)
		{
			if (!personal && !GameManager.Instance.playerModel.IsGuildMember)
			{
				personal = true;
			}
			HelpersUI.SetContentToLabel(titleLabelSmall, WeeklyChallengeHelper.GetFormatedTimeLeftToCurrentChallengeEnd());
			HelpersUI.SetContentToLabel(titleLabelBig, WeeklyChallengeHelper.GetCurrentChallengeName());
			if (progressBar != null)
			{
				progressBar.ShowProgress(personal);
			}
			if (challengeDifficultyProgressBar != null)
			{
				challengeDifficultyProgressBar.UpdateToCurrentProgression();
			}
			if (statsTitle != null)
			{
				statsTitle.text = LocalizationManager.GetText(personal ? "Popup.Challenge.Intro.Stats.Personal" : "Popup.Challenge.Intro.Stats.Guild");
			}
			GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
			List<string> statsString = GetStatsString(personal, weeklyChallengeModel, guildModel);
			for (int i = 0; i < statsRows.Length; i++)
			{
				if (!(statsRows[i] != null))
				{
					continue;
				}
				bool flag = statsString.Count / 2 > i;
				statsRows[i].gameObject.SetActive(flag);
				if (flag)
				{
					if (statsRows[i].labelArray.Length != 0)
					{
						statsRows[i].labelArray[0].text = statsString[i * 2];
					}
					if (statsRows[i].labelArray.Length > 1)
					{
						statsRows[i].labelArray[1].text = statsString[i * 2 + 1];
					}
				}
				if (i >= 2 && statsRows[i].spriteArray.Length != 0)
				{
					statsRows[i].spriteArray[0].gameObject.SetActive(!personal);
				}
			}
			List<UILabel> list = new List<UILabel> { starConditionLabel1, starConditionLabel2, starConditionLabel3 };
			MissionStarCondition[] array = null;
			if (weeklyChallengeModel != null && weeklyChallengeModel.GetMapMissionGroupModel() != null)
			{
				MapMissionGroupModel mapMissionGroupModel = weeklyChallengeModel.GetMapMissionGroupModel();
				if (mapMissionGroupModel != null && mapMissionGroupModel.Missions != null && mapMissionGroupModel.Missions.Count > 0)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[0];
					if (mapMissionModel != null && mapMissionModel.MissionData != null && mapMissionModel.MissionData.MissionStarConditions != null && mapMissionModel.MissionData.MissionStarConditions != null && mapMissionModel.MissionData.MissionStarConditions.Conditions != null)
					{
						array = mapMissionModel.MissionData.MissionStarConditions.Conditions;
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				UILabel uILabel = list[j];
				if (uILabel != null)
				{
					bool flag2 = j < array.Length && array[j] != null;
					if (flag2)
					{
						uILabel.text = LocalizationManager.GetText("Map.Star.Condition." + array[j].Type.ToString() + "{Parameter}", array[j].Parameter);
					}
					uILabel.gameObject.SetActive(flag2);
				}
			}
		}
		else
		{
			Debug.LogError("WeeklyChallengeStartOngoingPopup: Cant UpdateUI with NULL model!");
		}
	}

	public void Clear()
	{
		if (mainButton != null)
		{
			mainButton.Clear();
		}
		if (progressBar != null)
		{
			progressBar.Clear();
		}
		if (leftDataButton != null)
		{
			leftDataButton.Clear();
		}
		if (rightDataButton != null)
		{
			rightDataButton.Clear();
		}
	}

	private void TestUpdateInfoPanel(string[] content, int rows)
	{
		if (dataRows != null)
		{
			for (int i = 0; i < dataRows.Count; i++)
			{
				dataRows.SetDataToIndex(i, null);
			}
			for (int j = 0; j < rows; j++)
			{
				dataRows.SetDataToIndex(j, content);
			}
			dataRows.PositionRows();
		}
	}

	public void OnInfoClicked()
	{
		WeeklyChallengeInfoPopup.TryOpenFromClick();
	}

	private void OnMainButtonClicked(UIButtonExtended button)
	{
		OnClickClose();
	}

	private void OnDataButtonClickedLeft(UIButtonExtended button)
	{
		SwitchMode();
		UpdateUI();
	}

	private void OnDataButtonClickedRight(UIButtonExtended button)
	{
		SwitchMode();
		UpdateUI();
	}

	private void SwitchMode()
	{
		personal = !(GameManager.Instance != null) || GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.GuildModel == null || !personal;
	}

	public static List<string> GetStatsString(bool usePersonal, WeeklyChallengeModel weeklyChallengeModel, GuildModel guildModel)
	{
		string text = LocalizationManager.GetText("Popup.Challenge.Intro.Stats.LastWeek");
		string text2 = LocalizationManager.GetText("Popup.Challenge.Intro.Stats.AllTime");
		List<string> list = new List<string>();
		if (usePersonal && weeklyChallengeModel != null)
		{
			string item = weeklyChallengeModel.PreviousNumberStars.ToString();
			string item2 = GameManager.Instance.playerModel.HighestWeeklyChallengeScore.ToString();
			string text3 = LocalizationManager.GetText("Popup.Challenge.Intro.Stats.Personal.LastWeekDifficulty");
			string item3 = weeklyChallengeModel.PreviousChallengeHighestDifficulty.ToString();
			string text4 = LocalizationManager.GetText("Popup.Challenge.Intro.Stats.Personal.AllTimeDifficulty");
			string item4 = GameManager.Instance.playerModel.HighestWeeklyChallengeDifficulty.ToString();
			list.Add(text);
			list.Add(item);
			list.Add(text2);
			list.Add(item2);
			list.Add(text3);
			list.Add(item3);
			list.Add(text4);
			list.Add(item4);
		}
		else if (guildModel != null)
		{
			string item5 = guildModel.PreviousChallengeStars.ToString();
			string item6 = guildModel.HighestChallengeStarsCount.ToString();
			list.Add(text);
			list.Add(item5);
			list.Add(text2);
			list.Add(item6);
			List<GuildMemberInfo> guildMembersOrderedByScore = GuildModel.GetGuildMembersOrderedByScore(guildModel);
			FixedPoint fixedPoint = ((GameManager.Instance.playerModel.gameEconomyData != null && GameManager.Instance.playerModel.gameEconomyData.ConfigData != null) ? GameManager.Instance.playerModel.gameEconomyData.ConfigData.ChallengeGuildAchieverTopPlayersRatio : ((FixedPoint)0L));
			int num = ((GameManager.Instance.playerModel.gameEconomyData != null && GameManager.Instance.playerModel.gameEconomyData.ConfigData != null) ? GameManager.Instance.playerModel.gameEconomyData.ConfigData.ChallengeGuildAchieverMinimumMembers : 0);
			if (((guildMembersOrderedByScore.Count >= num) ? ((int)((float)guildMembersOrderedByScore.Count * fixedPoint)) : (-1)) > -1)
			{
				string text5 = LocalizationManager.GetText("Popup.Challenge.Intro.Stats.Guild.AveragePlayerScore");
				string item7 = Mathf.RoundToInt((float)guildMembersOrderedByScore.Average((GuildMemberInfo info) => info.PreviousChallengeStars)).ToString();
				list.Add(text5);
				list.Add(item7);
			}
			string text6 = LocalizationManager.GetText("Popup.Challenge.Intro.Stats.Guild.TopPlayerScore{PlayerName}", guildMembersOrderedByScore[0].Name);
			string item8 = ((guildMembersOrderedByScore.Count > 0) ? guildMembersOrderedByScore[0].PreviousChallengeStars.ToString() : "");
			list.Add(text6);
			list.Add(item8);
		}
		return list;
	}
}
