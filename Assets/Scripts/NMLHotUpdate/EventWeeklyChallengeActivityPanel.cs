using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EventWeeklyChallengeActivityPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject panelContainer;

	[SerializeField]
	private UISprite tokenIcon;

	[SerializeField]
	private UILabel tipsLabel;

	[SerializeField]
	private UISprite[] classIcons;

	[SerializeField]
	private UISprite[] checkIcons;

	[SerializeField]
	private GameObject effect;

	[SerializeField]
	private TeamSelectionSelectedSurvivorPanel selectedSurvivorPanel;

	private List<SurvivorModel> currentTeam;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "EventCurrentTeamUpdated")
		{
			currentTeam = parameter as List<SurvivorModel>;
			if (panelContainer.activeSelf)
			{
				UpdateSelectedClasses();
			}
		}
	}

	public void Init(MapMissionModel mapMissionModel)
	{
		if (!TutorialView.Instance.Running && mapMissionModel != null && mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.ApocalypticChallenge && GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.IsActive)
		{
			panelContainer.SetActive(value: true);
			List<SurvivorClass> classes = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.GetClasses();
			for (int i = 0; i < classIcons.Length; i++)
			{
				classIcons[i].spriteName = "UI_EventIcon_Class_" + classes[i];
			}
			tokenIcon.spriteName = HelpersGfx.GetCurrencyIconName(GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.StarCurrencyType);
			if (selectedSurvivorPanel != null)
			{
				currentTeam = selectedSurvivorPanel.GetCurrentTeam();
			}
			UpdateSelectedClasses();
		}
		else
		{
			panelContainer.SetActive(value: false);
		}
	}

	private void UpdateSelectedClasses()
	{
		UISprite[] array = checkIcons;
		for (int i = 0; i < array.Length; i++)
		{
			Helpers.GameObjectSetActive(array[i].gameObject, value: false);
		}
		List<SurvivorClass> classes = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.GetClasses();
		List<SurvivorModel> list = currentTeam;
		if (list == null || list.Count == 0 || classes == null || classes.Count != list.Count)
		{
			return;
		}
		List<SurvivorClass> list2 = new List<SurvivorClass>(classes);
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] != null && list2.IndexOf(list[j].SurvivorClass) != -1)
			{
				Helpers.GameObjectSetActive(checkIcons[j].gameObject, value: true);
				list2.Remove(list[j].SurvivorClass);
			}
		}
		string survivorClassName = HelpersLocalization.GetSurvivorClassName(classes[0]);
		HelpersUI.SetContentToLabel(tipsLabel, LocalizationManager.GetText("WeeklyChallengeClassTeamChallenge.PlayDesc", survivorClassName, survivorClassName));
		Helpers.GameObjectSetActive(effect, IsAllMissionRosterOfClass());
	}

	public void TipsButtonClicked()
	{
		WeeklyChallengeActivityInfo weeklyChallengeActivityInfo = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WeeklyChallengeActivityInfo) as WeeklyChallengeActivityInfo;
		if (weeklyChallengeActivityInfo != null)
		{
			weeklyChallengeActivityInfo.Open();
		}
	}

	private bool IsAllMissionRosterOfClass()
	{
		List<SurvivorClass> classes = GameManager.Instance.playerModel.WeeklyChallengeClassTeamActivity.CurrentDefinition.GetClasses();
		List<SurvivorModel> list = currentTeam;
		if (list == null || list.Count == 0 || classes == null)
		{
			return false;
		}
		if (classes.Count != list.Count)
		{
			return false;
		}
		List<SurvivorClass> list2 = new List<SurvivorClass>(classes);
		foreach (SurvivorModel item in list)
		{
			if (item == null || !list2.Remove(item.SurvivorClass))
			{
				return false;
			}
		}
		return list2.Count == 0;
	}
}
