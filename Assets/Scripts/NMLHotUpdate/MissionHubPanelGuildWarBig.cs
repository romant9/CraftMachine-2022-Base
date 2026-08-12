using BaseModel;
using TWDModel;
using UnityEngine;

public class MissionHubPanelGuildWarBig : MissionHubPanelBase
{
	[SerializeField]
	private GameObject guildWarAccesible;

	[SerializeField]
	private GameObject tutorialLocked;

	[SerializeField]
	private GameObject battleIsOn;

	[SerializeField]
	private GameObject nextBattle;

	[SerializeField]
	private GameObject warEnds;

	[SerializeField]
	private UILabel lockedLabel;

	[SerializeField]
	private UILabel joinBattleButtonLabel;

	[SerializeField]
	private GameObject spriteGlow;

	[SerializeField]
	private ShowTooltip infoButton;

	private void OnEnable()
	{
		UpdateUI();
		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void AddListeners()
	{
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed += OnGuildBattlePlayerChange;
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null && guildWarModel.CurrentBattle != null)
		{
			guildWarModel.CurrentBattle.Changed -= OnGuildWarModelChange;
			guildWarModel.CurrentBattle.Changed += OnGuildWarModelChange;
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
	}

	private void RemoveListeners()
	{
		GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.Changed -= OnGuildBattlePlayerChange;
		GuildWarModel guildWarModel = GuildWarHelper.GetGuildWarModel();
		if (guildWarModel != null && guildWarModel.CurrentBattle != null)
		{
			guildWarModel.CurrentBattle.Changed -= OnGuildWarModelChange;
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnGuildWarModelChange(TWDGroupModelChild model, string changed, object args)
	{
		switch (changed)
		{
		case "GuildBattleEnded":
		case "GuildBattleStarted":
		case "GuildBattlePlayerRegistered":
		case "GuildBattlePlayerResigned":
			UpdateUI();
			break;
		}
	}

	private void OnGuildBattlePlayerChange(ModelObject model, string changed, object args)
	{
		if (changed == "GuildBattleStarted")
		{
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		bool flag = GuildWarHelper.IsLockedByCouncilLevelOrTutorial();
		bool flag2 = !GuildWarHelper.IsGuildMember();
		bool flag3 = !GameManager.Instance.gameEconomyData.GetFeature("Social").Enabled;
		bool flag4 = flag || flag2 || flag3;
		Helpers.GameObjectSetActive(guildWarAccesible, !flag4);
		Helpers.GameObjectSetActive(tutorialLocked, flag4);
		if (flag4)
		{
			string text = "";
			HelpersUI.SetContentToLabel(content: (!flag3) ? (flag ? LocalizationManager.GetText(MissionHubGvGStateBase.MissionHubGvGStateLocalizations.GuildWarUnlockAtLevel, GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarUnlockAtCouncilLevel) : LocalizationManager.GetText(MissionHubGvGStateBase.MissionHubGvGStateLocalizations.GuildWarJoinGuild)) : LocalizationManager.GetText(MissionHubGvGStateBase.MissionHubGvGStateLocalizations.NotAvailable), label: lockedLabel);
			return;
		}
		bool flag5 = GuildWarHelper.IsPlayerRegisteredForBattle() || GuildWarHelper.CanPlayerRegisterForBattle();
		string textId = (flag5 ? "Popup.Guild.SignUpForBattle" : "Button.Spectate");
		HelpersUI.SetContentToLabel(joinBattleButtonLabel, LocalizationManager.GetText(textId));
		Helpers.GameObjectSetActive(spriteGlow, flag5);
		if (!flag5 && infoButton != null)
		{
			Helpers.GameObjectSetActive(infoButton, value: true);
			if (GuildWarHelper.IsLimitRegisted())
			{
				infoButton.LocalizationKey = MissionHubGvGStateBase.MissionHubGvGStateLocalizations.GuildWarMaxParticipants;
				infoButton.LocalizationParameter = GameManager.Instance.gameEconomyData.GuildWarConfig.GuildWarRegistrationLimit.ToString();
			}
			else if (GuildWarHelper.HasJoinedDuringBattle())
			{
				infoButton.LocalizationKey = MissionHubGvGStateBase.MissionHubGvGStateLocalizations.GuildBattleNewJoiner;
			}
			else
			{
				Helpers.GameObjectSetActive(infoButton, value: false);
			}
		}
		bool flag6 = GuildWarHelper.IsBattleOnGoing();
		bool flag7 = GuildWarHelper.CheckIfNextBattleExists();
		Helpers.GameObjectSetActive(battleIsOn, flag6);
		Helpers.GameObjectSetActive(nextBattle, !flag6 && flag7);
		Helpers.GameObjectSetActive(warEnds, !flag4 && !flag6 && !flag7);
		Helpers.GameObjectSetActive(guildWarAccesible, flag7);
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateUI();
	}
}
