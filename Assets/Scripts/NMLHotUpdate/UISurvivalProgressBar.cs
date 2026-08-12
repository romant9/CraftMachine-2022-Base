using UnityEngine;

public class UISurvivalProgressBar : UIProgressBarExtended
{
	private void Awake()
	{
		DebugIdString = "UISurvivalProgressBar";
	}

	public override void OnEnable()
	{
		base.OnEnable();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		int num = 0;
		int num2 = 0;
		if (WeeklySurvivalHelper.GetWeeklySurvivalModel() != null && WeeklySurvivalHelper.IsSurvivalOngoing() && WeeklySurvivalHelper.GetWeeklySurvivalModel().IsDifficultySelected)
		{
			num = WeeklySurvivalHelper.GetWeeklySurvivalModel().NumberCompleted;
			num2 = ((WeeklySurvivalHelper.GetWeeklySurvivalModel().CurrentDefinition == null) ? 1 : WeeklySurvivalHelper.GetWeeklySurvivalModel().CurrentDefinition.TotalMissionCount);
			if (progressBar != null)
			{
				progressBar.value = Mathf.InverseLerp(0f, num2, num);
			}
			if (num == num2)
			{
				HelpersUI.SetContentToLabel(progressBarLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.Survival.MissionProgressAllCompleted"));
			}
			else
			{
				HelpersUI.SetContentToLabel(progressBarLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.MissionHub.Survival.MissionProgress{Completed}{NumMissions}", num, num2));
			}
			Helpers.GameObjectSetActive(base.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}

	public override void Clear()
	{
		base.Clear();
	}
}
