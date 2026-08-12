using TWDModel;
using UnityEngine;

public class ScavengeMissionButton : NUIGridItem
{
	[Header("Cost")]
	[SerializeField]
	private GameObject costParent;

	[SerializeField]
	private UILabel costAmount;

	[SerializeField]
	private UISprite costSprite;

	[Header("Main Button")]
	[SerializeField]
	private UIButtonWithLabelAndIcon button;

	[Header("Tweens Configuration")]
	[SerializeField]
	private int normalTweenGroup = 10;

	[SerializeField]
	private int hardTweenGroup = 20;

	[SerializeField]
	private float delayBeforeUpdate = 0.25f;

	[Header("When Equipment Tweens")]
	[Tooltip("The equipmemt label text does not change between normal and hard. That is why the tween are removed")]
	[SerializeField]
	private UILabel removeLabelTweens;

	private GrindButtonDefinition definition;

	private DropCurrenciesStaticDefinition dropDefinition;

	private bool updateDifficultyTweens;

	private bool firstUpdate = true;

	public virtual void Init(GrindButtonDefinition newDefinition, HUDElement parent, UIButtonExtended.OnClickCallback clickCallback)
	{
		if (newDefinition == null)
		{
			return;
		}
		updateDifficultyTweens = definition == null || definition.GrindDifficulty != newDefinition.GrindDifficulty;
		definition = newDefinition;
		dropDefinition = GameManager.Instance.gameEconomyData.GetDropCurrencyStaticDefinition(definition.LootTag, definition.GetMissionLevel(GameManager.Instance.playerModel));
		dropDefinition = GameManager.Instance.playerModel.ActivityManager.ModifyActivityDefinition(dropDefinition);
		base.name = definition.DisplayOrder + "_" + definition.PrefabName + "(Prefab)";
		if (button != null)
		{
			button.SetClickCallback(clickCallback);
		}
		if (removeLabelTweens != null && definition != null && definition.LootTag == DropEventDefinition.DropEventTag.PreferEquipment)
		{
			UITweener[] components = removeLabelTweens.GetComponents<UITweener>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i] != null)
				{
					Object.Destroy(components[i]);
				}
			}
		}
		Init();
	}

	private int FindScavengeCost()
	{
		ConfigData configData = GameManager.Instance.gameEconomyData.ConfigData;
		int level = GameManager.Instance.playerModel.Level;
		if (GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.UnlimitedGas))
		{
			return 0;
		}
		return configData.GetGrindMissionCost(level);
	}

	public virtual void UpdateUI()
	{
		if (updateDifficultyTweens && definition != null)
		{
			int num = -1;
			if (definition.GrindDifficulty == GrindButtonDefinition.Difficulty.Normal)
			{
				num = normalTweenGroup;
			}
			else if (definition.GrindDifficulty == GrindButtonDefinition.Difficulty.Hard)
			{
				num = hardTweenGroup;
			}
			if (firstUpdate)
			{
				firstUpdate = false;
				TweenManager.PlayTweenGroup(base.gameObject, num, forward: true, null, resetToEnd: true);
				UpdateAfterTween();
			}
			else
			{
				TweenManager.PlayTweenGroup(base.gameObject, num);
				Invoke("UpdateAfterTween", delayBeforeUpdate);
			}
			updateDifficultyTweens = false;
		}
	}

	public void UpdateAfterTween()
	{
		if (button != null && definition != null)
		{
			button.SetContentToLabelOne(LocalizationManager.GetText(definition.TitleLocalizationKey));
			if (string.IsNullOrEmpty(definition.IconSpriteOverride))
			{
				button.SetContentToIconOne(HelpersGfx.GetSpriteNameForLootType(definition.LootTag));
			}
			else
			{
				button.SetContentToIconOne(definition.IconSpriteOverride);
			}
			if (definition.LootTag == DropEventDefinition.DropEventTag.PreferSP)
			{
				button.SetContentToLabelTwo(dropDefinition.MinSurvivalPoints.ToString());
			}
			else if (definition.LootTag == DropEventDefinition.DropEventTag.PreferSupplies)
			{
				button.SetContentToLabelTwo(dropDefinition.MinSupplies.ToString());
			}
			else if (definition.LootTag == DropEventDefinition.DropEventTag.PreferEquipment)
			{
				button.SetContentToLabelTwo(LocalizationManager.GetText("Popup.Scavenge.GrindButton.GetGear"));
			}
			else
			{
				button.SetContentToLabelTwo("");
			}
			HelpersUI.SetContentToLabel(costAmount, FindScavengeCost().ToString());
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (button != null)
		{
			button.Clear();
		}
	}

	public virtual GrindButtonDefinition GetDefinition()
	{
		return definition;
	}
}
