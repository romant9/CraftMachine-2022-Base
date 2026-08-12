using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class UIGuildBattleVictoryPointsProgressBar : UIProgressBarExtended
{
	[SerializeField]
	private UILabel guildPointAmountLabel;

	[SerializeField]
	private UILabel enemyGuildPointAmountLabel;

	[SerializeField]
	private UIButtonExtended infoButton;

	private bool isGuildMember;

	[SerializeField]
	private float refreshInterval = 1f;

	private float refreshTimer;

	private void Awake()
	{
		DebugIdString = "UIGuildBattleVictoryPointsProgressBar";
	}

	private void SubscribeForEvents()
	{
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.CurrentBattle.Changed -= OnModelChanged;
			guildWarModel.CurrentBattle.Changed += OnModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
		EventManager.OnEvent += OnEvent;
	}

	private void OnModelChanged(TWDGroupModelChild twdGroupModelChild, string changed, object args)
	{
		if (changed == "GuildBattleScoresUpdated")
		{
			UpdateScores();
		}
	}

	private void OnEvent(EventManager.EventType eventType, object parameter)
	{
		if (eventType == EventManager.EventType.GroupModelLoaded)
		{
			SubscribeForEvents();
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		HelpersUI.SetContentToLabel(guildPointAmountLabel, "-");
		HelpersUI.SetContentToLabel(enemyGuildPointAmountLabel, "-");
		isGuildMember = GuildWarHelper.IsGuildMember();
		if (isGuildMember)
		{
			if (infoButton != null)
			{
				infoButton.SetClickCallback(OnClickInfo);
			}
			UpdateUI();
			UpdateScores();
			SubscribeForEvents();
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (infoButton != null)
		{
			infoButton.Clear();
		}
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null)
		{
			guildWarModel.Changed -= OnModelChanged;
		}
		EventManager.OnEvent -= OnEvent;
	}

	public override void Update()
	{
		base.Update();
		if (OfflineManager.IsLoadDataManager) return;
		refreshTimer -= Time.deltaTime;
		if (refreshTimer <= 0f)
		{
			SingularityMonoBehaviour<GuildWarManager>.Instance.RequestBattleHighscoresUpdate();
			refreshTimer = refreshInterval;
		}
	}

	public void OnClickInfo(UIButtonExtended button)
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup);
		if (noCreation != null && noCreation.IsOpen)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GvGStartBattleFlowPopup, Helpers.GetUIParent(this.gameObject, true)) as GvGStartBattleFlowPopup).Open();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GuildWarHelper.GetGuildWarModel() != null)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}

	public void SetVictoryPoints(int vp, int enemyVp)
	{
		int num = enemyVp + vp;
		HelpersUI.SetContentToLabel(guildPointAmountLabel, (vp < 0) ? "-" : vp.ToString());
		HelpersUI.SetContentToLabel(enemyGuildPointAmountLabel, (enemyVp < 0) ? "-" : enemyVp.ToString());
		if (progressBar != null)
		{
			if (num > 0)
			{
				progressBar.value = (float)vp / (float)num;
			}
			else
			{
				progressBar.value = 0.5f;
			}
		}
	}

	public void UpdateScores()
	{
		List<ScoreDataEntry> guildScores = GuildWarHelper.GetGuildWarModel().CurrentBattle.UpdateGuildScrores();
		if (guildScores == null)
		{
			SetVictoryPoints(-1, -1);
			return;
		}
		int vp = 0;
		int enemyVp = 0;
		for (int i = 0; i < (guildScores?.Count ?? 0); i++)
		{
			ScoreDataEntry scoreDataEntry = guildScores[i];
			if (GuildWarHelper.IsOwnGuild(scoreDataEntry.Id))
			{
				vp = (int)Math.Min(scoreDataEntry.Score, 2147483647L);
			}
			else
			{
				enemyVp = (int)Math.Min(scoreDataEntry.Score, 2147483647L);
			}
		}
		SetVictoryPoints(vp, enemyVp);
	}

	public void CurrencyTweenAnimationComplete(bool iscomplete, CurrencyType currencytype)
	{
		TweenManager.PlayTweenGroup(base.gameObject, 3);
	}
}
