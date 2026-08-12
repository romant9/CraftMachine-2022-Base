namespace TWDModel
{
	public class Migration3900 : TWDModelMigration
	{
		public Migration3900()
		{
			base.Version = "3.9.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.TDogToken, CurrencyType.ShaneToken);
			for (int i = 0; i < player.SurvivorContainer.Survivors.Count; i++)
			{
				SurvivorModel survivorModel = player.SurvivorContainer.Survivors[i];
				if (!(survivorModel.ActorDefinitionID.ToLower() == "hero_rick"))
				{
					continue;
				}
				ActorDefinition actorDefinition = manager.GameEconomyData.GetActorDefinition(survivorModel.ActorDefinitionID);
				if (survivorModel.UpgradeTraits != null && survivorModel.UpgradeTraits.Count > 1)
				{
					string text = null;
					for (int j = 0; j < ((survivorModel.TraitContainer.Traits != null) ? survivorModel.TraitContainer.Traits.Count : 0); j++)
					{
						TraitEntry traitEntry = survivorModel.TraitContainer.Traits[j];
						if (UpgradeTraitsData.StripTraitLevelIdentifier(traitEntry.TraitIdentifier) == "LeaderBuffDontTouchMyAllies")
						{
							text = traitEntry.TraitIdentifier;
							break;
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						survivorModel.TraitContainer.RemoveTrait(text);
					}
					UpgradeTraitsData upgradeTraitsData = survivorModel.UpgradeTraits[1];
					if (UpgradeTraitsData.StripTraitLevelIdentifier(upgradeTraitsData.Identifier) == "LeaderBuffDontTouchMyAllies" && actorDefinition != null && actorDefinition.UpgradeTraits != null && actorDefinition.UpgradeTraits.Count > 0)
					{
						string text2 = UpgradeTraitsData.CompileUpgradeTraitIdentifier(actorDefinition.UpgradeTraits[0], upgradeTraitsData.RarityLevel, isLocked: false);
						TraitDefinition traitDefinition = manager.GameEconomyData.GetTraitDefinition(text2);
						if (traitDefinition != null)
						{
							survivorModel.UpgradeTraits[1].Identifier = traitDefinition.Identifier;
							break;
						}
						manager.Debug.LogError("Cannot migrate hero_rick leader trait, no trait found in game economy data for player=" + player.HashedId + " with trait id=" + text2);
					}
				}
				else
				{
					manager.Debug.LogError("Cannot migrate hero_rick leader trait, incorrect amount of traits for player=" + player.HashedId);
				}
			}
			return true;
		}
	}
}
