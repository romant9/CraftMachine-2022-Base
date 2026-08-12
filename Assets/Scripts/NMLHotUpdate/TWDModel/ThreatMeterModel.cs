using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class ThreatMeterModel : TWDModelObject
	{
		public const string threatMeterValueChanged = "threatMeterValueChanged";

		public const string threatMeterTurnCountChanged = "threatMeterTurnCountChanged";

		public const string waveTriggered = "waveTriggered";

		private int spawnLevelIncrease;

		public int ThreatLevel { get; set; }

		public int TurnCounter { get; set; }

		public int SpawnLevelOffset { get; set; }

		public int InitialTurnCountToWave { get; set; }

		public int InitialThreatLevel { get; set; }

		public int MaxThreatLevel => 10;

		public event ThreatValueChangedHandler ThreatValueChanged;

		public override void Initialize()
		{
			base.Initialize();
			ConfigData configData = base.manager.Player.gameEconomyData.ConfigData;
			SpawnLevelOffset = (base.manager.CombatModel.HasPvPRules ? configData.ThreatWaveInitialLevelOffsetPvP : configData.ThreatWaveInitialLevelOffset);
		}

		public override void Start()
		{
			base.Start();
			ConfigData configData = base.manager.Player.gameEconomyData.ConfigData;
			spawnLevelIncrease = (base.manager.CombatModel.HasPvPRules ? configData.ThreatWaveLevelIncreasePvP : configData.ThreatWaveLevelIncrease);
		}

		public override bool IsValid()
		{
			return true;
		}

		public void SetupForCombat(CombatModel combat)
		{
			InitialThreatLevel = combat.InitialThreatLevel;
			if (combat.MapCategory != MapCategory.Outpost && combat.MapCategory != MapCategory.Story && base.gameEconomyData.ConfigData.WeeklyEventStartThreatIncreasePercentage != 0)
			{
				FixedPoint fixedPoint = (FixedPoint)InitialThreatLevel * (FixedPoint)base.gameEconomyData.ConfigData.WeeklyEventStartThreatIncreasePercentage / 100L;
				InitialThreatLevel += (int)fixedPoint;
			}
			if (!(base.manager.Player.GetAttackTargetMissionModel() is MapMissionModel { IsInApocalyptiWeeklyChallenge: not false }))
			{
				InitialThreatLevel = Math.Min(InitialThreatLevel, MaxThreatLevel);
			}
			InitialTurnCountToWave = combat.InitialTurnCountToWave;
			ThreatLevel = InitialThreatLevel;
			TurnCounter = InitialTurnCountToWave;
		}

		public void ChangeThreatLevel(int value, ThreatInstigator instigator)
		{
			if (value != 0)
			{
				int threatLevel = ThreatLevel;
				ThreatLevel += value;
				ThreatLevel = Math.Max(ThreatLevel, 0);
				CombatModel combatModel = base.manager.CombatModel;
				if (!(base.manager.Player.GetAttackTargetMissionModel() is MapMissionModel { IsInApocalyptiWeeklyChallenge: not false }))
				{
					ThreatLevel = Math.Min(ThreatLevel, MaxThreatLevel);
				}
				if (ThreatLevel != threatLevel && combatModel != null && !combatModel.IsEndlessBattleMission)
				{
					NotifyChange("threatMeterValueChanged", threatLevel);
					NotifyThreatValueChanged(threatLevel, instigator);
				}
			}
		}

		public void SetTurnCount(int value)
		{
			int turnCounter = TurnCounter;
			TurnCounter = value;
			if (TurnCounter != turnCounter)
			{
				NotifyChange("threatMeterTurnCountChanged", turnCounter);
			}
		}

		public void UpdateTurnCount()
		{
			int turnCounter = TurnCounter;
			TurnCounter--;
			if (TurnCounter == 0)
			{
				TriggerWave();
				ThreatLevel = InitialThreatLevel;
				NotifyChange("waveTriggered", TurnCounter);
			}
			else if (TurnCounter < 0)
			{
				SpawnLevelOffset += spawnLevelIncrease;
				TurnCounter = InitialTurnCountToWave;
				ResetSpawnPoints();
			}
			NotifyChange("threatMeterTurnCountChanged", turnCounter);
		}

		public void TriggerWaveImmediately()
		{
			TriggerWave();
			ThreatLevel = InitialThreatLevel;
		}

		private void TriggerWave()
		{
			ActivateNearestSpawnPoints(ThreatLevel);
		}

		public void ResetThreatSpawnPoints()
		{
			if (base.manager != null && base.manager.CombatModel != null)
			{
				ResetSpawnPoints();
			}
		}

		private void ResetSpawnPoints()
		{
			foreach (ActorSpawnPointModel model in base.manager.CombatModel.GetModels<ActorSpawnPointModel>())
			{
				if (model != null && model.ActivationType == ActivationType.Threat)
				{
					model.SpawnCountPerAction = 1;
					model.Reset();
				}
			}
		}

		private void EnableContinuousSpawning()
		{
			foreach (ActorSpawnPointModel model in base.manager.CombatModel.GetModels<ActorSpawnPointModel>())
			{
				if (model != null && model.IsThreatActivated)
				{
					model.EnableContinuousSpawning();
				}
			}
		}

		private void ActivateNearestSpawnPoints(int count)
		{
			switch (count)
			{
			case -1:
				ActivateAllSpawnPoints();
				return;
			case 0:
				return;
			}
			List<ActorModel> factionActors = base.manager.CombatModel.GetFactionActors(Faction.Survivor);
			List<ActorSpawnPointModel> list = new List<ActorSpawnPointModel>();
			if (base.manager.GameEconomyData.GetFeature("BlockedSpawnPointsFix").Enabled)
			{
				for (int i = 0; i < count; i++)
				{
					FixedPoint fixedPoint = FixedPoint.MaxValue;
					ActorSpawnPointModel actorSpawnPointModel = null;
					foreach (ActorSpawnPointModel model in base.manager.CombatModel.GetModels<ActorSpawnPointModel>())
					{
						if (model == null || !model.CanActivate || model.GetAvailableSpawnCoordinatesAmount() <= 0 || model.ActivationType != ActivationType.Threat)
						{
							continue;
						}
						foreach (ActorModel item in factionActors)
						{
							FixedPoint fixedPoint2 = model.Location.Coordinate.DistanceTo(item.GridCoordinate);
							if (fixedPoint2 < fixedPoint)
							{
								fixedPoint = fixedPoint2;
								actorSpawnPointModel = model;
							}
						}
					}
					if (actorSpawnPointModel != null && !list.Contains(actorSpawnPointModel))
					{
						list.Add(actorSpawnPointModel);
						actorSpawnPointModel.Activate();
						actorSpawnPointModel.Alertness = AIAlertness.Homing;
					}
				}
				if (list == null || list.Count <= 0)
				{
					return;
				}
				int num = count - list.Count;
				while (num > 0)
				{
					bool flag = false;
					for (int j = 0; j < list.Count; j++)
					{
						if (num <= 0)
						{
							break;
						}
						if (list[j].SpawnCountPerAction < list[j].GetAvailableSpawnCoordinatesAmount())
						{
							list[j].SpawnCountPerAction++;
							num--;
							flag = true;
						}
					}
					if (!flag)
					{
						break;
					}
				}
				return;
			}
			for (int k = 0; k < count; k++)
			{
				FixedPoint fixedPoint3 = FixedPoint.MaxValue;
				ActorSpawnPointModel actorSpawnPointModel3 = null;
				foreach (ActorSpawnPointModel model2 in base.manager.CombatModel.GetModels<ActorSpawnPointModel>())
				{
					if (model2 == null || !model2.CanActivate || model2.ActivationType != ActivationType.Threat)
					{
						continue;
					}
					foreach (ActorModel item2 in factionActors)
					{
						FixedPoint fixedPoint4 = model2.Location.Coordinate.DistanceTo(item2.GridCoordinate);
						if (fixedPoint4 < fixedPoint3)
						{
							fixedPoint3 = fixedPoint4;
							actorSpawnPointModel3 = model2;
						}
					}
				}
				if (actorSpawnPointModel3 != null && !list.Contains(actorSpawnPointModel3))
				{
					list.Add(actorSpawnPointModel3);
					actorSpawnPointModel3.Activate();
					actorSpawnPointModel3.Alertness = AIAlertness.Homing;
				}
			}
			if (list == null || list.Count <= 0)
			{
				return;
			}
			int num2 = count - list.Count;
			int num3 = 0;
			for (int l = 0; l < num2; l++)
			{
				if (num3 < list.Count)
				{
					list[num3].SpawnCountPerAction++;
					num3++;
				}
				else
				{
					num3 = 0;
					list[num3].SpawnCountPerAction++;
				}
			}
		}

		private void ActivateAllSpawnPoints()
		{
			foreach (ActorSpawnPointModel model in base.manager.CombatModel.GetModels<ActorSpawnPointModel>())
			{
				if (model != null && model.CanActivate)
				{
					model.Activate();
				}
			}
		}

		private void NotifyThreatValueChanged(int oldValue, ThreatInstigator instigator)
		{
			this.ThreatValueChanged?.Invoke(oldValue, instigator);
		}
	}
}
