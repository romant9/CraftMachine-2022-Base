using System;
using TWDModel;
using UnityEngine;

public class SurvivalManualRankItem : MonoBehaviour
{
	[SerializeField]
	private UILabel rankNum;

	[SerializeField]
	private PlayerEmblemIcon playerEmblem;

	[SerializeField]
	private UILabel playerName;

	[SerializeField]
	private UISprite[] medalIcons;

	[SerializeField]
	private UILabel level;

	private SurvivalManualScoreDataEntry scoreDataEntry;

	private int rankIndex;

	public void Setup(int rankIndex, SurvivalManualScoreDataEntry dataEntry)
	{
		this.rankIndex = rankIndex;
		scoreDataEntry = dataEntry;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (scoreDataEntry == null)
		{
			return;
		}
		rankNum.text = rankIndex + 1 + ".";
		playerEmblem.SetEmblem(scoreDataEntry.PlayerEmblem);
		playerName.text = GameManager.Instance.GetFilteredText(scoreDataEntry.Name);
		if (scoreDataEntry.HaveMedalStoryIds != null && scoreDataEntry.HaveMedalStoryIds.Count > 0)
		{
			int num = Math.Min(3, scoreDataEntry.HaveMedalStoryIds.Count);
			for (int i = 0; i < num; i++)
			{
				SurvivalManualDefinition survivalManualDefinitionById = GameManager.Instance.playerModel.gameEconomyData.GetSurvivalManualDefinitionById(scoreDataEntry.HaveMedalStoryIds[i]);
				if (survivalManualDefinitionById != null)
				{
					medalIcons[i].spriteName = survivalManualDefinitionById.SouvenirMedalIcon;
				}
			}
		}
		level.text = "Lv." + scoreDataEntry.Score;
	}
}
