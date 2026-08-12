using TWDModel;
using UnityEngine;

public class SurvivorRarityAndClassPanel : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel classLabel;

	[SerializeField]
	private GameObject[] stars;

	[SerializeField]
	private GameObject[] starsEffects;

	[SerializeField]
	private GameObject[] featuredStars;

	[SerializeField]
	private UISprite pinkStarHighlight;

	[Header("Extra")]
	[SerializeField]
	private UILabel levelUpLabel;

	private SurvivorModel survivorModel;

	public bool IsLimited { get; set; }

	private void Awake()
	{
		DebugIdString = "SurvivorRarityAndClass";
	}

	public void UpdateWithSurvivor(SurvivorModel survivorModel, bool useRarityColor = true)
	{
		if (!IsNotNull(survivorModel))
		{
			return;
		}
		ColorEntry rarityColorData = GameManager.Instance.GetRarityColorData(survivorModel.SurvivorRarityLevel);
		this.survivorModel = survivorModel;
		if (IsNotNull(stars) && IsNotNull(starsEffects) && stars.Length == starsEffects.Length)
		{
			for (int i = 0; i < stars.Length; i++)
			{
				if (stars[i] != null)
				{
					if (survivorModel.SurvivorRarityLevel >= i)
					{
						Helpers.GameObjectSetActive(stars[i], value: true);
						Helpers.GameObjectSetActive(starsEffects[i], value: true);
					}
					else
					{
						Helpers.GameObjectSetActive(stars[i], value: false);
						Helpers.GameObjectSetActive(starsEffects[i], value: false);
					}
				}
			}
		}
		if (featuredStars.Length != 0)
		{
			for (int j = 0; j < featuredStars.Length; j++)
			{
				Helpers.GameObjectSetActive(featuredStars[j], value: false);
			}
			int num = 0;
			FeaturedHeroDefinition featuredDefinition = survivorModel.FeaturedDefinition;
			if (featuredDefinition != null && !IsLimited)
			{
				num = featuredDefinition.RarityModifier;
			}
			for (int k = 0; k < num; k++)
			{
				int num2 = (survivorModel.SurvivorRarityLevel + 1 + k) % featuredStars.Length;
				Helpers.GameObjectSetActive(featuredStars[num2], value: true);
			}
		}
		if (pinkStarHighlight != null)
		{
			HelpersUI.SetSprite(pinkStarHighlight, (survivorModel.SurvivorRarityLevel < 5) ? HelpersGfx.GetRarityBorderSpriteName(survivorModel.SurvivorRarityLevel) : "Ui_Border_Master");
		}
		if (classLabel != null && IsNotNull(rarityColorData))
		{
			classLabel.text = HelpersLocalization.GetSurvivorClassName(survivorModel.SurvivorClass) + " " + HelpersLocalization.GetRarityLevel(survivorModel.SurvivorRarityLevel);
			classLabel.gradientTop = rarityColorData.GradientColorTop;
			classLabel.gradientBottom = rarityColorData.GradientColorBottom;
		}
		if (levelUpLabel != null)
		{
			HelpersUI.SetContentToLabel(levelUpLabel, LocalizationManager.GetText("Popup.SurvivorLevelUp.Label.LevelUp{Level}", survivorModel.Level));
		}
	}

	public void OnStarTooltipClicked()
	{
		if (GameManager.Instance.gameEconomyData != null && survivorModel != null)
		{
			int highestLevelDiffForZeroBodyshot = GameManager.Instance.gameEconomyData.GetHighestLevelDiffForZeroBodyshot(Faction.Survivor, Faction.Walker);
			int rarityActorLevelModifier = GameManager.Instance.gameEconomyData.GetRarityActorLevelModifier(survivorModel.SurvivorRarityLevel);
			rarityActorLevelModifier += (survivorModel.IsHero ? 1 : 0);
			string text = (survivorModel.IsHero ? LocalizationManager.GetText("SurvivorInfo.Rarity.SurvivorType.Hero") : LocalizationManager.GetText("SurvivorInfo.Rarity.SurvivorType.Survivor"));
			string text2 = LocalizationManager.GetText("SurvivorInfo.Rarity.Tooltip{Rarity}{SurvivorType}{LevelDiff}", HelpersLocalization.GetRarityLevel(survivorModel.SurvivorRarityLevel), text, survivorModel.Level + highestLevelDiffForZeroBodyshot + rarityActorLevelModifier);
			FeaturedHeroDefinition activeFeaturedHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);
			if (activeFeaturedHero != null && activeFeaturedHero.ActorDefinitionID == survivorModel.Definition.ID && !IsLimited)
			{
				text2 = text2 + "\r\n" + LocalizationManager.GetText("SurvivorInfo.Rarity.TooltipFeaturedHero{parameter}", activeFeaturedHero.RarityModifier);
			}
			TooltipManager.OpenTextBoxWithText(pinkStarHighlight.gameObject, text2);
		}
	}
}
