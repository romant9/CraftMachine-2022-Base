using TWDModel;
using UnityEngine;

public class EndlessModeAttemptTeamEntry : MonoBehaviour
{
	[SerializeField]
	private UISprite classSprite;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UITexture portraitTexture;

	[SerializeField]
	private UISprite[] survivorStarsPanels;

	[SerializeField]
	private UISprite rarityBorderSprite;

	[SerializeField]
	private UITexture survivorSupportTexture;

	[SerializeField]
	private UILabel survivorSupportLabel;

	[SerializeField]
	private UISprite[] supportStarsPanel;

	[SerializeField]
	private UISprite supportRarityBorderSprite;

	public void SetTeamContent(SurvivorMockData survivorMockData)
	{
		string survivorClassIconName = HelpersGfx.GetSurvivorClassIconName(survivorMockData.SurvivorClass);
		HelpersUI.SetSprite(classSprite, survivorClassIconName);
		string content = survivorMockData.Level.ToString();
		HelpersUI.SetContentToLabel(levelLabel, content);
		string content2 = survivorMockData.Name;
		HelpersUI.SetContentToLabel(nameLabel, content2);
		HelpersGfx.SetSurvivorRarityRating(survivorStarsPanels, survivorMockData.RarityLevel);
		portraitTexture.mainTexture = HelpersGfx.GetSurvivorPortraitTextureBySurvivorMockData(survivorMockData, OnMissingPortraitRendered);
		HelpersGfx.UpdateSpriteAndKeepScale(rarityBorderSprite, HelpersGfx.GetRarityBorderSpriteName(survivorMockData.RarityLevel));
	}

	public void SetSupportContent(SurvivorSupportData supportData)
	{
		survivorSupportTexture.mainTexture = HelpersGfx.LoadSupportIcon(supportData.SupportId);
		survivorSupportLabel.text = HelpersLocalization.GetSupportName(supportData.SupportId);
		HelpersGfx.SetSurvivorRarityRating(supportStarsPanel, supportData.SupportLevel);
		HelpersGfx.UpdateSpriteAndKeepScale(supportRarityBorderSprite, HelpersGfx.GetRarityBorderSpriteName(supportData.SupportLevel));
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource source)
	{
		portraitTexture.mainTexture = PortraitManager.Instance.GetPortrait(source);
	}
}
