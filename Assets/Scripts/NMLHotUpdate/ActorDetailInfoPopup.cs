using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ActorDetailInfoPopup : HUDElement
{
	[SerializeField]
	private UISprite iconSprite;

	[SerializeField]
	private UISprite bossBg;

	[SerializeField]
	private UISprite actorBg;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel hpLabel;

	[SerializeField]
	private UILabel segmentedHPHpLabel;

	[SerializeField]
	private UIProgressBar hpProgressBar;

	[SerializeField]
	private UILabel shieldLabel;

	[SerializeField]
	private UIProgressBar shieldProgressBar;

	[SerializeField]
	private UILabel defenceValueLabel;

	[SerializeField]
	private GameObject detailContainer;

	[SerializeField]
	private GameObject titleGo;

	[SerializeField]
	private GameObject specicalGo;

	[SerializeField]
	private GameObject effectGo;

	[SerializeField]
	private GameObject textGo;

	[SerializeField]
	private UITable DetailTable;

	[SerializeField]
	private GameObject defenseDetailTooltipTarget;

	private List<GameObject> spawnedEffectEntries = new List<GameObject>();

	public override void Open()
	{
		if (!(TutorialView.Instance != null) || !TutorialView.Instance.RunningButNotSuggesting)
		{
			base.Open();
			UpdateUI();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (!IsModelRequestedType<ActorModel>())
		{
			return;
		}
		ActorModel actorModel = GetModel<ActorModel>();
		if (actorModel.manager != null)
		{
			_ = actorModel.manager.CombatModel;
		}
		if (bossBg != null && actorBg != null)
		{
			Helpers.GameObjectSetActive(bossBg.gameObject, actorModel.Definition.Class == "Boss");
			Helpers.GameObjectSetActive(actorBg.gameObject, actorModel.Definition.Class != "Boss");
		}
		if (nameLabel != null && iconSprite != null)
		{
			if (actorModel.Definition.BossType != BossType.Any)
			{
				nameLabel.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("ActorName." + actorModel.Definition.BossType);
				iconSprite.width = 100;
				iconSprite.height = 100;
			}
			else
			{
				nameLabel.text = actorModel.Name;
				iconSprite.width = 80;
				iconSprite.height = 80;
			}
			iconSprite.spriteName = actorModel.Definition.NormalHead;
		}
		if (levelLabel != null)
		{
			bool flag = !actorModel.IsEnvironmental;
			Helpers.GameObjectSetActive(levelLabel.gameObject, flag);
			if (flag)
			{
				HelpersUI.SetContentToLabel(levelLabel, "Lv. " + actorModel.Level);
			}
		}
		int hitpoints = actorModel.Hitpoints;
		int maxHitPoints = actorModel.MaxHitPoints;
		if (hpLabel != null)
		{
			hpLabel.text = Helpers.FormatNumber(hitpoints, 0, 1);
		}
		if (hpProgressBar != null)
		{
			hpProgressBar.value = ((maxHitPoints > 0) ? ((float)hitpoints / (float)maxHitPoints) : 0f);
		}
		if (segmentedHPHpLabel != null)
		{
			bool isSegmentedHP = actorModel.IsSegmentedHP;
			Helpers.GameObjectSetActive(segmentedHPHpLabel.gameObject, isSegmentedHP);
			segmentedHPHpLabel.text = (isSegmentedHP ? ("×" + actorModel.SegmentedHPCount) : string.Empty);
		}
		bool flag2 = actorModel.MaxShieldHitPoints > 0;
		if (shieldProgressBar != null)
		{
			Helpers.GameObjectSetActive(shieldProgressBar.gameObject, flag2);
			if (flag2)
			{
				shieldProgressBar.value = (float)actorModel.ShieldHitPoints / (float)actorModel.MaxShieldHitPoints;
			}
		}
		if (shieldLabel != null)
		{
			Helpers.GameObjectSetActive(shieldLabel.gameObject, flag2);
			if (flag2)
			{
				shieldLabel.text = actorModel.ShieldHitPoints + "/" + actorModel.MaxShieldHitPoints;
			}
		}
		_ = actorModel.GuildBossDefense;
		if (defenceValueLabel != null)
		{
			defenceValueLabel.text = actorModel.GuildBossDefense.ToString();
		}
		ClearActorDetailInfo();
		ShowSpecialInfo();
		ShowAttackInfo();
		ShowBuffInfo();
		DetailTable.Reposition();
	}

	private void ClearActorDetailInfo()
	{
		spawnedEffectEntries.ForEach(delegate(GameObject item)
		{
			UnityEngine.Object.Destroy(item);
		});
		spawnedEffectEntries.Clear();
	}

	private void ShowSpecialInfo()
	{
		ActorModel actorModel = GetModel<ActorModel>();
		if (actorModel == null)
		{
			return;
		}
		if (actorModel.Definition != null && actorModel.Definition.TagDisplay != null && actorModel.Definition.TagDisplay.Count > 0)
		{
			AddTitleInfo("ActorDetailInfo.Special");
		}
		for (int i = 0; i < actorModel.Definition.TagDisplay.Count; i++)
		{
			GameObject gameObject = detailContainer.AddChild(specicalGo);
			GuildBossEntryInfoItem component = gameObject.GetComponent<GuildBossEntryInfoItem>();
			if (!(component == null))
			{
				string[] array = actorModel.Definition.TagDisplay[i].Split(':');
				if (array.Length >= 2)
				{
					component.UpdateUI(array[0], array[1]);
					spawnedEffectEntries.Add(gameObject);
				}
			}
		}
	}

	private void ShowAttackInfo()
	{
		ActorModel actorModel = GetModel<ActorModel>();
		if (actorModel?.Definition?.SkillShow == null || actorModel.Definition.SkillShow.Count == 0)
		{
			return;
		}
		AddTitleInfo("ActorDetailInfo.Attack");
		for (int i = 0; i < actorModel.Definition.SkillShow.Count; i++)
		{
			string[] array = actorModel.Definition.SkillShow[i].Split(':');
			if (array.Length >= 3)
			{
				GameObject gameObject = detailContainer.AddChild(effectGo);
				ActorEffectInfoItem component = gameObject.GetComponent<ActorEffectInfoItem>();
				if (!(component == null))
				{
					component.UpdateUI(array[0], array[1], array[2]);
					spawnedEffectEntries.Add(gameObject);
				}
			}
		}
	}

	private void ShowBuffInfo()
	{
		ActorModel actorModel = GetModel<ActorModel>();
		ActorView actorView = GameManager.Instance.GetViewForModel(actorModel) as ActorView;
		actorView?.RefreshHealthBarEffectCache();
		AddTitleInfo("ActorDetailInfo.Buff");
		string localizedText = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("AIAlertness." + Enum.GetName(typeof(AIAlertness), actorModel.AIDataModel.Alertness));
		AddTextInfo(localizedText);
		List<TraitEntry> traits = actorModel.GetTraits();
		List<string> list = new List<string>();
		for (int i = 0; i < traits.Count; i++)
		{
			TraitEntry traitEntry = traits[i];
			string text = "CombatDetailsToolitp." + traitEntry.TraitIdentifier;
			if (!list.Contains(text) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text))
			{
				list.Add(text);
				AddTextInfo(text);
			}
		}
		if (actorModel.Abilities != null)
		{
			for (int j = 0; j < actorModel.Abilities.Count; j++)
			{
				AbilityModel abilityModel = actorModel.Abilities[j];
				string text2 = "CombatToolitp." + abilityModel.DefinitionID;
				if (!list.Contains(text2) && SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text2))
				{
					list.Add(text2);
					AddTextInfo(text2);
				}
			}
		}
		IReadOnlyList<ActorEffectInfoData> readOnlyList = actorView?.CachedHealthBarEffects;
		if (readOnlyList == null || readOnlyList.Count == 0)
		{
			return;
		}
		for (int k = 0; k < readOnlyList.Count; k++)
		{
			GameObject gameObject = detailContainer.AddChild(effectGo);
			ActorEffectInfoItem component = gameObject.GetComponent<ActorEffectInfoItem>();
			if (!(component == null))
			{
				component.UpdateUI(readOnlyList[k]);
				spawnedEffectEntries.Add(gameObject);
			}
		}
	}

	private void AddTitleInfo(string title)
	{
		GameObject gameObject = detailContainer.AddChild(titleGo);
		UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
		if (componentInChildren != null)
		{
			HelpersUI.SetContentToLabel(componentInChildren, LocalizationManager.GetText(title));
			spawnedEffectEntries.Add(gameObject);
		}
	}

	private void AddTextInfo(string text)
	{
		GameObject gameObject = detailContainer.AddChild(textGo);
		UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
		if (componentInChildren != null)
		{
			HelpersUI.SetContentToLabel(componentInChildren, LocalizationManager.GetText(text));
			spawnedEffectEntries.Add(gameObject);
		}
	}

	public void OnDefenseDetailClicked()
	{
		ShowLocalizedTooltip(LocalizationManager.GetText("Battle_CharStatePanel_DefInfo"), defenseDetailTooltipTarget);
	}

	public void ShowLocalizedTooltip(string localizationKey, GameObject target = null, params object[] localizationArgs)
	{
		if (!string.IsNullOrEmpty(localizationKey))
		{
			TooltipManager.OpenTextBoxWithText((target != null) ? target : base.gameObject, localizationKey);
		}
	}
}
