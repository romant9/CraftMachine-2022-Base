using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class WorldBossRetreatConfirmPopup : HUDElement
{
	[Header("Bundle Items List")]
	[SerializeField]
	private UIButton retreatButton;

	[SerializeField]
	private GameObject teamDesGM;

	private WorldBossModelManager worldBossModelManager;

	private WorldBossDispatchedTeamView _dispatchedTeam;

	private long _occupiedAtUtcMs = -1L;

	private const string HeroTokenNamePrefix = "HeroToken";

	private const int DurabilityBlockNum = 10;

	private const string DurabilityBlockNamePrefix = "Icon";

	private static readonly Color DurabilityColorGreen = Helpers.HexToColor("#a0c92f");

	private static readonly Color DurabilityColorYellow = Helpers.HexToColor("#f7c225");

	private static readonly Color DurabilityColorRed = Helpers.HexToColor("#fd3535");

	public void SetTeam(WorldBossDispatchedTeamView team)
	{
		_dispatchedTeam = team;
	}

	public void OnRetreatButtonClick()
	{
		if (_dispatchedTeam != null)
		{
			if (worldBossModelManager == null)
			{
				worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
			}
			if (worldBossModelManager != null && worldBossModelManager.WorldBossGuildFullSnapshot != null)
			{
				int currentSeasonId = worldBossModelManager.GetCurrentSeasonId();
				int currentCycleId = worldBossModelManager.GetCurrentCycleId();
				Helpers.ExecuteCommand(new WithdrawWorldBossCellCommand(currentSeasonId, currentCycleId, _dispatchedTeam.CapturePoint, _dispatchedTeam.Cell, _dispatchedTeam.SurvivorIds));
				CloseRetreatPopupIfOpen();
				Close();
			}
		}
	}

	private static void CloseRetreatPopupIfOpen()
	{
		WorldBossRetreatPopup worldBossRetreatPopup = SingularityMonoBehaviour<HUDManager>.Instance?.Get(UIType.WorldBossRetreatPopup) as WorldBossRetreatPopup;
		if (worldBossRetreatPopup != null && worldBossRetreatPopup.IsOpen)
		{
			worldBossRetreatPopup.Close();
		}
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateTeamInfo();
	}

	public override void Update()
	{
		base.Update();
	}

	private void UpdateTeamInfo()
	{
		if (worldBossModelManager == null)
		{
			worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		}
		_occupiedAtUtcMs = -1L;
		if (_dispatchedTeam != null && !(teamDesGM == null))
		{
			Transform transform = teamDesGM.transform.Find("TeamInfo1/HaveTeamContent");
			if (!(transform == null))
			{
				Helpers.GameObjectSetActive(transform.gameObject, value: true);
				Helpers.GameObjectSetActive(teamDesGM.transform.Find("TeamInfo1/HaveNoTeamContent")?.gameObject, value: false);
				UpdateDispatchedTeamHeroTokens(transform, _dispatchedTeam.SurvivorIds);
				HelpersUI.SetContentToLabel(transform.Find("TeamZone")?.GetComponent<UILabel>(), LocalizationManager.GetText(GetCapturePointDisplayName(_dispatchedTeam.CapturePoint)));
				UpdateDispatchedTeamZoneIcon(transform, _dispatchedTeam.CapturePoint);
				_occupiedAtUtcMs = _dispatchedTeam.OccupiedAtUtcMs;
				UpdateDispatchedTeamTimeLabel(transform);
				UpdateDispatchedTeamDurabilityBlocks(transform, _dispatchedTeam.DefenderRemainingDurability);
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

	private void UpdateDispatchedTeamTimer()
	{
		if (_occupiedAtUtcMs > 0 && !(teamDesGM == null))
		{
			Transform transform = teamDesGM.transform.Find("TeamInfo1/HaveTeamContent");
			if (!(transform == null) && transform.gameObject.activeInHierarchy)
			{
				UpdateDispatchedTeamTimeLabel(transform);
			}
		}
	}

	private void UpdateDispatchedTeamTimeLabel(Transform haveTeamContent)
	{
		UILabel uILabel = haveTeamContent?.Find("TeamTime")?.GetComponent<UILabel>();
		if (!(uILabel == null))
		{
			if (_occupiedAtUtcMs <= 0)
			{
				HelpersUI.SetContentToLabel(uILabel, "00:00:00");
				return;
			}
			long valueOrDefault = (GameManager.Instance?.playerModel?.UtcTimeStamp).GetValueOrDefault();
			long milliSeconds = ((valueOrDefault > _occupiedAtUtcMs) ? (valueOrDefault - _occupiedAtUtcMs) : 0);
			HelpersUI.SetContentToLabel(uILabel, Helpers.FormatTimeAsHms(milliSeconds));
		}
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
}
