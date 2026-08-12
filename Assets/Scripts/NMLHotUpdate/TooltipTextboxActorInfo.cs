using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class TooltipTextboxActorInfo : TooltipTextbox
{
	[Header("Header")]
	[SerializeField]
	private GameObject HeaderBackground;

	[SerializeField]
	private UISprite HeaderIcon;

	[SerializeField]
	private UILabel HeaderLabel;

	[SerializeField]
	private float HeaderLabelNoLevelPosX = 25f;

	private float HeaderLabelInitialPosX;

	[SerializeField]
	[Header("Main Content")]
	private UILabel StatusLabel;

	[Header("Resize Height Desc")]
	[SerializeField]
	private UILabel DescLabel;

	[Header("Health Content")]
	[SerializeField]
	private UISprite HealthIcon;

	[SerializeField]
	private UILabel HealthLabel;

	[Header("Additional Content")]
	[SerializeField]
	private UISprite ToughWalkerIcon;

	[SerializeField]
	private UISprite BurningIcon;

	[SerializeField]
	private UISprite ExplodingIcon;

	[SerializeField]
	private UISprite CoverIcon;

	[SerializeField]
	private UISprite BossWalkerIcon;

	private void Awake()
	{
		if (HeaderLabel != null)
		{
			HeaderLabelInitialPosX = HeaderLabel.transform.localPosition.x;
		}
	}

	public void SetParamAndValuesTexts(ActorModel actor)
	{
		CombatModel combatModel = actor.manager.CombatModel;
		string healthbarClassIconName = HelpersGfx.GetHealthbarClassIconName(actor);
		string text = Enum.GetName(typeof(Faction), actor.Faction);
		string text2 = "";
		text2 = ((combatModel == null || !combatModel.HasPvPRules || actor.Faction != Faction.Raider) ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(text + ".Class." + actor.Definition.Class) : SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("PvP.Class." + actor.Definition.Class));
		string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Level{Level}", actor.Level);
		string localizedText2 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("AIAlertness." + Enum.GetName(typeof(AIAlertness), actor.AIDataModel.Alertness));
		int hitpoints = actor.Hitpoints;
		int maxHitPoints = actor.MaxHitPoints;
		bool active = false;
		bool active2 = false;
		string text3 = "";
		List<string> list = new List<string>();
		List<TraitEntry> traits = actor.GetTraits();
		bool flag = actor.IsBePoisoned();
		bool isRemoteWeakened = actor.IsRemoteWeakened;
		int astheniaLeftTurns = actor.GetAstheniaLeftTurns();
		List<Faction> beGrenadeFragmentDamagedList = actor.GetBeGrenadeFragmentDamagedList();
		if (traits != null)
		{
			for (int i = 0; i < traits.Count; i++)
			{
				TraitEntry traitEntry = traits[i];
				string text4 = "CombatToolitp." + traitEntry.TraitIdentifier;
				if (!list.Contains(text4) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text4))
				{
					list.Add(text4);
				}
				if (traitEntry.TraitIdentifier.ToLower() == "Burning".ToLower())
				{
					active = true;
				}
				else if (traitEntry.TraitIdentifier.ToLower().Contains("Explosive".ToLower()))
				{
					active2 = true;
				}
				else
				{
					_ = traitEntry.TraitIdentifier.ToLower() == "Bleeding".ToLower();
				}
			}
			if (flag)
			{
				list.Add("CombatToolitp.Poison");
			}
			if (isRemoteWeakened)
			{
				list.Add("CombatToolitp.RemoteWeakened");
			}
			if (astheniaLeftTurns > 0)
			{
				list.Add("CombatToolitp.Asthenia");
			}
			if (beGrenadeFragmentDamagedList.Count > 0)
			{
				list.Add("CombatToolitp.GrenadeFragmentDamage");
			}
		}
		if (actor.IsTaunted)
		{
			string text5 = "CombatToolitp.Taunted";
			if (!list.Contains(text5) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text5))
			{
				list.Add(text5);
			}
		}
		if (actor.IsSneak)
		{
			string text6 = "CombatToolitp.Sneak";
			if (!list.Contains(text6) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text6))
			{
				list.Add(text6);
			}
		}
		if (actor.IsHeirloomsHershelFetter)
		{
			string text7 = "ActorNotification.IgniteBoost";
			if (!list.Contains(text7) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text7))
			{
				list.Add(text7);
			}
		}
		if (actor.IsQuantuned)
		{
			string text8 = "CombatToolitp.Quantun";
			if (!list.Contains(text8) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text8))
			{
				list.Add(text8);
			}
		}
		if (actor.IsMomentum())
		{
			string text9 = "CombatToolitp.Momentum";
			if (!list.Contains(text9) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text9))
			{
				list.Add(text9);
			}
		}
		if (actor.IsRiposte())
		{
			string text10 = "CombatToolitp.Riposte";
			if (!list.Contains(text10) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text10))
			{
				list.Add(text10);
			}
		}
		if (actor.IsSurvivalGameEnemy())
		{
			string text11 = "CombatToolitp.SurvivalGame";
			if (!list.Contains(text11) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text11))
			{
				list.Add(text11);
			}
		}
		if (actor.IsSurvivalGameEnemy())
		{
			string text12 = "CombatToolitp.OutOfTower";
			if (!list.Contains(text12) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text12))
			{
				list.Add(text12);
			}
		}
		if (actor.HasAnyLevelTrait("SurvivalManualStorySkill_D"))
		{
			string text13 = "CombatToolitp.SurvivalManualStorySkill_D";
			if (!list.Contains(text13) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text13))
			{
				list.Add(text13);
			}
		}
		if (actor.HasAnyLevelTrait("SurvivalManualStorySkill_E"))
		{
			string text14 = "CombatToolitp.SurvivalManualStorySkill_E";
			if (!list.Contains(text14) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text14))
			{
				list.Add(text14);
			}
		}
		if (actor.HasAnyLevelTrait("SurvivalManualStorySkill_F"))
		{
			string text15 = "CombatToolitp.SurvivalManualStorySkill_F";
			if (!list.Contains(text15) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text15))
			{
				list.Add(text15);
			}
		}
		if (actor.HasAnyLevelTrait("SurvivalManualStorySkill_H"))
		{
			string text16 = "CombatToolitp.SurvivalManualStorySkill_H";
			if (!list.Contains(text16) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text16))
			{
				list.Add(text16);
			}
		}
		if (actor.HasAnyLevelTrait("SurvivalManualStorySkill_I"))
		{
			string text17 = "CombatToolitp.SurvivalManualStorySkill_I";
			if (!list.Contains(text17) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text17))
			{
				list.Add(text17);
			}
		}
		if (actor.DeathsDoor_DmgUpLayer > 0)
		{
			string text18 = "CombatToolitp.DamageStacking";
			if (!list.Contains(text18) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text18))
			{
				list.Add(text18);
			}
		}
		if (actor.DeadlyFocusLeftCount_SourceSurvivor > 0 || actor.DeadlyFocusLeftCount_SourceRaider > 0)
		{
			string text19 = "Traits.LeaderBuffDeadlyFocus";
			if (!list.Contains(text19) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text19))
			{
				list.Add(text19);
			}
		}
		if (actor.Abilities != null)
		{
			for (int j = 0; j < actor.Abilities.Count; j++)
			{
				AbilityModel abilityModel = actor.Abilities[j];
				string text20 = "CombatToolitp." + abilityModel.DefinitionID;
				if (!list.Contains(text20) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text20))
				{
					list.Add(text20);
				}
			}
		}
		if (actor.EquipmentItems != null)
		{
			for (int k = 0; k < actor.EquipmentItems.Count; k++)
			{
				EquipmentItemModel equipmentItemModel = actor.EquipmentItems[k];
				if (equipmentItemModel.Definition.ActiveTraits != null)
				{
					List<string> equipmentActiveTraits = equipmentItemModel.GetEquipmentActiveTraits();
					for (int l = 0; l < equipmentActiveTraits.Count; l++)
					{
						string text21 = equipmentActiveTraits[l];
						string text22 = "CombatToolitp." + text21;
						if (!list.Contains(text22) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text22))
						{
							list.Add(text22);
						}
					}
				}
				List<UpgradeTraitsData> availableTraits = equipmentItemModel.GetAvailableTraits();
				for (int m = 0; m < availableTraits.Count; m++)
				{
					string identifier = availableTraits[m].Identifier;
					string text23 = "CombatToolitp." + identifier;
					if (!list.Contains(text23) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text23))
					{
						list.Add(text23);
					}
				}
			}
		}
		CoverIconState coverIconState = CoverIconState.None;
		if ((!actor.IsWalker || !actor.IsEnvironmental) && combatModel.HasCover(actor.GridCoordinate))
		{
			if (combatModel.IsCoverFlanked(actor.GridCoordinate, actor))
			{
				list.Insert(0, "CombatToolitp.InCover_Flanked");
				coverIconState = CoverIconState.Flanked;
			}
			else
			{
				list.Insert(0, "CombatToolitp.InCover");
				coverIconState = CoverIconState.HalfCover;
			}
		}
		if (CoverIcon != null)
		{
			if (coverIconState == CoverIconState.None)
			{
				CoverIcon.gameObject.SetActive(value: false);
			}
			else
			{
				CoverIcon.spriteName = HelpersGfx.GetCoverIconName(coverIconState);
				CoverIcon.gameObject.SetActive(value: true);
			}
		}
		if (combatModel != null && combatModel.HasPvPRules && actor.Faction == Faction.Raider)
		{
			switch (coverIconState)
			{
			case CoverIconState.Flanked:
				text3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("CombatToolitp.InCover_Flanked");
				text3 += "\n";
				break;
			case CoverIconState.HalfCover:
				text3 = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("CombatToolitp.InCover");
				text3 += "\n";
				break;
			}
			text3 += SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Tooltip.PvPDefender");
		}
		else
		{
			for (int n = 0; n < list.Count; n++)
			{
				text3 += SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText(list[n]);
				if (n < list.Count - 1)
				{
					text3 += "\n";
				}
			}
		}
		if (HeaderIcon != null)
		{
			HeaderIcon.spriteName = healthbarClassIconName;
		}
		if (HeaderLabel != null)
		{
			HeaderLabel.text = (actor.IsEnvironmental ? "" : localizedText);
			Vector3 localPosition = HeaderLabel.transform.localPosition;
			HeaderLabel.transform.localPosition = new Vector3(actor.IsEnvironmental ? HeaderLabelNoLevelPosX : HeaderLabelInitialPosX, localPosition.y, localPosition.z);
		}
		if (HeaderBackground != null)
		{
			HeaderBackground.SetActive(!actor.IsEnvironmental);
		}
		if (ToughWalkerIcon != null)
		{
			ToughWalkerIcon.gameObject.SetActive(actor.IsBoss);
		}
		if (BurningIcon != null)
		{
			BurningIcon.gameObject.SetActive(active);
		}
		if (ExplodingIcon != null)
		{
			ExplodingIcon.gameObject.SetActive(active2);
		}
		if (BossWalkerIcon != null)
		{
			BossWalkerIcon.gameObject.SetActive(actor.IsBossWalker);
		}
		SetText(text2);
		if (StatusLabel != null)
		{
			StatusLabel.text = localizedText2;
		}
		if (DescLabel != null)
		{
			DescLabel.text = text3;
		}
		if (HealthLabel != null)
		{
			HealthLabel.text = hitpoints + "/" + maxHitPoints;
		}
		Position();
	}

	private void LateUpdate()
	{
		Overlay();
	}

	public override void Update()
	{
		if (PlayerInputManager.Instance != null && PlayerInputManager.Instance.IsDragging)
		{
			TooltipManager.HideAll();
		}
		else
		{
			base.Update();
		}
	}
}
