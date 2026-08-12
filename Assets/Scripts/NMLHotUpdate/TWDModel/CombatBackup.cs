using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class CombatBackup : TWDModelObject
	{
		public Dictionary<int, int> bounsPhonePortraitTurnKilledNum = new Dictionary<int, int>();

		public Dictionary<string, int> PitfallAreasActorCooldownUntilTurns;

		public Dictionary<int, List<NodeBase>> NodeGraphs;

		public CombatHUDStateInfo CombatHUDState;

		public ModelList<ActorBackup> ActorBackups { get; set; }

		public ModelList<InteractiveObjectBuckup> InteractiveObjectBuckups { get; set; }

		public ThreatMeterBackup threatMeterBackup { get; set; }

		public Dictionary<int, int> SupportNextCanUseTurn { get; set; }

		public Dictionary<int, int> SupportNextInnerCanUseTurn { get; set; }

		public Dictionary<int, int> usedCount { get; set; }

		public MissionStatisticsBackup MissionStatistics { get; set; }

		public string CombatFailureReason { get; set; } = "";

		public List<EquipmentItemModel> Consumables { get; set; }

		public Dictionary<int, int> Variables { get; set; }

		public ModelList<LootModelBackup> LootModels { get; set; }

		public int AvailableKeys { get; set; }

		public List<LootKeySource> LootKeysSources { get; set; }

		public ModelList<ExplosiveModelBackup> ExplosiveModels { get; set; }

		public ModelList<DoorModelBackup> DoorModels { get; set; }

		public ModelList<CombatColliderModelBackup> CombatColliderModels { get; set; }

		public ModelList<CoverModelBackup> CoverModels { get; set; }

		public ModelList<MovableModelBackup> MovableModels { get; set; }

		public ModelList<MissionLogicModelBackup> MissionLogicModels { get; set; }

		public ModelList<CombatExitModelBackup> CombatExitModels { get; set; }

		public ModelList<SetMissionObjectiveModelBackup> SetMissionObjectiveModels { get; set; }

		public ModelList<TriggerModelBackup> TriggerModels { get; set; }

		public ModelList<ActorSpawnPointModelBackup> ActorSpawnPointModels { get; set; }

		public int TurnTimerActivationTurn { get; set; }

		public int CurrentTurnFlameTriggerCount { get; set; }

		public int PvPCollectedLootsCount { get; set; }

		public int PvPCollectedFlagsCount { get; set; }

		public RedactTimedEffectBackup RedactTimedEffect { get; set; }

		public List<int> WalkerLevels { get; set; }

		public EndlessModeCombatModelBackup EndlessModeCombatModel { get; set; }

		public List<TWDModelObject> CombatAreaModels { get; set; }

		public List<TWDModelObject> CombatAreaManagers { get; set; }

		public List<TWDModelObject> ActorToActorRelationModels { get; set; }

		public List<TWDModelObject> ActorToActorRelationManagers { get; set; }

		[IgnoreModelProperty]
		public ActorModel DashSurvivalFlagActor { get; set; }

		[IgnoreModelProperty]
		public ActorModel DashRaiderFlagActor { get; set; }

		public int DebuffQuantunRemove { get; set; }

		public int DebuffQuantunRemoveRaider { get; set; }

		public List<TWDModelObject> FactionToActorRelationModels { get; set; }

		public List<TWDModelObject> FactionToActorManagers { get; set; }

		public List<PersistentMissionVariable> variables { get; set; }

		public List<int> PVPKilledDefenderIndices { get; set; }

		public ResurgenceType1ContainerBackup ResurgenceType1ContainerBackup { get; set; }

		public ResurgenceType2ContainerBackup ResurgenceType2ContainerBackup { get; set; }

		public AttackChainContainerBackup AttackChainContainerBackup { get; set; }

		public MissionObjective MissionObjective { get; set; }

		public ModelList<SurvivalGameModelBackup> SurvivalGameModelBackups { get; set; }

		public List<GuardianVowBinding> GuardianVowBindings { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			ActorBackups = new ModelList<ActorBackup>();
			InteractiveObjectBuckups = new ModelList<InteractiveObjectBuckup>();
			LootModels = new ModelList<LootModelBackup>();
			ExplosiveModels = new ModelList<ExplosiveModelBackup>();
			DoorModels = new ModelList<DoorModelBackup>();
			threatMeterBackup = new ThreatMeterBackup();
			MissionStatistics = new MissionStatisticsBackup();
			Consumables = new List<EquipmentItemModel>();
			CombatColliderModels = new ModelList<CombatColliderModelBackup>();
			CoverModels = new ModelList<CoverModelBackup>();
			MovableModels = new ModelList<MovableModelBackup>();
			ActorSpawnPointModels = new ModelList<ActorSpawnPointModelBackup>();
			CombatExitModels = new ModelList<CombatExitModelBackup>();
			SetMissionObjectiveModels = new ModelList<SetMissionObjectiveModelBackup>();
			TriggerModels = new ModelList<TriggerModelBackup>();
			NodeGraphs = new Dictionary<int, List<NodeBase>>();
			MissionLogicModels = new ModelList<MissionLogicModelBackup>();
			ResurgenceType1ContainerBackup = new ResurgenceType1ContainerBackup();
			ResurgenceType2ContainerBackup = new ResurgenceType2ContainerBackup();
			SurvivalGameModelBackups = new ModelList<SurvivalGameModelBackup>();
			GuardianVowBindings = new List<GuardianVowBinding>();
		}

		public void RecordStatus(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			foreach (ActorModel allActor in combat.AllActors)
			{
				ActorBackup actorBackup = new ActorBackup();
				actorBackup.RecordStatus(allActor);
				ActorBackups.Add(actorBackup);
			}
			WalkerLevels = ((combat.WalkerLevels == null) ? null : new List<int>(combat.WalkerLevels));
			if (combat.EndlessModeCombatModel != null)
			{
				EndlessModeCombatModel = new EndlessModeCombatModelBackup();
				EndlessModeCombatModel.RecordStatus(combat.EndlessModeCombatModel);
			}
			variables = combat.PersistentMissionVariableManager.variables.Select((PersistentMissionVariable x) => new PersistentMissionVariable(x)).ToList();
			bounsPhonePortraitTurnKilledNum = new Dictionary<int, int>(combat.bounsPhonePortraitTurnKilledNum);
			PVPKilledDefenderIndices = new List<int>(combat.PVPKilledDefenderIndices);
			CombatHUDState = combat.CombatHUDState;
			CombatAreaModels = RecordCombatArea(combat);
			CombatAreaManagers = combat.GetModels<CombatAreasManager>();
			PitfallAreasManager model = combat.GetModel<PitfallAreasManager>();
			if (model != null && model.ActorCooldownUntilTurns != null)
			{
				PitfallAreasActorCooldownUntilTurns = new Dictionary<string, int>(model.ActorCooldownUntilTurns);
			}
			ActorToActorRelationModels = RecordCombatActorToActorRelations(combat);
			ActorToActorRelationManagers = combat.GetModels<ActorToActorRelationsManager>();
			FactionToActorRelationModels = RecordCombatFactionToActorRelations(combat);
			FactionToActorManagers = combat.GetModels<FactionToActorManager>();
			DashSurvivalFlagActor = combat.DashSurvivalFlagActor;
			DashRaiderFlagActor = combat.DashRaiderFlagActor;
			DebuffQuantunRemove = combat.DebuffQuantunRemove;
			DebuffQuantunRemoveRaider = combat.DebuffQuantunRemoveRaider;
			foreach (NodeGraph model14 in combat.GetModels<NodeGraph>())
			{
				NodeGraphs.Add(model14.GuidHash, model14.Nodes.Select((NodeBase x) => x.RecordValue()).ToList());
			}
			foreach (InteractiveObjectModel model15 in combat.GetModels<InteractiveObjectModel>())
			{
				InteractiveObjectBuckup interactiveObjectBuckup = new InteractiveObjectBuckup();
				interactiveObjectBuckup.RecordStatus(model15);
				InteractiveObjectBuckups.Add(interactiveObjectBuckup);
			}
			foreach (MissionLogicModel model16 in combat.GetModels<MissionLogicModel>())
			{
				MissionLogicModelBackup missionLogicModelBackup = new MissionLogicModelBackup();
				missionLogicModelBackup.RecordStatus(model16);
				MissionLogicModels.Add(missionLogicModelBackup);
			}
			foreach (LootModel model17 in combat.GetModels<LootModel>())
			{
				LootModelBackup lootModelBackup = new LootModelBackup();
				lootModelBackup.RecordStatus(model17);
				LootModels.Add(lootModelBackup);
			}
			AvailableKeys = manager.Player.LootManager.AvailableKeys;
			LootKeysSources = new List<LootKeySource>(manager.Player.LootManager.LootKeysSources);
			foreach (ExplosiveModel model18 in combat.GetModels<ExplosiveModel>())
			{
				ExplosiveModelBackup explosiveModelBackup = new ExplosiveModelBackup();
				explosiveModelBackup.RecordStatus(model18);
				ExplosiveModels.Add(explosiveModelBackup);
			}
			foreach (DoorModel model19 in combat.GetModels<DoorModel>())
			{
				DoorModelBackup doorModelBackup = new DoorModelBackup();
				doorModelBackup.RecordStatus(model19);
				DoorModels.Add(doorModelBackup);
			}
			foreach (CombatColliderModel model20 in combat.GetModels<CombatColliderModel>())
			{
				CombatColliderModelBackup combatColliderModelBackup = new CombatColliderModelBackup();
				combatColliderModelBackup.RecordStatus(model20);
				CombatColliderModels.Add(combatColliderModelBackup);
			}
			foreach (CoverModel model21 in combat.GetModels<CoverModel>())
			{
				CoverModelBackup coverModelBackup = new CoverModelBackup();
				coverModelBackup.RecordStatus(model21);
				CoverModels.Add(coverModelBackup);
			}
			foreach (MovableModel model22 in combat.GetModels<MovableModel>())
			{
				MovableModelBackup movableModelBackup = new MovableModelBackup();
				movableModelBackup.RecordStatus(model22);
				MovableModels.Add(movableModelBackup);
			}
			foreach (CombatExitModel model23 in combat.GetModels<CombatExitModel>())
			{
				CombatExitModelBackup combatExitModelBackup = new CombatExitModelBackup();
				combatExitModelBackup.RecordStatus(model23);
				CombatExitModels.Add(combatExitModelBackup);
			}
			foreach (SetMissionObjectiveModel model24 in combat.GetModels<SetMissionObjectiveModel>())
			{
				SetMissionObjectiveModelBackup setMissionObjectiveModelBackup = new SetMissionObjectiveModelBackup();
				setMissionObjectiveModelBackup.RecordStatus(model24);
				SetMissionObjectiveModels.Add(setMissionObjectiveModelBackup);
			}
			foreach (TriggerModel model25 in combat.GetModels<TriggerModel>())
			{
				TriggerModelBackup triggerModelBackup = new TriggerModelBackup();
				triggerModelBackup.RecordStatus(model25);
				TriggerModels.Add(triggerModelBackup);
			}
			Variables = new Dictionary<int, int>(combat.Variables);
			PvPCollectedFlagsCount = combat.PvPCollectedFlagsCount;
			PvPCollectedLootsCount = combat.PvPCollectedLootsCount;
			TurnTimerActivationTurn = combat.TurnTimerActivationTurn;
			CurrentTurnFlameTriggerCount = combat.CurrentTurnFlameTriggerCount;
			foreach (ActorSpawnPointModel model26 in combat.GetModels<ActorSpawnPointModel>())
			{
				ActorSpawnPointModelBackup actorSpawnPointModelBackup = ((model26 is CivilianSpawnPointModel) ? new CivilianSpawnPointModelBuckup() : ((model26 is RaiderSpawnPointModel) ? new RaiderSpawnPointModelBuckup() : ((model26 is SurvivorSpawnPointModel) ? new SurvivorSpawnPointModelBuckup() : ((!(model26 is WalkerSpawnPointModel)) ? new ActorSpawnPointModelBackup() : new WalkerSpawnPointModelBuckup()))));
				actorSpawnPointModelBackup.RecordStatus(model26);
				ActorSpawnPointModels.Add(actorSpawnPointModelBackup);
			}
			threatMeterBackup.RecordStatus(combat.ThreatMeter);
			if (combat.RedactTimedEffect != null)
			{
				RedactTimedEffect = new RedactTimedEffectBackup();
				RedactTimedEffect.RecordStatus(combat.RedactTimedEffect);
			}
			SupportNextCanUseTurn = new Dictionary<int, int>();
			SupportNextInnerCanUseTurn = new Dictionary<int, int>();
			usedCount = new Dictionary<int, int>();
			foreach (CombatSupportModel support in combat.SupportManager.Supports)
			{
				SupportNextCanUseTurn[support.SlotIndex] = support.NextUsableTurn;
				SupportNextInnerCanUseTurn[support.SlotIndex] = support.NextInnerUsableTurn;
				usedCount[support.SlotIndex] = support.usedCount;
			}
			MissionObjective = new MissionObjective(combat.CurrentMissionObjective);
			MissionStatistics.RecordStatus(combat.MissionStatistics);
			CombatFailureReason = combat.CombatFailureReason ?? "";
			foreach (EquipmentItemModel consumable in manager.Player.Equipment.Consumables)
			{
				Consumables.Add(consumable);
			}
			if (combat.ResurgenceType1Container != null)
			{
				ResurgenceType1ContainerBackup.RecordStatus(combat.ResurgenceType1Container);
			}
			if (combat.ResurgenceType2Container != null)
			{
				ResurgenceType2ContainerBackup.RecordStatus(combat.ResurgenceType2Container);
			}
			SurvivalGameModelBackups.Clear();
			foreach (SurvivalGameModel survivalGameModel in combat.SurvivalGameModelList)
			{
				SurvivalGameModelBackup survivalGameModelBackup = new SurvivalGameModelBackup();
				survivalGameModelBackup.RecordStatus(survivalGameModel);
				SurvivalGameModelBackups.Add(survivalGameModelBackup);
			}
			GuardianVowBindings = new List<GuardianVowBinding>();
			if (combat.GuardianVowBindings == null)
			{
				return;
			}
			foreach (GuardianVowBinding guardianVowBinding in combat.GuardianVowBindings)
			{
				GuardianVowBindings.Add(new GuardianVowBinding(guardianVowBinding));
			}
		}

		public void BackUp()
		{
			CombatModel combat = base.manager.Player.Combat;
			List<ActorModel> list = new List<ActorModel>();
			foreach (ActorModel actor in combat.AllActors)
			{
				if (ActorBackups.Find((ActorBackup x) => x.Actor == actor && x.Faction == actor.Faction) == null)
				{
					list.Add(actor);
				}
			}
			foreach (ActorModel item in list)
			{
				combat.UnregisterActor(item);
			}
			foreach (ActorBackup actorBackup in ActorBackups)
			{
				actorBackup.BackUp();
			}
			combat.AddCommonwealthArmorSupportTrait();
			combat.WalkerLevels = ((WalkerLevels == null) ? null : new List<int>(WalkerLevels));
			if (EndlessModeCombatModel != null)
			{
				EndlessModeCombatModel.BackUp();
			}
			combat.PersistentMissionVariableManager.variables = variables.Select((PersistentMissionVariable x) => new PersistentMissionVariable(x)).ToList();
			combat.bounsPhonePortraitTurnKilledNum = new Dictionary<int, int>(bounsPhonePortraitTurnKilledNum);
			combat.PVPKilledDefenderIndices = new List<int>(PVPKilledDefenderIndices);
			combat.CombatHUDState = CombatHUDState;
			combat.ClearDisorientedModel();
			combat.ClearModels<CombatArea>();
			CombatAreaModels.ForEach(delegate(TWDModelObject x)
			{
				x.SetManager(base.manager);
				combat.AddModel(x);
			});
			combat.GetModels<CombatAreasManager>().Except(CombatAreaManagers).ToList()
				.ForEach(delegate(TWDModelObject x)
				{
					combat.RemoveModel(x);
				});
			PitfallAreasManager model = combat.GetModel<PitfallAreasManager>();
			if (model != null)
			{
				model.ActorCooldownUntilTurns = ((PitfallAreasActorCooldownUntilTurns == null) ? new Dictionary<string, int>() : new Dictionary<string, int>(PitfallAreasActorCooldownUntilTurns));
			}
			combat.ClearModels<ActorToActorRelation>();
			ActorToActorRelationModels.ForEach(delegate(TWDModelObject x)
			{
				x.SetManager(base.manager);
				combat.AddModel(x);
			});
			combat.GetModels<ActorToActorRelationsManager>().Except(ActorToActorRelationManagers).ToList()
				.ForEach(delegate(TWDModelObject x)
				{
					combat.RemoveModel(x);
				});
			combat.DashSurvivalFlagActor = DashSurvivalFlagActor;
			combat.DashRaiderFlagActor = DashRaiderFlagActor;
			combat.DebuffQuantunRemove = DebuffQuantunRemove;
			combat.DebuffQuantunRemoveRaider = DebuffQuantunRemoveRaider;
			combat.ClearModels<FactionToActorRelation>();
			FactionToActorRelationModels.ForEach(delegate(TWDModelObject x)
			{
				x.SetManager(base.manager);
				combat.AddModel(x);
			});
			combat.GetModels<FactionToActorManager>().Except(FactionToActorManagers).ToList()
				.ForEach(delegate(TWDModelObject x)
				{
					combat.RemoveModel(x);
				});
			base.manager.ClearCollectedDelayedEvents();
			List<TWDModelObject> models = combat.GetModels<NodeGraph>();
			foreach (KeyValuePair<int, List<NodeBase>> nodeGraph2 in NodeGraphs)
			{
				if (nodeGraph2.Value.Count <= 0)
				{
					continue;
				}
				foreach (NodeGraph item2 in models)
				{
					if (item2.GuidHash == nodeGraph2.Key)
					{
						item2.Backup(nodeGraph2.Value.Select((NodeBase x) => x.RecordValue()).ToList());
						break;
					}
				}
			}
			foreach (MissionLogicModelBackup missionLogicModel in MissionLogicModels)
			{
				missionLogicModel.BackUp();
			}
			BackupReceiversNodeRefrence(combat);
			foreach (InteractiveObjectBuckup interactiveObjectBuckup in InteractiveObjectBuckups)
			{
				interactiveObjectBuckup.BackUp();
			}
			foreach (LootModelBackup lootModel in LootModels)
			{
				lootModel.BackUp();
			}
			base.manager.Player.LootManager.AvailableKeys = AvailableKeys;
			base.manager.Player.LootManager.LootKeysSources = new List<LootKeySource>(LootKeysSources);
			foreach (ExplosiveModelBackup explosiveModel in ExplosiveModels)
			{
				explosiveModel.BackUp();
			}
			foreach (CombatExitModelBackup combatExitModel in CombatExitModels)
			{
				combatExitModel.BackUp();
			}
			foreach (TriggerModelBackup triggerModel in TriggerModels)
			{
				triggerModel.BackUp();
			}
			foreach (SetMissionObjectiveModelBackup setMissionObjectiveModel in SetMissionObjectiveModels)
			{
				setMissionObjectiveModel.BackUp();
			}
			foreach (DoorModelBackup doorModel in DoorModels)
			{
				doorModel.BackUp();
			}
			foreach (CombatColliderModelBackup combatColliderModel in CombatColliderModels)
			{
				combatColliderModel.BackUp();
			}
			combat.UpdateDynamicColliders();
			foreach (CoverModelBackup coverModel in CoverModels)
			{
				coverModel.BackUp();
			}
			combat.UpdateCoverField();
			foreach (MovableModelBackup movableModel in MovableModels)
			{
				movableModel.BackUp();
			}
			combat.UpdateAllActorsVisibility();
			combat.UpdateObjectsVisibility();
			combat.UpdateOccupiers();
			combat.Variables = new Dictionary<int, int>(Variables);
			combat.PvPCollectedFlagsCount = PvPCollectedFlagsCount;
			combat.PvPCollectedLootsCount = PvPCollectedLootsCount;
			combat.TurnTimerActivationTurn = TurnTimerActivationTurn;
			combat.CurrentTurnFlameTriggerCount = CurrentTurnFlameTriggerCount;
			foreach (ActorSpawnPointModelBackup actorSpawnPointModel in ActorSpawnPointModels)
			{
				actorSpawnPointModel.BackUp();
			}
			threatMeterBackup.BackUp();
			if (RedactTimedEffect != null)
			{
				RedactTimedEffect.BackUp();
				combat.RedactTimedEffect = RedactTimedEffect.Model;
			}
			else
			{
				combat.RedactTimedEffect = null;
			}
			foreach (KeyValuePair<int, int> item3 in SupportNextCanUseTurn)
			{
				if (combat.SupportManager.TryGetSupport(item3.Key, out var combatSupportModel))
				{
					combatSupportModel.NextUsableTurn = item3.Value;
				}
			}
			foreach (KeyValuePair<int, int> item4 in SupportNextInnerCanUseTurn)
			{
				if (combat.SupportManager.TryGetSupport(item4.Key, out var combatSupportModel2))
				{
					combatSupportModel2.NextInnerUsableTurn = item4.Value;
				}
			}
			foreach (KeyValuePair<int, int> item5 in usedCount)
			{
				if (combat.SupportManager.TryGetSupport(item5.Key, out var combatSupportModel3))
				{
					combatSupportModel3.usedCount = item5.Value;
				}
			}
			combat.SupportManager.LastUsedTurn = combat.TurnManager.TurnCount - 1;
			combat.CurrentMissionObjective.Backup(MissionObjective);
			MissionStatistics.BackUp();
			combat.CombatFailureReason = CombatFailureReason ?? "";
			base.manager.Player.Equipment.Consumables.Clear();
			foreach (EquipmentItemModel consumable in Consumables)
			{
				if (consumable.ModelId == 0)
				{
					consumable.SetManager(base.Manager);
					consumable.Start();
				}
				base.manager.Player.Equipment.Consumables.Add(consumable);
			}
			base.manager.ClearCollectedDelayedEvents();
			if (ResurgenceType1ContainerBackup != null)
			{
				combat.ResurgenceType1Container.Backup(ResurgenceType1ContainerBackup);
			}
			if (ResurgenceType2ContainerBackup != null)
			{
				combat.ResurgenceType2Container.Backup(ResurgenceType2ContainerBackup);
			}
			BackupSurvivalGameModels(combat);
			combat.GuardianVowBindings.Clear();
			if (GuardianVowBindings == null)
			{
				return;
			}
			foreach (GuardianVowBinding guardianVowBinding in GuardianVowBindings)
			{
				combat.GuardianVowBindings.Add(new GuardianVowBinding(guardianVowBinding));
			}
		}

		public void BackupReceiversNodeRefrence(CombatModel combat)
		{
			foreach (InteractiveObjectModel model in combat.GetModels<InteractiveObjectModel>())
			{
				if (model.receivers == null)
				{
					continue;
				}
				List<InteractiveObjectNode> list = (from x in model.receivers.FindAll((InteractionReceiver x) => x is InteractiveObjectNode)
					select x as InteractiveObjectNode).ToList();
				model.receivers.RemoveAll((InteractionReceiver x) => x is InteractiveObjectNode);
				List<TWDModelObject> models = combat.GetModels<NodeGraph>();
				foreach (InteractiveObjectNode item in list)
				{
					foreach (NodeGraph item2 in models)
					{
						if (item2.GuidHash == item.GraphHash)
						{
							model.receivers.Add(item2.GetNode(item.GuidHash) as InteractiveObjectNode);
							break;
						}
					}
				}
			}
			foreach (TriggerModel model2 in combat.GetModels<TriggerModel>())
			{
				if (model2.receivers == null)
				{
					continue;
				}
				List<TriggerNode> list2 = (from x in model2.receivers.FindAll((TriggerReceiver x) => x is TriggerNode)
					select x as TriggerNode).ToList();
				model2.receivers.RemoveAll((TriggerReceiver x) => x is TriggerNode);
				List<TWDModelObject> models2 = combat.GetModels<NodeGraph>();
				foreach (TriggerNode item3 in list2)
				{
					foreach (NodeGraph item4 in models2)
					{
						if (item4.GuidHash == item3.GraphHash)
						{
							model2.receivers.Add(item4.GetNode(item3.GuidHash) as TriggerNode);
							break;
						}
					}
				}
			}
			foreach (MissionLogicModel model3 in combat.GetModels<MissionLogicModel>())
			{
				if (model3.Receivers == null)
				{
					continue;
				}
				List<TriggerNode> list3 = (from x in model3.Receivers.FindAll((TriggerReceiver x) => x is TriggerNode)
					select x as TriggerNode).ToList();
				model3.Receivers.RemoveAll((TriggerReceiver x) => x is TriggerNode);
				List<TWDModelObject> models3 = combat.GetModels<NodeGraph>();
				foreach (TriggerNode item5 in list3)
				{
					foreach (NodeGraph item6 in models3)
					{
						if (item6.GuidHash == item5.GraphHash)
						{
							model3.Receivers.Add(item6.GetNode(item5.GuidHash) as TriggerNode);
							break;
						}
					}
				}
			}
		}

		public List<TWDModelObject> RecordCombatArea(CombatModel combat)
		{
			List<TWDModelObject> list = new List<TWDModelObject>();
			foreach (TWDModelObject model in combat.GetModels<CombatArea>())
			{
				if (model is EmitArea)
				{
					list.Add(new EmitArea(model as EmitArea));
				}
				else if (model is PitfallArea)
				{
					list.Add(new PitfallArea(model as PitfallArea));
				}
				else if (model is SufferArea)
				{
					list.Add(new SufferArea(model as SufferArea));
				}
				else if (model is TrapFlameArea)
				{
					list.Add(new TrapFlameArea(model as TrapFlameArea));
				}
				else if (model is MagazineArea)
				{
					list.Add(new MagazineArea(model as MagazineArea));
				}
				else if (model is DelayedActionGrenadeArea)
				{
					list.Add(new DelayedActionGrenadeArea(model as DelayedActionGrenadeArea));
				}
			}
			return list;
		}

		public List<TWDModelObject> RecordCombatActorToActorRelations(CombatModel combat)
		{
			List<TWDModelObject> list = new List<TWDModelObject>();
			foreach (TWDModelObject model in combat.GetModels<ActorToActorRelation>())
			{
				if (model is PoisonRelation)
				{
					list.Add(new PoisonRelation(model as PoisonRelation));
				}
				if (model is AstheniaRelation)
				{
					list.Add(new AstheniaRelation(model as AstheniaRelation));
				}
				if (model is GrenadeFragmentDamageRelation)
				{
					list.Add(new GrenadeFragmentDamageRelation(model as GrenadeFragmentDamageRelation));
				}
			}
			return list;
		}

		public List<TWDModelObject> RecordCombatFactionToActorRelations(CombatModel combat)
		{
			List<TWDModelObject> list = new List<TWDModelObject>();
			foreach (TWDModelObject model in combat.GetModels<FactionToActorRelation>())
			{
				if (model is ElectronChargeRelation)
				{
					list.Add(new ElectronChargeRelation(model as ElectronChargeRelation));
				}
			}
			return list;
		}

		public void BackupSurvivalGameModels(CombatModel combat)
		{
			combat.ClearModels<SurvivalGameModel>();
			combat.SurvivalGameModelList.Clear();
			foreach (SurvivalGameModelBackup survivalGameModelBackup in SurvivalGameModelBackups)
			{
				SurvivalGameModel survivalGameModel = new SurvivalGameModel(survivalGameModelBackup);
				survivalGameModel.SetManager(base.manager);
				survivalGameModel.Initialize();
				combat.SurvivalGameModelList.Add(survivalGameModel);
			}
		}
	}
}
