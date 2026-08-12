using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class SupportsRemoveSurvivorLoadQueueMessage : SupportLoadQueueMessage
	{
		public List<SupportRemoveSupportItemEntry> SupportRemoveSurvivorEntries { get; set; }

		public SupportsRemoveSurvivorLoadQueueMessage()
		{
		}

		public SupportsRemoveSurvivorLoadQueueMessage(List<SupportRemoveSupportItemEntry> supportRemoveSurvivorEntries)
		{
			SupportRemoveSurvivorEntries = supportRemoveSurvivorEntries;
		}

		public override bool Execute(TWDModelManager manager)
		{
			manager.Metrics.AddResetCombat(manager.Player.Combat != null).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			if (manager.Player.Combat != null)
			{
				manager.Player.DeleteCombatModel(notify: false);
			}
			foreach (SupportRemoveSupportItemEntry survivor in SupportRemoveSurvivorEntries)
			{
				if (manager.Player != null && !string.IsNullOrEmpty(survivor.Identifier) && survivor.RemoveItem)
				{
					SurvivorModel survivorModel = manager.Player.SurvivorContainer.Survivors.First((SurvivorModel x) => x.GenerateName == survivor.Identifier);
					List<TeamTeamPreset> presets = manager.Player.TeamPresetsManager.Presets;
					TryRemoveFromTeamPresets(presets, survivorModel);
					TryReplaceSurvivorOutPostTeam(manager, survivorModel);
					TryUpdateGvgDefenders(manager.Player, manager.GameEconomyData);
					TryRemoveBadges(survivorModel);
					TryRemoveEquipment(survivorModel);
					manager.Player.SurvivorContainer.RemoveSurvivor(survivorModel);
					manager.Metrics.AddRemove().AddResources(new Dictionary<CurrencyType, OverflowableAmount> { 
					{
						manager.Player.GetCurrency(survivorModel.Definition.TraitUpgradeCurrency).Type,
						new OverflowableAmount
						{
							Amount = -manager.Player.GetCurrency(survivorModel.Definition.TraitUpgradeCurrency).Value
						}
					} }).AddSurvivor(survivorModel)
						.AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID)
						.Send();
					manager.Player.GetCurrency(survivorModel.Definition.TraitUpgradeCurrency).SetValue(0);
				}
			}
			return true;
		}

		private void TryRemoveFromTeamPresets(List<TeamTeamPreset> teamPresets, SurvivorModel survivorModel)
		{
			for (int num = teamPresets.Count - 1; num >= 0; num--)
			{
				if (teamPresets[num].Survivors.Contains(survivorModel))
				{
					teamPresets.RemoveAt(num);
				}
			}
		}

		private void TryReplaceSurvivorOutPostTeam(TWDModelManager manager, SurvivorModel survivorModel)
		{
			if (manager.Player.SurvivorContainer.IsOutpostDefending(survivorModel))
			{
				SurvivorModel newSurvivor = manager.Player.SurvivorContainer.Survivors.First((SurvivorModel x) => !manager.Player.SurvivorContainer.IsOutpostDefending(x) && x != survivorModel);
				manager.Player.SurvivorContainer.AddSurvivorToOutpostDefense(newSurvivor, survivorModel);
			}
		}

		private void TryUpdateGvgDefenders(PlayerModel playerModel, GameEconomyData gameEconomyData)
		{
			if (playerModel.GvGDefenders == null)
			{
				playerModel.Debug.LogError("Trying to update defenders when defenders are not initialized");
				return;
			}
			for (int i = 0; i < playerModel.GvGDefenders.Count; i++)
			{
				string defenderAnalyticId = playerModel.GvGDefenders[i].AnalyticsId;
				if (playerModel.SurvivorContainer.Survivors.Any((SurvivorModel x) => x.IdForAnalytics == defenderAnalyticId))
				{
					continue;
				}
				List<string> defenderIds = playerModel.GvGDefenders.Select((SurvivorMockData x) => x.AnalyticsId).ToList();
				SurvivorModel survivorModel = playerModel.SurvivorContainer.Survivors.FirstOrDefault((SurvivorModel x) => !defenderIds.Contains(x.IdForAnalytics));
				if (survivorModel == null)
				{
					int num = playerModel.SurvivorContainer.Survivors.Max((SurvivorModel x) => x.Level);
					survivorModel = playerModel.SurvivorContainer.CreateRandomSurvivor(0, num, num);
					if (!playerModel.SurvivorContainer.CanAddSurvivor())
					{
						playerModel.SurvivorContainer.SurvivorGiftSlotsCount++;
					}
					playerModel.SurvivorContainer.AddSurvivor(survivorModel);
				}
				if (!ReplaceSurvivorFromDefenders(playerModel, gameEconomyData, survivorModel, defenderAnalyticId))
				{
					playerModel.Debug.LogError("Error adding survivor to defenders");
				}
			}
		}

		private bool ReplaceSurvivorFromDefenders(PlayerModel playerModel, GameEconomyData gameEconomyData, SurvivorModel survivorModel, string analyticsIdToReplace)
		{
			if (playerModel.GvGDefenders.Any((SurvivorMockData x) => x.AnalyticsId == survivorModel.IdForAnalytics))
			{
				playerModel.Debug.LogError("Trying to add a duplicated survivor to defenders");
				return false;
			}
			if (playerModel.GvGDefenders.Count > 9)
			{
				playerModel.Debug.LogError("Trying to add a 10th survivor to defenders");
				return false;
			}
			SurvivorMockData survivorMockData = survivorModel.CreateMockData();
			survivorMockData.AdjustedLevel = (int)GvGModelHelper.GetAdjustedLevelForSurvivor(survivorModel, gameEconomyData);
			survivorMockData.TotalDamage = survivorModel.GetHitpoints();
			survivorMockData.OwnerHashedPlayerId = playerModel.HashedId;
			survivorMockData.MockWeapon = survivorModel.GetWeaponEquipment().CreateMockData();
			survivorMockData.MockArmor = survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor).CreateMockData();
			for (int num = 0; num < playerModel.GvGDefenders.Count; num++)
			{
				if (playerModel.GvGDefenders[num].AnalyticsId == analyticsIdToReplace)
				{
					playerModel.GvGDefenders[num] = survivorMockData;
					return true;
				}
			}
			return false;
		}

		private void TryRemoveBadges(SurvivorModel survivorModel)
		{
			for (int num = survivorModel.BadgeContainer.Badges.Count - 1; num >= 0; num--)
			{
				BadgeModel badgeModel = survivorModel.BadgeContainer.Badges[num];
				if (badgeModel != null)
				{
					survivorModel.ReclaimBadge(badgeModel, pay: false, returnBadgeInventory: true);
				}
			}
		}

		private void TryRemoveEquipment(SurvivorModel survivorModel)
		{
			survivorModel.UnequipAll();
		}
	}
}
