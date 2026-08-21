using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class WorldBossRetreatPopup : HUDElement
{
	[Header("Bundle Items List")]
	[SerializeField]
	private UIButton retreatButton;

	[SerializeField]
	private GameObject teamDesGM;

	private WorldBossModelManager worldBossModelManager;

	private readonly long[] _dispatchedTeamOccupiedAtUtcMs = new long[2] { -1L, -1L };

	private const string HeroTokenNamePrefix = "HeroToken";

	private const int DurabilityBlockNum = 10;

	private const string DurabilityBlockNamePrefix = "Icon";

	private static readonly Color DurabilityColorGreen = Helpers.HexToColor("#a0c92f");

	private static readonly Color DurabilityColorYellow = Helpers.HexToColor("#f7c225");

	private static readonly Color DurabilityColorRed = Helpers.HexToColor("#fd3535");

	private List<WorldBossDispatchedTeamView> dispatchedTeams = new List<WorldBossDispatchedTeamView>();

	public override void Open()
	{
		base.Open();
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		dispatchedTeams = worldBossModelManager?.GetMyDispatchedTeams() ?? new List<WorldBossDispatchedTeamView>();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateTeamInfo();
	}

	public void OnClickRetreatButton1()
	{
		if (dispatchedTeams.Count > 0)
		{
			OpenRetreatConfirm(dispatchedTeams[0]);
		}
	}

	public void OnClickRetreatButton2()
	{
		if (dispatchedTeams.Count > 1)
		{
			OpenRetreatConfirm(dispatchedTeams[1]);
		}
	}

	private void OpenRetreatConfirm(WorldBossDispatchedTeamView team)
	{
		WorldBossRetreatConfirmPopup worldBossRetreatConfirmPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossRetreatConfirmPopup) as WorldBossRetreatConfirmPopup;
		if (!(worldBossRetreatConfirmPopup == null) && team != null)
		{
			worldBossRetreatConfirmPopup.SetTeam(team);
			worldBossRetreatConfirmPopup.Open();
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public void UpdateTeamInfo()
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		int num = worldBossModelManager?.GetMyDispatchedTeamCount() ?? 0;
		worldBossModelManager?.GetDispatchTeamLimit();
		SetDispatchedTeamSlotState("TeamInfo1", num >= 1);
		SetDispatchedTeamSlotState("TeamInfo2", num >= 2);
		_dispatchedTeamOccupiedAtUtcMs[0] = -1L;
		_dispatchedTeamOccupiedAtUtcMs[1] = -1L;
		if (dispatchedTeams.Count >= 1)
		{
			FillDispatchedTeamInfo("TeamInfo1", dispatchedTeams[0], 0);
		}
		if (dispatchedTeams.Count >= 2)
		{
			FillDispatchedTeamInfo("TeamInfo2", dispatchedTeams[1], 1);
		}
	}

	private void FillDispatchedTeamInfo(string teamInfoName, WorldBossDispatchedTeamView team, int slotIndex)
	{
		if (!(teamDesGM == null) && team != null)
		{
			Transform transform = teamDesGM.transform.Find(teamInfoName + "/HaveTeamContent");
			if (!(transform == null))
			{
				UpdateDispatchedTeamHeroTokens(transform, team.SurvivorIds);
				HelpersUI.SetContentToLabel(transform.Find("TeamZone")?.GetComponent<UILabel>(), LocalizationManager.GetText(GetCapturePointDisplayName(team.CapturePoint)));
				UpdateDispatchedTeamZoneIcon(transform, team.CapturePoint);
				_dispatchedTeamOccupiedAtUtcMs[slotIndex] = team.OccupiedAtUtcMs;
				UpdateDispatchedTeamTimeLabel(transform, slotIndex, GetSynchronizedUtcNowMs());
				UpdateDispatchedTeamDurabilityBlocks(transform, team.DefenderRemainingDurability);
			}
		}
	}

	private void SetDispatchedTeamSlotState(string teamInfoName, bool hasTeam)
	{
		if (!(teamDesGM == null))
		{
			Transform transform = teamDesGM.transform.Find(teamInfoName);
			if (!(transform == null))
			{
				Helpers.GameObjectSetActive(transform.gameObject, value: true);
				Transform transform2 = transform.Find("HaveTeamContent");
				Transform obj = transform.Find("HaveNoTeamContent");
				Helpers.GameObjectSetActive(transform2?.gameObject, hasTeam);
				Helpers.GameObjectSetActive(obj?.gameObject, !hasTeam);
			}
		}
	}

	private void UpdateDispatchedTeamHeroTokens(Transform haveTeamContent, List<string> survivorAnalyticsIds)
	{
		for (int i = 0; i < 3; i++)
		{
			Transform transform = haveTeamContent.Find("HeroToken" + (i + 1));
			if (transform == null)
			{
				continue;
			}
			bool flag = survivorAnalyticsIds != null && i < survivorAnalyticsIds.Count && !string.IsNullOrEmpty(survivorAnalyticsIds[i]);
			Helpers.GameObjectSetActive(transform.gameObject, flag);
			if (flag)
			{
				SurvivorModel survivorByAnalyticsId = GetSurvivorByAnalyticsId(survivorAnalyticsIds[i]);
				UISprite component = transform.GetComponent<UISprite>();
				if (survivorByAnalyticsId != null && !(component == null))
				{
					component.spriteName = HelpersGfx.GetCurrencyIconName(SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivorByAnalyticsId));
				}
			}
		}
	}

	private void UpdateDispatchedTeamDurabilityBlocks(Transform haveTeamContent, int defenderRemainingDurability)
	{
		Transform transform = haveTeamContent?.Find("BlockUp");
		if (transform == null)
		{
			return;
		}
		int num = Mathf.Clamp(defenderRemainingDurability, 0, 10);
		Color dispatchedTeamDurabilityColor = GetDispatchedTeamDurabilityColor(num);
		for (int i = 0; i < 10; i++)
		{
			int num2 = i + 1;
			Transform transform2 = transform.Find("Icon" + num2);
			if (transform2 == null)
			{
				continue;
			}
			bool flag = num2 <= num;
			Helpers.GameObjectSetActive(transform2.gameObject, flag);
			if (flag)
			{
				UIButton component = transform2.GetComponent<UIButton>();
				if (component != null)
				{
					component.defaultColor = dispatchedTeamDurabilityColor;
					component.hover = dispatchedTeamDurabilityColor;
					component.pressed = dispatchedTeamDurabilityColor;
					component.disabledColor = dispatchedTeamDurabilityColor;
					component.enabled = false;
				}
				TweenColor component2 = transform2.GetComponent<TweenColor>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
				UISprite component3 = transform2.GetComponent<UISprite>();
				if (component3 != null)
				{
					component3.color = dispatchedTeamDurabilityColor;
				}
			}
		}
	}

	private static Color GetDispatchedTeamDurabilityColor(int durabilityLine)
	{
		if (durabilityLine >= 7)
		{
			return DurabilityColorGreen;
		}
		if (durabilityLine >= 4)
		{
			return DurabilityColorYellow;
		}
		return DurabilityColorRed;
	}

	private void UpdateDispatchedTeamTimers()
	{
		if (teamDesGM == null)
		{
			return;
		}
		long synchronizedUtcNowMs = GetSynchronizedUtcNowMs();
		for (int i = 0; i < _dispatchedTeamOccupiedAtUtcMs.Length; i++)
		{
			if (_dispatchedTeamOccupiedAtUtcMs[i] > 0)
			{
				string text = ((i == 0) ? "TeamInfo1" : "TeamInfo2");
				Transform transform = teamDesGM.transform.Find(text + "/HaveTeamContent");
				if (!(transform == null) && transform.gameObject.activeInHierarchy)
				{
					UpdateDispatchedTeamTimeLabel(transform, i, synchronizedUtcNowMs);
				}
			}
		}
	}

	private void UpdateDispatchedTeamTimeLabel(Transform haveTeamContent, int slotIndex, long now)
	{
		if (haveTeamContent == null || slotIndex < 0 || slotIndex >= _dispatchedTeamOccupiedAtUtcMs.Length)
		{
			return;
		}
		UILabel uILabel = haveTeamContent.Find("TeamTime")?.GetComponent<UILabel>();
		if (!(uILabel == null))
		{
			long num = _dispatchedTeamOccupiedAtUtcMs[slotIndex];
			if (num <= 0)
			{
				HelpersUI.SetContentToLabel(uILabel, "00:00:00");
				return;
			}
			long milliSeconds = ((now > num) ? (now - num) : 0);
			HelpersUI.SetContentToLabel(uILabel, Helpers.FormatTimeAsHms(milliSeconds));
		}
	}

	private static long GetSynchronizedUtcNowMs()
	{
		return (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault() / 1000 * 1000;
	}

	private string GetCapturePointDisplayName(string capturePoint)
	{
		if (string.IsNullOrEmpty(capturePoint))
		{
			return string.Empty;
		}
		int difficultyLevel = worldBossModelManager?.GetCurrentBattleDifficulty() ?? 0;
		WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = GameManager.Instance?.gameEconomyData?.FindWorldBossBattlegroundDefinitionByCapturePoint(capturePoint, difficultyLevel);
		if (worldBossBattlegroundDefinition != null && !string.IsNullOrEmpty(worldBossBattlegroundDefinition.BuildingName))
		{
			return worldBossBattlegroundDefinition.BuildingName;
		}
		return capturePoint;
	}

	private static void UpdateDispatchedTeamZoneIcon(Transform haveTeamContent, string capturePoint)
	{
		if (haveTeamContent == null || string.IsNullOrEmpty(capturePoint))
		{
			return;
		}
		UISprite uISprite = haveTeamContent.Find("TeamZoneIcon")?.GetComponent<UISprite>();
		if (!(uISprite == null))
		{
			switch (capturePoint)
			{
			case "TOWER-A":
			case "TOWER-B":
				uISprite.spriteName = "Ui_Icon_Outpost";
				break;
			case "DEPOT":
				uISprite.spriteName = "Ui_Icon_Tents";
				break;
			}
		}
	}

	private static SurvivorModel GetSurvivorByAnalyticsId(string analyticsId)
	{
		if (string.IsNullOrEmpty(analyticsId))
		{
			return null;
		}
		ModelList<SurvivorModel> modelList = GameManager.Instance?.playerModel?.SurvivorContainer?.Survivors;
		if (modelList == null)
		{
			return null;
		}
		for (int i = 0; i < modelList.Count; i++)
		{
			if (modelList[i].IdForAnalytics == analyticsId)
			{
				return modelList[i];
			}
		}
		return null;
	}

	private static string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	public void OnRetreatButtonClicked()
	{
		if (dispatchedTeams.Count > 0)
		{
			OpenRetreatConfirm(dispatchedTeams[0]);
		}
	}
}
