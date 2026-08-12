using TWDModel;
using UnityEngine;

public class SeasonCard : UIListCard<SeasonDefinition>
{
	[SerializeField]
	private UILabel Title;

	[SerializeField]
	private UILabel Subtitle;

	[SerializeField]
	private UITexture BackgroundTexture;

	[SerializeField]
	private UITexture HeroTexture;

	[SerializeField]
	private UILabel CompleteLabel;

	[SerializeField]
	private GameObject HeroTokenContainer;

	[SerializeField]
	private UISprite HeroTokenIcon;

	[SerializeField]
	private UILabel HeroTokenAmount;

	[SerializeField]
	private GameObject BadgeContainer;

	[SerializeField]
	private UISprite BadgeBackground;

	[SerializeField]
	private UILabel BadgeLabel;

	[SerializeField]
	private UILabel RewardDescriptionLabel;

	[SerializeField]
	private UISeasonProggressBar ProgressBar;

	[SerializeField]
	private GameObject ButtonContainer;

	[SerializeField]
	private GameObject LockedContainer;

	[SerializeField]
	private UILabel LockedTimeLabel;

	[SerializeField]
	private UIFeaturedTrialReward TrialRewards;

	[Header("Season Reward")]
	[SerializeField]
	private UISeasonRewardIcon seasonRewardIcon;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (base.Item == null)
		{
			return;
		}
		HelpersUI.SetContentToLabel(Title, HelpersLocalization.GetSeasonTitle(base.Item.Id));
		HelpersUI.SetContentToLabel(Subtitle, HelpersLocalization.GetSeasonSubtitle(base.Item.Id));
		HelpersUI.SetContentToLabel(HeroTokenAmount, "");
		if (RewardDescriptionLabel != null)
		{
			RewardDescriptionLabel.text = HelpersLocalization.GetSeasonRewardDescription(base.Item.Id);
		}
		if (BackgroundTexture != null)
		{
			BackgroundTexture.material = HelpersGfx.GetSeasonBackgroundMaterial(base.Item.Id);
		}
		if (HeroTexture != null)
		{
			HeroTexture.material = HelpersGfx.GetSeasonHeroMaterial(base.Item.Id);
		}
		if (base.Item.RewardCurrency != CurrencyType.None)
		{
			CurrencyModel currency = GameManager.Instance.playerModel.GetCurrency(base.Item.RewardCurrency);
			string heroId = SurvivorToken.GetHeroId(base.Item.RewardCurrency);
			ActorDefinition actorDefinition = GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(heroId);
			if (actorDefinition != null)
			{
				if (GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId))
				{
					Helpers.GameObjectSetActive(CompleteLabel, value: true);
					Helpers.GameObjectSetActive(HeroTokenContainer, value: false);
				}
				else
				{
					Helpers.GameObjectSetActive(CompleteLabel, value: false);
					Helpers.GameObjectSetActive(HeroTokenContainer, value: true);
					HelpersUI.SetContentToLabel(HeroTokenAmount, currency.Value + " / " + actorDefinition.TokensToUnlock);
					HelpersUI.SetSprite(HeroTokenIcon, HelpersGfx.GetTokenCurrencyIconName(base.Item.RewardCurrency));
				}
			}
		}
		else
		{
			Helpers.GameObjectSetActive(CompleteLabel, value: false);
			Helpers.GameObjectSetActive(HeroTokenContainer, value: false);
		}
		if (HasUnlockedHero())
		{
			if (TrialRewards != null)
			{
				TrialRewards.SetSeason(base.Item);
				if (TrialRewards.gameObject.activeInHierarchy)
				{
					Helpers.GameObjectSetActive(CompleteLabel, value: false);
				}
			}
			Helpers.GameObjectSetActive(ProgressBar, value: false);
			Helpers.GameObjectSetActive(TrialRewards, value: true);
		}
		else
		{
			if (ProgressBar != null)
			{
				ProgressBar.SetSeason(base.Item);
			}
			Helpers.GameObjectSetActive(ProgressBar, value: true);
			Helpers.GameObjectSetActive(TrialRewards, value: false);
		}
		if (BadgeContainer != null)
		{
			if (base.Item.Highlighted && GameManager.Instance.playerModel.MapContainerModel.HasUnseenContent(base.Item))
			{
				Helpers.GameObjectSetActive(BadgeContainer, value: true);
				HelpersUI.SetContentToLabel(BadgeLabel, LocalizationManager.GetText("Popup.MissionHub.SeasonSeven.NewEpisode"));
			}
			else
			{
				Helpers.GameObjectSetActive(BadgeContainer, value: false);
			}
		}
		long firstSeasonMissionUnlockTime = GameManager.Instance.gameEconomyData.GetFirstSeasonMissionUnlockTime(base.Item);
		if (firstSeasonMissionUnlockTime != -1 && GameManager.Instance.playerModel.UtcTimeStamp < firstSeasonMissionUnlockTime)
		{
			long num = firstSeasonMissionUnlockTime - GameManager.Instance.playerModel.UtcTimeStamp;
			num = ((num < 0) ? 0 : num);
			Helpers.GameObjectSetActive(ButtonContainer, value: false);
			Helpers.GameObjectSetActive(LockedContainer, value: true);
			HelpersUI.SetContentToLabel(LockedTimeLabel, Helpers.FormatTime(num));
		}
		else
		{
			Helpers.GameObjectSetActive(LockedContainer, value: false);
			Helpers.GameObjectSetActive(ButtonContainer, value: true);
		}
		if (seasonRewardIcon != null)
		{
			seasonRewardIcon.UpdateUI(base.Item);
		}
	}

	private bool HasUnlockedHero()
	{
		if (GameManager.Instance.gameEconomyData.IsHeroToken(base.Item.RewardCurrency))
		{
			string heroId = SurvivorToken.GetHeroId(base.Item.RewardCurrency);
			return GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId);
		}
		return false;
	}

	public void OnClick()
	{
		long firstSeasonMissionUnlockTime = GameManager.Instance.gameEconomyData.GetFirstSeasonMissionUnlockTime(base.Item);
		if (firstSeasonMissionUnlockTime == -1 || GameManager.Instance.playerModel.UtcTimeStamp >= firstSeasonMissionUnlockTime)
		{
			MissionHubNavigation.OpenSeasonMap(base.Item);
		}
	}
}
