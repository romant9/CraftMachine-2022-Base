using TWDModel;
using UnityEngine;

public class GuildBattleActiveBonusListElement : NUIListItem<string>
{
	[SerializeField]
	private UISprite bonusRewardIcon;

	[SerializeField]
	private UISprite bonusClassIcon;

	[SerializeField]
	private GameObject classIconParent;

	[SerializeField]
	private UILabel stackableAmount;

	private TraitDefinition traitDefinition;

	private int state;

	public override void UpdateUI()
	{
		string bonusName = GetData();
		traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(bonusName);
		GuildBattleProgressSnapshot currentCompletionSnapshot = GameManager.Instance.playerModel.GvGSeasonModelPlayer.GuildWarModelPlayer.GuildBattleModel.CurrentCompletionSnapshot;
		int count = GuildWarHelper.GetActiveBonusesList().FindAll((string t) => t == bonusName).Count;
		if (count > state)
		{
			state = count;
			if (count > 1)
			{
				stackableAmount.text = "x" + count;
			}
			if (traitDefinition.ConstructionParameters.Count == 2)
			{
				SurvivorClass parameter = traitDefinition.GetParameter<SurvivorClass>(1);
				HelpersUI.SetSprite(bonusClassIcon, HelpersGfx.GetSurvivorClassSmallIconName(parameter));
				Helpers.GameObjectSetActive(classIconParent.gameObject, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(classIconParent.gameObject, value: false);
			}
			if (!HelpersUI.SetSprite(bonusRewardIcon, HelpersGfx.GetGuildBattleBuffIconName(bonusName)))
			{
				HelpersUI.SetSprite(bonusRewardIcon, "Ui_Icon_Trait_Dodge");
			}
			int value = 0;
			if (!currentCompletionSnapshot.SectorBonusAnimationSeen.TryGetValue(bonusName, out value))
			{
				TweenManager.PlayTweenGroup(base.gameObject, 20);
				SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleBuffAnimationSeen(bonusName, count);
			}
			if (count != value && count > 1)
			{
				TweenManager.PlayTweenGroup(stackableAmount.gameObject, 21);
				SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleBuffAnimationSeen(bonusName, count);
			}
			else if (count > 1)
			{
				stackableAmount.alpha = 1f;
			}
		}
	}

	public void OnClickBonus()
	{
		if (traitDefinition != null)
		{
			string guildBonusTooltip = HelpersLocalization.GetGuildBonusTooltip(traitDefinition);
			if (!string.IsNullOrEmpty(guildBonusTooltip))
			{
				TooltipManager.OpenTextBoxWithText(base.gameObject, guildBonusTooltip);
			}
		}
	}
}
