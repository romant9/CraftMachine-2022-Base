using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleRewardBonus : MonoBehaviour
{
	public GuildBattleMapSectorModel Model;

	[SerializeField]
	private UISprite bonusRewardIcon;

	[SerializeField]
	private UISprite bonusClassIcon;

	[SerializeField]
	private GameObject classIconParent;

	private TraitDefinition traitDefinition;

	public void UpdateUI()
	{
		List<string> guildSectorBattleTraitBonus = GuildWarHelper.GetCurrentBattle().GetGuildSectorBattleTraitBonus(Model.SectorId);
		if (guildSectorBattleTraitBonus != null && guildSectorBattleTraitBonus.Count > 0)
		{
			traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(guildSectorBattleTraitBonus[0]);
			if (traitDefinition != null && traitDefinition.ConstructionParameters.Count == 2)
			{
				SurvivorClass parameter = traitDefinition.GetParameter<SurvivorClass>(1);
				HelpersUI.SetSprite(bonusClassIcon, HelpersGfx.GetSurvivorClassSmallIconName(parameter));
				Helpers.GameObjectSetActive(classIconParent.gameObject, value: true);
			}
			else
			{
				Helpers.GameObjectSetActive(classIconParent.gameObject, value: false);
			}
			if (!HelpersUI.SetSprite(bonusRewardIcon, HelpersGfx.GetGuildBattleBuffIconName(guildSectorBattleTraitBonus[0])))
			{
				HelpersUI.SetSprite(bonusRewardIcon, "Ui_Icon_Trait_Dodge");
			}
			Helpers.GameObjectSetActive(base.gameObject, value: true);
		}
		else
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
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
