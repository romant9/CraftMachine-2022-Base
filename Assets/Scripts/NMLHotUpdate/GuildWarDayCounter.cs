using UnityEngine;

public class GuildWarDayCounter : MonoBehaviour
{
	[SerializeField]
	private UILabel warDay;

	private const string warWeekLocalizationString = "GvG.SeasonInfo.WarWeek{Week}{NumOfWeeks}";

	private void OnEnable()
	{
		HelpersUI.SetContentToLabel(warDay, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.SeasonInfo.WarWeek{Week}{NumOfWeeks}", GuildWarHelper.GetActiveWarWeek(), GuildWarHelper.GetNumberOfWarsForActiveSeason()));
	}
}
