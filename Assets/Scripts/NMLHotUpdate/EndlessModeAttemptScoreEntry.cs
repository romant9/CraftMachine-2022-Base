using TWDModel;
using UnityEngine;

public class EndlessModeAttemptScoreEntry : MonoBehaviour
{
	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel rankLabel;

	[SerializeField]
	private GameObject affectsTotalScoreContainer;

	[SerializeField]
	private GameObject expertModeTag;

	[SerializeField]
	private Color expertModeColor;

	[SerializeField]
	private Color normalModeColor;

	[SerializeField]
	private UISprite actorIcon;

	public void SetContent(long score, int rank, bool affectsTotalScore, bool expertMode, string actorDefinitionID = null)
	{
		Color color = (expertMode ? expertModeColor : normalModeColor);
		HelpersUI.SetContentToLabel(scoreLabel, EndlessModeHelpers.GetFormattedScoreText(score));
		HelpersUI.SetColor(scoreLabel, color);
		if (rankLabel.gameObject.activeSelf)
		{
			HelpersUI.SetContentToLabel(rankLabel, rank.ToString());
		}
		if (expertMode && actorDefinitionID != null)
		{
			CurrencyType survivorTraitUpgradeCurrencyType = HelpersGfx.GetSurvivorTraitUpgradeCurrencyType(GameManager.Instance.gameEconomyData.GetActorDefinition(actorDefinitionID));
			actorIcon.spriteName = HelpersGfx.GetCurrencyIconName(survivorTraitUpgradeCurrencyType);
		}
		Helpers.GameObjectSetActive(affectsTotalScoreContainer, affectsTotalScore);
		Helpers.GameObjectSetActive(expertModeTag, expertMode);
	}
}
