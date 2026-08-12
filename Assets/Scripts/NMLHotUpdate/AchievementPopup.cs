using System.Collections;
using UnityEngine;

public class AchievementPopup : HUDElement
{
	[SerializeField]
	private GameObject GoogleAchievementParent;

	[SerializeField]
	private UITabs tabs;

	public static void OpenAchievement()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AchievementPopup).Open();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/achievement_open");
	}

	public override void Start()
	{
		base.Start();
		UpdateGoogleAchievementButton();
	}

	public void OnGooglePlayAchievements()
	{
		GameManager.Instance.GameCenterManager.OpenSystemDefaultAchievementsUI();
	}

	public override void Open()
	{
		base.Open();
		tabs.OnNewTabSelectedEvent += OnTabSelected;
	}

	public override void Close()
	{
		base.Close();
		tabs.OnNewTabSelectedEvent -= OnTabSelected;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/achievement_close");
	}

	protected IEnumerator OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			yield return null;
			yield return null;
			UpdateGoogleAchievementButton();
		}
	}

	private void UpdateGoogleAchievementButton()
	{
		Helpers.GameObjectSetActive(GoogleAchievementParent, value: false);
	}

	public void SelectTabAchievements()
	{
		tabs.SelectTab(0);
	}

	public void SelectTabDailyQuests()
	{
		tabs.SelectTab(1);
	}

	private void OnTabSelected(int tabindex)
	{
		GameObject content = tabs.GetContent(tabindex);
		if (content != null)
		{
			switch (tabindex)
			{
			case 0:
				content.GetComponent<AchievementListPanel>().Init();
				break;
			case 1:
				content.GetComponent<DailyQuestListPanel>().Init();
				break;
			}
		}
	}
}
