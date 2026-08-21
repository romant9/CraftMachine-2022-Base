using System.Collections.Generic;

namespace TWDModel
{
	public abstract class WorldBossParticipantAttackCommand : TWDWorldBossInternalCommand
	{
		protected sealed class PreparedParticipantCharge
		{
			public string SurvivorId { get; set; }

			public int ChargesAfterConsume { get; set; }

			public long BaseUtcMsAfterConsume { get; set; }
		}

		public List<string> ParticipantSurvivorIds { get; set; }

		protected WorldBossParticipantAttackCommand()
		{
		}

		protected WorldBossParticipantAttackCommand(int seasonId, int cycleId)
			: base(seasonId, cycleId)
		{
		}

		protected TWDModelResult ValidateParticipantSurvivorsAndFatigue(TWDModelManager manager)
		{
			if (ParticipantSurvivorIds == null || ParticipantSurvivorIds.Count == 0)
			{
				manager.Debug.LogError(GetType().Name + ": participant survivors are empty");
				return TWDModelResult.Error;
			}
			SurvivorContainerModel survivorContainer = manager.Player.SurvivorContainer;
			if (survivorContainer == null || survivorContainer.Survivors == null)
			{
				manager.Debug.LogError(GetType().Name + ": player survivor container/collection is null");
				return TWDModelResult.Error;
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string participantSurvivorId in ParticipantSurvivorIds)
			{
				if (string.IsNullOrEmpty(participantSurvivorId))
				{
					manager.Debug.LogError(GetType().Name + ": participant survivor id is empty");
					return TWDModelResult.Error;
				}
				if (!hashSet.Add(participantSurvivorId))
				{
					manager.Debug.LogError(GetType().Name + ": duplicate survivor: " + participantSurvivorId);
					return TWDModelResult.Error;
				}
				bool flag = false;
				for (int i = 0; i < survivorContainer.Survivors.Count; i++)
				{
					SurvivorModel survivorModel = survivorContainer.Survivors[i];
					if (survivorModel != null && survivorModel.IdForAnalytics == participantSurvivorId)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					manager.Debug.LogError(GetType().Name + ": survivor does not belong to player: " + participantSurvivorId);
					return TWDModelResult.Error;
				}
			}
			return ValidateParticipantHeroCharges(manager);
		}

		protected TWDModelResult ValidateParticipantHeroCharges(TWDModelManager manager)
		{
			WorldBossModelManager worldBossModelManager = manager.Player?.WorldBossModelManager;
			if (worldBossModelManager == null || worldBossModelManager.GetHeroChargeLimit() <= 0)
			{
				return TWDModelResult.OK;
			}
			if (ParticipantSurvivorIds == null)
			{
				return TWDModelResult.OK;
			}
			foreach (string participantSurvivorId in ParticipantSurvivorIds)
			{
				if (!string.IsNullOrEmpty(participantSurvivorId) && !worldBossModelManager.CanHeroBattle(base.SeasonId, base.CycleId, participantSurvivorId))
				{
					manager.Debug.LogError(GetType().Name + ": hero out of action charges: " + participantSurvivorId);
					return TWDModelResult.Error;
				}
			}
			return TWDModelResult.OK;
		}

