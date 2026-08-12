using System;
using TWDModel;
using UnityEngine;

public class FeaturedHeroPopup : HUDElement
{
	[SerializeField]
	private UILabel featuredHeroTitle;

	[SerializeField]
	private UILabel featuredHeroDescription;

	[SerializeField]
	private UILabel statsHealth;

	[SerializeField]
	private UILabel statsDamage;

	[SerializeField]
	private UILabel endingTime;

	[SerializeField]
	private UITexture characterGlow;

	[SerializeField]
	private UISprite popupBackground;

	[SerializeField]
	private UITexture characterTexture;

	[SerializeField]
	private UILabel featuredHeroCallButtonLabel;

	[SerializeField]
	private GameObject featuredHeroCallButton;

	private bool initialized;

	private FeaturedHeroDefinition FeaturedHeroDefinition => GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);

	public override void OpenWithStateData(object data)
	{
		initialized = true;
		base.OpenWithStateData(data);
		if (data is FeaturedHeroDefinition featuredHeroDefinition)
		{
			ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(featuredHeroDefinition.ActorDefinitionID);
			if (actorDefinition != null)
			{
				HelpersUI.SetContentToLabel(featuredHeroTitle, actorDefinition.Name);
				HelpersUI.SetContentToLabel(featuredHeroDescription, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.FeatureHero.Description{parameter}", actorDefinition.Name));
				Helpers.GameObjectSetActive(featuredHeroCallButton, !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.CampSurvivorInfoPopup));
				HelpersUI.SetContentToLabel(featuredHeroCallButtonLabel, GetFeaturedHeroCallLocalisedButtonText(), !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.CampSurvivorInfoPopup));
			}
			HelpersUI.SetContentToLabel(statsDamage, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Statistic.BuffDamage") + "+" + featuredHeroDefinition.DamageBoostMultiplier + "%");
			HelpersUI.SetContentToLabel(statsHealth, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Statistic.BuffHealth") + "+" + featuredHeroDefinition.HealthBoostMultiplier + "%");

			var textKey = "Popup.FeaturedHero.FeatureEnding{parameter}";
			long startTime = featuredHeroDefinition.TimeUntilEndMilliseconds(GameManager.Instance.playerModel.UtcTimeStamp);
			var delta = startTime - MyTools.TimeSpanToLong(TimeSpan.FromDays(7));
			if (delta > 0)
			{
				textKey = "Popup.Guild.WarStartingIn{Parameter}";
				startTime = delta;
			}
			HelpersUI.SetContentToLabel(endingTime, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(textKey, Helpers.FormatTime(startTime)));
			HelpersGfx.SetColorWithHex(popupBackground, featuredHeroDefinition.BackgroundColorHex);
			HelpersGfx.SetColorWithHex(characterGlow, featuredHeroDefinition.GlowColorHex);
			HelpersGfx.SetSeasonHeroMaterial(characterTexture, featuredHeroDefinition.HeroSeasonIDArt);
		}
	}

	public override void Open()
	{
		if (initialized)
		{
			base.Open();
		}
	}

	public void OnClickFeatureHeroCall()
	{
		Close();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (HasFeaturedHero())
		{
			OpenSurvivorInfoPopup();
		}
		else if (CanUnlockFeaturedHero())
		{
			OpenSurvivorManagementPopUp();
		}
		else
		{
			OpenRadiophoneFeaturePopup();
		}
	}

	private void OpenSurvivorInfoPopup()
	{
		SurvivorModel survivorById = GameManager.Instance.playerModel.SurvivorContainer.GetSurvivorById(FeaturedHeroDefinition?.ActorDefinitionID);
		if (survivorById != null)
		{
			SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
			if (survivorInfoPopup != null)
			{
				survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverview;
				survivorInfoPopup.OpenForModel(survivorById, new SurvivorFilterList(null));
			}
		}
	}

	private void OpenRadiophoneFeaturePopup()
	{
		NewPhonePopup.OpenRadiophoneFeaturePopup();
	}

	private void OpenSurvivorManagementPopUp()
	{
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampTrainingGrounds) as SurvivorManagementPopUp)?.Open();
	}

	private bool HasFeaturedHero()
	{
		FeaturedHeroDefinition activeFeaturedHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);
		return GameManager.Instance.playerModel.SurvivorContainer.HasHero(activeFeaturedHero?.ActorDefinitionID);
	}

	private bool CanUnlockFeaturedHero()
	{
		CurrencyType traitUpgradeCurrency = GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(FeaturedHeroDefinition?.ActorDefinitionID).TraitUpgradeCurrency;
		if (traitUpgradeCurrency != CurrencyType.None)
		{
			return GameManager.Instance.playerModel.SurvivorContainer.HasEnoughTokenToUnlock(traitUpgradeCurrency);
		}
		return false;
	}

	private string GetFeaturedHeroCallLocalisedButtonText()
	{
		if (HasFeaturedHero())
		{
			return LocalizationManager.GetText("Generic.Preview");
		}
		if (CanUnlockFeaturedHero())
		{
			string text = GameManager.Instance.gameEconomyData.GetActorDefinition(FeaturedHeroDefinition?.ActorDefinitionID).Name;
			return LocalizationManager.GetText("Popup.LoginReward.Unlock{HeroName}", text);
		}
		return LocalizationManager.GetText("Popup.FeaturedHeroOverview.Button");
	}
}
