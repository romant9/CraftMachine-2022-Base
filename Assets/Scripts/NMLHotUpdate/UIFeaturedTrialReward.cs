using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class UIFeaturedTrialReward : MonoBehaviourExtended
{
	[SerializeField]
	private UISprite currencyIcon;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private bool hideUntilSeasonHeroUnlocked = true;

	private SeasonDefinition currentSeason;

	private List<IReward> rewardsList;

	private RewardCurrency rewardCurrency;

	public void SetSeason(SeasonDefinition season)
	{
		currentSeason = season;
		UpdateUI();
	}

	public void OnEnable()
	{
		UpdateUI();
	}

	public void OnDisable()
	{
		Clear();
	}

	public void UpdateUI()
	{
		if (currentSeason == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		MapMissionGroupModel seasonCurrentMapMissionGroup = DetailMapPopUp.GetSeasonCurrentMapMissionGroup(currentSeason);
		if (seasonCurrentMapMissionGroup != null && seasonCurrentMapMissionGroup.IsFeaturedData != null && seasonCurrentMapMissionGroup.IsFeaturedData.CompletionRewards != null)
		{
			rewardsList = seasonCurrentMapMissionGroup.IsFeaturedData.CompletionRewards.GetRewardsOfType(RewardType.Currency);
			if (rewardsList != null && rewardsList.Count > 0 && rewardsList[0] != null && rewardsList[0] is RewardCurrency)
			{
				rewardCurrency = rewardsList[0] as RewardCurrency;
			}
		}
		if (rewardCurrency != null && rewardCurrency.CurrencyType != CurrencyType.None)
		{
			CurrencyType type = currentSeason.RewardCurrency;
			if (hideUntilSeasonHeroUnlocked && !GameManager.Instance.playerModel.SurvivorContainer.HasHero(SurvivorToken.GetHeroId(type)))
			{
				Helpers.GameObjectSetActive(base.gameObject, value: false);
				return;
			}
			string heroId = SurvivorToken.GetHeroId(rewardCurrency.CurrencyType);
			ActorDefinition actorDefinition = ((heroId != "") ? GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(heroId) : null);
			string text = ((actorDefinition != null) ? actorDefinition.Name : "");
			if (seasonCurrentMapMissionGroup.GetNonCompletedMissionsCount() == 0)
			{
				HelpersUI.SetContentToLabel(amountLabel, LocalizationManager.GetText("SeasonSevenTrial.Reward.Complete{actorName}{amount}", rewardCurrency.Amount, text));
			}
			else
			{
				HelpersUI.SetContentToLabel(amountLabel, LocalizationManager.GetText("SeasonSevenTrial.Reward.Amount{actorName}{amount}", rewardCurrency.Amount, text));
			}
			HelpersUI.SetSprite(currencyIcon, HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType));
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
		currentSeason = null;
		rewardsList = new List<IReward>();
		rewardCurrency = null;
	}
}
