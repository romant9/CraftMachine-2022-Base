using TWDModel;
using UnityEngine;

public class TooltipSupportSkill : TooltipBox
{
	[SerializeField]
	private UISprite typeSprite;

	[SerializeField]
	private UILabel typeLabel;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel cooldownLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UISprite skillSprite;

	[SerializeField]
	private UIScrollBar scrollBar;

	public override void Update()
	{
	}

	public void Set(SupportModel supportModel)
	{
		if (supportModel != null)
		{
			nameLabel.text = HelpersLocalization.GetSupportName(supportModel.SupportId);
			typeLabel.text = LocalizationManager.GetText("BattleTips_Type_Support");
			typeSprite.spriteName = "Ui_Bg_Trait_ActorSkill";
			string supportId = supportModel.SupportId;
			if (supportModel.definition.Category == 1)
			{
				cooldownLabel.text = LocalizationManager.GetText("Support.Cooldown_Type1");
			}
			else if (supportModel.definition.Category == 0)
			{
				cooldownLabel.text = HelpersLocalization.GetSupportCooldownText(supportModel.Cooldown);
			}
			descriptionLabel.text = HelpersLocalization.GetSupportSkillDescription(supportModel);
			skillSprite.spriteName = HelpersGfx.GetSupportSkillIconName(supportId);
			if (scrollBar != null)
			{
				scrollBar.value = 0f;
			}
		}
	}

	public void SetSurvivor()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null)
		{
			return;
		}
		ActorModel activeActor = combat.ActiveActor;
		BaseCommandSkill actorCommandSkill = activeActor.CommandSkillModelManager.ActorCommandSkill;
		if (actorCommandSkill == null)
		{
			return;
		}
		TraitEntry traitAnyLevel = activeActor.TraitContainer.GetTraitAnyLevel("LeaderBuffSurvivalGame");
		if (traitAnyLevel == null)
		{
			return;
		}
		TraitDefinition traitDefinition = combat.gameEconomyData.GetTraitDefinition(traitAnyLevel.TraitIdentifier);
		if (traitDefinition != null)
		{
			nameLabel.text = LocalizationManager.GetText(actorCommandSkill.Definition.Name);
			typeLabel.text = LocalizationManager.GetText("BattleTips_Type_ActorSkill");
			typeSprite.spriteName = "Ui_Bg_Trait_ActorSkill";
			FixedPoint value = 0.0;
			combat.manager.Player.AbilityManager.VisitParameter("LeaderBuffSurvivalGame_CDTurns", ref value, activeActor);
			cooldownLabel.text = LocalizationManager.GetText("CommandSkillCoolDownInfo{Parameter}", (int)value);
			descriptionLabel.text = HelpersLocalization.GetLeaderTraitTeamDescription(traitDefinition);
			skillSprite.spriteName = actorCommandSkill.Definition.Icon;
			if (scrollBar != null)
			{
				scrollBar.value = 0f;
			}
		}
	}

	public void SetShadowedGuard()
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null)
		{
			return;
		}
		ActorModel activeActor = combat.ActiveActor;
		BaseCommandSkill actorCommandSkill = activeActor.CommandSkillModelManager.ActorCommandSkill;
		if (actorCommandSkill == null)
		{
			return;
		}
		TraitEntry traitAnyLevel = activeActor.TraitContainer.GetTraitAnyLevel("LeaderBuffShadowedGuard");
		if (traitAnyLevel == null)
		{
			return;
		}
		TraitDefinition traitDefinition = combat.gameEconomyData.GetTraitDefinition(traitAnyLevel.TraitIdentifier);
		if (traitDefinition != null)
		{
			nameLabel.text = LocalizationManager.GetText(actorCommandSkill.Definition.Name);
			typeLabel.text = LocalizationManager.GetText("BattleTips_Type_ActorSkill");
			typeSprite.spriteName = "Ui_Bg_Trait_ActorSkill";
			FixedPoint value = 0.0;
			combat.manager.Player.AbilityManager.VisitParameter("LeaderBuffShadowedGuard_CDTurns", ref value, activeActor);
			cooldownLabel.text = LocalizationManager.GetText("CommandSkillCoolDownInfo{Parameter}", (int)value);
			descriptionLabel.text = HelpersLocalization.GetTraitDescription(traitDefinition);
			skillSprite.spriteName = actorCommandSkill.Definition.Icon;
			if (scrollBar != null)
			{
				scrollBar.value = 0f;
			}
		}
	}
}
