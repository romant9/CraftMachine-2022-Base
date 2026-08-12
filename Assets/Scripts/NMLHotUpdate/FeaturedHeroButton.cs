using TWDModel;
using UnityEngine;

public class FeaturedHeroButton : MonoBehaviour
{
	[SerializeField]
	private GameObject featuredHeroButton;

	[SerializeField]
	private UITexture featureHeroTexture;

	[SerializeField]
	private UITexture bgGlow;

	[SerializeField]
	private UITexture characterTexture;

	private FeaturedHeroDefinition featuredHero;

	private float refreshTimer;

	private const string FeaturedHeroPrefsKey = "FeaturedHero";

	private bool hasSeenNewFeaturedHero
	{
		get
		{
			if (TWDPlayerPrefs.HasKey("FeaturedHero") && TWDPlayerPrefs.GetString("FeaturedHero", string.Empty) != featuredHero?.ActorDefinitionID && TutorialView.Instance != null && !TutorialView.Instance.Running)
			{
				HUDManager instance = SingularityMonoBehaviour<HUDManager>.Instance;
				if ((object)instance == null)
				{
					return false;
				}
				return instance.OpenPopups?.Count == 0;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (!TWDPlayerPrefs.HasKey("FeaturedHero"))
		{
			TWDPlayerPrefs.SetString("FeaturedHero", "");
		}
	}

	public void OnClick()
	{
		if (featuredHero != null)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.FeaturedHeroPopup) as FeaturedHeroPopup)?.OpenWithStateData(featuredHero);
		}
		else
		{
			Helpers.GameObjectSetActive(featuredHeroButton, value: false);
		}
	}

	private bool CheckForFeatureHeroAvailability()
	{
		bool flag = GameManager.Instance.playerModel.Tutorial.HasCompletedPart("Phone");
		featuredHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);
		bool flag2 = GameManager.Instance.VersionValidUntil.HasValue && GameManager.Instance.VersionUpgradeNeeded;
		bool flag3 = TutorialView.Instance != null && TutorialView.Instance.Running;
		bool flag4 = CampView.Instance != null && CampView.Instance.CampViewBuildings.Moving;
		return Helpers.GameObjectSetActive(featuredHeroButton, !flag2 && flag && featuredHero != null && !flag3 && !flag4);
	}

	private void SetGraphics()
	{
		if (featuredHero != null)
		{
			HelpersGfx.SetColorWithHex(bgGlow, featuredHero.GlowColorHex);
			HelpersGfx.SetSeasonHeroMaterial(characterTexture, featuredHero.HeroSeasonIDArt);
		}
	}

	private void Update()
	{
		refreshTimer -= Time.deltaTime;
		if (!(refreshTimer <= 0f))
		{
			return;
		}
		if (CheckForFeatureHeroAvailability() && featuredHero.IsActivePeriod(GameManager.Instance.playerModel.UtcTimeStamp))
		{
			SetGraphics();
		}
		if (hasSeenNewFeaturedHero)
		{
			TWDPlayerPrefs.SetString("FeaturedHero", featuredHero?.ActorDefinitionID ?? string.Empty);
			if (featuredHero != null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.FeaturedHeroPopup)?.OpenWithStateData(featuredHero);
			}
		}
		refreshTimer = 1f;
	}
}
