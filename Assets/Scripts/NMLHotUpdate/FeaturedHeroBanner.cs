using TWDModel;
using UnityEngine;

public class FeaturedHeroBanner : MonoBehaviour
{
	[SerializeField]
	private UILabel description;

	[SerializeField]
	private UITexture characterTexture;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UITexture glow;

	public void UpdateUI()
	{
		if (GameManager.Instance.playerModel.Tutorial.HasCompletedPart("Phone"))
		{
			FeaturedHeroDefinition activeFeaturedHero = GameManager.Instance.gameEconomyData.GetActiveFeaturedHero(GameManager.Instance.playerModel.UtcTimeStamp);
			if (activeFeaturedHero != null)
			{
				Helpers.GameObjectSetActive(base.gameObject, value: true);
				HelpersGfx.SetColorWithHex(background, activeFeaturedHero.BackgroundColorHex);
				HelpersGfx.SetColorWithHex(glow, activeFeaturedHero.GlowColorHex);
				HelpersGfx.SetSeasonHeroMaterial(characterTexture, activeFeaturedHero.HeroSeasonIDArt);
			}
			else
			{
				Helpers.GameObjectSetActive(base.gameObject, value: false);
			}
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}
}
