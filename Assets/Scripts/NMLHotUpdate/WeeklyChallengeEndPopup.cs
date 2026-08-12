using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class WeeklyChallengeEndPopup : HUDElement
{
	public static Color StatsArrowUpColor = new Color(0.21960784f, 0.78039217f, 0f, 1f);

	public static Color StatsArrowDownColor = new Color(29f / 51f, 4f / 85f, 4f / 85f, 1f);

	[SerializeField]
	private UILabel difficultyLabel;

	[SerializeField]
	private UILabel challengeNameLabel;

	[SerializeField]
	private UILabel personalStarsAmountLabel;

	[SerializeField]
	private UISprite personalStarsDiffSprite;

	[SerializeField]
	private UILabel guildStarsAmountLabel;

	[SerializeField]
	private UISprite guildStarsDiffSprite;

	[SerializeField]
	private GameObject guildAchieverRewardContainer;

	[SerializeField]
	private UILabel guildAchieverCurrencyAmount;

	[SerializeField]
	private UISprite guildAchieverCurrencySprite;

	[SerializeField]
	private UILabel statsTitle;

	[SerializeField]
	private UIDataRow[] statsRows;

	private bool personal = true;

	[Header("Info Panel")]
	[SerializeField]
	private UIButtonWithLabelAndIcon leftDataButton;

	[SerializeField]
	private UIButtonWithLabelAndIcon rightDataButton;

	public static WeeklyChallenge LastChallengeData = null;

	public static bool TryOpenWithWeeklyModel(WeeklyChallengeModel model)
	{
		if (model != null)
		{
			WeeklyChallengeEndPopup weeklyChallengeEndPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeEnd) as WeeklyChallengeEndPopup;
			if (weeklyChallengeEndPopup != null)
			{
				weeklyChallengeEndPopup.OpenForModel(model);
				return true;
			}
		}
		else
		{
			Debug.LogError("Could not open WeeklyChallengeEndPopup because model was NULL");
		}
		return false;
	}

	public override void Open()
	{
		base.Open();
		personal = true;
		UpdateUI();
		if (leftDataButton != null)
		{
			leftDataButton.SetClickCallback(OnDataButtonClicked);
		}
		if (rightDataButton != null)
		{
			rightDataButton.SetClickCallback(OnDataButtonClicked);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		WeeklyChallengeModel weeklyChallengeModel = GetModel<WeeklyChallengeModel>();
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (weeklyChallengeModel != null)
		{
			if (weeklyChallengeModel.CurrentDefinition != null)
			{
				HelpersUI.SetContentToLabel(challengeNameLabel, WeeklyChallengeHelper.GetCurrentChallengeName());
			}
			else
			{
				if (LastChallengeData == null)
				{
					LastChallengeData = GameManager.Instance.gameEconomyData.GetLastEndedWeeklyChallenge(GameManager.Instance.playerModel.UtcTimeStamp);
				}
				if (LastChallengeData != null)
				{
					MissionSpawnPointGroup spawnPointGroup = GameManager.Instance.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(LastChallengeData.DetailMapId);
					string content = "";
					if (spawnPointGroup != null)
					{
						content = HelpersLocalization.GetEpisodeName(spawnPointGroup);
					}
					HelpersUI.SetContentToLabel(challengeNameLabel, content);
				}
			}
			if (difficultyLabel != null)
			{
				difficultyLabel.text = weeklyChallengeModel.CurrentRequiredSurvivorLevel.ToString();
			}
			if (personalStarsAmountLabel != null)
			{
				personalStarsAmountLabel.text = weeklyChallengeModel.NumberStars.ToString();
			}
			if (guildStarsAmountLabel != null)
			{
				guildStarsAmountLabel.text = ((guildModel != null) ? guildModel.CurrentChallengeStars.ToString() : "-");
			}
			SetupArrow(weeklyChallengeModel.NumberStars, weeklyChallengeModel.PreviousNumberStars, personalStarsDiffSprite);
			SetupArrow((guildModel != null) ? ((float)guildModel.CurrentChallengeStars) : 0f, (guildModel != null) ? ((float)guildModel.PreviousChallengeStars) : 0f, guildStarsDiffSprite);
			if (statsTitle != null)
			{
				statsTitle.text = LocalizationManager.GetText(personal ? "Popup.Challenge.Intro.Stats.Personal" : "Popup.Challenge.Intro.Stats.Guild");
			}
			if (guildModel != null)
			{
				List<GuildMemberInfo> guildMembersOrderedByScore = GuildModel.GetGuildMembersOrderedByScore(guildModel);
				FixedPoint fixedPoint = ((GameManager.Instance.playerModel.gameEconomyData != null && GameManager.Instance.playerModel.gameEconomyData.ConfigData != null) ? GameManager.Instance.playerModel.gameEconomyData.ConfigData.ChallengeGuildAchieverTopPlayersRatio : ((FixedPoint)0L));
				int num = ((GameManager.Instance.playerModel.gameEconomyData != null && GameManager.Instance.playerModel.gameEconomyData.ConfigData != null) ? GameManager.Instance.playerModel.gameEconomyData.ConfigData.ChallengeGuildAchieverMinimumMembers : 0);
				int num2 = ((guildMembersOrderedByScore.Count >= num) ? ((int)((float)guildMembersOrderedByScore.Count * fixedPoint)) : (-1));
				GuildMemberInfo guildMemberInfo = ((num2 > -1) ? guildMembersOrderedByScore[num2] : null);
				bool flag = guildMemberInfo != null && guildMemberInfo.CurrentChallengeStars <= weeklyChallengeModel.NumberStarsInCurrentGuild;
				if (guildAchieverRewardContainer != null)
				{
					guildAchieverRewardContainer.SetActive(flag);
					if (flag)
					{
						_ = guildAchieverCurrencySprite != null;
						if (!(guildAchieverCurrencyAmount != null))
						{
						}
					}
				}
			}
			else if (guildAchieverRewardContainer != null)
			{
				guildAchieverRewardContainer.SetActive(value: false);
			}
			List<string> statsString = WeeklyChallengeStartOngoingPopup.GetStatsString(personal, weeklyChallengeModel, guildModel);
			for (int i = 0; i < statsRows.Length; i++)
			{
				if (!(statsRows[i] != null))
				{
					continue;
				}
				bool flag2 = statsString.Count / 2 > i;
				statsRows[i].gameObject.SetActive(flag2);
				if (flag2)
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
			SetupStatsArrows();
		}
		else
		{
			Debug.LogError("WeeklyChallengeStartOngoingPopup: Cant UpdateUI with NULL model!");
		}
	}

	private void SetupStatsArrows()
	{
		WeeklyChallengeModel weeklyChallengeModel = GetModel<WeeklyChallengeModel>();
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (personal)
		{
			SetupArrowFromRowIndex(0, weeklyChallengeModel?.NumberStars ?? 0, weeklyChallengeModel?.PreviousNumberStars ?? 0, showStar: true);
			SetupArrowFromRowIndex(1, playerModel.HighestWeeklyChallengeScore, playerModel.HighestWeeklyChallengeScore, showStar: true);
			SetupArrowFromRowIndex(2, weeklyChallengeModel?.CurrentRequiredSurvivorLevel ?? 0, weeklyChallengeModel?.PreviousChallengeHighestDifficulty ?? 0, showStar: false);
			SetupArrowFromRowIndex(3, weeklyChallengeModel?.CurrentRequiredSurvivorLevel ?? 0, playerModel?.HighestWeeklyChallengeDifficulty ?? 0, showStar: false);
			return;
		}
		List<GuildMemberInfo> guildMembersOrderedByScore = GuildModel.GetGuildMembersOrderedByScore(guildModel);
		GuildMemberInfo guildMemberInfo = ((guildMembersOrderedByScore != null && guildMembersOrderedByScore.Count > 0) ? guildMembersOrderedByScore[0] : null);
		FixedPoint fixedPoint = ((GameManager.Instance.playerModel.gameEconomyData != null && GameManager.Instance.playerModel.gameEconomyData.ConfigData != null) ? GameManager.Instance.playerModel.gameEconomyData.ConfigData.ChallengeGuildAchieverTopPlayersRatio : ((FixedPoint)0L));
		int num = ((GameManager.Instance.playerModel.gameEconomyData != null && GameManager.Instance.playerModel.gameEconomyData.ConfigData != null) ? GameManager.Instance.playerModel.gameEconomyData.ConfigData.ChallengeGuildAchieverMinimumMembers : 0);
		int num2 = ((guildMembersOrderedByScore.Count >= num) ? ((int)((float)guildMembersOrderedByScore.Count * fixedPoint)) : (-1));
		GuildMemberInfo guildMemberInfo2 = ((num2 > -1) ? guildMembersOrderedByScore[num2] : null);
		SetupArrowFromRowIndex(0, guildModel?.CurrentChallengeStars ?? 0, guildModel?.PreviousChallengeStars ?? 0, showStar: true);
		SetupArrowFromRowIndex(1, guildModel?.CurrentChallengeStars ?? 0, guildModel?.HighestChallengeStarsCount ?? 0, showStar: true);
		SetupArrowFromRowIndex(2, guildMemberInfo2?.CurrentChallengeStars ?? 0, guildMemberInfo2?.CurrentChallengeStars ?? 0, showStar: true);
		SetupArrowFromRowIndex(3, guildMemberInfo?.CurrentChallengeStars ?? 0, guildMemberInfo?.CurrentChallengeStars ?? 0, showStar: true);
	}

	private void SetupArrowFromRowIndex(int index, float newValue, float oldValue, bool showStar)
	{
		UIDataRow uIDataRow = ((statsRows != null && statsRows.Length > index) ? statsRows[index] : null);
		if (uIDataRow != null && uIDataRow.gameObject.activeSelf && uIDataRow.spriteArray != null && uIDataRow.spriteArray.Length > 1)
		{
			SetupStatEntry(newValue, oldValue, uIDataRow.spriteArray[0], uIDataRow.spriteArray[1], showStar);
		}
	}

	private void SetupStatEntry(float newValue, float oldValue, UISprite starSprite, UISprite arrowSprite, bool showStar)
	{
		SetupArrow(newValue, oldValue, arrowSprite);
		if (starSprite != null)
		{
			starSprite.gameObject.SetActive(showStar);
		}
	}

	private void SetupArrow(float newValue, float oldValue, UISprite arrowSprite)
	{
		if (arrowSprite != null)
		{
			arrowSprite.gameObject.SetActive(newValue != oldValue);
			if (newValue > oldValue)
			{
				arrowSprite.color = StatsArrowUpColor;
				arrowSprite.flip = UIBasicSprite.Flip.Vertically;
			}
			else if (newValue < oldValue)
			{
				arrowSprite.color = StatsArrowDownColor;
				arrowSprite.flip = UIBasicSprite.Flip.Nothing;
			}
		}
	}

	private void OnDataButtonClicked(UIButtonExtended button)
	{
		SwitchMode();
		UpdateUI();
	}

	private void SwitchMode()
	{
		personal = !(GameManager.Instance != null) || GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.GuildModel == null || !personal;
	}
}
