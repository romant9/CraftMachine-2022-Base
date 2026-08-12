using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GvGFakeBattleContainer : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> opponentsContainer;

	[SerializeField]
	private UILabel briefingTitle;

	[SerializeField]
	private UILabel briefingDescription;

	[SerializeField]
	private UILabel enemyGuildName;

	private Dictionary<string, GameObject> opponentPortraits;

	private readonly string villainLocalizationString = "GvG.FakeBattle.Villain";

	private readonly string targetScoreLocalizationString = "GvG.FakeBattle.TargetScore";

	private void Init()
	{
		opponentPortraits = new Dictionary<string, GameObject>();
		foreach (GameObject item in opponentsContainer)
		{
			Helpers.GameObjectSetActive(item, value: false);
			if (!(item == null))
			{
				string[] array = item.name.Split('_');
				if (array.Length == 3)
				{
					opponentPortraits.Add(array[2], item);
				}
				else if (array.Length == 2)
				{
					opponentPortraits.Add(array[1], item);
				}
			}
		}
	}

	public void Setup(FakeBattleDefinition fakeBattleDefinition = null)
	{
		if (opponentPortraits == null)
		{
			Init();
		}
		if (fakeBattleDefinition == null)
		{
			int guildTier = GameManager.Instance.playerModel.GuildWarModel.CurrentBattle.GuildTier;
			fakeBattleDefinition = GameManager.Instance.gameEconomyData.FindFakeBattleDefinition(guildTier);
		}
		UpdateUI(fakeBattleDefinition);
	}

	private void UpdateUI(FakeBattleDefinition fakeBattleDefinition)
	{
		if (fakeBattleDefinition != null)
		{
			if (opponentPortraits.Count > 0 && opponentPortraits.ContainsKey(fakeBattleDefinition.OpponentName))
			{
				Helpers.GameObjectSetActive(opponentPortraits[fakeBattleDefinition.OpponentName], value: true);
			}
			if (briefingTitle != null)
			{
				HelpersUI.SetContentToLabel(briefingTitle, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(villainLocalizationString, fakeBattleDefinition.OpponentName));
			}
			if (briefingDescription != null)
			{
				HelpersUI.SetContentToLabel(briefingDescription, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(targetScoreLocalizationString, fakeBattleDefinition.TargetScore));
			}
			if (enemyGuildName != null)
			{
				HelpersUI.SetContentToLabel(enemyGuildName, fakeBattleDefinition.OpponentName);
			}
		}
	}
}
