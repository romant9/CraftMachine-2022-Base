using System;
using TWDModel;
using UnityEngine;

public class BattlePassBattleCurrencyPanel : MonoBehaviour
{
	[SerializeField]
	private ShaderProgressBar shaderProgressBar;

	[SerializeField]
	private UILabel dailyKillCapProgressBarLabel;

	[SerializeField]
	private GameObject dailyCapRefreshLabelObject;

	[SerializeField]
	private UILabel dailyCapRefreshLabel;

	[SerializeField]
	private GameObject playMissionObject;

	[SerializeField]
	private UISprite battlepassIcon;

	[SerializeField]
	private GameObject premiumActiveContainer;

	private BattlePassModel battlePassModel;

	private bool isPremiumOn => GameManager.Instance.playerModel.BattlePass.PremiumActive;

	private void Awake()
	{
		battlePassModel = GameManager.Instance.playerModel.BattlePass;
	}

	private void OnEnable()
	{
		Open();
		battlepassIcon.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType.BattlePassPoints);
	}

	private void Open()
	{
		Helpers.GameObjectSetActive(premiumActiveContainer, isPremiumOn);
		FillDailyKillProgressBar();
	}

	private void Update()
	{
		dailyKillCapProgressBarLabel.text = $"{battlePassModel.EarnedFromKillsThisCycle}/{battlePassModel.MaxDailyBCFromKills}";
		long killCapExpiryDateMilliseconds = battlePassModel.KillCapExpiryDateMilliseconds;
		long utcTimeStamp = battlePassModel.manager.Player.UtcTimeStamp;
		bool flag = battlePassModel.EarnedFromKillsThisCycle >= battlePassModel.MaxDailyBCFromKills;
		Helpers.GameObjectSetActive(dailyCapRefreshLabelObject, flag);
		Helpers.GameObjectSetActive(playMissionObject, !flag);
		if (flag)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(killCapExpiryDateMilliseconds - utcTimeStamp);
			dailyCapRefreshLabel.text = $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
	}

	public void PlayMissionButtonClick()
	{
		if (TutorialView.Allowed("MissionHub") && GameManager.Instance.playerModel.SurvivorContainer.StoryTeller.FirstQuestAccepted)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/view_change");
			EventManager.NotifyClick(EventManager.EventTypeClick.MissionHub);
			if ((bool)CampView.Instance && (bool)CampView.Instance.CampViewBuildings)
			{
				CampView.Instance.CampViewBuildings.UnselectBuilding();
			}
			MissionHubPopup.OpenPopup();
			SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.CampBuildMenu);
		}
	}

	public void KillBarClicked(GameObject clickedObject)
	{
		TooltipManager.OpenTextBoxWithText(clickedObject, LocalizationManager.GetText("Tooltip.BattlePass.DailyKills.RadialBar"));
	}

	private void FillDailyKillProgressBar()
	{
		float to = (float)battlePassModel.EarnedFromKillsThisCycle / (float)battlePassModel.MaxDailyBCFromKills;
		shaderProgressBar.StartFill(to);
	}
}
