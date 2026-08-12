namespace TWDModel
{
	public static class MigrationUtils
	{
		public static void AddNewCurrency(PlayerModel player, TWDModelManager manager, params CurrencyType[] currenciesToMigrate)
		{
			for (int i = 0; i < currenciesToMigrate.Length; i++)
			{
				CurrencyModel currencyModel = new CurrencyModel(currenciesToMigrate[i]);
				currencyModel.SetManager(manager);
				currencyModel.AddMultiplier = 1L;
				player.Currencies.Add(currencyModel);
			}
			player.UpdateCurrenciesCapacity();
		}

		public static void DeleteCombatModel(PlayerModel player)
		{
			if (player.Combat != null && player.Tutorial.StaticTutorialComplete)
			{
				player.DeleteCombatModel(notify: false);
			}
		}

		public static void MigrateLeaderTrait(TWDModelManager manager, string heroActorDefinition, string oldLeaderTrait)
		{
			PlayerModel player = manager.Player;
			for (int i = 0; i < player.SurvivorContainer.Survivors.Count; i++)
			{
				SurvivorModel survivorModel = player.SurvivorContainer.Survivors[i];
				if (!(survivorModel.ActorDefinitionID.ToLower() == heroActorDefinition))
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
						if (UpgradeTraitsData.StripTraitLevelIdentifier(traitEntry.TraitIdentifier) == oldLeaderTrait)
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
					if (UpgradeTraitsData.StripTraitLevelIdentifier(upgradeTraitsData.Identifier) == oldLeaderTrait && actorDefinition != null && actorDefinition.UpgradeTraits != null && actorDefinition.UpgradeTraits.Count > 0)
					{
						string text2 = UpgradeTraitsData.CompileUpgradeTraitIdentifier(actorDefinition.UpgradeTraits[0], upgradeTraitsData.RarityLevel, isLocked: false);
						TraitDefinition traitDefinition = manager.GameEconomyData.GetTraitDefinition(text2);
						if (traitDefinition != null)
						{
							survivorModel.UpgradeTraits[1].Identifier = traitDefinition.Identifier;
							break;
						}
						manager.Debug.LogError($"Cannot migrate {actorDefinition} leader trait, no trait found in game economy data for player= {player.HashedId} with trait id= {text2}");
					}
				}
				else
				{
					manager.Debug.LogError($"Cannot migrate {actorDefinition} leader trait, incorrect amount of traits for player=" + player.HashedId);
				}
			}
		}
	}
}
