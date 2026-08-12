using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class WalkerSpawnPointModel : ActorSpawnPointModel
	{
		public bool UseOverrideWalkerType { get; set; }

		public WalkerType OverrideWalkerType { get; set; }

		public List<WalkerType> OverrideWalkerTypes { get; set; }

		public DormantType DormantType { get; set; }

		public bool IsBoss { get; set; }

		public bool AllowSpawningToAdjacent { get; set; }

		public int OverrideWalkerLevel { get; set; }

		public List<int> WalkerVisualizationChances { get; set; }

		public WalkerSpawnPointModel()
		{
		}

		public WalkerSpawnPointModel(string viewId)
			: base(viewId)
		{
		}

		public override int GetAvailableSpawnCoordinatesAmount()
		{
			if (AllowSpawningToAdjacent)
			{
				return 50;
			}
			return base.GetAvailableSpawnCoordinatesAmount();
		}

		protected override int InternalSpawn(ActorModel instigator)
		{
			CombatModel combatModel = base.manager.CombatModel;
			MissionGenerationData missionGenerationData = base.gameEconomyData.GetMissionGenerationData(base.manager.Player.SelectedMissionDifficulty);
			string selectedMissionFlavor = base.manager.Player.SelectedMissionFlavor;
			MissionFlavorData missionFlavorData = base.gameEconomyData.GetMissionFlavorData(selectedMissionFlavor);
			int num = combatModel.GetFactionActors(Faction.Walker).Count + combatModel.GetFactionActors(Faction.Dormant).Count;
			int val = 50 - num;
			int num2 = Math.Min(base.SpawnCountPerAction, val);
			if (base.TotalSpawnCount >= 0)
			{
				num2 = Math.Min(base.TotalSpawnCount - base.CurrentSpawnCount, num2);
			}
			if (num2 <= 0)
			{
				return 0;
			}
			List<WalkerType> list = new List<WalkerType>();
			List<FixedPoint> list2 = new List<FixedPoint>();
			foreach (WalkerType value in Enum.GetValues(typeof(WalkerType)))
			{
				list.Add(value);
				FixedPoint[] array = new FixedPoint[5] { missionFlavorData.WalkerTypeNormal, missionFlavorData.WalkerTypeArmored, missionFlavorData.WalkerTypeTank, missionFlavorData.WalkerTypeExplosive, missionFlavorData.WalkerTypeSlim };
				FixedPoint item = (((int)value < array.Length) ? array[(int)value] : ((FixedPoint)0L));
				if (UseOverrideWalkerType)
				{
					item = ((value == OverrideWalkerType) ? ((FixedPoint)1.0) : ((FixedPoint)0.0));
				}
				list2.Add(item);
			}
			int num3 = ((base.ActivationType == ActivationType.Threat) ? combatModel.ThreatMeter.SpawnLevelOffset : 0);
			int randomInRange = base.manager.Player.PlayerRandom.GetRandomInRange(missionGenerationData.MinWalkerLevel, missionGenerationData.MaxWalkerLevel);
			int num4 = (IsBoss ? missionGenerationData.BossCount : 0);
			int bossLevelOffset = missionGenerationData.BossLevelOffset;
			List<GridCoordinate> spawnCoordinates = GetSpawnCoordinates();
			if ((base.Location.Coordinates.Count == 1 || AllowSpawningToAdjacent) && spawnCoordinates.Count < num2)
			{
				spawnCoordinates.AddRange(SolveAdjacentSpawnCoordinates());
			}
			int num5 = 0;
			if (spawnCoordinates.Count > 0)
			{
				num2 = Math.Min(num2, spawnCoordinates.Count);
				for (int i = 0; i < num2; i++)
				{
					int num6 = randomInRange;
					int index = 0;
					if (IsBoss)
					{
						if (num4 > 0)
						{
							if (UseOverrideWalkerType)
							{
								index = (int)OverrideWalkerType;
							}
							else
							{
								for (int j = 0; j < list2.Count; j++)
								{
									if (j == 0)
									{
										list2[j] = 0.0;
									}
									else
									{
										list2[j] = ((list2[j] > 0.0) ? list2[j] : ((FixedPoint)(1f / (float)list2.Count)));
									}
								}
								index = base.manager.Player.PlayerRandom.WeightedRandom(list2.ToArray());
							}
							num6 += bossLevelOffset;
						}
						else
						{
							index = 0;
						}
					}
					else if (OverrideWalkerTypes != null)
					{
						if (OverrideWalkerTypes.Count > 0)
						{
							index = (int)OverrideWalkerTypes[i];
						}
					}
					else
					{
						index = base.manager.Player.PlayerRandom.WeightedRandom(list2.ToArray());
					}
					num6 = num6 + num3 + base.LevelOffset;
					if (OverrideWalkerLevel > 0)
					{
						num6 = OverrideWalkerLevel + num3;
					}
					num6 = Math.Max(1, num6);
					WalkerType walkerType2 = list[index];
					if (walkerType2 == WalkerType.WalkerNormal && combatModel.SpawnModifiers != null)
					{
						if (combatModel.SpawnModifiers.PromoteWalkerCount > 0)
						{
							walkerType2 = ((combatModel.SpawnModifiers.PromoteWalkerType[0] != WalkerType.WalkerNormal) ? combatModel.SpawnModifiers.PromoteWalkerType[0] : ((base.manager.Player.PlayerRandom.GetRandomInRange(0, 1) != 0) ? WalkerType.WalkerTank : WalkerType.WalkerArmored));
							base.Alertness = AIAlertness.Aggressive;
							combatModel.SpawnModifiers.PromoteWalkerCount--;
							combatModel.SpawnModifiers.PromoteWalkerType.RemoveAt(0);
							if (combatModel.SpawnModifiers.UpgradePromotedWalkerCount > 0)
							{
								combatModel.SpawnModifiers.UpgradePromotedWalkerCount--;
								num6++;
							}
						}
						else if (combatModel.SpawnModifiers.PromoteThreatWalkerCount > 0 && base.IsThreatActivated)
						{
							walkerType2 = ((combatModel.SpawnModifiers.PromoteThreatWalkerType[0] != WalkerType.WalkerNormal) ? combatModel.SpawnModifiers.PromoteThreatWalkerType[0] : ((base.manager.Player.PlayerRandom.GetRandomInRange(0, 1) != 0) ? WalkerType.WalkerTank : WalkerType.WalkerArmored));
							base.Alertness = AIAlertness.Aggressive;
							combatModel.SpawnModifiers.PromoteThreatWalkerCount--;
							combatModel.SpawnModifiers.PromoteThreatWalkerType.RemoveAt(0);
						}
						else if (combatModel.SpawnModifiers.UpgradeWalkerCount > 0)
						{
							combatModel.SpawnModifiers.UpgradeWalkerCount--;
							num6++;
						}
					}
					if (combatModel.WalkerRandomizer.IsEnabled() && walkerType2 == WalkerType.WalkerNormal)
					{
						walkerType2 = combatModel.WalkerRandomizer.RandomizeWalker(spawnCoordinates[i], walkerType2);
					}
					Faction faction = base.Faction;
					if (WalkerType.ExplosiveBarrel == walkerType2)
					{
						faction = Faction.Environmental;
					}
					WalkerVisualization walkerVisualVariation = RollForWalkerVisualization();
					if (!(combatModel.CreateActor(spawnCoordinates[i], faction, num6, base.SpawnTag, Enum.GetName(typeof(WalkerType), walkerType2), null, ActorGender.NotSpecified, walkerVisualVariation) is WalkerModel walkerModel) || !walkerModel.IsValid())
					{
						continue;
					}
					walkerModel.MissionFailCondition = base.MissionFailCondition;
					walkerModel.ActivationType = base.ActivationType;
					walkerModel.SetupForCombat(combatModel);
					walkerModel.AIDataModel.Alertness = base.Alertness;
					if (base.ScriptedBehaviors != null && base.ScriptedBehaviors.Count > 0)
					{
						walkerModel.AIDataModel.ScriptedBehaviorClasses = base.ScriptedBehaviors;
					}
					if (base.AdditionalTraits != null && base.AdditionalTraits.Count > 0)
					{
						for (int k = 0; k < base.AdditionalTraits.Count; k++)
						{
							string traitIdentifier = base.AdditionalTraits[k];
							walkerModel.AddTrait(traitIdentifier);
						}
					}
					walkerModel.DormantType = DormantType;
					walkerModel.IsVisibleToSurvivors = false;
					walkerModel.IsBoss = num4 > 0;
					if (base.manager.ExecuteAction(new SpawnAction(walkerModel, this, spawnCoordinates[i], instigator)))
					{
						num5++;
						num4--;
					}
				}
			}
			if (base.ActivationType == ActivationType.Threat)
			{
				combatModel.SpawnedWalkerCount += num5;
			}
			return num5;
		}

		private WalkerVisualization RollForWalkerVisualization()
		{
			if (WalkerVisualizationChances == null || WalkerVisualizationChances.Count == 0)
			{
				return WalkerVisualization.Normal;
			}
			FixedPoint[] array = new FixedPoint[WalkerVisualizationChances.Count];
			int num = 0;
			for (int i = 0; i < WalkerVisualizationChances.Count; i++)
			{
				array[i] = WalkerVisualizationChances[i];
				num += WalkerVisualizationChances[i];
			}
			if (num == 0)
			{
				return WalkerVisualization.Normal;
			}
			return base.manager.Player.PlayerRandom.WeightedRandomEnum<WalkerVisualization>(array);
		}
	}
}
