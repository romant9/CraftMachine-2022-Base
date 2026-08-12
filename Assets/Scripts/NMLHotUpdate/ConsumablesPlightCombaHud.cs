using TWDModel;
using UnityEngine;

public class ConsumablesPlightCombaHud : HUDElement
{
	private enum PlightMode
	{
		Plight = 0,
		Effect = 1
	}

	[SerializeField]
	private PlightConsumableListPanel consumableListPanel;

	[SerializeField]
	private PlightConsumableListPanel consumableListPanel2;

	[SerializeField]
	private PlightConsumableEffectListPanel consumableEffectListPanel;

	[SerializeField]
	private PlightConsumableListPanel consumableHeroListPanel;

	private bool showDefalt = true;

	[SerializeField]
	private UIWidget plightContainer;

	[SerializeField]
	private UIWidget plightEffectContainer;

	[SerializeField]
	private UIWidget plightBackContainer;

	[SerializeField]
	private UIWidget plightHeroContainer;

	[SerializeField]
	private UIButtonToggle plightToggle;

	[SerializeField]
	private UIButtonToggle plightEffectToggle;

	[SerializeField]
	private UITexture plightBg;

	[SerializeField]
	private UITexture plightIcon;

	[SerializeField]
	private UITexture plightEffectBg;

	[SerializeField]
	private UITexture plightEffectIcon;

	public override void Open()
	{
		base.Open();
		ApocalypseWeeklyChallengeModel weeklyApocalypticChallengeModel = WeeklyChallengeHelper.GetWeeklyApocalypticChallengeModel();
		if (weeklyApocalypticChallengeModel != null && WeeklyChallengeHelper.IsChallengeOngoing() && consumableListPanel != null && weeklyApocalypticChallengeModel.CurrentCircleDefinition != null)
		{
			consumableListPanel.Init(weeklyApocalypticChallengeModel.CurrentCircleDefinition.DebuffConfigs);
			if (weeklyApocalypticChallengeModel.CurrentCircleDefinition.LTDebuffs != null && weeklyApocalypticChallengeModel.CurrentCircleDefinition.LTDebuffs.Count > 0)
			{
				showDefalt = false;
				consumableListPanel2.Init(weeklyApocalypticChallengeModel.CurrentCircleDefinition.DebuffConfigs);
				consumableHeroListPanel.InitHero(weeklyApocalypticChallengeModel.CurrentCircleDefinition.LTDebuffs);
			}
		}
		if (weeklyApocalypticChallengeModel != null && WeeklyChallengeHelper.IsChallengeOngoing() && consumableEffectListPanel != null)
		{
			consumableEffectListPanel.Init(weeklyApocalypticChallengeModel.weeklyChallengeApocalypseBuffs);
		}
		SetToggle(PlightMode.Plight);
		Helpers.GameObjectSetActive(plightEffectToggle, weeklyApocalypticChallengeModel != null && weeklyApocalypticChallengeModel.weeklyChallengeApocalypseBuffs?.Count > 0);
	}

	public void OnClickPlight()
	{
		SetToggle(PlightMode.Plight);
	}

	public void OnClickPlightEffect()
	{
		SetToggle(PlightMode.Effect);
	}

	private void SetToggle(PlightMode drawCardMode)
	{
		plightContainer.alpha = 0f;
		plightEffectContainer.alpha = 0f;
		plightBackContainer.alpha = 0f;
		plightHeroContainer.alpha = 0f;
		plightToggle.SetToggled(toggled: false);
		plightEffectToggle.SetToggled(toggled: false);
		plightBg.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_Label_Unselect", "itemgraphics");
		plightEffectBg.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_Label_Unselect", "itemgraphics");
		plightIcon.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_Debuff_Unselect", "itemgraphics");
		plightEffectIcon.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_buff_Unselect", "itemgraphics");
		switch (drawCardMode)
		{
		case PlightMode.Plight:
			if (showDefalt)
			{
				plightContainer.alpha = 1f;
			}
			else
			{
				plightBackContainer.alpha = 1f;
				plightHeroContainer.alpha = 1f;
			}
			plightToggle.SetToggled(toggled: true);
			plightBg.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_Label", "itemgraphics");
			plightIcon.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_Debuff", "itemgraphics");
			break;
		case PlightMode.Effect:
			plightEffectContainer.alpha = 1f;
			plightEffectToggle.SetToggled(toggled: true);
			plightEffectBg.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_Label", "itemgraphics");
			plightEffectIcon.mainTexture = (Texture)UnityUtils.LoadFromAssetBundle("Icon_Challenge_Apocalyptic_Bufflist_buff", "itemgraphics");
			break;
		}
	}
}
