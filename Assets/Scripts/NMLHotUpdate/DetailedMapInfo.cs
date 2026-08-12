using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class DetailedMapInfo : MonoBehaviour
{
	[SerializeField]
	private GameObject container;

	[SerializeField]
	private UILabel missionsCompletedLabel;

	[SerializeField]
	private UIProgressBar missionsCompletedProgressBar;

	[SerializeField]
	private GameObject progressBarContainer;

	[SerializeField]
	private UILabel episodeNumberLabel;

	[SerializeField]
	private UILabel episodeNameLabel;

	[SerializeField]
	private UIButtonExtended episodeNameButton;

	[Header("Episode map")]
	[SerializeField]
	private GameObject episodeContainer;

	[SerializeField]
	private GameObject completedContainer;

	[Header("Harder Episodes")]
	[SerializeField]
	private GameObject episodeDifficultyContainer;

	[SerializeField]
	private UILabel episodeDifficultyLabel;

	[SerializeField]
	private GameObject[] episodeDifficultyCompleted;

	[Header("Challenge map")]
	[SerializeField]
	private GameObject challengeContainer;

	[SerializeField]
	private UILabel timeLeftLabel;

	private bool isWeeklyChallenge;

	private long timeToEndChallenge;

	private MapMissionModel lastPlayedMissionModel;

	private MapMissionGroupModel currentGroupModel;

	[SerializeField]
	private GameObject hardModeStarBg;

	[SerializeField]
	private TweenScale[] normalModeStarTweens;

	[SerializeField]
	private TweenScale[] hardModeStarTweens;

	[SerializeField]
	private TweenScale[] nightmareModeStarTweens;

	[SerializeField]
	private float secondsBetweenStars = 0.5f;

	private List<GameObject> starsToActivate;

	public void OnEnable()
	{
		lastPlayedMissionModel = GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel;
		starsToActivate = new List<GameObject>();
		Helpers.GameObjectSetActive(timeLeftLabel, value: false);
		AddListerens();
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	public void OnDisable()
	{
		RemoveListerens();
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		episodeNumberLabel.text = HelpersLocalization.GetEpisodeTitle(currentGroupModel);
	}

	public void AddListerens()
	{
		if (episodeNameButton != null)
		{
			episodeNameButton.SetClickCallback(OnClickEpisodeName);
		}
	}

	public void RemoveListerens()
	{
		if (episodeNameButton != null)
		{
			episodeNameButton.Clear();
		}
	}

	public void SetCurrentMissionGroup(MapMissionGroupModel model)
	{
		currentGroupModel = model;
	}

	public void UpdateUI()
	{
		if (currentGroupModel == null)
		{
			return;
		}
		isWeeklyChallenge = currentGroupModel.IsWeeklyChallenge || currentGroupModel.IsInApocalyptiWeeklyChallenge;
		episodeContainer.SetActive(!isWeeklyChallenge);
		challengeContainer.SetActive(isWeeklyChallenge);
		episodeNumberLabel.text = HelpersLocalization.GetEpisodeTitle(currentGroupModel);
		if (isWeeklyChallenge)
		{
			if (GameManager.Instance.playerModel.WeeklyChallenge.Finished)
			{
				timeToEndChallenge = 0L;
			}
			else
			{
				timeToEndChallenge = GameManager.Instance.playerModel.WeeklyChallenge.CurrentDefinition.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
			}
			return;
		}
		episodeNameLabel.text = HelpersLocalization.GetEpisodeName(currentGroupModel.MissionSpawnPointGroup);
		MapMissionGroupModel currentEpisodeDifficultyGroupModel = currentGroupModel.GetCurrentEpisodeDifficultyGroupModel();
		MissionSpawnPointGroup missionSpawnPointGroup = currentEpisodeDifficultyGroupModel.MissionSpawnPointGroup;
		bool flag = currentGroupModel.AreAllStoryMissionsCompleted();
		bool flag2 = GameManager.Instance.playerModel.MapContainerModel.GetHarderVersion(currentEpisodeDifficultyGroupModel) == null;
		if (completedContainer != null)
		{
			completedContainer.SetActive(flag && currentGroupModel.GetNonCompletedMissionsCount() == 0 && flag2);
		}
		bool num = missionSpawnPointGroup.EpisodeDifficultyLevel != 1;
		bool flag3 = flag && currentGroupModel.GetNonCompletedMissionsCount() == 0;
		bool flag4 = num || (flag2 && flag3);
		episodeDifficultyContainer.SetActive(flag4);
		if (flag4)
		{
			episodeDifficultyLabel.text = LocalizationManager.GetText("Episode.DifficultyLevel." + missionSpawnPointGroup.EpisodeDifficultyLevel);
			for (int i = 0; i < episodeDifficultyCompleted.Length; i++)
			{
				episodeDifficultyCompleted[i].SetActive(value: false);
			}
			int num2 = missionSpawnPointGroup.EpisodeDifficultyLevel - 2;
			if (flag)
			{
				num2++;
			}
			episodeDifficultyCompleted[num2].SetActive(value: true);
			hardModeStarBg.SetActive(!flag3);
			if (lastPlayedMissionModel != null && currentEpisodeDifficultyGroupModel.MissionSpawnPointGroupId != lastPlayedMissionModel.MissionSpawnPointGroupId && GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(lastPlayedMissionModel.MissionSpawnPointGroupId).AreAllStoryMissionsCompleted())
			{
				StartCoroutine(TweenStars(num2));
			}
		}
		int num3 = currentGroupModel.GetNumberCompletedStoryMissions();
		int numberStoryMissions = currentGroupModel.GetNumberStoryMissions();
		bool active = !flag4;
		progressBarContainer.SetActive(active);
		if (lastPlayedMissionModel != null && lastPlayedMissionModel.IsCompleted && !currentGroupModel.IsWeeklyChallenge && !currentGroupModel.IsInApocalyptiWeeklyChallenge && !flag)
		{
			lastPlayedMissionModel = null;
			TweenProgressBar component = missionsCompletedProgressBar.GetComponent<TweenProgressBar>();
			if (component != null)
			{
				num3--;
				component.From = (float)num3 / (float)numberStoryMissions;
				component.To = (float)(num3 + 1) / (float)numberStoryMissions;
				component.delay = 2f;
				component.enabled = true;
				component.AddOnFinished(OnProgressBarAnimationDone);
			}
			else
			{
				OnProgressBarAnimationDone();
			}
		}
		missionsCompletedLabel.text = LocalizationManager.GetText("Map.MissionsCompleted{Number}{Total}", num3, numberStoryMissions);
		missionsCompletedProgressBar.value = (float)num3 / (float)numberStoryMissions;
	}

	private void OnProgressBarAnimationDone()
	{
		UpdateUI();
	}

	private void Update()
	{
		if (timeToEndChallenge > 0)
		{
			timeToEndChallenge -= (long)(Time.deltaTime * 1000f);
			timeLeftLabel.text = LocalizationManager.GetText("Map.WeeklyChallenge.EndsIn{Time}", Helpers.FormatTimeNoZero(timeToEndChallenge));
			if (timeToEndChallenge <= 0)
			{
				ChallengeOver();
			}
		}
		Helpers.GameObjectSetActive(timeLeftLabel, timeToEndChallenge > 0);
	}

	private void ChallengeOver()
	{
		AlertPopup.ShowPopupGetText("Popup.ChallengeEnded.Title", "Popup.ChallengeEnded.Description", "Button.Ok", ReloadChallengeMap);
	}

	private void ReloadChallengeMap()
	{
		DetailMapPopUp.ReloadChallengeMap();
	}

	public void OnClickProgressBar()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.QuestPopup).OpenForModel(GameManager.Instance.playerModel.SurvivorContainer.StoryTeller);
	}

	private void OnClickEpisodeName(UIButtonExtended button)
	{
	}

	private IEnumerator TweenStars(int emblemIndex)
	{
		starsToActivate.Clear();
		switch (emblemIndex)
		{
		case 0:
		{
			for (int j = 0; j < normalModeStarTweens.Length; j++)
			{
				normalModeStarTweens[j].gameObject.SetActive(value: false);
				starsToActivate.Add(normalModeStarTweens[j].gameObject);
				normalModeStarTweens[j].enabled = true;
			}
			break;
		}
		case 1:
		{
			for (int k = 0; k < hardModeStarTweens.Length; k++)
			{
				hardModeStarTweens[k].gameObject.SetActive(value: false);
				starsToActivate.Add(hardModeStarTweens[k].gameObject);
				hardModeStarTweens[k].enabled = true;
			}
			break;
		}
		case 2:
		{
			for (int i = 0; i < nightmareModeStarTweens.Length; i++)
			{
				nightmareModeStarTweens[i].gameObject.SetActive(value: false);
				starsToActivate.Add(nightmareModeStarTweens[i].gameObject);
				nightmareModeStarTweens[i].enabled = true;
			}
			break;
		}
		}
		for (int l = 0; l < starsToActivate.Count; l++)
		{
			if (l % 2 == 0)
			{
				yield return new WaitForSeconds(secondsBetweenStars);
			}
			starsToActivate[l].SetActive(value: true);
		}
	}
}