		protected bool TryPrepareParticipantCharges(TWDModelManager manager, out List<PreparedParticipantCharge> preparedCharges)
		{
			preparedCharges = new List<PreparedParticipantCharge>();
			int num = (manager.GameEconomyData?.WorldBossConfig)?.DailyHeroBattleLimit ?? 0;
			if (num <= 0)
			{
				return true;
			}
			if (ParticipantSurvivorIds == null || ParticipantSurvivorIds.Count == 0)
			{
				manager.Debug.LogError(GetType().Name + ": participant survivors are empty while preparing charges");
				return false;
			}
			WorldBossModelManager worldBossModelManager = manager.Player?.WorldBossModelManager;
			if (worldBossModelManager == null)
			{
				manager.Debug.LogError(GetType().Name + ": WorldBossModelManager is null while preparing charges");
				return false;
			}
			worldBossModelManager.ClearHeroFatigueIfOutdated(base.SeasonId, base.CycleId);
			long heroRecoverMs = worldBossModelManager.GetHeroRecoverMs();
			long utcTimeStamp = manager.Player.UtcTimeStamp;
			WorldBossHeroFatigueState worldBossHeroFatigue = manager.Player.WorldBossHeroFatigue;
			Dictionary<string, WorldBossHeroFatigueEntry> dictionary = ((worldBossHeroFatigue != null && worldBossHeroFatigue.IsForCycle(base.SeasonId, base.CycleId)) ? worldBossHeroFatigue.Entries : null);
			foreach (string participantSurvivorId in ParticipantSurvivorIds)
			{
				WorldBossHeroFatigueEntry value = null;
				dictionary?.TryGetValue(participantSurvivorId, out value);
				WorldBossHeroFatigueEntry worldBossHeroFatigueEntry = ((value != null) ? new WorldBossHeroFatigueEntry(value.Charges, value.BaseUtcMs) : new WorldBossHeroFatigueEntry(num, utcTimeStamp));
				if (!worldBossHeroFatigueEntry.TryConsume(num, heroRecoverMs, utcTimeStamp))
				{
					manager.Debug.LogError(GetType().Name + ": hero has no action charges while preparing: " + participantSurvivorId);
					return false;
				}
				preparedCharges.Add(new PreparedParticipantCharge
				{
					SurvivorId = participantSurvivorId,
					ChargesAfterConsume = worldBossHeroFatigueEntry.Charges,
					BaseUtcMsAfterConsume = worldBossHeroFatigueEntry.BaseUtcMs
				});
			}
			return preparedCharges.Count == ParticipantSurvivorIds.Count;
		}

		protected List<WorldBossModelManager.WorldBossFatigueChargeSnapshot> CaptureFatigueChargeSnapshots(TWDModelManager manager)
		{
			List<WorldBossModelManager.WorldBossFatigueChargeSnapshot> list = new List<WorldBossModelManager.WorldBossFatigueChargeSnapshot>();
			if (ParticipantSurvivorIds == null || ParticipantSurvivorIds.Count == 0)
			{
				return list;
			}
			WorldBossHeroFatigueState worldBossHeroFatigue = manager.Player.WorldBossHeroFatigue;
			Dictionary<string, WorldBossHeroFatigueEntry> dictionary = ((worldBossHeroFatigue != null && worldBossHeroFatigue.IsForCycle(base.SeasonId, base.CycleId)) ? worldBossHeroFatigue.Entries : null);
			foreach (string participantSurvivorId in ParticipantSurvivorIds)
			{
				if (!string.IsNullOrEmpty(participantSurvivorId))
				{
					WorldBossHeroFatigueEntry value = null;
					bool flag = dictionary != null && dictionary.TryGetValue(participantSurvivorId, out value) && value != null;
					list.Add(new WorldBossModelManager.WorldBossFatigueChargeSnapshot
					{
						SurvivorId = participantSurvivorId,
						ExistedBefore = flag,
						ChargesBefore = (flag ? value.Charges : 0),
						BaseUtcMsBefore = (flag ? value.BaseUtcMs : 0)
					});
				}
			}
			return list;
		}

		protected void ApplyPreparedParticipantCharges(TWDModelManager manager, List<PreparedParticipantCharge> preparedCharges)
		{
			if (preparedCharges == null || preparedCharges.Count == 0)
			{
				return;
			}
			WorldBossHeroFatigueState worldBossHeroFatigueState = manager.Player.WorldBossHeroFatigue;
			if (worldBossHeroFatigueState == null || !worldBossHeroFatigueState.IsForCycle(base.SeasonId, base.CycleId))
			{
				worldBossHeroFatigueState = new WorldBossHeroFatigueState(base.SeasonId, base.CycleId);
				manager.Player.WorldBossHeroFatigue = worldBossHeroFatigueState;
			}
			Dictionary<string, WorldBossHeroFatigueEntry> dictionary = worldBossHeroFatigueState.Entries;
			if (dictionary == null)
			{
				dictionary = (worldBossHeroFatigueState.Entries = new Dictionary<string, WorldBossHeroFatigueEntry>());
			}
			foreach (PreparedParticipantCharge preparedCharge in preparedCharges)
			{
				if (!dictionary.TryGetValue(preparedCharge.SurvivorId, out var value) || value == null)
				{
					value = new WorldBossHeroFatigueEntry();
					dictionary[preparedCharge.SurvivorId] = value;
				}
				value.Charges = preparedCharge.ChargesAfterConsume;
				value.BaseUtcMs = preparedCharge.BaseUtcMsAfterConsume;
			}
		}
	}
}
