using BaseModel;
using Mono.WebBrowser;
using System;
using TWDModel;

public class HeroUnlockHelper
{
	public static bool UnlockHero(ActorDefinition actorDefinition, Callback OnUnlockClose = null)
	{
		if (actorDefinition != null && actorDefinition.TokensToUnlock <= GameManager.Instance.playerModel.GetCurrency(actorDefinition.TraitUpgradeCurrency).Value && Helpers.ExecuteCommand(new UnlockHeroCommand(actorDefinition.TraitUpgradeCurrency)) == TWDModelResult.OK)
		{
			if (OfflineManager.IsLoadDataManager) return true;

			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/hero_unlock");
			SurvivorModel survivorWithActorDefinition = GetSurvivorWithActorDefinition(actorDefinition);
			if (survivorWithActorDefinition != null)
			{
				SurvivorInfoPopup survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
				survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorHeroUnlock;
				survivorInfoPopup.OpenForModel(survivorWithActorDefinition);
				if (OnUnlockClose != null)
				{
					survivorInfoPopup.OnCloseCallback = (Callback)Delegate.Remove(survivorInfoPopup.OnCloseCallback, OnUnlockClose);
					survivorInfoPopup.OnCloseCallback = (Callback)Delegate.Combine(survivorInfoPopup.OnCloseCallback, OnUnlockClose);
				}
				GameManager.Instance.RequestPltv();
				return true;
			}
		}
		return false;
	}

	private static SurvivorModel GetSurvivorWithActorDefinition(ActorDefinition actorDefinition)
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			ModelList<SurvivorModel> survivors = GameManager.Instance.playerModel.SurvivorContainer.Survivors;
			for (int i = 0; i < survivors.Count; i++)
			{
				if (survivors[i] != null && survivors[i].ActorDefinitionID.Equals(actorDefinition.ID))
				{
					return survivors[i];
				}
			}
		}
		return null;
	}

	public static SurvivorModel GetOrCreateMockSurvivorModel(ActorDefinition actorDef)
	{
		return ObjectCacheKeyValue<SurvivorModel, ActorDefinition>.Get(actorDef.ID, CreateMockSurvivorModel, actorDef);
	}

	private static SurvivorModel CreateMockSurvivorModel(ActorDefinition actorDef)
	{
		int num = GameManager.Instance.playerModel.SurvivorContainer.GetHighestLevelSurvivor() + actorDef.InitialLevelOffset;
		SurvivorModel survivorModel = GameManager.Instance.playerModel.SurvivorContainer.CreateSurvivorFromDefinition(actorDef.ID, num, num, actorDef.RarityLevel, num, actorDef.InitialEquipmentRarityLevel, new ModelRandom(), actorDef.InitialEquipmentsData[0].ID, actorDef.InitialEquipmentsData[1].ID, isMock: true);
		survivorModel.SetupMockTraits();
		if (!OfflineManager.IsLoadDataManager) ActorView.PrepareActor(survivorModel, isTransient: true);
		return survivorModel;
	}
}
