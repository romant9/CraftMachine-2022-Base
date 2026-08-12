using Client.Tweener;
using TWDModel;
using UnityEngine;

public class GuildBattleResultPopup : HUDElement
{
	[Header("Animations")]
	[SerializeField]
	private int introTweenGroup;

	[SerializeField]
	private int scoresTweenGroup;

	[Header("Player Guild")]
	[SerializeField]
	private UILabel guildName;

	[SerializeField]
	private UILabel guildVictoryPointsLabel;

	[SerializeField]
	private GameObject winnerTag;

	[SerializeField]
	private GameObject drawTag;

	[SerializeField]
	private GameObject defeatTag;

	[SerializeField]
	private UISprite guildTierIcon;

	[SerializeField]
	private UILabel guildTierNameLabel;

	[Header("Enemy Guild")]
	[SerializeField]
	private UILabel opponentGuildName;

	[SerializeField]
	private UILabel opponentFakeGuildName;

	[SerializeField]
	private UILabel opponentPointsLabel;

	[SerializeField]
	private GameObject opponentGuildEmblem;

	[SerializeField]
	private UISprite opponentGuildTierIcon;

	[SerializeField]
	private UILabel opponentGuildTierLabel;

	[SerializeField]
	private GvGFakeBattleContainer opponentFakeGuildEmblem;

	[Header("Container")]
	[SerializeField]
	private GameObject battleEndParent;

	[SerializeField]
	private GuildBattleHighscoresEndBattle guildBattleHighscoresEndBattle;

	private TweenTimeline tweenTimeLine = new TweenTimeline();

	private GuildBattleResultInfo guildBattleResult;

	private bool isVictory;

	private bool isDefeat;

	private bool isDraw;

	private bool animationsCompleted;

	public static bool CanShowOnlyRewardsPopup()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		bool flag = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsOngoingForPlayer();
		bool flag2 = playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.IsCurrentGuildBattle();
		if (flag)
		{
			return !flag2;
		}
		return false;
	}

	public override void OpenWithStateData(object data)
	{
		guildBattleResult = (GuildBattleResultInfo)data;
		Open();
		OpenBattleEnded();
	}

	public override void Open()
	{
		base.Open();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GvGStartBattleFlowPopup);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.MapTeamSelection);
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleSelectMissionPopup);
		GuildBattleMapPopup guildBattleMapPopup = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.GuildBattleMapPopup) as GuildBattleMapPopup;
		if (guildBattleMapPopup != null && guildBattleMapPopup.IsOpen)
		{
			guildBattleMapPopup.MapMissionModel = null;
			GuildBattleMapView viewInstance = guildBattleMapPopup.GetViewInstance();
			if (viewInstance != null)
			{
				viewInstance.Clear();
			}
		}
		OpenWaitingForBattleEnd();
	}

	private void OpenWaitingForBattleEnd()
	{
		animationsCompleted = false;
		TweenManager.PlayTweenGroup(base.gameObject, introTweenGroup);
		UpdateUI();
	}

	private void OpenBattleEnded()
	{
		isVictory = guildBattleResult.BattleResult == GuildBattleModel.GuildBattleResult.Victory;
		isDefeat = guildBattleResult.BattleResult == GuildBattleModel.GuildBattleResult.Defeat;
		isDraw = guildBattleResult.BattleResult == GuildBattleModel.GuildBattleResult.Draw;
		UpdateUI();
		if (guildBattleResult.isFakeBattle)
		{
			Helpers.GameObjectSetActive(opponentGuildEmblem, value: false);
			Helpers.GameObjectSetActive(opponentFakeGuildEmblem, value: true);
			GameManager.Instance.gameEconomyData.FindFakeBattleDefinition(guildBattleResult.GuildTier);
			opponentFakeGuildEmblem.Setup(GameManager.Instance.gameEconomyData.FindFakeBattleDefinition(guildBattleResult.GuildTier));
		}
		else
		{
			Helpers.GameObjectSetActive(opponentFakeGuildEmblem, value: false);
			Helpers.GameObjectSetActive(opponentGuildEmblem, value: true);
		}
		PlayResultsAnimation();
	}

	private void PlayResultsAnimation()
	{
		animationsCompleted = false;
		tweenTimeLine = new TweenTimeline();
		tweenTimeLine.Queue(TweenObjects.Group(base.gameObject, introTweenGroup));
		tweenTimeLine.Queue(TweenObjects.Group(base.gameObject, scoresTweenGroup));
		tweenTimeLine.Play();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(battleEndParent, value: true);
		guildBattleHighscoresEndBattle.SetScores(guildBattleResult);
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		HelpersUI.SetContentToLabel(guildName, guildModel.Name);
		HelpersUI.SetContentToLabel(guildVictoryPointsLabel, guildBattleResult.EndVictoryPoints.ToString());
		GuildTierDefinition guildTierDefinition = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(guildBattleResult.GuildTier);
		HelpersUI.SetSprite(guildTierIcon, guildTierDefinition?.IconSprite);
		HelpersUI.SetContentToLabel(guildTierNameLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(guildTierDefinition?.NameLocalizationKey));
		if (guildBattleResult.isFakeBattle)
		{
			FakeBattleDefinition fakeBattleDefinition = GameManager.Instance.gameEconomyData.FindFakeBattleDefinition(guildBattleResult.EnemyTier);
			if (fakeBattleDefinition != null)
			{
				HelpersUI.SetContentToLabel(opponentPointsLabel, fakeBattleDefinition.TargetScore.ToString());
			}
			HelpersUI.SetContentToLabel(opponentFakeGuildName, guildBattleResult.EnemyGuildName);
		}
		else
		{
			GuildTierDefinition guildTierDefinition2 = GameManager.Instance.gameEconomyData.GetGuildTierDefinition(guildBattleResult.EnemyTier);
			HelpersUI.SetSprite(opponentGuildTierIcon, guildTierDefinition2?.IconSprite);
			HelpersUI.SetContentToLabel(opponentGuildTierLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(guildTierDefinition2?.NameLocalizationKey));
			HelpersUI.SetContentToLabel(opponentGuildName, guildBattleResult.EnemyGuildName);
			HelpersUI.SetContentToLabel(opponentPointsLabel, guildBattleResult.EndEnemyVictoryPoints.ToString());
		}
		Helpers.GameObjectSetActive(winnerTag, isVictory);
		Helpers.GameObjectSetActive(defeatTag, isDefeat);
		Helpers.GameObjectSetActive(drawTag, isDraw);
	}

	public override void Close()
	{
		base.Close();
	}

	public override void OnBackButtonClicked()
	{
		if (animationsCompleted)
		{
			OnClickClose();
		}
	}
}
