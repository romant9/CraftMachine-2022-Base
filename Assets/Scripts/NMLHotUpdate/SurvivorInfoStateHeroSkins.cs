using TWDModel;

public class SurvivorInfoStateHeroSkins : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorHeroSkins;
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}

	public override void Enter()
	{
		base.Enter();
		if (base.SurvivorModel != null && base.HeroSkinsView != null)
		{
			HeroSkinResourceEntry heroSkinResourceEntry = GameManager.Instance.GetHeroSkinResourceEntry(base.SurvivorModel.Definition.ID);
			base.HeroSkinsView.Show(heroSkinResourceEntry, base.SurvivorModel);
		}
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Hide);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Hide);
	}

	public override void Exit()
	{
		HandleExit();
		base.Exit();
	}

	private void HandleExit()
	{
		bool flag = base.HeroSkinsView.HeroSkinInfo != null && GameManager.Instance.playerModel.SurvivorContainer.HeroSkinsOwned.Contains(base.HeroSkinsView.HeroSkinInfo.PrefabId);
		bool num = base.HeroSkinsView.OriginalHeroSkin != null && GameManager.Instance.playerModel.SurvivorContainer.HeroSkinsOwned.Contains(base.HeroSkinsView.OriginalHeroSkin.PrefabId);
		HeroSkinInfo heroSkinInfo = null;
		if (!num)
		{
			HeroSkinDefinition[] heroSkinDefinitions = GameManager.Instance.gameEconomyData.HeroSkinDefinitions;
			foreach (HeroSkinDefinition heroSkinDefinition in heroSkinDefinitions)
			{
				if (heroSkinDefinition.HeroID == base.SurvivorModel.ActorDefinitionID && GameManager.Instance.playerModel.SurvivorContainer.HeroSkinsOwned.Contains(heroSkinDefinition.ID))
				{
					heroSkinInfo = GameManager.Instance.GetHeroSkinInfoEntry(heroSkinDefinition.ID);
					break;
				}
			}
		}
		else
		{
			heroSkinInfo = base.HeroSkinsView.OriginalHeroSkin;
		}
		if (base.HeroSkinsView != null && heroSkinInfo != null && !flag)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.PermanentlySwitchToSkin(heroSkinInfo, delegate
			{
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.close();
			});
		}
	}
}
