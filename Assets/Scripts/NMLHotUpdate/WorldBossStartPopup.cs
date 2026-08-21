using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class WorldBossStartPopup : HUDElement
{
	[SerializeField]
	private GameObject infoContainer;

	[SerializeField]
	private GameObject bossBG;

	[SerializeField]
	private UILabel bossTitleLabel;

	[SerializeField]
	private GameObject loadBG;

	[SerializeField]
	private UILabel loadTitleLabel;

	[SerializeField]
	private UILabel tankNameLabel;

	[SerializeField]
	private UILabel bossDefenseLabel;

	[SerializeField]
	private UILabel bossPlusLabel;

	[SerializeField]
	private UILabel dailyDescLabel;

	[SerializeField]
	private GameObject bossDefenseDescContainer;

	[SerializeField]
	private GameObject bossScoreMultiplierDescContainer;

	private bool canGo;

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public void ClickGo()
	{
		if (!canGo)
		{
			return;
		}
		PlayerModel obj = GameManager.Instance?.playerModel;
		WorldBossModelManager worldBossModelManager = obj?.WorldBossModelManager;
		if (obj == null || worldBossModelManager == null)
		{
			return;
		}
		WorldBossBattlegroundDefinition def = GameManager.Instance.gameEconomyData.FindWorldBossBattlegroundDefinitionByCapturePoint("BOSS", worldBossModelManager.GetCurrentBattleDifficulty());
		WorldBossMissionType worldBossMissionType = WorldBossMissionType.BOSS;
		WorldBossMissionModel worldBossMissionModel = WorldBossMissionModel.Create(def, "BOSS", "", GameManager.Instance.gameEconomyData, worldBossMissionType);
		if (worldBossMissionModel != null && worldBossMissionModel.HasValidMissionBinding())
		{
			TeamSelectionPopup teamSelectionPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
			if (!(teamSelectionPopup == null))
			{
				teamSelectionPopup.OpenForWorldBoss(worldBossMissionModel, SurvivorContainerModel.SurvivorType.WorldBoss);
				EventManager.NotifyClick("SelectTeam");
				Close();
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(infoContainer, value: true);
		WorldBossModelManager worldBossModelManager = GameManager.Instance?.playerModel?.WorldBossModelManager;
		if (worldBossModelManager == null)
		{
			return;
		}
		if (bossTitleLabel != null || loadTitleLabel != null || tankNameLabel != null)
		{
			ActorDefinition bossActorDefinition = worldBossModelManager.GetBossActorDefinition("BOSS");
			if (bossActorDefinition != null)
			{
				if (tankNameLabel != null && !string.IsNullOrEmpty(bossActorDefinition.Name))
				{
					HelpersUI.SetContentToLabel(tankNameLabel, LocalizationManager.GetText(bossActorDefinition.Name));
				}
				ApplyBossTagDisplays(bossActorDefinition.TagDisplay);
			}
		}
		if (bossDefenseLabel != null)
		{
			HelpersUI.SetContentToLabel(bossDefenseLabel, worldBossModelManager.GetBossDefense("BOSS").ToString());
		}
		if (bossPlusLabel != null)
		{
			double myTowerBBossScoreMultiplier = worldBossModelManager.GetMyTowerBBossScoreMultiplier();
			int num = (int)Math.Max(0.0, Math.Round((myTowerBBossScoreMultiplier - 1.0) * 100.0));
			HelpersUI.SetContentToLabel(bossPlusLabel, num + "%");
		}
		if (dailyDescLabel != null)
		{
			long remainingBossBattleTimes = worldBossModelManager.GetRemainingBossBattleTimes();
			long dailyBossBattleLimit = worldBossModelManager.GetDailyBossBattleLimit();
			canGo = remainingBossBattleTimes > 0;
			HelpersUI.SetContentToLabel(dailyDescLabel, LocalizationManager.GetText("World.Boss.BOSS.DailyAttempte", remainingBossBattleTimes, dailyBossBattleLimit));
			dailyDescLabel.color = ((remainingBossBattleTimes == 0L) ? Helpers.HexToColor("#fd3535") : Color.white);
		}
	}

	private void ApplyBossTagDisplays(List<string> tagDisplay)
	{
		Helpers.GameObjectSetActive(bossBG, value: false);
		Helpers.GameObjectSetActive(bossTitleLabel, value: false);
		Helpers.GameObjectSetActive(loadBG, value: false);
		Helpers.GameObjectSetActive(loadTitleLabel, value: false);
		if (tagDisplay == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < tagDisplay.Count; i++)
		{
			string text = tagDisplay[i];
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			string[] array = text.Split(':');
			if (array.Length < 2)
			{
				continue;
			}
			string textId = array[0];
			Color color = Helpers.HexToColor("#" + array[1].TrimStart('#'));
			switch (num)
			{
			case 0:
				Helpers.GameObjectSetActive(bossBG, value: true);
				Helpers.GameObjectSetActive(bossTitleLabel, value: true);
				SetGameObjectSpriteColor(bossBG, color);
				HelpersUI.SetContentToLabel(bossTitleLabel, LocalizationManager.GetText(textId));
				if (bossTitleLabel != null)
				{
					bossTitleLabel.color = color;
				}
				break;
			case 1:
				Helpers.GameObjectSetActive(loadBG, value: true);
				Helpers.GameObjectSetActive(loadTitleLabel, value: true);
				SetGameObjectSpriteColor(loadBG, color);
				HelpersUI.SetContentToLabel(loadTitleLabel, LocalizationManager.GetText(textId));
				if (loadTitleLabel != null)
				{
					loadTitleLabel.color = color;
				}
				break;
			default:
				return;
			}
			num++;
		}
	}

	private static void SetGameObjectSpriteColor(GameObject go, Color color)
	{
		if (!(go == null))
		{
			UISprite component = go.GetComponent<UISprite>();
			if (component != null)
			{
				component.color = color;
			}
		}
	}

	public void OnClickBossDefenseDesc()
	{
		Helpers.GameObjectSetActive(bossDefenseDescContainer, value: true);
		OnCloseBossScoreMultiplierDesc();
	}

	public void OnCloseBossDefenseDesc()
	{
		Helpers.GameObjectSetActive(bossDefenseDescContainer, value: false);
	}

	public void OnClickBossScoreMultiplierDesc()
	{
		Helpers.GameObjectSetActive(bossScoreMultiplierDescContainer, value: true);
		OnCloseBossDefenseDesc();
	}

	public void OnCloseBossScoreMultiplierDesc()
	{
		Helpers.GameObjectSetActive(bossScoreMultiplierDescContainer, value: false);
	}
}
