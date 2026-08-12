using TWDModel;
using UnityEngine;

public class SelectSeasonPopup : HUDElement
{
	[SerializeField]
	private SeasonListPanel seasonListPanel;

	public override void Open()
	{
		base.Open();
		UITypeOpenOnClose = UIType.MissionHubPopup;
		SeasonDefinition[] seasonDefinitions = GameManager.Instance.gameEconomyData.SeasonDefinitions;
		if (seasonDefinitions != null)
		{
			seasonListPanel.SetCards(seasonDefinitions);
		}
	}
}
