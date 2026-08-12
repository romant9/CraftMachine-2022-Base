using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class OpenGvgDefendersButton : MonoBehaviour
{
	[SerializeField]
	private GameObject newIndicator;

	private UIButton button;

	private EventDelegate eventHandler;

	private void Awake()
	{
		button = GetComponent<UIButton>();
		eventHandler = new EventDelegate(OnClickEventHandler);
	}

	private void OnEnable()
	{
		newIndicator.SetActive(PlayerPrefs.GetInt("OpenedGvgDefenders", 0) == 0);
		button.onClick.Add(eventHandler);
	}

	private void OnDisable()
	{
		button.onClick.Remove(eventHandler);
	}

	private void OnClickEventHandler()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleOverviewPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.SocialPopupGuild);
		TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
		obj.SurvivorType = SurvivorContainerModel.SurvivorType.GvGDefenders;
		obj.Open();
		PlayerPrefs.SetInt("OpenedGvgDefenders", 1);
	}
}
